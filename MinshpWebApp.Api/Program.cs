using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Builders.impl;
using MinshpWebApp.Api.Options;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Dal.Repositories;
using MinshpWebApp.Domain.Repositories;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;
using MinshpWebApp.Domain.Services.Shipping;
using MinshpWebApp.Domain.Services.Shipping.impl;
using OpenIddict.Validation.AspNetCore;
using Stripe;
using System.Net.Http.Headers;
using QuestPDF.Infrastructure; // n'oublie pa


QuestPDF.Settings.License = LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);


// Activer les logs dans la console
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Minshp API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Bearer. Exemple: Bearer {token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddDbContext<MinshpDatabaseContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("MainDb"),
        sql =>
        {
            sql.MigrationsAssembly("MinshpWebApp.Dal");
            sql.MigrationsHistoryTable("__EFMigrationsHistory_Business", "dbo");
        }));
builder.Services.AddMvc();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// AuthN/AuthZ : l’API valide les tokens émis par ton IdentityServer OpenIddict
// AuthN/AuthZ : valider les tokens émis par ton IdentityServer OpenIddict
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("https://localhost:7183"); // ← URL EXACTE de ton IdentityServer
        options.AddAudiences("api-resource");     // décommente si tu utilises une audience nommée
        options.UseSystemNetHttp();
        options.UseAspNetCore();
    });

builder.Services.AddAuthorization();


// CORS (déclare une policy nommée pour la réutiliser)
const string CorsPolicy = "WebCors";
builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, p =>
    p.WithOrigins("http://localhost:3000", "http://localhost:5173", "https://bd158b87393d.ngrok.app")
     .AllowAnyHeader()
     .AllowAnyMethod()));




//scoped repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
builder.Services.AddScoped<IFeatureRepository, FeatureRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductFeatureRepository, ProductFeatureRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IFeatureCategoryRepository, FeatureCategoryRepository>();
builder.Services.AddScoped<ITaxeRepository, TaxeRepository>();
builder.Services.AddScoped<IPromotionCodeRepository, PromotionCodeRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IBillingAddressRepository, BillingAddressRepository>();
builder.Services.AddScoped<IDeliveryAddressRepository, DeliveryAddressRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IOrderCustomerProductRepository, OrderCustomerProductRepository>();
builder.Services.AddScoped<IPackageProfilRepository, PackageProfilRepository>();
builder.Services.AddScoped<ISubCategoryRepository, SubCategoryRepository>();


//scoped services
builder.Services.AddScoped<IProductService, MinshpWebApp.Domain.Services.impl.ProductService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<ICustomerService, MinshpWebApp.Domain.Services.impl.CustomerService>();
builder.Services.AddScoped<IProductFeatureService, MinshpWebApp.Domain.Services.impl.ProductFeatureService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IFeatureCategoryService, FeatureCategoryService>();
builder.Services.AddScoped<ITaxeService, TaxeService>();
builder.Services.AddScoped<IPromotionCodeService, MinshpWebApp.Domain.Services.impl.PromotionCodeService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IBillingAddressService, BillingAddressService>();
builder.Services.AddScoped<IDeliveryAddressService, DeliveryAddressService>();
builder.Services.AddScoped<IInvoiceService, MinshpWebApp.Domain.Services.impl.InvoiceService>();
builder.Services.AddScoped<IOrderCustomerProductService, OrderCustomerProductService>();
builder.Services.AddScoped<IShippingProvider, BoxtalProvider>();
builder.Services.AddScoped<IPackageProfilService, PackageProfilService>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryService>();





//scoped builders
builder.Services.AddScoped<IProductViewModelBuilder, ProductViewModelBuilder>();
builder.Services.AddScoped<IImageViewModelBuilder, ImageViewModelBuilder>();
builder.Services.AddScoped<IVideoViewModelBuilder, VideoViewModelBuilder>();
builder.Services.AddScoped<ICategoryViewModelBuilder, CategoryViewModelBuilder>();
builder.Services.AddScoped<IPromotionViewModelBuilder, PromotionViewModelBuilder>();
builder.Services.AddScoped<IFeatureViewModelBuilder, FeatureViewModelBuilder>();
builder.Services.AddScoped<ICustomerViewModelBuilder, CustomerViewModelBuilder>();
builder.Services.AddScoped<IOrderViewModelBuilder, OrderViewModelBuilder>();
builder.Services.AddScoped<IStockViewModelBuilder, StockViewModelBuilder>();
builder.Services.AddScoped<IProductFeatureViewModelBuilder, ProductFeatureViewModelBuilder>();
builder.Services.AddScoped<IFeatureCategoryViewModelBuilder, FeatureCategoryViewModelBuilder>();
builder.Services.AddScoped<ITaxeViewModelBuilder, TaxeViewModelBuilder>();
builder.Services.AddScoped<IPromotionCodeViewModelBuilder, PromotionCodeViewModelBuilder>();
builder.Services.AddScoped<IApplicationViewModelBuilder, ApplicationViewModelBuilder>();
builder.Services.AddScoped<IBillingAddressViewModelBuilder, BillingAddressViewModelBuilder>();
builder.Services.AddScoped<IDeliveryAddressViewModelBuilder, DeliveryAddressViewModelBuilder>();
builder.Services.AddScoped<IInvoiceViewModelBuilder, InvoiceViewModelBuilder>();
builder.Services.AddScoped<IOrderCustomerProductViewModelBuilder, OrderCustomerProductViewModelBuilder>();
builder.Services.AddScoped<IBoxalProviderViewModelBuilder, BoxalProviderViewModelBuilder>();
builder.Services.AddScoped<IPackageProfilViewModelBuilder, PackageProfilViewModelBuilder>();
builder.Services.AddScoped<ISubCategoryViewModelBuilder, SubCategoryViewModelBuilder>();
builder.Services.AddScoped<IMailViewModelBuilder, MailViewModelBuilder>();

//BOXTAL LIVRAISON
// 1) BoxtalOptions ← Shipping:Boxtal
var cfg = builder.Configuration;
builder.Services.Configure<BoxtalOptions>(cfg.GetSection("Shipping:Boxtal"));

// 2) Compléter FromZip/FromCountry depuis Shipping:From
builder.Services.PostConfigure<BoxtalOptions>(opt =>
{
    var from = cfg.GetSection("Shipping:From");
    opt.FromZip = from["Zip"] ?? opt.FromZip;
    opt.FromCountry = from["Country"] ?? opt.FromCountry;
});

// 3) HttpClient v1 (test.envoimoinscher.com)
builder.Services.AddHttpClient("BoxtalV1", (sp, c) =>
{
    var opt = sp.GetRequiredService<IOptions<BoxtalOptions>>().Value;
    c.BaseAddress = new Uri((opt.BaseUrlV1 ?? "").TrimEnd('/') + "/");
    c.DefaultRequestHeaders.UserAgent.ParseAdd("Minshp/1.0");
});

// 4) HttpClient v3 (api.boxtal.build)
builder.Services.AddHttpClient("BoxtalV3", (sp, c) =>
{
    var opt = sp.GetRequiredService<IOptions<BoxtalOptions>>().Value;
    c.BaseAddress = new Uri(opt.BaseUrlV3);
    c.DefaultRequestHeaders.Accept.Clear();
    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    c.DefaultRequestHeaders.UserAgent.ParseAdd("Minshp/1.0");
});
//BOXTAL LIVRAISON

builder.Services.AddMemoryCache();

builder.Services.AddHttpClient();


//stripe 
var stripeSettings = builder.Configuration.GetSection("Stripe").Get<StripeSettings>();
builder.Services.AddSingleton(_ => new StripeClient(stripeSettings.SecretKey));


builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 900; // 900 Mo
});




var app = builder.Build();


//Logger 
app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Une requête a été reçue !");
    return "Hello World!";
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseStaticFiles(); // pour wwwroot si besoin

app.UseCors(builder =>
    builder.WithOrigins("http://localhost:3000","https://bd158b87393d.ngrok.app")
           .AllowAnyHeader()
           .AllowAnyMethod()
);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
