# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar solución y restaurar dependencias
COPY ApiComederoPet.sln ./
COPY ApiComederoPet.csproj ./
RUN dotnet restore ApiComederoPet.sln

# Copiar todo el código fuente
COPY ApicomederoPet/ ./ApicomederoPet

# Publicar la aplicación
WORKDIR /src
RUN dotnet publish ApiComederoPet.sln -c Release -o /app

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ApiComederoPet.dll"]

