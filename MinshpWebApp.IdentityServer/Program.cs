using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.IdentityServer.Authentication;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 👉 Même base que le métier, mais schéma "auth" pour ce DbContext
builder.Services.AddDbContext<AuthDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("MainDb"),
        sql =>
        {
            // migrations stockées dans le projet IdentityServer
            sql.MigrationsAssembly(typeof(Program).Assembly.FullName);
            // 👉 Historique de migrations séparé (évite de partager __EFMigrationsHistory)
            sql.MigrationsHistoryTable("__EFMigrationsHistory_Auth", "auth");
        }));


builder.Services.AddIdentity<AppUser, IdentityRole>(o =>
{
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddOpenIddict()
    .AddCore(o => o.UseEntityFrameworkCore().UseDbContext<AuthDbContext>())
    .AddServer(o =>
    {
        o.SetAuthorizationEndpointUris("/connect/authorize");
        o.SetTokenEndpointUris("/connect/token");
        o.SetUserInfoEndpointUris("/connect/userinfo");
        o.SetEndSessionEndpointUris("/connect/logout");

        o.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();

        o.AllowPasswordFlow();                // 👈 POUR TESTS
        o.AllowRefreshTokenFlow();            // 👈 optionnel, pratique pour Postman

        o.AcceptAnonymousClients();           // 👈 utile pour tests avec client public
        o.DisableAccessTokenEncryption(); // 👈 dev seulement


        o.RegisterScopes(OpenIddictConstants.Scopes.OpenId,
                 OpenIddictConstants.Scopes.Profile,
                 "api");

        o.AddDevelopmentEncryptionCertificate()
         .AddDevelopmentSigningCertificate();

        o.UseAspNetCore()
         .EnableAuthorizationEndpointPassthrough()
         .EnableTokenEndpointPassthrough()
         .EnableUserInfoEndpointPassthrough()
         .EnableEndSessionEndpointPassthrough()
         .DisableTransportSecurityRequirement(); // dev
    });


// 1) Services
const string WebCors = "WebCors";
builder.Services.AddCors(o => o.AddPolicy(WebCors, p =>
    p.WithOrigins(
        "http://localhost:3000",  // ton React dev server
        "http://localhost:5173")  // si tu utilises vite
     .AllowAnyHeader()
     .AllowAnyMethod()
// .AllowCredentials() // inutile pour le password flow; utile plus tard si tu fais des requêtes avec cookies
));

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors(WebCors);
// Swagger UI (dev seulement si tu veux)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityServer API v1");
    c.RoutePrefix = "swagger";
});

using (var scope = app.Services.CreateScope())
{
    var apps = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
    var client = await apps.FindByClientIdAsync("react-spa");

    var descriptor = new OpenIddictApplicationDescriptor
    {
        ClientId = "react-spa",
        DisplayName = "React SPA",
        ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
        ClientType = OpenIddictConstants.ClientTypes.Public,

        RedirectUris = { new Uri("http://localhost:5173/callback") },
        PostLogoutRedirectUris = { new Uri("http://localhost:5173/") },

        Permissions =
        {
            // Endpoints
            OpenIddictConstants.Permissions.Endpoints.Authorization,
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.Endpoints.EndSession,

            // Flows
            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
            OpenIddictConstants.Permissions.GrantTypes.Password,     // ← important
            OpenIddictConstants.Permissions.GrantTypes.RefreshToken,  // ← si tu veux le refresh

            // Response types
            OpenIddictConstants.Permissions.ResponseTypes.Code,

            // Scopes
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Prefixes.Scope + "api"    // ← au lieu de "scp:api"
        },

        Requirements =
        {
            OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
        }
    };

    if (client is null)
    {
        await apps.CreateAsync(descriptor);
    }
    else
    {
        // Met à jour le client existant pour inclure le grant password
        await apps.UpdateAsync(client, descriptor);
    }

    var scopes = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
    if (await scopes.FindByNameAsync("api") is null)
    {
        await scopes.CreateAsync(new OpenIddictScopeDescriptor
        {
            Name = "api",
            DisplayName = "Access to Minshp API",
            Resources = { "api-resource" }
        });
    }
}


app.MapControllers();
app.MapGet("/", () => "IdentityServer (OpenIddict) OK");

app.Run();
