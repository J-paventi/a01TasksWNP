/*
*	FILE	        :   Logger.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   January 31, 2026
*   DESCRIPTION     :   This file logs any data sent to it by the server.
*/

using System.Configuration;

namespace ServerSide {
    internal class Logger {
        
        /*
        Method        : LogMessage
        Description   : 
        Parameters    : string message     :   message to log.
        Return Values : N/A
        */
        public static void LogMessage(string message) {
            string logPath = ConfigurationManager.AppSettings["LogFile"];

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string line = timestamp + " - " + message + "\n";

            try {
                File.AppendAllText(logPath, line);
            } catch(Exception ex) {
                UI.DisplayMessage("Failed to write to log file: " + ex.Message);
            }

            return;
        }
    }
}