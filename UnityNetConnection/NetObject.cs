using Entities;
using System;
using UnityEngine;

namespace UnityNetConnection
{
    public abstract class NetObject : MonoBehaviour
    {
        private NetObjectName netObjectName;
        public int NetId { get; private set; }

        protected event Action<Message> OnMessageCame;
        public event Action<Message> OnMessageSendToServer;

        public void SetIdandHub(Message message)
        {
            netObjectName = message.NetObjectName;
            NetId = message.NetId;
        }

        public void InvokeMethod(Message message)
        {
            OnMessageCame(message);
        }

        public void SendInfo(Message message)
        {
            OnMessageSendToServer(message);
        }
    }
}
