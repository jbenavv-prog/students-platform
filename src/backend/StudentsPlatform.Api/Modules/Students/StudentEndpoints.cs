using System.Security.Claims;
using StudentsPlatform.Api.Common.Security;
using StudentsPlatform.Api.Modules.Students.Requests;
using StudentsPlatform.Application.Students.CreateStudent;
using StudentsPlatform.Application.Students.DeleteStudent;
using StudentsPlatform.Application.Students.GetStudentById;
using StudentsPlatform.Application.Students.GetStudents;
using StudentsPlatform.Application.Students.UpdateStudent;
using StudentsPlatform.Application.Students.UpdateStudentSubjects;

namespace StudentsPlatform.Api.Modules.Students;

public static class StudentEndpoints
{
    public static IEndpointRouteBuilder MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/students")
            .WithTags("Students")
            .RequireAuthorization();

        group.MapGet("/", async (
            GetStudentsHandler handler,
            CancellationToken cancellationToken) =>
        {
            var response = await handler.HandleAsync(cancellationToken);
            return Results.Ok(response);
        })
        .WithName("GetStudents")
        .WithSummary("Returns the list of registered students.");

        group.MapGet("/{studentId:guid}", async (
            Guid studentId,
            ClaimsPrincipal user,
            GetStudentByIdHandler handler,
            CancellationToken cancellationToken) =>
        {
            if (!user.CanAccessStudent(studentId))
            {
                return Results.Forbid();
            }

            var response = await handler.HandleAsync(studentId, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("GetStudentById")
        .WithSummary("Returns the full student detail, including subjects, professors and classmates.");

        group.MapPost("/", async (
            UpsertStudentRequest request,
            CreateStudentHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateStudentCommand(
                request.FullName,
                request.Email,
                request.ProgramName,
                request.Password ?? string.Empty,
                request.SubjectIds ?? Array.Empty<Guid>());

            var response = await handler.HandleAsync(command, cancellationToken);
            return Results.Created($"/api/students/{response.Id}", response);
        })
        .RequireAuthorization(AuthorizationPolicies.AdminOnly)
        .WithName("CreateStudent")
        .WithSummary("Creates a student and optionally stores the initial subject selection.");

        group.MapPut("/{studentId:guid}", async (
            Guid studentId,
            ClaimsPrincipal user,
            UpsertStudentRequest request,
            UpdateStudentHandler handler,
            CancellationToken cancellationToken) =>
        {
            if (!user.CanAccessStudent(studentId))
            {
                return Results.Forbid();
            }

            var command = new UpdateStudentCommand(
                studentId,
                request.FullName,
                request.Email,
                request.ProgramName,
                request.Password,
                request.SubjectIds ?? Array.Empty<Guid>());

            var response = await handler.HandleAsync(command, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("UpdateStudent")
        .WithSummary("Updates the student profile and the selected subjects.");

        group.MapPut("/{studentId:guid}/subjects", async (
            Guid studentId,
            ClaimsPrincipal user,
            UpdateStudentSubjectsRequest request,
            UpdateStudentSubjectsHandler handler,
            CancellationToken cancellationToken) =>
        {
            if (!user.CanAccessStudent(studentId))
            {
                return Results.Forbid();
            }

            var command = new UpdateStudentSubjectsCommand(
                studentId,
                request.SubjectIds ?? Array.Empty<Guid>());

            var response = await handler.HandleAsync(command, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("UpdateStudentSubjects")
        .WithSummary("Updates only the selected subjects for a student.");

        group.MapDelete("/{studentId:guid}", async (
            Guid studentId,
            DeleteStudentHandler handler,
            CancellationToken cancellationToken) =>
        {
            await handler.HandleAsync(studentId, cancellationToken);
            return Results.NoContent();
        })
        .RequireAuthorization(AuthorizationPolicies.AdminOnly)
        .WithName("DeleteStudent")
        .WithSummary("Deletes a student and the associated enrollment.");

        return app;
    }
}
