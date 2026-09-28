using Content.Application.Abstractions;
using ContentEntity = Content.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore;

namespace Content.Infrastructure.Persistence;

public sealed class ContentDbContext : DbContext, IContentUnitOfWork
{
    public ContentDbContext(
        DbContextOptions<ContentDbContext> options)
        : base(options)
    {
    }

    public DbSet<ContentEntity> Contents => Set<ContentEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ContentDbContext).Assembly);
    }
}