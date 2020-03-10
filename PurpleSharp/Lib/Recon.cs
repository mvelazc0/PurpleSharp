using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Lib
{
    class Recon
    {
        public static Process GetHostProcess(bool elevated = false)
        {
            //TODO: need to avoid returning a process with Low integrity level

            //Process chrome = Process.GetProcessesByName("chrome").FirstOrDefault();
            //Process firefox = Process.GetProcessesByName("firefox").FirstOrDefault();
            //Process iexplore = Process.GetProcessesByName("iexplore").LastOrDefault();
            //Process edge = Process.GetProcessesByName("MicrosoftEdge").FirstOrDefault();
            //Process winword = Process.GetProcessesByName("winword").FirstOrDefault();
            //Process excel = Process.GetProcessesByName("excel").FirstOrDefault();
            //Process outlook = Process.GetProcessesByName("outlook").FirstOrDefault();
            Process explorer = Process.GetProcessesByName("explorer").FirstOrDefault();

            //if (chrome != null) return chrome;
            //else if (firefox != null) return firefox;
            //else if (iexplore != null) return iexplore;
            //else if (edge != null) return edge;
            //else if (winword != null) return excel;
            //else if (excel != null) return excel;
            //else if (outlook != null) return outlook;
            if (explorer != null) return explorer;
            else return null;

        }


        //https://codereview.stackexchange.com/questions/68076/user-logged-onto-windows
        public static string GetProcessOwner(int processId)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    // return DOMAIN\user
                    return argList[1] + "\\" + argList[0];
                }
            }

            return "NO OWNER";
        }
    }
}
