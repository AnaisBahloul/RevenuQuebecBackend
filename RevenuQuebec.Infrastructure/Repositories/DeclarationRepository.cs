using Microsoft.EntityFrameworkCore;
using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;

namespace RevenuQuebec.Infrastructure.Repositories
{
    public class DeclarationRepository: EfRepository<Declaration>, IDeclarationRepository
    {
        private readonly RevenuQuebecContext _dbContext;

        public DeclarationRepository(RevenuQuebecContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Declaration> GetByIdCompletAsync(int id)
        {
            return await _dbContext.Declarations
                .Include(d => d.RevenusEmploi)
                .Include(d => d.AutresRevenus)
                .Include(d => d.Fichiers)
                .Include(d => d.HistoriqueStatuts)
                .Include(d => d.Utilisateur)
                .Include(d => d.Avis)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public Declaration GetByIdComplet(int id)
        {
            return _dbContext.Declarations
                .Include(d => d.RevenusEmploi)
                .Include(d => d.AutresRevenus)
                .Include(d => d.Fichiers)
                .Include(d => d.HistoriqueStatuts)
                .Include(d => d.Utilisateur)
                .Include(d => d.Avis)
                .FirstOrDefault(d => d.Id == id);
        }
    }
}
