namespace Clt.Api.Application.Addresses.Commands.CreateAddress;

public sealed record CreateAddressCommand(
    string Street,
    string City,
    string Country,
    string? ZipCode);
