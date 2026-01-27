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
            Connect(); 

            int i = 0;
            while (!ct.IsCancellationRequested) {
                SendData(GenerateData());

                //for debugging
                i++;
                //if (i > 50) break;
            }
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

            //Establist connection to server.
            client = new TcpClient(serverIP, serverPort);
            stream = client.GetStream();

            return;
        }

        /*
        Method        : SendData
        Description   : 
        Parameters    : string data     :   
        Return Values : N/A
        */
        internal void SendData(string data) {
            try {
                byte[] byteData = Encoding.ASCII.GetBytes(data);
                stream.Write(byteData, 0, byteData.Length);
            } catch { 
                ClientProgram.CancelToken();
            }
            //maybe do syn ack

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
            Console.WriteLine(data);
            Console.WriteLine(max);

            return data;
        }
    }
}