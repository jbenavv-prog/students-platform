using Microsoft.AspNetCore.Authentication;
using StudentsPlatform.Api.Common.Security;
using StudentsPlatform.Application.Auth.Login;
using StudentsPlatform.Application.Catalog.Professors;
using StudentsPlatform.Application.Catalog.Subjects;
using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Application.Common.Security;
using StudentsPlatform.Application.Students.CreateStudent;
using StudentsPlatform.Application.Students.DeleteStudent;
using StudentsPlatform.Application.Students.GetStudentById;
using StudentsPlatform.Application.Students.GetStudents;
using StudentsPlatform.Application.Students.UpdateStudent;
using StudentsPlatform.Application.Students.UpdateStudentSubjects;
using StudentsPlatform.Infrastructure.Security;

namespace StudentsPlatform.Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.SectionName));

        services.AddAuthentication(BearerTokenAuthenticationHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, BearerTokenAuthenticationHandler>(
                BearerTokenAuthenticationHandler.SchemeName,
                _ => { });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.AdminOnly, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Administrator);
            });
        });

        services.AddScoped<GetSubjectsHandler>();
        services.AddScoped<GetProfessorsHandler>();
        services.AddScoped<GetStudentsHandler>();
        services.AddScoped<GetStudentByIdHandler>();
        services.AddScoped<CreateStudentHandler>();
        services.AddScoped<UpdateStudentHandler>();
        services.AddScoped<UpdateStudentSubjectsHandler>();
        services.AddScoped<DeleteStudentHandler>();
        services.AddScoped<LoginHandler>();
        services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
        services.AddSingleton<IAccessTokenService, HmacAccessTokenService>();
        services.AddSingleton<IAdminAccountProvider, ConfiguredAdminAccountProvider>();

        return services;
    }
}
