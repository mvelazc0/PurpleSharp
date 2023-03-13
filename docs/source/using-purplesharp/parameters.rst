
Command Line Parameters
^^^^^^^^^^^^^^^^^^^^^^^

.. warning::
    Using command line parameters to execute simulations with PurpleSharp does not leverage all available features.
    If you are looking to customize the simulations with more flexibility, you should use JSON playbooks.

******************************
Required Simulation Parameters
******************************

Remote Host (/rhost)
--------------------
Defines the remote host the simulation will run on. 

.. note:: If set to '**random**', PurpleSharp will perform LDAP queries on the defined domain controller and randomly pick a simulation target.


Remote User (/ruser)
--------------------
Defines the domain user used to deploy the simulation. This user needs to be part of the 'Administrators' group on the remote host.


Domain (/d)
--------------

Defines the domain the simulation target is part of.

Technique(s) (/t)
-----------------

Defines the MITRE ATT&CK Framework technique id or ids to use in the simulation.

.. note:: When using more than one technique, use a comma to separate them and **no space** between them.

.. code-block:: powershell

    PurpleSharp.exe /t T1055.002,T1055.003,T1055.004

******************************
Optional Simulation Parameters
******************************

Remote Password (/rpwd)
-----------------------

Defines the password for the user used to deploy the simulation. If not present, PurpleSharp will prompt for the password.

Domain Controller (/dc)
-----------------------

When deploying simulations on a random host, this settings specifices the Domain Controller to run the LDAP queries on.

Verbose (/v)
------------

When set, the Scout logs will be presented as part of the output.

Playbook Sleep Time (/pbsleep)
------------------------------

When simulating more than one technique, this parameter defines the amount of time in seconds to sleep between each technique execution. 

Technique Sleep Time (/tsleep)
-------------------------------

Certain techniques also support an internal sleep time defined with this parameter in seconds.

.. note:: When used with the Kerberoasting technique, PurpleSharp will sleep between each Kerberos Service Ticket request.

Scout Path (/scoutpath)
-----------------------

Defines the absolute path where the Scout will be uploaded to on the remote host. If not set, PurpleSharp will use the default path: **C:\\Windows\\Scout.exe**.

Simulator Path (/simpath)
-------------------------

Defines the relative path where the Simulator will be uploaded to on the remote host. If not set, PurpleSharp will use the default path: **\\Downloads\\Firefox_Installer.exe**.

No Clean Up (/nocleanup)
------------------------

Certain techniques will create an artifact on the remote endpoint. PurpleSharp will by default delete the artifact as part of the clean up process when a simulation is completed. When this parameter is set, the clean phase for the particular technique will be skipped. 

.. note:: As an example, when using the Windows Service technique (T1543.003) with **/nocleanup**, PurpleSharp will not delete the created  Windows Service from the simulation target after installing it.


No Opsec (/noopsec)
-------------------

When set, PurpleSharp will not use the Parent Process ID Spoofing technique the execute the Simulator. This will result in the Simulator running in the context of the service account used to deploy the simulation.

****************
Other Parameters
****************

Scout (/scout)
--------------

PurpleSharp can execute reconoissance tasks on remote hosts with the goal of providing the operator relevant information about them before running simulations. The following scout tasks are supported:

- **auditpol**: This action will retrieve the remote endpoint's advanced audit policy settings.

- **wef**: This action will retrieve the remote endpoint's Windows Event Subscription settings.

- **pws**: This action will retrieve the remote endpoint's Module Logging, Transcription Logging and SecriptBlock Logging PowerShell settints.

- **ps**: This action will retrieve the remote endpoint's running processes. 

- **svcs**: This action will retrieve the remote endpoint's running Windows services.

- **all**: This option will execute all of the above tasks.

.. code-block:: powershell

    PurpleSharp.exe PurpleSharp.exe /scout all /rhost host /ruser user /d domain

ATT&CK Navigator (/navigator)
-----------------------------

PurpleSharp integrates with `MITRE's ATT&CK Navigator`_ project. 

- **export**: This action will export an ATT&CK Navigator layer with all the of techniques supported by PurpleSharp. An online version of this layer can be viewed here_.

.. _here: https://mitre-attack.github.io/attack-navigator/enterprise/#layerURL=https://raw.githubusercontent.com/mvelazc0/PurpleSharp/master/PurpleSharp/Json/PurpleSharp_navigator.json

.. code-block:: powershell

    PurpleSharp.exe /navigator export

- **import**: With this action PurpleSharp will take a ATT&CK Navigator layer file as a parameter and create a JSON simulation playbook with all the supported techniques. 

.. _MITRE's ATT&CK Navigator: https://mitre-attack.github.io/attack-navigator/enterprise/


.. code-block:: powershell

    PurpleSharp.exe /navigator import APT1.json



Playbook (/pb)
--------------

This parameter defines the JSON Playbook to use as an input for the simulation.

.. code-block:: powershell

    PurpleSharp.exe /pb SimulationPlaybook.json