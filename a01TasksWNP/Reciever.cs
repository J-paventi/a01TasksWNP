using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide {

    /// <summary>
    /// 
    /// </summary>
    internal class Reciever {
        /*
        Method        : Worker
        Description   : 
        Parameters    : Object task     :   the task object that is passed to the Worker
                                            in order to have it work independently
        Return Values : N/A
        */
        internal async Task WorkerTask(TcpClient client, CancellationToken ct) {     // should probably give this a better name than "Worker"

            //WorkerTasks taskInfo = (WorkerTasks)task;
            string buffer = ConfigurationManager.AppSettings["BufferSize"];
            int.TryParse(buffer, out int bufferSize);

            Byte[] bytes = new byte[bufferSize];      // this can be changed, literally just a default value I'm using

            NetworkStream stream = client.GetStream();

            string filePath = ConfigurationManager.AppSettings["FilePath"];

            bool doneReading = false;

            while (!doneReading && !ct.IsCancellationRequested) {
                int i = await stream.ReadAsync(bytes, 0, bytes.Length, ct);
                if (i == 0) doneReading = true;

                string data = Encoding.ASCII.GetString(bytes, 0, i);

                // console write for debugging
                //Console.WriteLine("Received: {0}\n", data);

                FileIO.FileWrite(filePath, data);
            }

            // sutdown and end connection
            //client.Close();

            return;
        }
    }
}
