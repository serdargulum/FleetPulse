FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Services/AnalyticsWorker/AnalyticsWorker.csproj Services/AnalyticsWorker/
RUN dotnet restore Services/AnalyticsWorker/AnalyticsWorker.csproj 

COPY . .
WORKDIR "/src/Services/AnalyticsWorker"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AnalyticsWorker.dll"]