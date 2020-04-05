using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Lib
{

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
    }
}
