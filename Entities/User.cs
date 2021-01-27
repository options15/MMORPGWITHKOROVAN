namespace Entities
{
    public class User
    {
        public readonly int Id;
        public string Name;

        public User(int id)
        {
            Id = id;
        }

        public void ToName(string name)
        {
            Name = name;
        }
    }
}
