using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace EntityFrameworkSample.Contexts
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var cs = "Server=localhost;Database=QueryExtensions;User Id=sa;Password=Secret123!;";
            optionsBuilder.UseSqlServer(cs);
            //optionsBuilder.LogTo(Console.WriteLine, LogLevel.Trace);
        }

        public DbSet<User> Users { get; protected set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(
                         e => e.GetProperties().Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("varchar(100)");

            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}