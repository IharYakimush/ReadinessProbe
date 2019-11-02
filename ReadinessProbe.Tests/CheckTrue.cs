using System.Threading;
using System.Threading.Tasks;

namespace ReadinessProbe.Tests
{
    public class CheckTrue : IReadinessCheck
    {
        public Task<bool> Check(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }
    }
}