using System.Linq;
using Luizanac.Utils.App.Entities;
using Luizanac.Utils.Seeds.App;
using Microsoft.EntityFrameworkCore;

namespace Luizanac.Utils.Contexts.App
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Initial Catalog=LuizanacDev;User ID=sa;Password=Luizroot!;");
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