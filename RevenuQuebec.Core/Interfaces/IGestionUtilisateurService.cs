using RevenuQuebec.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Interfaces
{
    public interface IGestionUtilisateurService
    {
        Task AddUtilisateur(Utilisateur utilisateur);
        Task UpdateUtilisateur(Utilisateur utilisateur);
        Task DeleteUtilisateur(Utilisateur utilisateur);

        Task<Utilisateur> ConsulterUtilisateur(int id);
        Task<Utilisateur> ConsulterParCourriel(string courriel);

        Task<List<Utilisateur>> ObtenirTousLesUtilisateurs();
    }
}
