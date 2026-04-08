FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/backend/StudentsPlatform.Api/StudentsPlatform.Api.csproj src/backend/StudentsPlatform.Api/
COPY src/backend/StudentsPlatform.Application/StudentsPlatform.Application.csproj src/backend/StudentsPlatform.Application/
COPY src/backend/StudentsPlatform.Domain/StudentsPlatform.Domain.csproj src/backend/StudentsPlatform.Domain/
COPY src/backend/StudentsPlatform.Infrastructure/StudentsPlatform.Infrastructure.csproj src/backend/StudentsPlatform.Infrastructure/

RUN dotnet restore src/backend/StudentsPlatform.Api/StudentsPlatform.Api.csproj

COPY src/backend/ src/backend/

RUN dotnet publish src/backend/StudentsPlatform.Api/StudentsPlatform.Api.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "StudentsPlatform.Api.dll"]
