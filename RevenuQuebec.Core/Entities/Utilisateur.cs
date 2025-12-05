using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevenuQuebec.SharedKernel;
using RevenuQuebec.SharedKernel.Interfaces;

namespace RevenuQuebec.Core.Entities
{
    public class Utilisateur : BaseEntity, IAggregateRoot
    {
        public string Courriel { get; private set; }
        public string MotDePasse { get; private set; }
        public string Nom { get; private set; }
        public string Prenom { get; private set; }
        public string NAS { get; private set; }
        public DateTime DateNaissance { get; private set; }
        public string Adresse { get; private set; }
        public string Telephone { get; private set; }

        public ICollection<Declaration> Declarations { get; private set; } = new List<Declaration>();

        private Utilisateur() { } // EF Core

        public Utilisateur(
            string courriel,
            string motDePasse,
            string nom,
            string prenom,
            string nas,
            DateTime dateNaissance,
            string adresse,
            string telephone)
        {
            // Ici on valide avant
            Valider(courriel, motDePasse);

            Courriel = courriel;
            MotDePasse = motDePasse;
            Nom = nom;
            Prenom = prenom;
            NAS = nas;
            DateNaissance = dateNaissance;
            Adresse = adresse;
            Telephone = telephone;
        }

        public bool Valider(string courriel, string motDePasse)
        {
            if (string.IsNullOrWhiteSpace(courriel))
                throw new ArgumentException("Le courriel ne peut pas être vide");

            if (string.IsNullOrWhiteSpace(motDePasse))
                throw new ArgumentException("Le mot de passe ne peut pas être vide");

            if (motDePasse.Length <= 8)
                throw new ArgumentException("Le mot de passe doit contenir plus de 8 caractères");

            return true;
        }

        public void AddDeclaration(Declaration declaration)
        {
            if (declaration != null)
                Declarations.Add(declaration);
        }

        public void RemoveDeclaration(Declaration declaration)
        {
            if (declaration != null && Declarations.Contains(declaration))
                Declarations.Remove(declaration);
        }

    }

}
