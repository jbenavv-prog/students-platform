using StudentsPlatform.Domain.Catalog;

namespace StudentsPlatform.Infrastructure.Seed;

public static class CatalogSeedData
{
    public static readonly Guid AnaMariaTorresId = Guid.Parse("b0eafcda-f48d-4a9a-a970-3aa521de9951");
    public static readonly Guid CarlosRamirezId = Guid.Parse("66d2c0d6-0c41-40b7-a3a0-856fd4eac4bc");
    public static readonly Guid LauraMendozaId = Guid.Parse("9f3b6718-a9f8-4dde-92f4-1ab7f9cab2e3");
    public static readonly Guid DiegoPenaId = Guid.Parse("75dccfcb-f0d9-4f0e-a8f0-5140fd5be98f");
    public static readonly Guid JulianaPardoId = Guid.Parse("2538606d-8890-4ec5-b534-d984add289bd");

    public static IReadOnlyCollection<Professor> Professors { get; } =
    [
        Professor.Create(AnaMariaTorresId, "Ana Maria Torres"),
        Professor.Create(CarlosRamirezId, "Carlos Eduardo Ramirez"),
        Professor.Create(LauraMendozaId, "Laura Sofia Mendoza"),
        Professor.Create(DiegoPenaId, "Diego Fernando Pena"),
        Professor.Create(JulianaPardoId, "Juliana Pardo Rojas")
    ];

    public static IReadOnlyCollection<Subject> Subjects { get; } =
    [
        Subject.Create(Guid.Parse("4f7da76f-b7a6-4eab-ae64-b0fe944b85e7"), "Calculo I", 3, AnaMariaTorresId),
        Subject.Create(Guid.Parse("e69226c2-0df4-4c6d-9cde-76f9388aa96f"), "Algebra Lineal", 3, AnaMariaTorresId),
        Subject.Create(Guid.Parse("4467d270-8fc3-491b-b2ae-e7d33b1625b0"), "Programacion I", 3, CarlosRamirezId),
        Subject.Create(Guid.Parse("f6f0f0b2-68db-4c0f-9636-092c47f2e4cf"), "Estructuras de Datos", 3, CarlosRamirezId),
        Subject.Create(Guid.Parse("0e8a4288-68c0-4e2c-b3da-c8b0717254de"), "Bases de Datos", 3, LauraMendozaId),
        Subject.Create(Guid.Parse("22f81b6c-ee0d-4f9e-a845-40d4d716b6ca"), "Arquitectura de Software", 3, LauraMendozaId),
        Subject.Create(Guid.Parse("7e1c84a4-8975-4af2-87b9-dd08eb529ba4"), "Redes de Computadores", 3, DiegoPenaId),
        Subject.Create(Guid.Parse("71ff60e9-8820-4a9c-93d7-22f2d35232e7"), "Sistemas Operativos", 3, DiegoPenaId),
        Subject.Create(Guid.Parse("8743dd51-8b11-4145-afdd-f6dc5444fd3f"), "Ingles Tecnico", 3, JulianaPardoId),
        Subject.Create(Guid.Parse("9c3fe464-7dcf-425c-aea6-92f3d7590bde"), "Metodologia de la Investigacion", 3, JulianaPardoId)
    ];
}
