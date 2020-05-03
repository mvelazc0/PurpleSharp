using System;
using System.Management;
using System.Threading;

namespace PurpleSharp.Simulations
{
    class Persistence
    {
        public static void CreateAccountApi(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1136");
            //logger.TimestampInfo(String.Format("Starting T1136 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            try 
            {
                PersistenceHelper.CreateUser("haxor", logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void CreateAccountCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1136");
            //logger.TimestampInfo(String.Format("Starting T1136 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));

            try
            {
                string username = "haxor";
                string pwd = "Passw0rd123El7";
                ExecutionHelper.StartProcess("", String.Format("net user {0} {1} /add", username, pwd), logger);
                System.Threading.Thread.Sleep(2000);
                ExecutionHelper.StartProcess("", String.Format("net user {0} /delete", username), logger, true);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void RegistryRunKeyNET(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1060");
            //logger.TimestampInfo(String.Format("Starting T1060 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            try
            {
                PersistenceHelper.RegistryRunKey(logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
            
        }

        public static void RegistryRunKeyCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1060");
            //logger.TimestampInfo(String.Format("Starting T1060 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));

            try
            {
                string regKey = "BadApp";
                string binpath = @"C:\Windows\Temp\xyz12345.exe";

                ExecutionHelper.StartProcess("", String.Format(@"REG ADD HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run /V {0} /t REG_SZ /F /D {1}", regKey, binpath), logger);
                System.Threading.Thread.Sleep(3000);
                ExecutionHelper.StartProcess("", String.Format(@"REG DELETE HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run /V {0} /F", regKey), logger, true);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }

            
        }

        public static void CreateServiceApi(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1050");
            //logger.TimestampInfo(String.Format("Starting T1050 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));

            try
            {
                logger.TimestampInfo("Creating a service using CreateService Win32API");
                PersistenceHelper.CreateService(log, logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }

            
        }
        public static void CreateServiceCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1050");
            //logger.TimestampInfo(String.Format("Starting T1050 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));

            try
            {
                string serviceName = "UpdaterService";
                string servicePath = @"C:\Windows\Temp\superlegit.exe";
                logger.TimestampInfo("Creating a service using 'sc screate'");
                ExecutionHelper.StartProcess("", String.Format(@"sc create {0} binpath= {1} type= own start= auto", serviceName, servicePath), logger);
                System.Threading.Thread.Sleep(3000);
                ExecutionHelper.StartProcess("", String.Format(@"sc delete {0}", serviceName), logger, true);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }  
        }

        public static void WMIEventSubscription(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1084");
            //string vbscript64 = "<INSIDE base64 encoded VBS here>";
            //string vbscript = Encoding.UTF8.GetString(Convert.FromBase64String(vbscript64));
            try
            {
                ManagementObject EventFilter = null;
                ManagementObject EventConsumer = null;
                ManagementObject myBinder = null;

                ManagementScope scope = new ManagementScope(@"\\.\root\subscription");

                ManagementClass wmiEventFilter = new ManagementClass(scope, new
                ManagementPath("__EventFilter"), null);
                String strQuery = @"SELECT * FROM __InstanceCreationEvent WITHIN 5 " + "WHERE TargetInstance ISA \"Win32_Process\" " + "AND TargetInstance.Name = \"notepad.exe\"";

                WqlEventQuery myEventQuery = new WqlEventQuery(strQuery);
                EventFilter = wmiEventFilter.CreateInstance();
                EventFilter["Name"] = "MaliciousSubscription";
                EventFilter["Query"] = myEventQuery.QueryString;
                EventFilter["QueryLanguage"] = myEventQuery.QueryLanguage;
                EventFilter["EventNameSpace"] = @"\root\cimv2";
                EventFilter.Put();
                logger.TimestampInfo("Event filter 'MaliciousSubscription' created.");

                EventConsumer = new ManagementClass(scope, new ManagementPath("CommandLineEventConsumer"), null).CreateInstance();
                EventConsumer["Name"] = "MaliciousSubscription";
                EventConsumer["CommandLineTemplate"] = "powershell.exe";
                EventConsumer.Put();

                /*
                EventConsumer = new ManagementClass(scope, new ManagementPath("ActiveScriptEventConsumer"), null).CreateInstance();
                EventConsumer["Name"] = "BadActiveScriptEventConsumer";
                EventConsumer["ScriptingEngine"] = "VBScript";
                EventConsumer["ScriptText"] = vbscript;
                EventConsumer.Put();
                */

                logger.TimestampInfo("Event consumer 'MaliciousSubscription' created.");

                myBinder = new ManagementClass(scope, new ManagementPath("__FilterToConsumerBinding"), null).CreateInstance();
                myBinder["Filter"] = EventFilter.Path.RelativePath;
                myBinder["Consumer"] = EventConsumer.Path.RelativePath;
                myBinder.Put();

                logger.TimestampInfo("WMI Subscription created successfully");
                Thread.Sleep(3 * 1000);
                EventFilter.Delete();
                EventConsumer.Delete();
                myBinder.Delete();
                logger.TimestampInfo("WMI Subscription Deleted");
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
                
            } 
        }
    }
}
