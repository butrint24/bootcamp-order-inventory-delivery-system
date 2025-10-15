using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;

public class Order
{
    public Guid OrderId { get; set; } = Guid.NewGuid();

    public decimal Price { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.PENDING;

    public string Address { get; set; } = null!;

    public Guid User_Id { get; set; }

    public List<OrderItem> Items { get; set; } = new();

    public Order() { }

    public Order(
        Guid userId,
        string address
    )
    {
        User_Id = userId;
        Address = address;
    }

    public void AddItem(
        Guid productId,
        int qty,
        decimal unitPrice
    )
    {
        if (qty <= 0)
            throw new ArgumentOutOfRangeException(nameof(qty));

        Items.Add(new OrderItem(OrderId, productId, qty, unitPrice));
        Recalculate();
    }

    public void MarkProcessing() => Status = OrderStatus.PROCESSING;

    public void MarkCompleted() => Status = OrderStatus.COMPLETED;

    public void MarkCancelled() => Status = OrderStatus.CANCELLED;

    public void Confirm() => Status = OrderStatus.CONFIRMED;

    private void Recalculate()
        => Price = Items.Sum(i => i.LineTotal);
}
