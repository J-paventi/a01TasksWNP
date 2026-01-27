/*
*	FILE	        :   ServerListener.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   
*   DESCRIPTION     :   
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


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
                    Console.WriteLine("\nWaiting for connection...\n");

                    client = await server.AcceptTcpClientAsync();
                    
                    if (!ct.IsCancellationRequested){ 
                        // more dubigging console writes
                        Console.WriteLine("Connected!");

                        clients.Add(client);

                        Reciever work = new Reciever(ct);
                        Task worker = work.WorkerTask(client, ct);
                    }

                }
                //send kill command to clients
                //NetworkStream stream = client.GetStream();
                //byte[] byteData = Encoding.ASCII.GetBytes("Cancel Token");
                //stream.Write(byteData, 0, byteData.Length);

                
                //UI.Broadcast("Cancel Token", clients);



            } catch (Exception ex) {
                Console.WriteLine(ex.Message);      // move to UI class when created
            } finally {
                // stops the server as the final step of try/catch
                server.Stop();
            }

            return;
        }

    }
}
