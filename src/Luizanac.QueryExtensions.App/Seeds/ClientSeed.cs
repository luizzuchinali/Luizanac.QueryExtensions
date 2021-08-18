using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Luizanac.QueryExtensions.App.Entities;
using Luizanac.QueryExtensions.Contexts.App;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luizanac.QueryExtensions.Seeds.App
{
	public static class ClientSeed
	{
		public static void Seed(this AppDbContext dbContext)
		{
			var aux = 0;
			var quantity = 10000;
			var clients = new List<Client>(quantity);
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
				var client = new Client(person.FullName, person.Email, random.Next(16, 70), address);
				clients.Add(client);
				aux++;
			}

			dbContext.AddRange(clients);
			dbContext.SaveChanges();
		}
	}
}