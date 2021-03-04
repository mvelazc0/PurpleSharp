using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation.Language;

namespace PurpleSharp.Lib
{
    // Input classes
    public class SimulationExercise
    {
        public string domain { get; set; }
        public string username { get; set; }
        public string dc { get; set; }
        public int sleep { get; set; }
        public string type { get; set; }
        public List<SimulationPlaybook> playbooks { get; set; }
    }


    public class SimulationPlaybook
    {
        public string name { get; set; }
        public string scoutfpath { get; set; }
        public string simrpath { get; set; }
        public int pbsleep { get; set; }
        public int tsleep { get; set; }
        public string host { get; set; }
        public List<PlaybookTask> tasks { get; set; }
    }

    public class PlaybookTask
    {
        public string technique { get; set; }
        public int variation { get; set; }
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
        public int score { get; set; }
        public bool enabled { get; set; }
        //public object[] metadata { get; set; }
    }


    class Json
    {
        public static SimulationExercise ReadSimulationPlaybook(string jsoninput)
        {
            try
            {
                SimulationExercise engagement = JsonConvert.DeserializeObject<SimulationExercise>(jsoninput);
                return engagement;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
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
                    //taskresult.host = strip.Split(' ')[1];
                }
                else if (line.Contains("Simulator"))
                {
                    //string strip = line.Substring(line.LastIndexOf("]") + 1).Replace("Simulator running from ", "").Replace("with PID:", "").Trim();
                    //string strip = line.Substring(line.LastIndexOf("]") + 1).Replace("Simulator running from ", "").Replace("with PID:", "").Replace("as ", "").Trim();
                    string strip = line.Substring(line.LastIndexOf("]") + 1).Replace("Simulator running from ", "").Replace("with PID:", "|").Replace("as ", "|").Trim();

                    //taskresult.simprocess = strip.Split('|')[0];
                    //taskresult.simprocessid = Int32.Parse(strip.Split('|')[1]);
                    //taskresult.user = strip.Split('|')[2];
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

        public static SimulationPlaybookResult GetPlaybookResult(string results)
        {

            SimulationPlaybookResult playbookresult = new SimulationPlaybookResult();
            List<PlaybookTaskResult> taskresults = new List<PlaybookTaskResult>();
            string[] lines = results.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            //foreach (string line in lines)
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("Starting"))
                {
                    PlaybookTaskResult taskresult = new PlaybookTaskResult();
                    List<TaskDebugMsg> debugmsgs = new List<TaskDebugMsg>();

                    taskresult.timestamp = lines[i].Substring(0, lines[i].IndexOf('[')).Trim();
                    string strip = lines[i].Substring(lines[i].LastIndexOf("]") + 1).Replace("Starting ", "").Replace("Simulation on ", "").Trim();
                    taskresult.technique = strip.Split(' ')[0];
                    playbookresult.host = strip.Split(' ')[1];
                    bool finished = false;
                    int skipped = 0;
                    for (int k = i + 1; finished == false; k++)
                    {
                        if (lines[k].Contains("Simulator"))
                        {
                            //string strip = line.Substring(line.LastIndexOf("]") + 1).Replace("Simulator running from ", "").Replace("with PID:", "").Trim();
                            //string strip = line.Substring(line.LastIndexOf("]") + 1).Replace("Simulator running from ", "").Replace("with PID:", "").Replace("as ", "").Trim();
                            strip = lines[k].Substring(lines[k].LastIndexOf("]") + 1).Replace("Simulator running from ", "").Replace("with PID:", "|").Replace("as ", "|").Trim();

                            playbookresult.simprocess = strip.Split('|')[0].TrimEnd();
                            playbookresult.simprocessid = Int32.Parse(strip.Split('|')[1]);
                            playbookresult.user = strip.Split('|')[2];
                        }
                        else if (lines[k].Contains("Simulation Finished"))
                        {
                            taskresult.success = true;
                            finished = true;
                        }
                        else if (lines[k].Contains("Simulation Failed"))
                        {
                            taskresult.success = false;
                            finished = true;
                        }
                        else
                        {
                            TaskDebugMsg debugmsg = new TaskDebugMsg();
                            debugmsg.msg = lines[k];
                            debugmsgs.Add(debugmsg);
                        }
                        skipped += 1;
                    }
                    taskresult.debugmsgs = debugmsgs;
                    taskresults.Add(taskresult);
                    i += skipped;

                }


                //Console.WriteLine(line.Substring(line.LastIndexOf(']') + 1));
            }
            playbookresult.taskresults = taskresults;
            return playbookresult;
            //File.WriteAllText("result.json", JsonConvert.SerializeObject(taskresult));
            
        }
        public static void WriteJsonPlaybookResults(SimulationExerciseResult engagementResult, string outputfile)
        {
            File.WriteAllText(outputfile, JsonConvert.SerializeObject(engagementResult));

        }

        public static void WriteJsonPlaybookResults(SimulationPlaybookResult playbookResult, string outputfile)
        {
            File.WriteAllText(outputfile, JsonConvert.SerializeObject(playbookResult));

        }

        public static void ExportAttackLayer(string[] techniques)
        {

            NavigatorLayer layer = new NavigatorLayer();
            layer.version = "3.0";
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
                tech.score = 1;

                layertechniques.Add(tech);
            }
            layer.techniques = layertechniques;
            File.WriteAllText("PurpleSharp_navigator.json", JsonConvert.SerializeObject(layer));
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
            List<SimulationPlaybook> playbooks = new List<SimulationPlaybook>();
            int notsupported = 0;

            foreach (NavigatorTechnique technique in layer.techniques)
            {

                if (Array.IndexOf(supportedtechniques, technique.techniqueID) > -1)
                {
                    SimulationPlaybook playbook = new SimulationPlaybook();
                    playbook.name = layer.name;
                    playbook.host = "random";
                    playbook.scoutfpath = @"C:\Windows\Psexesvc.exe";
                    playbook.simrpath = @"\Downloads\Firefox_Installer.exe";
                    List<PlaybookTask> tasks = new List<PlaybookTask>();


                    PlaybookTask task = new PlaybookTask();
                    task.technique = technique.techniqueID;
                    tasks.Add(task);
                    playbook.tasks = tasks;
                    playbooks.Add(playbook);
                }
                else 
                {
                    Console.WriteLine("[!] {0} not supported by PurpleSharp",technique.techniqueID);
                    notsupported += 1;
                }
                
            }
            engagement.playbooks = playbooks;
            //playbook.tasks = tasks;
            //engagement.playbooks = new List<SimulationPlaybook>();
            //engagement.playbooks.Add(playbook);
            Console.WriteLine("[!] Found a total of {0} techniques not supported out of {1}", notsupported.ToString(), layer.techniques.Count.ToString());
            Console.WriteLine("[!] Final number of tasks supported: {0}", playbooks.Count.ToString());
            return engagement;
        }

        public static void CreateSimulationExercise(SimulationExercise engagement)
        {
            File.WriteAllText("simulation.json", JsonConvert.SerializeObject(engagement));

        }



    }
}