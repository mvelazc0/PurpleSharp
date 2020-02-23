using System;
using System.Collections.Generic;

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
            if (rhost != "")
            {
                ExecuteRemote(rhost, domain, ruser, rpwd, technique);

            }
            else 
            {
                ExecuteTechnique(technique, type, usertype, nusers, hosttype, nhosts, protocol, sleep, pwd, command, cleanup);
            }
            
            
        }

        public static void ExecuteRemote(string rhost, string domain, string ruser, string rpwd, string technique)
        {
            string uploadPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string executionPath = "C:\\Windows\\Temp\\PurpleSharp.exe";
            Lib.RemoteLauncher.upload(uploadPath, executionPath, rhost, ruser, rpwd, domain);
            System.Threading.Thread.Sleep(3000);
            string cmdline = "/technique "+ technique;
            Lib.RemoteLauncher.wmiexec(rhost, executionPath, cmdline, domain, ruser, rpwd);
            System.Threading.Thread.Sleep(3000);
            Lib.RemoteLauncher.delete(executionPath, rhost, ruser, rpwd, domain);


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

                default:
                    break;

            }
        }

    }

}
