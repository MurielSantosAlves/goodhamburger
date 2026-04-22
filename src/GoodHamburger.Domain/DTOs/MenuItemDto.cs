using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.DTOs;

public record MenuItemDto(
    int Id,
    string Name,
    decimal Price,
    MenuItemType Type
);
