using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Notebook.Application.Persistence;

namespace Notebook.Infrastructure.Persistence;

public static class InitializerExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        await initializer.InitializeAsync();
    }
}

public class ApplicationDbContextInitializer
{
    private readonly IApplicationDbContext context;

    public ApplicationDbContextInitializer(IApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task InitializeAsync()
    {
        await context.MigrateDatabaseAsync();
    }
}