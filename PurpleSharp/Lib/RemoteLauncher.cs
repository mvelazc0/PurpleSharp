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

    //Most of this is based on SharpExec (https://github.com/anthemtotheego/SharpExec)
    class RemoteLauncher
    {
        public static string readFile(string rhost, string path, string ruser, string rpwd, string domain)
        {
            string txt = "";
            path = @"\\"+rhost+"\\"+path.Replace(":", "$");
            using (new Impersonation(domain, ruser, rpwd))
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs);
                txt = sr.ReadToEnd();
                sr.Close();

            }
            return txt;
        }


        public static void upload(string uploadPath, string executionPath, string rhost, string ruser, string rpwd, string domain)
        {
            string share = executionPath.Replace(":", "$");
            string destpath = @"\\" + rhost + @"\" + share;
            try
            {

                if (ruser == "" && rpwd == "")
                {

                    //string share = executionPath.Replace(":", "$");
                    //string destpath = @"\\" + rhost + @"\" + share;
                    //Console.WriteLine("[+] Grabbing file from " + uploadPath);
                    Console.WriteLine("[+] Uploading PurpleSharp to " + destpath);
                    File.Copy(uploadPath, destpath);
                    Console.WriteLine("[+] File uploaded successfully");
                }
                else
                {
                    using (new Impersonation(domain, ruser, rpwd))
                    {
                        //string share = executionPath.Replace(":", "$");
                        //string destpath = @"\\" + rhost + @"\" + share;
                        //Console.WriteLine("[+] Grabbing file from " + uploadPath);
                        Console.WriteLine("[+] Uploading PurpleSharp to " + destpath);

                        /*
                        Console.WriteLine("ExecutionPath is: " + executionPath);
                        Console.WriteLine("uploadPath is: " + uploadPath);
                        Console.WriteLine("share is: " + share);
                        Console.WriteLine("destpath is is: " + destpath);
                        */


                        File.Copy(uploadPath, destpath);


                        //Console.WriteLine("[+] File uploaded successfully");
                    }
                }
            }
            catch
            {
                RemoteLauncher.delete(destpath, rhost, ruser, rpwd, domain);
                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("[+] Overwiring existing PurpleSharp binary..." + destpath);
                using (new Impersonation(domain, ruser, rpwd)) { File.Copy(uploadPath, destpath); }
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
                    //Console.WriteLine("[+] File removed successfully");
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
                        //Console.WriteLine("[+] File removed successfully");
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
                Console.WriteLine("[+] Executing command: " + executionPath + " " + cmdArgs + " On:" + rhost);
                myClass.InvokeMethod("Create", myProcess);
                Console.WriteLine("[!] Process created successfully");
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
                Console.WriteLine("[+] Executing command " + executionPath + " " + cmdArgs + " on:" + rhost);
                myClass.InvokeMethod("Create", myProcess);
                //Console.WriteLine("[!] Process created successfully");
            }
        }

    }
}