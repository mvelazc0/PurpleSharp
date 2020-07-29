using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace PurpleSharp.Simulations
{
    class Execution
    {
        static public void ExecutePowershellCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1059.001");
            logger.TimestampInfo("Using the command line to execute the technique");
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

        static public void ExecutePowershellNET(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1059.001");
            logger.TimestampInfo("Using the System.Management.Automation .NET namespace to execute the technique");
            try
            {
                PowerShell pstest = PowerShell.Create();
                String script = "UwB0AGEAcgB0AC0AUwBsAGUAZQBwACAALQBzACAAMgAwAA==";
                script = System.Text.Encoding.Unicode.GetString(System.Convert.FromBase64String(script));
                pstest.AddScript(script);
                Collection<PSObject> output = null;
                output = pstest.Invoke();
                logger.TimestampInfo("Succesfully invoked a PowerShell script using .NET");
                logger.SimulationFinished();

            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        static public void WindowsCommandShell(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1059.003");
            try
            {
                ExecutionHelper.StartProcess("", "cmd.exe /C whoami", logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        static public void ServiceExecution(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1035");
            try
            {
                ExecutionHelper.StartProcess("", "net start UpdaterService", logger);
                ExecutionHelper.StartProcess("", "sc start UpdaterService", logger);

                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        static public void VisualBasic(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1059.005");
            
            try
            {
                string file = "invoice0420.vbs";
                ExecutionHelper.StartProcess("", String.Format("wscript.exe {0}", file), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        static public void JScript(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1059.007");

            try
            {
                string file = "invoice0420.js";
                ExecutionHelper.StartProcess("", String.Format("wscript.exe {0}", file), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

    }
}
