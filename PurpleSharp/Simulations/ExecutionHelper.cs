using System;
using System.IO;
using System.Runtime.InteropServices;

namespace PurpleSharp.Simulations
{ 
    public class ExecutionHelper
    {
        public static void StartProcess(string binary, string cmdline)
        {
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + "PurpleSharp.txt");
            const uint NORMAL_PRIORITY_CLASS = 0x0020;
            logger.TimestampInfo(String.Format("Starting Process Execution on {0}", Environment.MachineName));
            logger.TimestampInfo(@cmdline);

            bool retValue;
            string CommandLine = @cmdline;
            Structs.PROCESS_INFORMATION pInfo = new Structs.PROCESS_INFORMATION();
            Structs.STARTUPINFO sInfo = new Structs.STARTUPINFO();
            Structs.SECURITY_ATTRIBUTES pSec = new Structs.SECURITY_ATTRIBUTES();
            Structs.SECURITY_ATTRIBUTES tSec = new Structs.SECURITY_ATTRIBUTES();
            pSec.nLength = Marshal.SizeOf(pSec);
            tSec.nLength = Marshal.SizeOf(tSec);

            retValue = WinAPI.CreateProcess(null, cmdline, ref pSec, ref tSec, false, NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref sInfo, out pInfo);

            if (retValue)
            {
                logger.TimestampInfo(String.Format("Process ID (PID): " + pInfo.dwProcessId));
                logger.TimestampInfo(String.Format("Process Handle: " + pInfo.hProcess));
            }
            else logger.TimestampInfo(String.Format("CreateProcess Failed"));


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


        //Simulations.StartProcess("", "powershell.exe -NoExit -enc RwBlAHQALQBQAHIAbwBjAGUAcwBzAA==");
        //Simulations.StartProcess("", "notepad.exe");
    }

}