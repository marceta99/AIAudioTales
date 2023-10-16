# Use the official .NET SDK image, which contains both the SDK and runtime
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

ENV ASPNETCORE_URLS=http://+5000

# Set the working directory in the container
WORKDIR /app

# Copy your application files to the container
COPY . ./

# Build the application in Release mode
RUN dotnet publish -c Release -o out

# Expose port 5000 for the application (optional)
EXPOSE 5000

# Set the entry point for the application
CMD ["dotnet", "/app/out/AIAudioTalesServer.dll"]
