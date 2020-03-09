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
        public static void RunServer(string npipe, string technique)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + "PurpleSharp.txt");

            Process[] pr = Process.GetProcessesByName("explorer");
            Process[] winlogon = Process.GetProcessesByName("winlogon");
            string loggedUser = GetProcessOwner(pr[0].Id).Split('\\')[1];

            string path = "C:\\Users\\" + loggedUser + "\\Downloads\\ChromeSetup.exe";

            var pipeServer = new NamedPipeServerStream(npipe, PipeDirection.InOut);
            var reader = new StreamReader(pipeServer);
            var writer = new StreamWriter(pipeServer);
            var running = true;

            while (running)
            {
                logger.TimestampInfo("Server is waiting for a client");
                pipeServer.WaitForConnection();
                logger.TimestampInfo("Server has connection from client");
                writer.WriteLine(loggedUser);
                writer.Flush();
                pipeServer.WaitForPipeDrain();
                var message = reader.ReadLine();
                Console.WriteLine("Server: Recieved from client {0}", message);
                logger.TimestampInfo("Server: Recieved message from client: "+ message);
                if (message.Equals("quit"))
                {
                    logger.TimestampInfo("Exitting named pipe servlce");
                    writer.WriteLine("quit");
                    running = false;

                    
                    
                    logger.TimestampInfo("Spoofing explorer.exe. PID: " + pr[0].Id.ToString());
                    logger.TimestampInfo("Executing: " + path + " /technique " + technique);

                    Launcher.SpoofParent(pr[0].Id, path, "ChromeSetup.exe /technique " + technique);
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
                    Console.WriteLine("[!] Client is connected");

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
                                Console.WriteLine("[!] Obtained info from named pipe server");
                                //Console.WriteLine("Client: Recieved from server {0}", message);
                                return message;
                            }

                        }
                        return "";
                    }                    
                }
            }
        }

        //https://codereview.stackexchange.com/questions/68076/user-logged-onto-windows
        public static string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return argList[1] + "\\" + argList[0];
                }
            }

            return "NO OWNER";
        }
    }
}
