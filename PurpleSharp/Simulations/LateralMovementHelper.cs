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

        private static string DateTimetoUTC(DateTime dateParam)
        {
            string buffer = dateParam.ToString("********HHmmss.ffffff");
            TimeSpan tickOffset = TimeZone.CurrentTimeZone.GetUtcOffset(dateParam);
            buffer += (tickOffset.Ticks >= 0) ? '+' : '-';
            buffer += (Math.Abs(tickOffset.Ticks) / System.TimeSpan.TicksPerMinute).ToString("d3");
            return buffer;
        }

        public static void CreateRemoteScheduledTaskWmi(Computer computer, PlaybookTask playbook_task, Logger logger)
        {

            string target = "";
            if (!computer.Fqdn.Equals("")) target = computer.Fqdn;
            else if (!computer.ComputerName.Equals("")) target = computer.ComputerName;
            else target = computer.IPv4;

            try
            {
                ConnectionOptions connectoptions = new ConnectionOptions();
                connectoptions.EnablePrivileges = true;



                //var processToRun = new[] { playbook_task.command };
                object[] cmdParams = { playbook_task.command, DateTimetoUTC(DateTime.Now.AddMinutes(1)), false, null, null, true, 0 };
                var wmiScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", target), connectoptions);

                wmiScope.Connect();
                Console.WriteLine("Connected");

                var wmiScheduledJob = new ManagementClass(wmiScope, new ManagementPath("Win32_ScheduledJob"), new ObjectGetOptions());

                ManagementBaseObject inParams = wmiScheduledJob.GetMethodParameters("Create");
                string StartTime = DateTimetoUTC(DateTime.Now.AddMinutes(1));
                Console.WriteLine(StartTime);
                inParams["StartTime"] = "20101129105409.000000+330";
                inParams["Command"] = "notepad.exe";
                //inParams["InteractWithDesktop"] = true;
                //inParams["RunRepeatedly"] = true;
                //inParams["Caption"] = "Suspicious ScheduledTask";
                ManagementBaseObject outParams = wmiScheduledJob.InvokeMethod("Create", inParams, null);
                Console.WriteLine("JobId: " + outParams["JobId"]);
                Console.ReadKey();


                //wmiScheduledJob.InvokeMethod("Create", cmdParams);
                logger.TimestampInfo(String.Format("Successfully created a scheduled task remotely {0} using WMI's Win32_ScheduledJob on {1}", playbook_task.command, target));
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

        // Based on https://www.programmersought.com/article/80103808764/
        public static void CreateRemoteScheduledTaskCmdline(Computer computer, PlaybookTask playbook_task, Logger logger)
        {
            string target = "";
            if (!computer.Fqdn.Equals("")) target = computer.Fqdn;
            else if (!computer.ComputerName.Equals("")) target = computer.ComputerName;
            else target = computer.IPv4;

            string creatTaskArg = string.Format(@"/create /s {0} /sc ONCE /st 10:00 /tn ""{1}"" /tr {2} /rl HIGHEST /ru Local /IT", target, playbook_task.taskName, playbook_task.taskPath);
            string runTaskArg = string.Format(@"/run /s {0} /tn ""{1}""", target, playbook_task.taskName); ;
            string deleteTaskArg = string.Format(@"/delete /s {0} /tn ""{1}"" /F", target, playbook_task.taskName);
            ExecutionHelper.StartProcessNET("schtasks.exe", creatTaskArg, logger);
            ExecutionHelper.StartProcessNET("schtasks.exe", runTaskArg, logger);

            if (playbook_task.cleanup) ExecutionHelper.StartProcessNET("schtasks.exe", deleteTaskArg, logger);



        }

    }
}
