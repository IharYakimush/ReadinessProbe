using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using System.Threading.Tasks;

namespace ReadinessProbe.Tests
{
    public class ReadinessProbeTests
    {
        [Fact]
        public void ReadinessProbeIndicator()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddReadinessIndicatorFor<ReadinessProbeTests>();

            // Unable to register indicator for same type twice
            Assert.Throws<InvalidOperationException>(() => services.AddReadinessIndicatorFor<ReadinessProbeTests>());

            ServiceProvider provider = services.BuildServiceProvider();

            ReadinessIndicatorFor<ReadinessProbeTests> indicator =
                provider.GetRequiredService<ReadinessIndicatorFor<ReadinessProbeTests>>();

            IReadinessCheck check = provider.GetRequiredService<IReadinessCheck>();

            Assert.False(indicator.IsReady);
            Assert.False(check.Check(CancellationToken.None).Result);
            
            indicator.Ready();

            Assert.True(indicator.IsReady);
            Assert.True(check.Check(CancellationToken.None).Result);

            // Unable to call Ready method twice
            Assert.Throws<InvalidOperationException>(() => indicator.Ready());
        }

        [Fact]
        public async Task MiddlewareDefault()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddReadinessProbe();
            services.AddReadinessProbe();

            ServiceProvider provider = services.BuildServiceProvider();

            ReadinessProbeMiddleware middleware = provider.GetRequiredService<ReadinessProbeMiddleware>();

            Assert.NotNull(middleware.Options);
            Assert.NotNull(middleware.Logger);
            Assert.IsType<NullLogger>(middleware.Logger);
                        
            await Assert.ThrowsAsync<ArgumentNullException>(() => middleware.InvokeAsync(null, null));

            DefaultHttpContext context = new DefaultHttpContext();
            context.RequestServices = provider;

            context.Response.StatusCode = 0;

            middleware.InvokeAsync(context, null).Wait();

            Assert.Equal(200, context.Response.StatusCode);
        }
    }
}
