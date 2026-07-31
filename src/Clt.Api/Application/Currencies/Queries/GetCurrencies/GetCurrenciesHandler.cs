using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Application.Currencies.Queries.GetCurrencies;

public sealed record GetCurrenciesQuery;

public sealed class GetCurrenciesHandler(AppDbContext dbContext)
    : IQueryHandler<GetCurrenciesQuery, IReadOnlyCollection<CurrencyResponse>>
{
    public async Task<IReadOnlyCollection<CurrencyResponse>> Handle(
        GetCurrenciesQuery query,
        CancellationToken cancellationToken)
    {
        return await dbContext.Currencies
            .AsNoTracking()
            .OrderBy(currency => currency.Code)
            .Select(currency => new CurrencyResponse(
                currency.Id,
                currency.Code,
                currency.Name,
                currency.RateToBase))
            .ToListAsync(cancellationToken);
    }
}
