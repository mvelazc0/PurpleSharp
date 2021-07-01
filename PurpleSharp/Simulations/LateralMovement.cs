using PurpleSharp.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class LateralMovement
    {

        static public void CreateRemoteServiceOnHosts_Old(int nhost, int tsleep, bool cleanup, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1021");
            logger.TimestampInfo("Using the Win32 API CreateService function to execute this technique");

            try
            {
                var rand = new Random();
                int computertype = rand.Next(1, 6);
                logger.TimestampInfo(String.Format("Querying LDAP for random targets..."));
                List<Computer> targethosts = Targets.GetHostTargets_old(computertype, nhost, logger);
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
                            LateralMovementHelper.CreateRemoteServiceApi_Old(temp, cleanup, logger);
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

        static public void CreateRemoteServiceOnHosts(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1021.002");
            logger.TimestampInfo("Using the Win32 API CreateService function to execute this technique against remote hosts");

            List<Computer> host_targets = new List<Computer>();
            List<Task> tasklist = new List<Task>();


            if (playbook_task.serviceName.Equals("random"))
            {
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
                Random random = new Random();
                logger.TimestampInfo("Using random Service Name");
                playbook_task.serviceName = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
            }

            try
            {
                host_targets = Targets.GetHostTargets(playbook_task, logger);
                foreach (Computer computer in host_targets)
                {
                    Computer temp = computer;
                    if (!computer.ComputerName.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            LateralMovementHelper.CreateRemoteServiceApi(temp, playbook_task, logger);
                        }));
                        if (playbook_task.task_sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between attempt", playbook_task.task_sleep));
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

        static public void ModifyRemoteServiceOnHosts(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1021.002");
            logger.TimestampInfo("Using the Win32 API CreateService function to execute this technique against remote hosts");

            List<Computer> host_targets = new List<Computer>();
            List<Task> tasklist = new List<Task>();

            try
            {
                host_targets = Targets.GetHostTargets(playbook_task, logger);

                foreach (Computer computer in host_targets)
                {
                    Computer temp = computer;
                    if (!computer.ComputerName.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        //LateralMovementHelper.ModifyRemoteServiceApi(temp, playbook_task, logger);
                        
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            LateralMovementHelper.ModifyRemoteServiceApi(temp, playbook_task, logger);
                        }));
                        
                        if (playbook_task.task_sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between attempt", playbook_task.task_sleep));
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


        static public void WinRmCodeExec(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1021.006");
            logger.TimestampInfo("Using the System.Management.Automation .NET namespace to execute this technique");

            List<Computer> host_targets = new List<Computer>();
            List<Task> tasklist = new List<Task>();
            try
            {
                host_targets = Targets.GetHostTargets(playbook_task, logger);

                foreach (Computer computer in host_targets)
                {
                    Computer temp = computer;
                    if (!computer.Fqdn.ToUpper().Contains(Environment.MachineName.ToUpper()))
                    {
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            LateralMovementHelper.WinRMCodeExecution(temp, playbook_task, logger);
                        }));
                        if (playbook_task.task_sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between attempt", playbook_task.task_sleep));
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