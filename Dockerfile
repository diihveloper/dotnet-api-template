FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY DiihTemplate.sln .
COPY src/DiihTemplate.Api/DiihTemplate.Api.csproj src/DiihTemplate.Api/
COPY src/DiihTemplate.Application/DiihTemplate.Application.csproj src/DiihTemplate.Application/
COPY src/DiihTemplate.Application.Contracts/DiihTemplate.Application.Contracts.csproj src/DiihTemplate.Application.Contracts/
COPY src/DiihTemplate.Core/DiihTemplate.Core.csproj src/DiihTemplate.Core/
COPY src/DiihTemplate.Data/DiihTemplate.Data.csproj src/DiihTemplate.Data/
COPY src/DiihTemplate.Domain/DiihTemplate.Domain.csproj src/DiihTemplate.Domain/
COPY src/DiihTemplate.Domain.Shared/DiihTemplate.Domain.Shared.csproj src/DiihTemplate.Domain.Shared/
COPY src/DiihTemplate.Infra/DiihTemplate.Infra.csproj src/DiihTemplate.Infra/

RUN dotnet restore

COPY src/ src/
RUN dotnet publish src/DiihTemplate.Api/DiihTemplate.Api.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DiihTemplate.Api.dll"]
