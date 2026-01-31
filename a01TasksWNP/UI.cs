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
        Method        : Broadcast
        Description   : The method will broadcast to any connected client any messages the
                        server needs to send to the clients.
        Parameters    : string msg      :   message to broadcast to clients.
                       
        Return Values : N/A
        */
        internal static void Broadcast(string msg, List<TcpClient> clients) {
            byte[] byteData = Encoding.ASCII.GetBytes(msg);

            for (int i = clients.Count - 1; i >= 0; i--)
            {
                TcpClient client = clients[i];

                try
                {
                    if (client.Connected)
                    {
                        NetworkStream stream = client.GetStream();
                        stream.Write(byteData, 0, byteData.Length);
                    }
                    else
                    {
                        clients.RemoveAt(i);
                    }
                }
                catch
                {
                    //dead client, remove it.
                    clients.RemoveAt(i);
                }
            }

            return;
        }

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
