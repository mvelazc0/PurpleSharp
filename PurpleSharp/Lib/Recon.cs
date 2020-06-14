using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
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

        public static Process GetExplorer()
        {
            Process explorer = Process.GetProcessesByName("explorer").FirstOrDefault();
            if (explorer != null) return explorer;
            else return null;
            
        }

        public static Process GetHostProcess(bool privileged = false)
        {
            if (privileged)
            {
                Process mmc = GetHighIntegrityProc(Process.GetProcessesByName("mmc"));
                Process winlogon = Process.GetProcessesByName("winlogon").FirstOrDefault();
                Process svchost = Process.GetProcessesByName("svchost").FirstOrDefault();

                if (mmc != null) return mmc;
                else if (winlogon != null) return winlogon;
                else if (svchost != null) return svchost;
                else return null;
            }
            else
            {
                Process chrome = GetMediumIntegrityProc(Process.GetProcessesByName("chrome"));
                Process firefox = GetMediumIntegrityProc(Process.GetProcessesByName("firefox"));
                Process iexplore = GetMediumIntegrityProc(Process.GetProcessesByName("iexplore"));
                Process winword = GetMediumIntegrityProc(Process.GetProcessesByName("winword"));
                Process outlook = GetMediumIntegrityProc(Process.GetProcessesByName("outlook"));
                Process excel = GetMediumIntegrityProc(Process.GetProcessesByName("excel"));
                Process explorer = Process.GetProcessesByName("explorer").FirstOrDefault();

                //if (chrome != null) return chrome;
                //else if (firefox != null) return firefox;
                //else if (iexplore != null) return iexplore;
                //else if (winword != null) return excel;
                //else if (excel != null) return excel;
                //else if (outlook != null) return outlook;
                if (explorer != null) return explorer;
                else return null;
            }

        }

        //https://stackoverflow.com/questions/777548/how-do-i-determine-the-owner-of-a-process-in-c
        public static string GetProcessOwner(Process process)
        {
            IntPtr processHandle = IntPtr.Zero;
            try
            {
                WinAPI.OpenProcessToken(process.Handle, 8, ref processHandle);
                WindowsIdentity wi = new WindowsIdentity(processHandle);
                string user = wi.Name;
                return user;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (processHandle != IntPtr.Zero)
                {
                    WinAPI.CloseHandle(processHandle);
                }
            }
        }

        public static string GetProcessOwnerWmi(Process process)
        {
            string query = "Select * From Win32_Process Where ProcessID = " + process.Id;
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

        public static Process GetHighIntegrityProc(Process[] procs)
        {
            foreach (Process proc in procs)
            {
                if (IsHighIntegrity(proc)) return proc;
            }
            return null;
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
        public static string GetProcs()
        {
            string output= "======== Processes ========" + Environment.NewLine; ; ;
            foreach (Process proc in Process.GetProcesses())
            {
                try
                {
                    string arch = (Environment.Is64BitProcess)? "x64" : "x86";
                    output += String.Format("{0} - {1} - {2} - {3}", proc.MainModule.FileName, proc.Id, arch, GetProcessOwnerWmi(proc))  + Environment.NewLine;
                }
                catch    
                {
                }
            }
            output += Environment.NewLine + Environment.NewLine;
            return output;
        }
        //Pre Assessment Functions

        public static string GetServices()
        {
            string output = "======== Running Services ========" + Environment.NewLine; ;
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                //string running = (service.Status == ServiceControllerStatus.Running ) ? "Running" : "Stopped";
                if (service.Status == ServiceControllerStatus.Running)
                {
                    output += String.Format("{0}  -  {1} ", service.ServiceName, service.DisplayName) + Environment.NewLine;
                }
                    
            }
            output += Environment.NewLine + Environment.NewLine;
            return output;
        }
             
        public static string GetAuditPolicy()
        {
            string output = "======== Audit Policy Settings ========" + Environment.NewLine;
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "auditpol.exe";
            p.StartInfo.Arguments = "/get /category:*";
            p.Start();
            //logger.TimestampInfo(output);
            p.WaitForExit();
            output += p.StandardOutput.ReadToEnd();
            output += Environment.NewLine;
            return output;
        }

        public static string GetPwsLoggingSettings()
        {
            List<String> hives = new List<string>() { "Module Logging", "Transcription Logging", "ScriptBlock Logging" };
            string output = "======== PowerShell Logging Settings ========" + Environment.NewLine;

            foreach (String hive in hives)
            {
                //
                Dictionary<string, object> settings = GetRegValues("HKLM", "SOFTWARE\\Policies\\Microsoft\\Windows\\PowerShell\\"+hive.Replace(" ",""));
                output += "-------- "+hive+ " --------" + Environment.NewLine;
                if ((settings != null) && (settings.Count != 0))
                {
                    foreach (KeyValuePair<string, object> kvp in settings)
                    {
                        if (kvp.Value.GetType().IsArray && (kvp.Value.GetType().GetElementType().ToString() == "System.String"))
                        {
                            string result = string.Join(",", (string[])kvp.Value);
                            //PrintItemValue(kvp.Key, result);
                            output += $"{kvp.Key}: {result}";
                        }
                        else
                        {
                            output += $"{kvp.Key}: {kvp.Value}";
                            //PrintItemValue(kvp.Key, kvp.Value);
                        }
                    }
                    output += Environment.NewLine + Environment.NewLine;
                    
                }
                else
                {
                    output += "No "+ hive + " Settings Found";
                    output += Environment.NewLine + Environment.NewLine;
                }

            }

            return output;

        }

        public static string GetCmdlineAudittingSettings()
        {
            string output = "======== Command Line Auditing Settings ========" + Environment.NewLine;
            Dictionary<string, object> settings = GetRegValues("HKLM", "HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit\\");
            //output += "settings " + settings.ToString() + Environment.NewLine;
            //output += "count: " + settings.Count.ToString() + Environment.NewLine; ;
            if ((settings != null) && (settings.Count != 0))
            {
                foreach (KeyValuePair<string, object> kvp in settings)
                {
                    if (kvp.Value.GetType().IsArray && (kvp.Value.GetType().GetElementType().ToString() == "System.String"))
                    {
                        string result = string.Join(",", (string[])kvp.Value);
                        //PrintItemValue(kvp.Key, result);
                        output += $"{kvp.Key}: {result}";
                    }
                    else
                    {
                        output += $"{kvp.Key}: {kvp.Value}";
                        //PrintItemValue(kvp.Key, kvp.Value);
                    }
                }
                output += Environment.NewLine + Environment.NewLine;
                return output;
            }
            else
            {
                output += "No Command Line Auditing Settings Found";
                output += Environment.NewLine + Environment.NewLine;
                return output;
            }
        }

        //Credits to https://github.com/jaredhaight/scout
        public static string GetWefSettings()
        {
            string output = "======== WEF Settings ========" + Environment.NewLine;
            Dictionary<string, object> settings = GetRegValues("HKLM", "Software\\Policies\\Microsoft\\Windows\\EventLog\\EventForwarding\\SubscriptionManager\\");
            //output += "settings " + settings.ToString() + Environment.NewLine;
            //output += "count: " + settings.Count.ToString() + Environment.NewLine; ;
            if ((settings != null) && (settings.Count != 0))
            {
                foreach (KeyValuePair<string, object> kvp in settings)
                {
                    if (kvp.Value.GetType().IsArray && (kvp.Value.GetType().GetElementType().ToString() == "System.String"))
                    {
                        string result = string.Join(",", (string[])kvp.Value);
                        //PrintItemValue(kvp.Key, result);
                        output += $"{kvp.Key}: {result}";
                    }
                    else
                    {
                        output += $"{kvp.Key}: {kvp.Value}";
                        //PrintItemValue(kvp.Key, kvp.Value);
                    }
                }
                output += Environment.NewLine + Environment.NewLine;
                return output;
            }
            else
            {
                output += "No WEF Settings Found";
                output += Environment.NewLine + Environment.NewLine;
                return output;
            }
        }

        //Credits to https://github.com/jaredhaight/scout
        public static RegistryKey GetRegistryKey(string hive, string path)
        {
            if (hive == "HKCU")
            {
                //return RegistryKey.OpenRemoteBaseKey(RegistryHive.CurrentUser, COMPUTERNAME).OpenSubKey(path);
                return RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64).OpenSubKey(path);
            }
            else if (hive == "HKU")
            {
                //return RegistryKey.OpenRemoteBaseKey(RegistryHive.Users, COMPUTERNAME).OpenSubKey(path);
                return RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64).OpenSubKey(path);
            }
            else
            {
                //return RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, COMPUTERNAME).OpenSubKey(path);
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(path);
            }
        }

        //Credits to https://github.com/jaredhaight/scout
        public static string GetRegValue(string hive, string path, string value)
        {
            // returns a single registry value under the specified path in the specified hive (HKLM/HKCU)
            string regKeyValue = "";
            var regKey = GetRegistryKey(hive, path);
            if (regKey != null)
            {
                regKeyValue = String.Format("{0}", regKey.GetValue(value));
            }
            return regKeyValue;
        }

        //Credits to https://github.com/jaredhaight/scout
        public static Dictionary<string, object> GetRegValues(string hive, string path)
        {
            // returns all registry values under the specified path in the specified hive (HKLM/HKCU)
            Dictionary<string, object> keyValuePairs = null;
            try
            {
                using (var regKeyValues = GetRegistryKey(hive, path))
                {
                    if (regKeyValues != null)
                    {
                        var valueNames = regKeyValues.GetValueNames();
                        keyValuePairs = valueNames.ToDictionary(name => name, regKeyValues.GetValue);
                    }
                }
                return keyValuePairs;
            }
            catch
            {
                return null;
            }
        }


    }
}
