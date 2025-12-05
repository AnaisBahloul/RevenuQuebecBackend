using Microsoft.EntityFrameworkCore;
using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using System.Collections.Generic;
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
                .Include(a => a.Declaration) // inclut la déclaration liée
                .ToListAsync();
        }

        // Récupérer l'avis d'une déclaration
        public async Task<Avis> GetByDeclarationIdAsync(int declarationId)
        {
            return await _dbContext.Avis
                .Include(a => a.Declaration)
                .FirstOrDefaultAsync(a => a.DeclarationId == declarationId);
        }

        // Récupérer un avis par Id
        public async Task<Avis> GetByIdAsync(int id)
        {
            return await _dbContext.Avis
                .Include(a => a.Declaration)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
