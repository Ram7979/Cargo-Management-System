using FluentValidation;
using MediatR;
using ValidationException = CMS.Shared.Exceptions.ValidationException;

namespace CMS.WarehouseService.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that runs all registered FluentValidation validators
/// for a request before it reaches its handler in the Warehouse Service.
///
/// <para><b>Workflow:</b></para>
/// <list type="number">
///   <item>If no validators are registered for the request type, the request is forwarded immediately.</item>
///   <item>All registered validators are executed against the request object.</item>
///   <item>All validation failures from all validators are collected.</item>
///   <item>If any failures exist, a <see cref="CMS.Shared.Exceptions.ValidationException"/> is thrown
///         with the full list of error messages — the handler is never reached.</item>
///   <item>If validation passes, the request is forwarded to the next behavior or the handler.</item>
/// </list>
///
/// <para><b>HTTP response:</b> <see cref="CMS.Shared.Middleware.GlobalExceptionMiddleware"/> maps
/// <see cref="CMS.Shared.Exceptions.ValidationException"/> to HTTP 400 Bad Request.</para>
/// </summary>
/// <typeparam name="TRequest">The MediatR request type being validated.</typeparam>
/// <typeparam name="TResponse">The response type returned by the handler.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of <see cref="ValidationBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="validators">
    /// All FluentValidation validators registered for <typeparamref name="TRequest"/>.
    /// Empty collection if no validators are registered for this request type.
    /// </param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Validates the incoming request. Throws if any rule fails; forwards if all pass.
    /// </summary>
    /// <param name="request">The incoming MediatR request to validate.</param>
    /// <param name="next">Delegate to invoke the next behavior or the actual handler.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The response produced by the downstream handler if validation passes.</returns>
    /// <exception cref="CMS.Shared.Exceptions.ValidationException">
    /// Thrown when one or more validation rules fail. Contains all error messages.
    /// </exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Fast path: no validators registered for this request type
        if (!_validators.Any())
            return await next(cancellationToken);

        // Run all validators and collect every failure
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // Throw with full error list — handler is never reached on failure
        if (failures.Count != 0)
        {
            var errors = failures.Select(f => f.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        return await next(cancellationToken);
    }
}
