using FluentValidation;
using GoodHamburger.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Domain.DTOs.Validators;

public class UpdateOrderDtoValidator : AbstractValidator<UpdateOrderDto>
{
    private readonly ApplicationDbContext _context;
    
    public UpdateOrderDtoValidator(ApplicationDbContext context)
    {
        _context = context;
        
        RuleFor(x => x.MenuItemIds)
            .NotEmpty().WithMessage("O pedido deve conter pelo menos um item");
        
        RuleFor(x => x.MenuItemIds)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("O pedido contém itens duplicados");
        
        RuleFor(x => x.MenuItemIds)
            .MustAsync(AllItemsExist).WithMessage("Um ou mais itens do menu não foram encontrados");
        
        RuleFor(x => x.MenuItemIds)
            .MustAsync(NoDuplicateTypes).WithMessage("Cada pedido pode conter apenas um sanduíche, uma batata e um refrigerante");
    }
    
    private async Task<bool> AllItemsExist(List<int> menuItemIds, CancellationToken cancellationToken)
    {
        var existingIds = await _context.MenuItems
            .Where(m => menuItemIds.Contains(m.Id))
            .Select(m => m.Id)
            .ToListAsync(cancellationToken);
        
        return menuItemIds.All(id => existingIds.Contains(id));
    }
    
    private async Task<bool> NoDuplicateTypes(List<int> menuItemIds, CancellationToken cancellationToken)
    {
        var menuItems = await _context.MenuItems
            .Where(m => menuItemIds.Contains(m.Id))
            .ToListAsync(cancellationToken);
        
        var groupedByType = menuItems.GroupBy(m => m.Type);
        
        // Cada tipo pode ter no máximo 1 item
        return groupedByType.All(g => g.Count() == 1);
    }
}
