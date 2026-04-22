namespace GoodHamburger.Domain.DTOs;

public record UpdateOrderDto(
    List<int> MenuItemIds
);
