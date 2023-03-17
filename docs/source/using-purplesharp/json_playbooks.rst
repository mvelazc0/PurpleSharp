
JSON Playbooks
^^^^^^^^^^^^^^

Using command line parameters became a limitation when trying to run adversary simulation playbooks that execute several techniques with mulitple variations. 
Thats why PurpleSharp also supports the use of JSON files to describe one or more multi-technique playbooks. 

Using JSON files also enables us to further customize the simulation with technique-specific parameters. Each technique may leverage multiple parameters. These parameters may also be used across more than one technique. For example, the **serviceName** parameter is only relevant for the
Create Service technique but the **dllPath** parameter can be use for several techniques like Rundll32.exe and Regsvr32.exe. If not explicitly defined, a default value is used to execute a 
technique.

The following JSON playbook instructs PurpleShap to executes 4 techniques sequentially with a 10 second sleep time between each.

.. note:: Some of the parameters of the playbook below are just informational and are not required nor used by PurpleSharp.

.. code-block:: javascript

   {
   "type": "local",
   "sleep": 10,
   "playbooks": [
      {
         "name": "Simulation Playbook",
         "enabled": true,
         "tasks": [
         {
            "technique_name": "Create or Modify System Process: Windows Service",
            "technique_id": "T1543.003",
            "serviceName": "Legit Service",
            "servicePath": "C:\\Windows\\SysWOW64\\WindowsPowerShell\\v1.0\\powershell.exe",
            "cleanup": true,
            "variation": 1,
            "description": "This variation uses the Win32 APIs: CreateService, OpenService and DeleteService to create a service",
         },
         {
            "technique_name": "Create or Modify System Process: Windows Service",
            "technique_id": "T1543.003",
            "serviceName": "Legit Service",
            "servicePath": "C:\\Windows\\System32\\msiexec.exe",
            "cleanup": false,
            "variation": 2,
            "description": "This variation executes the command 'sc create Legit Service binpath= C:\\Windows\\System32\\msiexec.exe' to create a service",
            "description2": "The service will not be deleted as per the cleanup variable",

         }
      }
   ]
   }

We can execute this playbook using the **/pb** parameter as shown below. If you want to avoid the use of command line parameteres altogether and have PurpleSharp automatically execute
a playbook, you can embed your JSON playbook to the PurpleSharp assembly as a resource_. PurpleSharp will automatically read and execute the
**Playbook.json** embedded resource. At the moment, the only way of achieveing this is by manually adding your playbook to the project and building it with Visual Studio. More details here_.

.. _resource: https://learn.microsoft.com/en-us/dotnet/core/extensions/resources

.. _here: https://stackoverflow.com/questions/39367349/code-or-command-to-use-embedded-resource-in-visual-studio

.. code-block:: powershell

    PurpleSharp.exe /pb simulation_playbook.json

For more simulation playbooks examples, visit the `Active Directory Purple Team Playbook`_, a repository of ready-to-use JSON playbooks for PurpleSharp.

.. _Active Directory Purple Team Playbook: https://github.com/mvelazc0/PurpleAD/

If you want to create custom playbooks and want to know about all the possible parameters each technique supports, or all the possible simulation parameters visit the **Supported Techniques** section or review the projects `Model.cs`_ source file.

.. _Model.cs: https://github.com/mvelazc0/PurpleSharp/blob/master/PurpleSharp/Lib/Models.cs