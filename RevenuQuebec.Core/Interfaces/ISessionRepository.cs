using RevenuQuebec.Core.Entities;
using System.Threading.Tasks;
using RevenuQuebec.SharedKernel.Interfaces;

namespace RevenuQuebec.Core.Interfaces
{
    public interface ISessionRepository : IAsyncRepository<Session>
    {
        Task AddSessionAsync(Session session);
        Task UpdateSessionAsync(Session session);
        Task DeleteSessionAsync(int sessionId);
        Task<Session> GetSessionByIdAsync(int sessionId);

        void AddSession(Session session);
        void UpdateSession(Session session);
        void DeleteSession(int sessionId);
        Session GetSessionById(int sessionId);
    }
}
