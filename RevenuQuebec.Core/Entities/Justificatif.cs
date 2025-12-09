using RevenuQuebec.SharedKernel;
using RevenuQuebec.SharedKernel.Interfaces;

namespace RevenuQuebec.Core.Entities;
public class Justificatif : BaseEntity, IAggregateRoot
{
    public string Nom { get; set; }
    public string Url { get; set; }
    public int DeclarationId { get; set; } 
    public Declaration Declaration { get; set; }

    public Justificatif() { }

    public Justificatif(string nom, string url)
    {
        Nom = nom;
        Url = url;
    }
}
