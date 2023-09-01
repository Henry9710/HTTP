using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main(string[] args)
    {
        TcpListener server = null;
        try
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            int port = 12345;
            server = new TcpListener(ipAddress, port);

            server.Start();
            Console.WriteLine("Server listening on 127.0.0.1:12345");

            while (true)
            {
                using (TcpClient client = server.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream())
                {
                    Console.WriteLine("Accepted connection from: " + ((IPEndPoint)client.Client.RemoteEndPoint).ToString());

                    byte[] buffer = new byte[4];
                    while (stream.Read(buffer, 0, buffer.Length) > 0)
                    {
                        int messageLength = BitConverter.ToInt32(buffer, 0);
                        buffer = new byte[messageLength];
                        stream.Read(buffer, 0, buffer.Length);
                        string receivedMessage = Encoding.UTF8.GetString(buffer);

                        Console.WriteLine("Received: " + receivedMessage);

                        Console.Write("Enter a message to send: ");
                        string response = Console.ReadLine();
                        byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                        byte[] lengthBytes = BitConverter.GetBytes(responseBuffer.Length);

                        stream.Write(lengthBytes, 0, lengthBytes.Length);
                        stream.Write(responseBuffer, 0, responseBuffer.Length);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
        finally
        {
            server.Stop();
        }
    }
}

class Client
{
    static void Main(string[] args)
    {
        try
        {
            TcpClient client = new TcpClient("127.0.0.1", 12345);
            NetworkStream stream = client.GetStream();

            while (true)
            {
                Console.Write("Enter a message to send: ");
                string message = Console.ReadLine();
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);

                stream.Write(lengthBytes, 0, lengthBytes.Length);
                stream.Write(messageBytes, 0, messageBytes.Length);

                byte[] buffer = new byte[4];
                stream.Read(buffer, 0, buffer.Length);
                int responseLength = BitConverter.ToInt32(buffer, 0);
                buffer = new byte[responseLength];
                stream.Read(buffer, 0, buffer.Length);
                string receivedResponse = Encoding.UTF8.GetString(buffer);

                Console.WriteLine("Received: " + receivedResponse);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
        }
    }
}