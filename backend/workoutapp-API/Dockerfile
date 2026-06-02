FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Run as non-root user
RUN adduser --disabled-password --gecos "" appuser
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["workoutapp-API.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build --chown=appuser:appuser /app/publish .
ENTRYPOINT ["dotnet", "workoutapp-API.dll"]
