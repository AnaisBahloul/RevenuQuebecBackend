using RevenuQuebec.SharedKernel;
using RevenuQuebec.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RevenuQuebec.Core.Entities.Declaration;

namespace RevenuQuebec.Core.Entities
{
    public class Status : BaseEntity, IAggregateRoot
    {
        public int DeclarationId { get; set; }
        public Declaration Declaration { get; set; }

        // Étape atteinte
        public DeclarationStatus Etat { get; set; }

        // Timestamp de l'événement
        public DateTime DateEvenement { get; set; } = DateTime.UtcNow;

        // Message facultatif (ex: "Votre dossier est en analyse")
        public string Message { get; set; }

        public Status() { }

        public Status(DeclarationStatus etat, string message = null, DateTime? dateEvenement = null)
        {
            Etat = etat;
            Message = message;
            DateEvenement = dateEvenement ?? DateTime.UtcNow; // si null, prend maintenant
        }

    }

    public enum DeclarationStatus
    {
        Recu,
        ValideeAutomatiquement,
        EnRevisionParAgent,
        Cloturee
    }


}
