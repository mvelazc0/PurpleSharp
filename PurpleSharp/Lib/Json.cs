using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static PlaybookTaskResult GetTaskResult(string results, string user)
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
                    taskresult.user = user;
                }
                else if (line.Contains("Simulation agent"))
                {
                    string strip = line.Substring(line.LastIndexOf("]") + 1).Replace("Simulation agent running as ", "").Replace("with PID:", "").Trim();

                    taskresult.simprocess = strip.Split(' ')[0];
                    taskresult.simprocessid = Int32.Parse(strip.Split(' ')[1]);
                    //taskresult.simprocessid = 111;
                    //taskresult.success = true;
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


    }
}
