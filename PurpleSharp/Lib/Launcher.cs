using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class Launcher
{

    const int TOKEN_DUPLICATE = 0x0002;
    const uint MAXIMUM_ALLOWED = 0x2000000;
    const int CREATE_NEW_CONSOLE = 0x00000010;

    const int IDLE_PRIORITY_CLASS = 0x40;
    const int NORMAL_PRIORITY_CLASS = 0x20;
    const int HIGH_PRIORITY_CLASS = 0x80;
    const int REALTIME_PRIORITY_CLASS = 0x100;

    [DllImport("kernel32.dll")]
    static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

    public static bool SpoofParent(int parentProcessId, string binaryPath)
    {

        // STARTUPINFOEX members
        const int PROC_THREAD_ATTRIBUTE_PARENT_PROCESS = 0x00020000;

        // STARTUPINFO members (dwFlags and wShowWindow)
        const int STARTF_USESTDHANDLES = 0x00000100;
        const int STARTF_USESHOWWINDOW = 0x00000001;
        const short SW_HIDE = 0x0000;

        // dwCreationFlags
        const uint EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
        const uint CREATE_NO_WINDOW = 0x08000000;

        //var error = Marshal.GetLastWin32Error();

        var pInfo = new Structs.PROCESS_INFORMATION();
        var siEx = new Structs.STARTUPINFOEX();

        // Be sure to set the cb member of the STARTUPINFO structure to sizeof(STARTUPINFOEX).
        siEx.StartupInfo.cb = Marshal.SizeOf(siEx);
        IntPtr lpValueProc = IntPtr.Zero;
        IntPtr hSourceProcessHandle = IntPtr.Zero;

        if (parentProcessId > 0)
        {
            var lpSize = IntPtr.Zero;
            var success = WinAPI.InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref lpSize);
            if (success || lpSize == IntPtr.Zero)
            {
                return false;
            }
            Console.WriteLine("successfully used InitializeProcThreadAttributeList ");

            siEx.lpAttributeList = Marshal.AllocHGlobal(lpSize);
            success = WinAPI.InitializeProcThreadAttributeList(siEx.lpAttributeList, 1, 0, ref lpSize);
            if (!success)
            {
                return false;
            }
            Console.WriteLine("successfully used InitializeProcThreadAttributeList");

            IntPtr parentHandle = WinAPI.OpenProcess(Structs.ProcessAccessFlags.CreateProcess | Structs.ProcessAccessFlags.DuplicateHandle, false, parentProcessId);
            if (parentHandle == null)
            {
                return false;
            }

            Console.WriteLine("obtained a handle to parent process");

            // This value should persist until the attribute list is destroyed using the DeleteProcThreadAttributeList function
            lpValueProc = Marshal.AllocHGlobal(IntPtr.Size);
            Marshal.WriteIntPtr(lpValueProc, parentHandle);

            success = WinAPI.UpdateProcThreadAttribute(siEx.lpAttributeList, 0, (IntPtr)PROC_THREAD_ATTRIBUTE_PARENT_PROCESS, lpValueProc, (IntPtr)IntPtr.Size, IntPtr.Zero, IntPtr.Zero);
            if (!success)
            {
                return false;
            }
            Console.WriteLine("successfully used UpdateProcThreadAttribute");

            
            IntPtr hCurrent = System.Diagnostics.Process.GetCurrentProcess().Handle;
            IntPtr hNewParent = WinAPI.OpenProcess(Structs.ProcessAccessFlags.DuplicateHandle, true, parentProcessId);
            if (hNewParent == null)
            {
                return false;
            }

            Console.WriteLine("successfully used OpenProcess");
            

        }

        siEx.StartupInfo.dwFlags = STARTF_USESHOWWINDOW | STARTF_USESTDHANDLES;
        siEx.StartupInfo.wShowWindow = SW_HIDE;

        var ps = new Structs.SECURITY_ATTRIBUTES();
        var ts = new Structs.SECURITY_ATTRIBUTES();
        ps.nLength = Marshal.SizeOf(ps);
        ts.nLength = Marshal.SizeOf(ts);
        //bool ret = CreateProcess(null, command, ref ps, ref ts, true, EXTENDED_STARTUPINFO_PRESENT | CREATE_NO_WINDOW, IntPtr.Zero, null, ref siEx, out pInfo);
        Console.WriteLine("About to call create processd");
        bool ret = WinAPI.CreateProcess(binaryPath, null, ref ps, ref ts, true, EXTENDED_STARTUPINFO_PRESENT | CREATE_NO_WINDOW, IntPtr.Zero, null, ref siEx, out pInfo);
        Console.WriteLine(Marshal.GetLastWin32Error());
        if (!ret)
        {
            Console.WriteLine("[!] Proccess failed to execute!");
            return false;
        }

        Console.WriteLine("successfully used CreateProcess");
        return true;
    }

    public static bool StartProcessAsLoggedUser(String applicationName, string startingDir, out Structs.PROCESS_INFORMATION procInfo)
    {
        int winlogonPid = 0;
        IntPtr hUserTokenDup = IntPtr.Zero, hPToken = IntPtr.Zero, hProcess = IntPtr.Zero;
        procInfo = new Structs.PROCESS_INFORMATION();

        // obtain the currently active session id; every logged on user in the system has a unique session id
        uint dwSessionId = WinAPI.WTSGetActiveConsoleSessionId();

        // obtain the process id of the winlogon process that is running within the currently active session
        // -- chaged by ty 
        // Process[] processes = Process.GetProcessesByName("winlogon");
        Process[] processes = Process.GetProcessesByName("explorer");
        foreach (Process p in processes)
        {
            if ((uint)p.SessionId == dwSessionId)
            {
                winlogonPid = p.Id;
            }
        }

        // obtain a handle to the winlogon process
        //hProcess = OpenProcess(MAXIMUM_ALLOWED, false, winlogonPid);
        hProcess = WinAPI.OpenProcess(Structs.ProcessAccessFlags.CreateProcess | Structs.ProcessAccessFlags.DuplicateHandle | Structs.ProcessAccessFlags.QueryInformation, false, winlogonPid);

        // obtain a handle to the access token of the winlogon process
        if (!WinAPI.OpenProcessToken(hProcess, TOKEN_DUPLICATE, ref hPToken))
        {
            WinAPI.CloseHandle(hProcess);
            return false;
        }

        // Security attibute structure used in DuplicateTokenEx and CreateProcessAsUser
        // I would prefer to not have to use a security attribute variable and to just 
        // simply pass null and inherit (by default) the security attributes
        // of the existing token. However, in C# structures are value types and therefore
        // cannot be assigned the null value.
        Structs.SECURITY_ATTRIBUTES sa = new Structs.SECURITY_ATTRIBUTES();
        sa.nLength = Marshal.SizeOf(sa);

        // copy the access token of the winlogon process; the newly created token will be a primary token
        if (!WinAPI.DuplicateTokenEx(hPToken, MAXIMUM_ALLOWED, ref sa, (int)Structs.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, (int)Structs.TOKEN_TYPE.TokenPrimary, ref hUserTokenDup))
        {
            WinAPI.CloseHandle(hProcess);
            WinAPI.CloseHandle(hPToken);
            return false;
        }

        // By default CreateProcessAsUser creates a process on a non-interactive window station, meaning
        // the window station has a desktop that is invisible and the process is incapable of receiving
        // user input. To remedy this we set the lpDesktop parameter to indicate we want to enable user 
        // interaction with the new process.
        Structs.STARTUPINFO2 si = new Structs.STARTUPINFO2();
        si.cb = (int)Marshal.SizeOf(si);
        si.lpDesktop = @"winsta0\default"; // interactive window station parameter; basically this indicates that the process created can display a GUI on the desktop

        // flags that specify the priority and creation method of the process
        int dwCreationFlags = NORMAL_PRIORITY_CLASS | CREATE_NEW_CONSOLE;

        // create a new process in the current user's logon session
        bool result = WinAPI.CreateProcessAsUser(hUserTokenDup,        // client's access token
                                        null,                   // file to execute
                                        applicationName,        // command line
                                        ref sa,                 // pointer to process SECURITY_ATTRIBUTES
                                        ref sa,                 // pointer to thread SECURITY_ATTRIBUTES
                                        false,                  // handles are not inheritable
                                        dwCreationFlags,        // creation flags
                                        IntPtr.Zero,            // pointer to new environment block 
                                        startingDir,                   // name of current directory 
                                        ref si,                 // pointer to STARTUPINFO structure
                                        out procInfo            // receives information about new process
                                        );

        // invalidate the handles
        WinAPI.CloseHandle(hProcess);
        WinAPI.CloseHandle(hPToken);
        WinAPI.CloseHandle(hUserTokenDup);

        return result; // return the result
    }
}