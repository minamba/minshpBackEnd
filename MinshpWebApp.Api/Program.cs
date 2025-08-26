using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using MinshpWebApp.Api.Builders;
using MinshpWebApp.Api.Builders.impl;
using MinshpWebApp.Dal.Entities;
using MinshpWebApp.Dal.Repositories;
using MinshpWebApp.Domain.Repositories;
using MinshpWebApp.Domain.Services;
using MinshpWebApp.Domain.Services.impl;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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


//scoped services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IProductFeatureService, ProductFeatureService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IFeatureCategoryService, FeatureCategoryService>();
builder.Services.AddScoped<ITaxeService, TaxeService>();
builder.Services.AddScoped<IPromotionCodeService, PromotionCodeService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IBillingAddressService, BillingAddressService>();
builder.Services.AddScoped<IDeliveryAddressService, DeliveryAddressService>();



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


builder.Services.AddHttpClient();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 900; // 900 Mo
});




var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
