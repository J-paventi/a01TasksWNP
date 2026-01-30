/*
*	FILE	        :   ClientProgram.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   
*   DESCRIPTION     :   
*/

using System.Configuration;


namespace ClientSide {
    internal class ClientProgram {
        private static CancellationTokenSource cts = new CancellationTokenSource(); 
        static async Task Main(string[] args) {
            CancellationToken token = cts.Token;

            //ClientListener clientListener = new ClientListener(token);
            //Task listenTask = clientListener.StartListener(token);
            //Task listenTask = new Task(StartListener, token);

            string numOfClients = ConfigurationManager.AppSettings["NumberOfClients"];
            int.TryParse(numOfClients, out int clientCount);
            List<Task> clientTasks = new List<Task>();

            for (int i = 0; i < clientCount; i++) {
                Client clientSender = new Client();
                Task sendTask = clientSender.Run(token);
                clientTasks.Add(sendTask);
            }

            try {
                await Task.WhenAll(clientTasks);
            } catch (OperationCanceledException) {
                CancelToken();
                Console.WriteLine("All clients stopped.");
            }
        }
        
        internal static void CancelToken(){ 
            cts.Cancel();
        }
    }
}
