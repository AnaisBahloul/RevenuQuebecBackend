using Microsoft.AspNetCore.Mvc;
using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using RevenuQuebec.Core.Services;
using RevenuQuebec.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace RevenuQuebec.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthentificationController : ControllerBase
    {
        private readonly IAuthentificationService _authService;
        private readonly IGestionUtilisateurService _userService;
        private readonly ISessionRepository _sessionRepository;

        public AuthentificationController(
            IAuthentificationService authService,
            IGestionUtilisateurService userService,
            ISessionRepository sessionRepository)
        {
            _authService = authService;
            _userService = userService;
            _sessionRepository = sessionRepository;
        }

        // 1. LOGIN → Utilise ton service EXISTANT
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Appelle ta méthode EXISTANTE
                var utilisateur = await _authService.ValiderAuthentification(
                    request.Courriel,
                    request.MotDePasse
                );

                if (utilisateur == null)
                {
                    return Unauthorized(new { message = "Email ou mot de passe incorrect" });
                }

                var session = new Session();
                session.Connecter();

                session.UtilisateurId = utilisateur.Id;

                await _sessionRepository.AddSessionAsync(session);


                // Réponse au frontend
                return Ok(new
                {
                    message = "Connexion réussie",
                    sessionId = session.Id,
                    utilisateurId = utilisateur.Id,
                    utilisateur = new
                    {
                        id = utilisateur.Id,
                        nom = utilisateur.Nom,
                        prenom = utilisateur.Prenom,
                        email = utilisateur.Courriel,
                        nas = utilisateur.NAS,
                        dob = utilisateur.DateNaissance.ToString("yyyy-MM-dd"),
                        adresse = utilisateur.Adresse,
                        telephone = utilisateur.Telephone
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. REGISTER → Utilise ton service EXISTANT
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Vérifier si l'utilisateur existe déjà
                var existing = await _userService.ConsulterParCourriel(request.Courriel);
                if (existing != null)
                {
                    return BadRequest(new { message = "Cet email est déjà utilisé" });
                }

                // Créer l'utilisateur COMME DANS TON ConsoleTestApp
                var nouvelUtilisateur = new Utilisateur(
                    courriel: request.Courriel,
                    motDePasse: request.MotDePasse, // En clair, comme tu veux
                    nom: request.Nom,
                    prenom: request.Prenom,
                    nas: request.NAS,
                    dateNaissance: request.DateNaissance,
                    adresse: request.Adresse ?? "",
                    telephone: request.Telephone ?? ""
                );

                // Utiliser ton service EXISTANT GestionUtilisateurService
                await _userService.AddUtilisateur(nouvelUtilisateur);

                return Ok(new
                {
                    message = "Compte créé avec succès",
                    utilisateur = new
                    {
                        id = nouvelUtilisateur.Id,
                        nom = nouvelUtilisateur.Nom,
                        prenom = nouvelUtilisateur.Prenom,
                        email = nouvelUtilisateur.Courriel
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var session = await _sessionRepository.GetSessionByIdAsync(request.SessionId);
            if (session != null)
            {
                session.Deconnexion();
                await _sessionRepository.UpdateSessionAsync(session);
            }
            return Ok(new { message = "Déconnexion réussie" });
        }



        public class LogoutRequest
{
    public int SessionId { get; set; }
}

    }


    // Classes simples pour recevoir les données du frontend
    public class LoginRequest
    {
        public string Courriel { get; set; }
        public string MotDePasse { get; set; }
    }

    public class RegisterRequest
    {
        public string Courriel { get; set; }
        public string MotDePasse { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string NAS { get; set; }
        public DateTime DateNaissance { get; set; }
        public string Adresse { get; set; }
        public string Telephone { get; set; }
    }
}