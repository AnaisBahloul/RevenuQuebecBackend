// Controllers/DeclarationsController.cs
using Microsoft.AspNetCore.Mvc;
using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevenuQuebec.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeclarationsController : ControllerBase
    {
        private readonly IDeclarationRepository _declarationRepository;
        private readonly IUtilisateurRepository _utilisateurRepository;

        public DeclarationsController(
            IDeclarationRepository declarationRepository,
            IUtilisateurRepository utilisateurRepository)
        {
            _declarationRepository = declarationRepository;
            _utilisateurRepository = utilisateurRepository;
        }

        // 1. CRÉER une déclaration
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDeclarationRequest request)
        {
            try
            {
                // Vérifier l'utilisateur
                var utilisateur = await _utilisateurRepository.GetByIdAsync(request.UtilisateurId);
                if (utilisateur == null)
                    return NotFound(new { message = "Utilisateur non trouvé" });

                // Vérifier s'il existe déjà un brouillon (seulement pour les brouillons)
                if (request.EstBrouillon || request.Etat == DeclarationStatus.Brouillon)
                {
                    var brouillonExistant = await _declarationRepository.GetBrouillonParUtilisateurAsync(request.UtilisateurId);
                    if (brouillonExistant != null)
                    {
                        return BadRequest(new
                        {
                            message = "Vous avez déjà un brouillon en cours",
                            brouillonId = brouillonExistant.Id
                        });
                    }
                }

                // Créer la déclaration
                var declaration = new Declaration(
                    request.Adresse,
                    request.Email,
                    request.Telephone,
                    request.Citoyennete)
                {
                    UtilisateurId = request.UtilisateurId,
                    EstBrouillon = request.EstBrouillon,
                    ConfirmationExactitude = request.ConfirmationExactitude
                };

                // DÉFINIR L'ÉTAT SELON LA REQUÊTE OU PAR DÉFAUT
                if (request.Etat.HasValue)
                {
                    // Utiliser l'état fourni dans la requête
                    declaration.Etat = request.Etat.Value;

                    // Si l'état n'est pas "Brouillon", mais que EstBrouillon est true, corriger
                    if (request.Etat.Value != DeclarationStatus.Brouillon && request.EstBrouillon)
                    {
                        declaration.EstBrouillon = false;
                    }

                    // Si l'état est "Brouillon", forcer EstBrouillon à true
                    if (request.Etat.Value == DeclarationStatus.Brouillon)
                    {
                        declaration.EstBrouillon = true;
                    }

                    // Ajouter le statut initial
                    declaration.AddStatus(new Status(
                        request.Etat.Value,
                        GetMessageForStatus(request.Etat.Value, request.CurrentStep)
                    ));
                }
                else
                {
                    // Pas d'état fourni, utiliser la logique par défaut
                    if (request.EstBrouillon)
                    {
                        declaration.Etat = DeclarationStatus.Brouillon;
                        declaration.AddStatus(new Status(
                            DeclarationStatus.Brouillon,
                            request.CurrentStep.HasValue
                                ? $"Étape {request.CurrentStep} sauvegardée"
                                : "Déclaration créée comme brouillon"
                        ));
                    }
                    else
                    {
                        declaration.Etat = DeclarationStatus.Recu;
                        declaration.DateSoumission = DateTime.UtcNow;
                        declaration.AddStatus(new Status(
                            DeclarationStatus.Recu,
                            "Déclaration soumise"
                        ));
                    }
                }

                /*
                 
                  // AJOUTER LA DATE DE SOUMISSION POUR LES ÉTATS NON-BROUILLONS
        if (declaration.Etat != DeclarationStatus.Brouillon && !declaration.DateSoumission.HasValue)
        {
            declaration.DateSoumission = DateTime.UtcNow;
        }
 

                 */

                // Ajouter les revenus
                if (request.RevenusEmploi != null)
                {
                    foreach (var revenu in request.RevenusEmploi)
                    {
                        if (!string.IsNullOrWhiteSpace(revenu.Employeur))
                        {
                            declaration.AddRevenuEmploi(new RevenuEmploi(revenu.Employeur, revenu.Montant));
                        }
                    }
                }

                if (request.AutresRevenus != null)
                {
                    foreach (var revenu in request.AutresRevenus)
                    {
                        if (revenu.Type != null)
                        {
                            declaration.AddAutreRevenu(new AutreRevenu(revenu.Type.Value, revenu.Montant));
                        }
                    }
                }

                // Ajouter les fichiers
                if (request.Fichiers != null)
                {
                    foreach (var fichier in request.Fichiers)
                    {
                        if (!string.IsNullOrWhiteSpace(fichier.Nom))
                        {
                            declaration.AddJustificatif(new Justificatif(fichier.Nom, fichier.Url ?? ""));
                        }
                    }
                }

                // Sauvegarder
                await _declarationRepository.AddAsync(declaration);

                // Associer à l'utilisateur
                utilisateur.AddDeclaration(declaration);
                await _utilisateurRepository.UpdateAsync(utilisateur);

                return Ok(new
                {
                    id = declaration.Id,
                    message = GetSuccessMessage(declaration.Etat),
                    estBrouillon = declaration.EstBrouillon,
                    etat = declaration.GetEtatAffichage(),
                    etatCode = (int)declaration.Etat
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", details = ex.Message });
            }
        }

        // Méthode helper pour le message selon l'état
        private string GetMessageForStatus(DeclarationStatus etat, int? currentStep)
        {
            return etat switch
            {
                DeclarationStatus.Brouillon => currentStep.HasValue
                    ? $"Étape {currentStep} sauvegardée"
                    : "Déclaration créée comme brouillon",
                DeclarationStatus.Recu => "Déclaration soumise",
                DeclarationStatus.ValideeAutomatiquement => "Déclaration validée automatiquement",
                DeclarationStatus.EnRevisionParAgent => "Déclaration mise en révision par un agent",
                DeclarationStatus.Cloturee => "Déclaration clôturée",
                _ => "État défini"
            };
        }

        // Méthode helper pour le message de succès
        private string GetSuccessMessage(DeclarationStatus etat)
        {
            return etat switch
            {
                DeclarationStatus.Brouillon => "Brouillon créé",
                DeclarationStatus.Recu => "Déclaration soumise",
                DeclarationStatus.ValideeAutomatiquement => "Déclaration validée automatiquement",
                DeclarationStatus.EnRevisionParAgent => "Déclaration mise en révision par un agent",
                DeclarationStatus.Cloturee => "Déclaration clôturée",
                _ => "Déclaration créée"
            };
        }

        // 2. RÉCUPÉRER le brouillon d'un utilisateur
        [HttpGet("brouillon/{utilisateurId}")]
        public async Task<IActionResult> GetBrouillon(int utilisateurId)
        {
            try
            {
                var brouillon = await _declarationRepository.GetBrouillonParUtilisateurAsync(utilisateurId);

                if (brouillon == null)
                    return NotFound(new { message = "Aucun brouillon trouvé" });

                // Trouver l'étape sauvegardée
                var etapeStatus = brouillon.HistoriqueStatuts
                    .Where(h => h.Message != null && h.Message.Contains("Étape"))
                    .OrderByDescending(h => h.DateEvenement)
                    .FirstOrDefault();

                int? currentStep = null;
                if (etapeStatus != null && int.TryParse(
                    etapeStatus.Message.Replace("Étape", "").Replace("sauvegardée", "").Trim(),
                    out int step))
                {
                    currentStep = step;
                }

                return Ok(new
                {
                    id = brouillon.Id,
                    adresse = brouillon.Adresse,
                    email = brouillon.Email,
                    telephone = brouillon.Telephone,
                    citoyennete = brouillon.Citoyennete,
                    revenusEmploi = brouillon.RevenusEmploi.Select(r => new { r.Employeur, r.Montant }),
                    autresRevenus = brouillon.AutresRevenus.Select(r => new
                    {
                        type = r.Type,
                        montant = r.Montant,
                        typeDescription = r.Type.ToString()
                    }),
                    fichiers = brouillon.Fichiers.Select(f => new { f.Nom, f.Url }),
                    confirmationExactitude = brouillon.ConfirmationExactitude,
                    estBrouillon = brouillon.EstBrouillon,
                    etat = brouillon.GetEtatAffichage(),
                    etatCode = (int)brouillon.Etat,
                    currentStep = currentStep,
                    historiqueStatuts = brouillon.HistoriqueStatuts.Select(s => new
                    {
                        etat = s.Etat.ToString(),
                        date = s.DateEvenement.ToString("yyyy-MM-dd HH:mm"),
                        message = s.Message
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", details = ex.Message });
            }
        }

        // 3. SOUMETTRE un brouillon
        [HttpPost("{id}/soumettre")]
        public async Task<IActionResult> Soumettre(int id)
        {
            try
            {
                var declaration = await _declarationRepository.GetByIdAsync(id);
                if (declaration == null)
                    return NotFound(new { message = "Déclaration non trouvée" });

                if (!declaration.EstBrouillon)
                    return BadRequest(new { message = "Cette déclaration n'est pas un brouillon" });

                // Soumettre le brouillon
                declaration.SoumettreBrouillon();
                await _declarationRepository.UpdateAsync(declaration);

                return Ok(new
                {
                    message = "Déclaration soumise avec succès",
                    id = declaration.Id,
                    dateSoumission = declaration.DateSoumission,
                    etat = declaration.GetEtatAffichage()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", details = ex.Message });
            }
        }

        // 4. RÉCUPÉRER toutes les déclarations d'un utilisateur
        [HttpGet("user/{utilisateurId}")]
        public async Task<IActionResult> GetByUser(int utilisateurId)
        {
            try
            {
                var declarations = await _declarationRepository.GetDeclarationsByUserAsync(utilisateurId);

                return Ok(declarations.Select(d => new
                {
                    id = d.Id,
                    adresse = d.Adresse,
                    email = d.Email,
                    dateSoumission = d.DateSoumission?.ToString("yyyy-MM-dd"),
                    estBrouillon = d.EstBrouillon,
                    etat = d.GetEtatAffichage(),
                    etatCode = (int)d.Etat,
                    nbRevenusEmploi = d.RevenusEmploi.Count,
                    nbAutresRevenus = d.AutresRevenus.Count,
                    nbFichiers = d.Fichiers.Count,
                    avisId = d.AvisId,
                    // AJOUTE ÇA pour les avis
                    avis = d.Avis != null ? new
                    {
                        id = d.Avis.Id,
                        title = d.Avis.Title,
                        amount = d.Avis.Amount,
                        amountPayable = d.Avis.AmountPayable,
                        year = d.Avis.Year
                    } : null
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", details = ex.Message });
            }
        }

        // 5. RÉCUPÉRER une déclaration par ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var declaration = await _declarationRepository.GetByIdCompletAsync(id);
                if (declaration == null)
                    return NotFound(new { message = "Déclaration non trouvée" });

                return Ok(new
                {
                    id = declaration.Id,
                    adresse = declaration.Adresse,
                    email = declaration.Email,
                    telephone = declaration.Telephone,
                    citoyennete = declaration.Citoyennete,
                    revenusEmploi = declaration.RevenusEmploi.Select(r => new { r.Employeur, r.Montant }),
                    autresRevenus = declaration.AutresRevenus.Select(r => new
                    {
                        type = r.Type,
                        montant = r.Montant,
                        typeDescription = r.Type.ToString()
                    }),
                    fichiers = declaration.Fichiers.Select(f => new { f.Nom, f.Url }),
                    confirmationExactitude = declaration.ConfirmationExactitude,
                    estBrouillon = declaration.EstBrouillon,
                    etat = declaration.GetEtatAffichage(),
                    etatCode = (int)declaration.Etat,
                    dateSoumission = declaration.DateSoumission,
                    utilisateurId = declaration.UtilisateurId,
                    utilisateur = declaration.Utilisateur != null ? new
                    {
                        id = declaration.Utilisateur.Id,
                        nom = declaration.Utilisateur.Nom,
                        prenom = declaration.Utilisateur.Prenom
                    } : null,
                    avis = declaration.Avis != null ? new
                    {
                        id = declaration.Avis.Id,
                        title = declaration.Avis.Title
                    } : null,
                    historiqueStatuts = declaration.HistoriqueStatuts.Select(s => new
                    {
                        etat = s.Etat.ToString(),
                        date = s.DateEvenement.ToString("yyyy-MM-dd HH:mm"),
                        message = s.Message
                    }).OrderBy(s => s.date)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", details = ex.Message });
            }
        }
    }

    // Classes de requête
    public class CreateDeclarationRequest
    {
        public string Adresse { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Citoyennete { get; set; }
        public List<RevenuEmploiRequest> RevenusEmploi { get; set; } = new();
        public List<AutreRevenuRequest> AutresRevenus { get; set; } = new();
        public List<FichierRequest> Fichiers { get; set; } = new();
        public bool ConfirmationExactitude { get; set; }
        public bool EstBrouillon { get; set; }
        public DeclarationStatus? Etat { get; set; }
        public int UtilisateurId { get; set; }
        public int? CurrentStep { get; set; }
    }

    public class RevenuEmploiRequest
    {
        public string Employeur { get; set; }
        public decimal Montant { get; set; }
    }

    public class AutreRevenuRequest
    {
        public AutreRevenu.TypeRevenu? Type { get; set; }
        public decimal Montant { get; set; }
    }

    public class FichierRequest
    {
        public string Nom { get; set; }
        public string Url { get; set; }
    }
}