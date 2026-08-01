FROM mcr.microsoft.com/dotnet/sdk:10.0.302-noble AS build

WORKDIR /source

COPY src/Clt.Api/Clt.Api.csproj src/Clt.Api/
RUN dotnet restore src/Clt.Api/Clt.Api.csproj

COPY . .
RUN dotnet publish src/Clt.Api/Clt.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0.10-noble AS runtime

WORKDIR /app

RUN mkdir -p /app/data && chown app:app /app/data

COPY --from=build --chown=app:app /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0

EXPOSE 8080
VOLUME ["/app/data"]

USER app

ENTRYPOINT ["dotnet", "Clt.Api.dll"]
