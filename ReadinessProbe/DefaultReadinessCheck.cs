using System.Threading;
using System.Threading.Tasks;

namespace ReadinessProbe
{
    public class DefaultReadinessCheck : IReadinessCheck
    {
        private static readonly Task<bool> Result = Task.FromResult(true);

        public static DefaultReadinessCheck Instance { get; } = new DefaultReadinessCheck();

        public Task<bool> Check(CancellationToken cancellationToken)
        {
            return Result;
        }
    }
}