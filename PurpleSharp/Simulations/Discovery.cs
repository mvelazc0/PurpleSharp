using PurpleSharp.Lib;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class Discovery
    {
        public static void EnumerateShares(int nhosts, int tsleep, string log)
        {

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1135");
            logger.TimestampInfo("Using the Win32 API NetShareEnum function to execute this technique");

            try
            {
                List<Task> tasklist = new List<Task>();
                var rand = new Random();
                int computertype = rand.Next(1, 6);

                List<Computer> targetcomputers = Lib.Targets.GetHostTargets_old(computertype, nhosts, logger);
                logger.TimestampInfo(String.Format("Obtained {0} target computers", targetcomputers.Count));
                if (tsleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between each enumeration attempt", tsleep));
                foreach (Computer computer in targetcomputers)
                {
                    if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            DiscoveryHelper.ShareEnum(computer, logger);

                        }));
                        if (tsleep > 0) Thread.Sleep(tsleep * 1000);
                    }

                }
                Task.WaitAll(tasklist.ToArray());
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void PrivilegeEnumeration(int nhost, int sleep, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T0XXX");
            logger.TimestampInfo("Using the System.Management .NET API to execute this technique");

            List<Task> tasklist = new List<Task>();
            var rand = new Random();
            int computertype = rand.Next(1, 6);

            List<Computer> targetcomputers = Lib.Targets.GetHostTargets_old(computertype, nhost, logger);
            Console.WriteLine("[*] Starting Find local administrator from {0} as {1}", Environment.MachineName, WindowsIdentity.GetCurrent().Name);
            if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between enumeration", sleep);
            foreach (Computer computer in targetcomputers)
            {
                if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                {
                    tasklist.Add(Task.Factory.StartNew(() =>
                    {
                        Simulations.DiscoveryHelper.FindLocalAdminAccess(computer);
                        if (sleep > 0) Thread.Sleep(sleep * 1000);
                    }));
                }
            }
            Task.WaitAll(tasklist.ToArray());
        }
        public static void NetworkServiceDiscovery(int nhost, int tsleep, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1046");
            logger.TimestampInfo("Using the System.Net.Sockets .NET namespace to execute this technique");

            try
            {
                var rand = new Random();
                int computertype = rand.Next(1, 6);
                List<Task> tasklist = new List<Task>();
                List<Computer> targetcomputers = Lib.Targets.GetHostTargets_old(computertype, nhost, logger);
                logger.TimestampInfo(String.Format("Obtained {0} target computers for the scan", targetcomputers.Count));
                if (tsleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between each network scan", tsleep));
                foreach (Computer computer in targetcomputers)
                {
                    if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        Computer temp = computer;
                        TimeSpan interval = TimeSpan.FromSeconds(5);

                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            logger.TimestampInfo(String.Format("Starting port scan against {0} ({1})", temp.ComputerName, temp.IPv4));
                            DiscoveryHelper.PortScan(temp, interval);

                        }));
                        if (tsleep > 0) Thread.Sleep(tsleep * 1000);

                    }
                }
                Task.WaitAll(tasklist.ToArray());
                logger.SimulationFinished();

            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        public static void DomainAccountDiscoveryLdap(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1087.002");
            logger.TimestampInfo("Using LDAP to execute this technique");
            try
            {
                DiscoveryHelper.LdapQueryForObjects(logger, 1);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }   
        }

        public static void DomainAccountDiscoveryCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1087.002");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcess("", "net user /domain", logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }   
        }

        public static void LocalAccountDiscoveryCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1087.001");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                //ExecutionHelper.StartProcess("", "net group \"Domain Admins\" /domain", log);
                ExecutionHelper.StartProcess("", "net user", logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void SystemServiceDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1007");
            try
            {
                ExecutionHelper.StartProcess("", "net start", logger);
                ExecutionHelper.StartProcess("", "tasklist /svc", logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        public static void SystemUserDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1033");
            try
            {
                ExecutionHelper.StartProcess("", "whoami", logger);
                ExecutionHelper.StartProcess("", "query user", logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
            
        }

        public static void SystemNetworkConnectionsDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1049");
            try
            {
                ExecutionHelper.StartProcess("", "netstat", logger);
                ExecutionHelper.StartProcess("", "net use", logger);
                ExecutionHelper.StartProcess("", "net session", logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        static public void SystemNetworkConfigurationDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1016");
            try
            {
                ExecutionHelper.StartProcess("", "ipconfig /all", logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        static public void FileAndDirectoryDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1083");

            try
            {
                ExecutionHelper.StartProcess("", @"dir c:\ >> %temp%\download", logger);
                ExecutionHelper.StartProcess("", @"dir C:\Users\ >> %temp%\download", logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void DomainTrustDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1482");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcess("","nltest /domain_trusts", logger);
                ExecutionHelper.StartProcess("", "nltest /all_trusts", logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void PasswordPolicyDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1201");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcess("", "net accounts", logger);
                ExecutionHelper.StartProcess("", "net accounts /domain", logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void LocalGroups(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1069.001");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcess("", "net localgroup", logger);
                ExecutionHelper.StartProcess("", "net localgroup \"Administrators\"", logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void DomainGroupDiscoveryCmd(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1069.002");
            logger.TimestampInfo("Using the command line to execute technique");
            try
            {
                if (playbook_task.groups.Length > 0)
                {
                    foreach (string group in playbook_task.groups)
                    {
                        ExecutionHelper.StartProcess("", String.Format("net group \"{0}\" /domain", group), logger);
                    }
                    logger.SimulationFinished();
                }
                else
                {
                    ExecutionHelper.StartProcess("", "net group /domain", logger);
                    logger.SimulationFinished();

                }
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void DomaiGroupDiscoveryLdap(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1069.002");
            logger.TimestampInfo("Using LDAP to execute technique");
            try
            {
                if (playbook_task.groups.Length > 0)
                {
                    foreach (string group in playbook_task.groups)
                    {
                        logger.TimestampInfo(String.Format("Querying LDAP for members of '{0}'", group));
                        DiscoveryHelper.LdapQueryForObjects(logger, 2, "", group);
                    }
                    logger.SimulationFinished();
                }
                else
                {
                    logger.TimestampInfo("Querying LDAP for all groups");
                    DiscoveryHelper.LdapQueryForObjects(logger, 2);
                    logger.SimulationFinished();
                }

            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void QueryRegistry(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1012");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcess("", "reg query HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall", logger);
                ExecutionHelper.StartProcess("", "reg query \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\"", logger);
                ExecutionHelper.StartProcess("", "reg query HKLM\\System\\Currentcontrolset\\Service", logger);

                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void SecuritySoftwareDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1518.001");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcess("", "netsh advfirewall firewall show rule name=all", logger);
                ExecutionHelper.StartProcess("", "wmic / Namespace:\\\\root\\SecurityCenter2 Path AntiVirusProduct Get displayName / Format:List", logger);
                

                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void SystemInformationDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1082");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcess("", "systeminfo", logger);
                ExecutionHelper.StartProcess("", "net config workstation", logger);

                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void SystemTimeDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1124");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcess("", "w32tm /tz", logger);
                ExecutionHelper.StartProcess("", "time /T", logger);

                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }
        
    }
}
