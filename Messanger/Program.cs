using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Messanger
{
    class Program
    {
        static ServerObject serverObject;
        static Thread thread;
        static void Main(string[] args)
        {
            try
            {
                serverObject = new ServerObject();
                thread = new Thread(new ThreadStart(serverObject.Listen));
                thread.Start();
            }
            catch(Exception e)
            {
                serverObject.CloseSocket();
                Console.WriteLine(e.Message);
            }
        }
    }
}
