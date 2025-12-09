// Controllers/AvisController.cs
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using RevenuQuebec.Core.Entities;
using RevenuQuebec.Core.Interfaces;
using System.IO;
using System.Threading.Tasks;
using static RevenuQuebec.Core.Entities.AutreRevenu;

namespace RevenuQuebec.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AvisController : ControllerBase
    {
        private readonly IAvisRepository _avisRepository;

        public AvisController(IAvisRepository avisRepository)
        {
            _avisRepository = avisRepository;
        }

        // Lister tous les avis
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Avis>>> GetAll()
        {
            var avis = await _avisRepository.ListAllAsync();
            return Ok(avis); // Retourne directement les entités
        }

        // Avis par Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Avis>> GetById(int id)
        {
            var avis = await _avisRepository.GetByIdAsync(id);
            if (avis == null) return NotFound();
            return Ok(avis); // Retourne directement l'entité
        }

        // Avis par utilisateur
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Avis>>> GetByUserId(int userId)
        {
            var avisList = await _avisRepository.GetByUserIdAsync(userId);
            return Ok(avisList);
        }

        // Avis par déclaration
        [HttpGet("declaration/{declarationId}")]
        public async Task<ActionResult<Avis>> GetByDeclarationId(int declarationId)
        {
            var avis = await _avisRepository.GetByDeclarationIdAsync(declarationId);
            if (avis == null) return NotFound();
            return Ok(avis);
        }
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadPdf(int id)
        {
            var avis = await _avisRepository.GetByIdAsync(id);
            if (avis == null)
                return NotFound("Avis non trouvé");

            var declaration = avis.Declaration;
            if (declaration == null)
                return NotFound("Déclaration associée non trouvée");

            byte[] pdfBytes;

            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                // Header
                document.Add(new Paragraph($"Avis de cotisation • {avis.Year}")
                    .SetFont(boldFont)
                    .SetFontSize(18));
                document.Add(new Paragraph($"Avis fiscal {avis.Year}")
                    .SetFontSize(14)
                    .SetFont(boldFont));
                document.Add(new Paragraph($"Référence : {avis.RefNumber}"));
                document.Add(new Paragraph($"Rédigé par un agent le : {avis.GenerationDate:yyyy-MM-dd HH:mm:ss}"));

                // Identité
                document.Add(new Paragraph("Identité du contribuable").SetFont(boldFont));
                document.Add(new Paragraph($"Nom : {declaration.Utilisateur.Prenom} {declaration.Utilisateur.Nom}"));
                document.Add(new Paragraph($"NAS : {declaration.Utilisateur.NAS}"));
                document.Add(new Paragraph($"Année fiscale : {avis.Year}"));

                // Montant final
                document.Add(new Paragraph("Montant final").SetFont(boldFont));
                document.Add(new Paragraph($"{avis.Amount}"));
                document.Add(new Paragraph("Montant à recevoir / Montant à payer"));

                // Revenus
                document.Add(new Paragraph("Résumé des revenus déclarés").SetFont(boldFont));
                foreach (var revenu in declaration.RevenusEmploi)
                {
                    document.Add(new Paragraph($"Revenus d'emploi - {revenu.Employeur} : {revenu.Montant}$"));
                }
                foreach (var autre in declaration.AutresRevenus)
                {
                    string type = autre.Type == TypeRevenu.Interets ? "Intérêts" : "Autres revenus";
                    document.Add(new Paragraph($"{type} : {autre.Montant}$"));
                }

                // Calcul de l'impôt
                document.Add(new Paragraph("Calcul de l’impôt").SetFont(boldFont));
                document.Add(new Paragraph($"Revenu imposable : {avis.TaxableIncome}"));
                document.Add(new Paragraph($"Déductions : {avis.Deductions}"));
                document.Add(new Paragraph($"Impôt net : {avis.NetTax}"));
                document.Add(new Paragraph($"Montant à payer / recevoir : {avis.AmountPayable}"));

                // Notes d'ajustement
                if (avis.AdjustmentNotes != null && avis.AdjustmentNotes.Count > 0)
                {
                    document.Add(new Paragraph("Avis personnalisé par un agent").SetFont(boldFont));
                    document.Add(new Paragraph("Ajustements / précisions :"));
                    foreach (var note in avis.AdjustmentNotes)
                    {
                        document.Add(new Paragraph($"- {note}"));
                    }
                }

                document.Add(new Paragraph("RevenuQuébec © 2025").SetTextAlignment(TextAlignment.CENTER));

                document.Close();
                pdfBytes = ms.ToArray();
            }

            return File(pdfBytes, "application/pdf", $"avis-{avis.Id}.pdf");
        }



    }
}
