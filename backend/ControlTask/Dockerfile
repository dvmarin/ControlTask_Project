# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 1. Copiar archivo .sln si existe
COPY *.sln ./

# 2. Copiar archivos .csproj de todos los proyectos
COPY ControlTask/*.csproj ./ControlTask/
COPY ControlTask.Application/*.csproj ./ControlTask.Application/
COPY ControlTask.Domain/*.csproj ./ControlTask.Domain/
COPY ControlTask.Infrastructure/*.csproj ./ControlTask.Infrastructure/
COPY ControlTask.Tests/*.csproj ./ControlTask.Tests/

# 3. Restaurar dependencias solo del proyecto API (no de toda la solución)
RUN dotnet restore "ControlTask/ControlTask.API.csproj"

# 4. Copiar todo el código
COPY . .

# 5. Build del proyecto API
WORKDIR /app/ControlTask
RUN dotnet build "ControlTask.API.csproj" -c Release -o /out

# Etapa 2: Publicar
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
WORKDIR /app
COPY --from=build /app .
WORKDIR /app/ControlTask
RUN dotnet publish "ControlTask.API.csproj" -c Release -o /app/publish

# Etapa 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ControlTask.API.dll"]