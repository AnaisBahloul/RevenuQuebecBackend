using RevenuQuebec.SharedKernel;
using RevenuQuebec.SharedKernel.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace RevenuQuebec.Core.Entities
{
    public class Avis : BaseEntity, IAggregateRoot
    {
        public int DeclarationId { get; set; }
        [JsonIgnore]
        public Declaration Declaration { get; set; }

        [NotMapped]
        public int UserId { get; set; }

        public AvisType Type { get; set; }
        public bool RequiresAgentReview { get; set; }
        public DateTime GenerationDate { get; set; } = DateTime.UtcNow;

        public string Title { get; set; }
        public string RefNumber { get; set; }
        public string Year { get; set; }
        public string Amount { get; set; }
        public string TaxableIncome { get; set; }
        public string Deductions { get; set; }
        public string NetTax { get; set; }
        public string AmountPayable { get; set; }

        public List<string> AdjustmentNotes { get; set; } = new();

        

        public void AddNote(string note)
        {
            if (!string.IsNullOrWhiteSpace(note))
                AdjustmentNotes.Add(note);
        }

        public void RemoveNote(string note)
        {
            if (AdjustmentNotes.Contains(note))
                AdjustmentNotes.Remove(note);
        }

        // Constructeur par défaut
        public Avis() { }

        // Constructeur avec paramètres principaux
        public Avis(AvisType type, string title, string year, string refNumber, string amount, bool requiresAgentReview,
                    string taxableIncome, string deductions, string netTax, string amountPayable)
        { 
            Type = type;
            Title = title;
            Year = year;
            RequiresAgentReview = requiresAgentReview;
            RefNumber = refNumber;
            Amount = amount;
            TaxableIncome = taxableIncome;
            Deductions = deductions;
            NetTax = netTax;
            AmountPayable = amountPayable;
            //Declaration = declaration;
        }
    }

    public enum AvisType
    {
        Automatique,
        Personnalise
    }




}
