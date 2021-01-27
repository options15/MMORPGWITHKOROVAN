using Entities;
using System;

namespace MMORPG
{
    class Program
    {
        private static Server.Server connection;
        static void Main(string[] args)
        {
            connection = new Server.Server();
            connection.OnServerEvent += ShowMessage;
            connection.Start();
            Console.ReadLine();
        }

        private static void ShowMessage(Message message, int id)
        {
            Console.WriteLine(id + ": " + message?.Data[0]);
        }
    }
}
