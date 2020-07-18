using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Management;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Security.AccessControl;

namespace PurpleSharp.Lib
{
    class NamedPipes
    {

        //Based on https://github.com/malcomvetter/NamedPipes
        public static void RunScoutService(string scout_np, string simulator_np, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            bool running = true;
            bool privileged = false;

            string technique, opsec, simpfath, simrpath, duser, user, simbinary, cleanup;
            technique = opsec = simpfath = simrpath = duser = user = simbinary = cleanup = "";
            Process parentprocess = null;
            int pbsleep, tsleep;
            pbsleep = tsleep = 0;
            System.Threading.Thread.Sleep(1500);

            try
            {
                using (var pipeServer = new NamedPipeServerStream(scout_np, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message))
                {
                    logger.TimestampInfo("Starting scout namedpipe service with PID:"+ Process.GetCurrentProcess().Id);
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
                        else if (line.ToLower().Equals("auditpol"))
                        {
                            writer.WriteLine(System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(Recon.GetAuditPolicy())));
                            writer.Flush();
                        }
                        else if (line.ToLower().Equals("wef"))
                        {
                            writer.WriteLine(System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(Recon.GetWefSettings())));
                            writer.Flush();
                        }
                        else if (line.ToLower().Equals("pws"))
                        {
                            writer.WriteLine(System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(Recon.GetPwsLoggingSettings())));
                            writer.Flush();
                        }
                        else if (line.ToLower().Equals("ps"))
                        {
                            writer.WriteLine(System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(Recon.GetProcs())));
                            writer.Flush();
                        }
                        else if (line.ToLower().Equals("svcs"))
                        {
                            writer.WriteLine(System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(Recon.GetServices())));
                            writer.Flush();
                        }
                        else if (line.ToLower().Equals("cmdline"))
                        {
                            writer.WriteLine(System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(Recon.GetCmdlineAudittingSettings())));
                            writer.Flush();
                        }

                        else if (line.ToLower().StartsWith("recon:"))
                        {
                            string payload = "";
                            if (line.Replace("recon:", "").Equals("privileged")) privileged = true;
                            parentprocess = Recon.GetHostProcess(privileged);
                            if (parentprocess != null && Recon.GetExplorer() != null)
                            {
                                duser = Recon.GetProcessOwnerWmi(Recon.GetExplorer());
                                user = duser.Split('\\')[1];
                                logger.TimestampInfo(String.Format("Recon identified {0} logged in. Process to Spoof: {1} PID: {2}", duser, parentprocess.ProcessName, parentprocess.Id));
                                payload = String.Format("{0},{1},{2},{3}", duser, parentprocess.ProcessName, parentprocess.Id, privileged.ToString());

                            }
                            else
                            {
                                payload = ",,,";
                                logger.TimestampInfo("Recon did not identify any logged users");
                            }
                            writer.WriteLine(payload);
                            writer.Flush();
                        }
                        else if (line.ToLower().StartsWith("sc:"))
                        {
                            writer.WriteLine("ACK");
                            writer.Flush();
                        }
                        else if (line.ToLower().StartsWith("technique:"))
                        {
                            technique = line.Replace("technique:", "");
                            //logger.TimestampInfo("Got params from client");
                            //logger.TimestampInfo("sending back to client: " + "ACK");
                            writer.WriteLine("ACK");
                            writer.Flush();
                        }
                        else if (line.ToLower().StartsWith("pbsleep:"))
                        {
                            pbsleep = Int32.Parse(line.Replace("pbsleep:", ""));
                            //logger.TimestampInfo("Got params from client");
                            //logger.TimestampInfo("sending back to client: " + "ACK");
                            writer.WriteLine("ACK");
                            writer.Flush();
                        }
                        else if (line.ToLower().StartsWith("tsleep:"))
                        {
                            tsleep = Int32.Parse(line.Replace("tsleep:", ""));
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
                        else if (line.ToLower().StartsWith("cleanup:"))
                        {
                            cleanup = line.Replace("cleanup:", "");
                            writer.WriteLine("ACK");
                            writer.Flush();
                        }
                        else if (line.ToLower().StartsWith("simrpath:"))
                        {
                            simrpath = line.Replace("simrpath:", "");
                            //logger.TimestampInfo("sending back to client: " + "ACK");
                            //simpath = "C:\\Users\\" + loggeduser + "\\Downloads\\" + simbin;
                            simpfath = "C:\\Users\\" + user + "\\" + simrpath;
                            int index = simrpath.LastIndexOf(@"\");
                            simbinary = simrpath.Substring(index + 1);

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
                                logger.TimestampInfo("Executing: " + simpfath + " /n");
                                //Launcher.SpoofParent(parentprocess.Id, simpath, simbin + " " + cmdline);
                                //Launcher.SpoofParent(parentprocess.Id, simpfath, simrpath + " /s");

                                Launcher.SpoofParent(parentprocess.Id, simpfath, simbinary + " /n");
                                //Launcher.SpoofParent(parentprocess.Id, simpfath, simbinary + " /s");

                                System.Threading.Thread.Sleep(3000);
                                logger.TimestampInfo("Sending payload to Scout Aggent through namedpipe: " + "technique:" + technique + " pbsleep:" + pbsleep.ToString() + " tsleep:" + tsleep.ToString() + " cleanup:" + cleanup);
                                RunNoAuthClient(simulator_np, "technique:" + technique + " pbsleep:" + pbsleep.ToString() + " tsleep:"+tsleep.ToString() + " cleanup:" + cleanup);
                                System.Threading.Thread.Sleep(2000);
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
        public static string[] RunSimulationService(string npipe, string log)
        {
            string[] result = new string[4];
            try
            {
                //https://helperbyte.com/questions/171742/how-to-connect-to-a-named-pipe-without-administrator-rights
                PipeSecurity ps = new PipeSecurity();
                ps.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));

                //logger.TimestampInfo("starting!");
                string technique, pbsleep, tsleep, cleanup;
                using (var pipeServer = new NamedPipeServerStream(npipe, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 4028, 4028, ps))
                
                {
                    var reader = new StreamReader(pipeServer);
                    var writer = new StreamWriter(pipeServer);

                    pipeServer.WaitForConnection();
                    var line = reader.ReadLine();
                
                    if (line.ToLower().StartsWith("technique:"))
                    {
                        string[] options = line.Split(' ');
                        technique = options[0].Replace("technique:", "");
                        pbsleep = options[1].Replace("pbsleep:", "");
                        tsleep = options[2].Replace("tsleep:", "");
                        cleanup = options[3].Replace("cleanup:", "");
                        writer.WriteLine("ACK");
                        writer.Flush();

                        result[0] = technique;
                        result[1] = pbsleep;
                        result[2] = tsleep;
                        result[3] = cleanup;
                        return result;
                    }
                    pipeServer.Disconnect();
                }
                return result;
            }
            catch
            {
                return result;
            }

        }

        //Based on https://github.com/malcomvetter/NamedPipes
        public static string RunClient(string rhost, string domain, string ruser, string rpwd, string npipe, string request)
        {
            using (new Impersonation(domain, ruser, rpwd))
            {
                using (var pipeClient = new NamedPipeClientStream(rhost, npipe, PipeDirection.InOut))
                {
                    pipeClient.Connect(100000);
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

        public static string RunNoAuthClient(string npipe, string request)
        {
            using (var pipeClient = new NamedPipeClientStream(".", npipe, PipeDirection.InOut))
            {
                pipeClient.Connect(10000);
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
