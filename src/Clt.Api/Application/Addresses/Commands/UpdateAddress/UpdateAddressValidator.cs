using FluentValidation;

namespace Clt.Api.Application.Addresses.Commands.UpdateAddress;

public sealed class UpdateAddressValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressValidator()
    {
        RuleFor(command => command.Street)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(command => command.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.Country)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(command => command.ZipCode)
            .MaximumLength(20);
    }
}
