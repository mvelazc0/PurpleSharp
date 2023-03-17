Execution
^^^^^^^^^

============================================================
`T1059.001`_ - Command and Scripting Interpreter: PowerShell
============================================================

.. _T1059.001: https://attack.mitre.org/techniques/T1059/001/

Variation 1
-----------

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **powershell -enc UwB0AGEAcgB0AC0AUwBsAGUAZQBwACAALQBzACAAMgAwAA==**

Variation 2
-----------

This module uses the the System.Management.Automation .NET namespace to execute the same script.

===================================================================
T1059.003_ Command and Scripting Interpreter: Windows Command Shell
===================================================================

.. _T1059.003: https://attack.mitre.org/techniques/T1059/003/

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **cmd.exe /C whoami**

==========================================================
T1059.005_ Command and Scripting Interpreter: Visual Basic
==========================================================

.. _T1059.005: https://attack.mitre.org/techniques/T1059/005/

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **wscript.exe invoice0420.vbs**


================================================================
T1059.007_ Command and Scripting Interpreter: JavaScript/JScript
================================================================

.. _T1059.007: https://attack.mitre.org/techniques/T1059/007/

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **wscript.exe invoice0420.js**

=============================================
T1053.005_ Scheduled Task/Job: Scheduled Task
=============================================

.. _T1053.005: https://attack.mitre.org/techniques/T1053/005/

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **SCHTASKS /CREATE /SC DAILY /TN BadScheduledTask /TR "C:\Windows\Temp\xyz12345.exe" /ST 13:00**


=============================================
T1569.002_ System Services: Service Execution
=============================================

.. _T1569.002: https://attack.mitre.org/techniques/T1569/002/

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **net start UpdaterService**

