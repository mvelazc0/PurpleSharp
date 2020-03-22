using System;
using System.Runtime.InteropServices;
using System.Security;

public static class WinAPI
{

    [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
    public static extern int NetShareEnum(string ServerName, int level, ref IntPtr bufPtr, uint prefmaxlen, ref int entriesread, ref int totalentries, ref int resume_handle);

    [DllImport("Netapi32.dll", EntryPoint = "NetApiBufferFree")]
    public static extern uint NetApiBufferFree(IntPtr buffer);


    [DllImport("mpr.dll")]
    public static extern int WNetCancelConnection2(string name, int flags,bool force);

    [DllImport("mpr.dll")]
    public static extern int WNetAddConnection2(NetResource netResource,string password, string username, int flags);



    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, ref Structs.SECURITY_ATTRIBUTES lpProcessAttributes, ref Structs.SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref Structs.STARTUPINFOEX lpStartupInfo, out Structs.PROCESS_INFORMATION lpProcessInformation);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, ref Structs.SECURITY_ATTRIBUTES lpProcessAttributes, ref Structs.SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref Structs.STARTUPINFO lpStartupInfo, out Structs.PROCESS_INFORMATION lpProcessInformation);


    [DllImport("kernel32.dll")]
    public static extern IntPtr CreateProcessA(String lpApplicationName, String lpCommandLine, Structs.SecurityAttributes lpProcessAttributes, Structs.SecurityAttributes lpThreadAttributes, Boolean bInheritHandles, Structs.CreateProcessFlags dwCreationFlags,
    IntPtr lpEnvironment,
    String lpCurrentDirectory,
    [In] Structs.StartupInfo lpStartupInfo,
    out Structs.ProcessInformation lpProcessInformation

);


    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(Structs.ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);


    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern UInt32 WaitForSingleObject(IntPtr handle, UInt32 milliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UpdateProcThreadAttribute(IntPtr lpAttributeList, uint dwFlags, IntPtr Attribute, IntPtr lpValue, IntPtr cbSize, IntPtr lpPreviousValue, IntPtr lpReturnSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool InitializeProcThreadAttributeList(IntPtr lpAttributeList, int dwAttributeCount, int dwFlags, ref IntPtr lpSize);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetHandleInformation(IntPtr hObject, Structs.HANDLE_FLAGS dwMask, Structs.HANDLE_FLAGS dwFlags);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, ref IntPtr lpTargetHandle, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);


    [DllImport("kernel32.dll")]
    //public static extern IntPtr VirtualAllocEx(IntPtr lpHandle,IntPtr lpAddress, IntPtr dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
    public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll")]
    public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);


    [DllImport("kernel32.dll")]
    public static extern uint WTSGetActiveConsoleSessionId();

    [DllImport("advapi32", SetLastError = true), SuppressUnmanagedCodeSecurity]
    public static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

    [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
    public extern static bool DuplicateTokenEx(IntPtr ExistingTokenHandle, uint dwDesiredAccess, ref Structs.SECURITY_ATTRIBUTES lpThreadAttributes, int TokenType, int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);

    [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    public extern static bool CreateProcessAsUser(IntPtr hToken, String lpApplicationName, String lpCommandLine, ref Structs.SECURITY_ATTRIBUTES lpProcessAttributes,
    ref Structs.SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, int dwCreationFlags, IntPtr lpEnvironment,
    String lpCurrentDirectory, ref Structs.STARTUPINFO2 lpStartupInfo, out Structs.PROCESS_INFORMATION lpProcessInformation);

    
    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

    [DllImport("kernel32.dll")]
    public static extern bool ReadProcessMemory(int hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

    [DllImport("Dbghelp.dll")]
    public static extern bool MiniDumpWriteDump(IntPtr hProcess, uint processId, SafeHandle hFile, uint dumpType, IntPtr expParam, IntPtr userStreamParam, IntPtr callbackParam);

    
    [DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int NetUserAdd(string servername, UInt32 level, Structs.USER_INFO_2 userInfo, out UInt32 parm_err);

    [DllImport("NetApi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int NetLocalGroupAddMembers(string servername, string groupname, UInt32 level, ref Structs.LOCALGROUP_MEMBERS_INFO_3 buf, UInt32 totalentries);

    [DllImport("NetApi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public extern static int NetUserDel(string servername, string username);

    
    [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool CreateProcessWithLogonW(String userName, String domain, String password, Structs.LogonFlags logonFlags, String applicationName, String commandLine, Structs.CreationFlags creationFlags, UInt32 environment, String currentDirectory, ref Structs.STARTUPINFO startupInfo, out Structs.PROCESS_INFORMATION processInformation);

    

    [DllImport("advapi32.dll", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr OpenSCManagerW(string machineName, string databaseName, uint dwAccess);

    

    [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, Structs.SCM_ACCESS scParameter);

    [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr CreateService(
    IntPtr serviceControlManagerHandle,
    string lpSvcName,
    string lpDisplayName,
    Structs.SERVICE_ACCESS dwDesiredAccess,
    Structs.SERVICE_TYPES dwServiceType,
    Structs.SERVICE_START_TYPES dwStartType,
    Structs.SERVICE_ERROR_CONTROL dwErrorControl,
    string lpPathName,
    string lpLoadOrderGroup,
    IntPtr lpdwTagId,
    string lpDependencies,
    string lpServiceStartName,
    string lpPassword);

    [DllImport("advapi32", SetLastError = true)]
    public static extern bool CloseServiceHandle(IntPtr serviceHandle);

    [DllImport("advapi32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ControlService(IntPtr hService, Structs.SERVICE_CONTROL dwControl, ref Structs.SERVICE_STATUS lpServiceStatus);

    [DllImport("advapi32", SetLastError = true)]
    public static extern int StartService(IntPtr serviceHandle, int dwNumServiceArgs, string lpServiceArgVectors);

    [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern IntPtr OpenService(IntPtr serviceControlManagerHandle, string lpSvcName, Structs.SERVICE_ACCESS dwDesiredAccess);

    [DllImport("advapi32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteService(IntPtr serviceHandle);



    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern IntPtr GetSidSubAuthority(IntPtr sid, UInt32 subAuthorityIndex);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern IntPtr GetSidSubAuthorityCount(IntPtr sid);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern bool GetTokenInformation(
    IntPtr TokenHandle,
    Structs.TOKEN_INFORMATION_CLASS TokenInformationClass,
    IntPtr TokenInformation,
    uint TokenInformationLength,
    out uint ReturnLength
    );






}
/*
public class NETRESOURCE
{
    public ResourceScope dwScope = 0;
    public ResourceType dwType = 0;
    public ResourceDisplayType dwDisplayType = 0;
    public ResourceUsage dwUsage = 0;
    public string lpLocalName = null;
    public string lpRemoteName = null;
    public string lpComment = null;
    public string lpProvider = null;
};

public enum ResourceScope
{
    RESOURCE_CONNECTED = 1,
    RESOURCE_GLOBALNET,
    RESOURCE_REMEMBERED,
    RESOURCE_RECENT,
    RESOURCE_CONTEXT
};

public enum ResourceType
{
    RESOURCETYPE_ANY = 0 ,
    RESOURCETYPE_DISK = 1,
    RESOURCETYPE_PRINT = 2,
    RESOURCETYPE_RESERVED = 8
};

public enum ResourceUsage
{
    RESOURCEUSAGE_CONNECTABLE = 0x00000001,
    RESOURCEUSAGE_CONTAINER = 0x00000002,
    RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
    RESOURCEUSAGE_SIBLING = 0x00000008,
    RESOURCEUSAGE_ATTACHED = 0x00000010,
    RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
};

public enum ResourceDisplayType
{
    RESOURCEDISPLAYTYPE_GENERIC,
    RESOURCEDISPLAYTYPE_DOMAIN,
    RESOURCEDISPLAYTYPE_SERVER,
    RESOURCEDISPLAYTYPE_SHARE,
    RESOURCEDISPLAYTYPE_FILE,
    RESOURCEDISPLAYTYPE_GROUP,
    RESOURCEDISPLAYTYPE_NETWORK,
    RESOURCEDISPLAYTYPE_ROOT,
    RESOURCEDISPLAYTYPE_SHAREADMIN,
    RESOURCEDISPLAYTYPE_DIRECTORY,
    RESOURCEDISPLAYTYPE_TREE,
    RESOURCEDISPLAYTYPE_NDSCONTAINER
};
*/
public class NetResource
{
    public ResourceScope Scope;
    public ResourceType ResourceType;
    public ResourceDisplaytype DisplayType;
    public int Usage;
    public string LocalName;
    public string RemoteName;
    public string Comment;
    public string Provider;
}

public enum ResourceScope : int
{
    Connected = 1,
    GlobalNetwork,
    Remembered,
    Recent,
    Context
};

public enum ResourceType : int
{
    Any = 0,
    Disk = 1,
    Print = 2,
    Reserved = 8,
}

public enum ResourceDisplaytype : int
{
    Generic = 0x0,
    Domain = 0x01,
    Server = 0x02,
    Share = 0x03,
    File = 0x04,
    Group = 0x05,
    Network = 0x06,
    Root = 0x07,
    Shareadmin = 0x08,
    Directory = 0x09,
    Tree = 0x0a,
    Ndscontainer = 0x0b
}