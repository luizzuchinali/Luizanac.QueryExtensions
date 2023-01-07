using Shared.Entities;

namespace Shared.Seeds;

public static class DataGenerator
{
    public static IList<User> CreateUsers(int size = 100)
    {
        var users = new List<User>(size);
        for (var i = 0; i < size; i++)
        {
            var person = new Bogus.Person();
            var bAddress = new Bogus.Faker().Address;
            var address = new Address(bAddress.City(),
                bAddress.StreetName(),
                bAddress.BuildingNumber(),
                bAddress.State());
            var user = new User(person.FullName, person.Email, i, address);
            users.Add(user);
        }

        return users;
    }
}