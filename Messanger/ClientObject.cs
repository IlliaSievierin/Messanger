using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Messanger
{
    class ClientObject
    {
        public string Name { get; set; }

        private ServerObject _server;

        internal Socket socket;

        public ClientObject(ServerObject server,Socket socket)
        {
            this._server = server;
            server.AddUser(this);
            this.socket = socket;
        }
        
        public void Process()
        {
            try
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);

                Name = ReceiveMessage();
                string message = Name + " " + "залетел к нам в чат =)";
                _server.SendAllMessage(message, Name);

                string users = "";
                for (int i = 0; i < _server.Users.Count; i++)
                {
                    users += _server.Users[i].Name + "\n";
                }

                _server.SendMyselfMessage(users, Name);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {

                        message = ReceiveMessage();
                        message = String.Format(Name + ": " + message);
                        Console.WriteLine(message);
                        _server.SendAllMessage(message, Name);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        message = String.Format(Name + ": ушел от нас =(");
                        _server.SendAllMessage(message, Name);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                CloseSocket(socket);
            }
        }

         internal string ReceiveMessage()
         { 
                int count;
                StringBuilder stringbuilder = new StringBuilder();
                byte[]        data          = new byte[256];

                do
                {
                    count = socket.Receive(data, data.Length, 0);
                    stringbuilder.Append(Encoding.Unicode.GetString(data, 0, count));
                }
                while (socket.Available > 0);
               
                return stringbuilder.ToString();

        }

        private void CloseSocket(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}

