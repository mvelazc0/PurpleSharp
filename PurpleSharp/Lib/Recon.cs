using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Lib
{


    class Recon
    {
        const int TOKEN_QUERY = 0x0008;

        const int SECURITY_MANDATORY_UNTRUSTED_RID = (0x00000000);
        const int SECURITY_MANDATORY_LOW_RID = (0x00001000);
        const int SECURITY_MANDATORY_MEDIUM_RID = (0x00002000);
        const int SECURITY_MANDATORY_HIGH_RID = (0x00003000);
        const int SECURITY_MANDATORY_SYSTEM_RID = (0x00004000);
        const int SECURITY_MANDATORY_PROTECTED_PROCESS_RID = (0x00005000);

        const int ERROR_INVALID_PARAMETER = 87;

        public enum IntegrityLevel
        {
            Low, Medium, High, System, None,
        }
        public static Process GetHostProcess(bool elevated = false)
        {
            Process chrome = GetMediumIntegrityProc(Process.GetProcessesByName("chrome"));
            Process firefox = GetMediumIntegrityProc(Process.GetProcessesByName("firefox"));
            Process iexplore = GetMediumIntegrityProc(Process.GetProcessesByName("iexplore"));
            Process winword = GetMediumIntegrityProc(Process.GetProcessesByName("winword"));
            Process outlook = GetMediumIntegrityProc(Process.GetProcessesByName("outlook"));
            Process excel = GetMediumIntegrityProc(Process.GetProcessesByName("excel"));
            Process explorer = Process.GetProcessesByName("explorer").FirstOrDefault();

            if (chrome != null) return chrome;
            else if (firefox != null) return firefox;
            else if (iexplore != null) return iexplore;
            else if (winword != null) return excel;
            else if (excel != null) return excel;
            else if (outlook != null) return outlook;
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


        public static Process GetMediumIntegrityProc(Process[] procs)
        {
            foreach (Process proc in procs)
            {
                if (IsMediumIntegrity(proc)) return proc;
            }
            return null;
        }
        public static bool IsMediumIntegrity(Process process)
        {
            if (GetIntegrityLevel(process) == IntegrityLevel.Medium) return true;
            else return false;
        }

        public static  bool IsHighIntegrity(Process process)
        {
            if (GetIntegrityLevel(process) == IntegrityLevel.High) return true;
            else return false;
        }

        //http://pinvoke.net/default.aspx/Constants/SECURITY_MANDATORY.html
        public static IntegrityLevel GetIntegrityLevel(Process process)
        {
            IntPtr pId = process.Handle;

            IntPtr hToken = IntPtr.Zero;

            if (WinAPI.OpenProcessToken(pId, TOKEN_QUERY, ref hToken))
            {
                try
                {
                    IntPtr pb = Marshal.AllocCoTaskMem(1000);
                    try
                    {
                        uint cb = 1000;
                        if (WinAPI.GetTokenInformation(hToken, Structs.TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pb, cb, out cb))
                        {
                            IntPtr pSid = Marshal.ReadIntPtr(pb);

                            int dwIntegrityLevel = Marshal.ReadInt32(WinAPI.GetSidSubAuthority(pSid, (Marshal.ReadByte(WinAPI.GetSidSubAuthorityCount(pSid)) - 1U)));

                            if (dwIntegrityLevel == SECURITY_MANDATORY_LOW_RID)
                            {
                                return IntegrityLevel.Low;
                            }
                            else if (dwIntegrityLevel >= SECURITY_MANDATORY_MEDIUM_RID && dwIntegrityLevel < SECURITY_MANDATORY_HIGH_RID)
                            {
                                // Medium Integrity
                                return IntegrityLevel.Medium;
                            }
                            else if (dwIntegrityLevel >= SECURITY_MANDATORY_HIGH_RID)
                            {
                                // High Integrity
                                return IntegrityLevel.High;
                            }
                            else if (dwIntegrityLevel >= SECURITY_MANDATORY_SYSTEM_RID)
                            {
                                // System Integrity
                                return IntegrityLevel.System;
                            }
                            return IntegrityLevel.None;
                        }
                        else
                        {
                            int errno = Marshal.GetLastWin32Error();
                            if (errno == ERROR_INVALID_PARAMETER)
                            {
                                throw new NotSupportedException();
                            }
                            throw new Win32Exception(errno);
                        }
                    }
                    finally
                    {
                        Marshal.FreeCoTaskMem(pb);
                    }
                }
                finally
                {
                    WinAPI.CloseHandle(hToken);
                }
            }
            {
                int errno = Marshal.GetLastWin32Error();
                throw new Win32Exception(errno);
            }
        }
    }
}
