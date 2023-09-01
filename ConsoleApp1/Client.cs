using System;
using System.Net.Sockets;
using System.Security;
using System.Text;

class Client
{
    static void Main(string[] args)
    {
        
        try
        {
            byte[] lengthBuffer = new byte[4];
            byte[] contentBuffer = new byte[1024];
            int offset = 0;
            TcpClient client = new TcpClient("127.0.0.1", 8080);
            NetworkStream stream = client.GetStream();
            BinaryReader reader = new BinaryReader(stream);
            BinaryWriter writer = new BinaryWriter(stream);


            while (true)
            {
                Console.Write("Client: ");
                string clientMessage = Console.ReadLine();
                byte[] clientData = Encoding.UTF8.GetBytes(clientMessage);
                writer.Write(clientData.Length);
                writer.Write(clientData);
                
                int bytesRead = stream.Read(lengthBuffer, 0, 4);
                if (bytesRead == 0) break;
                int contentLength = BitConverter.ToInt32(lengthBuffer, 0);
                offset = 0;

                // Read payload
                while (offset < contentLength)
                {
                    bytesRead = stream.Read(contentBuffer, offset, contentLength - offset);
                    if (bytesRead == 0) break;
                    offset += bytesRead;
                }

                string message = Encoding.UTF8.GetString(contentBuffer, 0, contentLength);
                Console.WriteLine("Server: " + message);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}