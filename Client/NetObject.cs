using Entities;
using UnityEngine;

namespace Client
{
    abstract class NetObject : MonoBehaviour
    {
        private readonly string hub;

        public readonly int NetId;

        public NetObject(int netId, string hub)
        {
            NetId = netId;
            this.hub = hub;
        }

        public void SendInfo(Message message)
        {

        }
    }
}
