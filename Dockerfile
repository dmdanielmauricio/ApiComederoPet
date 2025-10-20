# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar soluci�n y proyecto
COPY ApiComederoPet.sln ./
COPY ApiComederoPet.csproj ./

# Restaurar dependencias
RUN dotnet restore ApiComederoPet.sln

# Copiar carpeta con el c�digo fuente
COPY ApicomederoPet/ ./ApicomederoPet

# Publicar la app
RUN dotnet publish ApiComederoPet.sln -c Release -o /app

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ApiComederoPet.dll"]



