# Stage 1: Build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

# Set working directory inside the container
WORKDIR /App

# Copy everything from the local directory to the container
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build and publish the application in Release mode
RUN dotnet publish -c Release -o out

# Stage 2: Runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set working directory inside the container
WORKDIR /App

# Copy published output from the build-env stage to the current directory
COPY --from=build-env /App/out .

# Specify the entry point for the container
ENTRYPOINT ["dotnet", "SuperTracker.PixelApi.dll"]