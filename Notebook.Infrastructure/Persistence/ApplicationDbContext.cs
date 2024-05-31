using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Notebook.Application.Persistence;
using Notebook.Domain.Entities;

namespace Notebook.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions options,
    IEnumerable<ISaveChangesInterceptor> saveChangesInterceptors
) : DbContext(options), IApplicationDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(saveChangesInterceptors);
    
    public DbSet<NoteEntity> Notes => Set<NoteEntity>();
    
    public async Task MigrateDatabaseAsync()
    {
        try
        {
            await Database.MigrateAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Migration failed", e);
        }
    }
}