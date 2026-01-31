/*
*	FILE	        :   ClientProgram.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   January 31, 2026
*   DESCRIPTION     :   This is the main program for the client. It creates and runs the 
*                       clients and contains the cancellation token that will allow the
*                       tasks to cancel if there is an error or if the server has closed.
*/

using System.Configuration;

namespace ClientSide {
    internal class ClientProgram {
        private static CancellationTokenSource cts = new CancellationTokenSource(); 
        static async Task Main(string[] args) {
            CancellationToken token = cts.Token;

            // get and parse the number of clients the program generates
            string numOfClients = ConfigurationManager.AppSettings["NumberOfClients"];
            int.TryParse(numOfClients, out int clientCount);
            List<Task> clientTasks = new List<Task>();      // store clients in List

            // create the tasks that each client will use
            for (int i = 0; i < clientCount; i++) {
                Client clientSender = new Client();
                Task sendTask = clientSender.Run(token);
                clientTasks.Add(sendTask);
            }

            try {
                await Task.WhenAll(clientTasks);
            } catch (OperationCanceledException) {
                // cancel clients and inform them they have stopped
                CancelToken();
                UI.DisplayMessage("All clients stopped.");
            }
        }

        /*
        Method        : CancelToken
        Description   : Cancels the cancellation token used by the client.
        Parameters    : N/A
        Return Values : N/A
        */
        internal static void CancelToken(){ 
            cts.Cancel();
        }
    }
}
