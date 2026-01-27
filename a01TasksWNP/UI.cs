/*
*	FILE	        :   UI.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   
*   DESCRIPTION     :   
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide {
    internal class UI {

        /*
        Method        : Broadcast
        Description   : 
        Parameters    : string msg      :   message to broadcast to clients.
                       
        Return Values : N/A
        */
        internal static void Broadcast(string msg, List<TcpClient> clients) {
            byte[] byteData = Encoding.ASCII.GetBytes(msg);
            
            foreach (TcpClient client in clients) {
                client.GetStream().Write(byteData, 0, byteData.Length);
            }

            return;
        }

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
