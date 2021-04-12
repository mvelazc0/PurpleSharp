using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Host;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Lib
{
    // Command Line Parameter Model
    public class CommandlineParameters
    {
        public string scout_full_path;

        public string simulator_relative_path;

        public string remote_host;

        public string remote_user;

        public string remote_password;

        public string domain;

        public string techniques;

        public string domain_controller;

        public int variation;

        public int playbook_sleep;

        public int task_sleep;

        public bool cleanup;

        public bool opsec;

        public bool verbose;

        public string scout_namepipe;

        public string simulator_namedpipe;

        public string log_filename;

        public string scout_action;

        public CommandlineParameters()
        {

        }
        public CommandlineParameters(string scout_path, string simulator_path, string rhost, string ruser, string rpwd, string d, string techs, string dc, string sc_action, string scout_np, string simulator_np, string log, int var, int pbsleep, int tsleep, bool cln, bool ops, bool verb)
        {
            scout_full_path = scout_path;
            simulator_relative_path = simulator_path;
            remote_host = rhost;
            remote_user = ruser;
            remote_password = rpwd;
            domain = d;
            techniques = techs;
            domain_controller = dc;
            scout_action = sc_action;
            scout_namepipe = scout_np;
            simulator_namedpipe = simulator_np;
            log_filename = log;
            variation = var;
            playbook_sleep = pbsleep;
            task_sleep = tsleep;
            cleanup = cln;
            opsec = ops;
            verbose = verb;
        }
    }

    // Input classes
    public class SimulationExercise
    {
        public string domain { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string domain_controller { get; set; }
        public int sleep { get; set; }
        public string type { get; set; }
        public List<SimulationPlaybook> playbooks { get; set; }
    }
    public class SimulationPlaybook
    {
        public string name { get; set; }
        public string description { get; set; }
        public string scout_full_path { get; set; }
        public string simulator_relative_path { get; set; }
        public int playbook_sleep { get; set; }
        public string remote_host { get; set; }
        public string opsec { get; set; } = "ppid";
        public bool enabled { get; set; } = true;
        public List<PlaybookTask> tasks { get; set; }
        public SimulationPlaybook(int pbsleep)
        {
            playbook_sleep = pbsleep;

        }
        public SimulationPlaybook()
        {

        }
    }
    public class PlaybookTask
    {
        // Generic variables
        public string technique_id { get; set; }
        public int variation { get; set; } = 1;
        public int task_sleep { get; set; } = 0;
        public bool cleanup { get; set; } = true;

        // Password Spraying
        public string protocol { get; set; } = "Kerberos";
        public int user_target_type { get; set; } = 1;
        public int host_target_type { get; set; } = 1;
        public int user_target_total { get; set; } = 5;
        public int host_target_total { get; set; } = 5;
        public string[] user_targets { get; set; }
        public string[] host_targets { get; set; }

        public PlaybookTask()
        {
        }

        public PlaybookTask(string tech, int var, int t_sleep, bool cl = true)
        {
            technique_id = tech;
            variation = var;
            task_sleep = t_sleep;
            cleanup = cleanup;
        }
    }

    // Result classes
    public class SimulationExerciseResult
    {
        public List<SimulationPlaybookResult> playbookresults { get; set; }
    }

    public class SimulationPlaybookResult
    {
        public string name { get; set; }
        public string host { get; set; }
        public string user { get; set; }
        public string simprocess { get; set; }
        public int simprocessid { get; set; }

        public List<PlaybookTaskResult> taskresults { get; set; }
    }
    public class PlaybookTaskResult
    {
        public string timestamp { get; set; }
        public string technique { get; set; }
        //public string host { get; set; }
        public bool success { get; set; }
        public List<TaskDebugMsg> debugmsgs { get; set; }

    }
    public class TaskDebugMsg
    {
        public string msg { get; set; }
    }

    // ATT&CK Classes
    public class NavigatorLayer
    {
        public string name { get; set; }
        public string version { get; set; }
        public string domain { get; set; }
        public string description { get; set; }
        public bool hideDisabled { get; set; }

        public NavigatorFilters filters { get; set; }
        public List<NavigatorTechnique> techniques { get; set; }


        //public Gradient gradient { get; set; }
        //public object[] legendItems { get; set; }
        //public object[] metadata { get; set; }
        //public bool showTacticRowBackground { get; set; }
        //public string tacticRowBackground { get; set; }
        //public bool selectTechniquesAcrossTactics { get; set; }
    }
    public class NavigatorFilters
    {
        public string[] stages { get; set; }
        public string[] platforms { get; set; }
    }
    public class NavigatorGradient
    {
        public string[] colors { get; set; }
        public int minValue { get; set; }
        public int maxValue { get; set; }
    }
    public class NavigatorTechnique
    {
        public string techniqueID { get; set; }
        //public string tactic { get; set; }
        public string color { get; set; }
        //public string comment { get; set; }
        public int score { get; set; }
        public bool enabled { get; set; }
        //public object[] metadata { get; set; }
    }

    // Named Pipe Comms Classes
    public class SimulationRequest
    {
        public string header;

        public string recon_type;

        public SimulationPlaybook playbook;

        public SimulationRequest(string hd, string re_type = "", SimulationPlaybook pb = null)
        {
            header = hd;
            recon_type = re_type;
            playbook = pb;
        }
    }
    public class SimulationResponse
    {
        public string header;

        public ReconResponse recon_response;

        public ScoutResponse scout_response;

        public SimulationResponse(string stat, ReconResponse recon_resp = null, ScoutResponse sc_resp = null)
        {
            header = stat;
            recon_response = recon_resp;
            scout_response = sc_resp;
        }
    }
    public class ReconResponse
    {
        public string user;

        public string process;

        public string process_id;

        public string process_integrity;
        public ReconResponse(string u, string proc, string proc_id, string proc_int)
        {
            user = u;
            process = proc;
            process_id = proc_id;
            process_integrity = proc_int;
        }
    }

    public class ScoutResponse
    {
        public string results;
        public ScoutResponse(string res)
        {
            results = res;
        }

    }
}
