using System;
using System.Collections.Generic;
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


    }

}
