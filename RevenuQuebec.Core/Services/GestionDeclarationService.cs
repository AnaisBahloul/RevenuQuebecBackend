using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Services
{
    public class GestionDeclarationService : IGestionDeclarationService
    {
        private readonly IDeclarationRepository _declarationRepository;

        public GestionDeclarationService(IDeclarationRepository declarationRepository)
        {
            _declarationRepository = declarationRepository;
        }

        public async Task<bool> AddDeclaration(Declaration declaration)
        {
            await _declarationRepository.AddAsync(declaration);
            return true;
        }

        public async Task UpdateDeclarationAsync(Declaration declaration)
        {
            await _declarationRepository.UpdateAsync(declaration);
        }

        public async Task DeleteDeclaration(Declaration declaration)
        {
            await _declarationRepository.DeleteAsync(declaration);
        }

        public async Task<Declaration> ConsulterDeclaration(int id)
        {
            return await _declarationRepository.GetByIdAsync(id);
        }
        public async Task<IReadOnlyList<Declaration>> ListerDeclarations()
        {
            return await _declarationRepository.ListAllAsync();
        }

        public async Task<List<Declaration>> ListerDeclarationsParUtilisateur(int utilisateurId)
        {
            return await _declarationRepository.GetDeclarationsByUserAsync(utilisateurId);
        }


    }
}
