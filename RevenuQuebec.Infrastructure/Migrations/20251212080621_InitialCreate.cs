using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RevenuQuebec.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Avis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeclarationId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RequiresAgentReview = table.Column<bool>(type: "bit", nullable: false),
                    GenerationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxableIncome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deductions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetTax = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AmountPayable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdjustmentNotes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Utilisateurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Courriel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MotDePasse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NAS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateNaissance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilisateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Declarations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telephone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Citoyennete = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfirmationExactitude = table.Column<bool>(type: "bit", nullable: false),
                    EstBrouillon = table.Column<bool>(type: "bit", nullable: false),
                    DateSoumission = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AvisId = table.Column<int>(type: "int", nullable: true),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    Etat = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Declarations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Declarations_Avis_AvisId",
                        column: x => x.AvisId,
                        principalTable: "Avis",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Declarations_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateConnexion = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DateDeconnexion = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Utilisateurs_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "Utilisateurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AutresRevenus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeclarationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutresRevenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AutresRevenus_Declarations_DeclarationId",
                        column: x => x.DeclarationId,
                        principalTable: "Declarations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Justificatifs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeclarationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Justificatifs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Justificatifs_Declarations_DeclarationId",
                        column: x => x.DeclarationId,
                        principalTable: "Declarations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RevenusEmploi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Employeur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DeclarationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevenusEmploi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RevenusEmploi_Declarations_DeclarationId",
                        column: x => x.DeclarationId,
                        principalTable: "Declarations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Statuts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeclarationId = table.Column<int>(type: "int", nullable: false),
                    Etat = table.Column<int>(type: "int", nullable: false),
                    DateEvenement = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Statuts_Declarations_DeclarationId",
                        column: x => x.DeclarationId,
                        principalTable: "Declarations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AutresRevenus_DeclarationId",
                table: "AutresRevenus",
                column: "DeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_Declarations_AvisId",
                table: "Declarations",
                column: "AvisId",
                unique: true,
                filter: "[AvisId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Declarations_UtilisateurId",
                table: "Declarations",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Justificatifs_DeclarationId",
                table: "Justificatifs",
                column: "DeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_RevenusEmploi_DeclarationId",
                table: "RevenusEmploi",
                column: "DeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UtilisateurId",
                table: "Sessions",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Statuts_DeclarationId",
                table: "Statuts",
                column: "DeclarationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutresRevenus");

            migrationBuilder.DropTable(
                name: "Justificatifs");

            migrationBuilder.DropTable(
                name: "RevenusEmploi");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Statuts");

            migrationBuilder.DropTable(
                name: "Declarations");

            migrationBuilder.DropTable(
                name: "Avis");

            migrationBuilder.DropTable(
                name: "Utilisateurs");
        }
    }
}
