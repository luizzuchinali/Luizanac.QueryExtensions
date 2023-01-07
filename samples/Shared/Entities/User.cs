namespace Shared.Entities;

public class User : Entity
{
	public string Name { get; private set; }
	public string Email { get; private set; }
	public int Age { get; private set; }
	public Address Address { get; set; }

	public User(string name, string email, int age)
	{
		Name = name;
		Email = email;
		Age = age;
	}

	public User(string name, string email, int age, Address address)
			: this(name, email, age)
	{
		Address = address;
	}
}