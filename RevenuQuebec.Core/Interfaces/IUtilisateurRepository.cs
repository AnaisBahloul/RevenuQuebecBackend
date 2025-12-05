using RevenuQuebec.Core.Entities;
using RevenuQuebec.SharedKernel.Interfaces;

namespace RevenuQuebec.Core.Interfaces
{
    public interface IUtilisateurRepository : IAsyncRepository<Utilisateur>
    {
        Task<Utilisateur> GetByCourrielAsync(string courriel);
        Utilisateur GetByCourriel(string courriel);
    }
}
