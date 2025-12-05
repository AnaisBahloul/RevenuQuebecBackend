using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Services
{
    public class GestionAvisService : IGestionAvisService
    {
        private readonly IAvisRepository _avisRepository;

        public GestionAvisService(IAvisRepository avisRepository)
        {
            _avisRepository = avisRepository;
        }

        // Consulter un avis par Id
        public async Task<Avis> ConsulterAvis(int id)
        {
            return await _avisRepository.GetByIdAsync(id);
        }

        // Consulter l'avis d'une déclaration
        public async Task<Avis> ConsulterAvisParDeclaration(int declarationId)
        {
            return await _avisRepository.GetByDeclarationIdAsync(declarationId);
        }

        // Lister tous les avis
        public async Task<List<Avis>> ListerAvis()
        {
            return await _avisRepository.ListAllAsync();
        }
    }
}
