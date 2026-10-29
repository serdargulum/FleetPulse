FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["./Services/VehicleGateway/VehicleGateway.csproj", "./Services/VehicleGateway/"]
RUN dotnet restore "./Services/VehicleGateway/VehicleGateway.csproj"

COPY . .
WORKDIR "/src/Services/VehicleGateway"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "VehicleGateway.dll"]