/*
*	FILE	        :   ServerListener.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   
*   DESCRIPTION     :   
*/

using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace ServerSide {

    /*
    Method        : StartListener
    Description   : 
    Parameters    : N/A
    Return Values : N/A
    */
    internal class ServerListener {

        private int kMaxByteSize = 1024;
        private int kMaxFileSize;
        
        internal void StartListener() {
            TcpListener server = null;
            string serverIP = ConfigurationManager.AppSettings["ServerIP"];
            string serverPortStr = ConfigurationManager.AppSettings["ServerPort"];
            string serverBufferSize = ConfigurationManager.AppSettings["BufferSize"];
            int.TryParse(serverBufferSize, out kMaxByteSize);
            string serverMaxClients = ConfigurationManager.AppSettings["MaxClients"];
            string serverMaxFileSize = ConfigurationManager.AppSettings["MaxFileSize"];
            int.TryParse(serverMaxFileSize, out kMaxFileSize);

            try {
                int port = 0;
                int.TryParse(serverPortStr, out port);
                IPAddress localAddress = IPAddress.Parse(serverIP);    // again added to config file


                server = new TcpListener(localAddress, port);

                server.Start();

                while(true) {

                    // console writing used for debugging
                    Console.WriteLine("Waiting for connection...\n");

                    TcpClient client = server.AcceptTcpClient();

                    // more dubigging console writes
                    Console.WriteLine("Connected!");

                    Task.Run(() => Worker(client));     // I don't understand tasks, clearly, couldn't get this to run without a lambda
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);      // move to UI class when created
            } finally {
                // stops the server as the final step of try/catch
                server.Stop();
            }

            return;
        }

        /*
        Method        : Worker
        Description   : 
        Parameters    : Object task     :   the task object that is passed to the Worker
                                            in order to have it work independently
        Return Values : N/A
        */
        public void Worker(Object task){     // should probably give this a better name than "Worker"
            // cast the object to a TcpClient object
            TcpClient client = (TcpClient)task;

            Byte[] bytes = new byte[kMaxByteSize];      // this can be changed, literally just a default value I'm using
            string data = null;

            NetworkStream stream = client.GetStream();

            int i;

            // currently this is just receiving the client communication and not doing anything
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) {
                data = Encoding.ASCII.GetString(bytes, 0, i);

                // console write for debugging
                Console.WriteLine("Received: {0}\n", data);
            }

            // sutdown and end connection
            client.Close();

            return;
        }
    }
}
