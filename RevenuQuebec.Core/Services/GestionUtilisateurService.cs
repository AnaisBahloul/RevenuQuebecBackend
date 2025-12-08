using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Services
{
    public class GestionUtilisateurService : IGestionUtilisateurService
    {
        private readonly IUtilisateurRepository _utilisateurRepository;

        public GestionUtilisateurService(IUtilisateurRepository utilisateurRepository)
        {
            _utilisateurRepository = utilisateurRepository;
        }

        public async Task AddUtilisateur(Utilisateur utilisateur)
        {
            await _utilisateurRepository.AddAsync(utilisateur);
        }

        public async Task UpdateUtilisateur(Utilisateur utilisateur)
        {
            Console.WriteLine("🟣 [Service] UpdateUtilisateur()");
            Console.WriteLine("🟣 Nouveaux champs: "
                + $"Courriel={utilisateur.Courriel}, Adresse={utilisateur.Adresse}, Tel={utilisateur.Telephone}");
            await _utilisateurRepository.UpdateAsync(utilisateur);
        }

        public async Task DeleteUtilisateur(Utilisateur utilisateur)
        {
            await _utilisateurRepository.DeleteAsync(utilisateur);
        }

        public async Task<Utilisateur> ConsulterUtilisateur(int id)
        {
            return await _utilisateurRepository.GetByIdAsync(id);
        }

        public async Task<Utilisateur> ConsulterParCourriel(string courriel)
        {
            return await _utilisateurRepository.GetByCourrielAsync(courriel);
        }

        public async Task<List<Utilisateur>> ObtenirTousLesUtilisateurs()
        {
            var utilisateurs = await _utilisateurRepository.ListAllAsync();
            return utilisateurs.ToList();
        }
    }
}
