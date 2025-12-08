using RevenuQuebec.SharedKernel;
using RevenuQuebec.SharedKernel.Interfaces;
using System;

namespace RevenuQuebec.Core.Entities
{
    public class Session : BaseEntity, IAggregateRoot
    {
        public DateTimeOffset DateConnexion { get; set; }
        public DateTimeOffset DateDeconnexion { get; set; }

        public int UtilisateurId { get; set; }
        public Utilisateur Utilisateur { get; set; }

        public Session() { }

        public void Connecter()
        {
            DateConnexion = DateTimeOffset.Now;
            Console.WriteLine($"Session commencée à {DateConnexion}");
        }

        public void Deconnexion()
        {
            DateDeconnexion = DateTimeOffset.Now;
            Console.WriteLine($"Session terminée à {DateDeconnexion}");
        }

        public void AfficherFormulaireAuthentification()
        {
            Console.WriteLine("Affichage du formulaire d'authentification...");
        }

        public bool ValiderAuthentification()
        {
            if (DateConnexion != null && DateDeconnexion == null)
            {
                Console.WriteLine("Authentification réussie. Session active.");
                return true;
            }
            else if (DateDeconnexion != null)
            {
                Console.WriteLine("La session est terminée. Veuillez vous reconnecter.");
                return false;
            }

            Console.WriteLine("Session non valide. Veuillez vous connecter.");
            return false;
        }

        public void AfficherMessageErreur(string message)
        {
            Console.WriteLine($"Erreur : {message}");
        }

        
    }
}
