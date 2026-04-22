using GoodHamburger.Domain.Data;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;

namespace GoodHamburger.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove all database-related services
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.RemoveAll(typeof(DbContextOptions));
            services.RemoveAll<ApplicationDbContext>();
            
            // Remove any lingering database provider registrations
            var descriptorsToRemove = services
                .Where(d => d.ServiceType.Name.Contains("DbContextOptions") || 
                           d.ServiceType.Name.Contains("ApplicationDbContext") ||
                           d.ServiceType.Name.Contains("Npgsql"))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
            {
                services.Remove(descriptor);
            }

            // Add ApplicationDbContext using in-memory database for testing
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("InMemoryTestDb"));
        });

        builder.ConfigureServices(services =>
        {
            // Build a new service provider for testing
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                // Ensure the database is created with migrations
                db.Database.EnsureCreated();

                // Seed menu items
                if (!db.MenuItems.Any())
                {
                    db.MenuItems.AddRange(new[]
                    {
                        new MenuItem { Id = 1, Name = "X Burger", Price = 5.00m, Type = MenuItemType.Sandwich },
                        new MenuItem { Id = 2, Name = "X Egg", Price = 4.50m, Type = MenuItemType.Sandwich },
                        new MenuItem { Id = 3, Name = "X Bacon", Price = 7.00m, Type = MenuItemType.Sandwich },
                        new MenuItem { Id = 4, Name = "Batata frita", Price = 2.00m, Type = MenuItemType.SideDish },
                        new MenuItem { Id = 5, Name = "Refrigerante", Price = 2.50m, Type = MenuItemType.Drink }
                    });
                    db.SaveChanges();
                }
            }
        });
    }
}
