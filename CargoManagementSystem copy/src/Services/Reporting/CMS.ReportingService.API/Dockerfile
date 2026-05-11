FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/Reporting/CMS.ReportingService.API/CMS.ReportingService.API.csproj", "src/Services/Reporting/CMS.ReportingService.API/"]
COPY ["src/Services/Reporting/CMS.ReportingService.Application/CMS.ReportingService.Application.csproj", "src/Services/Reporting/CMS.ReportingService.Application/"]
COPY ["src/Services/Reporting/CMS.ReportingService.Domain/CMS.ReportingService.Domain.csproj", "src/Services/Reporting/CMS.ReportingService.Domain/"]
COPY ["src/Services/Reporting/CMS.ReportingService.Infrastructure/CMS.ReportingService.Infrastructure.csproj", "src/Services/Reporting/CMS.ReportingService.Infrastructure/"]
COPY ["src/Shared/CMS.Shared/CMS.Shared.csproj", "src/Shared/CMS.Shared/"]

RUN dotnet restore "src/Services/Reporting/CMS.ReportingService.API/CMS.ReportingService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/Reporting/CMS.ReportingService.API"
RUN dotnet build "CMS.ReportingService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS.ReportingService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.ReportingService.API.dll"]
