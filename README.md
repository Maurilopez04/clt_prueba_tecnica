# Prueba técnica CLT

API REST desarrollada con .NET 10, Minimal API, Entity Framework Core, SQLite, FluentValidation y CQRS.

## Ejecución con .NET

```bash
dotnet restore
dotnet dev-certs https --trust
dotnet run --project src/Clt.Api --launch-profile https
```

- API: `https://localhost:7080`
- Swagger: `https://localhost:7080/swagger`

## Ejecución con Docker

Requiere únicamente Docker:

```bash
docker compose up --build -d
```

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`

La base SQLite se conserva en el volumen `clt-data` al detener o reemplazar el contenedor.

## API Key

Todos los endpoints requieren:

```http
X-API-KEY: clt-dev-8D7C4F91A6E23B50D9F714C82A30E65B
```

La clave se configura en `src/Clt.Api/appsettings.json` para ambos modos de ejecución.

## Swagger

Swagger siempre requiere autenticación Basic:

```text
Usuario: clt
Contraseña: clt-docs-6A19F472C8D34E50B71F9A25D8036C4E
```

Una vez dentro, el botón `Authorize` permite ingresar la API Key para probar los endpoints.

## Base de datos

SQLite se crea y actualiza automáticamente al iniciar la aplicación mediante migraciones de Entity Framework Core. El archivo se guarda en `src/Clt.Api/clt.db`.

Para crear una migración después de modificar el modelo:

```bash
dotnet tool restore
dotnet ef migrations add NombreMigracion --project src/Clt.Api
```

## Endpoints

| Método | Ruta |
| --- | --- |
| POST | `/users` |
| GET | `/users?isActive=true` |
| GET | `/users/{id}` |
| PUT | `/users/{id}` |
| DELETE | `/users/{id}` |
| POST | `/users/{userId}/addresses` |
| GET | `/users/{userId}/addresses` |
| PUT | `/addresses/{id}` |
| DELETE | `/addresses/{id}` |
| GET | `/currencies` |
| POST | `/currencies` |
| POST | `/currency/convert` |

## Ejemplo

```bash
curl -X POST https://localhost:7080/users \
  -H "X-API-KEY: clt-dev-8D7C4F91A6E23B50D9F714C82A30E65B" \
  -H "Content-Type: application/json" \
  -d '{"name":"Juan","email":"juan@test.com"}'
```

Se implementaron todos los endpoints solicitados, validaciones con FluentValidation, seguridad por API Key, CQRS, Swagger y persistencia con EF Core y SQLite. La aplicación crea PYG y USD como monedas iniciales.
