using RevenuQuebec.Core.Entities;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Interfaces
{
    public interface IAuthentificationService
    {
        Task<Utilisateur> ValiderAuthentification(string courriel, string motDePasse);
    }
}
