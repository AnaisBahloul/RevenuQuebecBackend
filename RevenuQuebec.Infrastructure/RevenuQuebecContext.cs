using Microsoft.EntityFrameworkCore;
using RevenuQuebec.Core.Entities;

namespace RevenuQuebec.Infrastructure
{
    public class RevenuQuebecContext : DbContext
    {
        // DbSets pour toutes tes entités
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Declaration> Declarations { get; set; }
        public DbSet<Avis> Avis { get; set; }
        public DbSet<RevenuEmploi> RevenusEmploi { get; set; }
        public DbSet<AutreRevenu> AutresRevenus { get; set; }
        public DbSet<Justificatif> Justificatifs { get; set; }
        public DbSet<Status> Statuts { get; set; }
        public DbSet<Session> Sessions { get; set; }

        public RevenuQuebecContext(DbContextOptions<RevenuQuebecContext> options)
            : base(options)
        { }

        public RevenuQuebecContext()
            : base(new DbContextOptionsBuilder<RevenuQuebecContext>()
                .UseSqlServer(@"Server=.;Database=RevenuQuebec_DB;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AutreRevenu>()
                .Property(ar => ar.Montant)
                .HasPrecision(18, 2);  // 18 chiffres au total, 2 décimales

            modelBuilder.Entity<RevenuEmploi>()
                .Property(re => re.Montant)
                .HasPrecision(18, 2);
           
            
            
            modelBuilder.Entity<Declaration>()
    .HasOne(d => d.Avis)
    .WithOne(a => a.Declaration)
    .HasForeignKey<Declaration>(d => d.AvisId);

        }

    }
}
