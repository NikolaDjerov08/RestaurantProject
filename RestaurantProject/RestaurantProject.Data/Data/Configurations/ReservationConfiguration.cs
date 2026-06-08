using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Core.Entities;

namespace Restaurant.Infrastructure.Data.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.User)
               .WithMany(u => u.Reservations)
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.ReservationDate);

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
