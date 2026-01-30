using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using a01TasksWNP;

namespace ServerSide {
    internal class FileMonitor {
        internal async Task Monitor(CancellationToken ct) {
            string serverMaxFileSize = ConfigurationManager.AppSettings["MaxFileSize"];
            int.TryParse(serverMaxFileSize, out int maxFileSize);
            string serverFilePath = ConfigurationManager.AppSettings["FilePath"];
            
            //Purely for debugging.
            File.Delete(serverFilePath);

            FileIO.VerifyFileExists(serverFilePath);

            Stopwatch elapsedTime = new Stopwatch();
            elapsedTime.Start();

            //Continuously check the file size until stopRequested is true.
            while (!ct.IsCancellationRequested) {
                try {
                    long currentSize = new FileInfo(serverFilePath).Length;
                    Console.Write("\n[Monitor] File size: {0}", currentSize);

                    //If maximum file size reached, stop all writer threads.
                    if (currentSize >= maxFileSize) {
                        Console.WriteLine("\nReached max file size — stopping writers...");
                        ServerProgram.CancelToken();
                    } else {//time subject to change
                        //Check file 10 times per second.
                        Thread.Sleep(100);
                    }
                } catch (Exception ex) {
                    //Handle case where file may not yet exist or is inaccessible.
                    Console.WriteLine($"Monitor error: {ex.Message}");
                }
            }
            elapsedTime.Stop();
            Logger.LogMessage($"Elapsed Time to Wirte File: {elapsedTime}");

            return;
        }
    }
}
