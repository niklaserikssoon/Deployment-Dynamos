namespace User_API.Filters
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Diagnostics;

    /// <summary>
    /// Performance filter, automatic = no need to manually check every action.
    /// Measures and logs the execution time of every controller action.
    /// Starts a stopwatch before the action executes and stops it after, logging the elapsed milliseconds.
    /// For identifying slow endpoints.
    /// </summary>
    public class PerformanceFilter : ActionFilterAttribute
    {
        private readonly ILogger<PerformanceFilter> _logger;
        private Stopwatch _stopwatch = new();

        public PerformanceFilter(ILogger<PerformanceFilter> logger)
        {
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            _stopwatch.Stop();
            _logger.LogInformation(
                "{Controller}.{Action} executed in {ElapsedMs}ms",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"],
                _stopwatch.ElapsedMilliseconds
            );
        }
    }
}
