using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restaurant.Core.Entities;

namespace Restaurant.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.Name);

        // Soft delete filter
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}
