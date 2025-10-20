# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos todo el proyecto
COPY . .

# Busca autom�ticamente un .sln o .csproj
RUN dotnet restore **/*.sln || dotnet restore **/*.csproj

# Si tu .csproj est� en una subcarpeta, c�mbiala aqu�
WORKDIR /src/ApiComederoPet

RUN dotnet publish -c Release -o /app

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ApiComederoPet.dll"]
