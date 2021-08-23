
Command line Cheat Sheet
^^^^^^^^^^^^^^^^^^^^^^^^

.. warning::
    Using command line parameters to execute simulations with PurpleSharp does not leverage all available features and is no longer maintained.

    Command line parameters do still work but they should only be used for simple simulations and/or troubleshooting purposes.

    Current development is focused on using JSON files to describe simulations playbooks.

Execute the T1059.001 technique against win10-1.

::

   C:\> PurpleSharp.exe /rhost win10-1 /ruser psharp /d hacklabz /t T1059.001 

Execute 3 chained techniques against win10-1 and wait 30 seconds between each.

::

   C:\> PurpleSharp.exe /rhost win10-1 /ruser psharp /d hacklabz /t T1059.001,T1059.002,T1059.003 /pbsleep 30

Execute techniques using custom Scout and Simulator paths

::

   C:\> PurpleSharp.exe /rhost win10-1 /ruser psharp /d hacklabz /t T1059.001 /scoutpath C:\PSEXSVC.exe /simpath \AppData\Local\Temp\invoice.exe

Obtain the Windows Event Subscription settings on host win10-1.

::

   C:\> PurpleSharp.exe /rhost win10-1 /ruser psharp /d hacklabz /scout wef