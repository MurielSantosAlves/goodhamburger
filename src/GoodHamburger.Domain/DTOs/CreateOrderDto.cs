namespace GoodHamburger.Domain.DTOs;

public record CreateOrderDto(
    List<int> MenuItemIds
);
