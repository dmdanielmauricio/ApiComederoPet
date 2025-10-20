# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiamos los archivos del proyecto
COPY ApiComederoPet.csproj ./
RUN dotnet restore ApiComederoPet.csproj

# Copiamos todo el resto del código
COPY . ./
RUN dotnet publish ApiComederoPet.csproj -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ApiComederoPet.dll"]




