using Entities;
using System.Linq;

namespace Server
{
    class ServerActions
    {
        internal void SendMessageOther(int senderId, Message message)
        {
            foreach (var conn in Server.Connections)
            {
                if (senderId != conn.User.Id)
                {
                    conn?.SendMessage(message);
                }
            }
        }
        internal void SendMessageAll(int senderId, Message message)
        {
            foreach (var conn in Server.Connections)
            {
                conn?.SendMessage(message);
            }
        }
        internal void SendMessageById(int senderId, Message message)
        {
            var conn = Server.Connections.FirstOrDefault(x => x.User.Id == senderId);
            conn?.SendMessage(message);
        }
    }
}
