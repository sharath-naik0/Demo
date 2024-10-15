FROM mcr.microsoft.com/dotnet/sdk:7.0

# Install necessary tools (like Chromium for Selenium)
RUN apt-get update && apt-get install -y \
    chromium \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
