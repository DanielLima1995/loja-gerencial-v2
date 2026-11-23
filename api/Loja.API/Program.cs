using Loja.Application.Interfaces;
using Loja.Application.Services;
using Loja.Domain.Entities;
using Loja.Domain.Interfaces;
using Loja.Infrastructure.Context;
using Loja.Infrastructure.Repositories;
using Loja.Infrastructure.Services;
using Loja.API.Messaging;
using Amazon.S3;
using Amazon;
using Amazon.Runtime;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? Array.Empty<string>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ==========================================
// 2. Autenticação e JWT
// ==========================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddMemoryCache();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // true Em produção 
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>();
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (cache.TryGetValue(token, out _))
            {
                context.Fail("Este token foi revogado.");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ==========================================
// AWS S3 Configuration
// ==========================================
var storageSettings = builder.Configuration.GetSection("StorageSettings:S3");
var regionName = builder.Configuration["StorageSettings:Region"]
    ?? storageSettings["RegionEndpoint"]
    ?? "us-east-1";
var region = RegionEndpoint.GetBySystemName(regionName);
var awsAccessKeyId = builder.Configuration["AWS:AccessKeyId"];
var awsSecretAccessKey = builder.Configuration["AWS:SecretAccessKey"];

if (string.IsNullOrWhiteSpace(awsAccessKeyId) != string.IsNullOrWhiteSpace(awsSecretAccessKey))
{
    throw new InvalidOperationException(
        "Configure AWS:AccessKeyId e AWS:SecretAccessKey juntos, ou use a cadeia padrão de credenciais da AWS.");
}

builder.Services.AddSingleton<IAmazonS3>(_ =>
    string.IsNullOrWhiteSpace(awsAccessKeyId)
        ? new AmazonS3Client(region)
        : new AmazonS3Client(new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey), region));

builder.Services.AddScoped<IStorageProvider, S3StorageProvider>();

builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IProdutoImagensRepository, ProdutoImagensRepository>();

// Serviços
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IProdutoImagensService, ProdutoImagensService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ITagsRepository, TagsRepository>();
builder.Services.AddScoped<ITagsService, TagsService>();

builder.Services.AddScoped<ITagsProdutosRepository, TagsProdutosRepository>();
builder.Services.AddScoped<ITagsProdutosService, TagsProdutosService>();

builder.Services.AddScoped<IClienteEnderecosRepository, ClienteEnderecosRepository>();
builder.Services.AddScoped<IClienteEnderecosService, ClienteEnderecosService>();

builder.Services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();
builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();

builder.Services.AddScoped<ICarrinhoProdutosRepository, CarrinhoProdutosRepository>();
builder.Services.AddScoped<ICarrinhoProdutosService, CarrinhoProdutosService>();

builder.Services.AddScoped<IVendasRepository, VendasRepository>();
builder.Services.AddScoped<IVendasService, VendasService>();
builder.Services.AddSingleton<IVendasStatusQueue, RabbitVendasStatusQueue>();
builder.Services.AddHostedService<VendasStatusConsumer>();

builder.Services.AddScoped<IVendasProdutosRepository, VendasProdutosRepository>();
builder.Services.AddScoped<IVendasProdutosService, VendasProdutosService>();

builder.Services.AddScoped<IVendasStatusRepository, VendasStatusRepository>();
builder.Services.AddScoped<IVendasStatusService, VendasStatusService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;

    // Ignora referências circulares
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(); 

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT."
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            [new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }] = new List<string>()
        });
    });



    var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "A Minha API v1");
    });

    // app.MapOpenApi(); 
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
