# Development stage - with hot-reload support
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS development
WORKDIR /app/src

# Install dotnet watch
RUN dotnet tool install -g dotnet-watch

# Restore dependencies - cached layer
COPY ["Loja.API/Loja.API.csproj", "Loja.API/"]
COPY ["Loja.Application/Loja.Application.csproj", "Loja.Application/"]
COPY ["Loja.Domain/Loja.Domain.csproj", "Loja.Domain/"]
COPY ["Loja.Infrastructure/Loja.Infrastructure.csproj", "Loja.Infrastructure/"]
RUN dotnet restore "Loja.API/Loja.API.csproj"

COPY . .
WORKDIR /app/src/Loja.API
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "watch", "run", "--no-launch-profile", "--non-interactive"]

# Production stage - optimized build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Loja.API/Loja.API.csproj", "Loja.API/"]
COPY ["Loja.Application/Loja.Application.csproj", "Loja.Application/"]
COPY ["Loja.Domain/Loja.Domain.csproj", "Loja.Domain/"]
COPY ["Loja.Infrastructure/Loja.Infrastructure.csproj", "Loja.Infrastructure/"]
RUN dotnet restore "Loja.API/Loja.API.csproj"

COPY . .
WORKDIR /src/Loja.API
RUN dotnet publish "Loja.API.csproj" --configuration Release --output /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Loja.API.dll"]
