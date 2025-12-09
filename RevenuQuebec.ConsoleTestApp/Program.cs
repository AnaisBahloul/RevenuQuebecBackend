using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using RevenuQuebec.Core.Services;
using RevenuQuebec.Infrastructure;
using RevenuQuebec.Infrastructure.Repositories;
using RevenuQuebec.SharedKernel.Interfaces;
//using RevenuQuebec.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevenuQuebec.ConsoleTestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Test1();
            await Test2();
            await Test3();
            await Test4();
        }

        // Test création et ajout d'entités
        static void Test1()
        {
            var context = new RevenuQuebecContext();

            // Création d'un utilisateur
            Utilisateur user = new Utilisateur(
                courriel: "anaisbahloul@example.com",
                motDePasse: "Motdepasse123!",
                nom: "Bahloul",
                prenom: "Anaïs",
                nas: "123456789",
                dateNaissance: new DateTime(2004, 3, 15),
                adresse: "45 Rue Principale, Moncton",
                telephone: "5061234567"
            );
            context.Add(user);

            // Création de déclarations
            Declaration declaration1 = new Declaration(
                adresse: "45 Rue Principale, Moncton",
                email: "anaisbahloul@example.com",
                telephone: "5061234567",
                citoyennete: "Canadienne"
            );

            // Ajouter RevenuEmploi et AutreRevenu
            RevenuEmploi emploi = new RevenuEmploi("Entreprise ABC", 25000);
            declaration1.AddRevenuEmploi(emploi);

            AutreRevenu autre = new AutreRevenu(AutreRevenu.TypeRevenu.Interets, 500);
            declaration1.AddAutreRevenu(autre);

            // Ajouter un justificatif
            Justificatif justificatif = new Justificatif("Relevé bancaire", "https://exemple.com/releve.pdf");
            declaration1.AddJustificatif(justificatif);

            // Création d'un avis
            Avis avis = new Avis(
                 type: AvisType.Personnalise,                  // Personnalise pour tester les notes
                 title: "Avis fiscal 2025",
                 year: "2025",
                 requiresAgentReview: true,
                 refNumber: "REF12345",
                 amount: "1500$",
                 taxableIncome: "40000$",
                 deductions: "5000$",
                 netTax: "2000$",
                 amountPayable: "1500$"
             );
            // Notes seulement si avis personnalisé
            if (avis.Type == AvisType.Personnalise)
            {
                avis.AddNote("Revenus d’intérêts initialement manquants");
                avis.AddNote("Données incohérentes");
            }
            declaration1.Avis = avis;

            // Ajouter historique de statut
            declaration1.ChangerEtat(DeclarationStatus.Recu, "Déclaration reçue");

            // Lier la déclaration à l'utilisateur
            user.AddDeclaration(declaration1);

            context.Add(declaration1);

            context.SaveChanges();

            Console.WriteLine("Test1: Entités créées et ajoutées au contexte.");
        }

        // Test récupération simple via repository
        static async Task Test2()
        {
            var context = new RevenuQuebecContext();
            IAsyncRepository<Utilisateur> userRepo = new EfRepository<Utilisateur>(context);

            // Récupérer un utilisateur par ID
            Utilisateur user = await userRepo.GetByIdAsync(1);
            if (user != null)
                Console.WriteLine($"Utilisateur trouvé: {user.Nom} {user.Prenom}");
            else
                Console.WriteLine("Utilisateur introuvable");
        }

        // Test récupération via filtre (ex: email)
        static async Task Test3()
        {
            var context = new RevenuQuebecContext();
            
            var repo = new UtilisateurRepository(context);

            var user = await repo.GetByCourrielAsync("anaisbahloul@example.com");

            if (user != null)
                Console.WriteLine($"Utilisateur filtré trouvé: {user.Nom} {user.Prenom}");
            else
                Console.WriteLine("Utilisateur introuvable par email");
        }

        // Test ajout via service
        static async Task Test4()
        {
            var context = new RevenuQuebecContext();
            IUtilisateurRepository userRepo = new UtilisateurRepository(context);
            IGestionUtilisateurService userService = new GestionUtilisateurService(userRepo);

            Utilisateur newUser = new Utilisateur(
                courriel: "nouvel.utilisateur@example.com",
                motDePasse: "Motdepasse456!",
                nom: "Dupont",
                prenom: "Marc",
                nas: "987654321",
                dateNaissance: new DateTime(1990, 5, 20),
                adresse: "78 Rue Secondaire, Montréal",
                telephone: "5149876543"
            );

            await userService.AddUtilisateur(newUser);

            Console.WriteLine("Test4: Nouvel utilisateur ajouté via le service.");
        }
    }
}
