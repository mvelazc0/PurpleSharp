# PurpleSharp
[![Open_Threat_Research Community](https://img.shields.io/badge/Open_Threat_Research-Community-brightgreen.svg)](https://twitter.com/OTR_Community)
[![BlackHat Arsenal 2021](https://raw.githubusercontent.com/toolswatch/badges/master/arsenal/usa/2021.svg)](https://www.blackhat.com/us-21/arsenal/schedule/#purplesharp--active-directory-attack-simulations-24011)

<p align="center">
  <img src="https://github.com/mvelazc0/PurpleSharp/blob/docs/docs/source/images/new-logo2.png?raw=true" width="75%" height="75%" alt="PurpleSharp Logo"/>
</p>

Defending enterprise networks against attackers continues to present a difficult challenge for blue teams. Prevention has fallen short; improving detection & response capabilities has proven to be a step in the right direction. However, without the telemetry produced by adversary behavior, building new and testing existing detection capabilities will be constrained. 

PurpleSharp is an open source adversary simulation tool written in C# that executes adversary techniques within Windows Active Directory environments. The resulting telemetry
can be leveraged to measure and improve the efficacy of a detection engineering program. PurpleSharp leverages the MITRE ATT&CK Framework and executes different techniques across the attack life cycle: execution, persistence, privilege escalation, credential access, lateral movement, etc. It currently supports [47 unique ATT&CK techniques](https://mitre-attack.github.io/attack-navigator/enterprise/#layerURL=https://raw.githubusercontent.com/mvelazc0/PurpleSharp/master/PurpleSharp/Json/PurpleSharp_navigator.json).

<p align="center">
  <img src="https://raw.githubusercontent.com/mvelazc0/PurpleSharp/docs/docs/source/images/mitre-layer.png" width="75%" height="75%"/>
</p>

PurpleSharp was first presented at [Derbycon IX](https://www.youtube.com/watch?v=7TVp4g4hkpg) on September 2019.

An updated version was released on August 6th 2020 as part of [BlackHat Arsenal 2020](https://www.youtube.com/watch?v=yaeNwdElYaQ). The latest version was released on August 2021 as part of [BlackHat Arsenal 2021](https://www.youtube.com/watch?v=jvpVgJQPoXw) 

Visit the [Demos](https://www.purplesharp.com/en/latest/demos/demos.html) section to see PurpleSharp in action.

## Goals / Use Cases

The attack telemetry produced by simulating techniques with PurpleSharp aids research & detection teams in:

* Building new detecttion analytics
* Testing existing detection analytics
* Validating detection resiliency
* Identifying gaps in visibility
* Identifing issues with event logging pipeline

## Quick Start Guide

### Build from Source

PurpleSharp can be built with Visual Studio Community 2019 or 2020.

### Download Latest Release

[Download](https://github.com/mvelazc0/PurpleSharp/releases) the latest release binary ready to be used to execute TTP simulations.

.NET Framework 4.5 is required.

### Simulate

The PurpleSharp assembly is all you need to start simulating attacks. 

For simulation ideas, check out the [Active Directory Purple Team Playbook](https://github.com/mvelazc0/PurpleAD/), a repository of ready-to-use JSON playbooks for PurpleSharp.

## Documentation

[https://www.purplesharp.com/](https://www.purplesharp.com/)


## Authors

* **Mauricio Velazco** - [@mvelazco](https://twitter.com/mvelazco)

## Acknowledgments

The community is a great source of ideas and feedback. Thank you all.

* [Olaf Hartong](https://twitter.com/olafhartong)
* [Roberto Rodriguez](https://twitter.com/Cyb3rWard0g)
* [Matt Graeber](https://twitter.com/mattifestation)
* [Jonny Johnson](https://twitter.com/jsecurity101)
* [Keith McCammon](https://twitter.com/kwm)
* [Andrew Schwartz](https://twitter.com/4ndr3w6S)

## License

This project is licensed under the BSD 3-Clause License - see the [LICENSE](LICENSE) file for details
