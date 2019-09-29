# Stage 1 - Build

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

WORKDIR /app

# Copy .sln + .csproj and restore as a distinct layer
COPY *.sln .
COPY *.csproj .
# COPY Health0.Tests/*.csproj ./Health0.Tests/
RUN dotnet restore

# Copy over everything else and build app
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2 - Run

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.5-alpine3.9 AS runtime
WORKDIR /app
COPY --from=build /app/Health0/out ./

ENTRYPOINT ["dotnet", "Health0.dll", "--urls", "http://0.0.0.0:5001"]
