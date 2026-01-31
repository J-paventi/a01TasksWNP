/*
*	FILE	        :   ServerProgram.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   January 31, 2026
*   DESCRIPTION     :   This file contains all the logic for the server to
*                       run. It creates the tasks the server will use and
*                       controls the method to kill the tasks the server has
*                       created.
*/

using System.Net.Sockets;
using ServerSide;

namespace a01TasksWNP {
    internal class ServerProgram {
        private static List<TcpClient> clients = new List<TcpClient>();
        private static CancellationTokenSource cts = new CancellationTokenSource(); 

        static async Task Main(string[] args) {
            CancellationToken token = cts.Token;

            // Create listener for a client request and start the listener
            ServerListener listener = new ServerListener();
            Task serverListener = listener.StartListener(token, clients);

            FileMonitor monitor = new FileMonitor();
            Task fileMonitor = monitor.Monitor(token);

            await Task.WhenAll(serverListener, fileMonitor);
        }

        /*
        Method        : CancelToken
        Description   : This method cancels the cancellation token
                        thereby killing all of the server's tasks
        Parameters    : N/A
        Return Values : N/A
        */
        internal static void CancelToken(){ 
            UI.Broadcast("Cancel Token", clients);

            clients.Clear();

            cts.Cancel();
        }
    }
}
