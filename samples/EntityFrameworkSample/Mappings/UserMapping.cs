using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Entities;

namespace EntityFrameworkSample.Mappings
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Email).IsRequired();
            builder.Property(x => x.Age).IsRequired();

            builder.OwnsOne(x => x.Address, x =>
            {
                x.Property(x => x.City).HasColumnName(nameof(Address.City));
                x.Property(x => x.Street).HasColumnName(nameof(Address.Street));
                x.Property(x => x.Number).HasColumnName(nameof(Address.Number));
                x.Property(x => x.State).HasColumnName(nameof(Address.State));
            });

            builder.ToTable("Clients");
        }
    }
}