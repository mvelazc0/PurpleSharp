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

        //Based on https://github.com/malcomvetter/NamedPipes
        /*
        public static void RunServer_test(string npipe, string technique, string simulator, string log, bool privileged = false)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            bool running = true;
            using (var pipe = new NamedPipeServerStream(npipe, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message))
            {
                while (running)
                {
                    logger.TimestampInfo("[*] Waiting for client connection...");
                    pipe.WaitForConnection();
                    logger.TimestampInfo("[*] Client connected!");
                    var messageBytes = ReadMessage(pipe);
                    var line = Encoding.UTF8.GetString(messageBytes);
                    logger.TimestampInfo("[*] Received from client: " + line);

                    if (line.ToLower().Equals("syn"))
                    {
                        var response = Encoding.UTF8.GetBytes("ACK");
                        logger.TimestampInfo("[*] sending back to client: " + "SYN/ACK");
                        pipe.Write(response, 0, response.Length);
                    }
                    else if (line.ToLower().Equals("recon"))
                    {
                        Process parentprocess = Recon.GetHostProcess(privileged);
                        if (parentprocess != null)
                        {
                            string loggedUser = Recon.GetProcessUser(Recon.GetExplorer()).Split('\\')[1];
                            string payload = String.Format("{0},{1},{2}", loggedUser, parentprocess.ProcessName, parentprocess.Id);
                            var recon = Encoding.UTF8.GetBytes(payload);
                            logger.TimestampInfo("[*] sending back to client: " + payload);
                            pipe.Write(recon, 0, recon.Length);
                        }

                    }
                    else if (line.ToLower().Equals("quit"))
                    {
                        logger.TimestampInfo("[*] Received quit! Exitting namedpipe");
                        var quit = Encoding.UTF8.GetBytes("quit");
                        logger.TimestampInfo("[*] sending back to client: " + "quit");
                        pipe.Write(quit, 0, quit.Length);
                        running = false;
                    }
                    else if (line.ToLower().StartsWith("sc:"))
                    {
                        logger.TimestampInfo("[*] Got shellcode from client");
                        var quit = Encoding.UTF8.GetBytes("ACK");
                        logger.TimestampInfo("[*] sending back to client: " + "ACK");
                        pipe.Write(quit, 0, quit.Length);


                    }

                    pipe.Disconnect();
                }

            }

        }
        */

        //Based on https://github.com/malcomvetter/NamedPipes
        public static void RunServer2(string npipe, string technique, string simulator, string log, bool privileged = false)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            bool running = true;
            using (var pipeServer = new NamedPipeServerStream(npipe, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message))
            {
                while (running)
                {
                    var reader = new StreamReader(pipeServer);
                    var writer = new StreamWriter(pipeServer);
                    logger.TimestampInfo("Waiting for client connection...");
                    pipeServer.WaitForConnection();
                    logger.TimestampInfo("Client connected!");

                    var line = reader.ReadLine();

                    logger.TimestampInfo("Received from client: " + line);

                    if (line.ToLower().Equals("syn"))
                    {
                        logger.TimestampInfo("sending back to client: " + "SYN/ACK");
                        writer.WriteLine("SYN/ACK");
                        writer.Flush();
                    }
                    else if (line.ToLower().Equals("recon"))
                    {
                        Process parentprocess = Recon.GetHostProcess(privileged);
                        if (parentprocess != null)
                        {
                            string loggedUser = Recon.GetProcessUser(Recon.GetExplorer()).Split('\\')[1];
                            string payload = String.Format("{0},{1},{2}", loggedUser, parentprocess.ProcessName, parentprocess.Id);
                            logger.TimestampInfo("sending back to client: " + payload);
                            writer.WriteLine(payload);
                            writer.Flush();
                        }
                    }
                    else if (line.ToLower().Equals("quit"))
                    {
                        logger.TimestampInfo("Received quit! Exitting namedpipe");
                        logger.TimestampInfo("sending back to client: " + "quit");
                        writer.WriteLine("quit");
                        writer.Flush();
                        running = false;
                    }
                    else if (line.ToLower().StartsWith("sc:"))
                    {
                        logger.TimestampInfo("Got shellcode from client");
                        logger.TimestampInfo("sending back to client: " + "ACK");
                        writer.WriteLine("ACK");
                        writer.Flush();
                    }
                    pipeServer.Disconnect();
                }

            }

        }

        /*
        public static string RunClient_test(string rhost, string domain, string ruser, string rpwd, string npipe, string request)
        {
            using (new Impersonation(domain, ruser, rpwd))
            {
                using (var pipe = new NamedPipeClientStream(rhost, npipe, PipeDirection.InOut))
                {
                    pipe.Connect(5000);
                    pipe.ReadMode = PipeTransmissionMode.Message;
                    byte[] bytes = Encoding.Default.GetBytes(request);
                    pipe.Write(bytes, 0, bytes.Length);
                    //if (input.ToLower() == "exit") return;
                    var result = ReadMessage(pipe);
                    //Console.WriteLine(Encoding.UTF8.GetString(result));
                    //Console.WriteLine();
                    return (Encoding.UTF8.GetString(result));

                }
            }
        }
        */

        //Based on https://github.com/malcomvetter/NamedPipes
        public static string RunClient2(string rhost, string domain, string ruser, string rpwd, string npipe, string request)
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
