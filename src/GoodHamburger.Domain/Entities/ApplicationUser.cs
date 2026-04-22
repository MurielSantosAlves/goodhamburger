using Microsoft.AspNetCore.Identity;

namespace GoodHamburger.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
