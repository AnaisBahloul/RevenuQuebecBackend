using RevenuQuebec.SharedKernel;
using RevenuQuebec.SharedKernel.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

        // État
        public bool EstBrouillon { get; set; } = false;

        public DateTime? DateSoumission { get; set; } = DateTime.UtcNow;

       
        [JsonIgnore]
        public Avis Avis { get; set; }
        public int? AvisId { get; set; }

        // User
        public int UtilisateurId { get; set; }

        [JsonIgnore]
        public Utilisateur Utilisateur { get; set; }

        // ÉTAT ACTUEL
        public DeclarationStatus Etat { get; set; }

        // Historique des statuts
        public List<Status> HistoriqueStatuts { get; set; } = new();

       
        public Declaration() { }
        public Declaration(string adresse, string email, string telephone, string citoyennete)
        {
            Adresse = adresse;
            Email = email;
            Telephone = telephone;
            Citoyennete = citoyennete;
        }



        // Changer l'état et ajouter à l'historique
        public void ChangerEtat(DeclarationStatus nouveauStatut, string message)
        {
            Etat = nouveauStatut;

            HistoriqueStatuts.Add(new Status
            {
                Etat = nouveauStatut,
                DateEvenement = DateTime.UtcNow,
                Message = message
            });
        }


        // Pour l'affichage dans le tableau
        public string GetEtatAffichage()
        {
            return Etat switch
            {
                DeclarationStatus.Brouillon => "Brouillon",
                DeclarationStatus.Cloturee => "Traitée",
                DeclarationStatus.EnRevisionParAgent => "En révision",
                DeclarationStatus.ValideeAutomatiquement => "Validée automatiquement",
                DeclarationStatus.Recu => "Reçue",
                _ => "En traitement"
            };
        }

        // Pour soumettre un brouillon
        public void SoumettreBrouillon()
        {
            if (!EstBrouillon) return;

            EstBrouillon = false;
            DateSoumission = DateTime.UtcNow;
            ChangerEtat(DeclarationStatus.Recu, "Déclaration soumise par l'utilisateur");
        }

        public void AddRevenuEmploi(RevenuEmploi revenu)
        {
            if (revenu != null)  
            {
                revenu.DeclarationId = Id;
                RevenusEmploi.Add(revenu);
            }
        }

        public void RemoveRevenuEmploi(RevenuEmploi revenu)
        {
            if (revenu != null && RevenusEmploi.Contains(revenu))
            {
                RevenusEmploi.Remove(revenu);
            }
        }

        public void AddAutreRevenu(AutreRevenu revenu)
        {
            if (revenu != null)  
            {
                revenu.DeclarationId = Id;
                AutresRevenus.Add(revenu);
            }
        }

        public void RemoveAutreRevenu(AutreRevenu revenu)
        {
            if (revenu != null && AutresRevenus.Contains(revenu))
            {
                AutresRevenus.Remove(revenu);
            }
        }

        public void AddJustificatif(Justificatif justificatif)
        {
            if (justificatif != null)  
            {
                justificatif.DeclarationId = Id;
                Fichiers.Add(justificatif);
            }
        }

        public void RemoveJustificatif(Justificatif justificatif)
        {
            if (justificatif != null && Fichiers.Contains(justificatif))
            {
                Fichiers.Remove(justificatif);
            }
        }

        public void AddStatus(Status statut)
        {
            if (statut != null) 
            {
                statut.DeclarationId = Id;
                HistoriqueStatuts.Add(statut);
            }
        }

        public void RemoveStatus(Status statut)
        {
            if (statut != null && HistoriqueStatuts.Contains(statut))
            {
                HistoriqueStatuts.Remove(statut);
            }
        }
    }
}
