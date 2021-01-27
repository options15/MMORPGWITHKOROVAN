using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class Server
    {
        private readonly IPAddress localIP = IPAddress.Parse("127.0.0.1");
        private const int port = 8888;
        private TcpListener server;
        private int nextId = 0;

        private static List<Connection> connections = new List<Connection>();

        public event Action<Message, int> OnServerEvent;

        internal static IReadOnlyCollection<Connection> Connections => connections;
        public void Start()
        {
            server = new TcpListener(localIP, port);

            server.Start();

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                User user = CreateNewUser();

                var connection = new Connection(client, user);
                connections.Add(connection);
                ConnectionSubscribe(connection);

                OnServerEvent(new Message
                    {
                        NetObjectName = NetObjectName.Chat,
                        Method = "connection",
                        Data = new string[] { "Присоеденился новый клиент!" }
                    }, 0);
            }
        }

        private void ConnectionSubscribe(Connection connection)
        {
            connection.OnUserAction += UserAction;
            connection.OnDisconnect += ClientDisconnect;
        }

        private void ConnectionUnsubscribe(Connection connection)
        {
            connection.OnUserAction -= UserAction;
            connection.OnDisconnect -= ClientDisconnect;
        }

        private void ClientDisconnect(int id)
        {
            var conn = connections.FirstOrDefault(x => x.User.Id == id);
            if (conn == null)
                return;
            ConnectionUnsubscribe(conn);
            connections.Remove(conn);
        }
        private User CreateNewUser()
        {
            User user = new User(nextId);
            nextId++;
            return user;
        }

        private void UserAction(Message message, int id)
        {
            OnServerEvent(message, id);
        }
    }
}
