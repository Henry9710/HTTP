using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main(string[] args)
    {
        try
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            TcpListener server = new TcpListener(ipAddress, 8080);
            server.Start();
            Console.WriteLine("Server started on 127.0.0.1:8080");

            TcpClient client = server.AcceptTcpClient();
            NetworkStream stream = client.GetStream();
            BinaryReader reader = new BinaryReader(stream);
            BinaryWriter writer = new BinaryWriter(stream);

            while (true)
            {
                
                int messageLength = reader.ReadInt32();
                byte[] clientData = reader.ReadBytes(messageLength);
                string clientMessage = Encoding.UTF8.GetString(clientData);
                Console.Write("Client: " + clientMessage);

                // Send message
                string serverMessage = clientMessage;
                Console.WriteLine("Server: " + serverMessage);
                byte[] serverData = Encoding.UTF8.GetBytes(serverMessage);
                writer.Write(serverData.Length);
                writer.Write(serverData);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
