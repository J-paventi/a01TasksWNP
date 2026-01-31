/*
*	FILE	        :   ClientProgram.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   January 31, 2026
*   DESCRIPTION     :   A class that is responsible for printing any messages
*                       to the screen. 
*/

namespace ClientSide {
    internal class UI {
        /*
        Method        : DisplayMessage
        Description   : 
        Parameters    : string msg      :   message to display to user
        Return Values : N/A
        */
        internal static void DisplayMessage(string msg) {
            Console.WriteLine(msg);
            
            return;
        }
    }
}
