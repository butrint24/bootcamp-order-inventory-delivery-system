using InventoryService.Domain.Enums;

namespace InventoryService.Domain.Entities;
public class Product
{
    public Guid ProductId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = null!;
    public int Stock { get; private set; }
    public string Origin { get; private set; } = null!;
    public Category Category { get; private set; }
    public decimal Price { get; private set; }

    private Product() { }
    public Product(string name, int stock, string origin, Category category, decimal price)
    { Name = name; Stock = stock; Origin = origin; Category = category; Price = price; }

    public bool CanReserve(int qty) => qty > 0 && Stock >= qty;
    public void Reserve(int qty) { if (!CanReserve(qty)) throw new InvalidOperationException("Not enough stock"); Stock -= qty; }
    public void Release(int qty) { if (qty > 0) Stock += qty; }
}
