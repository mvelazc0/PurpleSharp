using PurpleSharp.Lib;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PurpleSharp.Simulations
{ 
    public class ExecutionHelper
    {
        public static void StartProcess_old(string binary, string cmdline, Lib.Logger logger, bool cleanup = false)
        {
            if (!cleanup) logger.TimestampInfo("Executing Command: " + cmdline);
            else logger.TimestampInfo("Executing Cleanup Command: " + cmdline);

            const uint NORMAL_PRIORITY_CLASS = 0x0020;
            bool retValue;
            Structs.PROCESS_INFORMATION pInfo = new Structs.PROCESS_INFORMATION();
            Structs.STARTUPINFO sInfo = new Structs.STARTUPINFO();
            Structs.SECURITY_ATTRIBUTES pSec = new Structs.SECURITY_ATTRIBUTES();
            Structs.SECURITY_ATTRIBUTES tSec = new Structs.SECURITY_ATTRIBUTES();
            pSec.nLength = Marshal.SizeOf(pSec);
            tSec.nLength = Marshal.SizeOf(tSec);

            retValue = WinAPI.CreateProcess(null, cmdline, ref pSec, ref tSec, false, NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref sInfo, out pInfo);

            if (retValue && cleanup == false)
            {
                logger.TimestampInfo(String.Format("Process successfully created. (PID): " + pInfo.dwProcessId));
            }
            else if (retValue != false && cleanup == false ) logger.TimestampInfo("Could not start process!");
        }

        public static void StartProcessApi(string binary, string cmdline, Lib.Logger logger)
        {
            
            const uint NORMAL_PRIORITY_CLASS = 0x0020;
            bool retValue;
            Structs.PROCESS_INFORMATION pInfo = new Structs.PROCESS_INFORMATION();
            Structs.STARTUPINFO sInfo = new Structs.STARTUPINFO();
            // avoid stdout to be printed
            sInfo.dwFlags = 0x00000100 | 0x00000001;

            Structs.SECURITY_ATTRIBUTES pSec = new Structs.SECURITY_ATTRIBUTES();
            Structs.SECURITY_ATTRIBUTES tSec = new Structs.SECURITY_ATTRIBUTES();
            pSec.nLength = Marshal.SizeOf(pSec);
            tSec.nLength = Marshal.SizeOf(tSec);
            logger.TimestampInfo(String.Format("Using the Win32 API call CreateProcess to execute: '{0}'", cmdline));
            retValue = WinAPI.CreateProcess(null, cmdline, ref pSec, ref tSec, false, NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref sInfo, out pInfo);

            if (retValue)
            {
                logger.TimestampInfo(String.Format("Process successfully created. (PID): " + pInfo.dwProcessId));
            }
            else logger.TimestampInfo("Could not start process!");
        }

        public static string StartProcessNET(string binary, string cmdline, Logger logger)
        {

            using (Process process = new Process())
            {
                process.StartInfo.FileName = binary;
                process.StartInfo.Arguments = cmdline;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                logger.TimestampInfo(String.Format("Using the System.Diagnostics .NET namespace to execute '{0} {1}'", binary, cmdline));
                process.Start();
                logger.TimestampInfo(String.Format("Process successfully created. (PID): " + process.Id));

                /*
                string standard_output;
                string error_output;
                string final_output="";
                int i = 0;
                
                while ((standard_output = process.StandardOutput.ReadLine()) != null && i < 10)
                {
                    if (!standard_output.Trim().Equals(""))
                    {
                        //do something
                        logger.TimestampInfo(standard_output);
                        final_output += standard_output;
                        //Console.WriteLine(standard_output);
                        i++;
                        //break;
                    }
                }
                while ((error_output = process.StandardError.ReadLine()) != null && i < 10)
                {
                    if (!error_output.Trim().Equals(""))
                    {
                        logger.TimestampInfo(error_output);
                        final_output += error_output;
                        i++;
                        //break;
                    }
                }
                */
                //process.WaitForExit();
                return "";

            }
        }
        public static void StartProcessAsUser(string binary, string cmdline, string domain, string username, string password)
        {
            Structs.STARTUPINFO startupInfo = new Structs.STARTUPINFO();

            Structs.LogonFlags lflags = new Structs.LogonFlags();

            //UInt32 exitCode = 123456;
            Structs.PROCESS_INFORMATION processInfo = new Structs.PROCESS_INFORMATION();

            String command = @"c:\windows\notepad.exe";
            String currentDirectory = System.IO.Directory.GetCurrentDirectory();

            WinAPI.CreateProcessWithLogonW(username, domain, password, lflags, command, command, (UInt32)0, (UInt32)0, currentDirectory, ref startupInfo, out processInfo);

        }
    }

}