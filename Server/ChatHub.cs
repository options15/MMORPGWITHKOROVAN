using Entities;

namespace Server
{
    class ChatHub : BaseHub
    {
        public void SendMessage(int senderId, string[] message)
        {
            var mess = new Message
            {
                NetObjectName = NetObjectName.Chat,
                Method = "ChatMessageIncoming",
                Data = message 
            };

            serverActions.SendMessageOther(senderId, mess);
        }
    }
}
