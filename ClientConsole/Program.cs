using Client;
using Entities;
using System;
using UnityEngine;

namespace ClientConsole
{
    public class Program
    {
        private static ClientConnection connection = new ClientConnection();
        static void Main(string[] args)
        {
            connection.Connect();
            connection.OnMessageComing += ShowMessage;
            while (true)
            {
                string message = Console.ReadLine();
                try
                {
                    Message mess = new Message
                    {
                        NetObjectName = NetObjectName.Chat,
                        Method = "SendMessage",
                        Data = new string[] { message }
                    };

                    connection.SendMessage(mess);
                }
                catch
                {
                    Console.WriteLine("Сервер не работает!");
                    Console.ReadLine();
                    return;
                }
            }
        }

        private static void ShowMessage(Message message)
        {
            if(message != null)
            Console.WriteLine(message.Data[0]);
        }
    }
}
