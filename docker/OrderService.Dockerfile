# ============ BUILD ============
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/OrderService/*.csproj src/OrderService/
COPY src/Shared/*.csproj      src/Shared/

RUN dotnet restore src/OrderService/OrderService.csproj

COPY . .
RUN dotnet publish src/OrderService/OrderService.csproj -c Release -o /out

# ============ RUNTIME ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://0.0.0.0:7002
EXPOSE 7002
ENTRYPOINT ["dotnet","OrderService.dll"]
