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
        public static void EnumerateShares(int hosttype, int nhosts, int sleep)
        {

            List<Task> tasklist = new List<Task>();
            List<Computer> targetcomputers = Lib.Targets.GetHostTargets(hosttype, nhosts);
            Console.WriteLine("[*] Starting Network Share enumreation from {0}", Environment.MachineName);
            if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between enumeration", sleep);
            foreach (Computer computer in targetcomputers)
            {
                if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                {
                    tasklist.Add(Task.Factory.StartNew(() =>
                    {
                        Simulations.DiscoveryHelper.ShareEnum(computer);
                        
                    }));
                    if (sleep > 0) Thread.Sleep(sleep * 1000);
                }
                
            }
            Task.WaitAll(tasklist.ToArray());
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
        public static void NetworkServiceDiscovery(int computertype, int nhost, int sleep)
        {
            List<Task> tasklist = new List<Task>();
            List<Computer> targetcomputers = Lib.Targets.GetHostTargets(computertype, nhost);
            Console.WriteLine("[*] Starting Network Discovery from {0} ", Environment.MachineName);
            if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between each scan", sleep);
            foreach (Computer computer in targetcomputers)
            {
                if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                {
                    Computer temp = computer;
                    TimeSpan interval = TimeSpan.FromSeconds(5);

                    tasklist.Add(Task.Factory.StartNew(() =>
                    {
                        DiscoveryHelper.PortScan(temp, interval);
                        
                    }));
                    if (sleep > 0) Thread.Sleep(sleep * 1000);
                    
                }
            }
            Task.WaitAll(tasklist.ToArray());

        }

        public static void AccountDiscoveryLdap(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1087 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            DiscoveryHelper.ListUsersLdap(logger);

        }

        public static void AccountDiscoveryCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1087 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            ExecutionHelper.StartProcess3("", "net user /domain", logger);
            //ExecutionHelper.StartProcess("", "net group \"Domain Admins\" /domain", log);

        }

        public static void SystemServiceDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1007 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            ExecutionHelper.StartProcess3("", "net start", logger);
            ExecutionHelper.StartProcess3("", "tasklist /svc", logger);

        }

        public static void SystemUserDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1033 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            ExecutionHelper.StartProcess3("", "whoami", logger);
            ExecutionHelper.StartProcess3("", "query user", logger);
        }

        public static void SystemNetworkDiscovery(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1049 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            ExecutionHelper.StartProcess3("", "netstat", logger);
            ExecutionHelper.StartProcess3("", "net use", logger);
            ExecutionHelper.StartProcess3("", "net session", logger);
        }


    }

}
