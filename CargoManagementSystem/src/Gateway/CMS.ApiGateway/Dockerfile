FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Gateway/CMS.ApiGateway/CMS.ApiGateway.csproj", "src/Gateway/CMS.ApiGateway/"]
COPY ["src/Shared/CMS.Shared/CMS.Shared.csproj", "src/Shared/CMS.Shared/"]

RUN dotnet restore "src/Gateway/CMS.ApiGateway/CMS.ApiGateway.csproj"

COPY . .
WORKDIR "/src/src/Gateway/CMS.ApiGateway"
RUN dotnet build "CMS.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CMS.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.ApiGateway.dll"]
