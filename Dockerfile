# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyecto
COPY ApiComederoPet.sln ./
COPY ApiComederoPet.csproj ./

# Restaurar dependencias
RUN dotnet restore ApiComederoPet.sln

# Copiar toda la carpeta con código fuente
COPY ApicomederoPet/ ./ApicomederoPet

# Publicar la aplicación
RUN dotnet publish ApiComederoPet.sln -c Release -o /app

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ApiComederoPet.dll"]


