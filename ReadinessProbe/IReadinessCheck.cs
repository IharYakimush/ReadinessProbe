using System.Threading;
using System.Threading.Tasks;

namespace ReadinessProbe
{
    public interface IReadinessCheck
    {
        Task<bool> Check(CancellationToken cancellationToken);
    }
}