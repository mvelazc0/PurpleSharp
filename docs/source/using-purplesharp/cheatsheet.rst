
Command line Cheat Sheet
^^^^^^^^^^^^^^^^^^^^^^^^

.. warning::
    Using command line parameters to execute simulations with PurpleSharp does not leverage all available features.
    If you are looking to customize the simulations with more flexibility, you should use JSON playbooks.

Execute the T1059.001 technique on local host

.. code-block:: powershell

    PurpleSharp.exe /t T1059.001

Execute 3 techniques on local host

.. code-block:: powershell

    PurpleSharp.exe /t T1055.002,T1055.003,T1055.004
    PurpleSharp.exe /t "T1055.002, T1055.003, T1055.004"


Execute 3 techniques on local host adding a sleep time of 5 seconds between technique.

.. code-block:: powershell

    PurpleSharp.exe /t "T1055.002, T1055.003, T1055.004" /pbsleep 5

Execute the T1059.001 technique on win10-1.

.. code-block:: powershell

   PurpleSharp.exe /rhost win10-1 /ruser psharp /d hacklabz /t T1059.001

Execute 3 chained techniques against win10-1 and wait 30 seconds between each.

::

   C:\> PurpleSharp.exe /rhost win10-1 /ruser psharp /d hacklabz /t T1059.001,T1059.002,T1059.003 /pbsleep 30

Execute techniques using custom Scout and Simulator paths

::

   C:\> PurpleSharp.exe /rhost win10-1 /ruser psharp /d hacklabz /t T1059.001 /scoutpath C:\PSEXSVC.exe /simpath \AppData\Local\Temp\invoice.exe

Obtain the Windows Event Subscription settings on host win10-1.

::

   C:\> PurpleSharp.exe /rhost win10-1 /ruser psharp /d hacklabz /scout wef