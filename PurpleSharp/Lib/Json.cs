using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


namespace PurpleSharp.Lib
{
    // Input classes
    public class SimulationExercise
    {
        public string domain { get; set; }
        public string username { get; set; }
        public string dc { get; set; }
        public int sleep { get; set; }
        public List<SimulationPlaybook> playbooks { get; set; }
    }

    public class PlaybookTask
    {
        public string technique { get; set; }
        public string host { get; set; }
    }

    public class SimulationPlaybook
    {
        public string name { get; set; }
        public string scoutfpath { get; set; }
        public string simrpath { get; set; }
        public int sleep { get; set; }
        public List<PlaybookTask> tasks { get; set; }
    }


    // Result classes
    public class SimulationExerciseResult
    {
        public List<SimulationPlaybookResult> playbookresults { get; set; }
    }

    public class SimulationPlaybookResult
    {
        public string name { get; set; }
        public List<PlaybookTaskResult> taskresults { get; set; }
    }

    public class PlaybookTaskResult
    {
        public string timestamp { get; set; }
        public string technique { get; set; }
        public string host { get; set; }
        public string user { get; set; }
        public string simprocess { get; set; }
        public int simprocessid { get; set; }
        public bool success { get; set; }
        public List<TaskDebugMsg> debugmsgs { get; set; }

    }
    public class TaskDebugMsg
    {
        public string msg { get; set; }
    }

    //Mitre ATT&CK 

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
        public bool enabled { get; set; }
        //public object[] metadata { get; set; }
    }


    class Json
    {
        public static SimulationExercise ReadJson(string jsoninput)
        {
            try
            {
                SimulationExercise engagement = JsonConvert.DeserializeObject<SimulationExercise>(jsoninput);
                return engagement;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static PlaybookTaskResult GetTaskResult(string results)
        {

            PlaybookTaskResult taskresult = new PlaybookTaskResult();
            List<TaskDebugMsg> debugmsgs = new List<TaskDebugMsg>();


            string[] lines = results.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (line.Contains("Starting"))
                {
                    taskresult.timestamp = line.Substring(0, line.IndexOf('[')).Trim();

                    string strip = line.Substring(line.LastIndexOf("]") + 1).Replace("Starting ", "").Replace("Simulation on ", "").Trim();

                    taskresult.technique = strip.Split(' ')[0];
                    taskresult.host = strip.Split(' ')[1];
                }
                else if (line.Contains("Simulation agent"))
                {
                    //string strip = line.Substring(line.LastIndexOf("]") + 1).Replace("Simulation agent running from ", "").Replace("with PID:", "").Trim();
                    //string strip = line.Substring(line.LastIndexOf("]") + 1).Replace("Simulation agent running from ", "").Replace("with PID:", "").Replace("as ", "").Trim();
                    string strip = line.Substring(line.LastIndexOf("]") + 1).Replace("Simulation agent running from ", "").Replace("with PID:", "|").Replace("as ", "|").Trim();

                    taskresult.simprocess = strip.Split('|')[0];
                    taskresult.simprocessid = Int32.Parse(strip.Split('|')[1]);
                    taskresult.user = strip.Split('|')[2];
                }
                else if (line.Contains("Simulation Finished"))
                {
                    taskresult.success = true;
                }
                else if (line.Contains("Simulation Failed"))
                {
                    taskresult.success = false;
                }
                else
                {
                    TaskDebugMsg debugmsg = new TaskDebugMsg();
                    debugmsg.msg = line;
                    debugmsgs.Add(debugmsg);
                }
                //Console.WriteLine(line.Substring(line.LastIndexOf(']') + 1));
            }
            taskresult.debugmsgs = debugmsgs;
            return taskresult;
            //File.WriteAllText("result.json", JsonConvert.SerializeObject(taskresult));
        }
        public static void WriteJsonPlaybookResults(SimulationExerciseResult engagementResult)
        {
            File.WriteAllText("result.json", JsonConvert.SerializeObject(engagementResult));

        }

        public static void ExportAttackLayer(string[] techniques)
        {

            NavigatorLayer layer = new NavigatorLayer();
            layer.version = "2.2";
            layer.name = "PurpleSharp Coverage";
            layer.domain = "mitre-enterprise";
            layer.description = "Layer of techniques supported by PurpleSharp";
            layer.hideDisabled = true;

            NavigatorFilters filters = new NavigatorFilters();
            filters.stages = new string [] { "act" };
            filters.platforms = new string []{ "Windows" };

            layer.filters = filters;

            List<NavigatorTechnique> layertechniques = new List<NavigatorTechnique>();

            foreach (string technique in techniques)
            {
                NavigatorTechnique tech = new NavigatorTechnique();
                tech.techniqueID = technique;
                tech.color = "#756bb1";
                //tech.comment = "";
                tech.enabled = true;

                layertechniques.Add(tech);
            }
            layer.techniques = layertechniques;
            File.WriteAllText("PurpleSharp.json", JsonConvert.SerializeObject(layer));
        }

        public static NavigatorLayer ReadNavigatorLayer(string jsoninput)
        {
            try
            {
                NavigatorLayer navlayer  = JsonConvert.DeserializeObject<NavigatorLayer>(jsoninput);
                return navlayer;
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static SimulationExercise ConvertNavigatorToSimulationExercise(NavigatorLayer layer, string[] supportedtechniques)
        {
            SimulationExercise engagement = new SimulationExercise();
            SimulationPlaybook playbook = new SimulationPlaybook();
            playbook.name = layer.name;

            //playbook.tasks = new List<PlaybookTask>();
            List<PlaybookTask> tasks = new List<PlaybookTask>();
            int notsupported = 0;

            foreach (NavigatorTechnique technique in layer.techniques)
            {
                if (Array.IndexOf(supportedtechniques, technique.techniqueID) > -1)
                {
                    PlaybookTask task = new PlaybookTask();
                    task.technique = technique.techniqueID;
                    task.host = "random";
                    tasks.Add(task);
                }
                else 
                {
                    Console.WriteLine("[!] {0} not supported by PurpleSharp",technique.techniqueID);
                    notsupported += 1;
                }
                    
            }
            playbook.tasks = tasks;
            engagement.playbooks = new List<SimulationPlaybook>();
            engagement.playbooks.Add(playbook);
            Console.WriteLine("[!] Found a total of {0} techniques not supported out of {1}", notsupported.ToString(), layer.techniques.Count.ToString());
            Console.WriteLine("[!] Final number of tasks supported: {0}", tasks.Count.ToString());
            return engagement;
        }

        public static void CreateSimulationExercise(SimulationExercise engagement)
        {
            File.WriteAllText("simulation.json", JsonConvert.SerializeObject(engagement));

        }



    }
}
