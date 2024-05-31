using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Notebook.Application;

public static class ConfigureServices
{
    public static void AddMapster(this IServiceCollection services)
    {
        var assembly = typeof(ConfigureServices).Assembly;
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(assembly);
        var mapperConfig = new Mapper(typeAdapterConfig);
        services.AddSingleton<IMapper>(mapperConfig);
        services.AddSingleton(typeAdapterConfig);
    }
}