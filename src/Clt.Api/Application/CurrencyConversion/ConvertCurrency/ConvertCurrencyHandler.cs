using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Application.Common.Exceptions;
using Clt.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Application.CurrencyConversion.ConvertCurrency;

public sealed class ConvertCurrencyHandler(AppDbContext dbContext)
    : ICommandHandler<ConvertCurrencyCommand, CurrencyConversionResponse>
{
    public async Task<CurrencyConversionResponse> Handle(
        ConvertCurrencyCommand command,
        CancellationToken cancellationToken)
    {
        var fromCode = command.FromCurrencyCode.Trim().ToUpperInvariant();
        var toCode = command.ToCurrencyCode.Trim().ToUpperInvariant();

        var currencies = await dbContext.Currencies
            .AsNoTracking()
            .Where(currency => currency.Code == fromCode || currency.Code == toCode)
            .ToDictionaryAsync(currency => currency.Code, cancellationToken);

        if (!currencies.TryGetValue(fromCode, out var fromCurrency))
        {
            throw new NotFoundException($"Currency '{fromCode}' not found.");
        }

        if (!currencies.TryGetValue(toCode, out var toCurrency))
        {
            throw new NotFoundException($"Currency '{toCode}' not found.");
        }

        var baseAmount = command.Amount * fromCurrency.RateToBase;
        var convertedAmount = baseAmount / toCurrency.RateToBase;

        return new CurrencyConversionResponse(
            fromCode,
            toCode,
            command.Amount,
            convertedAmount);
    }
}
