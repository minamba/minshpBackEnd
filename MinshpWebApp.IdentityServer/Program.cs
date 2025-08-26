using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.IdentityServer.Authentication;
using MinshpWebApp.IdentityServer.Claims;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// URL de ton API (adapte si besoin)
builder.Services.AddHttpClient("api", c =>
{
    c.BaseAddress = new Uri("https://localhost:7057/");
    c.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});


// DbContext Auth (schéma "auth")
builder.Services.AddDbContext<AuthDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("MainDb"),
        sql =>
        {
            sql.MigrationsAssembly(typeof(Program).Assembly.FullName);
            sql.MigrationsHistoryTable("__EFMigrationsHistory_Auth", "auth");
        }));

// Identity + rôles
builder.Services.AddIdentity<AppUser, IdentityRole>(o =>
{
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireUppercase = false;
    o.User.RequireUniqueEmail = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// Claims factory (pour inclure les rôles dans le principal)
builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomUserClaimsPrincipalFactory>();

// CORS — doit être AVANT Build()
const string WebCors = "WebCors";
builder.Services.AddCors(o => o.AddPolicy(WebCors, p =>
    p.WithOrigins(
        "http://localhost:3000",
        "https://localhost:3000",
        "http://localhost:5173"
    )
    .AllowAnyHeader()
    .AllowAnyMethod()
// .AllowCredentials() // pas nécessaire pour /connect/token
));



// Pour que [Authorize] utilise la validation par défaut
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = OpenIddict.Validation.AspNetCore
        .OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIddict.Validation.AspNetCore
        .OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});




// OpenIddict — AVANT Build()
builder.Services.AddOpenIddict()
    .AddCore(o => o.UseEntityFrameworkCore().UseDbContext<AuthDbContext>())
    .AddValidation(o =>
     {
            // Même serveur (local) : réutilise les clés/signatures du serveur
            o.UseLocalServer();

            // Branche la validation sur l’auth ASP.NET Core (schéma Bearer)
            o.UseAspNetCore();
     })
    .AddServer(o =>
    {
        o.SetAuthorizationEndpointUris("/connect/authorize");
        o.SetTokenEndpointUris("/connect/token");
        o.SetUserInfoEndpointUris("/connect/userinfo");
        o.SetEndSessionEndpointUris("/connect/logout");

        o.AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange();
        o.AllowPasswordFlow();
        o.AllowRefreshTokenFlow();

        // nécessaire pour récupérer un jeton technique dans Register()
        o.AllowClientCredentialsFlow();


        o.AcceptAnonymousClients();
        o.DisableAccessTokenEncryption(); // dev

        o.RegisterScopes(Scopes.OpenId, Scopes.Profile, Scopes.Roles, "api");

        o.AddDevelopmentEncryptionCertificate()
         .AddDevelopmentSigningCertificate();

        o.UseAspNetCore()
         .EnableAuthorizationEndpointPassthrough()
         .EnableTokenEndpointPassthrough()
         .EnableUserInfoEndpointPassthrough()
         .EnableEndSessionEndpointPassthrough()
         .DisableTransportSecurityRequirement(); // dev

        // destinations des claims
        o.AddEventHandler<OpenIddictServerEvents.ProcessSignInContext>(b =>
        {
            b.UseInlineHandler(ctx =>
            {
                foreach (var claim in ctx.Principal!.Claims)
                {
                    var dest = new List<string> { Destinations.AccessToken };
                    if (claim.Type is Claims.Subject or Claims.Name or Claims.Email or Claims.Role)
                        dest.Add(Destinations.IdentityToken);
                    claim.SetDestinations(dest);
                }
                return default;
            });
        });
    });


builder.Services.AddControllers();

var app = builder.Build();

// seed rôles et admin
async Task SeedRolesAsync(IServiceProvider sp)
{
    var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var r in new[] { "Admin", "Manager", "Customer" })
        if (!await roleMgr.RoleExistsAsync(r))
            await roleMgr.CreateAsync(new IdentityRole(r));
}
using (var scope = app.Services.CreateScope())
{
    await SeedRolesAsync(scope.ServiceProvider);

    // client OpenIddict
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
            OpenIddictConstants.Permissions.Endpoints.Authorization,
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.Endpoints.EndSession,
            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
            OpenIddictConstants.Permissions.GrantTypes.Password,
            OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
            OpenIddictConstants.Permissions.ResponseTypes.Code,
            OpenIddictConstants.Permissions.Scopes.Profile,
             OpenIddictConstants.Permissions.Scopes.Roles,
            OpenIddictConstants.Permissions.Prefixes.Scope + "api"
        },
        Requirements = { OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange }
    };
    if (client is null) await apps.CreateAsync(descriptor);
    else await apps.UpdateAsync(client, descriptor);


    // --- CLIENT CONFIDENTIEL pour appels serveur-à-serveur ---
    var svcClient = await apps.FindByClientIdAsync("minshp-api-client");
    var confidential = new OpenIddictApplicationDescriptor
    {
        ClientId = "minshp-api-client",
        ClientType = OpenIddictConstants.ClientTypes.Confidential,
        ClientSecret = "super-secret-change-me", // en dev OK. En prod: Hash + secret manager.

        Permissions =
        {
            // Endpoints
            OpenIddictConstants.Permissions.Endpoints.Token,

            // Grant
            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,

            // Scopes
            OpenIddictConstants.Permissions.Prefixes.Scope + "api"
        }
    };

    if (svcClient is null) await apps.CreateAsync(confidential);
    else await apps.UpdateAsync(svcClient, confidential);



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

    // admin
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var admin = await userMgr.FindByEmailAsync("contact@minshp.com");
    if (admin is null)
    {
        admin = new AppUser { UserName = "mins@admin.com", Email = "contact@minshp.com", EmailConfirmed = true };
        await userMgr.CreateAsync(admin, "cdjeneba19882025");
        await userMgr.AddToRoleAsync(admin, "Admin");
    }
}

// ------- pipeline (ordre important)
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(WebCors);
app.UseAuthentication();
app.UseAuthorization();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityServer API v1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();
app.MapGet("/", () => "IdentityServer (OpenIddict) OK");

app.Run();
