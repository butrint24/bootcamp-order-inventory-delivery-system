namespace OrderService.Domain.Entities;

public class OrderItem
{
    public Guid OrderItemId { get; set; } = Guid.NewGuid();

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal => UnitPrice * Quantity;

    public OrderItem() { }

    public OrderItem(
        Guid orderId,
        Guid productId,
        int qty,
        decimal unitPrice
    )
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = qty;
        UnitPrice = unitPrice;
    }
}
