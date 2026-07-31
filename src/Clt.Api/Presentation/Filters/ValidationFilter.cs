using FluentValidation;

namespace Clt.Api.Presentation.Filters;

public sealed class ValidationFilter<T>(IValidator<T> validator) : IEndpointFilter
    where T : class
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var request = context.Arguments.OfType<T>().FirstOrDefault();

        if (request is null)
        {
            return Results.BadRequest();
        }

        var validationResult = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);

        if (validationResult.IsValid)
        {
            return await next(context);
        }

        var errors = validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).Distinct().ToArray());

        return Results.ValidationProblem(errors);
    }
}
