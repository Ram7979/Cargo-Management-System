FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Services/Customer/CMS.CustomerService.API/CMS.CustomerService.API.csproj", "src/Services/Customer/CMS.CustomerService.API/"]
COPY ["src/Services/Customer/CMS.CustomerService.Application/CMS.CustomerService.Application.csproj", "src/Services/Customer/CMS.CustomerService.Application/"]
COPY ["src/Services/Customer/CMS.CustomerService.Domain/CMS.CustomerService.Domain.csproj", "src/Services/Customer/CMS.CustomerService.Domain/"]
COPY ["src/Services/Customer/CMS.CustomerService.Infrastructure/CMS.CustomerService.Infrastructure.csproj", "src/Services/Customer/CMS.CustomerService.Infrastructure/"]
COPY ["src/Shared/CMS.Shared/CMS.Shared.csproj", "src/Shared/CMS.Shared/"]

RUN dotnet restore "src/Services/Customer/CMS.CustomerService.API/CMS.CustomerService.API.csproj"

COPY . .
WORKDIR "/src/src/Services/Customer/CMS.CustomerService.API"
RUN dotnet build "CMS.CustomerService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS.CustomerService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.CustomerService.API.dll"]
