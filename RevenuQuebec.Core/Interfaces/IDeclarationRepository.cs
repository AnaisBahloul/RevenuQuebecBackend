using RevenuQuebec.Core.Entities;
using RevenuQuebec.SharedKernel.Interfaces;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Interfaces
{
    public interface IDeclarationRepository
     : IAsyncRepository<Declaration>
    {
        Task<Declaration> GetByIdCompletAsync(int id);
        Declaration GetByIdComplet(int id);
        Task<List<Declaration>> GetDeclarationsByUserAsync(int utilisateurId);

    }

}
