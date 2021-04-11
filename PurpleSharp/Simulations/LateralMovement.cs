using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class LateralMovement
    {

        static public void CreateRemoteServiceOnHosts(int nhost, int tsleep, bool cleanup, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1021");
            logger.TimestampInfo("Using the Win32 API CreateService function to execute this technique");

            try
            {
                var rand = new Random();
                int computertype = rand.Next(1, 6);
                logger.TimestampInfo(String.Format("Querying LDAP for random targets..."));
                List<Computer> targethosts = Lib.Targets.GetHostTargets_old(computertype, nhost, logger);
                logger.TimestampInfo(String.Format("Obtained {0} target computers", targethosts.Count));
                List<Task> tasklist = new List<Task>();
                //Console.WriteLine("[*] Starting Service Based Lateral Movement attack from {0} as {1}", Environment.MachineName, WindowsIdentity.GetCurrent().Name);
                if (tsleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between attempt", tsleep));

                foreach (Computer computer in targethosts)
                {
                    Computer temp = computer;
                    if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            LateralMovementHelper.CreateRemoteServiceApi(temp, cleanup, logger);
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

        static public void WinRmCodeExec(int nhost, int tsleep, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1021.006");
            logger.TimestampInfo("Using the System.Management.Automation .NET namespace to execute this technique");

            try
            {
                var rand = new Random();
                int computertype = rand.Next(1, 6);
                logger.TimestampInfo(String.Format("Querying LDAP for random targets..."));
                List<Computer> targethosts = Lib.Targets.GetHostTargets_old(computertype, nhost, logger);
                logger.TimestampInfo(String.Format("Obtained {0} target computers", targethosts.Count));
                List<Task> tasklist = new List<Task>();
                //Console.WriteLine("[*] Starting WinRM Based Lateral Movement attack from {0} running as {1}", Environment.MachineName, WindowsIdentity.GetCurrent().Name);
                if (tsleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between attempt", tsleep));

                foreach (Computer computer in targethosts)
                {
                    Computer temp = computer;
                    if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            LateralMovementHelper.WinRMCodeExecution(temp, "powershell.exe", logger);
                        }));
                        if (tsleep > 0) Thread.Sleep(tsleep * 1000);
                    }
                }
                Task.WaitAll(tasklist.ToArray());
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        static public void ExecuteWmiOnHosts(int nhost, int tsleep, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1047");
            logger.TimestampInfo("Using the System.Management .NET API to execute this technique");

            try
            {
                var rand = new Random();
                int computertype = rand.Next(1, 6);

                logger.TimestampInfo(String.Format("Querying LDAP for random targets..."));
                List<Computer> targethosts = Lib.Targets.GetHostTargets_old(computertype, nhost, logger);
                logger.TimestampInfo(String.Format("Obtained {0} target computers", targethosts.Count));
                List<Task> tasklist = new List<Task>();
                if (tsleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between attempt", tsleep));

                foreach (Computer computer in targethosts)
                {
                    Computer temp = computer;
                    if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            LateralMovementHelper.WmiCodeExecution(temp, "powershell.exe", logger);
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

        static public void CreateSchTaskOnHosts(int nhost, int sleep, bool cleanup)
        {
            /*
            var rand = new Random();
            int computertype = rand.Next(1, 6);

            List<Computer> targethosts = Lib.Targets.GetHostTargets(computertype, nhost);
            List<Task> tasklist = new List<Task>();
            Console.WriteLine("[*] Starting Scheduled Task based Lateral Movement simulation from {0} running as {1}", Environment.MachineName, WindowsIdentity.GetCurrent().Name);
            if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between attempt", sleep);
            foreach (Computer computer in targethosts)
            {
                if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                {
                    Computer temp = computer;
                    LateralMovementHelper.CreateRemoteScheduledTask(temp, "powershell.exe", cleanup);
                    
                    tasklist.Add(Task.Factory.StartNew(() =>
                    {
                        LateralMovementHelper.CreateRemoteScheduledTask(computer, command, cleanup);
                    }));
                    if (sleep > 0) Thread.Sleep(sleep * 1000);
                    
                }
            }
            //Task.WaitAll(tasklist.ToArray());
            */

        }

    }
}