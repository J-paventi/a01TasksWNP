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
    internal class Worker
    {

        /*
        Method        : Worker
        Description   : 
        Parameters    : Object task     :   the task object that is passed to the Worker
                                            in order to have it work independently
        Return Values : N/A
        */
        public void WorkerTask(Object task)
        {     // should probably give this a better name than "Worker"

            //WorkerTasks taskInfo = (WorkerTasks)task;
            if (task is WorkerTasks info) {



                //cast MaxByteSize to int
                int mByteSize = info.maxByteSize;

                // cast the object to a TcpClient object
                TcpClient client = info.client;

                Byte[] bytes = new byte[mByteSize];      // this can be changed, literally just a default value I'm using
                string data = null;

                NetworkStream stream = client.GetStream();

                int i;

                string filePath = ConfigurationManager.AppSettings["FilePath"];

                // currently this is just receiving the client communication and not doing anything
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) {
                    data = Encoding.ASCII.GetString(bytes, 0, i);

                    // console write for debugging
                    Console.WriteLine("Received: {0}\n", data);

                    FileIO.FileWrite(filePath, data);
                }

                // sutdown and end connection
                client.Close();
            }
            return;
        }
    }
}
