using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace ReadinessProbe
{
    public class ReadinessProbeMiddleware : IMiddleware
    {
        private static readonly EventId ExceptionEvent = new EventId(500, "ReadinessCheckException");
        private static readonly EventId NotReadyEvent = new EventId(503, "ReadinessCheckNegative");
        private static readonly EventId RequestAbortedEvent = new EventId(505, "ReadinessCheckRequestAborted");

        public ReadinessProbeMiddleware(IOptions<ReadinessProbeOptions> options, ILoggerFactory loggerFactory = null)
        {
            Options = options.Value;
            loggerFactory = loggerFactory ?? new NullLoggerFactory();
            logger = loggerFactory.CreateLogger(this.Options.LoggerCategory);
        }

        public ReadinessProbeOptions Options { get; }

        public ILogger logger;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Response.HasStarted)
            {
                foreach (var check in context.RequestServices.GetServices<IReadinessCheck>())
                {
                    try
                    {
                        context.RequestAborted.ThrowIfCancellationRequested();

                        bool result = await check.Check(context.RequestAborted);

                        if (!result)
                        {
                            context.Response.StatusCode = (int) this.Options.NotReadyCode;

                            if (this.Options.LogNotReady)
                            {
                                this.logger.LogError(NotReadyEvent, NotReadyEvent.Name);
                            }

                            return;
                        }
                    }
                    catch (OperationCanceledException canceledException)
                    {
                        if (context.RequestAborted.IsCancellationRequested)
                        {
                            if (this.Options.LogRequestAborted)
                            {
                                this.logger.LogWarning(RequestAbortedEvent, canceledException, RequestAbortedEvent.Name);
                            }

                            return;
                        }

                        // OperationCanceledException from unknown source treated as generic exception
                        HandleException(context, canceledException);

                        return;
                    }
                    catch (Exception e)
                    {
                        HandleException(context, e);

                        return;
                    }   
                }

                context.Response.StatusCode = (int)this.Options.ReadyCode;
            }
        }

        private void HandleException(HttpContext context, Exception e)
        {
            context.Response.StatusCode = (int) this.Options.ErrorCode;

            if (this.Options.LogError)
            {
                this.logger.LogError(ExceptionEvent, e, ExceptionEvent.Name);
            }
        }
    }
}
