using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Services
{
    public class AuthentificationService : IAuthentificationService
    {
        private readonly IUtilisateurRepository _utilisateurRepository;

        public AuthentificationService(IUtilisateurRepository utilisateurRepository)
        {
            _utilisateurRepository = utilisateurRepository;
        }

        public async Task<Utilisateur> ValiderAuthentification(string courriel, string motDePasse)
        {
            var utilisateur = await _utilisateurRepository.GetByCourrielAsync(courriel);

            if (utilisateur == null || utilisateur.MotDePasse != motDePasse)
            {
                return null; // Authentification échouée
            }

            return utilisateur;
        }
    }
}
