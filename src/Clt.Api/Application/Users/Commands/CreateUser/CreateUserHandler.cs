using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Application.Common.Exceptions;
using Clt.Api.Application.Common.Security;
using Clt.Api.Domain.Entities;
using Clt.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Application.Users.Commands.CreateUser;

public sealed class CreateUserHandler(AppDbContext dbContext, IPasswordHasher passwordHasher)
    : ICommandHandler<CreateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        var email = command.Email.Trim().ToLowerInvariant();

        if (await dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken))
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var user = new User
        {
            Name = command.Name.Trim(),
            Email = email,
            PasswordHash = command.Password is null
                ? null
                : passwordHasher.Hash(command.Password),
            IsActive = true
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return UserResponse.FromEntity(user);
    }
}
