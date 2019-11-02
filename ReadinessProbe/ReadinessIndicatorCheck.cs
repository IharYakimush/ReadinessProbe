using System.Threading;
using System.Threading.Tasks;

namespace ReadinessProbe
{
    public class ReadinessIndicatorCheck : IReadinessCheck
    {
        private readonly ReadinessIndicator readinessIndicator;

        public ReadinessIndicatorCheck(ReadinessIndicator readinessIndicator)
        {
            this.readinessIndicator = readinessIndicator ?? throw new System.ArgumentNullException(nameof(readinessIndicator));
        }

        public Task<bool> Check(CancellationToken cancellationToken)
        {
            return Task.FromResult(this.readinessIndicator.IsReady);
        }
    }
}