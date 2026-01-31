using System.Configuration;
using System.Diagnostics;
using a01TasksWNP;

namespace ServerSide {
    internal class FileMonitor {
        internal async Task Monitor(CancellationToken ct) {
            string serverMaxFileSize = ConfigurationManager.AppSettings["MaxFileSize"];
            long.TryParse(serverMaxFileSize, out long maxFileSize);
            string serverFilePath = ConfigurationManager.AppSettings["FilePath"];
            
            //Purely for debugging.
            File.Delete(serverFilePath);

            FileIO.VerifyFileExists(serverFilePath);

            Stopwatch elapsedTime = new Stopwatch();
            bool started = false;
            long lastSize = 0;
            //Continuously check the file size until stopRequested is true.
            while (!ct.IsCancellationRequested) {
                try {
                    long currentSize = new FileInfo(serverFilePath).Length;
                    if (!started && currentSize != 0) {
                        elapsedTime.Start();
                        UI.DisplayMessage("[Monitor] Monitoring started...");
                        UI.DisplayMessage($"\nMax File Size: {maxFileSize:N0}");
                        started = true;
                    }
                    
                    if (started && lastSize != currentSize){
                        UI.DisplayMessage($"[Monitor] File size: {currentSize:N0}");
                        lastSize = currentSize;

                        //If maximum file size reached, stop all writer threads.
                        if (currentSize >= maxFileSize) {
                            UI.DisplayMessage("\nReached max file size — stopping writers...");
                            ServerProgram.CancelToken();
                        } else {//time subject to change
                            //Check file 10 times per second.
                            Thread.Sleep(100);
                        }
                    }
                } catch (Exception ex) {
                    //Handle case where file may not yet exist or is inaccessible.
                    UI.DisplayMessage($"Monitor error: {ex.Message}");
                }
            }
            elapsedTime.Stop();
            Logger.LogMessage($"Elapsed Time to Wirte File: {elapsedTime}");

            return;
        }
    }
}
