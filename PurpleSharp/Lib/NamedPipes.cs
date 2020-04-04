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

        //Based on https://github.com/malcomvetter/NamedPipes
        public static void RunOrchestrationService(string npipe, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            bool running = true;
            bool privileged = false;

            string cmdline, opsec, simpath, simbin, loggeduser;
            cmdline = opsec = simpath = simbin = loggeduser = "";
            Process parentprocess = null;

            try
            {


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
                                //loggeduser = Recon.GetProcessOwner(Recon.GetExplorer()).Split('\\')[1];
                                loggeduser = Recon.GetProcessOwnerWmi(Recon.GetExplorer()).Split('\\')[1];
                                logger.TimestampInfo(String.Format("{0} Recon identified {1} logged in. Process to Spoof: {2} PID: {3}", privileged.ToString(), loggeduser, parentprocess.ProcessName, parentprocess.Id));
                                payload = String.Format("{0},{1},{2}", loggeduser, parentprocess.ProcessName, parentprocess.Id);
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
                        else if (line.ToLower().StartsWith("simbin:"))
                        {
                            simbin = line.Replace("simbin:", "");
                            //logger.TimestampInfo("Got opsec technique from client");
                            //logger.TimestampInfo("sending back to client: " + "ACK");
                            simpath = "C:\\Users\\" + loggeduser + "\\Downloads\\" + simbin;
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
                                logger.TimestampInfo("Executing: " + simpath + " " + cmdline);
                                Launcher.SpoofParent(parentprocess.Id, simpath, simbin + " " + cmdline);
                            }
                        }
                        else if (line.ToLower().Equals("quit"))
                        {
                            logger.TimestampInfo("Received quit! Exitting namedpipe");
                            //logger.TimestampInfo("sending back to client: " + "quit");
                            writer.WriteLine("quit");
                            writer.Flush();
                            running = false;
                        }
                        pipeServer.Disconnect();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.TimestampInfo(ex.ToString());
                logger.TimestampInfo(ex.Message.ToString());
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
