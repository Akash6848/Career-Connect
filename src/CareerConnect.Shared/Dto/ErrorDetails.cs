namespace CareerConnect.Shared.Dto;

public class ErrorDetails
{
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public int StatusCode { get; set; }
}
