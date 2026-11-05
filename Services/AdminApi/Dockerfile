FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["./Services/AdminApi/AdminApi.Api/AdminApi.Api.csproj", "./Services/AdminApi/AdminApi.Api/"]
COPY ["./Services/AdminApi/AdminApi.Application/AdminApi.Application.csproj", "./Services/AdminApi/AdminApi.Application/"]
COPY ["./Services/AdminApi/AdminApi.Domain/AdminApi.Domain.csproj", "./Services/AdminApi/AdminApi.Domain/"]
COPY ["./Services/AdminApi/AdminApi.Infrastructure/AdminApi.Infrastructure.csproj", "./Services/AdminApi/AdminApi.Infrastructure/"]
RUN dotnet restore "./Services/AdminApi/AdminApi.Api/AdminApi.Api.csproj"

COPY . .
RUN dotnet publish "./Services/AdminApi/AdminApi.Api/AdminApi.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# PostgreSQL / Npgsql için gerekli GSSAPI / Kerberos kütüphanesini ekliyoruz
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgssapi-krb5-2 \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AdminApi.Api.dll"]
