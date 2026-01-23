/*
*	FILE	        :   ClientProgram.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   
*   DESCRIPTION     :   
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ClientSide {
    internal class ClientProgram {
        static void Main(string[] args) {

            //create token

            //manage token in listener
            ClientListener clientListener = new ClientListener();
            Task listenTask = new Task(clientListener.StartListener);



            Client clientSender = new Client();
            Task sendTask = new Task(clientSender.Run);

        }
    }
}
