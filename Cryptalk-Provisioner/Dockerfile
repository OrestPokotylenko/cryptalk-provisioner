# --- Base runtime (used by final app image) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Cryptalk-Provisioner/Cryptalk-Provisioner.csproj Cryptalk-Provisioner/
RUN dotnet restore Cryptalk-Provisioner/Cryptalk-Provisioner.csproj
COPY . .
WORKDIR /src/Cryptalk-Provisioner
RUN dotnet build Cryptalk-Provisioner.csproj -c $BUILD_CONFIGURATION -o /app/build

# --- Publish stage ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish Cryptalk-Provisioner.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- Final app image ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cryptalk-Provisioner.dll"]

# --- Migration runner image ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS migrator
WORKDIR /app

COPY Cryptalk-Provisioner/Cryptalk-Provisioner.csproj Cryptalk-Provisioner/
RUN dotnet restore Cryptalk-Provisioner/Cryptalk-Provisioner.csproj
COPY . .

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# copy migrate.sh into container
COPY migrate.sh /app/migrate.sh
RUN chmod +x /app/migrate.sh

ENTRYPOINT ["/app/migrate.sh"]