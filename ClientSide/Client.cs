/*
*	FILE	        :   Client.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   
*   DESCRIPTION     :   
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientSide {
    internal class Client {
        private TcpClient client;
        private NetworkStream stream;
        private CancellationToken ct;
        /*
        Method        : Run
        Description   : 
        Parameters    : N/A
        Return Values : N/A
        */
        internal async Task Run(CancellationToken ct) {
            //Connect(); 
            
            string serverBufferSize = ConfigurationManager.AppSettings["BufferSize"];
            int.TryParse(serverBufferSize, out int maxBufferSize);
            byte[] buffer = new byte[maxBufferSize];

            while (!ct.IsCancellationRequested) {
                try{
                    Connect(); 
            
                    SendData(GenerateData());

                    if (stream.DataAvailable) {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                        string msg = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        if (msg.StartsWith("Cancel Token")) {
                            ClientProgram.CancelToken();
                        }
                    }
                    
                    Disconnect();
                } catch {
                    //Server is closed. clients should stop.
                    ClientProgram.CancelToken();
                }
                await Task.Delay(1, ct);
            }

            return;
        }
        /*
       Method        : Connect
       Description   : 
       Parameters    : N/A
       Return Values : N/A
       */
        internal void Connect() {
            //Retrieve & parse server ip & port from config.
            string serverIP = ConfigurationManager.AppSettings["ServerIP"];
            string serverPortStr = ConfigurationManager.AppSettings["ServerPort"];
            int serverPort = 0;
            int.TryParse(serverPortStr, out serverPort);

            try {
                //Establist connection to server.
                client = new TcpClient(serverIP, serverPort);
                stream = client.GetStream();
            } catch {
                //Server is closed. clients should stop.
                ClientProgram.CancelToken();
            }

            return;
        }
        /*
       Method        : Disconnect
       Description   : 
       Parameters    : N/A
       Return Values : N/A
       */
        internal void Disconnect() {
            stream.Close();
            client.Close();
            stream = null;
            client = null;

            return;
        }

        /*
        Method        : SendData
        Description   : 
        Parameters    : string data     :   
        Return Values : N/A
        */
        internal void SendData(string data) {
            bool canSend = !(client == null || stream == null);

            if (canSend) {
                try {
                    byte[] byteData = Encoding.ASCII.GetBytes(data);
                    stream.Write(byteData, 0, byteData.Length);
                } catch { 
                    ClientProgram.CancelToken();
                }
                //maybe do syn ack
            }

            return;
        }

        /*
        Method        : GenerateData
        Description   : 
        Parameters    : N/A  
        Return Values : string          :   
        */
        internal string GenerateData(){ 
            Random numberOfGUIDS = new Random();
            int max = numberOfGUIDS.Next(1, 101);
            string data = string.Empty;

            for (int i = 0; i < max; i++){
                string part = Guid.NewGuid().ToString();
                data += part; 
            }
            // for debugging
            //Console.WriteLine(data);
            //Console.WriteLine(max);

            return data;
        }
    }
}