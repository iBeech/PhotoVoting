# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy the project files to the container
COPY ["PhotoVotingApp.csproj", "./"]

# Restore NuGet packages
RUN dotnet restore "./PhotoVotingApp.csproj"

# Copy the rest of the project files
COPY . .

# Build the application
RUN dotnet build "PhotoVotingApp.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PhotoVotingApp.csproj" -c Release -o /app/publish

# Use the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the ASPNETCORE_ENVIRONMENT to Production
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose the ports
EXPOSE 80
EXPOSE 443

# Entry point command
ENTRYPOINT ["dotnet", "PhotoVotingApp.dll"]