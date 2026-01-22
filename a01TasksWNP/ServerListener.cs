using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    /*
    Method        : StartListener
    Description   : 
    Parameters    : N/A
    Return Values : N/A
    */
    internal class ServerListener
    {
        
        internal void StartListener() {
            TcpListener server = null;

            try {
                Int32 port = 5000;      // This should not be hard coded but added into config file
                IPAddress localAddress = IPAddress.Parse("127.0.0.1");    // again added to config file

                server.Start();

                while(true) {
                    // console writing used for debugging
                    Console.WriteLine("Waiting for connection...\n");

                    TcpClient client = server.AcceptTcpClient();

                    // more dubigging console writes
                    Console.WriteLine("Connected!");

                    Task.Run(() => Worker(client));     // I don't understand tasks, clearly, couldn't get this to run without a lambda
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);      // move to UI class when created
            }
            finally {
                // stops the server as the final step of try/catch
                server.Stop();
            }

            return;
        }

        /*
        Method        : Worker
        Description   : 
        Parameters    : N/A
        Return Values : N/A
        */
        public void Worker(Object task){     // should probably give this a better name than "Worker"
            // cast the object to a TcpClient object
            TcpClient client = (TcpClient)task;

            Byte[] bytes = new byte[1024];      // this can be changed, literally just a default value I'm using
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
        }
    }
}
