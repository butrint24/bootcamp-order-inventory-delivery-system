# ============ BUILD ============
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/DeliveryService/*.csproj src/DeliveryService/
COPY src/Shared/*.csproj         src/Shared/

RUN dotnet restore src/DeliveryService/DeliveryService.csproj

COPY . .

RUN dotnet publish src/DeliveryService/DeliveryService.csproj -c Release -o /out

# ============ RUNTIME ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://0.0.0.0:7004
EXPOSE 7004
ENTRYPOINT ["dotnet","DeliveryService.dll"]
