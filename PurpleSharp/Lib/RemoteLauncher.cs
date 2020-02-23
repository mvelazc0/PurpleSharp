using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Management;

namespace PurpleSharp.Lib
{

    /*
    //Reference https://stackoverflow.com/questions/22544903/impersonate-for-entire-application-lifecycle
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public  class Impersonation : IDisposable
    {
        private readonly SafeTokenHandle _handle;
        private readonly WindowsImpersonationContext _context;

        const int LOGON32_LOGON_NEW_CREDENTIALS = 2;

        public Impersonation(string domain, string username, string password)
        {
            var ok = LogonUser(username, domain, password,
                           LOGON32_LOGON_NEW_CREDENTIALS, 0, out this._handle);
            if (!ok)
            {
                var errorCode = Marshal.GetLastWin32Error();
                throw new ApplicationException(string.Format("Could not impersonate the elevated user.  LogonUser returned error code {0}.", errorCode));
            }

            this._context = WindowsIdentity.Impersonate(this._handle.DangerousGetHandle());
        }

        public void Dispose()
        {
            this._context.Dispose();
            this._handle.Dispose();
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeTokenHandle()
                : base(true) { }

            [DllImport("kernel32.dll")]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr handle);

            protected override bool ReleaseHandle()
            {
                return CloseHandle(handle);
            }
        }
    }
    */

    //Most of this is based on SharpExec (https://github.com/anthemtotheego/SharpExec)
    class RemoteLauncher
    {
        public static void upload(string uploadPath, string executionPath, string rhost, string ruser, string rpwd, string domain)
        {
            if (ruser == "" && rpwd == "")
            {

                string share = executionPath.Replace(":", "$");
                string destpath = @"\\" + rhost + @"\" + share;
                Console.WriteLine("[+] Grabbing file from " + uploadPath);
                //Console.WriteLine();
                Console.WriteLine("[+] Uploading to " + destpath);
                File.Copy(uploadPath, destpath);
                Console.WriteLine("[+] File uploaded successfully");
            }
            else
            {
                using (new Impersonation(domain, ruser, rpwd))
                {
                    string share = executionPath.Replace(":", "$");
                    string destpath = @"\\" + rhost + @"\" + share;
                    Console.WriteLine("[+] Grabbing file from " + uploadPath);
                    Console.WriteLine();
                    Console.WriteLine("[+] Uploading to " + destpath);

                    /*
                    Console.WriteLine("ExecutionPath is: " + executionPath);
                    Console.WriteLine("uploadPath is: " + uploadPath);
                    Console.WriteLine("share is: " + share);
                    Console.WriteLine("destpath is is: " + destpath);
                    */


                    File.Copy(uploadPath, destpath);

                    Console.WriteLine("[+] File uploaded successfully");
                }
            }
        }

        public static void delete(string executionPath, string rhost, string username, string password, string domain)
        {
            try
            {
                if (username == "" && password == "")
                {
                    string share = executionPath.Replace(":", "$");
                    string destpath = @"\\" + rhost + @"\" + share;
                    Console.WriteLine("[+] Deleting " + destpath);
                    File.Delete(destpath);
                    Console.WriteLine("[+] File removed successfully");
                    Console.WriteLine();
                }
                else
                {
                    using (new Impersonation(domain, username, password))
                    {
                        string share = executionPath.Replace(":", "$");
                        string destpath = @"\\" + rhost + @"\" + share;
                        Console.WriteLine("[+] Deleting " + destpath);
                        File.Delete(destpath);
                        Console.WriteLine("[+] File removed successfully");
                        Console.WriteLine();
                    }
                }
            }
            catch
            {
                Console.WriteLine("[-] File was not removed.  Please remove manually");
            }
        }

        public static void wmiexec(string rhost, string executionPath, string cmdArgs, string domain, string username, string password)
        {
            object[] myProcess = { executionPath + " " + cmdArgs };

            if (username == "" && password == "")
            {
                ManagementScope myScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", rhost));
                ManagementClass myClass = new ManagementClass(myScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                myClass.InvokeMethod("Create", myProcess);
                Console.WriteLine();
                Console.WriteLine("[+] Process created successfully");
            }
            else
            {
                ConnectionOptions myConnection = new ConnectionOptions();
                string uname = domain + @"\" + username;
                myConnection.Impersonation = ImpersonationLevel.Impersonate;
                myConnection.EnablePrivileges = true;
                myConnection.Timeout = new TimeSpan(0, 0, 30);
                myConnection.Username = uname;
                myConnection.Password = password;
                ManagementScope myScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", rhost), myConnection);
                ManagementClass myClass = new ManagementClass(myScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                myClass.InvokeMethod("Create", myProcess);
                Console.WriteLine("[!] Executing command " + executionPath + " " + cmdArgs + " on:" + rhost);
                //Console.WriteLine();
                Console.WriteLine("[+] Process created successfully");
            }
        }

    }
}