# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# Copy everything
COPY . .

# Restore dependencies and publish
RUN dotnet restore "./YtDownloader/YtDownloader.csproj"
RUN dotnet publish "./YtDownloader/YtDownloader.csproj" -c Release -o /app --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

# Install ffmpeg
RUN apt-get update && apt-get install -y ffmpeg && rm -rf /var/lib/apt/lists/*

# Copy published files from build stage
COPY --from=build /app ./

# Set the entry point
ENTRYPOINT ["dotnet", "YtDownloader.dll"]
