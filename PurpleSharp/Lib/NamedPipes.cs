using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Management;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Security.AccessControl;
using Newtonsoft.Json;
using System.Threading;

namespace PurpleSharp.Lib
{
    class NamedPipes
    {
        public static void RunScoutServiceSerialized(string scout_np, string simulator_np, string log)
        {
            //Based on https://github.com/malcomvetter/NamedPipes

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            bool running = true;
            bool privileged = false;

            string duser, simulator_full_path, user, simulator_binary;
            duser = simulator_full_path = user = simulator_binary = "";
            Process parentprocess = null;
            SimulationPlaybook PlaybookForSimulator = null;
            Thread.Sleep(1500);

            try
            {
                using (var pipeServer = new NamedPipeServerStream(scout_np, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message))
                {

                    logger.TimestampInfo("Starting scout namedpipe service with PID:" + Process.GetCurrentProcess().Id);
                    while (running)
                    {
                        SimulationResponse sim_response;
                        logger.TimestampInfo("Waiting for client connection...");
                        pipeServer.WaitForConnection();
                        logger.TimestampInfo("Client connected.");
                        var messageBytes = ReadMessage(pipeServer);
                        var line = Encoding.UTF8.GetString(messageBytes);
                        logger.TimestampInfo("Received from client: " + line);
                        SimulationRequest sim_request = JsonConvert.DeserializeObject<SimulationRequest>(line);
                        ScoutResponse scout_response = null;

                        // Scout recon actions
                        if (sim_request.header.Equals("SCT"))
                        {
                            logger.TimestampInfo("Received SCT");
                            switch (sim_request.recon_type)
                            {
                                case "auditpol":
                                    scout_response = new ScoutResponse(Convert.ToBase64String(Encoding.ASCII.GetBytes(Recon.GetAuditPolicy())));
                                    break;
                                case "wef":
                                    scout_response = new ScoutResponse(Convert.ToBase64String(Encoding.ASCII.GetBytes(Recon.GetWefSettings())));
                                    break;
                                case "pws":
                                    scout_response = new ScoutResponse(Convert.ToBase64String(Encoding.ASCII.GetBytes(Recon.GetPwsLoggingSettings())));
                                    break;
                                case "ps":
                                    scout_response = new ScoutResponse(Convert.ToBase64String(Encoding.ASCII.GetBytes(Recon.GetProcs())));
                                    break;
                                case "svcs":
                                    scout_response = new ScoutResponse(Convert.ToBase64String(Encoding.ASCII.GetBytes(Recon.GetServices())));
                                    break;
                                case "cmdline":
                                    scout_response = new ScoutResponse(Convert.ToBase64String(Encoding.ASCII.GetBytes(Recon.GetCmdlineAudittingSettings())));
                                    break;
                                case "all":
                                    string results = Recon.GetAuditPolicy() + "\n" + Recon.GetWefSettings() + "\n" + Recon.GetPwsLoggingSettings()+"\n"+ Recon.GetProcs()+"\n"+ Recon.GetServices()+"\n"+ Recon.GetCmdlineAudittingSettings();
                                    scout_response = new ScoutResponse(Convert.ToBase64String(Encoding.ASCII.GetBytes(results)));
                                    break;
                                default:
                                    break;
                            }
                            sim_response = new SimulationResponse("SYN/ACK", null, scout_response);
                            byte[] bytes_sim_response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sim_response));
                            pipeServer.Write(bytes_sim_response, 0, bytes_sim_response.Length);
                            logger.TimestampInfo(String.Format("Sent SimulationResponse object"));
                            running = false;
                            
                        }

                        else if (sim_request.header.Equals("SYN"))
                        {
                            logger.TimestampInfo("Received SYN");
                            PlaybookForSimulator = sim_request.playbook;
                            ReconResponse recon_response;
                            if (sim_request.recon_type.Equals("privileged")) privileged = true;
                            parentprocess = Recon.GetHostProcess(privileged);
                            if (parentprocess != null && Recon.GetExplorer() != null)
                            {
                                duser = Recon.GetProcessOwnerWmi(Recon.GetExplorer());
                                recon_response = new ReconResponse(duser, parentprocess.ProcessName, parentprocess.Id.ToString(), privileged.ToString());
                                user = duser.Split('\\')[1];
                                logger.TimestampInfo(String.Format("Recon identified {0} logged in. Process to Spoof: {1} PID: {2}", duser, parentprocess.ProcessName, parentprocess.Id));
                            }
                            else
                            {
                                recon_response = new ReconResponse("", "", "", "");
                                logger.TimestampInfo("Recon did not identify any logged users");
                            }

                            simulator_full_path = "C:\\Users\\" + user + "\\" + sim_request.playbook.simulator_relative_path;
                            int index = sim_request.playbook.simulator_relative_path.LastIndexOf(@"\");
                            simulator_binary = sim_request.playbook.simulator_relative_path.Substring(index + 1);

                            sim_response = new SimulationResponse("SYN/ACK", recon_response);
                            byte[] bytes_sim_response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sim_response));
                            pipeServer.Write(bytes_sim_response, 0, bytes_sim_response.Length);
                            logger.TimestampInfo(String.Format("Sent SimulationResponse object"));

                        }
                        else if (sim_request.header.Equals("ACT"))
                        {
                            logger.TimestampInfo("Received ACT");
                            if (PlaybookForSimulator.opsec.Equals("ppid"))
                            {

                                logger.TimestampInfo("Using Parent Process Spoofing technique for Opsec");
                                logger.TimestampInfo("Spoofing " + parentprocess.ProcessName + " PID: " + parentprocess.Id.ToString());
                                logger.TimestampInfo("Executing: " + simulator_full_path + " /n");
                                //Launcher.SpoofParent(parentprocess.Id, simpath, simbin + " " + cmdline);
                                //Launcher.SpoofParent(parentprocess.Id, simpfath, simrpath + " /s");

                                Launcher.SpoofParent(parentprocess.Id, simulator_full_path, simulator_binary + " /n");
                                //Launcher.SpoofParent(parentprocess.Id, simpfath, simbinary + " /s");

                                //logger.TimestampInfo("Sending payload to Simulation Agent through namedpipe: " + "technique:" + s_payload.techniques + " pbsleep:" + s_payload.playbook_sleep + " tsleep:" + s_payload.task_sleep + " cleanup:" + s_payload.cleanup);
                                logger.TimestampInfo("Sending Simulation Playbook to Simulation Agent through namedpipe: " + PlaybookForSimulator.simulator_relative_path);

                                byte[] bytes_sim_rqeuest = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new SimulationRequest("ACK", "", PlaybookForSimulator)));
                                string result = NamedPipes.RunNoAuthClientSerialized(simulator_np, bytes_sim_rqeuest);
                                logger.TimestampInfo("Received back from Simulator " + result);
                            }

                            sim_response = new SimulationResponse("ACK");
                            byte[] bytes_sim_response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sim_response));
                            pipeServer.Write(bytes_sim_response, 0, bytes_sim_response.Length);
                            logger.TimestampInfo(String.Format("Sent SimulationResponse object 2"));
                            running = false;

                        }
                        else if (sim_request.header.Equals("FIN"))
                        {
                            logger.TimestampInfo("Received a FIN command");
                            sim_response = new SimulationResponse("ACK");
                            byte[] bytes_sim_response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sim_response));
                            pipeServer.Write(bytes_sim_response, 0, bytes_sim_response.Length);
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
        private static byte[] ReadMessage(PipeStream pipe)
        {
            byte[] buffer = new byte[1024];
            using (var ms = new MemoryStream())
            {
                do
                {
                    var readBytes = pipe.Read(buffer, 0, buffer.Length);
                    ms.Write(buffer, 0, readBytes);
                }
                while (!pipe.IsMessageComplete);

                return ms.ToArray();
            }
        }
        public static SimulationPlaybook RunSimulationServiceSerialized(string npipe, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
    
            SimulationPlaybook playbook = new SimulationPlaybook();
            try
            {
                //https://helperbyte.com/questions/171742/how-to-connect-to-a-named-pipe-without-administrator-rights
                PipeSecurity ps = new PipeSecurity();
                ps.SetAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));

                logger.TimestampInfo("starting Simulator!");
                using (var pipeServer = new NamedPipeServerStream(npipe, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 4028, 4028, ps))

                {
                    SimulationResponse sim_response;
                    logger.TimestampInfo("Waiting for client connection...");
                    pipeServer.WaitForConnection();
                    logger.TimestampInfo("Client connected.");
                    var messageBytes = ReadMessage(pipeServer);
                    var line = Encoding.UTF8.GetString(messageBytes);
                    logger.TimestampInfo("Received from client: " + line);
                    SimulationRequest sim_request = JsonConvert.DeserializeObject<SimulationRequest>(line);

                    playbook = sim_request.playbook;
                    sim_response = new SimulationResponse("ACK");
                    byte[] bytes_sim_response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(sim_response));
                    pipeServer.Write(bytes_sim_response, 0, bytes_sim_response.Length);
                    logger.TimestampInfo("Replied to client: " + Encoding.UTF8.GetString(bytes_sim_response));
                    pipeServer.Disconnect();
                    return playbook;
                }
            }
            catch(Exception ex)
            {
                logger.TimestampInfo(ex.Message);
                return playbook;
            }

        }
        public static string RunClientSerialized(string rhost, string domain, string ruser, string rpwd, string npipe, byte[] serialized_object)
        {
            //Based on https://github.com/malcomvetter/NamedPipes

            using (new Impersonation(domain, ruser, rpwd))
            {
                using (var pipeClient = new NamedPipeClientStream(rhost, npipe, PipeDirection.InOut))
                {
                    pipeClient.Connect(100000);
                    pipeClient.ReadMode = PipeTransmissionMode.Message;

                    var reader = new StreamReader(pipeClient);
                    //var writer = new StreamWriter(pipeClient);
                    var writer2 = new BinaryWriter(pipeClient);
                    writer2.Write(serialized_object);
                    var restul1 = reader.ReadLine();
                    return (restul1.ToString());
                }
            }
        }
        public static string RunNoAuthClientSerialized(string npipe, byte[] serialized_object)
        {
            using (var pipeClient = new NamedPipeClientStream(".", npipe, PipeDirection.InOut))
            {
                pipeClient.Connect(10000);
                pipeClient.ReadMode = PipeTransmissionMode.Message;

                var reader = new StreamReader(pipeClient);
                var writer2 = new BinaryWriter(pipeClient);
                writer2.Write(serialized_object);
                //var writer = new StreamWriter(pipeClient);
                //writer.WriteLine(request);
                //writer.Flush();
                var result = reader.ReadLine();
                return (result.ToString());
            }
        }
    }
}
