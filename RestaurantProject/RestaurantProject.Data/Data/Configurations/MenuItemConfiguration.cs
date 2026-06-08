using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Core.Entities;

namespace Restaurant.Infrastructure.Data.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasOne(m => m.Category)
               .WithMany(c => c.MenuItems)
               .HasForeignKey(m => m.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => m.Name);
        builder.HasIndex(m => m.CategoryId);

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
