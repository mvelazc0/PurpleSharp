using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace PurpleSharp.Simulations
{ 
    public class ExecutionHelper
    {
        public static void StartProcess(string binary, string cmdline, Lib.Logger logger, bool cleanup = false)
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