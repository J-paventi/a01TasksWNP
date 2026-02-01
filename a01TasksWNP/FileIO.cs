/*
*	FILE	        :   FileIO.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   January 31, 2026
*   DESCRIPTION     :   This file is responsible for all of the server's
*                       file operations. It controls the reading and writing
*                       operations.
*/

namespace ServerSide {
    internal class FileIO {
        private static readonly object locker = new object();

        /*
        Method        : FileWrite
        Description   : 
        Parameters    : string path     :   path of file to write to.
                        string data     :   data to write to file.
        Return Values : N/A
        */
        internal static void FileWrite(string path, string data){
             lock (locker){
                try{ 
                    using (StreamWriter sw = new StreamWriter(path, true)) {
                        sw.Write(data);
                    } 
                } catch (Exception ex) { 
                    UI.DisplayMessage(ex.ToString());
                    Logger.LogMessage($"{ex.Message}");
                }
            }

            return;
        }
        
        /*
        Method        : FileGetSize
        Description   : 
        Parameters    : string path     :   path of file to get size of.
        Return Values : long            :   returns a long with the current size of the file
        */
        internal static long FileGetSize(string path){ 
            return new FileInfo(path).Length;
        }
        
        /*
        Method        : VerifyFileExists
        Description   : 
        Parameters    : string path     :   path of file to get size of.
        Return Values : bool            :   returns a long with the current size of the file
        */
        internal static bool VerifyFileExists(string path){
            bool fileResult = false;
            try {
                if (!File.Exists(path)){
                    File.Create(path).Dispose();
                } 
                fileResult = true;
            } catch (Exception e){
                Logger.LogMessage("Exception occured while creating file: " + e.Message);
                fileResult = false;
            }

            return fileResult;
        }
    }
}