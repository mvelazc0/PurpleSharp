Simulation Deployment
^^^^^^^^^^^^^^^^^^^^^

Local
-----

PurpleSharp can be used to run simulation playbooks on local endpoints while on an interactive session. Depending on the used techniques in the playbook, 
the simulation techniques may interact with remote hosts in the network. 

This type of deployment can be used to test detection and prevention controls on host we have physical access to. The only requirement is to load the PurpleSharp assembly.

Below is an example of locally running three **Process Injection** techniques using command line parameters (T1055.002_, T1055.003_ and T1055.004_): 

::

   C:\> PurpleSharp.exe /t T1055.002,T1055.003,T1055.004

.. _T1055.002: https://attack.mitre.org/techniques/T1055/002/

.. _T1055.003: https://attack.mitre.org/techniques/T1055/003/

.. _T1055.004: https://attack.mitre.org/techniques/T1055/004/

.. raw:: html

   <p><iframe width="560" height="315" src="https://www.youtube.com/embed/lZRE0XX_MXs" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe><br></p>

Remote
------

PurpleSharp can be also used to deploy simulation playbooks on remote endpoints. This type of deployment can be used to test
the detection and prevention controls on a remote endpoint that may be sitting across the globe. 

.. note:: PurpleSharp leverages native Windows features like SMB and RPC to deploy remote simulations. To work properly, the core requirements are to have network connectivity and administrative credentials on the remote host. 

Below is an example of using the command line to deploy a remote simulation:

::

   C:\> PurpleSharp.exe /rhost win10-1 /ruser psharp /rpwd Passw0rd1 /d hacklabz.com /t T059.001

.. raw:: html

   <p><iframe width="560" height="315" src="https://www.youtube.com/embed/IDPIrjbNO-0" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe><br></p>


The next example uses the JSON playbook parameter to deploy simulations on remote hosts:

.. raw:: html

   <p><iframe width="560" height="315" src="https://www.youtube.com/embed/jvpVgJQPoXw?start=1460" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe><br></p>





