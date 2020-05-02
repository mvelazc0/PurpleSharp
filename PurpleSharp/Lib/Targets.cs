using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Lib
{
    public class Targets
    {

        public static List<Computer> GetNetworkNeighborTargets(int count)
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


        public static List<Computer> GetDomainNeighborTargets(int count)
        {
            List<Computer> targets = new List<Computer>();
            /*
            string logonserver = Environment.GetEnvironmentVariable("logonserver").Replace("\\","");
            string dnsdomain = Environment.GetEnvironmentVariable("USERDNSDOMAIN");
            string dc = logonserver + "." + dnsdomain;
            */
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            string dc = context.ConnectedServer;
            //logger.TimestampInfo("[*] Obtaining domain neighbor targets ...");
            Console.WriteLine("[*] Obtaining domain neighbor targets ...");
            targets = Ldap.GetADComputers(count, dc);
            //Console.WriteLine("[*] Finished");
            return targets;


        }

        public static List<Computer> GetDomainRandomTargets(int count)
        {
            List<Computer> targets = new List<Computer>();
            Console.WriteLine("[*] Obtaining domain random targets ...");
            targets = Ldap.GetADComputers(count);

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
                        //Console.WriteLine("trying to resolve " + ip.ToString());
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


        public static List<User> GetRandomUsernames(int count)
        {
            List<User> users = new List<User>();
            Console.WriteLine("[*] Generating random usernames ...");
            for (int i = 0; i < count; i++ )
            {
                Random random = new Random();
                string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                User nuser = new User();
                nuser.UserName = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray()); ;
                users.Add(nuser);
            }
            return users;
        }

        public static List<User> GetUserTargets(int usertype, int nuser)
        {
            List<User> targetusers = new List<User>();
            /*
            string logonserver, dnsdomain, dc;
            logonserver = Environment.GetEnvironmentVariable("logonserver").Replace("\\", "");
            dnsdomain = Environment.GetEnvironmentVariable("USERDNSDOMAIN");
            dc = logonserver + "." + dnsdomain;
            */
            
            PrincipalContext context = new PrincipalContext(ContextType.Domain);
            string dc = context.ConnectedServer;


            switch (usertype)
            {
                case 1:

                    Console.WriteLine("[*] Targeting domain neighbor users");
                    targetusers = Ldap.GetADUsers(nuser, dc, true);
                    break;

                case 2:
                    Console.WriteLine("[*] Targeting domain foreign users");
                    targetusers = Ldap.GetADUsers(nuser, "", true);
                    break;

                case 3:
                    Console.WriteLine("[*] Targeting disabled users");
                    targetusers = Ldap.GetADUsers(nuser, dc, false);
                    break;

                case 4:
                    Console.WriteLine("[*] Targeting administrative accounts (adminCount=1) ");
                    targetusers = Ldap.GetADAdmins(nuser);
                    break;

                case 5:
                    Console.WriteLine("[*] Targeting domain admins");
                    targetusers = Ldap.GetDomainAdmins();
                    break;
               
                case 6:
                    targetusers = Targets.GetRandomUsernames(nuser);
                    break;

                default:
                    return targetusers;
            }

            return targetusers;
        }

        public static List<Computer> GetHostTargets(int servertype, int nhosts)
        {
            List<Computer> targethosts = new List<Computer>();
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

            return targethosts;

        }

    }
}
