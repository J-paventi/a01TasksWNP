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
using System.Text;


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
                Logger.LogMessage($"{ex.Message}");
            } finally {
                // stops the server as the final step of try/catch
                server.Stop();
            }

            return;
        }
        
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
    }
}
