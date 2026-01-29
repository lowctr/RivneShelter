FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RivneSheltersProject.csproj", "./"]
RUN dotnet restore "RivneSheltersProject.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "RivneSheltersProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RivneSheltersProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Create data directory for SQLite database
RUN mkdir -p /app/data

COPY --from=publish /app/publish .

# Copy KML file for initial seeding
COPY easy.kml /app/easy.kml

ENTRYPOINT ["dotnet", "RivneSheltersProject.dll"]
