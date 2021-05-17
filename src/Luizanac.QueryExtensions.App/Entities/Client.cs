namespace Luizanac.QueryExtensions.App.Entities
{
    public class Client : Entity
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public int Age { get; private set; }

        public Client(string name, string email, int age)
        {
            Name = name;
            Email = email;
            Age = age;
        }

    }
}