using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Application.Common.Exceptions;
using Clt.Api.Domain.Entities;
using Clt.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Application.Addresses.Commands.CreateAddress;

public sealed record CreateAddressRequest(int UserId, CreateAddressCommand Command);

public sealed class CreateAddressHandler(AppDbContext dbContext)
    : ICommandHandler<CreateAddressRequest, AddressResponse>
{
    public async Task<AddressResponse> Handle(
        CreateAddressRequest request,
        CancellationToken cancellationToken)
    {
        if (!await dbContext.Users.AnyAsync(user => user.Id == request.UserId, cancellationToken))
        {
            throw new NotFoundException("User not found.");
        }

        var address = new Address
        {
            UserId = request.UserId,
            Street = request.Command.Street.Trim(),
            City = request.Command.City.Trim(),
            Country = request.Command.Country.Trim(),
            ZipCode = string.IsNullOrWhiteSpace(request.Command.ZipCode)
                ? null
                : request.Command.ZipCode.Trim()
        };

        dbContext.Addresses.Add(address);
        await dbContext.SaveChangesAsync(cancellationToken);

        return AddressResponse.FromEntity(address);
    }
}
