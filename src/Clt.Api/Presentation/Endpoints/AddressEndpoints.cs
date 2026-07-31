using Clt.Api.Application.Addresses.Commands.DeleteAddress;
using Clt.Api.Application.Addresses.Commands.UpdateAddress;
using Clt.Api.Presentation.Filters;

namespace Clt.Api.Presentation.Endpoints;

public static class AddressEndpoints
{
    public static IEndpointRouteBuilder MapAddressEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var addresses = endpoints.MapGroup("/addresses").WithTags("Addresses");

        addresses.MapPut("/{id:int:min(1)}", async (
            int id,
            UpdateAddressCommand command,
            UpdateAddressHandler handler,
            CancellationToken cancellationToken) =>
        {
            var address = await handler.Handle(
                new UpdateAddressRequest(id, command),
                cancellationToken);
            return Results.Ok(address);
        })
        .AddEndpointFilter<ValidationFilter<UpdateAddressCommand>>()
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);

        addresses.MapDelete("/{id:int:min(1)}", async (
            int id,
            DeleteAddressHandler handler,
            CancellationToken cancellationToken) =>
        {
            await handler.Handle(new DeleteAddressCommand(id), cancellationToken);
            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }
}
