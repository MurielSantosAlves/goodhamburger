namespace GoodHamburger.Domain.DTOs;

public record OrderItemDto(
    int Id,
    string MenuItemName,
    decimal Price
);

public record OrderResponseDto(
    int Id,
    DateTime OrderDate,
    List<OrderItemDto> Items,
    decimal SubTotal,
    decimal DiscountPercentage,
    decimal DiscountAmount,
    decimal Total
);
