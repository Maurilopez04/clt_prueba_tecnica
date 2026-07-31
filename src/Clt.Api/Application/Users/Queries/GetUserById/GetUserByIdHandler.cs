using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Application.Common.Exceptions;
using Clt.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(int Id);

public sealed class GetUserByIdHandler(AppDbContext dbContext)
    : IQueryHandler<GetUserByIdQuery, UserResponse>
{
    public async Task<UserResponse> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(user => user.Id == query.Id)
            .Select(user => new UserResponse(user.Id, user.Name, user.Email, user.IsActive))
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("User not found.");
    }
}
