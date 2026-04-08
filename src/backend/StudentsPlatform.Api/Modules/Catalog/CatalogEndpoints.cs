using StudentsPlatform.Application.Catalog.Professors;
using StudentsPlatform.Application.Catalog.Subjects;

namespace StudentsPlatform.Api.Modules.Catalog;

public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Catalog");

        group.MapGet("/subjects", async (
            GetSubjectsHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(cancellationToken);
            return Results.Ok(response);
        })
        .WithName("GetSubjects")
        .WithSummary("Returns the seeded catalog of subjects.");

        group.MapGet("/professors", async (
            GetProfessorsHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(cancellationToken);
            return Results.Ok(response);
        })
        .WithName("GetProfessors")
        .WithSummary("Returns the seeded catalog of professors.");

        return app;
    }
}
