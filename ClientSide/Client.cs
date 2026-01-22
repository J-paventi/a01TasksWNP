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
        }

    }
}
