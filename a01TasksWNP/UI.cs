/*
*	FILE	        :   UI.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   January 31, 2026
*   DESCRIPTION     :   This is the file responsible for communicating any issues
*                       to the console or to the connected clients.
*/

using System.Net.Sockets;
using System.Text;

namespace ServerSide {
    internal class UI {

        /*
        Method        : DisplayMessage
        Description   : This will display any message that may be pertinent to the
                        server.
        Parameters    : string msg      :   message to display to user
        Return Values : N/A
        */
        internal static void DisplayMessage(string msg) {
            Console.WriteLine(msg);
            
            return;
        }
    }
}
