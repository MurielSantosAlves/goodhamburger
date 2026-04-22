# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/GoodHamburger.Web/GoodHamburger.Web.csproj", "src/GoodHamburger.Web/"]
COPY ["src/GoodHamburger.Domain/GoodHamburger.Domain.csproj", "src/GoodHamburger.Domain/"]
RUN dotnet restore "src/GoodHamburger.Web/GoodHamburger.Web.csproj"

# Copy all files and build
COPY ["src/GoodHamburger.Web/", "src/GoodHamburger.Web/"]
COPY ["src/GoodHamburger.Domain/", "src/GoodHamburger.Domain/"]
WORKDIR "/src/src/GoodHamburger.Web"
RUN dotnet build "GoodHamburger.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "GoodHamburger.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoodHamburger.Web.dll"]
