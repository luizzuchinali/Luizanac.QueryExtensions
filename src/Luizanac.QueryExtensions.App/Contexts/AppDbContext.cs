using System;
using System.Linq;
using Luizanac.QueryExtensions.App.Entities;
using Luizanac.QueryExtensions.Seeds.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Luizanac.QueryExtensions.Contexts.App
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("LOCAL_DB_CONNECTION_STRING"));
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Debug);
        }

        public DbSet<Client> Clients { get; protected set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
               e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<Client>().Seed();
        }
    }
}