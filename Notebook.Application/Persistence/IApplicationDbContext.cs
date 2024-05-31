using Microsoft.EntityFrameworkCore;
using Notebook.Domain.Entities;

namespace Notebook.Application.Persistence;

public interface IApplicationDbContext
{
    DbSet<NoteEntity> Notes { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task MigrateDatabaseAsync();
}