using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace DocumentManagerApi.Filters
{
    public class LogActionFilter : IActionFilter
    {
        private readonly ILogger<LogActionFilter> _logger;
        private Stopwatch? _sw;

        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _sw = Stopwatch.StartNew();
            _logger.LogInformation("Starting action {Action}", context.ActionDescriptor.DisplayName);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _sw?.Stop();
            _logger.LogInformation("Finished action {Action} in {Elapsed}ms", context.ActionDescriptor.DisplayName, _sw?.ElapsedMilliseconds);
        }
    }
}
