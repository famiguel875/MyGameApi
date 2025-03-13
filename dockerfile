# Etapa de compilación: imagen SDK de .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build
WORKDIR /MyGameApi

# Copia todo el contenido del contexto
COPY . ./

# Restaura dependencias y publica en modo Release
RUN dotnet restore "MyGameApi.csproj"
RUN dotnet publish "MyGameApi.csproj" -c Release -o out

# Etapa final: imagen runtime de ASP.NET Core (.NET 8.0)
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
WORKDIR /MyGameApi
COPY --from=build /MyGameApi/out .
ENTRYPOINT ["dotnet", "MyGameApi.dll"]