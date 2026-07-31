using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Application.Common.Exceptions;
using Clt.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Application.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(int Id);

public sealed class DeleteUserHandler(AppDbContext dbContext)
    : ICommandHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken)
    {
        var deleted = await dbContext.Users
            .Where(user => user.Id == command.Id)
            .ExecuteDeleteAsync(cancellationToken);

        if (deleted == 0)
        {
            throw new NotFoundException("User not found.");
        }

        return true;
    }
}
