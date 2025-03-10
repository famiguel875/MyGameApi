# Etapa base: imagen runtime de ASP.NET Core (.NET 8.0)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# Si en Render necesitas un usuario específico, puedes configurar la variable de entorno APP_UID;
# en caso contrario, elimina o comenta la siguiente línea:
# USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de compilación: imagen SDK de .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copia el archivo de proyecto y restaura las dependencias
COPY ["MyGameApi.csproj", "./"]
RUN dotnet restore "MyGameApi.csproj"
# Copia el resto del código fuente y compila la aplicación
COPY . .
WORKDIR "/src/"
RUN dotnet build "MyGameApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publicación: genera la versión optimizada para producción
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MyGameApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final: imagen runtime que ejecutará la aplicación publicada
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyGameApi.dll"]
