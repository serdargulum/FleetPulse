FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Services/VehicleDataSimulator/VehicleDataSimulator.csproj", "Services/VehicleDataSimulator/"]
RUN dotnet restore "Services/VehicleDataSimulator/VehicleDataSimulator.csproj"
COPY . .
WORKDIR "/src/Services/VehicleDataSimulator"
RUN dotnet publish "VehicleDataSimulator.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VehicleDataSimulator.dll"]