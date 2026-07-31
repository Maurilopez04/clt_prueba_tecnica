using Clt.Api.Application.Currencies.Commands.CreateCurrency;
using Clt.Api.Application.Currencies.Queries.GetCurrencies;
using Clt.Api.Application.CurrencyConversion.ConvertCurrency;
using Clt.Api.Presentation.Filters;

namespace Clt.Api.Presentation.Endpoints;

public static class CurrencyEndpoints
{
    public static IEndpointRouteBuilder MapCurrencyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var currencies = endpoints.MapGroup("/currencies").WithTags("Currencies");

        currencies.MapGet("", async (
            GetCurrenciesHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetCurrenciesQuery(), cancellationToken);
            return Results.Ok(result);
        });

        currencies.MapPost("", async (
            CreateCurrencyCommand command,
            CreateCurrencyHandler handler,
            CancellationToken cancellationToken) =>
        {
            var currency = await handler.Handle(command, cancellationToken);
            return Results.Created($"/currencies/{currency.Id}", currency);
        })
        .AddEndpointFilter<ValidationFilter<CreateCurrencyCommand>>()
        .Produces(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status409Conflict);

        endpoints.MapPost("/currency/convert", async (
            ConvertCurrencyCommand command,
            ConvertCurrencyHandler handler,
            CancellationToken cancellationToken) =>
        {
            var conversion = await handler.Handle(command, cancellationToken);
            return Results.Ok(conversion);
        })
        .WithTags("Currency conversion")
        .AddEndpointFilter<ValidationFilter<ConvertCurrencyCommand>>()
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return endpoints;
    }
}
