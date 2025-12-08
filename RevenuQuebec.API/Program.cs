using Microsoft.EntityFrameworkCore;
using RevenuQuebec.Core.Interfaces;
using RevenuQuebec.Core.Services;
using RevenuQuebec.Infrastructure; 
using RevenuQuebec.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS pour React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// 2. Base de données SQL Server (ENLÈVE .Data)
builder.Services.AddDbContext<RevenuQuebecContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Services (comme dans ConsoleTestApp)
builder.Services.AddScoped<IAuthentificationService, AuthentificationService>();
builder.Services.AddScoped<IGestionAvisService, GestionAvisService>();
builder.Services.AddScoped<IGestionDeclarationService, GestionDeclarationService>();
builder.Services.AddScoped<IGestionUtilisateurService, GestionUtilisateurService>();
builder.Services.AddScoped<IGestionSessionService, GestionSessionService>();

// 4. Repositories (comme dans ConsoleTestApp)
builder.Services.AddScoped<IAvisRepository, AvisRepository>();
builder.Services.AddScoped<IDeclarationRepository, DeclarationRepository>();
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

// 5. Contrôleurs + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp"); // AVANT UseAuthorization
app.UseAuthorization();

// 7. Map controllers
app.MapControllers();

app.Run();