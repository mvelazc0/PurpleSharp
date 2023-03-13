Simulation Deployment
^^^^^^^^^^^^^^^^^^^^^

Local Simulations
-----------------

PurpleSharp can be used to run simulation playbooks on local endpoints through an interactive session. This type of deployment can be used to test detection and prevention controls on host we have physical access to. 
The only requirement for this type of simulations is to copy the PurpleSharp assembly on the host. 

Depending on the used techniques in the playbook, the simulation may interact with remote hosts in the network. For example, running the PowerShell (T1059.001_) technique will execute PowerShell commandlets locally.
However, the Password Spraying (T1110.003_) technique, will interact with the domain controller or others hosts in the network. 

.. _T1059.001: https://attack.mitre.org/techniques/T1059/001/

.. _T1110.003: https://attack.mitre.org/techniques/T1110/003/

Below is an example of locally running three Process Injection techniques using PurpleSharp's command line parameters (T1055.002_, T1055.003_ and T1055.004_): 

::

   PurpleSharp.exe /t T1055.002,T1055.003,T1055.004

.. _T1055.002: https://attack.mitre.org/techniques/T1055/002/

.. _T1055.003: https://attack.mitre.org/techniques/T1055/003/

.. _T1055.004: https://attack.mitre.org/techniques/T1055/004/

The same simulation playbook can be executed locally using a JSON file as shown below.

.. code-block:: javascript

   {
   "type": "local",
   "sleep": 5,
   "playbooks": [
      {
         "name": "Process Injection Simulation Playbook",
         "enabled": true,
         "tasks": [
         {
            "technique_id": "T1055.002",
            "variation": 1
         },
         {
            "technique_id": "T1055.003",
            "variation": 1
         },
         {
            "technique_id": "T1055.004",
            "variation": 1
         }
         ]
      }
   ]
   }

::

   PurpleSharp.exe /pb simulation.json




A demo video of the above simulation can be found here_.

.. _here: https://youtu.be/lZRE0XX_MXs

Remote Simulations
------------------

PurpleSharp can be also used to deploy simulation playbooks on remote endpoints. This type of deployment can be used to test
the detection and prevention controls on a remote endpoint that may be sitting at a different location across the globe. 

To achieve this, PurpleSharp interacts with the remote host trough the network leveraging native Windows features like SMB and RPC. The core requirements for a remote simulation to work include:

- Administrative credentials on the remote host
- Network connectivty to SMB port TCP/445
- Network connectivty to RPC ports TCP/135 and 

.. image:: /images/remote_simulation.png

Below is an example of using PurpleSharp's command line parameter to deploy a remote simulation:

::

   PurpleSharp.exe /rhost win10-1 /ruser psharp /rpwd Passw0rd1 /d hacklabz.com /t T059.001


The same simulation playbook can be executed remotely using a JSON file as shown below.

.. code-block:: javascript

   {
   "type": "remote",
   "domain": "labz.com",
   "username": "SuperUser",
   "sleep": 5,
   "playbooks": [
      {
         "name": "Process Injection Simulation Playbook",
         "remote_host": "192.168.1.2",
         "scout_full_path": "C:\\Windows\\Temp\\Installer.exe",
         "simulator_relative_path": "AppData\\Local\\Temp\\tmp12345.exe",
         "enabled": true,
         "tasks": [
         {
            "technique_id": "T1055.002",
            "variation": 1
         },
         {
            "technique_id": "T1055.003",
            "variation": 1
         },
         {
            "technique_id": "T1055.004",
            "variation": 1
         }
         ]
      }
   ]
   }

::

   PurpleSharp.exe /pb simulation.json


A demo video of the above simulation can be found on this link_.

.. _link: https://youtu.be/IDPIrjbNO-0&t=93s
