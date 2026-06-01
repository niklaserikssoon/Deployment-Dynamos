# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["workoutapp-API.csproj", "workoutapp-API/"]
RUN dotnet restore "workoutapp-API/workoutapp-API.csproj"

COPY . workoutapp-API/
WORKDIR /src/workoutapp-API
RUN dotnet publish "workoutapp-API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "workoutapp-API.dll"]