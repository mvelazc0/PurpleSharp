Persistence
^^^^^^^^^^^


==========================================
T1136.001_ - Create Account: Local Account
==========================================


.. _T1136.001: https://attack.mitre.org/techniques/T1136/001/

Variations
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Variation**
     - **Description**
   * - 1
     - | This module uses the Win32 API NetUserAdd to create a local account
       | with the specified parameters.
   * - 2
     - | This module uses the Win32 API CreateProcess to create a local account 
       | with the specified parameters.
       | net user **user** **password** /add

Parameters
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Parameter**
     - **Description**
   * - user
     - The user to be created.
   * - password
     - The password to be used.
   * - cleanup
     - Bool parameter to delete the user after created.

==============================================================
T1543.003_ - Create or Modify System Process: Windows Service
==============================================================


.. _T1543.003: https://attack.mitre.org/techniques/T1543/003/

Variations
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Variation**
     - **Description**
   * - 1
     - | This module uses the Win32 API CreateService to create a Windows 
       | Service with the specified parameters.
   * - 2
     - | This module uses the Win32 API CreateProcess to create a Windows
       | Service with the specified parameters.
       | sc create **serviceName** binpath= **servicePath** type= own start= auto

Parameters
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Parameter**
     - **Description**
   * - serviceName
     - The name of the Windows service to be created.
   * - servicePath
     - The path of the binary that will be executed by the service.
   * - serviceDisplayName
     - The service display name.
   * - cleanup
     - Bool parameter to delete the Service after created.


==================================================================
T1547.001_ - Boot or Logon Autostart Execution: Registry Run Keys
==================================================================


.. _T1547.001: https://attack.mitre.org/techniques/T1547/001/


Variation 1
-----------

| This module uses the the Microsoft.Win32 .NET namespace to create a Registry Key.

Variation 2
-----------

| This module uses the Win32 API CreateProcess to execute a specific command: 
| **REG ADD HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run /V BadApp /t REG_SZ /F /D C:\Windows\Temp\xyz12345.exe**


Variations
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Variation**
     - **Description**
   * - 1
     - | This module uses the Microsoft.Win32 .NET namespace to create a 
       | Registry Key with the specified parameters.
   * - 2
     - | This module uses the Win32 API CreateProcess to create a Registry
       | Key with the specified parameters.
       | reg add **reg_** /V BadApp /t REG_SZ /F /D C:\Windows\Temp\xyz12345.exe

Parameters
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Parameter**
     - **Description**
   * - serviceName
     - The name of the Windows service to be created.
   * - servicePath
     - The path of the binary that will be executed by the service.
   * - serviceDisplayName
     - The service display name.
   * - cleanup
     - Bool parameter to delete the Service after created.


=====================================================================================================
T1546.003_ - Event Triggered Execution: Windows Management Instrumentation Event Subscription
=====================================================================================================


.. _T1546.003: https://attack.mitre.org/techniques/T1546/003/


|  This module uses the System.Management .NET namespace to create the main pieces of a WMI Event Subscription: an Event Filter, an Event Consumer and a FilterToConsumerBinding.
