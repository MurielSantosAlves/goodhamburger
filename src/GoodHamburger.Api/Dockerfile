# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/GoodHamburger.Api/GoodHamburger.Api.csproj", "src/GoodHamburger.Api/"]
COPY ["src/GoodHamburger.Domain/GoodHamburger.Domain.csproj", "src/GoodHamburger.Domain/"]
RUN dotnet restore "src/GoodHamburger.Api/GoodHamburger.Api.csproj"

# Copy all files and build
COPY ["src/GoodHamburger.Api/", "src/GoodHamburger.Api/"]
COPY ["src/GoodHamburger.Domain/", "src/GoodHamburger.Domain/"]
WORKDIR "/src/src/GoodHamburger.Api"
RUN dotnet build "GoodHamburger.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "GoodHamburger.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoodHamburger.Api.dll"]
