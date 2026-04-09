using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StudentsPlatform.Api.Common.HealthChecks;
using StudentsPlatform.Api.Common.Middleware;
using StudentsPlatform.Api.Configuration;
using StudentsPlatform.Api.Modules.Catalog;
using StudentsPlatform.Api.Modules.Students;
using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Infrastructure.Persistence;
using StudentsPlatform.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:4200", "https://localhost:4200"];

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddApplicationServices();
builder.Services.AddHealthChecks()
    .AddCheck(
        "self",
        () => HealthCheckResult.Healthy("The API process is running."),
        tags: ["live", "ready"])
    .AddCheck<DatabaseHealthCheck>(
        "database",
        tags: ["ready"]);
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext>(serviceProvider => serviceProvider.GetRequiredService<ApplicationDbContext>());

var app = builder.Build();

app.UseMiddleware<RequestObservabilityMiddleware>();
app.UseMiddleware<ProblemDetailsMiddleware>();
app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Students Platform API v1");
        options.RoutePrefix = "swagger";
    });
}

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitializer");
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DatabaseInitializer.InitializeAsync(dbContext, logger);
}

if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/swagger"))
        .ExcludeFromDescription();
}
else
{
    app.MapGet("/", () => Results.Ok(new { service = "Students Platform API" }))
        .ExcludeFromDescription();
}

app.MapHealthChecks("/health/live", HealthCheckResponseWriter.Create(registration => registration.Tags.Contains("live")))
    .ExcludeFromDescription();

app.MapHealthChecks("/health/ready", HealthCheckResponseWriter.Create(registration => registration.Tags.Contains("ready")))
    .ExcludeFromDescription();

app.MapHealthChecks("/health", HealthCheckResponseWriter.Create(_ => true))
    .ExcludeFromDescription();

app.MapCatalogEndpoints();
app.MapStudentEndpoints();

app.Run();

public partial class Program;
