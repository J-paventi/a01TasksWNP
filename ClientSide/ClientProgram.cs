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

            //ClientListener clientListener = new ClientListener(token);
            //Task listenTask = new Task(clientListener.StartListener, token);
            Task listenTask = new Task(StartListener, token);


            Client clientSender = new Client(token);
            Task sendTask = new Task(clientSender.Run);
            sendTask.Start();
            sendTask.Wait();
        }

        internal static void StartListener() {
            TcpListener client = null;
            try {
                string serverIP = ConfigurationManager.AppSettings["ServerIP"];
                string serverPortStr = ConfigurationManager.AppSettings["ServerPort"];
                int port = 0;
                int.TryParse(serverPortStr, out port);
                IPAddress localAddress = IPAddress.Parse(serverIP);    // again added to config file


                client = new TcpListener(localAddress, port);

                client.Start();

                Byte[] bytes = new byte[64]; 

                while (!cts.IsCancellationRequested) {

                    // console writing used for debugging
                    Console.WriteLine("Waiting for connection...\n");

                    TcpClient server = client.AcceptTcpClient();

                    // more dubigging console writes
                    Console.WriteLine("Connected!");
                    
                    NetworkStream stream = server.GetStream();
                    int i = stream.Read(bytes, 0, bytes.Length);
                    string data = Encoding.ASCII.GetString(bytes, 0, i);
                    if(data.StartsWith("Cancel Token")){
                        cts.Cancel();
                    }

                    //Task.Run(() => Worker.WorkerTask(client, kMaxByteSize));     // I don't understand tasks, clearly, couldn't get this to run without a lambda
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);      // move to UI class when created
            } finally {
                // stops the server as the final step of try/catch
                client.Stop();
            }

            return;
        }
    }
}
