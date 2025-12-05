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
    public class AutreRevenu : BaseEntity, IAggregateRoot
    {
        public TypeRevenu Type { get; set; }
        public decimal Montant { get; set; }

        public enum TypeRevenu
        {
            Emploi = 1,
            Interets = 2,
            Placement = 3,
            Autre = 99
        }

        // Constructeur par défaut
        public AutreRevenu() { }

        // Constructeur avec paramètres
        public AutreRevenu(TypeRevenu type, decimal montant)
        {
            Type = type;
            Montant = montant;
        }
    }

}
