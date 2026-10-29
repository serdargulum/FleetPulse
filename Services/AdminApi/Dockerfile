FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["./Services/AdminApi/AdminApi.csproj", "./Services/AdminApi/"]
RUN dotnet restore "./Services/AdminApi/AdminApi.csproj"

COPY . .
WORKDIR "/src/Services/AdminApi"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# PostgreSQL / Npgsql için gerekli GSSAPI / Kerberos kütüphanesini ekliyoruz
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AdminApi.dll"]