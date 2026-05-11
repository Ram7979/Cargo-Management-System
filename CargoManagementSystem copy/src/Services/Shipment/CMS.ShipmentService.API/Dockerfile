FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/Shipment/CMS.ShipmentService.API/CMS.ShipmentService.API.csproj", "src/Services/Shipment/CMS.ShipmentService.API/"]
COPY ["src/Services/Shipment/CMS.ShipmentService.Application/CMS.ShipmentService.Application.csproj", "src/Services/Shipment/CMS.ShipmentService.Application/"]
COPY ["src/Services/Shipment/CMS.ShipmentService.Domain/CMS.ShipmentService.Domain.csproj", "src/Services/Shipment/CMS.ShipmentService.Domain/"]
COPY ["src/Services/Shipment/CMS.ShipmentService.Infrastructure/CMS.ShipmentService.Infrastructure.csproj", "src/Services/Shipment/CMS.ShipmentService.Infrastructure/"]
COPY ["src/Shared/CMS.Shared/CMS.Shared.csproj", "src/Shared/CMS.Shared/"]

RUN dotnet restore "src/Services/Shipment/CMS.ShipmentService.API/CMS.ShipmentService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/Shipment/CMS.ShipmentService.API"
RUN dotnet build "CMS.ShipmentService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS.ShipmentService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.ShipmentService.API.dll"]
