namespace Shared.Models
{
    public class ProblemDetailsResponse
    {
        public string Type { get; set; } = default!;
        public string Title { get; set; } = default!;
        public int Status { get; set; }
        public string Detail { get; set; } = default!;
        public string Instance { get; set; } = default!;
        public string? TraceId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public object? Errors { get; set; }
        public string? User { get; set; }
    }
}
