using System.Text;
using FluentValidation;
using GoodHamburger.Domain.Data;
using GoodHamburger.Domain.DTOs.Validators;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Repositories;
using GoodHamburger.Domain.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do Identity com regras de senha
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false; // caracteres especiais não obrigatórios
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddErrorDescriber<PortugueseIdentityErrorDescriber>() // mensagens de erro em português
.AddDefaultTokenProviders();

// Autenticação JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// controladores e rotas
builder.Services.AddControllers();

// validação com FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

// injeção de dependências dos serviços
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IDiscountRuleRepository, DiscountRuleRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IDiscountCalculator, DiscountCalculator>();

// documentação Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Good Hamburger API",
        Version = "v1",
        Description = "API para gerenciamento de pedidos da lanchonete Good Hamburger"
    });
    
    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS para permitir chamadas do Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("http://localhost:8080", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// aplica migrations e cria dados iniciais
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        
        // cria usuário admin padrão
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var adminEmail = "admin@goodhamburger.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                Console.WriteLine("Admin user created successfully");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating or seeding the database: {ex.Message}");
    }
}

// configuração do pipeline de requisições
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Good Hamburger API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowBlazor");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class public for integration tests
public partial class Program { }
