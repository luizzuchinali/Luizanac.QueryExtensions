using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Luizanac.Utils.App.Entities;
using Luizanac.Utils.Contexts.App;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luizanac.Utils.Seeds.App
{
    public static class ClientSeed
    {

        public static void Seed(this EntityTypeBuilder<Client> builder)
        {
            var aux = 0;
            var quantity = 10000;
            var clients = new List<Client>(quantity);
            while (aux != quantity)
            {
                var person = new Bogus.Person();
                var random = new Random();
                var client = new Client(person.FullName, person.Email, random.Next(16, 70));
                clients.Add(client);
                aux++;
            }

            builder.HasData(clients);
        }
    }
}