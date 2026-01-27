using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientSide
{
    internal class ClientListener {
        private int kMaxByteSize = 1024;
        private int kMaxFileSize;
        private readonly CancellationToken _Token;
        public ClientListener (CancellationToken token){ 
            _Token = token;
        }

        internal void StartListener() {
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

                while (!_Token.IsCancellationRequested) {

                    // console writing used for debugging
                    Console.WriteLine("Waiting for connection...\n");

                    TcpClient server = client.AcceptTcpClient();

                    // more dubigging console writes
                    Console.WriteLine("Connected!");
                    
                    NetworkStream stream = server.GetStream();
                    int i = stream.Read(bytes, 0, bytes.Length);
                    string data = Encoding.ASCII.GetString(bytes, 0, i);
                    if(data.StartsWith("Cancel Token")){
                        ClientProgram.CancelToken();
                        Console.WriteLine("Server has disconnected.");
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
