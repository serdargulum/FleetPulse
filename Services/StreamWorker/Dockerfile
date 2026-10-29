FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["./Services/StreamWorker/StreamWorker.csproj", "./Services/StreamWorker/"]
RUN dotnet restore "./Services/StreamWorker/StreamWorker.csproj"

COPY . .
WORKDIR "/src/Services/StreamWorker"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StreamWorker.dll"]