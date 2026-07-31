using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Application.Common.Exceptions;
using Clt.Api.Domain.Entities;
using Clt.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Application.Currencies.Commands.CreateCurrency;

public sealed class CreateCurrencyHandler(AppDbContext dbContext)
    : ICommandHandler<CreateCurrencyCommand, CurrencyResponse>
{
    public async Task<CurrencyResponse> Handle(
        CreateCurrencyCommand command,
        CancellationToken cancellationToken)
    {
        var code = command.Code.Trim().ToUpperInvariant();

        if (await dbContext.Currencies.AnyAsync(currency => currency.Code == code, cancellationToken))
        {
            throw new ConflictException("A currency with this code already exists.");
        }

        var currency = new Currency
        {
            Code = code,
            Name = command.Name.Trim(),
            RateToBase = command.RateToBase
        };

        dbContext.Currencies.Add(currency);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CurrencyResponse.FromEntity(currency);
    }
}
