/*
*	FILE	        :   Client.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   
*   DESCRIPTION     :   
*/

using System.Configuration;
using System.Net.Sockets;
using System.Text;

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
            string serverBufferSize = ConfigurationManager.AppSettings["BufferSize"];
            int.TryParse(serverBufferSize, out int maxBufferSize);
            byte[] buffer = new byte[maxBufferSize];

            while (!ct.IsCancellationRequested) {
                try{
                    if(Connect()) { 
                        SendData(GenerateData());

                        /*if (stream.DataAvailable) {
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                            string msg = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                            if (msg.StartsWith("Cancel Token")) {
                                ClientProgram.CancelToken();
                            }
                        }*/
                    
                        Disconnect();
                        await Task.Delay(50);
                    } else {
                        await Task.Delay(10000);
                    }
                } catch (Exception e){
                        //Server is closed. clients should stop.
                        UI.DisplayMessage($"Client Run Error: {e}");
                        ClientProgram.CancelToken();
                    }
                }

            return;
        }
        /*
       Method        : Connect
       Description   : 
       Parameters    : N/A
       Return Values : N/A
       */
        internal bool Connect() {
            bool result = false;
            //Retrieve & parse server ip & port from config.
            string serverIP = ConfigurationManager.AppSettings["ServerIP"];
            string serverPortStr = ConfigurationManager.AppSettings["ServerPort"];
            int serverPort = 0;
            int.TryParse(serverPortStr, out serverPort);

            try {
                //Establist connection to server.
                client = new TcpClient(serverIP, serverPort);
                stream = client.GetStream();
                result = true;
            } catch (SocketException ex) {
                    Disconnect();

                    bool handled = false;
                    if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse) {
                        UI.DisplayMessage("Port exhaustion detected - delaying 10 seconds...");
                        handled = true;
                    }

                    if (!handled && ex.SocketErrorCode == SocketError.ConnectionRefused) {
                        UI.DisplayMessage("Server refused connection - cancelling client.");
                        ClientProgram.CancelToken();
                        handled = true;
                    }

                    if (!handled) {
                        UI.DisplayMessage($"Client Run: Unhandled socket error: {ex.SocketErrorCode}");
                        ClientProgram.CancelToken();
                    }
            } catch (Exception e){
                UI.DisplayMessage($"Client Connect Error: {e}");
                result = false;
                ClientProgram.CancelToken();
            }

            return result;
        }
        /*
       Method        : Disconnect
       Description   : 
       Parameters    : N/A
       Return Values : N/A
       */
        internal void Disconnect() {
            try {
                stream?.Close();
                stream?.Dispose();
            } catch {}

            try {
                client?.Close();
                client?.Dispose();
            } catch {}

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
            //UI.DisplayMessage(data);
            //UI.DisplayMessage(max);

            return data;
        }
    }
}