public class ReturnOrderResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? OrderId { get; set; }
    public string? UpdatedStatus { get; set; }
}
