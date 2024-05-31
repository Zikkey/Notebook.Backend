using Notebook.Application;
using Notebook.Infrastructure;
using Notebook.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args).AddLogging();

builder.Host.UseDefaultServiceProvider(static o =>
{
    o.ValidateScopes = true;
    o.ValidateOnBuild = true;
});

var services = builder.Services;
var config = builder.Configuration;
var environment = builder.Environment;

services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.ConfigureAppJsonOptions());

services.AddSwagger();

builder.Host.UseSerilog();

services.AddMapster();
services.AddInfrastructureServices(config);

services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseCors("AllowAllOrigins");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

if (!environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();

using var scope = app.Services.CreateScope();
await app.InitializeDatabaseAsync();

app.Run();