namespace OrderService.Domain.Entities;
public class OrderItem
{
    public Guid OrderItemId { get; private set; } = Guid.NewGuid();
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal LineTotal => UnitPrice * Quantity;

    private OrderItem() { }
    public OrderItem(Guid orderId, Guid productId, int qty, decimal unitPrice)
    { OrderId = orderId; ProductId = productId; Quantity = qty; UnitPrice = unitPrice; }
}
