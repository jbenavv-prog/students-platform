using Microsoft.EntityFrameworkCore;
using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Application.Common.Exceptions;
using StudentsPlatform.Domain.Catalog;
using StudentsPlatform.Domain.Enrollments;
using StudentsPlatform.Domain.Students;

namespace StudentsPlatform.Application.Students.Shared;

internal static class StudentDataAccess
{
    public static async Task<Student> GetStudentForUpdateAsync(
        IApplicationDbContext dbContext,
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var student = await dbContext.Students
            .Include(entity => entity.Enrollment)
            .ThenInclude(enrollment => enrollment!.Subjects)
            .SingleOrDefaultAsync(entity => entity.Id == studentId, cancellationToken);

        return student ?? throw new NotFoundException("Student", studentId);
    }

    public static async Task<IReadOnlyCollection<Subject>> LoadSelectedSubjectsAsync(
        IApplicationDbContext dbContext,
        IReadOnlyCollection<Guid> subjectIds,
        CancellationToken cancellationToken)
    {
        StudentRequestValidator.ValidateSubjectIds(subjectIds);

        if (subjectIds.Count == 0)
        {
            return Array.Empty<Subject>();
        }

        if (subjectIds.GroupBy(id => id).Any(group => group.Count() > 1))
        {
            throw new RequestValidationException(new Dictionary<string, string[]>
            {
                ["subjectIds"] = ["No se puede repetir una materia dentro de la misma inscripcion."]
            });
        }

        var uniqueIds = subjectIds.Distinct().ToArray();
        var subjects = await dbContext.Subjects
            .AsNoTracking()
            .Include(subject => subject.Professor)
            .Where(subject => uniqueIds.Contains(subject.Id))
            .ToDictionaryAsync(subject => subject.Id, cancellationToken);

        if (subjects.Count != uniqueIds.Length)
        {
            throw new RequestValidationException(new Dictionary<string, string[]>
            {
                ["subjectIds"] = ["La seleccion contiene materias que no existen."]
            });
        }

        return subjectIds.Select(subjectId => subjects[subjectId]).ToArray();
    }

    public static async Task<StudentDetailDto> BuildStudentDetailAsync(
        IApplicationDbContext dbContext,
        Guid studentId,
        CancellationToken cancellationToken)
    {
        var student = await dbContext.Students
            .AsNoTracking()
            .Include(entity => entity.Enrollment)
            .ThenInclude(enrollment => enrollment!.Subjects)
            .SingleOrDefaultAsync(entity => entity.Id == studentId, cancellationToken);

        if (student is null)
        {
            throw new NotFoundException("Student", studentId);
        }

        if (student.Enrollment is null || student.Enrollment.Subjects.Count == 0)
        {
            return new StudentDetailDto(
                student.Id,
                student.FullName,
                student.Email,
                student.ProgramName,
                student.CreatedAt,
                null);
        }

        var subjectIds = student.Enrollment.Subjects.Select(subject => subject.SubjectId).ToArray();

        var subjects = await dbContext.Subjects
            .AsNoTracking()
            .Include(subject => subject.Professor)
            .Where(subject => subjectIds.Contains(subject.Id))
            .OrderBy(subject => subject.Name)
            .ToListAsync(cancellationToken);

        var classmates = await dbContext.EnrollmentSubjects
            .AsNoTracking()
            .Where(enrollmentSubject => subjectIds.Contains(enrollmentSubject.SubjectId))
            .Join(
                dbContext.Enrollments.AsNoTracking(),
                enrollmentSubject => enrollmentSubject.EnrollmentId,
                enrollment => enrollment.Id,
                (enrollmentSubject, enrollment) => new
                {
                    enrollmentSubject.SubjectId,
                    enrollment.StudentId
                })
            .Where(match => match.StudentId != student.Id)
            .Join(
                dbContext.Students.AsNoTracking(),
                match => match.StudentId,
                otherStudent => otherStudent.Id,
                (match, otherStudent) => new
                {
                    match.SubjectId,
                    otherStudent.FullName
                })
            .ToListAsync(cancellationToken);

        var classmatesLookup = classmates
            .GroupBy(item => item.SubjectId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyCollection<string>)group
                    .Select(item => item.FullName)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(name => name)
                    .ToArray());

        var subjectDetails = subjects
            .Select(subject => new EnrolledSubjectDetailDto(
                subject.Id,
                subject.Name,
                subject.Credits,
                subject.ProfessorId,
                subject.Professor!.FullName,
                classmatesLookup.TryGetValue(subject.Id, out var names)
                    ? names
                    : Array.Empty<string>()))
            .ToArray();

        return new StudentDetailDto(
            student.Id,
            student.FullName,
            student.Email,
            student.ProgramName,
            student.CreatedAt,
            new EnrollmentDetailDto(
                student.Enrollment.Id,
                student.Enrollment.CreatedAt,
                subjects.Sum(subject => subject.Credits),
                subjectDetails));
    }
}
