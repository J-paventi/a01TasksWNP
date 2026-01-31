/*
*	FILE	        :   ServerListener.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   
*   DESCRIPTION     :   
*/

using System.Configuration;
using System.Net;
using System.Net.Sockets;


namespace ServerSide {
    /*
    Method        : StartListener
    Description   : 
    Parameters    : N/A
    Return Values : N/A
    */
    internal class ServerListener {
        
        internal async Task StartListener(CancellationToken ct, List<TcpClient> clients) {

            TcpListener server = null;
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

                while (!ct.IsCancellationRequested) {

                    // console writing used for debugging
                    //UI.DisplayMessage("\nWaiting for connection...\n");

                    client = await server.AcceptTcpClientAsync();
                    
                    if (!ct.IsCancellationRequested){ 
                        // more dubigging console writes
                        //UI.DisplayMessage("Connected!");

                        clients.Add(client);

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
