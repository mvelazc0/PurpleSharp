using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Lib
{
    static class Networking
    {
        public static string PingHost(string host)
        {
            Ping sendPing = new Ping();

            try
            {
                PingReply reply = sendPing.Send(host, 2000);

                if (reply.Status == IPStatus.Success)
                {
                    //host is alive
                    //var ipv4 = reply.Address.ToString();
                    //Console.WriteLine(host + " is alive");
                    return reply.Address.ToString();

                }
                else
                {
                    //Console.WriteLine(host + " is offline");
                    return "";
                }
            }
            catch
            {
                return "";
            }


        }

        public static string ResolveIp(IPAddress address)
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(address);
                return host.HostName;
            }
            catch
            {
                return "";
            }
        }

        public static IPAddress ResolveHostname(string hostname)
        {
            try
            {
                IPHostEntry host = Dns.GetHostEntry(hostname);
                return host.AddressList[0];
            }
            catch
            {
                return null;
            }

        }


        public static bool OpenPort(string server, int port, TimeSpan timeout)
        {
            IPAddress server2 = IPAddress.Parse(server);
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
                    return true;
                }
                else
                {
                    sender.Close();
                    //Console.WriteLine("port is closed on: " + remoteEP.ToString());
                    return false;
                    //throw new SocketException(10060); // Connection timed out.
                }

            }
            catch
            {
                Console.WriteLine("port is closed on: " + remoteEP.ToString() + " (Exception)");
                return false;
            }

        }

        public static List<IPAddress> GetNeighbors(int count)
        {
            NetworkInterface[] Interfaces = NetworkInterface.GetAllNetworkInterfaces();
            List<IPAddress> neighbors = new List<IPAddress>();

            foreach (NetworkInterface Interface in Interfaces)
            {
                
                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Loopback || Interface.Description.Contains("ISATAP") || Interface.Description.Contains("VMnet")) continue;
                Console.WriteLine(Interface.Description);
                if (Interface.Description.Contains("Realtek"))
                //if (Interface.Description == "vmxnet3 Ethernet Adapter")
                {
                    Console.WriteLine(Interface.Description);
                    foreach (UnicastIPAddressInformation ip in Interface.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            //Console.WriteLine("\tIP Address is {0}", ip.Address);
                            //Console.WriteLine("\tSubnet Mask is {0}", ip.IPv4Mask);

                            Console.WriteLine("\tSubnet Mask is {0}", ip_masktocidr(ip.IPv4Mask.ToString()));

                            neighbors=GetRange(ip.Address, ip_masktocidr(ip.IPv4Mask.ToString()),count);   
                        }                       
                    }
                }              
            }
            return neighbors;

        }

        //Modified from https://stackoverflow.com/questions/1470792/how-to-calculate-the-ip-range-when-the-ip-address-and-the-netmask-is-given/1470835#1470835
        public static List<IPAddress> GetRange(IPAddress ip, int bits, int count)
        {
            uint mask = ~(uint.MaxValue >> bits);

            // Convert the IP address to bytes.
            byte[] ipBytes = ip.GetAddressBytes();

            // BitConverter gives bytes in opposite order to GetAddressBytes().
            byte[] maskBytes = BitConverter.GetBytes(mask).Reverse().ToArray();

            byte[] startIPBytes = new byte[ipBytes.Length];
            byte[] endIPBytes = new byte[ipBytes.Length];

            // Calculate the bytes of the start and end IP addresses.
            for (int i = 0; i < ipBytes.Length; i++)
            {
                startIPBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
                endIPBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
            }

            // Convert the bytes to IP addresses.
            IPAddress startIP = new IPAddress(startIPBytes);
            IPAddress endIP = new IPAddress(endIPBytes);

            List<IPAddress> ips = new List<IPAddress>();
            Random rnd = new Random();


            var Range = new IPAddressRange(startIP, endIP);
            foreach (IPAddress ipaddr in Range)
            {
                ips.Add(ipaddr);
            }

            return ips.OrderBy(arg => Guid.NewGuid()).Take(count).ToList();

        }

        //http://keroger2k.github.io/blog/2012/06/06/system-dot-net-dot-ip-dot-address-shortcomings/
        public static byte ip_masktocidr(string Mask)
        {
            uint mask = ip_iptouint(Mask);
            byte bits = 0;
            for (uint pointer = 0x80000000; (mask & pointer) != 0; pointer >>= 1)
            {
                bits++;
            }
            return bits;
        }

        //http://keroger2k.github.io/blog/2012/06/06/system-dot-net-dot-ip-dot-address-shortcomings/
        public static uint ip_iptouint(string ip)
        {
            IPAddress i = IPAddress.Parse(ip);
            byte[] ipByteArray = i.GetAddressBytes();

            uint ipUint = (uint)ipByteArray[0] << 24;
            ipUint += (uint)ipByteArray[1] << 16;
            ipUint += (uint)ipByteArray[2] << 8;
            ipUint += (uint)ipByteArray[3];

            return ipUint;
        }
        public static List<Computer> NetworkScan(List<Computer> computers, int port, TimeSpan timeout)
        {
            List<Computer> results = new List<Computer>();
            List<Task> tasklist = new List<Task>();
            foreach (Computer computer in computers)
            {

                IPAddress server2 = IPAddress.Parse(computer.IPv4);
                IPEndPoint remoteEP = new IPEndPoint(server2, port);

                tasklist.Add(Task.Factory.StartNew(() => {
                    //Console.WriteLine($"About scan port on: {computer.IPv4}"); 
                    TimeSpan interval = TimeSpan.FromSeconds(5);
                    if (OpenPort(computer.IPv4, port, interval))
                    {
                        results.Add(computer);
                    }
                }));
            }
            Task.WaitAll(tasklist.ToArray());
            return results;
        }
        
    }


}
