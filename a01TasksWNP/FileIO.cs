/*
*	FILE	        :   FileIO.cs
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
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    Console.WriteLine(ex.ToString());
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
    }
}