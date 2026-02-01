/*
*	FILE	        :   FileMonitor.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   January 31, 2026
*   DESCRIPTION     :   This file is used to monitor the state of the file that the server 
*                       is writing to. 
*/

using System.Configuration;
using System.Diagnostics;
using a01TasksWNP;

namespace ServerSide {
    internal class FileMonitor {

        /*
        Method        : Monitor
        Description   : This method monitors the file and gives updates on the file size
        Parameters    : CancellationToken ct        :   The cancellation token that is required to
                                                        determine if the task needs to continue to
                                                        run or end.
        Return Values : Task                        :   As an Async method, it is required to return
                                                        a task. This allows the method to return control
                                                        to its caller.
        */
        internal async Task Monitor(CancellationToken ct) {
            // read and parse the file information to monitor from apconfig
            string serverMaxFileSize = ConfigurationManager.AppSettings["MaxFileSize"];
            long.TryParse(serverMaxFileSize, out long maxFileSize);
            string serverFilePath = ConfigurationManager.AppSettings["FilePath"];
            
            //File.Delete(serverFilePath);      // if deleting the file is wanted as part of running

            // verify if the file exists or create if it doesn't
            FileIO.VerifyFileExists(serverFilePath);

            Stopwatch elapsedTime = new Stopwatch();
            bool started = false;
            long lastSize = 0;
            //Continuously check the file size until stopRequested is true.
            while (!ct.IsCancellationRequested) {
                try {
                    long currentSize = new FileInfo(serverFilePath).Length;
                    if (!started && currentSize != 0) {
                        elapsedTime.Start();        // begin stopwatch when writing operations start
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
                    Logger.LogMessage($"{ex.Message}");
                }
            }
            // end stopwatch time when file reaches max and log it
            elapsedTime.Stop();
            Logger.LogMessage($"Elapsed Time to Wirte File: {elapsedTime}");

            return;
        }
    }
}
