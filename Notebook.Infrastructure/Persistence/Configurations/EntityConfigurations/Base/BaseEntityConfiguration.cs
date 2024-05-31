using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Notebook.Domain.Base;

namespace Notebook.Infrastructure.Persistence.Configurations.EntityConfigurations.Base;

internal abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Property(x => x.Id).UseIdentityColumn().HasValueGenerator<GuidValueGenerator>();
        builder.Property(x => x.CreatedOn).HasDefaultValueSql("NOW()");
        builder.Property(x => x.ModifiedOn).HasDefaultValueSql("NOW()");
    }
}