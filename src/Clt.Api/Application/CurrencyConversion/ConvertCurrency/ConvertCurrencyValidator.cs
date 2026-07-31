using FluentValidation;

namespace Clt.Api.Application.CurrencyConversion.ConvertCurrency;

public sealed class ConvertCurrencyValidator : AbstractValidator<ConvertCurrencyCommand>
{
    public ConvertCurrencyValidator()
    {
        RuleFor(command => command.FromCurrencyCode)
            .NotEmpty()
            .Length(3)
            .Matches("^[A-Za-z]{3}$");

        RuleFor(command => command.ToCurrencyCode)
            .NotEmpty()
            .Length(3)
            .Matches("^[A-Za-z]{3}$");

        RuleFor(command => command.Amount)
            .GreaterThan(0);
    }
}
