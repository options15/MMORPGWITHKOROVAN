using CommonClasses;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientConnection 
    {
        private readonly int port = 8888;
        private readonly string address = "127.0.0.1";
        private TcpClient client;

        private List<NetObject> netObjects;
        private NetworkStream stream;
        private MessageParser messageParser;

        public event Action<Message> OnMessageComing;
        public ClientConnection()
        {
            client = new TcpClient();
            messageParser = new MessageParser();
        }

        public void Connect()
        {
            try
            {
                client.Connect(address, port);
                stream = client.GetStream();
                Reader();
            }
            catch (Exception e)
            {
                stream.Close();
                client.Close();
                OnMessageComing(new Message
                {
                    NetObjectName = NetObjectName.Chat,
                    Method = "SendMessage",
                    Data = new string[] { "Не соеденилось!" }
                });
            }

        }

        private async void Reader()
        {
            await Task.Factory.StartNew((Action)(() =>
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
                            bytes = stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        } while (stream.DataAvailable);

                        string message = builder.ToString();
                        var input = messageParser.ParseData(message);
                        OnMessageComing(input);

                        //if (netObjects.Exists(x => x.NetId == input.NetId))
                        //{
                        //    InvokeNetObjectMethod(input);
                        //}
                        //else
                        //{
                        //    InstantiateNetObject(input);
                        //}
                    }
                }
                catch
                {
                    stream.Close();
                    client.Close();
                    OnMessageComing(new Message
                    {
                        NetObjectName = NetObjectName.Chat,
                        Method = "SendMessage",
                        Data = new string[] { "Не соеденилось!" }
                    });
                }
            }));
        }

        private void InstantiateNetObject(Message message)
        {
            try
            {
                var assembly = Assembly.GetCallingAssembly();
                Type type = assembly.GetType($"{assembly.GetName().Name}.{message.NetObjectName}", false, false);
                NetObject netObject = (NetObject)Activator.CreateInstance(type, message.NetId, message.Data[0]);
                netObjects.Add(netObject);
            }
            catch
            {

            }
        }

        private void DeleteNetObject(int netId)
        {
            netObjects.Remove(netObjects.FirstOrDefault(x => x.NetId == netId));
        }

        private void InvokeNetObjectMethod(Message message)
        {
            try
            {
                var netObject = netObjects.FirstOrDefault(x => x.NetId == message.NetId);
                MethodInfo method = netObject.GetType().GetMethod(message.Method);
                method.Invoke(netObject, message.Data);
            }
            catch
            {

            }
        }

        public void SendMessage(Message message)
        {
            string str = messageParser.ParseMessage(message);
            byte[] data = Encoding.Unicode.GetBytes(str);
            stream.Write(data, 0, data.Length);
        }
    }
}
