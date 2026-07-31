using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Application.Common.Exceptions;
using Clt.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Application.Addresses.Queries.GetUserAddresses;

public sealed record GetUserAddressesQuery(int UserId);

public sealed class GetUserAddressesHandler(AppDbContext dbContext)
    : IQueryHandler<GetUserAddressesQuery, IReadOnlyCollection<AddressResponse>>
{
    public async Task<IReadOnlyCollection<AddressResponse>> Handle(
        GetUserAddressesQuery query,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Users.AnyAsync(user => user.Id == query.UserId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }

        return await dbContext.Addresses
            .AsNoTracking()
            .Where(address => address.UserId == query.UserId)
            .OrderBy(address => address.Id)
            .Select(address => new AddressResponse(
                address.Id,
                address.UserId,
                address.Street,
                address.City,
                address.Country,
                address.ZipCode))
            .ToListAsync(cancellationToken);
    }
}
