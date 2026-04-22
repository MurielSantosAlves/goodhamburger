using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Domain.Services;

public interface IDiscountCalculator
{
    decimal CalculateDiscountPercentage(Order order);
}
