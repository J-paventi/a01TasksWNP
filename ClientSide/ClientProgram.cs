/*
*	FILE	        :   ClientProgram.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   
*   DESCRIPTION     :   
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ClientSide {
    internal class ClientProgram {
        private static CancellationTokenSource cts = new CancellationTokenSource(); 
        static async Task Main(string[] args) {
            CancellationToken token = cts.Token;

            ClientListener clientListener = new ClientListener(token);
            Task listenTask = new Task(clientListener.StartListener, token);
            //Task listenTask = new Task(StartListener, token);


            Client clientSender = new Client(token);
            Task sendTask = new Task(clientSender.Run);
            sendTask.Start();
            sendTask.Wait();
        }
        
        internal static void CancelToken(){ 
            cts.Cancel();
        }
    }
}
