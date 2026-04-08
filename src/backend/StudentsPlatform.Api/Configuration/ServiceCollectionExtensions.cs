using StudentsPlatform.Application.Catalog.Professors;
using StudentsPlatform.Application.Catalog.Subjects;
using StudentsPlatform.Application.Students.CreateStudent;
using StudentsPlatform.Application.Students.DeleteStudent;
using StudentsPlatform.Application.Students.GetStudentById;
using StudentsPlatform.Application.Students.GetStudents;
using StudentsPlatform.Application.Students.UpdateStudent;
using StudentsPlatform.Application.Students.UpdateStudentSubjects;

namespace StudentsPlatform.Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<GetSubjectsHandler>();
        services.AddScoped<GetProfessorsHandler>();
        services.AddScoped<GetStudentsHandler>();
        services.AddScoped<GetStudentByIdHandler>();
        services.AddScoped<CreateStudentHandler>();
        services.AddScoped<UpdateStudentHandler>();
        services.AddScoped<UpdateStudentSubjectsHandler>();
        services.AddScoped<DeleteStudentHandler>();

        return services;
    }
}
