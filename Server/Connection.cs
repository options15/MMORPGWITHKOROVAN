using Entities;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Connection
    {
        private NetworkStream _stream;
        private TcpClient _client;
        private MessageParser parser;
        private HubInvoker invoker;

        public User User;

        public event Action<Message, int> OnUserAction;
        public event Action<int> OnDisconnect;
        public Connection(TcpClient client, User user)
        {
            _client = client;
            User = user;
            parser = new MessageParser();
            invoker = new HubInvoker();
            Connect();
        }

        public void Connect()
        {
            _stream = _client.GetStream();
            Reader();
        }

        private async void Reader()
        {
            await Task.Factory.StartNew(() =>
            {
                byte[] data = new byte[64];
                StringBuilder builder = new StringBuilder();
                try
                {
                    while (true)
                    {
                        int bytes = 0;
                        builder.Clear();
                        do
                        {
                            bytes = _stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (_stream.DataAvailable);

                        string message = builder.ToString();
                        
                        Message input = parser.ParseData(message);

                        if (input == null)
                            throw new ArgumentNullException("Пришло пустое сообщение!");

                        OnUserAction(input, User.Id);
                        invoker.MethodInvoke(User.Id, input);
                    }
                }
                catch
                {
                    Disconnect();
                }
            });
        }
        public void Disconnect()
        {
            _stream.Close();
            _client.Close();
            OnDisconnect(User.Id);
        }
        public void SendMessage(Message message)
        {
            string str = parser.ParseMessage(message);
            byte[] data = Encoding.Unicode.GetBytes(str);
            _stream.Write(data, 0, data.Length);
        }
    }
}

