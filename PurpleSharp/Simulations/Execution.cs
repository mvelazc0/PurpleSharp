using PurpleSharp.Lib;
using System;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace PurpleSharp.Simulations
{
    class Execution
    {

        static public void ExecuteWmiCmd(string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + log);
            logger.SimulationHeader("T1047");
            logger.TimestampInfo("Using the command line to execute the technique");
            try
            {
                ExecutionHelper.StartProcessNET("wmic.exe", String.Format(@"process call create ""powershell.exe"""), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }
        static public void ExecutePowerShellCmd(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1059.001");
            logger.TimestampInfo("Using the command line to execute the technique");
            try
            {
                ExecutionHelper.StartProcessApi("", String.Format("powershell.exe -command \"{0}\"", playbook_task.commandlet), logger);
                logger.SimulationFinished();
            }
            catch(Exception ex)
            {
                logger.SimulationFailed(ex);
            } 
        }

        static public void ExecutePowerShellCmdEncoded(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1059.001");
            logger.TimestampInfo("Using the command line to execute the technique with Base64 encoding");
            try
            {
                byte[] bytes = System.Text.Encoding.Unicode.GetBytes(playbook_task.commandlet);
                string encodedPwd = Convert.ToBase64String(bytes);
                ExecutionHelper.StartProcessApi("", String.Format("powershell.exe -exec bypass -enc {0}", encodedPwd), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        static public void ExecutePowerShellNET(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1059.001");
            logger.TimestampInfo("Using the System.Management.Automation .NET namespace to execute the technique");
            try
            {
                PowerShell pstest = PowerShell.Create();
                pstest.AddScript(playbook_task.commandlet);
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

        static public void WindowsCommandShell(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1059.003");
            try
            {
                ExecutionHelper.StartProcessApi("", String.Format("cmd.exe /C {0}",playbook_task.command), logger);
                //ExecutionHelper.StartProcessApi("", String.Format("{0}", playbook_task.command), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        static public void ServiceExecution(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1569.002");
            try
            {
                ExecutionHelper.StartProcessApi("", String.Format("net start {0}", playbook_task.serviceName ), logger);
                //ExecutionHelper.StartProcessApi("", String.Format("sc start {0}", playbook_task.serviceName), logger);

                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

        static public void VisualBasic(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1059.005");
            
            try
            {
                ExecutionHelper.StartProcessApi("", String.Format("wscript.exe {0}", playbook_task.file_path), logger);
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
                ExecutionHelper.StartProcessApi("", String.Format("wscript.exe {0}", file), logger);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

    }
}
