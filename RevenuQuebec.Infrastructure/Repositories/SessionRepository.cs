using Microsoft.EntityFrameworkCore;
using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;

namespace RevenuQuebec.Infrastructure.Repositories
{
    public class SessionRepository : EfRepository<Session>, ISessionRepository
    {
        public SessionRepository(RevenuQuebecContext context)
            : base(context)
        { }

        public async Task AddSessionAsync(Session session)
        {
            if (session.Id == 0)
            {
                session.Connecter();
                await _context.Sessions.AddAsync(session);
            }
            else
            {
                var existing = await _context.Sessions.FindAsync(session.Id);

                if (existing != null)
                {
                    existing.Connecter();
                    _context.Sessions.Update(existing);
                }
                else
                {
                    session.Connecter();
                    await _context.Sessions.AddAsync(session);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteSessionAsync(int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);

            if (session != null)
            {
                _context.Sessions.Remove(session);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Session?> GetSessionByIdAsync(int sessionId)
        {
            return await _context.Sessions
                .FirstOrDefaultAsync(s => s.Id == sessionId);
        }

        public async Task UpdateSessionAsync(Session session)
        {
            var existing = await _context.Sessions.FindAsync(session.Id);

            if (existing != null)
            {
                existing.DateDeconnexion = session.DateDeconnexion;
                _context.Sessions.Update(existing);
                await _context.SaveChangesAsync();
            }
        }

        // ==== SYNCHRONE ====

        public void AddSession(Session session)
        {
            session.Connecter();
            _context.Sessions.Add(session);
            _context.SaveChanges();
        }

        public void DeleteSession(int sessionId)
        {
            var session = _context.Sessions.Find(sessionId);

            if (session != null)
            {
                _context.Sessions.Remove(session);
                _context.SaveChanges();
            }
        }

        public Session? GetSessionById(int sessionId)
        {
            return _context.Sessions
                .FirstOrDefault(s => s.Id == sessionId);
        }

        public void UpdateSession(Session session)
        {
            var existing = _context.Sessions.Find(session.Id);

            if (existing != null)
            {
                existing.DateDeconnexion = session.DateDeconnexion;
                _context.Sessions.Update(existing);
                _context.SaveChanges();
            }
        }
    }
}
