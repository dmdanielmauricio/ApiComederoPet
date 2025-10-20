# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todo el contenido del proyecto
COPY . .

# Entramos a la carpeta donde está el .csproj
WORKDIR /src/ApiComederoPet

# Restauramos dependencias
RUN dotnet restore

# Publicamos en modo Release
RUN dotnet publish -c Release -o /app

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ApiComederoPet.dll"]
