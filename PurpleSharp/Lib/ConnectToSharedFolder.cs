//Taken from https://www.c-sharpcorner.com/blogs/how-to-access-network-drive-using-c-sharp

/*
other examples: 
http://pinvoke.net/default.aspx/mpr.WNetAddConnection2
https://www.rhyous.com/2011/08/07/how-to-authenticate-and-access-the-registry-remotely-using-c/
http://lookfwd.doitforme.gr/blog/media/PinvokeWindowsNetworking.cs
*/
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Net;
using PurpleSharp;

public class ConnectToSharedFolder : IDisposable
{
    readonly string _networkName;

    public ConnectToSharedFolder(Computer computer, string networkName, NetworkCredential credentials, bool Kerberos)
    {
        _networkName = networkName;
        string protocol= "Kerberos";
        if (!Kerberos) protocol = "NTLM";
        DateTime dtime;

        var netResource = new NetResource
        {
            Scope = ResourceScope.GlobalNetwork,
            ResourceType = ResourceType.Disk,
            DisplayType = ResourceDisplaytype.Share,
            RemoteName = networkName,

        };

        var userName = string.IsNullOrEmpty(credentials.Domain)
            ? credentials.UserName
            : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

        var result = WNetAddConnection2(netResource,credentials.Password,userName,0);

        switch (result.ToString())
        {
            case ("0"):
                dtime = DateTime.Now;
                Console.WriteLine("{0}[{1}] Successfully authenticated as {2} against {3} ({4})", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), userName, computer.ComputerName, protocol);
                break;
            /*
            case ("1323"):
                //Console.WriteLine($"The password is incorrect on {networkName}s");
                break;

            case ("1326"):
                //Console.WriteLine($"The user name or password is incorrect on {networkName}");
                break;

            case ("1328"):
                //Console.WriteLine($"Invalid logon hours on {networkName}");
                break;

            case ("1330"):
                //Console.WriteLine($"Password expired on {networkName}");
                break;

            */

            default:
                //Console.WriteLine("Unknown Error Code on" + result.ToString());
                dtime = DateTime.Now;
                Console.WriteLine("{0}[{1}] Failed to authenticate as {2} against {3} ({4}). Error Code:{5}", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), userName, computer.ComputerName, protocol, result.ToString());
                break;
        }

        //Console.WriteLine("WNetAddConnection2 returned " + result.ToString());

        /*
        if (result != 0)
        {
            //throw new Win32Exception(result, "Error connecting to remote share");
            //throw;
        }*/
    }

    ~ConnectToSharedFolder()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        WNetCancelConnection2(_networkName, 0, true);
    }

    [DllImport("mpr.dll")]
    private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

    [DllImport("mpr.dll")]
    private static extern int WNetCancelConnection2(string name, int flags, bool force);

    [StructLayout(LayoutKind.Sequential)]
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
}