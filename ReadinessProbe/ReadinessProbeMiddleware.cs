using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ReadinessProbe
{
    public class ReadinessProbeMiddleware : IMiddleware
    {
        private static readonly EventId ExceptionEvent = new EventId(500, "ReadinessCheckException");
        private static readonly EventId NotReadyEvent = new EventId(503, "ReadinessCheckNegative");
        private static readonly EventId RequestAbortedEvent = new EventId(505, "ReadinessCheckRequestAborted");

        public ReadinessProbeMiddleware(ReadinessProbeOptions options, ILoggerFactory loggerFactory = null)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            loggerFactory = loggerFactory ?? new NullLoggerFactory();
            Logger = loggerFactory.CreateLogger(this.Options.LoggerCategory);
        }

        public ReadinessProbeOptions Options { get; }

        public ILogger Logger { get; }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

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
                                this.Logger.LogError(NotReadyEvent, NotReadyEvent.Name);
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
                                this.Logger.LogWarning(RequestAbortedEvent, canceledException, RequestAbortedEvent.Name);
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
                this.Logger.LogError(ExceptionEvent, e, ExceptionEvent.Name);
            }
        }
    }
}
