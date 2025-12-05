using RevenuQuebec.SharedKernel;
using RevenuQuebec.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;

namespace RevenuQuebec.Core.Entities
{
    public class Declaration : BaseEntity, IAggregateRoot
    {
        // Coordonnées
        public string Adresse { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Citoyennete { get; set; }

        // Revenus
        public List<RevenuEmploi> RevenusEmploi { get; set; } = new();
        public List<AutreRevenu> AutresRevenus { get; set; } = new();

        // Fichiers
        public List<Justificatif> Fichiers { get; set; } = new();

        // Validation
        public bool ConfirmationExactitude { get; set; }

        // Tracking
        public DateTime DateSoumission { get; set; } = DateTime.UtcNow;
        public bool EstBrouillon { get; set; } = false;

        public Avis Avis {  get; set; }
        // User
        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }

        //  ÉTAT ACTUEL (pour tableau historique)
        public DeclarationStatus Etat { get; set; }

        // TIMELINE COMPLETE (chaque item = une étape)
        public List<Status> HistoriqueStatuts { get; set; } = new();

        // --- Domain logic ---
        public void ChangerEtat(DeclarationStatus nouveauStatut, string message)
        {
            Etat = nouveauStatut;

            HistoriqueStatuts.Add(new Status
            {
                DeclarationId = Id,
                Etat = nouveauStatut,
                DateEvenement = DateTime.UtcNow,
                Message = message
            });
        }

        // Constructeur par défaut
        public Declaration() { }

        // Constructeur avec paramètres principaux
        public Declaration(string adresse, string email, string telephone, string citoyennete)
        {
            Adresse = adresse;
            Email = email;
            Telephone = telephone;
            Citoyennete = citoyennete;
          
        }

        //  Business rule simple pour UI tableau
        public string EtatPourTableau()
        {
            return Etat == DeclarationStatus.Cloturee
                ? "Traitée"
                : "En traitement";
        }


        public void AddRevenuEmploi(RevenuEmploi revenu)
        {
            if (revenu != null)
                RevenusEmploi.Add(revenu);
        }

        public void RemoveRevenuEmploi(RevenuEmploi revenu)
        {
            if (revenu != null && RevenusEmploi.Contains(revenu))
                RevenusEmploi.Remove(revenu);
        }

        public void AddAutreRevenu(AutreRevenu revenu)
        {
            if (revenu != null)
                AutresRevenus.Add(revenu);
        }

        public void RemoveAutreRevenu(AutreRevenu revenu)
        {
            if (revenu != null && AutresRevenus.Contains(revenu))
                AutresRevenus.Remove(revenu);
        }

        public void AddJustificatif(Justificatif justificatif)
        {
            if (justificatif != null)
                Fichiers.Add(justificatif);
        }

        public void RemoveJustificatif(Justificatif justificatif)
        {
            if (justificatif != null && Fichiers.Contains(justificatif))
                Fichiers.Remove(justificatif);
        }

        public void AddStatus(Status statut)
        {
            if (statut != null)
                HistoriqueStatuts.Add(statut);
        }

        public void RemoveStatus(Status statut)
        {
            if (statut != null && HistoriqueStatuts.Contains(statut))
                HistoriqueStatuts.Remove(statut);
        }



    }
}
