using Entities;
using Newtonsoft.Json;

namespace CommonClasses
{
    public class MessageParser
    {
        public Message ParseData(string data)
        {
            return JsonConvert.DeserializeObject<Message>(data);
        }

        public string ParseMessage(Message message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }
}