/*
*	FILE	        :   a01TasksWNP.cs
*	PROJECT         :   A01 Tasks - Windows Network Programming
*   PROGRAMMER      :   Jonathan Paventi, Josh Visentin, and Trent Beitz
*   FIRST VERSION   :   
*   DESCRIPTION     :   
*/

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using ServerSide;

namespace a01TasksWNP {
    internal class ServerProgram {
        static void Main(string[] args) {
            // Create listener for a client request and start the listener
            ServerListener listener = new ServerListener();
            listener.StartListener();
        }
    }
}
