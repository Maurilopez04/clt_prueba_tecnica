namespace Clt.Api.Application.Addresses.Commands.UpdateAddress;

public sealed record UpdateAddressCommand(
    string Street,
    string City,
    string Country,
    string? ZipCode);
