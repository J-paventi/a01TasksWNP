/*
*	FILE	        :   a01TasksWNP.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   
*   DESCRIPTION     :   
*/

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Timers;
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
            //Task fileMonitor = new Task(FileMonitor, token); 

            await Task.WhenAll(serverListener, fileMonitor);
        }
        
        internal static void CancelToken(){ 
            UI.Broadcast("Cancel Token", clients);

            clients.Clear();

            cts.Cancel();
        }
    }
}
