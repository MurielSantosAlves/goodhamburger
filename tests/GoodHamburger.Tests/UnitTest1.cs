using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Services;

namespace GoodHamburger.Tests;

public class DiscountCalculatorTests
{
    private readonly DiscountCalculator _calculator = new();
    
    [Fact]
    public void CalculateDiscountPercentage_WhenNoItems_ShouldReturn0()
    {
        // Arrange
        var order = new Order { Items = new List<OrderItem>() };
        
        // Act
        var discount = _calculator.CalculateDiscountPercentage(order);
        
        // Assert
        Assert.Equal(0m, discount);
    }
    
    [Fact]
    public void CalculateDiscountPercentage_WhenOnlySandwich_ShouldReturn0()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.Sandwich },
                    Price = 10m
                }
            }
        };
        
        // Act
        var discount = _calculator.CalculateDiscountPercentage(order);
        
        // Assert
        Assert.Equal(0m, discount);
    }
    
    [Fact]
    public void CalculateDiscountPercentage_WhenSandwichAndFries_ShouldReturn10Percent()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.Sandwich },
                    Price = 10m
                },
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.SideDish },
                    Price = 5m
                }
            }
        };
        
        // Act
        var discount = _calculator.CalculateDiscountPercentage(order);
        
        // Assert
        Assert.Equal(0.10m, discount);
    }
    
    [Fact]
    public void CalculateDiscountPercentage_WhenSandwichAndDrink_ShouldReturn15Percent()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.Sandwich },
                    Price = 10m
                },
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.Drink },
                    Price = 4m
                }
            }
        };
        
        // Act
        var discount = _calculator.CalculateDiscountPercentage(order);
        
        // Assert
        Assert.Equal(0.15m, discount);
    }
    
    [Fact]
    public void CalculateDiscountPercentage_WhenFullCombo_ShouldReturn20Percent()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.Sandwich },
                    Price = 10m
                },
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.SideDish },
                    Price = 5m
                },
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.Drink },
                    Price = 4m
                }
            }
        };
        
        // Act
        var discount = _calculator.CalculateDiscountPercentage(order);
        
        // Assert
        Assert.Equal(0.20m, discount);
    }
}

public class OrderCalculationTests
{
    [Fact]
    public void Order_SubTotal_ShouldBeSumOfAllItems()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem { Price = 10m },
                new OrderItem { Price = 5m },
                new OrderItem { Price = 4m }
            }
        };
        
        // Act
        var subtotal = order.SubTotal;
        
        // Assert
        Assert.Equal(19m, subtotal);
    }
    
    [Fact]
    public void Order_WithFullCombo_ShouldApply20PercentDiscount()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.Sandwich },
                    Price = 10m
                },
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.SideDish },
                    Price = 5m
                },
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.Drink },
                    Price = 4m
                }
            }
        };
        
        // Act
        var discountPercentage = order.DiscountPercentage;
        var discountAmount = order.DiscountAmount;
        var total = order.Total;
        
        // Assert
        Assert.Equal(0.20m, discountPercentage);
        Assert.Equal(3.8m, discountAmount); // 19 * 0.20
        Assert.Equal(15.2m, total); // 19 - 3.8
    }
    
    [Fact]
    public void Order_WithSandwichAndDrink_ShouldApply15PercentDiscount()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.Sandwich },
                    Price = 10m
                },
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.Drink },
                    Price = 4m
                }
            }
        };
        
        // Act
        var discountPercentage = order.DiscountPercentage;
        var discountAmount = order.DiscountAmount;
        var total = order.Total;
        
        // Assert
        Assert.Equal(0.15m, discountPercentage);
        Assert.Equal(2.1m, discountAmount); // 14 * 0.15
        Assert.Equal(11.9m, total); // 14 - 2.1
    }
    
    [Fact]
    public void Order_WithNoCombo_ShouldNotApplyDiscount()
    {
        // Arrange
        var order = new Order
        {
            Items = new List<OrderItem>
            {
                new OrderItem 
                { 
                    MenuItem = new MenuItem { Type = MenuItemType.Drink },
                    Price = 4m
                }
            }
        };
        
        // Act
        var discountPercentage = order.DiscountPercentage;
        var discountAmount = order.DiscountAmount;
        var total = order.Total;
        
        // Assert
        Assert.Equal(0m, discountPercentage);
        Assert.Equal(0m, discountAmount);
        Assert.Equal(4m, total);
    }
}


