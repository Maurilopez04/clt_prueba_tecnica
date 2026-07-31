using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Application.Common.Exceptions;
using Clt.Api.Infrastructure.Persistence;

namespace Clt.Api.Application.Addresses.Commands.UpdateAddress;

public sealed record UpdateAddressRequest(int Id, UpdateAddressCommand Command);

public sealed class UpdateAddressHandler(AppDbContext dbContext)
    : ICommandHandler<UpdateAddressRequest, AddressResponse>
{
    public async Task<AddressResponse> Handle(
        UpdateAddressRequest request,
        CancellationToken cancellationToken)
    {
        var address = await dbContext.Addresses.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Address not found.");

        address.Street = request.Command.Street.Trim();
        address.City = request.Command.City.Trim();
        address.Country = request.Command.Country.Trim();
        address.ZipCode = string.IsNullOrWhiteSpace(request.Command.ZipCode)
            ? null
            : request.Command.ZipCode.Trim();

        await dbContext.SaveChangesAsync(cancellationToken);

        return AddressResponse.FromEntity(address);
    }
}
