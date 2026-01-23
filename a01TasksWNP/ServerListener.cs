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

        struct WorkerTasks{
            public TcpClient client;
            public int maxByteSize;

        };



   
    /*
    Method        : StartListener
    Description   : 
    Parameters    : N/A
    Return Values : N/A
    */
    internal class ServerListener {
        private CancellationTokenSource cts = new CancellationTokenSource(); 
        
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

                
                //Monitor monitor = new Monitor(token);
                //Task fileMonitor = new Task(monitor.FileMonitor, token);
                Task fileMonitor = new Task(FileMonitor, token); 
                fileMonitor.Start();
                TcpClient client = new TcpClient();

                while (!cts.Token.IsCancellationRequested) {

                    // console writing used for debugging
                    Console.WriteLine("Waiting for connection...\n");

                    client = server.AcceptTcpClient();

                    // more dubigging console writes
                    Console.WriteLine("Connected!");

                    WorkerTasks tasks = new WorkerTasks();
                    tasks.client = client;
                    tasks.maxByteSize = maxBufferSize;

                    Reciever work = new Reciever(token);
                    Task worker = new Task(work.WorkerTask, tasks, token);
                    worker.Start();

                }
                fileMonitor.Wait();
                //send kill command to clients
                NetworkStream stream = client.GetStream();
                byte[] byteData = Encoding.ASCII.GetBytes("Cancel Token");
                stream.Write(byteData, 0, byteData.Length);


                /*
                 * make a list of all the CLIENTS an cyce throught hemwhen the kill token is ativated
                 */


            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);      // move to UI class when created
            } finally {
                // stops the server as the final step of try/catch
                server.Stop();
            }

            return;
        }



        /*
        Method        : file Monitor
        Description   : 
        Parameters    : Object task     :   the task object that is passed to the Worker
                                            in order to have it work independently
        Return Values : N/A
        */
        internal void FileMonitor() {
            string serverMaxFileSize = ConfigurationManager.AppSettings["MaxFileSize"];
            int.TryParse(serverMaxFileSize, out int maxFileSize);
            string serverFilePath = ConfigurationManager.AppSettings["FilePath"];

            //Continuously check the file size until stopRequested is true.
            while (!cts.Token.IsCancellationRequested) {//replace with cancellaion token
                try {
                    long currentSize = new FileInfo(serverFilePath).Length;
                    Console.Write("\n[Monitor] File size: {0}", currentSize);

                    //If maximum file size reached, stop all writer threads.
                    if (currentSize >= maxFileSize) {
                        Console.WriteLine("\nReached max file size — stopping writers...");
                        cts.Cancel();
                    } else {//time subject to change
                        //Check file 10 times per second.
                        Thread.Sleep(100);
                    }
                } catch (Exception ex) {
                    //Handle case where file may not yet exist or is inaccessible.
                    Console.WriteLine($"Monitor error: {ex.Message}");
                }
            }
            return;
        }
    }
}
