using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class LateralMovement
    {

        static public void CreateRemoteServiceOnHosts(int computertype, int nhost, int sleep, Boolean cleanup, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1021");

            try
            {
                List<Computer> targethosts = Lib.Targets.GetHostTargets(computertype, nhost);
                List<Task> tasklist = new List<Task>();
                //Console.WriteLine("[*] Starting Service Based Lateral Movement attack from {0} as {1}", Environment.MachineName, WindowsIdentity.GetCurrent().Name);

                foreach (Computer computer in targethosts)
                {
                    Computer temp = computer;
                    if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            LateralMovementHelper.CreateRemoteService(temp, cleanup, logger);
                        }));
                        if (sleep > 0) Thread.Sleep(sleep * 1000);
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

        static public void ExecuteWinRMOnHosts(int computertype, int nhost, int sleep, string command, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1028");
            try
            {

                List<Computer> targethosts = Lib.Targets.GetHostTargets(computertype, nhost);
                List<Task> tasklist = new List<Task>();
                //Console.WriteLine("[*] Starting WinRM Based Lateral Movement attack from {0} running as {1}", Environment.MachineName, WindowsIdentity.GetCurrent().Name);
                if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between attempt", sleep);

                foreach (Computer computer in targethosts)
                {
                    Computer temp = computer;
                    if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            LateralMovementHelper.WinRMCodeExecution(temp, command, logger);
                        }));
                        if (sleep > 0) Thread.Sleep(sleep * 1000);
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

        static public void ExecuteWmiOnHosts(int computertype, int nhost, int sleep, string command, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1047");
            try
            {
                logger.TimestampInfo(String.Format("Querying LDAP for random targets..."));
                List<Computer> targethosts = Lib.Targets.GetHostTargets(computertype, nhost);
                logger.TimestampInfo(String.Format("Obtained {0} target computers", targethosts.Count));
                List<Task> tasklist = new List<Task>();
                //Console.WriteLine("[*] Starting WMI Based Lateral Movement attack from {0} running as {1}", Environment.MachineName, WindowsIdentity.GetCurrent().Name);
                if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between attempt", sleep);

                foreach (Computer computer in targethosts)
                {
                    Computer temp = computer;
                    if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            LateralMovementHelper.WmiCodeExecution(temp, command, logger);
                        }));
                        if (sleep > 0) Thread.Sleep(sleep * 1000);
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

        static public void CreateSchTaskOnHosts(int computertype, int nhost, int sleep, string command, bool cleanup)
        {
            List<Computer> targethosts = Lib.Targets.GetHostTargets(computertype, nhost);
            List<Task> tasklist = new List<Task>();
            Console.WriteLine("[*] Starting Scheduled Task based Lateral Movement simulation from {0} running as {1}", Environment.MachineName, WindowsIdentity.GetCurrent().Name);
            if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between attempt", sleep);
            foreach (Computer computer in targethosts)
            {
                if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                {
                    Computer temp = computer;
                    LateralMovementHelper.CreateRemoteScheduledTask(temp, command, cleanup);
                    /*
                    tasklist.Add(Task.Factory.StartNew(() =>
                    {
                        LateralMovementHelper.CreateRemoteScheduledTask(computer, command, cleanup);
                    }));
                    if (sleep > 0) Thread.Sleep(sleep * 1000);
                    */
                }
            }
            //Task.WaitAll(tasklist.ToArray());


        }

    }
}