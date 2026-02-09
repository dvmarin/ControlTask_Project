# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY ["ControlTask.API/ControlTask.API.csproj", "ControlTask.API/"]
COPY ["ControlTask.Application/ControlTask.Application.csproj", "ControlTask.Application/"]
COPY ["ControlTask.Domain/ControlTask.Domain.csproj", "ControlTask.Domain/"]
COPY ["ControlTask.Infrastructure/ControlTask.Infrastructure.csproj", "ControlTask.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "ControlTask.API/ControlTask.API.csproj"

# Copiar todo el código
COPY . .

# Construir la aplicación
WORKDIR "/src/ControlTask.API"
RUN dotnet build "ControlTask.API.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "ControlTask.API.csproj" -c Release -o /app/publish

# Etapa de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copiar la aplicación publicada
COPY --from=publish /app/publish .

# Variable de entorno para base de datos (se puede sobreescribir)
ENV ConnectionStrings__DefaultConnection="Server=sqlserver;Database=TeamTasksSample;User=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True"

# Comando de inicio
ENTRYPOINT ["dotnet", "ControlTask.API.dll"]