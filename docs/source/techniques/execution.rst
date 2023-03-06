Execution
^^^^^^^^^

=============================================
Command and Scripting Interpreter: PowerShell
=============================================

T1059.001_

.. _T1059.001: https://attack.mitre.org/techniques/T1059/001/

Variation 1
-----------

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **powershell -enc UwB0AGEAcgB0AC0AUwBsAGUAZQBwACAALQBzACAAMgAwAA==**

Variation 2
-----------

This module uses the the System.Management.Automation .NET namespace to execute the same script.

========================================================
Command and Scripting Interpreter: Windows Command Shell
========================================================

T1059.003_

.. _T1059.003: https://attack.mitre.org/techniques/T1059/003/

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **cmd.exe /C whoami**

===============================================
Command and Scripting Interpreter: Visual Basic
===============================================

T1059.005_

.. _T1059.005: https://attack.mitre.org/techniques/T1059/005/

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **wscript.exe invoice0420.vbs**


=====================================================
Command and Scripting Interpreter: JavaScript/JScript
=====================================================

T1059.007_

.. _T1059.007: https://attack.mitre.org/techniques/T1059/007/

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **wscript.exe invoice0420.js**

===================================
Scheduled Task/Job: Scheduled Task
===================================

T1053.005_

.. _T1053.005: https://attack.mitre.org/techniques/T1053/005/

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **SCHTASKS /CREATE /SC DAILY /TN BadScheduledTask /TR "C:\Windows\Temp\xyz12345.exe" /ST 13:00**


==================================
System Services: Service Execution
==================================

T1569.002_

.. _T1569.002: https://attack.mitre.org/techniques/T1569/002/

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **net start UpdaterService**

