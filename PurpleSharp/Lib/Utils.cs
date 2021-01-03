using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Lib
{
    class Utils
    {
        public static string GetPassword()
        {
            StringBuilder passwordBuilder = new StringBuilder();
            bool continueReading = true;
            char newLineChar = '\r';

            while (continueReading)
            {
                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                char passwordChar = consoleKeyInfo.KeyChar;

                if (passwordChar == newLineChar)
                {
                    continueReading = false;
                }
                else
                {
                    passwordBuilder.Append(passwordChar.ToString());
                }
            }
            string pass = passwordBuilder.ToString();
            Console.WriteLine();
            return pass;
        }

        public static string GetCurrentTime()
        {
            string datetimeFormat = "MM/dd/yyyy HH:mm:ss";
            return System.DateTime.Now.ToString(datetimeFormat);
        }

        public static string GetPlaybookName(string results)
        {
            string Pbname = "";
            string resu123 = "";
            string[] lines = results.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("Running"))
                {
                    Pbname = lines[i].Substring(lines[i].LastIndexOf("Running ")+ 8).Trim();
                    return Pbname;

                }
            }
            return Pbname;
        }
    }
}
