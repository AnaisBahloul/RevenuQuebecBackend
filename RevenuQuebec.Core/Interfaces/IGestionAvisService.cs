using RevenuQuebec.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Interfaces
{
    public interface IGestionAvisService
    {
        Task<Avis> ConsulterAvis(int id);
        Task<Avis> ConsulterAvisParDeclaration(int declarationId);
        Task<List<Avis>> ListerAvis();
        Task<List<Avis>> ListerAvisParUtilisateur(int userId);

    }
}
