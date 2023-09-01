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
                byte[] lengthBuffer = new byte[4];
                byte[] payloadBuffer = new byte[1024];
                int offset = 0;
                int bytesRead = stream.Read(lengthBuffer, 0, lengthBuffer.Length);
                if (bytesRead == 0) break;
                int payloadLength = BitConverter.ToInt32(lengthBuffer, 0);
                
                while (offset < payloadLength)
                {
                    bytesRead = stream.Read(payloadBuffer, offset, payloadLength - offset);
                    if (bytesRead == 0) break;
                    offset += bytesRead;
                }
                
                string message = Encoding.UTF8.GetString(payloadBuffer, 0, payloadLength);
                Console.WriteLine("Received: " + message);

                // Send message
                string serverMessage = message;
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
