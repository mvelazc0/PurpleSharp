using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Management;


namespace PurpleSharp.Lib
{
    class NamedPipes
    {

        //based on https://stackoverflow.com/questions/49838628/named-pipe-input-output-in-c-sharp
        public static void RunServer(string npipe, string technique, string simulator, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);

            Process parentprocess = Recon.GetHostProcess();
            //Process[] pr = Process.GetProcessesByName("explorer");
            //string loggedUser = Recon.GetProcessOwner(pr[0].Id).Split('\\')[1];
            string path = "";
            string loggedUser = "";
            

            var pipeServer = new NamedPipeServerStream(npipe, PipeDirection.InOut);
            var reader = new StreamReader(pipeServer);
            var writer = new StreamWriter(pipeServer);
            var running = true;

            while (running)
            {
                logger.TimestampInfo("Server is waiting for a client");
                pipeServer.WaitForConnection();
                logger.TimestampInfo("Server has connection from client");
                string payload = "";
                //string payload = String.Format("{0},{1},{2}",loggedUser, pr[0].ProcessName,pr[0].Id);
                if (parentprocess != null)
                {
                    //loggedUser = Recon.GetProcessOwner(parentprocess.Id).Split('\\')[1];
                    loggedUser = Recon.GetProcessUser(parentprocess).Split('\\')[1];
                    //path = "C:\\Users\\" + loggedUser + "\\Downloads\\ChromeSetup.exe";
                    path = "C:\\Users\\" + loggedUser + "\\Downloads\\"+ simulator;
                    payload = String.Format("{0},{1},{2}", loggedUser, parentprocess.ProcessName, parentprocess.Id);
                }
                else payload = ",,";

                //string payload =String.Format("{0},{1},{2}", loggedUser, parentprocess.ProcessName, parentprocess.Id);
                writer.WriteLine(payload);
                writer.Flush();
                pipeServer.WaitForPipeDrain();
                var message = reader.ReadLine();
                logger.TimestampInfo("Server: Recieved message from client: "+ message);
                if (message.Equals("quit"))
                {
                    logger.TimestampInfo("Exitting named pipe servlce");
                    writer.WriteLine("quit");
                    running = false;
                    if (parentprocess != null)
                    {
                        //logger.TimestampInfo("Spoofing explorer.exe. PID: " + pr[0].Id.ToString());
                        logger.TimestampInfo("Spoofing " + parentprocess.ProcessName + " PID: " + parentprocess.Id.ToString());
                        logger.TimestampInfo("Executing: " + path + " /technique " + technique);
                        //Launcher.SpoofParent(pr[0].Id, path, "ChromeSetup.exe /technique " + technique);
                        Launcher.SpoofParent(parentprocess.Id, path, simulator + " /technique " + technique);
                    }
                    else logger.TimestampInfo("Did not find a candidate process. Exitting");

                    //Launcher.SpoofParent(winlogon[0].Id, path, "ChromeSetup.exe /technique " + technique);

                }
                pipeServer.Disconnect(); 
            }
        }

        public static string RunClient(string rhost, string domain, string ruser, string rpwd, string npipe, bool quit = false)
        {
            string msg = "ACK";
            if (quit) msg = "quit";

            using (new Impersonation(domain, ruser, rpwd))
            {

                using (var pipeClient = new NamedPipeClientStream(rhost, npipe, PipeDirection.InOut))
                {
                    //Console.WriteLine("Client is waiting to connect");
                    if (!pipeClient.IsConnected) { pipeClient.Connect(); }
                    //Console.WriteLine("[!] Client is connected");

                    using (var reader = new StreamReader(pipeClient))
                    {
                        using (var writer = new StreamWriter(pipeClient))
                        {
                            //Console.WriteLine("Client sending msg: "+msg);
                            writer.WriteLine(msg);
                            //Console.WriteLine("Client is waiting for input");
                            var message = reader.ReadLine();
                            if (message != null)
                            {
                                //Console.WriteLine("[!] Obtained info from named pipe server");
                                //Console.WriteLine("Client: Recieved from server {0}", message);
                                return message;
                            }

                        }
                        return "";
                    }                    
                }
            }
        }

    }
}
