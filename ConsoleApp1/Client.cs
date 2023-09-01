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
                
                int messageLength = reader.ReadInt32();
                byte[] serverData = reader.ReadBytes(messageLength);
                string serverMessage = Encoding.UTF8.GetString(serverData);
                Console.WriteLine("Server: " + serverMessage);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}