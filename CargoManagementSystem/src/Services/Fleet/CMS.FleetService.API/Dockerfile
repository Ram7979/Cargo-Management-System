FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/Fleet/CMS.FleetService.API/CMS.FleetService.API.csproj", "src/Services/Fleet/CMS.FleetService.API/"]
COPY ["src/Services/Fleet/CMS.FleetService.Application/CMS.FleetService.Application.csproj", "src/Services/Fleet/CMS.FleetService.Application/"]
COPY ["src/Services/Fleet/CMS.FleetService.Domain/CMS.FleetService.Domain.csproj", "src/Services/Fleet/CMS.FleetService.Domain/"]
COPY ["src/Services/Fleet/CMS.FleetService.Infrastructure/CMS.FleetService.Infrastructure.csproj", "src/Services/Fleet/CMS.FleetService.Infrastructure/"]
COPY ["src/Shared/CMS.Shared/CMS.Shared.csproj", "src/Shared/CMS.Shared/"]

RUN dotnet restore "src/Services/Fleet/CMS.FleetService.API/CMS.FleetService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/Fleet/CMS.FleetService.API"
RUN dotnet build "CMS.FleetService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS.FleetService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.FleetService.API.dll"]
