using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Lib
{
    public class Targets
    {

        public static List<Computer> GetNetworkNeighborTargets(int count, Logger logger)
        {
            List<Computer> targets = new List<Computer>();
            List<IPAddress> neighbors = Lib.Networking.GetNeighbors(count);
            List<Task> tasklist = new List<Task>();
            Console.WriteLine("[*] Obtaining network neighbor targets ...");

            foreach (IPAddress ip in neighbors)
            {
                tasklist.Add(Task.Factory.StartNew(() => {


                    TimeSpan interval = TimeSpan.FromSeconds(3);
                    if (Lib.Networking.OpenPort(ip.ToString(), 445, interval))
                    {
                        Console.WriteLine("trying to resolve " + ip.ToString());
                        string hostname = Lib.Networking.ResolveIp(ip);
                        if (hostname != "")
                        {
                            Computer newhost = new Computer();
                            newhost.Fqdn = hostname;
                            newhost.IPv4 = ip.ToString();
                            targets.Add(newhost);
                            //Console.WriteLine("Found " + hostname);
                        }
                    }
                }));
            }
            Task.WaitAll(tasklist.ToArray());

            return targets;

        }


        public static List<Computer> GetDomainNeighborTargets(int count, Lib.Logger logger)
        {

            List<Computer> targets = new List<Computer>();
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            string dc = context.ConnectedServer;
            logger.TimestampInfo("Obtaining domain neighbor targets ...");
            targets = Ldap.GetADComputers(count, logger, dc);
            //Console.WriteLine("[*] Finished");
            return targets;


        }

        public static List<Computer> GetDomainRandomTargets(int count, Logger logger)
        {
            List<Computer> targets = new List<Computer>();
            logger.TimestampInfo("Obtaining domain random targets ...");
            targets = Ldap.GetADComputers(count, logger);

            return targets;

        }

        public static List<Computer> GetServerRangeTargets(int count)
        {
            List<Task> tasklist = new List<Task>();
            Console.WriteLine("[*] Obtaining random server targets ...");

            List<Computer> targets = new List<Computer>();
            List<IPAddress> ips = new List<IPAddress>();
            List<Computer> dcs = Ldap.GetDcs();

            Random random = new Random();
            int index = random.Next(dcs.Count);
            IPAddress dc = IPAddress.Parse(dcs[index].IPv4);
            Console.WriteLine("[*] Wrandomly picked DC {0}", dcs[index].Fqdn);

            ips = Lib.Networking.GetRange(dc, 24, count);
            foreach (IPAddress ip in ips)
            {
                tasklist.Add(Task.Factory.StartNew(() => {


                    TimeSpan interval = TimeSpan.FromSeconds(3);
                    if (Lib.Networking.OpenPort(ip.ToString(), 445, interval))
                    {
                        string hostname = Lib.Networking.ResolveIp(ip);
                        if (hostname != "")
                        {
                            Computer newhost = new Computer();
                            newhost.Fqdn = hostname;
                            newhost.IPv4 = ip.ToString();
                            targets.Add(newhost);
                            //Console.WriteLine("Found " + hostname);
                        }
                    }
                }));
            }
            Task.WaitAll(tasklist.ToArray());

            return targets;

        }


        public static List<User> GetRandomUsernames(int count, Random random)
        {
            List<User> users = new List<User>();
            for (int i = 0; i < count; i++ )
            {
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                User nuser = new User(new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray()));
                users.Add(nuser);
            }
            return users;
        }

        public static List<User> GetUserTargets(PlaybookTask playbook_task, Logger logger)
        {
            PrincipalContext context;
            string dc;
            List<User> targetusers = new List<User>();


            switch (playbook_task.user_target_type)
            {
                case 1:
                    logger.TimestampInfo("Targeting playbook defined users");
                    foreach (string user in playbook_task.user_targets)
                    {
                        User nuser = new User(user);
                        targetusers.Add(nuser);
                    }
                    break;

                case 2:
                    context = new PrincipalContext(ContextType.Domain);
                    dc = context.ConnectedServer;
                    logger.TimestampInfo("Targeting random domain users");
                    targetusers = Ldap.GetADUsers(playbook_task.user_target_total, logger, dc, true);
                    logger.TimestampInfo(String.Format("Obtained {0} user records", targetusers.Count));

                    break;

                case 3:
                    logger.TimestampInfo("Targeting randomly generated users");
                    targetusers = Targets.GetRandomUsernames(playbook_task.user_target_total, new Random());
                    break;

                case 4:
                    logger.TimestampInfo("Targeting administrative accounts (adminCount=1) ");
                    targetusers = Ldap.GetADAdmins(playbook_task.user_target_total, logger);
                    logger.TimestampInfo(String.Format("Obtained {0} user records", targetusers.Count));
                    break;

                case 5:
                    logger.TimestampInfo("Targeting domain admins");
                    targetusers = Ldap.GetDomainAdmins(logger);
                    logger.TimestampInfo(String.Format("Obtained {0} user records", targetusers.Count));
                    break;

                case 6:
                    context = new PrincipalContext(ContextType.Domain);
                    dc = context.ConnectedServer;
                    logger.TimestampInfo("Targeting disabled users");
                    targetusers = Ldap.GetADUsers(playbook_task.user_target_total, logger, dc, false);
                    logger.TimestampInfo(String.Format("Obtained {0} user records", targetusers.Count));
                    break;

                default:
                    return targetusers;
            }

            return targetusers;
        }

        public static List<Computer> GetHostTargets_old(int servertype, int nhosts, Lib.Logger logger)
        {
            List<Computer> targethosts = new List<Computer>();
            /*
            switch (servertype)
            {
                case 1:
                    targethosts = GetDomainNeighborTargets(nhosts);
                    break;
                case 2:
                    targethosts = GetDomainRandomTargets(nhosts);
                    break;
                case 3:
                    targethosts = GetServerRangeTargets(nhosts);
                    break;
                case 4:
                    targethosts = GetNetworkNeighborTargets(nhosts);
                    break;
                default:
                    return targethosts; 
            }
            */
            targethosts = GetDomainNeighborTargets(nhosts, logger);
            return targethosts;

        }

        public static List<Computer> GetHostTargets(PlaybookTask playbook_task, Logger logger)
        {
            List<Computer> host_targets = new List<Computer>();
            Computer host_target = new Computer();
            IPAddress ipAddress = null;

            bool isValidIp;

            switch (playbook_task.host_target_type)
            {
                case 1:
                    if (playbook_task.host_targets.Length > 1)
                    {
                        foreach (string target in playbook_task.host_targets)
                        {
                            isValidIp = IPAddress.TryParse(target, out ipAddress);
                            if (isValidIp) host_target = new Computer("", target);
                            else host_target = new Computer(target, Networking.ResolveHostname(target).ToString());
                            host_targets.Add(host_target);
                        }
                        logger.TimestampInfo(String.Format("Using {0} targets defined in the playbook", playbook_task.host_targets.Length.ToString()));
                    }
                    else
                    {
                        isValidIp = IPAddress.TryParse(playbook_task.host_targets[0], out ipAddress);
                        if (isValidIp) host_target = new Computer("", playbook_task.host_targets[0]);
                        else host_target = new Computer(playbook_task.host_targets[0], Networking.ResolveHostname(playbook_task.host_targets[0]).ToString());
                        logger.TimestampInfo(String.Format("Using {0} {1} as the target", host_target.ComputerName, host_target.IPv4));
                        host_targets.Add(host_target);
                    }
                    break;

                case 2:
                    logger.TimestampInfo("Targeting a random domain host target");
                    host_targets = GetDomainNeighborTargets(playbook_task.host_target_total, logger);
                    logger.TimestampInfo(String.Format("Obtained {0} host records", host_targets.Count));
                    var random = new Random();
                    int index = random.Next(host_targets.Count);
                    host_target = host_targets[index];
                    logger.TimestampInfo(String.Format("Randomly picked {0} {1} as the target", host_target.ComputerName, host_target.IPv4));
                    host_targets.Clear();
                    host_targets.Add(host_target);
                    break;
                
                case 4:
                    logger.TimestampInfo("Targeting random domain hosts");
                    host_targets = GetDomainNeighborTargets(playbook_task.host_target_total, logger);
                    logger.TimestampInfo(String.Format("Obtained {0} host records", host_targets.Count));
                    break;
                
                default:
                    return host_targets;
            }
            return host_targets;

        }

    }
}
