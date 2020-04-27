using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace PurpleSharp.Simulations
{
    public class PersistenceHelper
    {
        public void Persistence_SchTask()
        {
            /*
            using (var ts = new TaskService())
            {
                var task = ts.NewTask();
                task.Actions.Add(new ExecAction("cmd.exe",
                    string.Format("/c net stop {0} & net start {0}", serviceName)));
                task.Triggers.Add(
                    new DailyTrigger() { StartBoundary = DateTime.Today.AddHours(19) });
                task.Principal.RunLevel = TaskRunLevel.Highest;
                ts.RootFolder.RegisterTaskDefinition(serviceName + " Restart", task);
            }*/

        }

        public static void RegistryRunKey(Lib.Logger logger)
        {
            try
            {
                RegistryKey registryKey1 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                //RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true);
                registryKey1.SetValue("BadApp", @"C:\Windows\Temp\xyz123456.exe");
                logger.TimestampInfo(@"Created Regkey: HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run - C:\Windows\Temp\xyz123456.exe ");
                //registryKey2.SetValue("BadApp", @"C:\Windows\Temp\xyz123456.exe");
                //logger.TimestampInfo(@"Created Regkey: HKCU\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce - C:\Windows\Temp\xyz123456.exe ");
                registryKey1.DeleteValue("BadApp");
                logger.TimestampInfo(@"Deleted : HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                //registryKey2.DeleteValue("BadApp");
                //logger.TimestampInfo(@"Deleted: HKCU\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce");

            }
            catch ( Exception ex)
            {
                logger.TimestampInfo("Error Creating Registry keys");
            }

        }

        public static void CreateUser(String username, Lib.Logger logger)
        {
            //https://stackoverflow.com/questions/1100926/pinvoke-of-netuseradd-returns-24
            Structs.USER_INFO_2 userInfo2 = new Structs.USER_INFO_2()
            {
                acct_expires = long.MaxValue,
                auth_flags = 0, // Must be 0 for NetUserAddCalls
                bad_pw_count = -1,  //ignored for NetUserAdd calls
                //code_page = ?,
                comment = "H4x0r Account",
                //country_code = ?,
                flags = 0x10000,// & UF_ACCOUNTDISABLE,
                full_name = "h4x0r",
                home_dir = "",
                last_logoff = 0,
                last_logon = 0,
                logon_hours = IntPtr.Zero, // User is given no logon time.
                logon_server = "", //ignored for NetUserAdd calls
                max_storage = 0,
                name = username,
                num_logons = -1, //ignored for NetUserAdd calls
                parms = "",
                password = "Passw0rd123$%^Elite98765432102",
                password_age = -1,
                priv = 1,
                script_path = "",
                units_per_week = -1, //ignored for NetUserAdd calls
                usr_comment = "",
                workstations = ""
            };

            //string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            //Lib.Logger logger = new Lib.Logger(currentPath + log );
            //logger.TimestampInfo(String.Format("Starting Create Account Simulation on {0}", Environment.MachineName));

            uint output;
            int result = WinAPI.NetUserAdd(null, 2, userInfo2, out output);
            if (result == 0)
            {
                //Console.WriteLine("Successfully created local user");
                logger.TimestampInfo(String.Format("Successfully created local user {0} with NetUserAdd", username));
            }
            else
            {
                //Console.WriteLine("Could not create user");
                logger.TimestampInfo(String.Format("Could not create local user {0}. Error code: {1} ", username, result.ToString()));
            }

            //#TODO Look addmin group with LookupAccountSid: https://www.pinvoke.net/default.aspx/netapi32.netlocalgroupaddmembers
            /*
            Structs.LOCALGROUP_MEMBERS_INFO_3 info;
            info.Domain = username;
            int result2 = WinAPI.NetLocalGroupAddMembers(null, "Administrators", 3, ref info, 1);
            if (result2 == 0)
            {
                //Console.WriteLine("Successfully added created user to the Administrators group");
                logger.TimestampInfo("Successfully added created user to the Administrators group");
            }
            */
            System.Threading.Thread.Sleep(4000);
            if (result == 0)
            {
                int result3 = WinAPI.NetUserDel(null, username);
                if (result3 == 0)
                {
                    logger.TimestampInfo(String.Format("Successfully removed user with NetUserDel", username));
                    
                }
                else
                {
                    //Console.WriteLine("Could not delete user");
                    logger.TimestampInfo("Could not delete user");
                    //Console.WriteLine(result3);
                }
            }

        }

        // From https://stackoverflow.com/questions/23481394/programmatically-install-windows-service-on-remote-machine
        static bool CreateService(IntPtr scmHandle, string servicePath, string serviceName, string serviceDispName, out IntPtr serviceHandleCreated, out int errorCodeIfFailed)
        {
            serviceHandleCreated = IntPtr.Zero;
            errorCodeIfFailed = 0;

            serviceHandleCreated = WinAPI.CreateService(
                scmHandle,
                serviceName,
                serviceDispName,
                Structs.SERVICE_ACCESS.SERVICE_ALL_ACCESS,
                Structs.SERVICE_TYPES.SERVICE_WIN32_OWN_PROCESS,
                Structs.SERVICE_START_TYPES.SERVICE_AUTO_START,
                Structs.SERVICE_ERROR_CONTROL.SERVICE_ERROR_NORMAL,
                servicePath,
                null,
                IntPtr.Zero,
                null,
                null,
                null);

            if (serviceHandleCreated == IntPtr.Zero)
            {
                errorCodeIfFailed = Marshal.GetLastWin32Error();
            }

            return serviceHandleCreated != IntPtr.Zero;
        }

        public static void CreateService(string log, Lib.Logger logger)
        {

            //string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            //Lib.Logger logger = new Lib.Logger(currentPath + log);
            //logger.TimestampInfo(String.Format("Starting Create Service Simulation on {0}", Environment.MachineName));

            var scmHandle = WinAPI.OpenSCManager(null, null, Structs.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE);

            if (scmHandle == IntPtr.Zero)
            {
                DateTime dtime = DateTime.Now;
                int err = Marshal.GetLastWin32Error();
                //Console.WriteLine("{0}[{1}] Could not obtain a handle to SCM on {2}. Not an admin ?", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn);
                return;

            }
            string servicePath = @"C:\Windows\Temp\superlegit.exe";      // A path to some running service now
            string serviceName = "UpdaterService";
            string serviceDispName = "Super Legit Update Service";

            IntPtr svcHandleCreated = IntPtr.Zero;
            int createdErr = 0;
            bool created = CreateService(scmHandle, servicePath, serviceName, serviceDispName, out svcHandleCreated, out createdErr);

            if (created)
            {
                //DateTime dtime = DateTime.Now;
                logger.TimestampInfo(String.Format("Successfully created Service: {0} ImagePath: {1} using CreateService", serviceName, servicePath));
                //Console.WriteLine("{0}[{1}] Successfully created a service on {2}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn);
                IntPtr svcHandleOpened = WinAPI.OpenService(scmHandle, serviceName, Structs.SERVICE_ACCESS.SERVICE_ALL_ACCESS);
                bool deletedService = WinAPI.DeleteService(svcHandleOpened);
                logger.TimestampInfo(String.Format("Deleted Service: {0} ImagePath: {1} with DeleteService", serviceName, servicePath));
                WinAPI.CloseServiceHandle(svcHandleOpened);
            }
            else
            {
                logger.TimestampInfo("Could not create Service. Error Code: " + createdErr);
            }
            /*
            if (!created)
            {
                if (createdErr == 1073)
                {
                    // Error: "The specified service already exists"

                    IntPtr svcHandleOpened = WinAPI.OpenService(scmHandle, serviceName, Structs.SERVICE_ACCESS.SERVICE_ALL_ACCESS);

                    if (svcHandleOpened != IntPtr.Zero)
                    {
                        bool deletedService = WinAPI.DeleteService(svcHandleOpened);
                        WinAPI.CloseServiceHandle(svcHandleOpened);

                        if (deletedService)
                        {
                            // Try to create it again:
                            bool created2 = CreateService(scmHandle, servicePath, serviceName, serviceDispName, out svcHandleCreated, out createdErr);
                            if (created2)
                            {
                                DateTime dtime = DateTime.Now;
                                Console.WriteLine("{0}[{1}] Successfully deleted and recreated a service on {2}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn);
                                //throw new Win32Exception(createdErr);

                                if (cleanup)
                                {
                                    IntPtr svcHandleOpened2 = WinAPI.OpenService(scmHandle, serviceName, Structs.SERVICE_ACCESS.SERVICE_ALL_ACCESS);
                                    bool deletedService2 = WinAPI.DeleteService(svcHandleOpened2);
                                    WinAPI.CloseServiceHandle(svcHandleOpened2);

                                }

                            }
                        }
                        else
                        {
                            DateTime dtime = DateTime.Now;
                            Console.WriteLine("{0}[{1}] Failed to create service on {2}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn);

                            // Service was successfully opened, but unable to delete the service
                        }
                    }
                    else
                    {
                        // Unable to open that service name w/ All Access
                        DateTime dtime = DateTime.Now;
                        Console.WriteLine("{0}[{1}] Failed to create service on {2}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn);
                        int openErr = Marshal.GetLastWin32Error();
                        //throw new Win32Exception(openErr);
                    }

                }
                else
                {
                    // Some other serice creation error than it already existing
                    DateTime dtime = DateTime.Now;
                    Console.WriteLine("{0}[{1}] Failed to create service on {2}. ", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn);
                    //throw new Win32Exception(createdErr);
                }
            }
            */
        }
    }
}