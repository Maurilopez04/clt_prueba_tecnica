namespace Clt.Api.Application.CurrencyConversion.ConvertCurrency;

public sealed record ConvertCurrencyCommand(
    string FromCurrencyCode,
    string ToCurrencyCode,
    decimal Amount);

public sealed record CurrencyConversionResponse(
    string FromCurrency,
    string ToCurrency,
    decimal OriginalAmount,
    decimal ConvertedAmount);
