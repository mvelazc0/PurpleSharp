Credential Access
^^^^^^^^^^^^^^^^^

============================================
T1110.003_ - Brute Force: Password Spraying
============================================


.. _T1110.003: https://attack.mitre.org/techniques/T1110/003/

Variation 1
-----------

| This module uses the LogonUser Win32 API to test a single password across random users obtained via LDAP.

Variation 2
-----------

| This module uses the WNetAddConnection2 Win32 API to test a single password across random users and random hosts obtained via LDAP.

======================================================================
T1558.003_ - Steal or Forge Kerberos Tickets: Kerberoasting
======================================================================


.. _T1558.003: https://attack.mitre.org/techniques/T1558/003/

| This module uses the KerberosRequestorSecurityToken Class to obtain Kerberos service tickets.

================================================
T1003.001_ - OS Credential Dumping: LSASS Memory
================================================

.. _T1003.001: https://attack.mitre.org/techniques/T1003/001/

| This module uses the GetProcessesByName and MiniDumpWriteDump Win32 API functions to create a memory dump of the lsass.exe process.

