Persistence
^^^^^^^^^^^


=============================
Create Account: Local Account
=============================

T1136.001_

.. _T1136.001: https://attack.mitre.org/techniques/T1136/001/


Variation 1
-----------

| This module uses the Win32 API NetUserAdd to create a local account.

Variation 2
-----------

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **net user hax0r Passw0rd123El7 /add**

================================================
Create or Modify System Process: Windows Service
================================================
T1543.003_

.. _T1543.003: https://attack.mitre.org/techniques/T1543/003/


Variation 1
-----------

| This module uses the Win32 API CreateService to create a Windows Service.

Variation 2
-----------

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **sc create UpdaterService binpath= C:\Windows\Temp\superlegit.exe type= own start= auto**

====================================================
Boot or Logon Autostart Execution: Registry Run Keys
====================================================
T1547.001_

.. _T1547.001: https://attack.mitre.org/techniques/T1547/001/


Variation 1
-----------

| This module uses the the Microsoft.Win32 .NET namespace to create a Registry Key.

Variation 2
-----------

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **REG ADD HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run /V BadApp /t REG_SZ /F /D C:\Windows\Temp\xyz12345.exe**

================================================================================
Event Triggered Execution: Windows Management Instrumentation Event Subscription
================================================================================
T1546.003_

.. _T1546.003: https://attack.mitre.org/techniques/T1546/003/


|  This module uses the System.Management .NET namespace to create the main pieces of a WMI Event Subscription: an Event Filter, an Event Consumer and a FilterToConsumerBinding.
