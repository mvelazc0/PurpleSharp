using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace PurpleSharp.Lib
{

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
        public static SimulationExerciseResult GetSimulationExerciseResult(string results)
        {
            SimulationExerciseResult simulationresult = new SimulationExerciseResult();
            simulationresult.playbookresults = new List<SimulationPlaybookResult>();
            string[] lines = results.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string strip = "";

                if (lines[i].Contains("Running Playbook"))
                {
                    SimulationPlaybookResult playbookresult = new SimulationPlaybookResult();
                    List<PlaybookTaskResult> taskresults = new List<PlaybookTaskResult>();

                    PlaybookTaskResult taskresult = new PlaybookTaskResult();
                    List<TaskDebugMsg> debugmsgs = new List<TaskDebugMsg>();

                    strip = lines[i].Substring(lines[i].LastIndexOf("]") + 1).Replace("Running Playbook", "").Trim();
                    playbookresult.name = strip;
                    bool finished = false;
                    int skipped = 0;

                    for (int k = i + 1; finished == false; k++)
                    {
                        if (lines[k].Contains("Starting"))
                        {
                            taskresult.timestamp = lines[i].Substring(0, lines[i].IndexOf('[')).Trim();
                            strip = lines[k].Substring(lines[k].LastIndexOf("]") + 1).Replace("Starting ", "").Replace("Simulation on ", "").Trim();
                            taskresult.technique = strip.Split(' ')[0];
                            playbookresult.host = strip.Split(' ')[1];
                        }
                        else if (lines[k].Contains("Simulator"))
                        {
                            strip = lines[k].Substring(lines[k].LastIndexOf("]") + 1).Replace("Simulator running from ", "").Replace("with PID:", "|").Replace("as ", "|").Trim();
                            playbookresult.simprocess = strip.Split('|')[0].TrimEnd();
                            playbookresult.simprocessid = Int32.Parse(strip.Split('|')[1]);
                            playbookresult.user = strip.Split('|')[2];
                        }
                        else if (lines[k].Contains("Simulation Finished"))
                        {
                            //finished = true;
                            taskresult.success = true;
                            taskresult.debugmsgs = debugmsgs;
                            taskresults.Add(taskresult);
                            //playbookresult.taskresults = taskresults;

                            taskresult = new PlaybookTaskResult();
                            debugmsgs = new List<TaskDebugMsg>();
                        }
                        else if (lines[k].Contains("Simulation Failed"))
                        {
                            //finished = true;
                            taskresult.success = false;
                            taskresult.debugmsgs = debugmsgs;
                            taskresults.Add(taskresult);
                            //playbookresult.taskresults = taskresults;

                            taskresult = new PlaybookTaskResult();
                            debugmsgs = new List<TaskDebugMsg>();
                        }
                        else if (lines[k].Contains("Playbook Finished")) 
                        {
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
                    i += skipped;
                    playbookresult.taskresults = taskresults;
                    simulationresult.playbookresults.Add(playbookresult);
                }

            }
            return simulationresult;
        }

        /*
        public static SimulationExerciseResult GetSimulationExerciseResultOld(string results)
        {
            SimulationExerciseResult simulationresult = new SimulationExerciseResult();
            simulationresult.playbookresults = new List<SimulationPlaybookResult>();
            string[] lines = results.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                string strip = "";
                SimulationPlaybookResult playbookresult = new SimulationPlaybookResult();
                List<PlaybookTaskResult> taskresults = new List<PlaybookTaskResult>();


                if (lines[i].Contains("Running Playbook"))
                {
                    strip = lines[i].Substring(lines[i].LastIndexOf("]") + 1).Replace("Running Playbook", "").Trim();
                    playbookresult.name = strip;

                }
                else if (lines[i].Contains("Starting"))
                {
                    PlaybookTaskResult taskresult = new PlaybookTaskResult();
                    List<TaskDebugMsg> debugmsgs = new List<TaskDebugMsg>();

                    taskresult.timestamp = lines[i].Substring(0, lines[i].IndexOf('[')).Trim();
                    strip = lines[i].Substring(lines[i].LastIndexOf("]") + 1).Replace("Starting ", "").Replace("Simulation on ", "").Trim();
                    taskresult.technique = strip.Split(' ')[0];
                    playbookresult.host = strip.Split(' ')[1];
                    bool finished = false;
                    int skipped = 0;
                    for (int k = i + 1; finished == false; k++)
                    {
                        if (lines[k].Contains("Simulator"))
                        {
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
                    playbookresult.taskresults = taskresults;
                    i += skipped;
                    simulationresult.playbookresults.Add(playbookresult);
                }
            }
            return simulationresult;
        }
        */

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
        public static void WriteJsonSimulationResults(SimulationExerciseResult engagementResult, string outputfile)
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
            layer.version = "4.2";
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
                    playbook.remote_host = "random";
                    playbook.scout_full_path = @"C:\Windows\Psexesvc.exe";
                    playbook.simulator_relative_path = @"\Downloads\Firefox_Installer.exe";
                    List<PlaybookTask> tasks = new List<PlaybookTask>();


                    PlaybookTask task = new PlaybookTask();
                    task.technique_id = technique.techniqueID;
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