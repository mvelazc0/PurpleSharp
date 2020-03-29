using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class Persistence
    {
        public static void CreateAccountApi(string log)
        {
            PersistenceHelper.CreateUser("haxor", log);
        }

        public static void CreateAccountCmd(string log)
        {
            ExecutionHelper.StartProcess("", "net user haxor Passw0rd123El7 /add", log);
            ExecutionHelper.StartProcess("", "net localgroup Administrators haxor /add", log);
        }

        public static void ResistryRunKey(string log)
        {
            PersistenceHelper.RegistryRunKey(log);
        }

        public static void ResistryRunKeyCmd(string log)
        {
            ExecutionHelper.StartProcess("", @"REG ADD HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run /V BadApp /t REG_SZ /F /D C:\Windows\Temp\xyz12345.exe", log);
        }

        public static void CreateService(string log)
        {
            PersistenceHelper.CreateService(log);
        }
    }
}
