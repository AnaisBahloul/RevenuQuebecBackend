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

                // Ajouter les revenus - VERSION POUR BROUILLONS (accepte les vides)
                if (request.RevenusEmploi != null)
                {
                    foreach (var revenu in request.RevenusEmploi)
                    {
                        // POUR LES BROUILLONS : AJOUTE TOUT, même les lignes vides
                        declaration.AddRevenuEmploi(new RevenuEmploi(
                            revenu.Employeur ?? "",  // Garde string vide si null
                            revenu.Montant
                        ));
                    }
                }

                if (request.AutresRevenus != null)
                {
                    foreach (var revenu in request.AutresRevenus)
                    {
                        // POUR LES BROUILLONS : AJOUTE TOUT, même si Type est null
                        var type = revenu.Type ?? AutreRevenu.TypeRevenu.Autre;
                        declaration.AddAutreRevenu(new AutreRevenu(type, revenu.Montant));
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
                // Utilise les mêmes messages que GetDefaultMessage()
                DeclarationStatus.Recu => "Déclaration soumise avec succès",
                DeclarationStatus.ValideeAutomatiquement => "Validation automatique terminée",
                DeclarationStatus.EnRevisionParAgent => "Examen par un agent en cours",
                DeclarationStatus.Traitee => "Déclaration traitée et clôturée",
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
                DeclarationStatus.Traitee => "Déclaration traitée",
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


        // 6. SUPPRIMER un brouillon
        [HttpDelete("brouillon/{utilisateurId}")]
        public async Task<IActionResult> DeleteBrouillon(int utilisateurId)
        {
            try
            {
                var brouillon = await _declarationRepository.GetBrouillonParUtilisateurAsync(utilisateurId);

                if (brouillon == null)
                    return NotFound(new { message = "Aucun brouillon trouvé" });

                if (!brouillon.EstBrouillon)
                    return BadRequest(new { message = "Cette déclaration n'est pas un brouillon" });

                // CORRECTION : Passe l'objet brouillon, pas l'ID
                await _declarationRepository.DeleteAsync(brouillon);

                return Ok(new { message = "Brouillon supprimé avec succès" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", details = ex.Message });
            }
        }

        // 7. METTRE À JOUR un brouillon existant - VERSION CORRIGÉE
        [HttpPut("brouillon/{id}")]
        public async Task<IActionResult> UpdateBrouillon(int id, [FromBody] UpdateDeclarationRequest request)
        {
            try
            {
                // Récupérer le brouillon existant avec toutes les relations
                var brouillon = await _declarationRepository.GetByIdCompletAsync(id);
                if (brouillon == null)
                    return NotFound(new { message = "Brouillon non trouvé" });

                if (!brouillon.EstBrouillon)
                    return BadRequest(new { message = "Cette déclaration n'est pas un brouillon" });

                // Mettre à jour les champs de base seulement si fournis
                if (!string.IsNullOrWhiteSpace(request.Adresse))
                    brouillon.Adresse = request.Adresse;

                if (!string.IsNullOrWhiteSpace(request.Email))
                    brouillon.Email = request.Email;

                if (!string.IsNullOrWhiteSpace(request.Telephone))
                    brouillon.Telephone = request.Telephone;

                if (!string.IsNullOrWhiteSpace(request.Citoyennete))
                    brouillon.Citoyennete = request.Citoyennete;

                brouillon.ConfirmationExactitude = request.ConfirmationExactitude;
                brouillon.RevenusEmploi.Clear();

                if (request.RevenusEmploi != null)
                {
                    foreach (var revenu in request.RevenusEmploi)
                    {
                        // POUR LES BROUILLONS : AJOUTE TOUT
                        brouillon.AddRevenuEmploi(new RevenuEmploi(
                            revenu.Employeur ?? "",
                            revenu.Montant
                        ));
                    }
                }

                // 2. AUTRES REVENUS - Remplacer SIMPLEMENT
                brouillon.AutresRevenus.Clear();
                if (request.AutresRevenus != null)
                {
                    foreach (var revenu in request.AutresRevenus)
                    {
                        var type = revenu.Type ?? AutreRevenu.TypeRevenu.Autre;
                        brouillon.AddAutreRevenu(new AutreRevenu(type, revenu.Montant));
                    }
                }



                // Mettre à jour les fichiers

                brouillon.Fichiers.Clear();
                if (request.Fichiers != null)
                {
                    foreach (var fichier in request.Fichiers)
                    {
                        if (!string.IsNullOrWhiteSpace(fichier.Nom))
                        {
                            brouillon.AddJustificatif(new Justificatif(fichier.Nom, fichier.Url ?? ""));
                        }
                    }
                }

                // Mettre à jour l'étape
                if (request.CurrentStep.HasValue)
                {
                    // Ajouter un statut pour l'étape
                    brouillon.AddStatus(new Status(
                        DeclarationStatus.Brouillon,
                        $"Étape {request.CurrentStep} sauvegardée"
                    ));
                }

                // Sauvegarder les modifications
                await _declarationRepository.UpdateAsync(brouillon);

                return Ok(new
                {
                    id = brouillon.Id,
                    message = "Brouillon mis à jour",
                    estBrouillon = brouillon.EstBrouillon,
                    etat = brouillon.GetEtatAffichage(),
                    currentStep = request.CurrentStep
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", details = ex.Message });
            }
        }

        // 8. RÉCUPÉRER l'historique des statuts d'une déclaration
        [HttpGet("{id}/historique")]
        public async Task<IActionResult> GetHistoriqueStatuts(int id)
        {
            try
            {
                var declaration = await _declarationRepository.GetByIdCompletAsync(id);
                if (declaration == null)
                    return NotFound(new { message = "Déclaration non trouvée" });

                // Filtrer pour exclure les statuts "Brouillon" (optionnel)
                var statuts = declaration.HistoriqueStatuts
                    .Where(s => s.Etat != DeclarationStatus.Brouillon)
                    .OrderBy(s => s.DateEvenement)
                    .Select(s => new
                    {
                        etat = s.Etat.ToString(),
                        etatAffichage = GetEtatAffichage(s.Etat),
                        date = s.DateEvenement.ToString("yyyy-MM-dd HH:mm"),
                        dateSimple = s.DateEvenement.ToString("yyyy-MM-dd"),
                        message = s.Message ?? GetDefaultMessage(s.Etat),
                        icon = GetIconForStatus(s.Etat),
                        color = GetColorForStatus(s.Etat)
                    })
                    .ToList();

                // S'assurer qu'on retourne au moins le statut actuel si l'historique est vide
                if (statuts.Count == 0 && declaration.Etat != DeclarationStatus.Brouillon)
                {
                    statuts.Add(new
                    {
                        etat = declaration.Etat.ToString(),
                        etatAffichage = GetEtatAffichage(declaration.Etat),
                        date = declaration.DateSoumission?.ToString("yyyy-MM-dd HH:mm") ?? DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm"),
                        dateSimple = declaration.DateSoumission?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd"),
                        message = "Déclaration en cours de traitement",
                        icon = GetIconForStatus(declaration.Etat),
                        color = GetColorForStatus(declaration.Etat)
                    });
                }

                return Ok(new
                {
                    id = declaration.Id,
                    etatActuel = declaration.GetEtatAffichage(),
                    dateSoumission = declaration.DateSoumission?.ToString("yyyy-MM-dd"),
                    statuts = statuts,
                    dernierMessage = statuts.LastOrDefault()?.message,
                    // AJOUTER : indiquer si c'est un vrai historique ou généré
                    isRealHistorique = declaration.HistoriqueStatuts.Any(s => s.Etat != DeclarationStatus.Brouillon)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", details = ex.Message });
            }
        }

        // 9. ENDPOINT DE TEST : Ajouter manuellement un statut avec message par défaut
        [HttpPost("{id}/ajouter-statut-test")]
        public async Task<IActionResult> AjouterStatutTest(int id, [FromBody] AjouterStatutTestRequest request)
        {
            try
            {
                var declaration = await _declarationRepository.GetByIdCompletAsync(id);
                if (declaration == null)
                    return NotFound(new { message = "Déclaration non trouvée" });

                // Vérifier si l'état fourni est valide (1, 2, 3, 4)
                if (request.Etat < 1 || request.Etat > 4)
                {
                    return BadRequest(new
                    {
                        message = "État invalide. Utilisez : 1=Reçu, 2=ValidéeAuto, 3=EnRevision, 4=Traitée",
                        etatsValides = new[] { 1, 2, 3, 4 }
                    });
                }

                // Convertir l'entier en enum
                var etat = (DeclarationStatus)request.Etat;

                // Utiliser le message par défaut correspondant à l'état
                var message = GetDefaultMessage(etat);

                // Mettre à jour l'état actuel de la déclaration
                declaration.Etat = etat;

                // Ajouter le statut à l'historique
                var statut = new Status(etat, message);

                // Si une date est fournie, l'utiliser (sinon DateTime.UtcNow par défaut)
                if (request.DateEvenement.HasValue)
                {
                    statut.DateEvenement = request.DateEvenement.Value;
                }

                declaration.AddStatus(statut);

                // Sauvegarder les modifications
                await _declarationRepository.UpdateAsync(declaration);

                return Ok(new
                {
                    message = "Statut ajouté avec succès",
                    declarationId = declaration.Id,
                    etatActuel = declaration.GetEtatAffichage(),
                    etatCode = (int)declaration.Etat,
                    statutAjoute = new
                    {
                        etat = statut.Etat.ToString(),
                        etatAffichage = GetEtatAffichage(statut.Etat),
                        date = statut.DateEvenement.ToString("yyyy-MM-dd HH:mm"),
                        message = statut.Message,
                        icon = GetIconForStatus(statut.Etat),
                        color = GetColorForStatus(statut.Etat)
                    },
                    historiqueCount = declaration.HistoriqueStatuts
                        .Count(s => s.Etat != DeclarationStatus.Brouillon)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erreur serveur", details = ex.Message });
            }
        }
    

        // Helper pour les messages par défaut
        private string GetDefaultMessage(DeclarationStatus etat)
        {
            return etat switch
            {
                DeclarationStatus.Recu => "Déclaration soumise avec succès",
                DeclarationStatus.ValideeAutomatiquement => "Validation automatique terminée",
                DeclarationStatus.EnRevisionParAgent => "Examen par un agent en cours",
                DeclarationStatus.Traitee => "Déclaration traitée et clôturée",
                _ => "État mis à jour"
            };
        }

        // Helper pour les icônes
        private string GetIconForStatus(DeclarationStatus etat)
        {
            return etat switch
            {
                DeclarationStatus.Recu => "✓",
                DeclarationStatus.ValideeAutomatiquement => "…",
                DeclarationStatus.EnRevisionParAgent => "•",
                DeclarationStatus.Traitee => "✓",
                _ => "○"
            };
        }

        // Helper pour les couleurs
        private string GetColorForStatus(DeclarationStatus etat)
        {
            return etat switch
            {
                DeclarationStatus.Recu => "success",
                DeclarationStatus.ValideeAutomatiquement => "warning",
                DeclarationStatus.EnRevisionParAgent => "primary",
                DeclarationStatus.Traitee => "success",
                _ => "secondary"
            };
        }

        // Helper pour l'affichage de l'état
        private string GetEtatAffichage(DeclarationStatus etat)
        {
            return etat switch
            {
                DeclarationStatus.Brouillon => "Brouillon",
                DeclarationStatus.Recu => "Reçue",
                DeclarationStatus.ValideeAutomatiquement => "Validée automatiquement",
                DeclarationStatus.EnRevisionParAgent => "En révision par un agent",
                DeclarationStatus.Traitee => "Traitée",
                _ => "Inconnu"
            };
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

    // Ajoute cette classe dans DeclarationsController.cs
    public class UpdateDeclarationRequest
    {
        public string Adresse { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Citoyennete { get; set; }
        public List<RevenuEmploiRequest> RevenusEmploi { get; set; } = new();
        public List<AutreRevenuRequest> AutresRevenus { get; set; } = new();
        public List<FichierRequest> Fichiers { get; set; } = new();
        public bool ConfirmationExactitude { get; set; }
        public int? CurrentStep { get; set; }
    }

    // Classe pour la requête d'ajout de statut test
    public class AjouterStatutTestRequest
    {
        public int Etat { get; set; } // 1=Reçu, 2=ValidéeAuto, 3=EnRevision, 4=Traitée
        public DateTime? DateEvenement { get; set; } // Optionnel : date personnalisée
    }
}