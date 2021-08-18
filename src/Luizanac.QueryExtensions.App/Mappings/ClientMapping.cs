using Luizanac.QueryExtensions.App.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Luizanac.QueryExtensions.App.Mappings
{
    public class ClientMapping : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
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