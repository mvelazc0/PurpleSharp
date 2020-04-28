using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class Discovery
    {
        public static void EnumerateShares(int hosttype, int nhosts, int sleep, string log)
        {

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1135");
            logger.SimulationDetails();
            //logger.TimestampInfo(String.Format("Starting T1135 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));

            try
            {
                List<Task> tasklist = new List<Task>();
                List<Computer> targetcomputers = Lib.Targets.GetHostTargets(hosttype, nhosts);
                logger.TimestampInfo(String.Format("Obtained {0} target computers", targetcomputers.Count));
                //Console.WriteLine("[*] Starting Network Share enumreation from {0}", Environment.MachineName);
                if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between enumeration", sleep);
                foreach (Computer computer in targetcomputers)
                {
                    if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            Simulations.DiscoveryHelper.ShareEnum(computer, logger);

                        }));
                        if (sleep > 0) Thread.Sleep(sleep * 1000);
                    }

                }
                Task.WaitAll(tasklist.ToArray());
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed();
            }
        }

        public static void PrivilegeEnumeration(int computertype, int nhost, int sleep)
        {
            
            List<Task> tasklist = new List<Task>();
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
        public static void NetworkServiceDiscovery(int computertype, int nhost, int sleep, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1046");
            logger.SimulationDetails();
            //logger.TimestampInfo(String.Format("Starting T1046 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            try
            {
                List<Task> tasklist = new List<Task>();
                List<Computer> targetcomputers = Lib.Targets.GetHostTargets(computertype, nhost);
                logger.TimestampInfo(String.Format("Obtained {0} target computers for the scan", targetcomputers.Count));
                //Console.WriteLine("[*] Starting Network Discovery from {0} ", Environment.MachineName);
                if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between each scan", sleep);
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
                        if (sleep > 0) Thread.Sleep(sleep * 1000);

                    }
                }
                Task.WaitAll(tasklist.ToArray());
                logger.SimulationFinished();

            }
            catch (Exception ex)
            {
                logger.SimulationFailed();
            }

        }

        public static void AccountDiscoveryLdap(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1087");
            logger.SimulationDetails();
            //logger.TimestampInfo(String.Format("Starting T1087 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));

            try
            {
                DiscoveryHelper.ListUsersLdap(logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
            
        }

        public static void AccountDiscoveryCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1087");
            logger.SimulationDetails();
            //logger.TimestampInfo(String.Format("Starting T1087 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));

            try
            {
                //ExecutionHelper.StartProcess("", "net group \"Domain Admins\" /domain", log);
                ExecutionHelper.StartProcess("", "net user /domain", logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
            

        }

        public static void SystemServiceDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1007");
            logger.SimulationDetails();
            //logger.TimestampInfo(String.Format("Starting T1007 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            try
            {
                ExecutionHelper.StartProcess("", "net start", logger);
                ExecutionHelper.StartProcess("", "tasklist /svc", logger);
                logger.SimulationFinished();
            }
            catch 
            {
                logger.SimulationFailed();
            }

        }

        public static void SystemUserDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1033");
            logger.SimulationDetails();
            //logger.TimestampInfo(String.Format("Starting T1033 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            try
            {
                ExecutionHelper.StartProcess("", "whoami", logger);
                ExecutionHelper.StartProcess("", "query user", logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
            
        }

        public static void SystemNetworkDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1049");
            logger.SimulationDetails();

            //logger.TimestampInfo(String.Format("Starting T1049 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            try
            {
                ExecutionHelper.StartProcess("", "netstat", logger);
                ExecutionHelper.StartProcess("", "net use", logger);
                ExecutionHelper.StartProcess("", "net session", logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
        }


    }

}
