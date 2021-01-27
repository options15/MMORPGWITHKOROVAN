namespace Entities
{
    public class Message
    {
        public NetObjectName NetObjectName { get; set; }
        public string Method { get; set; }
        public int NetId { get; set; }
        public string[] Data { get; set; }
    }
}
