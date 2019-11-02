using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace ReadinessProbe.Sample
{
    public class LongRunningAction : BackgroundService
    {
        public LongRunningAction(ReadinessIndicatorFor<LongRunningAction> readinessIndicator)
        {
            ReadinessIndicator = readinessIndicator ?? throw new ArgumentNullException(nameof(readinessIndicator));
        }

        public ReadinessIndicatorFor<LongRunningAction> ReadinessIndicator { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int l = 60;

            for (int i = 0; i < l; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                Console.WriteLine($"{i + 1}/{l}");
            }

            this.ReadinessIndicator.Ready();
        }
    }
}