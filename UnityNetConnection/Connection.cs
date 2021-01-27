using CommonClasses;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityNetConnection
{
    public class Connection : MonoBehaviour
    {
        private readonly int _port = 8888;
        private readonly string _address = "127.0.0.1";

        private TcpClient _client;
        private NetworkStream _stream;

        private MessageParser _messageParser = new MessageParser();
        private List<NetObject> _netObjects = new List<NetObject>();

        public List<GameObject> Prefabs = new List<GameObject>();

        public GameObject Chat;
        private NetObject _chat;

        public void Start()
        {
            _chat = Chat.AddComponent<NetObject>();
            _chat.OnMessageSendToServer += SendMessage;
        }

        public bool IsConnected() => _client.Connected;

        public void Connect()
        {
            _client = new TcpClient();

            try
            {
                _client.Connect(_address, _port);
                _stream = _client.GetStream();
                Reader();
            }
            catch (Exception e)
            {
                _stream.Close();
                _client.Close();
            }

        }

        private void Reader()
        {
            Task.Factory.StartNew(() =>
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

                       string input = builder.ToString();
                       var message = _messageParser.ParseData(input);
                       ExecuteMessage(message);
                    }
               }
               catch
               {
                   _stream.Close();
                   _client.Close();
               }
            });

        }

        public void ExecuteMessage(Message message)
        {
            switch (message.Method)
            {
                case "CreateNetObject":
                    InstantiateNetObject(message);
                    break;
                case "DeleteNetObject":
                    DeleteNetObject(message.NetId);
                    break;
                case "ChatMessageIncoming":
                    _chat.InvokeMethod(message);
                    break;
                default:
                    var netObject = _netObjects.FirstOrDefault();

                    if (netObject == null)
                        return;
                    netObject.GetComponent<NetObject>().InvokeMethod(message);
                    break;
            }
        }

        private void SubscribeObject(NetObject netObject)
        {
            netObject.OnMessageSendToServer += SendMessage;
        }
        private void UnsubscribeObject(NetObject netObject)
        {
            netObject.OnMessageSendToServer -= SendMessage;
        }
        private void InstantiateNetObject(Message message)
        {

            var gameOgjectPrefab =  Prefabs.FirstOrDefault(x => x.name == message.NetObjectName.ToString());
            if (gameOgjectPrefab == null)
                return;

            var instance =  Instantiate(gameOgjectPrefab);
            instance.AddComponent<NetObject>();
            var netObject = instance.GetComponent<NetObject>();

            netObject.SetIdandHub(message);
            SubscribeObject(netObject);
            _netObjects.Add(netObject);
        }
        private void DeleteNetObject(int netId)
        {
            var gameObject = _netObjects.FirstOrDefault(x => x.NetId == netId);
            if (gameObject == null)
                return;

            _netObjects.Remove(gameObject);
            UnsubscribeObject(gameObject);
            Destroy(gameObject.gameObject);
        }

        public void SendMessage(Message message)
        {
            if (!_client.Connected || message.Data == null)
                return;

            string str = _messageParser.ParseMessage(message);
            byte[] data = Encoding.Unicode.GetBytes(str);
            _stream.Write(data, 0, data.Length);
        }
    }
}
