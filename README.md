# Prueba técnica CLT

API REST desarrollada con .NET 10, Minimal API, Entity Framework Core, SQLite, FluentValidation y CQRS.

## Ejecución

```bash
dotnet restore
dotnet dev-certs https --trust
dotnet run --project src/Clt.Api --launch-profile https
```

- API: `https://localhost:7080`
- Swagger: `https://localhost:7080/swagger`

## API Key

Todos los endpoints requieren:

```http
X-API-KEY: clt-dev-8D7C4F91A6E23B50D9F714C82A30E65B
```

La clave está configurada en `appsettings.json` y puede reemplazarse mediante la variable de entorno `ApiKey`.

## Swagger

En desarrollo, Swagger se puede abrir directamente. Fuera de `Development` requiere autenticación Basic con credenciales configuradas mediante variables de entorno:

```text
Swagger__Username
Swagger__Password
```

Una vez dentro de Swagger, el botón `Authorize` permite ingresar la API Key para probar los endpoints.

## Base de datos

SQLite se crea automáticamente al iniciar la aplicación. El archivo se guarda en `src/Clt.Api/clt.db`.

Para recrear el esquema, se debe detener la aplicación, eliminar `clt.db` y volver a iniciarla.

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
