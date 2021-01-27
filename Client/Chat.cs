using Entities;

namespace Client
{
    internal class Chat : NetObject
    {
        public Chat(int netId, string hub) : base(netId, hub)
        {
        }
        public void ShowMessage(Message message)
        {

        }
    }
}
