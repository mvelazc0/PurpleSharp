using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Management;
using System.Linq;
using System.Text;

namespace PurpleSharp.Lib
{
    class NamedPipes
    {
        /*
        //based on https://stackoverflow.com/questions/49838628/named-pipe-input-output-in-c-sharp
        public static void RunServer(string npipe, string technique, string simulator, string log, bool privileged = false)
        {
            

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            Process parentprocess = Recon.GetHostProcess(privileged);
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
                    //TODO: If the SeDebug privilege is disabled, GetProcessUser will fail. Need to handle that error.
                    //loggedUser = Recon.GetProcessUser(parentprocess).Split('\\')[1];
                    loggedUser = Recon.GetProcessUser(Recon.GetExplorer()).Split('\\')[1];
                    path = "C:\\Users\\" + loggedUser + "\\Downloads\\"+ simulator;
                    payload = String.Format("{0},{1},{2}", loggedUser, parentprocess.ProcessName, parentprocess.Id);
                }
                else payload = ",,";

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
                }
                pipeServer.Disconnect(); 
            }
        }

     */

         /*
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
        */

        //Based on https://github.com/malcomvetter/NamedPipes
        //public static void RunOrchestrationService(string npipe, string technique, string simulator, string log, bool privileged = false)
        public static void RunOrchestrationService(string npipe, string technique, string simulator, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            bool running = true;
            bool privileged = false;

            string cmdline, opsec , path;
            cmdline = opsec = path =  "";
            Process parentprocess = null;


            using (var pipeServer = new NamedPipeServerStream(npipe, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message))
            {
                logger.TimestampInfo("Starting orchestrator namedpipe service...");
                while (running)
                {
                    var reader = new StreamReader(pipeServer);
                    var writer = new StreamWriter(pipeServer);
                    
                    //logger.TimestampInfo("Waiting for client connection...");
                    pipeServer.WaitForConnection();
                    //logger.TimestampInfo("Client connected!");

                    var line = reader.ReadLine();

                    logger.TimestampInfo("Received from client: " + line);

                    if (line.ToLower().Equals("syn"))
                    {
                        //logger.TimestampInfo("sending back to client: " + "SYN/ACK");
                        writer.WriteLine("SYN/ACK");
                        writer.Flush();
                    }

                    else if (line.ToLower().StartsWith("recon:"))
                    {
                        string payload = "";
                        if (line.Replace("recon:", "").Equals("privileged")) privileged = true;
                        parentprocess = Recon.GetHostProcess(privileged);
                        if (parentprocess != null && Recon.GetExplorer() != null)
                        {
                            string loggedUser = Recon.GetProcessUser(Recon.GetExplorer()).Split('\\')[1];
                            logger.TimestampInfo(String.Format("{0} Recon identified {1} logged in. Process to Spoof: {2} PID: {3}", privileged.ToString(), loggedUser, parentprocess.ProcessName, parentprocess.Id));
                            payload = String.Format("{0},{1},{2}", loggedUser, parentprocess.ProcessName, parentprocess.Id);
                            path = "C:\\Users\\" + loggedUser + "\\Downloads\\" + simulator;
                            //logger.TimestampInfo("sending back to client: " + payload);

                        }
                        else
                        {
                            payload = ",,";
                            logger.TimestampInfo("Recon did not identify any logged users");
                        }
                        writer.WriteLine(payload);
                        writer.Flush();
                    }
                    else if (line.ToLower().Equals("quit"))
                    {
                        logger.TimestampInfo("Received quit! Exitting namedpipe");
                        //logger.TimestampInfo("sending back to client: " + "quit");
                        writer.WriteLine("quit");
                        writer.Flush();
                        running = false;
                    }
                    else if (line.ToLower().StartsWith("sc:"))
                    {
                        //logger.TimestampInfo("Got shellcode from client");
                        //logger.TimestampInfo("sending back to client: " + "ACK");
                        writer.WriteLine("ACK");
                        writer.Flush();
                    }
                    else if (line.ToLower().StartsWith("params:"))
                    {
                        cmdline = line.Replace("params:", "");
                        //logger.TimestampInfo("Got params from client");
                        //logger.TimestampInfo("sending back to client: " + "ACK");
                        writer.WriteLine("ACK");
                        writer.Flush();
                    }
                    else if (line.ToLower().StartsWith("opsec:"))
                    {
                        opsec = line.Replace("opsec:", "");
                        //logger.TimestampInfo("Got opsec technique from client");
                        //logger.TimestampInfo("sending back to client: " + "ACK");
                        writer.WriteLine("ACK");
                        writer.Flush();
                    }
                    else if (line.Equals("act"))
                    {
                        logger.TimestampInfo("Received act!");
                        //logger.TimestampInfo("sending back to client: " + "ACK");
                        writer.WriteLine("ACK");
                        writer.Flush();

                        if (opsec.Equals("ppid"))
                        {
                            logger.TimestampInfo("Using Parent Process Spoofing technique for Opsec");
                            logger.TimestampInfo("Spoofing " + parentprocess.ProcessName + " PID: " + parentprocess.Id.ToString());
                            logger.TimestampInfo("Executing: " + path + " " + cmdline);
                            //Launcher.SpoofParent(pr[0].Id, path, "ChromeSetup.exe /technique " + technique);
                            //Launcher.SpoofParent(parentprocess.Id, path, simulator + " /technique " + technique);
                            Launcher.SpoofParent(parentprocess.Id, path, simulator + " "+cmdline);
                        }
                    }
                    pipeServer.Disconnect();
                }
            }
        }

        //Based on https://github.com/malcomvetter/NamedPipes
        public static string RunClient(string rhost, string domain, string ruser, string rpwd, string npipe, string request)
        {
            using (new Impersonation(domain, ruser, rpwd))
            {
                using (var pipeClient = new NamedPipeClientStream(rhost, npipe, PipeDirection.InOut))
                {
                    pipeClient.Connect(5000);
                    pipeClient.ReadMode = PipeTransmissionMode.Message;

                    var reader = new StreamReader(pipeClient);
                    var writer = new StreamWriter(pipeClient);
                    writer.WriteLine(request);
                    writer.Flush();
                    var result = reader.ReadLine();
                    return (result.ToString());
                }
            }
        }
    }
}
