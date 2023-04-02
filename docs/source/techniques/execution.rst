Execution
^^^^^^^^^

============================================================
`T1059.001`_ - Command and Scripting Interpreter: PowerShell
============================================================

.. _T1059.001: https://attack.mitre.org/techniques/T1059/001/

Variations
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Variation**
     - **Description**
   * - 1
     - | This module uses the Win32 API CreateProcess to execute the specified 
       | commandlet:
       | powershell.exe -command {**commandlet**}
   * - 2
     - | This module uses the the System.Management.Automation .NET namespace
       | to execute the specified commandlet.


Parameters
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Parameter**
     - **Description**
   * - commandlet
     - The PowerShell commandlet to be executed in the simulation.

===================================================================
T1059.003_ Command and Scripting Interpreter: Windows Command Shell
===================================================================

.. _T1059.003: https://attack.mitre.org/techniques/T1059/003/

Variations
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Variation**
     - **Description**
   * - 1
     - | This module uses the Win32 API CreateProcess to execute the specified 
       | command:
       | cmd.exe /c **command**


Parameters
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Parameter**
     - **Description**
   * - command
     - The command shell to be executed in the simulation.


==========================================================
T1059.005_ Command and Scripting Interpreter: Visual Basic
==========================================================

.. _T1059.005: https://attack.mitre.org/techniques/T1059/005/

Variations
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Variation**
     - **Description**
   * - 1
     - | This module uses the Win32 API CreateProcess to execute the specified 
       | VB script:
       | wscript.exe **file_path**

Parameters
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Parameter**
     - **Description**
   * - file_path
     - The local file path of the VB script.

================================================================
T1059.007_ Command and Scripting Interpreter: JavaScript/JScript
================================================================

.. _T1059.007: https://attack.mitre.org/techniques/T1059/007/

Variations
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Variation**
     - **Description**
   * - 1
     - | This module uses the Win32 API CreateProcess to execute the specified 
       | JS script:
       | wscript.exe **file_path**

Parameters
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Parameter**
     - **Description**
   * - file_path
     - The local file path of the JS script.

=============================================
T1053.005_ Scheduled Task/Job: Scheduled Task
=============================================

.. _T1053.005: https://attack.mitre.org/techniques/T1053/005/

Variations
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Variation**
     - **Description**
   * - 1
     - | This module uses the Win32 API CreateProcess to create a scheduled 
       | task:
       | SCHTASKS /CREATE /SC DAILY /TN **taskName** /TR **taskPath** /ST 13:00

Parameters
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Parameter**
     - **Description**
   * - taskName
     - The name of the task to be created.
   * - taskPath   
     - The path of the binary to be executed by the scheduled task.
   * - cleanup   
     - Bool parameter to delete the scheduled task after created.

=============================================
T1569.002_ System Services: Service Execution
=============================================

.. _T1569.002: https://attack.mitre.org/techniques/T1569/002/

Variations
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Variation**
     - **Description**
   * - 1
     - | This module uses the Win32 API CreateProcess to start the specified 
       | Windows service:
       | net start **serviceName**

Parameters
----------

.. list-table:: 
   :align: center
   :widths: 10 75

   * - **Parameter**
     - **Description**
   * - serviceName
     - The name of the Windows service to be started.