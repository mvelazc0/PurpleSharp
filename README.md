# PurpleSharp
[![Open_Threat_Research Community](https://img.shields.io/badge/Open_Threat_Research-Community-brightgreen.svg)](https://twitter.com/OTR_Community)

Defending enterprise networks against attackers continues to present a difficult challenge for blue teams. Prevention has fallen short; improving detection & response capabilities has proven to be a step in the right direction. However, without the telemetry produced by adversary behavior, building new and testing existing detection capabilities will be constrained. 

PurpleSharp is an open source adversary simulation tool written in C# that executes adversary techniques within Windows Active Directory environments. The resulting telemetry
can be leveraged to measure and improve the efficacy of a detection engineering program. PurpleSharp leverages the MITRE ATT&CK Framework and executes different techniques across the attack life cycle: execution, persistence, privilege escalation, credential access, lateral movement, etc. It currently supports [47 unique ATT&CK techniques](https://mitre-attack.github.io/attack-navigator/enterprise/#layerURL=https://raw.githubusercontent.com/mvelazc0/PurpleSharp/master/PurpleSharp/Json/PurpleSharp_navigator.json).

PurpleSharp was first presented at [Derbycon IX](https://www.youtube.com/watch?v=7TVp4g4hkpg) on September 2019.

An updated version was released on August 6th 2020 as part of [BlackHat Arsenal 2020](https://www.youtube.com/watch?v=yaeNwdElYaQ). 

Visit the [Demos](https://www.purplesharp.com/en/latest/demos/demos.html) section to see PurpleSharp in action.

## Goals / Use Cases

The attack telemetry produced by simulating techniques with PurpleSharp aids detection teams in:

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
* [Andrew Schwartz](https://www.trustedsec.com/team/andrew-schwartz/)

## License

This project is licensed under the BSD 3-Clause License - see the [LICENSE](LICENSE) file for details
