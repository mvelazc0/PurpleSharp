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

                List<Computer> targetcomputers = Lib.Targets.GetHostTargets(computertype, nhosts);
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

        public static void PrivilegeEnumeration(int nhost, int sleep)
        {
            
            List<Task> tasklist = new List<Task>();
            var rand = new Random();
            int computertype = rand.Next(1, 6);

            List<Computer> targetcomputers = Lib.Targets.GetHostTargets(computertype, nhost);
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
                List<Computer> targetcomputers = Lib.Targets.GetHostTargets(computertype, nhost);
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

        public static void AccountDiscoveryLdap(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1087");
            logger.TimestampInfo("Using LDAP to execute this technique");
            try
            {
                DiscoveryHelper.ListUsersLdap(logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
            
        }

        public static void AccountDiscoveryCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1087");
            logger.TimestampInfo("Using the command line to execute the technique");

            try
            {
                //ExecutionHelper.StartProcess("", "net group \"Domain Admins\" /domain", log);
                ExecutionHelper.StartProcess("", "net user /domain", logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
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
    }
}
