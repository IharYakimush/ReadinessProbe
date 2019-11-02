using System.Threading;
using System.Threading.Tasks;

namespace ReadinessProbe.Tests
{
    public class CheckFalse : IReadinessCheck
    {
        public Task<bool> Check(CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }
    }
}