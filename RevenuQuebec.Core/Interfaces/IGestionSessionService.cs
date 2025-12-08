using RevenuQuebec.Core.Entities;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Interfaces
{
    public interface IGestionSessionService
    {
        Task<Session> GetCurrentSessionAsync(int sessionId);
    }
}
