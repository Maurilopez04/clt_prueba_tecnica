using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Application.Common.Exceptions;
using Clt.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Application.Addresses.Commands.DeleteAddress;

public sealed record DeleteAddressCommand(int Id);

public sealed class DeleteAddressHandler(AppDbContext dbContext)
    : ICommandHandler<DeleteAddressCommand, bool>
{
    public async Task<bool> Handle(
        DeleteAddressCommand command,
        CancellationToken cancellationToken)
    {
        var deleted = await dbContext.Addresses
            .Where(address => address.Id == command.Id)
            .ExecuteDeleteAsync(cancellationToken);

        if (deleted == 0)
        {
            throw new NotFoundException("Address not found.");
        }

        return true;
    }
}
