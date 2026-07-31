namespace Clt.Api.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(string Name, string Email, string? Password = null);
