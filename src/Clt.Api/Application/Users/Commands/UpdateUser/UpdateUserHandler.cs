using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Application.Common.Exceptions;
using Clt.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserRequest(int Id, UpdateUserCommand Command);

public sealed class UpdateUserHandler(AppDbContext dbContext)
    : ICommandHandler<UpdateUserRequest, UserResponse>
{
    public async Task<UserResponse> Handle(
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("User not found.");

        var email = request.Command.Email.Trim().ToLowerInvariant();
        var emailInUse = await dbContext.Users.AnyAsync(
            candidate => candidate.Email == email && candidate.Id != request.Id,
            cancellationToken);

        if (emailInUse)
        {
            throw new ConflictException("A user with this email already exists.");
        }

        user.Name = request.Command.Name.Trim();
        user.Email = email;
        user.IsActive = request.Command.IsActive;

        await dbContext.SaveChangesAsync(cancellationToken);

        return UserResponse.FromEntity(user);
    }
}
