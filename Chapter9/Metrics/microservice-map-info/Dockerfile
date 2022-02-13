#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["microservice-map-info/microservice-map-info.csproj", "microservice-map-info/"]
RUN dotnet restore "microservice-map-info/microservice-map-info.csproj"
COPY . .
WORKDIR "/src/microservice-map-info"
RUN dotnet build "microservice-map-info.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "microservice-map-info.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "microservice-map-info.dll"]