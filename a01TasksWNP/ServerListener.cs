/*
*	FILE	        :   ServerListener.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   January 31, 2026
*   DESCRIPTION     :   This file contains all the logic for the server to
*                       listen for the clients' communication. 
*/

using System.Configuration;
using System.Net;
using System.Net.Sockets;


namespace ServerSide {
    internal class ServerListener {

        /*
        Method        : StartListener
        Description   : This method is the listener that creates and establishes
                        the IP and Port that the server will function on. It also
                        controls the server's maximum number of clients who could
                        possibly connect to it. 
        Parameters    : CancellationToken ct    :   The token required for the tasks to know
                                                    when and if the cancellation token has been
                                                    cancelled.
                        List<TcpClient> clients :   The list of tasks responsible for each client
                                                    that is availble to communicate with the server.
        Return Values : Task                    :   As an Async method, it is required to return
                                                    a task. This allows the method to return control
                                                    to its caller.
        */
        internal async Task StartListener(CancellationToken ct, List<TcpClient> clients) {

            TcpListener server = null;

            // Parse and use the appconfig's parameters for the IP, Port, Buffer, and clients
            string serverIP = ConfigurationManager.AppSettings["ServerIP"];
            string serverPortStr = ConfigurationManager.AppSettings["ServerPort"];
            string serverBufferSize = ConfigurationManager.AppSettings["BufferSize"];
            int.TryParse(serverBufferSize, out int maxBufferSize);
            string serverMaxClients = ConfigurationManager.AppSettings["MaxClients"];

            try {
                int port = 0;
                int.TryParse(serverPortStr, out port);
                IPAddress localAddress = IPAddress.Parse(serverIP);    // again added to config file

                server = new TcpListener(localAddress, port);
                server.Start();

                TcpClient client = new TcpClient();

                // the server's lisening loop to accept clients
                while (!ct.IsCancellationRequested) {
                    client = await server.AcceptTcpClientAsync();
                    
                    if (!ct.IsCancellationRequested){ 
                        clients.Add(client);

                        // creates tasks for the server to receive the data
                        Reciever work = new Reciever();
                        Task worker = work.WorkerTask(client, ct);
                    }

                }
            } catch (Exception ex) {
                UI.DisplayMessage(ex.Message);      // move to UI class when created
            } finally {
                // stops the server as the final step of try/catch
                server.Stop();
            }

            return;
        }

    }
}
