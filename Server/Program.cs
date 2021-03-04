using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SocketTcpClient
{
    class Program
    {
        static string Name;
       static Socket socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
          

            try
            {
                Console.WriteLine("Введите свое имя: ");
                Name = Console.ReadLine();
                IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
                try
                {
                    socket2.Connect(ip);
                    string msg = Name;
                    byte[] data = Encoding.Unicode.GetBytes(msg);
                    socket2.Send(data);
                    Console.Write("Введите сообщение: ");
                   
                        Thread receivethread = new Thread(new ThreadStart(ReceiveMessage));
                        receivethread.Start();
                        
                        SendMessage();
                    
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    CloseSocket();
                }
  
               
           
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
      
        static void SendMessage()
        {
            while (true)
            {

                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                socket2.Send(data);
            }

        }
        static void ReceiveMessage()
        {
           while (true)
            {
                try
                {
                    string string1="";
                    int count = 0;
                    //StringBuilder stringbuilder = new StringBuilder();
                    byte[] data = new byte[256]; // буфер для ответа


                    do
                    {
                        count = socket2.Receive(data);
                        // stringbuilder.Append(Encoding.Unicode.GetString(data, 0, count));
                        string1 += Encoding.Unicode.GetString(data, 0, count);
                    }
                    while (socket2.Available > 0);

                    Console.WriteLine(string1);
                }

                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    CloseSocket();
                }
            }
       }
        static void CloseSocket()
        {
            socket2.Shutdown(SocketShutdown.Both);
            socket2.Close();
        }
    }
}