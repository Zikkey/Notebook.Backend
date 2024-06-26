﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Notebook.Domain.Base;
using Notebook.Infrastructure.Extensions;

namespace Notebook.Infrastructure.Persistence.Interceptors;

public class BaseEntityInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider dateTime;

    public BaseEntityInterceptor(TimeProvider dateTime)
    {
        this.dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added) entry.Entity.CreatedOn = dateTime.GetUtcNow();

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified ||
                entry.HasChangedOwnedEntities()) entry.Entity.ModifiedOn = dateTime.GetUtcNow();
            
            if (entry.State != EntityState.Deleted)
                continue;

            if (entry.Entity.IsDeleted)
            {
                entry.State = EntityState.Unchanged;
                continue;
            }

            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
        }
    }
}