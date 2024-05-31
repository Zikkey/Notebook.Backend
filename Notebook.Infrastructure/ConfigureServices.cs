using System.Data.Common;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Notebook.Application.Persistence;
using Notebook.Application.Services;
using Notebook.Infrastructure.Extensions;
using Notebook.Infrastructure.Persistence;
using Notebook.Infrastructure.Persistence.Interceptors;
using Notebook.Infrastructure.Services;
using Npgsql;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;

namespace Notebook.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<INoteService, NoteService>();
        AddDbContext(services, configuration.GetConnectionString("db")!);
        
        return services;
    }

    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        var environment = builder.Environment.EnvironmentName;
        var configuration = builder.Configuration;

        var appName = string.Join("-", Assembly.GetEntryAssembly()!.GetName().Name!.ToLower().Split(".").Skip(1));

        var formatter = new RenderedCompactJsonFormatter();

        var loggerBuilder = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.Debug()
            .WriteTo.Console()
            .Enrich.WithProperty("App", appName)
            .Enrich.WithProperty("Environment", environment)
            .ReadFrom.Configuration(configuration);

        if (!builder.Environment.IsDevelopment())
        {
            loggerBuilder
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning);
        }

        Log.Logger = loggerBuilder.CreateLogger();

        return builder;
    }
    
    private static IServiceCollection AddDbContext(this IServiceCollection services, string dbConnectionStr)
    {
        var npgsqlDataSource = new NpgsqlDataSourceBuilder(dbConnectionStr).Build();

        services.AddDbContext<ApplicationDbContext>((_, opt) =>
        {
            opt.ConfigureOptionsBuilder(npgsqlDataSource, true);
        });

        services.TryAddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.TryAddScoped<ApplicationDbContextInitializer>();
        services.TryAddTransient<ISaveChangesInterceptor, BaseEntityInterceptor>();

        return services;
    }

    private static DbContextOptionsBuilder ConfigureOptionsBuilder(this DbContextOptionsBuilder builder,
        DbDataSource npgsqlDataSource, bool useRetry)
    {
        return builder.UseNpgsql(npgsqlDataSource,
            x =>
            {
                if (useRetry) x.EnableRetryOnFailure(3);
            });
    }


    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "API",
                Description = "API"
            });
            var xmlFile = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static void ConfigureAppJsonOptions(this JsonSerializerOptions options)
    {
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    }
}