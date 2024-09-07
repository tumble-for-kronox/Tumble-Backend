# Stage 1: Build the application
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /build

# Copy csproj and restore as distinct layers
COPY ["TumbleBackend/*.csproj", "TumbleBackend/"]
RUN dotnet restore "TumbleBackend/TumbleBackend.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/build/TumbleBackend"
RUN dotnet build "TumbleBackend.csproj" -c Release -o /app/build

# Publish the project
RUN dotnet publish "TumbleBackend.csproj" -c Release -o /app/publish

# Stage 2: Prepare the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "TumbleBackend.dll"]
