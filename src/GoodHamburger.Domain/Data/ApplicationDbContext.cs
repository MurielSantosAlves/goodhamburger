using GoodHamburger.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Domain.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<DiscountRule> DiscountRules => Set<DiscountRule>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // configuração da tabela de itens do menu
        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Type).IsRequired();
        });
        
        // configuração da tabela de pedidos
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.OrderDate).IsRequired();
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // propriedades calculadas não vão pro banco
            entity.Ignore(e => e.SubTotal);
            entity.Ignore(e => e.DiscountPercentage);
            entity.Ignore(e => e.DiscountAmount);
            entity.Ignore(e => e.Total);
        });
        
        // Configuração de OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            
            entity.HasOne(e => e.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.MenuItem)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(e => e.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // dados iniciais do cardápio
        modelBuilder.Entity<MenuItem>().HasData(
            new MenuItem { Id = 1, Name = "X Burger", Price = 5.00m, Type = GoodHamburger.Domain.Enums.MenuItemType.Sandwich },
            new MenuItem { Id = 2, Name = "X Egg", Price = 4.50m, Type = GoodHamburger.Domain.Enums.MenuItemType.Sandwich },
            new MenuItem { Id = 3, Name = "X Bacon", Price = 7.00m, Type = GoodHamburger.Domain.Enums.MenuItemType.Sandwich },
            new MenuItem { Id = 4, Name = "Batata frita", Price = 2.00m, Type = GoodHamburger.Domain.Enums.MenuItemType.SideDish },
            new MenuItem { Id = 5, Name = "Refrigerante", Price = 2.50m, Type = GoodHamburger.Domain.Enums.MenuItemType.Drink }
        );
        
        // Configuração de DiscountRule
        modelBuilder.Entity<DiscountRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DiscountPercentage).HasPrecision(5, 4);
            entity.Property(e => e.IsActive).IsRequired();
            entity.Property(e => e.Priority).IsRequired();
        });
        
        // Dados iniciais de regras de desconto
        modelBuilder.Entity<DiscountRule>().HasData(
            new DiscountRule 
            { 
                Id = 1, 
                Name = "Combo Completo", 
                Description = "Sanduíche + Batata + Refrigerante", 
                DiscountPercentage = 0.20m, 
                RequiresSandwich = true, 
                RequiresSideDish = true, 
                RequiresDrink = true, 
                IsActive = true, 
                Priority = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new DiscountRule 
            { 
                Id = 2, 
                Name = "Combo Bebida", 
                Description = "Sanduíche + Refrigerante", 
                DiscountPercentage = 0.15m, 
                RequiresSandwich = true, 
                RequiresSideDish = false, 
                RequiresDrink = true, 
                IsActive = true, 
                Priority = 2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new DiscountRule 
            { 
                Id = 3, 
                Name = "Combo Batata", 
                Description = "Sanduíche + Batata Frita", 
                DiscountPercentage = 0.10m, 
                RequiresSandwich = true, 
                RequiresSideDish = true, 
                RequiresDrink = false, 
                IsActive = true, 
                Priority = 3,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
