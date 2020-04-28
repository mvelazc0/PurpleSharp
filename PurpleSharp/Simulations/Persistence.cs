using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
