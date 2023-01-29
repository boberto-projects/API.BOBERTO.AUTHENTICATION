#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["API.AUTHENTICATION.BOBERTO.WEB/API.AUTHENTICATION.BOBERTO.WEB.csproj", "API.AUTHENTICATION.BOBERTO.WEB/"]
RUN dotnet restore "API.AUTHENTICATION.BOBERTO.WEB/API.AUTHENTICATION.BOBERTO.WEB.csproj"
COPY . .
WORKDIR "/src/API.AUTHENTICATION.BOBERTO.WEB"
RUN dotnet build "API.AUTHENTICATION.BOBERTO.WEB.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.AUTHENTICATION.BOBERTO.WEB.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.AUTHENTICATION.BOBERTO.WEB.dll"]