using StudentsPlatform.Api.Modules.Auth.Requests;
using StudentsPlatform.Application.Auth.Login;

namespace StudentsPlatform.Api.Modules.Auth;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Auth");

        group.MapPost("/login", async (
            LoginRequest request,
            LoginHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new LoginCommand(request.Email, request.Password);
            var response = await handler.HandleAsync(command, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("Login")
        .WithSummary("Authenticates a student and returns the session payload.");

        return app;
    }
}
