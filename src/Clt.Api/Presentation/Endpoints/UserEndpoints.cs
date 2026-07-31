using Clt.Api.Application.Addresses.Commands.CreateAddress;
using Clt.Api.Application.Addresses.Queries.GetUserAddresses;
using Clt.Api.Application.Users.Commands.CreateUser;
using Clt.Api.Application.Users.Commands.DeleteUser;
using Clt.Api.Application.Users.Commands.UpdateUser;
using Clt.Api.Application.Users.Queries.GetUserById;
using Clt.Api.Application.Users.Queries.GetUsers;
using Clt.Api.Presentation.Filters;

namespace Clt.Api.Presentation.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var users = endpoints.MapGroup("/users").WithTags("Users");

        users.MapPost("", async (
            CreateUserCommand command,
            CreateUserHandler handler,
            CancellationToken cancellationToken) =>
        {
            var user = await handler.Handle(command, cancellationToken);
            return Results.Created($"/users/{user.Id}", user);
        })
        .AddEndpointFilter<ValidationFilter<CreateUserCommand>>()
        .Produces(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status409Conflict);

        users.MapGet("", async (
            bool? isActive,
            GetUsersHandler handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetUsersQuery(isActive), cancellationToken);
            return Results.Ok(result);
        });

        users.MapGet("/{id:int:min(1)}", async (
            int id,
            GetUserByIdHandler handler,
            CancellationToken cancellationToken) =>
        {
            var user = await handler.Handle(new GetUserByIdQuery(id), cancellationToken);
            return Results.Ok(user);
        })
        .ProducesProblem(StatusCodes.Status404NotFound);

        users.MapPut("/{id:int:min(1)}", async (
            int id,
            UpdateUserCommand command,
            UpdateUserHandler handler,
            CancellationToken cancellationToken) =>
        {
            var user = await handler.Handle(new UpdateUserRequest(id, command), cancellationToken);
            return Results.Ok(user);
        })
        .AddEndpointFilter<ValidationFilter<UpdateUserCommand>>()
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        users.MapDelete("/{id:int:min(1)}", async (
            int id,
            DeleteUserHandler handler,
            CancellationToken cancellationToken) =>
        {
            await handler.Handle(new DeleteUserCommand(id), cancellationToken);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        users.MapPost("/{userId:int:min(1)}/addresses", async (
            int userId,
            CreateAddressCommand command,
            CreateAddressHandler handler,
            CancellationToken cancellationToken) =>
        {
            var address = await handler.Handle(
                new CreateAddressRequest(userId, command),
                cancellationToken);
            return Results.Created($"/addresses/{address.Id}", address);
        })
        .AddEndpointFilter<ValidationFilter<CreateAddressCommand>>()
        .Produces(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        users.MapGet("/{userId:int:min(1)}/addresses", async (
            int userId,
            GetUserAddressesHandler handler,
            CancellationToken cancellationToken) =>
        {
            var addresses = await handler.Handle(
                new GetUserAddressesQuery(userId),
                cancellationToken);
            return Results.Ok(addresses);
        })
        .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
