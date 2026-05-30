using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace WLR.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer = new();
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Restart();
        var response = await next();
        _timer.Stop();

        if (_timer.ElapsedMilliseconds > 500)
            _logger.LogWarning("WLR Long Running Request: {RequestName} ({Elapsed}ms) {@Request}",
                typeof(TRequest).Name, _timer.ElapsedMilliseconds, request);

        return response;
    }
}
