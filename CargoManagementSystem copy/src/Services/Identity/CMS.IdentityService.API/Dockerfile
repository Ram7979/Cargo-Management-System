FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/Identity/CMS.IdentityService.API/CMS.IdentityService.API.csproj", "src/Services/Identity/CMS.IdentityService.API/"]
COPY ["src/Services/Identity/CMS.IdentityService.Application/CMS.IdentityService.Application.csproj", "src/Services/Identity/CMS.IdentityService.Application/"]
COPY ["src/Services/Identity/CMS.IdentityService.Domain/CMS.IdentityService.Domain.csproj", "src/Services/Identity/CMS.IdentityService.Domain/"]
COPY ["src/Services/Identity/CMS.IdentityService.Infrastructure/CMS.IdentityService.Infrastructure.csproj", "src/Services/Identity/CMS.IdentityService.Infrastructure/"]
COPY ["src/Shared/CMS.Shared/CMS.Shared.csproj", "src/Shared/CMS.Shared/"]

RUN dotnet restore "src/Services/Identity/CMS.IdentityService.API/CMS.IdentityService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/Identity/CMS.IdentityService.API"
RUN dotnet build "CMS.IdentityService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS.IdentityService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.IdentityService.API.dll"]
