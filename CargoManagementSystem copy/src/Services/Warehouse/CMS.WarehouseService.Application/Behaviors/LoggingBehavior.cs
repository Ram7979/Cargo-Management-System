using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.WarehouseService.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that measures and logs the execution time of every
/// request passing through the Warehouse Service's request pipeline.
///
/// <para><b>Workflow:</b></para>
/// <list type="number">
///   <item>A Stopwatch is started before the request is forwarded.</item>
///   <item>An entry log is written: "Handling {RequestName} at {StartTime}".</item>
///   <item>The request is forwarded to the next behavior or the actual handler.</item>
///   <item>After the handler returns, the stopwatch is stopped.</item>
///   <item>An exit log is written: "Handled {RequestName} in {ElapsedMs}ms".</item>
/// </list>
///
/// <para><b>Performance monitoring:</b> Elapsed time is logged in milliseconds.
/// Slow operations (e.g., bin allocation, cargo receipt) can be identified in Seq
/// by filtering on the <c>ElapsedMs</c> property.</para>
/// </summary>
/// <typeparam name="TRequest">The MediatR request type (command or query).</typeparam>
/// <typeparam name="TResponse">The response type returned by the handler.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="LoggingBehavior{TRequest, TResponse}"/>.
    /// </summary>
    /// <param name="logger">Serilog-backed logger for structured performance entries.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Wraps the handler execution with start/end log entries and elapsed time measurement.
    /// </summary>
    /// <param name="request">The incoming MediatR request.</param>
    /// <param name="next">Delegate to invoke the next behavior or the actual handler.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The response produced by the downstream handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var startTime = DateTime.UtcNow;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Handling {RequestName} at {StartTime}", requestName, startTime);

        var response = await next(cancellationToken);

        stopwatch.Stop();
        _logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms", requestName, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
