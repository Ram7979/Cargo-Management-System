FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/Billing/CMS.BillingService.API/CMS.BillingService.API.csproj", "src/Services/Billing/CMS.BillingService.API/"]
COPY ["src/Services/Billing/CMS.BillingService.Application/CMS.BillingService.Application.csproj", "src/Services/Billing/CMS.BillingService.Application/"]
COPY ["src/Services/Billing/CMS.BillingService.Domain/CMS.BillingService.Domain.csproj", "src/Services/Billing/CMS.BillingService.Domain/"]
COPY ["src/Services/Billing/CMS.BillingService.Infrastructure/CMS.BillingService.Infrastructure.csproj", "src/Services/Billing/CMS.BillingService.Infrastructure/"]
COPY ["src/Shared/CMS.Shared/CMS.Shared.csproj", "src/Shared/CMS.Shared/"]

RUN dotnet restore "src/Services/Billing/CMS.BillingService.API/CMS.BillingService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/Billing/CMS.BillingService.API"
RUN dotnet build "CMS.BillingService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS.BillingService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.BillingService.API.dll"]
