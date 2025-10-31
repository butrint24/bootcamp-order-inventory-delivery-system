# ============ BUILD ============
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy vetëm csproj për cache të shpejtë
COPY src/InventoryService/*.csproj src/InventoryService/
COPY src/Shared/*.csproj          src/Shared/

RUN dotnet restore src/InventoryService/InventoryService.csproj

# Tani kopjo gjithë repo-n
COPY . .

RUN dotnet publish src/InventoryService/InventoryService.csproj -c Release -o /out

# ============ RUNTIME ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://0.0.0.0:7001
EXPOSE 7001
ENTRYPOINT ["dotnet","InventoryService.dll"]
