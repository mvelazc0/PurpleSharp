using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{

    public class DiscoveryHelper
    {
        public static void ShareEnum(Computer computer)
        {
            var bufPtr = IntPtr.Zero;
            var EntriesRead = 0;
            var TotalRead = 0;
            var ResumeHandle = 0;

            const uint MAX_PREFERRED_LENGTH = 0xFFFFFFFF;

            var res = WinAPI.NetShareEnum(computer.Fqdn, 1, ref bufPtr, MAX_PREFERRED_LENGTH, ref EntriesRead, ref TotalRead, ref ResumeHandle);
            var errorCode = Marshal.GetLastWin32Error();

            
            var Offset = bufPtr.ToInt64();

            // 0 = syccess
            if (res == 0)
            {
                DateTime dtime = DateTime.Now;
                Console.WriteLine("{0}[{1}] Successfully enumerated shares on {2} as {3} ", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn, WindowsIdentity.GetCurrent().Name);
            }
            else
            {
                DateTime dtime = DateTime.Now;
                Console.WriteLine("{0}[{1}] Failed to enumerate shares on {2} as {3}. Error Code:{4}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn, WindowsIdentity.GetCurrent().Name, errorCode);
            }

        }

        //From SharpView
        public static void FindLocalAdminAccess(Computer computer)
        {


            var Handle = WinAPI.OpenSCManagerW($@"\\" + computer.Fqdn, "ServicesActive", 0xF003F);


            var errorCode = Marshal.GetLastWin32Error();
            if (Handle != IntPtr.Zero)
            {
                WinAPI.CloseServiceHandle(Handle);
                DateTime dtime = DateTime.Now;
                Console.WriteLine("{0}[{1}] {2} is a local admin on {3}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), Environment.UserName, computer.Fqdn);
            }
            else
            {
                DateTime dtime = DateTime.Now;
                if (errorCode == 5) Console.WriteLine("{0}[{1}] {2} is not a local admin on {3}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), Environment.UserName, computer.Fqdn);
                else Console.WriteLine("{0}[{1}] Could not confirm if {2} is local admin on {3}. Error Code:{4}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), Environment.UserName, computer.Fqdn, errorCode);
            }

        }

        public static void PortScan(Computer computer, TimeSpan timeout)
        {
            IPAddress server2 = IPAddress.Parse(computer.IPv4);
            List<int> ports = new List<int> { 21, 22, 23, 25, 80, 135, 139, 443, 445, 1433, 3306, 3389, 8080, 8000, 10000 };

            foreach (int port in ports)
            {
                //Console.WriteLine("Scanning port {0} on {1}", port, computer.Fqdn);
                IPEndPoint remoteEP = new IPEndPoint(server2, port);
                Socket sender = new Socket(remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {

                    var result = sender.BeginConnect(remoteEP, null, null);

                    bool success = result.AsyncWaitHandle.WaitOne(timeout, true);
                    if (success)
                    {
                        sender.EndConnect(result);
                        //Console.WriteLine("port is open on: " + remoteEP.ToString());
                        //return true;
                    }
                    else
                    {
                        sender.Close();
                        //Console.WriteLine("port is closed on: " + remoteEP.ToString());
                        //return false;
                        //throw new SocketException(10060); // Connection timed out.
                    }

                }
                catch
                {
                    //Console.WriteLine("port is closed on: " + remoteEP.ToString() + " (Exception)");
                    //DateTime dtime = DateTime.Now;
                    //Console.WriteLine("{0}[{1}] Could not perform network service scan on {2}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn);
                    //return false;

                }

            }
            DateTime dtime = DateTime.Now;
            Console.WriteLine("{0}[{1}] Finished network service scan on {2}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn);


        }
    }
}