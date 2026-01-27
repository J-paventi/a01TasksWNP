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
        private static List<TcpClient> clients = new List<TcpClient>();
        private static CancellationTokenSource cts = new CancellationTokenSource(); 
        
        internal async Task StartListener() {

            TcpListener server = null;
            string serverIP = ConfigurationManager.AppSettings["ServerIP"];
            string serverPortStr = ConfigurationManager.AppSettings["ServerPort"];
            string serverBufferSize = ConfigurationManager.AppSettings["BufferSize"];
            int.TryParse(serverBufferSize, out int maxBufferSize);
            string serverMaxClients = ConfigurationManager.AppSettings["MaxClients"];

            CancellationToken token = cts.Token;

            try {
                int port = 0;
                int.TryParse(serverPortStr, out port);
                IPAddress localAddress = IPAddress.Parse(serverIP);    // again added to config file


                server = new TcpListener(localAddress, port);

                server.Start();

                
                Monitor monitor = new Monitor(token);
                Task fileMonitor = new Task(monitor.FileMonitor, token);
                //Task fileMonitor = new Task(FileMonitor, token); 
                fileMonitor.Start();
                TcpClient client = new TcpClient();

                while (!cts.Token.IsCancellationRequested) {

                    // console writing used for debugging
                    Console.WriteLine("Waiting for connection...\n");

                    client = server.AcceptTcpClient();
                    
                    if (!cts.Token.IsCancellationRequested){ 
                        // more dubigging console writes
                        Console.WriteLine("Connected!");

                        clients.Add(client);

                        Reciever work = new Reciever(token);
                        Task worker = new Task(work.WorkerTask, client, token);
                        worker.Start();
                    }
                }
                fileMonitor.Wait();
                //send kill command to clients
                //NetworkStream stream = client.GetStream();
                //byte[] byteData = Encoding.ASCII.GetBytes("Cancel Token");
                //stream.Write(byteData, 0, byteData.Length);

                
                //UI.Broadcast("Cancel Token", clients);



            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);      // move to UI class when created
            } finally {
                // stops the server as the final step of try/catch
                server.Stop();
            }

            return;
        }

        internal static void CancelToken(){ 
            UI.Broadcast("Cancel Token", clients);
            cts.Cancel();
        }
    }
}
