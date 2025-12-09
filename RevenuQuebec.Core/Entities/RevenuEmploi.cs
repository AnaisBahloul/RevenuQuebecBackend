using RevenuQuebec.SharedKernel;
using RevenuQuebec.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenuQuebec.Core.Entities
{
    public class RevenuEmploi : BaseEntity, IAggregateRoot
    {
        public string Employeur { get; set; }
        public decimal Montant { get; set; }

        public int DeclarationId { get; set; } // <- Ajouté
        public Declaration Declaration { get; set; }

        public RevenuEmploi() { }

        public RevenuEmploi(string employeur, decimal montant)
        {
            Employeur = employeur;
            Montant = montant;
        }
    }
}
