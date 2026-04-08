# .NET 10
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# sln and csproj's so docker reuses the cached layer and dotnet restore will not be run on every build unless a nuget dependency changes
COPY SportsBookingSystem.sln .
COPY SportsBookingSystem.API/SportsBookingSystem.API.csproj                         SportsBookingSystem.API/
COPY SportsBookingSystem.Application/SportsBookingSystem.Application.csproj         SportsBookingSystem.Application/
COPY SportsBookingSystem.Domain/SportsBookingSystem.Domain.csproj                   SportsBookingSystem.Domain/
COPY SportsBookingSystem.Infrastructure/SportsBookingSystem.Infrastructure.csproj   SportsBookingSystem.Infrastructure/

RUN dotnet restore

# Copy the rest of the source code
COPY . .

RUN dotnet publish SportsBookingSystem.API/SportsBookingSystem.API.csproj \
    -c Release \
    --no-restore \
    -o /app/publish

# runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# uploads directory for avatars
RUN mkdir -p wwwroot/uploads

# expose port 8080
EXPOSE 8080

# The entry point: runs `dotnet SportsBookingSystem.API.dll` when the container starts.
ENTRYPOINT ["dotnet", "SportsBookingSystem.API.dll"]