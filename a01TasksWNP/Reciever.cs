/*
*	FILE	        :   Receiver.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   January 31, 2026
*   DESCRIPTION     :   
*/

using System.Configuration;
using System.Net.Sockets;
using System.Text;

namespace ServerSide {

    /// <summary>
    /// 
    /// </summary>
    internal class Reciever {
        /*
        Method        : WorkerTask
        Description   : 
        Parameters    : Object task             :   the task object that is passed to the Worker
                                                    in order to have it work independently
                        CancellationToken ct    :   Token used to indicate to the worker if the
                                                    task has been cancelled.
        Return Values : Task                    :   As an Async method, it is required to return
                                                    a task. This allows the method to return control
                                                    to its caller.
        */
        internal async Task WorkerTask(TcpClient client, CancellationToken ct) {
            // read and parse buffer size from appconfig
            string buffer = ConfigurationManager.AppSettings["BufferSize"];
            int.TryParse(buffer, out int bufferSize);

            Byte[] bytes = new byte[bufferSize];

            NetworkStream stream = client.GetStream();

            // read and parse the file path from appconfig
            string filePath = ConfigurationManager.AppSettings["FilePath"];

            bool doneReading = false;

            while (!doneReading && !ct.IsCancellationRequested) {
                int i = await stream.ReadAsync(bytes, 0, bytes.Length, ct);
                if (i == 0) doneReading = true;

                string data = Encoding.ASCII.GetString(bytes, 0, i);
                FileIO.FileWrite(filePath, data);
            }

            return;
        }
    }
}
