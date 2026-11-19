# ===== BUILD STAGE =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Couchy.Api.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app

# ===== RUN STAGE =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Couchy.Api.dll"]
