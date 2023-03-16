.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Home
   
   PurpleSharp <home/purplesharp>

.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Using PurpleSharp

   Simulation Deployment <using-purplesharp/deployment>
   Command Line Cheat Sheet <using-purplesharp/cheatsheet>
   Command Line Parameters <using-purplesharp/parameters>
   JSON Playbooks <using-purplesharp/json_playbooks>

.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Supported Techniques

   Techniques <techniques/techniques>
   Execution <techniques/execution>
   Persitence <techniques/persistence>
   Privilege Escalation <techniques/privilege_escalation>
   Defense Evasion <techniques/defense_evasion>
   Credential Access <techniques/credential_access>
   Discovery <techniques/discovery>
   Lateral Movement <techniques/lateral_movement>


.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Presentations/Demos

   Presentations <presentations/presentations>
   Demos <demos/demos>


.. image:: /images/new-logo2.png

|

Defending enterprise networks against attackers continues to present a difficult challenge for blue teams. Prevention has fallen short; improving detection
& response capabilities has proven to be a step in the right direction. However, without the telemetry produced by adversary behavior, building new and testing
existing detection capabilities will be constrained.

PurpleSharp_ is an open source adversary simulation tool written in C# that executes adversary techniques within Windows
Active Directory environments. The resulting telemetry can be leveraged to measure and improve the efficacy of a detection
engineering program. PurpleSharp leverages the MITRE ATT&CK Framework and executes different techniques across the attack
life cycle: execution, persistence, privilege escalation, credential access, lateral movement, etc. It currently supports
`47 unique ATT&CK techniques`_.

.. _PurpleSharp: https://github.com/mvelazc0/PurpleSharp

.. _47 unique ATT&CK techniques: https://mitre-attack.github.io/attack-navigator/enterprise/#layerURL=https://raw.githubusercontent.com/mvelazc0/PurpleSharp/master/PurpleSharp/Json/PurpleSharp_navigator.json

|

.. image:: /images/mitre-layer.png
   :target: https://mitre-attack.github.io/attack-navigator/#layerURL=https://raw.githubusercontent.com/mvelazc0/PurpleSharp/master/PurpleSharp/Json/PurpleSharp_navigator.json

|


PurpleSharp was first presented at `Derbycon IX`_ on September 2019.
An updated version was released on August 6th 2020 as part of `BlackHat Arsenal 2020`_. The latest version was released on August 2021 as part of `BlackHat Arsenal 2021`_.

.. _Derbycon IX: https://www.youtube.com/watch?v=7TVp4g4hkpg

.. _Blackhat Arsenal 2020: https://www.youtube.com/watch?v=yaeNwdElYaQ

.. _Blackhat Arsenal 2021: https://www.youtube.com/watch?v=jvpVgJQPoXw

Visit the `Demos <demos/demos.html>`_ section to see PurpleSharp in action.

Goals / Use Cases
-----------------

The attack telemetry produced by simulating techniques with PurpleSharp aids detection teams in:

- Building new detection analytics
- Testing existing detection analytics
- Validating detection resiliency
- Identifying gaps in visibility
- Identifing issues with event logging pipeline

Quick Start Guide
-----------------

Build from Source
~~~~~~~~~~~~~~~~~

PurpleSharp can be built with Visual Studio Community 2019 or 2020.


Download Latest Release 
~~~~~~~~~~~~~~~~~~~~~~~

`Download`_ the latest release binary ready to be used to execute TTP simulations.

.. _Download: https://github.com/mvelazc0/PurpleSharp/releases

Simulate 
~~~~~~~~

The PurpleSharp assembly is all you need to start simulating attacks.

For simulation ideas, check out the `Active Directory Purple Team Playbook`_, a repository of ready-to-use JSON playbooks for PurpleSharp.

.. _Active Directory Purple Team Playbook: https://github.com/mvelazc0/PurpleAD/

Authors
------- 

- Mauricio Velazco - `@mvelazco`_

.. _@mvelazco: https://twitter.com/mvelazco


Acknowledgments
---------------

The community is a great source of ideas and feedback. Thank you all.

- `Olaf Hartong`_
- `Roberto Rodriguez`_
- `Matt Graeber`_
- `Jonny Johnson`_
- `Keith McCammon`_
- `Andrew Schwartz`_


.. _Olaf Hartong: https://twitter.com/olafhartong
.. _Roberto Rodriguez: https://twitter.com/Cyb3rWard0g
.. _Matt Graeber: https://twitter.com/mattifestation
.. _Jonny Johnson: https://twitter.com/jsecurity101
.. _Keith McCammon: https://twitter.com/kwm
.. _Andrew Schwartz: https://www.trustedsec.com/team/andrew-schwartz/