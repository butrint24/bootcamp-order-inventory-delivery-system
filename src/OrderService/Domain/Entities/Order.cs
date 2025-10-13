using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities;
public class Order
{
    public Guid OrderId { get; private set; } = Guid.NewGuid();
    public decimal Price { get; private set; }           // total price persistuar
    public OrderStatus Status { get; private set; } = OrderStatus.PENDING;
    public string Address { get; private set; } = null!;
    public Guid User_Id { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { }
    public Order(Guid userId, string address)
    { User_Id = userId; Address = address; }

    public void AddItem(Guid productId, int qty, decimal unitPrice)
    {
        if (qty <= 0) throw new ArgumentOutOfRangeException(nameof(qty));
        _items.Add(new OrderItem(OrderId, productId, qty, unitPrice));
        Recalculate();
    }

    public void MarkProcessing() => Status = OrderStatus.PROCESSING;
    public void MarkCompleted() => Status = OrderStatus.COMPLETED;
    public void MarkCancelled() => Status = OrderStatus.CANCELLED;
    public void Confirm() => Status = OrderStatus.CONFIRMED;

    private void Recalculate() => Price = _items.Sum(i => i.LineTotal);
}
