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
    }
}
