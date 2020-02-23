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
            ExecutionHelper.StartProcess("", "powershell.exe -enc UwB0AGEAcgB0AC0AUAByAG8AYwBlAHMAcwAgAC0ARgBpAGwAZQBQAGEAdABoACAAbgBvAHQAZQBwAGEAZAA=");

        }

    }
}
