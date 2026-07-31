namespace Clt.Api.Application.Currencies.Commands.CreateCurrency;

public sealed record CreateCurrencyCommand(string Code, string Name, decimal RateToBase);
