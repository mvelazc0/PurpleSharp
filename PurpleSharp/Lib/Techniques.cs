using System.Linq;

namespace PurpleSharp.Lib
{
    public static class Techniques
    {

        /*
         string[] execution = new string[] { "T1053", "T1053.005", "T1059", "T1059.001", "T1059.003", "T1059.005", "T1059.007", "T1569", "T1569.002" };
         string[] persistence = new string[] { "T1053", "T1053.005", "T1136", "T1136.001", "T1197", "T1543", "T1543.003", "T1546", "T1546.003", "T1547", "T1547.001" };
         string[] privilege_escalation = new string[] { "T1053.005", "T1134", "T1134.004" , "T1543.003", "T1547.001", "T1546.003", "T1055", "T1055.002", "T1055.004"};
         string[] defense_evasion = new string[] { "T1055.002", "T1055.003", "T1055.004", "T1070", "T1070.001", "T1218", "T1218.003", "T1218.010", "T1218.004", "T1218.005", "T1218.009", "T1218.011", "T1140", "T1197", "T1134", "T1134.004", "T1220" };
         string[] credential_access = new string[] { "T1003", "T1003.001", "T1110", "T1110.003", "T1558", "T1558.003"};
         string[] discovery = new string[] { "T1135", "T1046", "T1087", "T1087.001", "T1087.002", "T1007", "T1033", "T1049", "T1016", "T1018", "T1083", "T1482", "T1201","T1069", "T1069.001", "T1069.002", "T1012", "T1518", "T1518.001", "T1082", "T1124" };
         string[] lateral_movement = new string[] { "T1021", "T1021.002", "T1021.006", "T1047" };
         string[] supported_techniques = execution.Union(persistence).Union(privilege_escalation).Union(defense_evasion).Union(credential_access).Union(discovery).Union(lateral_movement).ToArray();
         */
        //should move this to sqlite, an embedded resource or an external JSON file.


        public static string[] unsupported_techniques = new string[] { "T1189", "T1190", "T1133", "T1200", "T1566", "T1566.001", "T1566.002", "T1566.003", "T1091", "T1195", "T1195.001", "T1195.002", "T1195.003", "T1199", "T1078", "T1078.001", "T1078.002", "T1078.003", "T1557", "T1557.001", "T1557.002", "T1557.003", "T1560", "T1560.001", "T1560.002", "T1560.003", "T1123", "T1119", "T1185", "T1115", "T1213", "T1213.002", "T1005", "T1039", "T1025", "T1074", "T1074.001", "T1074.002", "T1114", "T1114.001", "T1114.002", "T1114.003", "T1056", "T1056.001", "T1056.002", "T1056.003", "T1056.004", "T1113", "T1125", "T1071", "T1071.001", "T1071.002", "T1071.003", "T1071.004", "T1092", "T1132", "T1132.001", "T1132.002", "T1001", "T1001", "T1001.001", "T1001.002", "T1001.003", "T1568", "T1568.001", "T1568.002", "T1568.003", "T1573", "T1573.001", "T1573.002", "T1008", "T1105", "T1104", "T1095", "T1571", "T1572", "T1090", "T1090.001", "T1090.002", "T1090.003", "T1090.004", "T1219", "T1205", "T1205.001", "T1205.002", "T1102", "T1102.001", "T1102.002", "T1102.003", "T1020", "T1030", "T1048", "T1048.001", "T1048.002", "T1048.003", "T1041", "T1011", "T1011.001", "T1052", "T1052.001", "T1567", "T1567.001", "T1567.002", "T1029", "T1531", "T1485", "T1486", "T1565", "T1565.001", "T1565.002", "T1565.003", "T1491", "T1491.001", "T1491.002", "T1561", "T1561.001", "T1561.002", "T1499", "T1499.001", "T1499.002", "T1499.003", "T1499.004", "T1495", "T1490", "T1498", "T1498.001", "T1498.002", "T1496", "T1489", "T1529"};
        public static string[] execution_techniques = new string[] { "T1053", "T1059", "T1569" };
        public static string[] execution_subtechniques = new string[] { "T1053.005", "T1059.001", "T1059.003", "T1059.005", "T1059.007", "T1569.002" };
        public static string[] persistence_techniques = new string[] { "T1053", "T1136", "T1197", "T1543", "T1546", "T1547" };
        public static string[] persistence_subtechniques = new string[] { "T1053.005", "T1136.001", "T1543.003", "T1546.003", "T1547.001" };
        public static string[] privilege_escalation_techniques = new string[] { "T1134", "T1055", };
        public static string[] privilege_escalation_subtechniques = new string[] { "T1053.005", "T1134.004", "T1543.003", "T1547.001", "T1546.003", "T1055.002", "T1055.004" };
        public static string[] defense_evasion_techniques = new string[] { "T1055", "T1070", "T1218", "T1140", "T1197", "T1134", "T1220" };
        public static string[] defense_evasion_subtechniques = new string[] { "T1055.002", "T1055.003", "T1055.004", "T1070.001", "T1218.003", "T1218.010", "T1218.004", "T1218.005", "T1218.009", "T1218.011", "T1134.004" };
        public static string[] credential_access_techniques = new string[] { "T1003", "T1110", "T1558" };
        public static string[] credential_access_subtechniques = new string[] { "T1003.001", "T1110.003", "T1558.003" };
        public static string[] discovery_techniques = new string[] { "T1135", "T1046", "T1087", "T1007", "T1033", "T1049", "T1016", "T1018", "T1083", "T1482", "T1201", "T1069", "T1012", "T1518", "T1082", "T1124" };
        public static string[] discovery_subtechniques = new string[] { "T1087.001", "T1087.002", "T1007", "T1069.001", "T1069.002", "T1518.001" };
        public static string[] lateral_movement_techniques = new string[] { "T1021", "T1047" };
        public static string[] lateral_movement_subtechniques = new string[] { "T1021.002", "T1021.006" };
        public static string[] supported_techniques = execution_techniques.Union(persistence_techniques).Union(privilege_escalation_techniques).Union(defense_evasion_techniques).Union(credential_access_techniques).Union(discovery_techniques).Union(lateral_movement_techniques).ToArray();
        public static string[] supported_subtechniques = execution_subtechniques.Union(persistence_subtechniques).Union(privilege_escalation_subtechniques).Union(defense_evasion_subtechniques).Union(credential_access_subtechniques).Union(discovery_subtechniques).Union(lateral_movement_subtechniques).ToArray();



    }
}
