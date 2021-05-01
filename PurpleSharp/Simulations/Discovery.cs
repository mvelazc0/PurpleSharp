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

        public static void NetworkShareEnumerationCmdLocal(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1135");
            logger.TimestampInfo("Using the command line to execute the technique");
            try
            {
                ExecutionHelper.StartProcessApi("", "net share", logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void NetworkShareEnumerationCmdRemote(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1135");
            logger.TimestampInfo("Using the command line to execute the technique");
            try
            {
                List<Computer> target_hosts = Targets.GetHostTargets(playbook_task, logger);
                if (playbook_task.task_sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between each network scan", playbook_task.task_sleep));
                foreach (Computer computer in target_hosts)
                {
                    ExecutionHelper.StartProcessApi("", String.Format("net view \\\\{0}", computer.IPv4), logger);
                }
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void NetworkShareEnumerationApiRemote(PlaybookTask playbook_task, string log)
        {

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1135");
            logger.TimestampInfo("Using the Win32 API NetShareEnum function to execute this technique");

            try
            {
                //List<Computer> targetcomputers = Lib.Targets.GetHostTargets_old(computertype, nhosts, logger);
                List<Task> tasklist = new List<Task>();
                List<Computer> target_hosts = Targets.GetHostTargets(playbook_task, logger);
                if (playbook_task.task_sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between each enumeration attempt", playbook_task.task_sleep));
                foreach (Computer computer in target_hosts)
                {
                    if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            DiscoveryHelper.ShareEnum(computer, logger);

                        }));
                        if (playbook_task.task_sleep > 0) Thread.Sleep(playbook_task.task_sleep * 1000);
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
        public static void NetworkServiceDiscovery(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1046");
            logger.TimestampInfo("Using the System.Net.Sockets .NET namespace to execute this technique");

            try
            {
                List<Task> tasklist = new List<Task>();
                List<Computer> target_hosts = Targets.GetHostTargets(playbook_task, logger);
                //logger.TimestampInfo(String.Format("Obtained {0} target computers for the scan", target_hosts.Count));
                if (playbook_task.task_sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between each network scan", playbook_task.task_sleep));
                foreach (Computer computer in target_hosts)
                {
                    //if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    //{
                    Computer temp = computer;
                    TimeSpan interval = TimeSpan.FromSeconds(5);

                    tasklist.Add(Task.Factory.StartNew(() =>
                    {
                        logger.TimestampInfo(String.Format("Starting port scan against {0} ({1})", temp.ComputerName, temp.IPv4));
                        DiscoveryHelper.PortScan(temp, interval, playbook_task.ports, logger) ;
                    }));
                    if (playbook_task.task_sleep > 0) Thread.Sleep(playbook_task.task_sleep * 1000);

                    //}
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
            Logger logger = new Logger(currentPath + log);
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
            Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1087.002");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                //ExecutionHelper.StartProcessApi("", "net user /domain", logger);
                ExecutionHelper.StartProcessNET("net.exe", "user /domain", logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }   
        }

        public static void DomainAccountDiscoveryPowerShell(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1087.002");
            logger.TimestampInfo("Using PowerShell to execute the technique");

            try
            {
                string cleanPws = String.Format("Get-ADUser -Filter * | Select-Object SamAccountNAme");
                logger.TimestampInfo(String.Format("Using plaintext PowerShell command: {0}", cleanPws));
                var cleanPwsBytes = System.Text.Encoding.Unicode.GetBytes(cleanPws);
                ExecutionHelper.StartProcessApi("", String.Format("powershell.exe -enc {0}", Convert.ToBase64String(cleanPwsBytes)), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
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
                ExecutionHelper.StartProcessApi("", "net user", logger);
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
                ExecutionHelper.StartProcessApi("", "net start", logger);
                ExecutionHelper.StartProcessApi("", "tasklist /svc", logger);
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
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1033");
            try
            {
                ExecutionHelper.StartProcessApi("", "whoami", logger);
                ExecutionHelper.StartProcessApi("", "query user", logger);
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
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1049");
            try
            {
                ExecutionHelper.StartProcessApi("", "netstat", logger);
                ExecutionHelper.StartProcessApi("", "net use", logger);
                ExecutionHelper.StartProcessApi("", "net session", logger);
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
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1016");
            try
            {
                ExecutionHelper.StartProcessApi("", "ipconfig /all", logger);
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
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1083");

            try
            {
                ExecutionHelper.StartProcessApi("", @"dir c:\ >> %temp%\download", logger);
                ExecutionHelper.StartProcessApi("", @"dir C:\Users\ >> %temp%\download", logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void DomainTrustDiscoveryCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1482");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcessApi("","nltest /domain_trusts", logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void DomainTrustDiscoveryPowerShell(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1482");
            logger.TimestampInfo("Using PowerShell to execute the technique");

            try
            {
                string cleanPws = String.Format("Get-DomainTrusts");
                logger.TimestampInfo(String.Format("Using plaintext PowerShell command: {0}", cleanPws));
                var plainTextBytes = System.Text.Encoding.Unicode.GetBytes(cleanPws);
                ExecutionHelper.StartProcessApi("", String.Format("powershell.exe -enc {0}", Convert.ToBase64String(plainTextBytes)), logger);
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
                ExecutionHelper.StartProcessApi("", "net accounts", logger);
                ExecutionHelper.StartProcessApi("", "net accounts /domain", logger);
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
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1069.001");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                ExecutionHelper.StartProcessApi("", "net localgroup", logger);
                ExecutionHelper.StartProcessApi("", "net localgroup \"Administrators\"", logger);
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
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1069.002");
            logger.TimestampInfo("Using the command line to execute technique");
            try
            {
                if (playbook_task.groups.Length > 0)
                {
                    foreach (string group in playbook_task.groups)
                    {
                        ExecutionHelper.StartProcessApi("", String.Format("net group \"{0}\" /domain", group), logger);
                    }
                    logger.SimulationFinished();
                }
                else
                {
                    ExecutionHelper.StartProcessApi("", "net group /domain", logger);
                    logger.SimulationFinished();

                }
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void DomainGroupDiscoveryPowerShell(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1069.002");
            logger.TimestampInfo("Using PowerShell to execute the technique");
            try
            {
                if (playbook_task.groups.Length > 0)
                {
                    foreach (string group in playbook_task.groups)
                    {
                        string cleanPws = String.Format("Get-AdGroup -Filter {{Name -like '{0}'}} | Get-ADGroupMember | Select SamAccountName", group);
                        logger.TimestampInfo(String.Format("Using plaintext PowerShell command: {0}", cleanPws));
                        var plainTextBytes = System.Text.Encoding.Unicode.GetBytes(cleanPws);
                        ExecutionHelper.StartProcessApi("", String.Format("powershell.exe -enc {0}", Convert.ToBase64String(plainTextBytes)), logger);
                    }
                    logger.SimulationFinished();
                }
                else
                {
                    string cleanPws = String.Format("Get-AdGroup -Filter {{Name -like 'Domain Admins'}} | Get-ADGroupMember | Select SamAccountName");
                    logger.TimestampInfo(String.Format("Using plaintext PowerShell command: {0}", cleanPws));
                    var plainTextBytes = System.Text.Encoding.Unicode.GetBytes(cleanPws);
                    ExecutionHelper.StartProcessApi("", String.Format("powershell.exe -enc {0}", Convert.ToBase64String(plainTextBytes)), logger);
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
                ExecutionHelper.StartProcessApi("", "reg query HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall", logger);
                ExecutionHelper.StartProcessApi("", "reg query \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\"", logger);
                ExecutionHelper.StartProcessApi("", "reg query HKLM\\System\\Currentcontrolset\\Service", logger);

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
                ExecutionHelper.StartProcessApi("", "netsh advfirewall firewall show rule name=all", logger);
                ExecutionHelper.StartProcessApi("", "wmic / Namespace:\\\\root\\SecurityCenter2 Path AntiVirusProduct Get displayName / Format:List", logger);
                

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
                ExecutionHelper.StartProcessApi("", "systeminfo", logger);
                ExecutionHelper.StartProcessApi("", "net config workstation", logger);

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
                ExecutionHelper.StartProcessApi("", "w32tm /tz", logger);
                ExecutionHelper.StartProcessApi("", "time /T", logger);

                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }
        
    }
}
