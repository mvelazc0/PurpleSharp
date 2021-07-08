using PurpleSharp.Lib;
using System;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using TaskScheduler;

namespace PurpleSharp.Simulations
{
    class LateralMovementHelper
    {

        // From https://stackoverflow.com/questions/23481394/programmatically-install-windows-service-on-remote-machine
        // and https://stackoverflow.com/questions/37983453/how-to-deploy-windows-service-on-remote-system-using-c-sharp-programatically
        public static void CreateRemoteServiceApi_Old(Computer computer, bool cleanup, Lib.Logger logger)
        {
            //var scmHandle = WinAPI.OpenSCManager(computer.Fqdn, null, Structs.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE);
            var scmHandle = WinAPI.OpenSCManager(computer.ComputerName, null, Structs.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE);

            if (scmHandle == IntPtr.Zero)
            {
                DateTime dtime = DateTime.Now;
                int err = Marshal.GetLastWin32Error();
                logger.TimestampInfo(String.Format("Could not obtain a handle to the Service Control Manager on {0}.", computer.Fqdn));
                throw new ArgumentException("Could not obtain a handle to SCM", "Error");

            }
            string servicePath = @"C:\Windows\Temp\superlegit.exe";      // A path to some running service now
            string serviceName = "UpdaterService";
            string serviceDispName = "Super Legit Update Service";

            IntPtr svcHandleCreated = IntPtr.Zero;
            int createdErr = 0;
            bool created = CreateService(scmHandle, servicePath, serviceName, serviceDispName, out svcHandleCreated, out createdErr);

            if (created)
            {
                DateTime dtime = DateTime.Now;
                logger.TimestampInfo(String.Format("Created service '{0}' on {1} with 'CreateService' Win32 API", serviceName, computer.ComputerName));

                if (cleanup)
                {
                    IntPtr svcHandleOpened = WinAPI.OpenService(scmHandle, serviceName, Structs.SERVICE_ACCESS.SERVICE_ALL_ACCESS);
                    bool deletedService = WinAPI.DeleteService(svcHandleOpened);
                    logger.TimestampInfo(String.Format("Deleted service '{0}' on {1} with 'DeleteService' Win32API", serviceName, computer.ComputerName));
                    WinAPI.CloseServiceHandle(svcHandleOpened);
                }
                else
                {
                    logger.TimestampInfo(String.Format("The created Service: {0} was not deleted on {1} as part of the simulation", serviceName, computer.ComputerName));
                }
            }
            
            else
            {
                // service was not created
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
            

            WinAPI.StartService(svcHandleCreated, 0, null);


            WinAPI.CloseServiceHandle(svcHandleCreated);
            WinAPI.CloseServiceHandle(scmHandle);
        }

        // From https://stackoverflow.com/questions/23481394/programmatically-install-windows-service-on-remote-machine
        // and https://stackoverflow.com/questions/37983453/how-to-deploy-windows-service-on-remote-system-using-c-sharp-programatically
        public static void CreateRemoteServiceApi(Computer computer, PlaybookTask playbook_task, Logger logger)
        {
            var scmHandle = IntPtr.Zero;
            int createdErr = 0;

            if (!computer.Fqdn.Equals("")) scmHandle = WinAPI.OpenSCManager(computer.Fqdn, null, Structs.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE);
            else if (!computer.ComputerName.Equals("")) scmHandle = WinAPI.OpenSCManager(computer.ComputerName, null, Structs.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE);
            else scmHandle = WinAPI.OpenSCManager(computer.IPv4, null, Structs.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE);

            if (scmHandle == IntPtr.Zero)
            {
                createdErr = Marshal.GetLastWin32Error();
                logger.TimestampInfo(String.Format("Could not obtain a handle to the Service Control Manager on {0}.", computer.Fqdn));
                throw new Win32Exception(createdErr);

            }

            IntPtr svcHandleCreated = IntPtr.Zero; 
            bool created = CreateService(scmHandle, playbook_task.servicePath, playbook_task.serviceName, playbook_task.serviceName, out svcHandleCreated, out createdErr); ;

            if (created)
            {
                logger.TimestampInfo(String.Format("Created service '{0}' on {1} with 'CreateService' Win32 API", playbook_task.serviceName, computer.ComputerName));

                WinAPI.StartService(svcHandleCreated, 0, null);
                logger.TimestampInfo(String.Format("Service '{0}' started on {1} with 'StartService' Win32 API", playbook_task.serviceName, computer.ComputerName));

                if (playbook_task.cleanup)
                {
                    IntPtr svcHandleOpened = WinAPI.OpenService(scmHandle, playbook_task.serviceName, Structs.SERVICE_ACCESS.SERVICE_ALL_ACCESS);
                    bool deletedService = WinAPI.DeleteService(svcHandleOpened);
                    logger.TimestampInfo(String.Format("Deleted service '{0}' on {1} with 'DeleteService' Win32API", playbook_task.serviceName, computer.ComputerName));
                    WinAPI.CloseServiceHandle(svcHandleOpened);
                }
                else
                {
                    logger.TimestampInfo(String.Format("The created Service: {0} was not deleted on {1} as part of the simulation", playbook_task.serviceName, computer.ComputerName));
                }
            }

            else
            {
                // service was not created

                if (createdErr == 1073)
                {
                    // Error: "The specified service already exists"
                    logger.TimestampInfo(String.Format("Failed to create service {0} on {1}. Service already exists", playbook_task.serviceName, computer.ComputerName));

                }
                else
                {
                    // Some other serice creation error
                    logger.TimestampInfo(String.Format("Failed to create service {0} on {1}.", playbook_task.serviceName, computer.ComputerName));
                    throw new Win32Exception(createdErr);
                }

            }
            WinAPI.CloseServiceHandle(svcHandleCreated);
            WinAPI.CloseServiceHandle(scmHandle);
        }

        //Based on https://github.com/Mr-Un1k0d3r/SCShell
        public static void ModifyRemoteServiceApi(Computer computer, PlaybookTask playbook_task, Logger logger)
        {
            var scmHandle = IntPtr.Zero;
            int createdErr = 0;
            int bytesNeeded = 0;
            IntPtr qscPtr = IntPtr.Zero;


            if (!computer.Fqdn.Equals("")) scmHandle = WinAPI.OpenSCManager(computer.Fqdn, null, Structs.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE);
            else if (!computer.ComputerName.Equals("")) scmHandle = WinAPI.OpenSCManager(computer.ComputerName, null, Structs.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE);
            else scmHandle = WinAPI.OpenSCManager(computer.IPv4, null, Structs.SCM_ACCESS.SC_MANAGER_CREATE_SERVICE);

            if (scmHandle == IntPtr.Zero)
            {
                createdErr = Marshal.GetLastWin32Error();
                logger.TimestampInfo(String.Format("Could not obtain a handle to the Service Control Manager on {0}.", computer.Fqdn));
                throw new Win32Exception(createdErr);
            }
            if (!playbook_task.serviceName.Equals("random"))
            {
                IntPtr svcHandleOpened = WinAPI.OpenService(scmHandle, playbook_task.serviceName, Structs.SERVICE_ACCESS.SERVICE_ALL_ACCESS);
                if (svcHandleOpened == IntPtr.Zero)
                {
                    logger.TimestampInfo(String.Format("Could not obtain a handle to remote Service {0}.", playbook_task.serviceName));
                    throw new Win32Exception(createdErr);
                }
                Structs.QueryServiceConfig qscs = new Structs.QueryServiceConfig();
                int retCode = WinAPI.QueryServiceConfig(svcHandleOpened, qscPtr, 0, ref bytesNeeded);
                if (retCode == 0  && bytesNeeded == 0)
                {
                    throw new Win32Exception();
                }
                
                qscPtr = Marshal.AllocCoTaskMem((int)bytesNeeded);
                retCode = WinAPI.QueryServiceConfig(svcHandleOpened, qscPtr, bytesNeeded, ref bytesNeeded);
                logger.TimestampInfo(String.Format("Got handle for remote Service {0}.", playbook_task.serviceName));
                qscs = (Structs.QueryServiceConfig)Marshal.PtrToStructure(qscPtr, new Structs.QueryServiceConfig().GetType());
                string originalBinaryPath = Marshal.PtrToStringAuto(qscs.binaryPathName);
                logger.TimestampInfo("Original binary path " + originalBinaryPath);

                bool serviceChanged = WinAPI.ChangeServiceConfig(svcHandleOpened, 0xFFFFFFFF, 0x00000003, 0, playbook_task.servicePath, null, null, null, null, null, null);
                if (!serviceChanged)
                {
                    logger.TimestampInfo(String.Format("Could not modify remote Service '{0}'.", playbook_task.serviceName));
                    throw new Win32Exception(createdErr);
                }
                logger.TimestampInfo(String.Format("Succesfully modified remote Service '{0}' using ChangeServiceConfig.", playbook_task.serviceName));

                WinAPI.StartService(svcHandleOpened, 0, null);
                logger.TimestampInfo(String.Format("Service '{0}' started  with new ServicePath {1}", playbook_task.serviceName, playbook_task.servicePath));
                Thread.Sleep(3000);

                serviceChanged = WinAPI.ChangeServiceConfig(svcHandleOpened, 0xFFFFFFFF, 0x00000003, 0, originalBinaryPath, null, null, null, null, null, null);
                if (serviceChanged)
                {
                    logger.TimestampInfo(String.Format("Restored remote Service '{0}' to the original path.", playbook_task.serviceName));
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

        public static void WinRMCodeExecution(Computer computer, PlaybookTask playbook_task, Logger logger)
        {

            string target = "";
            if (!computer.Fqdn.Equals("")) target = computer.Fqdn;
            else if (!computer.ComputerName.Equals("")) target = computer.ComputerName;
            else target = computer.IPv4;

            try
            {
                var connectTo = new Uri(String.Format("http://{0}:5985/wsman", target));
                logger.TimestampInfo(String.Format("Connecting to http://{0}:5985/wsman", target));
                var connection = new WSManConnectionInfo(connectTo);
                var runspace = RunspaceFactory.CreateRunspace(connection);
                runspace.Open();
                using (var powershell = PowerShell.Create())
                {
                    powershell.Runspace = runspace;
                    powershell.AddScript(playbook_task.command);
                    var results = powershell.Invoke();
                    runspace.Close();
                    logger.TimestampInfo(String.Format("Successfully executed {0} using WinRM on {1}", playbook_task.command, computer.ComputerName));

                    /*
                    Console.WriteLine("Return command ");
                    foreach (var obj in results.Where(o => o != null))
                    {
                        Console.WriteLine("\t" + obj);
                    }
                    */
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Access is denied"))
                {
                    logger.TimestampInfo(String.Format("Failed to start a process using WinRM on {0}. (Access Denied)", computer.Fqdn));
                    throw new Exception();
                }
                else if (ex.GetType().ToString().Contains("PSRemotingTransportException"))
                {
                    logger.TimestampInfo(String.Format("Failed to start a process using WinRM on {0}. (Connection Issues)", computer.Fqdn));
                    throw new Exception();
                }
                else
                {
                    logger.TimestampInfo(String.Format("Failed to start a process using WinRM on {0}. {1}", computer.Fqdn, ex.GetType()));
                    throw new Exception();
                }
            }
        }

        public static void WmiCodeExecution(Computer computer, PlaybookTask playbook_task, Logger logger)
        {
            string target = "";
            if (!computer.Fqdn.Equals("")) target = computer.Fqdn;
            else if (!computer.ComputerName.Equals("")) target = computer.ComputerName;
            else target = computer.IPv4;


            try
            {
                ConnectionOptions connectoptions = new ConnectionOptions();

                var processToRun = new[] { playbook_task.command };
                var wmiScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", target), connectoptions);
                var wmiProcess = new ManagementClass(wmiScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
                wmiProcess.InvokeMethod("Create", processToRun);
                logger.TimestampInfo(String.Format("Successfully executed {0} using WMI's Win32_Process on {1}", playbook_task.command, target));
            }
            catch (Exception ex)
            {
                //DateTime dtime = DateTime.Now;
                if (ex.Message.Contains("ACCESSDENIED"))
                {
                    logger.TimestampInfo(String.Format("Failed to start a process using WMI Win32_Create on {0}. (Access Denied)", target));
                }
                else
                {
                    logger.TimestampInfo(String.Format("Failed to start a process using WMI Win32_Create on {0}. {1}", target, ex.GetType()));
                }
            }
        }

        public static void CreateRemoteScheduledTask(Computer computer, string command, bool cleanup)
        {
            try
            {
                /*
                ConnectionOptions connectoptions = new ConnectionOptions();
                var wmiScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", computer.Fqdn), connectoptions);
                wmiScope.Connect();

                //Getting time
                string serverTime = null;
                SelectQuery timeQuery = new SelectQuery(@"select LocalDateTime from Win32_OperatingSystem");
                ManagementObjectSearcher timeQuerySearcher = new ManagementObjectSearcher(timeQuery);
                foreach (ManagementObject mo in timeQuerySearcher.Get())
                {
                    serverTime = mo["LocalDateTime"].ToString();
                }

                //Adding 2 minutes to the time
                Console.WriteLine("Got Remote computer time {0}", serverTime);

                //running command
                object[] cmdParams = { command, serverTime, false, null, null, true, 0 };
                ManagementClass serverCommand = new ManagementClass(wmiScope, new ManagementPath("Win32_ScheduledJob"), null);
                serverCommand.InvokeMethod("Create", cmdParams);
                DateTime dtime = DateTime.Now;
                Console.WriteLine("{0}[{1}] Successfully created a process using WMI on {2}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn);
                */
                
                /*
                string strJobId = "";
                ConnectionOptions connectoptions = new ConnectionOptions();
                connectoptions.Impersonation = ImpersonationLevel.Impersonate;
                connectoptions.Authentication = AuthenticationLevel.PacketPrivacy;
                connectoptions.EnablePrivileges = true;
                Console.WriteLine(computer.Fqdn);
                
                var wmiScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", computer.Fqdn), connectoptions);

                //ManagementScope manScope = new ManagementScope(computer.Fqdn, connOptions);
                

                wmiScope.Connect();
                ObjectGetOptions objectGetOptions = new ObjectGetOptions();
                ManagementPath managementPath = new ManagementPath("Win32_ScheduledJob");
                ManagementClass processClass = new ManagementClass(wmiScope, managementPath, objectGetOptions);


                var processToRun = new[] { command };
                */

                /*
                ManagementBaseObject inParams = processClass.GetMethodParameters("Create");
                //inParams["Name"] = "TESTER";
                inParams["Owner"] = "Tester";
                inParams["Command"] = "ipconfig.exe";
                //inParams["StartTime"] = "********171000.000000-300";
                */


                /*


                ManagementBaseObject inParams = processClass.GetMethodParameters("Create");
                inParams["Caption"] = "Suspicious ScheduledTask";
                inParams["Command"] = "iponfig.exe";
                //inParams["TaskName"] = "TESTER";
                string StartTime = DateTime.Now.AddMinutes(1).ToUniversalTime().ToString();
                inParams["StartTime"] = "********171000.000000-300";

                //Console.WriteLine("got this far #1");
                var wmiProcess = new ManagementClass(wmiScope, new ManagementPath("Win32_ScheduledJob"), new ObjectGetOptions());
                //Console.WriteLine("got this far #2");
                ManagementBaseObject outParams = processClass.InvokeMethod("Create", inParams, null);
                //Console.WriteLine("got this far #3");
                //wmiProcess.InvokeMethod("Create", processToRun);


                
                strJobId = outParams["JobId"].ToString();
                Console.WriteLine("Out parameters:");
                Console.WriteLine("JobId: " + outParams["JobId"]);

                Console.WriteLine("ReturnValue: " + outParams["ReturnValue"]);
                

                DateTime dtime = DateTime.Now;
                Console.WriteLine("{0}[{1}] Successfully created a scheduled Task using WMI on {2}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn);
                */

                /*
                string strJobId;
                int DaysOfMonth = 0;
                int DaysOfWeek = 0;
                ManagementClass classInstance = new ManagementClass(String.Format("\\\\{0}\\root\\cimv2", computer.Fqdn), "Win32_ScheduledJob", null);
                ManagementBaseObject inParams = classInstance.GetMethodParameters("Create");
                inParams["Name"] = "TestTestTest";
                inParams["Command"] = "Notepad.exe";
                inParams["InteractWithDesktop"] = false;
                inParams["RunRepeatedly"] = true;
                if (DaysOfMonth > 0)
                    inParams["DaysOfMonth"] = 0;
                if (DaysOfWeek > 0)
                    inParams["DaysOfWeek"] = 0;
                inParams["StartTime"] = "20101129105409.000000+330";
                ManagementBaseObject outParams = classInstance.InvokeMethod("Create", inParams, null);

                strJobId = outParams["JobId"].ToString();
                Console.WriteLine("Out parameters:");
                Console.WriteLine("JobId: " + outParams["JobId"]);

                Console.WriteLine("ReturnValue: " + outParams["ReturnValue"]);
                */

                /*
                ConnectionOptions connection = new ConnectionOptions();
                var wmiScope = new ManagementScope(string.Format(@"\\{0}\root\CIMV2", computer.Fqdn), connection);
                wmiScope.Connect();
                ObjectGetOptions objectGetOptions = new ObjectGetOptions();
                ManagementPath managementPath = new ManagementPath(path: "Win32_ScheduledJob");

                DateTime currentTime = DateTime.Now;

                ManagementClass classInstance = new ManagementClass(scope: wmiScope, path: managementPath, options: objectGetOptions);
                ManagementBaseObject inParams = classInstance.GetMethodParameters("Create");
                //inParams["Name"] = "TestTest";
                inParams["Command"] = "cmd.exe";
                //inParams["StartTime"] = string.Format("********{0}{1}{2}.000000+{3}", currentTime.Hour, currentTime.Minute, currentTime.Second, 240);
                inParams["InteractWithDesktop"] = true;

                ManagementBaseObject outParams = classInstance.InvokeMethod("Create", inParams, null);
                Console.WriteLine("JobId: " + outParams["jobId"]);
                Console.WriteLine("ReturnValue: " + outParams["returnValue"]);
                Console.ReadKey();
                */


            }
            catch (Exception ex)
            {
                DateTime dtime = DateTime.Now;
                if (ex.Message.Contains("ACCESSDENIED")) Console.WriteLine("{0}[{1}] Failed to execute execute a process using WMI on {2}. (Access Denied)", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn);
                else Console.WriteLine("{0}[{1}] Failed to execute a process using WMI on {2}. {3}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), computer.Fqdn, ex.GetType());
                Console.WriteLine(ex);
            }


        }




    }
}
