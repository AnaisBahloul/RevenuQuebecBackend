using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Services
{
    public class GestionSessionService : IGestionSessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public GestionSessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<Session> GetCurrentSessionAsync(int sessionId)
        {
            return await _sessionRepository.GetSessionByIdAsync(sessionId);
        }
    }
}
