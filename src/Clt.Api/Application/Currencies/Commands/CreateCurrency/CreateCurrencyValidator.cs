using FluentValidation;

namespace Clt.Api.Application.Currencies.Commands.CreateCurrency;

public sealed class CreateCurrencyValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyValidator()
    {
        RuleFor(command => command.Code)
            .NotEmpty()
            .Length(3)
            .Matches("^[A-Za-z]{3}$");

        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(80);

        RuleFor(command => command.RateToBase)
            .GreaterThan(0);
    }
}
