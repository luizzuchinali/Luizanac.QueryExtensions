using EntityFrameworkSample.Contexts;
using Shared.Entities;

namespace EntityFrameworkSample.Seeds
{
    public static class UserSeed
    {
        public static void Seed(this AppDbContext dbContext, int quantity = 1000)
        {
            var aux = 0;
            var clients = new List<User>(quantity);
            while (aux != quantity)
            {
                var person = new Bogus.Person();
                var random = new Random();
                var bAddress = new Bogus.Faker().Address;
                var address = new Address(
                    bAddress.City(),
                    bAddress.StreetName(),
                    bAddress.BuildingNumber(),
                    bAddress.State());
                var client = new User(person.FullName, person.Email, random.Next(16, 70), address);
                clients.Add(client);
                aux++;
            }

            dbContext.AddRange(clients);
            dbContext.SaveChanges();
        }
    }
}