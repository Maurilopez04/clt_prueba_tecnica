using Clt.Api.Application.Common.Abstractions;
using Clt.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery(bool? IsActive);

public sealed class GetUsersHandler(AppDbContext dbContext)
    : IQueryHandler<GetUsersQuery, IReadOnlyCollection<UserResponse>>
{
    public async Task<IReadOnlyCollection<UserResponse>> Handle(
        GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        var users = dbContext.Users.AsNoTracking();

        if (query.IsActive.HasValue)
        {
            users = users.Where(user => user.IsActive == query.IsActive.Value);
        }

        return await users
            .OrderBy(user => user.Id)
            .Select(user => new UserResponse(user.Id, user.Name, user.Email, user.IsActive))
            .ToListAsync(cancellationToken);
    }
}
