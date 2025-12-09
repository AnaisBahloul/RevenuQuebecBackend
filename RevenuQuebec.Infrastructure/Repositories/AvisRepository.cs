using Microsoft.EntityFrameworkCore;
using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevenuQuebec.Infrastructure.Repositories
{
    public class AvisRepository : EfRepository<Avis>, IAvisRepository
    {
        private readonly RevenuQuebecContext _dbContext;

        public AvisRepository(RevenuQuebecContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        // Lister tous les avis
        public async Task<List<Avis>> ListAllAsync()
        {
            return await _dbContext.Avis
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.Utilisateur)
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.RevenusEmploi)
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.AutresRevenus)
                .ToListAsync();
        }

        // Récupérer un avis par Id
        public async Task<Avis> GetByIdAsync(int id)
        {
            return await _dbContext.Avis
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.Utilisateur)
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.RevenusEmploi)
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.AutresRevenus)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // Récupérer les avis d'un utilisateur
        public async Task<List<Avis>> GetByUserIdAsync(int userId)
        {
            return await _dbContext.Avis
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.Utilisateur)
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.RevenusEmploi)
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.AutresRevenus)
                .Where(a => a.Declaration.UtilisateurId == userId)
                .ToListAsync();
        }

        // Récupérer l'avis d'une déclaration
        public async Task<Avis> GetByDeclarationIdAsync(int declarationId)
        {
            return await _dbContext.Avis
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.Utilisateur)
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.RevenusEmploi)
                .Include(a => a.Declaration)
                    .ThenInclude(d => d.AutresRevenus)
                .FirstOrDefaultAsync(a => a.DeclarationId == declarationId);
        }
    }
}
