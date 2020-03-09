using Microsoft.Win32;
using System;


namespace PurpleSharp.Simulations
{
    public class PersistenceHelper
    {
        public void Persistence_SchTask()
        {
            /*
            using (var ts = new TaskService())
            {
                var task = ts.NewTask();
                task.Actions.Add(new ExecAction("cmd.exe",
                    string.Format("/c net stop {0} & net start {0}", serviceName)));
                task.Triggers.Add(
                    new DailyTrigger() { StartBoundary = DateTime.Today.AddHours(19) });
                task.Principal.RunLevel = TaskRunLevel.Highest;
                ts.RootFolder.RegisterTaskDefinition(serviceName + " Restart", task);
            }*/

        }

        public static void Persistence_RegKey()
        {

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            registryKey.SetValue("ApplicationName", "aaaaaaaaaa");

            Console.WriteLine("done");

        }

        public static void CreateUser(String username)
        {
            //https://stackoverflow.com/questions/1100926/pinvoke-of-netuseradd-returns-24
            Structs.USER_INFO_2 userInfo2 = new Structs.USER_INFO_2()
            {
                acct_expires = long.MaxValue,
                auth_flags = 0, // Must be 0 for NetUserAddCalls
                bad_pw_count = -1,  //ignored for NetUserAdd calls
                //code_page = ?,
                comment = "H4x0r Account",
                //country_code = ?,
                flags = 0x10000,// & UF_ACCOUNTDISABLE,
                full_name = "h4x0r",
                home_dir = "",
                last_logoff = 0,
                last_logon = 0,
                logon_hours = IntPtr.Zero, // User is given no logon time.
                logon_server = "", //ignored for NetUserAdd calls
                max_storage = 0,
                name = username,
                num_logons = -1, //ignored for NetUserAdd calls
                parms = "",
                password = "Passw0rd123$%^Elite98765432102",
                password_age = -1,
                priv = 1,
                script_path = "",
                units_per_week = -1, //ignored for NetUserAdd calls
                usr_comment = "",
                workstations = ""
            };

            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            Lib.Logger logger = new Lib.Logger(currentPath + "PurpleSharp.txt");
            logger.TimestampInfo(String.Format("Starting Create Account Simulation on {0}", Environment.MachineName));

            uint output;
            int result = WinAPI.NetUserAdd(null, 2, userInfo2, out output);
            if (result == 0)
            {
                Console.WriteLine("Successfully created local user");
                logger.TimestampInfo(String.Format("Successfully created local user {0}", username));
            }
            else
            {

                Console.WriteLine("Could not create user");
                logger.TimestampInfo(String.Format("Could not create local user {0}. Error code: {1} ", username, result.ToString()));

            }

            //#TODO Look addmin group with LookupAccountSid: https://www.pinvoke.net/default.aspx/netapi32.netlocalgroupaddmembers

            Structs.LOCALGROUP_MEMBERS_INFO_3 info;
            info.Domain = username;
            int result2 = WinAPI.NetLocalGroupAddMembers(null, "Administrators", 3, ref info, 1);
            if (result2 == 0)
            {
                Console.WriteLine("Successfully added created user to the Administrators group");
                logger.TimestampInfo("Successfully added created user to the Administrators group");
            }

            if (result == 0)
            {
                int result3 = WinAPI.NetUserDel(null, username);
                if (result3 == 0)
                {
                    logger.TimestampInfo(String.Format("Successfully removed user {0}", username));
                    
                }
                else
                {
                    Console.WriteLine("Could not delete user");
                    logger.TimestampInfo("Could not delete user");
                    Console.WriteLine(result3);
                }
            }

        }
    }
}