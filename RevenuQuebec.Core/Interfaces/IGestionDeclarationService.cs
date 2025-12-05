using RevenuQuebec.Core.Entities;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Interfaces
{
    public interface IGestionDeclarationService
    {
        Task<bool> AddDeclaration(Declaration declaration);
        Task UpdateDeclarationAsync(Declaration declaration);
        Task DeleteDeclaration(Declaration declaration);
        Task<Declaration> ConsulterDeclaration(int id);
    }
}
