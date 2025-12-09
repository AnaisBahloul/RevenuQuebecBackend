using RevenuQuebec.Core.Entities;
using RevenuQuebec.SharedKernel.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Interfaces
{
    public interface IAvisRepository : IAsyncRepository<Avis>
    {
        Task<List<Avis>> ListAllAsync();
        Task<Avis> GetByDeclarationIdAsync(int declarationId);
        Task<Avis> GetByIdAsync(int id);
        Task<List<Avis>> GetByUserIdAsync(int userId);

    }
}
