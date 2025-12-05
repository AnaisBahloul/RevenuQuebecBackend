using Microsoft.EntityFrameworkCore;
using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;

namespace RevenuQuebec.Infrastructure.Repositories
{
    public class UtilisateurRepository : EfRepository<Utilisateur>, IUtilisateurRepository
    {
        private readonly RevenuQuebecContext _dbContext;

        public UtilisateurRepository(RevenuQuebecContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Utilisateur> GetByCourrielAsync(string courriel)
        {
            return await _dbContext.Utilisateurs
                .FirstOrDefaultAsync(u => u.Courriel == courriel);
        }

        public Utilisateur GetByCourriel(string courriel)
        {
            return _dbContext.Utilisateurs
                .FirstOrDefault(u => u.Courriel == courriel);
        }
    }
}
