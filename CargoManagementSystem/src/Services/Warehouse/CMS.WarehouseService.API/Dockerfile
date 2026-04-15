FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/Warehouse/CMS.WarehouseService.API/CMS.WarehouseService.API.csproj", "src/Services/Warehouse/CMS.WarehouseService.API/"]
COPY ["src/Services/Warehouse/CMS.WarehouseService.Application/CMS.WarehouseService.Application.csproj", "src/Services/Warehouse/CMS.WarehouseService.Application/"]
COPY ["src/Services/Warehouse/CMS.WarehouseService.Domain/CMS.WarehouseService.Domain.csproj", "src/Services/Warehouse/CMS.WarehouseService.Domain/"]
COPY ["src/Services/Warehouse/CMS.WarehouseService.Infrastructure/CMS.WarehouseService.Infrastructure.csproj", "src/Services/Warehouse/CMS.WarehouseService.Infrastructure/"]
COPY ["src/Shared/CMS.Shared/CMS.Shared.csproj", "src/Shared/CMS.Shared/"]

RUN dotnet restore "src/Services/Warehouse/CMS.WarehouseService.API/CMS.WarehouseService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/Warehouse/CMS.WarehouseService.API"
RUN dotnet build "CMS.WarehouseService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS.WarehouseService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.WarehouseService.API.dll"]
