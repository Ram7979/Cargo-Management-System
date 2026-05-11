FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/Notification/CMS.NotificationService.API/CMS.NotificationService.API.csproj", "src/Services/Notification/CMS.NotificationService.API/"]
COPY ["src/Services/Notification/CMS.NotificationService.Application/CMS.NotificationService.Application.csproj", "src/Services/Notification/CMS.NotificationService.Application/"]
COPY ["src/Services/Notification/CMS.NotificationService.Domain/CMS.NotificationService.Domain.csproj", "src/Services/Notification/CMS.NotificationService.Domain/"]
COPY ["src/Services/Notification/CMS.NotificationService.Infrastructure/CMS.NotificationService.Infrastructure.csproj", "src/Services/Notification/CMS.NotificationService.Infrastructure/"]
COPY ["src/Shared/CMS.Shared/CMS.Shared.csproj", "src/Shared/CMS.Shared/"]

RUN dotnet restore "src/Services/Notification/CMS.NotificationService.API/CMS.NotificationService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/Notification/CMS.NotificationService.API"
RUN dotnet build "CMS.NotificationService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS.NotificationService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.NotificationService.API.dll"]
