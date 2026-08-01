using Clt.Api.Application.Addresses.Commands.CreateAddress;
using Clt.Api.Application.Addresses.Commands.DeleteAddress;
using Clt.Api.Application.Addresses.Commands.UpdateAddress;
using Clt.Api.Application.Addresses.Queries.GetUserAddresses;
using Clt.Api.Application.Common.Security;
using Clt.Api.Application.Currencies.Commands.CreateCurrency;
using Clt.Api.Application.Currencies.Queries.GetCurrencies;
using Clt.Api.Application.CurrencyConversion.ConvertCurrency;
using Clt.Api.Application.Users.Commands.CreateUser;
using Clt.Api.Application.Users.Commands.DeleteUser;
using Clt.Api.Application.Users.Commands.UpdateUser;
using Clt.Api.Application.Users.Queries.GetUserById;
using Clt.Api.Application.Users.Queries.GetUsers;
using Clt.Api.Domain.Entities;
using Clt.Api.Infrastructure.Persistence;
using Clt.Api.Infrastructure.Security;
using Clt.Api.Presentation.Endpoints;
using Clt.Api.Presentation.Middleware;
using FluentValidation;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("DefaultConnection is required.");
}

var apiKey = builder.Configuration["ApiKey"];
if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException("ApiKey configuration is required.");
}

var sqliteConnection = new SqliteConnectionStringBuilder(connectionString);
if (!Path.IsPathRooted(sqliteConnection.DataSource))
{
    sqliteConnection.DataSource = Path.Combine(
        builder.Environment.ContentRootPath,
        sqliteConnection.DataSource);
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(sqliteConnection.ConnectionString));
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>(ServiceLifetime.Scoped);
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();

builder.Services.AddScoped<CreateUserHandler>();
builder.Services.AddScoped<UpdateUserHandler>();
builder.Services.AddScoped<DeleteUserHandler>();
builder.Services.AddScoped<GetUsersHandler>();
builder.Services.AddScoped<GetUserByIdHandler>();
builder.Services.AddScoped<CreateAddressHandler>();
builder.Services.AddScoped<UpdateAddressHandler>();
builder.Services.AddScoped<DeleteAddressHandler>();
builder.Services.AddScoped<GetUserAddressesHandler>();
builder.Services.AddScoped<CreateCurrencyHandler>();
builder.Services.AddScoped<GetCurrenciesHandler>();
builder.Services.AddScoped<ConvertCurrencyHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Description = "API key required to access the endpoints."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("ApiKey", document)] = []
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Configuration.GetValue("HttpsRedirection:Enabled", true))
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<SwaggerBasicAuthMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();

app.MapUserEndpoints();
app.MapAddressEndpoints();
app.MapCurrencyEndpoints();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();

    var seedCurrencies = new[]
    {
        new Currency { Code = "PYG", Name = "Paraguayan Guaraní", RateToBase = 1m },
        new Currency { Code = "USD", Name = "US Dollar", RateToBase = 6100m }
    };

    var seedCodes = seedCurrencies.Select(currency => currency.Code).ToArray();
    var existingCodes = await dbContext.Currencies
        .Where(currency => seedCodes.Contains(currency.Code))
        .Select(currency => currency.Code)
        .ToListAsync();

    var missingCurrencies = seedCurrencies
        .Where(currency => !existingCodes.Contains(currency.Code))
        .ToArray();

    if (missingCurrencies.Length > 0)
    {
        dbContext.Currencies.AddRange(missingCurrencies);
        await dbContext.SaveChangesAsync();
    }
}

await app.RunAsync();
