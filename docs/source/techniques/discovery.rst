Discovery
^^^^^^^^^

=============================================
T1049_ - System Network Connections Discovery
=============================================


.. _T1049: https://attack.mitre.org/techniques/T1049/

| This module uses the CreateProcess Win32 API to execute
| **netstat.exee**
| **net.exe use**
| **net.exe sessions**

====================================
T1033_ - System Owner/User Discovery
====================================


.. _T1033: https://attack.mitre.org/techniques/T1033/

| This module uses the CreateProcess Win32 API to execute
| **whoami.exe**
| **query user**


=================================
T1007_ - System Service Discovery
=================================


.. _T1007: https://attack.mitre.org/techniques/T1007/

| This module uses the CreateProcess Win32 API to execute
| **net.exe start**
| **tasklist.exe /svc**


==============================================
T1087.002_ - Account Discovery: Domain Account
==============================================


.. _T1087.002: https://attack.mitre.org/techniques/T1087/002/

Variation 1
-----------

| This module uses the Sytem.DirectoryServices .NET NameSpace to query a domain environment using LDAP.

Variation 2
-----------

| This module uses the CreatePRocess Win32 API to execute:
| **net.exe user /domain**


=================================
T1046_ - Network Service Scanning
=================================


.. _T1046: https://attack.mitre.org/techniques/T1046/

| This module uses the  System.Net.Sockets .NET namespace to scan ports on remote endpoints randomly picked using LDAP.

=============================================
T1087.001_ - Account Discovery: Local Account
=============================================

.. _T1087.001: https://attack.mitre.org/techniques/T1087/001/

| This module uses the CreateProcess Win32 API to execute
| **net.exe user**

===============================================
T1016_ - System Network Configuration Discovery
===============================================


.. _T1016: https://attack.mitre.org/techniques/T1016/


| This module uses the CreateProcess Win32 API to execute
| **ipconfig.exe /all"**

=====================================
T1083_ - File and Directory Discovery
=====================================

.. _T1083: https://attack.mitre.org/techniques/T1083/


| This module uses the CreateProcess Win32 API to execute
| **dir.exe c:\ >> %temp%\download**
| **dir.exe C:\Users\ >> %temp%\download**

================================
T1135_ - Network Share Discovery
================================


.. _T1135: https://attack.mitre.org/techniques/T1135/

| This module uses the NetShareEnum Win32 API function to enumerate shared on remote endpoints randomly picked using LDAP.

