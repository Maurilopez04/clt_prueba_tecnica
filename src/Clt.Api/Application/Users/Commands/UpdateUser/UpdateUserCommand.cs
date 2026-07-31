namespace Clt.Api.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(string Name, string Email, bool IsActive);
