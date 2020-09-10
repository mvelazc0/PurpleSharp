.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Home
   
   PurpleSharp <home/purplesharp>

.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Using PurpleSharp

   Cheat Sheet <using-purplesharp/cheatsheet>
   Commandline Parameter Overview <using-purplesharp/parameters>

.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Supported Techniques

   Techniques <techniques/techniques>

.. toctree::
   :maxdepth: 2
   :hidden:
   :caption: Demos

   Demos <demos/demos>



PurpleSharp
===========

Defending enterprise networks against attackers continues to present a difficult challenge for blue teams. Prevention has fallen short; improving detection
& response capabilities has proven to be a step in the right direction. However, without the telemetry produced by adversary behavior, building new and testing
existing detection capabilities will be constrained.

PurpleSharp_ is an open source adversary simulation tool written in C# that executes adversary techniques within Windows
Active Directory environments. The resulting telemetry can be leveraged to measure and improve the efficacy of a detection
engineering program. PurpleSharp leverages the MITRE ATT&CK Framework and executes different techniques across the attack
life cycle: execution, persistence, privilege escalation, credential access, lateral movement, etc. It currently supports
`38 unique ATT&CK techniques`_.

.. _PurpleSharp: https://github.com/mvelazc0/PurpleSharp

.. _37 unique ATT&CK techniques: https://mitre-attack.github.io/attack-navigator/enterprise/#layerURL=https://raw.githubusercontent.com/mvelazc0/PurpleSharp/master/PurpleSharp/Json/PurpleSharp_navigator.json

PurpleSharp was first presented at `Derbycon IX`_ on September 2019.
An updated version was released on August 6th 2020 as part of `BlackHat Arsenal 2020`_.

.. _Derbycon IX: https://www.youtube.com/watch?v=7TVp4g4hkpg

.. _Blackhat Arsenal 2020: https://www.youtube.com/watch?v=yaeNwdElYaQ

Visit the `Demos <demos/demos.html>`_ section to see PurpleSharp in action.

Goals / Use Cases
-----------------

The attack telemetry produced by simulating techniques with PurpleSharp aids detection teams in:

- Building new detecttion analytics
- Testing existing detection analytics
- Validating detection resiliency
- Identifying gaps in visibility
- Identifing issues with event logging pipeline

Quick Start Guide
-----------------

PurpleSharp can be built with Visual Studio Community 2019 or 2020.

Authors
------- 

- Mauricio Velazco - `@mvelazco`_

.. _@mvelazco: www.google.com


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