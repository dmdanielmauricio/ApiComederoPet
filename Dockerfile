# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivo de proyecto y restaurar dependencias
COPY ApiComederoPet/*.csproj ApiComederoPet/
RUN dotnet restore ApiComederoPet/ApiComederoPet.csproj

# Copiar todo el código
COPY . .

# Compilar y publicar
WORKDIR /src/ApiComederoPet
RUN dotnet publish -c Release -o /app

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ApiComederoPet.dll"]
