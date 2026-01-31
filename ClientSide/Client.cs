/*
*	FILE	        :   Client.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   January 31, 2026
*   DESCRIPTION     :   This file contains the logic that allows the client to do
*                       the things it is required to do. It handles all of the connection,
*                       disconnection, writing and sending of data to the server, as well
*                       as catching any errors that may occur. 
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
        Description   : This method takes the majority of the logic required by the client and
                        encapsulates it into a method that is responsible for executing all the
                        things that the client needs to do while running.
        Parameters    : CancellationToken ct    :   This token is required to maintain consistency
                                                    throughout the program with the cancellation token.
                                                    It allows the method to know when the token is 
                                                    cancelled.
        Return Values : Task                    :   As an Async method, it is required to return
                                                    a task. This allows the method to return control
                                                    to its caller.
        */
        internal async Task Run(CancellationToken ct) {
            string serverBufferSize = ConfigurationManager.AppSettings["BufferSize"];
            int.TryParse(serverBufferSize, out int maxBufferSize);
            byte[] buffer = new byte[maxBufferSize];

            // while the canellation token is valid, this while loop runs
            while (!ct.IsCancellationRequested) {
                try{
                    if(Connect()) { 
                        SendData(GenerateData());
                    
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
       Description   : This method contains all the logic that is necessary for the client to connect to the
                        server. It informs the client if port exhaustion occurs, something that occurs when
                        all available ports have been used within the last two minutes.
       Parameters    : N/A
       Return Values : bool     :   This method returns a bool depending on the client's ability to 
                                    connect to the server or not
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
                    
                    // if there is a socet excpetion that occurs due to port exhastion
                    bool handled = false;
                    if (ex.SocketErrorCode == SocketError.AddressAlreadyInUse) {
                        // displays a message to the client allowing them to know that they
                        // program is waiting for an available port to come back online
                        UI.DisplayMessage("Port exhaustion detected - delaying 10 seconds...");
                        handled = true;
                    }

                    // if the connection with the server has been closed, this error occurs
                    if (!handled && ex.SocketErrorCode == SocketError.ConnectionRefused) {
                        UI.DisplayMessage("Server refused connection - cancelling client.");
                        ClientProgram.CancelToken();
                        handled = true;
                    }

                    // for any other socket exceptions that are not the previous two
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
       Description   : This method handles the logic for the client to disconnect from the server.
                        It ensures that the stream and client have properly closed and clears them.
       Parameters    : N/A
       Return Values : N/A
       */
        internal void Disconnect() {
            try {
                stream?.Close();
                stream?.Dispose();      // failsafe in case the garbage collector isn't keeping up
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
        Description   : This method contains the logic for packing the data generated by the client.
                        If for some reason it cannot pack the data it cancels the token ending the
                        client object.
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
            }

            return;
        }

        /*
        Method        : GenerateData
        Description   : This method randomly generates a number between 1 and 100. It then
                        creates GUIDs and appends them all into one single string that is
                        returned to the client Run method.
        Parameters    : N/A  
        Return Values : string          :   Returns a string of GUIDs that have been turned
                                            into a single string
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