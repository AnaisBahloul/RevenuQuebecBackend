// Controllers/UtilisateursController.cs
using Microsoft.AspNetCore.Mvc;
using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using RevenuQuebec.Core.Services;
using System.Threading.Tasks;

namespace RevenuQuebec.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilisateursController : ControllerBase
    {
        private readonly IGestionUtilisateurService _userService;

        public UtilisateursController(IGestionUtilisateurService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var utilisateur = await _userService.ConsulterUtilisateur(id);
            if (utilisateur == null)
                return NotFound();

            // Retourne TOUTES les infos
            return Ok(new
            {
                id = utilisateur.Id,
                nom = utilisateur.Nom,
                prenom = utilisateur.Prenom,
                courriel = utilisateur.Courriel,
                nas = utilisateur.NAS,
                dateNaissance = utilisateur.DateNaissance.ToString("yyyy-MM-dd"),
                adresse = utilisateur.Adresse,
                telephone = utilisateur.Telephone
            });
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult> GetByEmail(string email)
        {
            var utilisateur = await _userService.ConsulterParCourriel(email);
            if (utilisateur == null)
                return NotFound();
            return Ok(utilisateur);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUtilisateurRequest request)
        {
            try
            {
                var utilisateur = await _userService.ConsulterUtilisateur(id);
                if (utilisateur == null)
                    return NotFound(new { message = "Utilisateur non trouvé" });

                // Mettre à jour seulement les champs fournis
                if (!string.IsNullOrEmpty(request.Courriel))
                {
                    utilisateur.UpdateCourriel(request.Courriel);
                }

                if (request.Adresse != null) // Accepte les chaînes vides
                {
                    utilisateur.UpdateAdresse(request.Adresse);
                }

                if (request.Telephone != null) // Accepte les chaînes vides
                {
                    utilisateur.UpdateTelephone(request.Telephone);
                }

                await _userService.UpdateUtilisateur(utilisateur);

                return Ok(new
                {
                    message = "Profil mis à jour avec succès",
                    utilisateur = new
                    {
                        id = utilisateur.Id,
                        nom = utilisateur.Nom,
                        prenom = utilisateur.Prenom,
                        courriel = utilisateur.Courriel,
                        adresse = utilisateur.Adresse,
                        telephone = utilisateur.Telephone
                    }
                });
                Console.WriteLine(" PUT reçu pour ID: " + id);
                Console.WriteLine(" Données reçues: "
                    + $"Courriel={request.Courriel}, Adresse={request.Adresse}, Tel={request.Telephone}");

            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur interne", details = ex.Message });
            }
        }

        public class UpdateUtilisateurRequest
        {
            public string Courriel { get; set; }
            public string Adresse { get; set; }
            public string Telephone { get; set; }
        }
    }
}