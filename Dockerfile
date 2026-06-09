# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["linkedin-api.csproj", "./"]
RUN dotnet restore "linkedin-api.csproj"

# Copy the rest of the source code
COPY . .

# Build the project in Release mode
RUN dotnet build "linkedin-api.csproj" -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish "linkedin-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Expose port 8080 (default for .NET 8)
EXPOSE 8080

# Copy published output from build stage
COPY --from=publish /app/publish .

# Create the SimpleStorage directory so physical file providers don't throw on startup
RUN mkdir -p /app/SimpleStorage

# Run the API
ENTRYPOINT ["dotnet", "linkedin-api.dll"]
