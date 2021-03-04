using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;


namespace Messanger
{
    class ServerObject
    {
        Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        internal List<ClientObject> Users = new List<ClientObject>();
       
        internal void AddUser(ClientObject clientObj)
        {
            Users.Add(clientObj);
        }

        internal void DeleteUser(string nameUs)
        {
            ClientObject clientObj = Users.FirstOrDefault(a => a.Name == nameUs);
            if (clientObj != null)
            {
                Users.Remove(clientObj);
            }
        }

        internal void Listen()
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);

            try
            {
                _socket.Bind(ip);

                _socket.Listen(20);

                Console.WriteLine("Сервер успешно запущен.");

                while (true)
                {
                    ClientObject clientObj    = new ClientObject(this, _socket.Accept());
                    Thread       clientThread = new Thread(clientObj.Process);
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseSocket();
            }

        }

        internal void CloseSocket()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        internal void SendAllMessage(string message, string senderName)
        {
            if (message.Contains("/r"))
            {
                string messageTepm = message;
                int    index       = messageTepm.IndexOf(" ");

                for (int i = 0; i < 2; i++)
                {
                    messageTepm = messageTepm.Remove(0, index + 1);
                    index = messageTepm.IndexOf(" ");
                }

                string name = messageTepm.Substring(0, index);
                messageTepm = messageTepm.Remove(0, index + 1);
                messageTepm = "(Личное сообщение от "+senderName +")"+ ": " + messageTepm;
                byte[] data = Encoding.Unicode.GetBytes(messageTepm);
                foreach (var user in Users.Where(us => us.Name == name))
                {
                    user.socket.Send(data);
                }
            }
            else if(message.Contains("/disconect"))
            {
                byte[] data = Encoding.Unicode.GetBytes(String.Format($"{senderName}  ушел от нас = ("));
                foreach (var user in Users.Where(us => us.Name != senderName))
                {
                    user.socket.Send(data);
                }

                DeleteUser(senderName);
            }
            else
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                foreach (var user in Users.Where(us => us.Name != senderName))
                {
                    user.socket.Send(data);
                }
            }

        }

        internal void SendMyselfMessage(string message, string name)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            foreach (var user in Users.Where(t => t.Name == name))
            {
                user.socket.Send(data);
            }
        }
    }
}
