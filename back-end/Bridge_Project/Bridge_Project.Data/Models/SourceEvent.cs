namespace Bridge_Project.Data.Models;

public class SourceEvent
{
    public string Id { get; set; } = null!;
    public string? PublicKeySender { get; set; }
    public string JsonData { get; set; } = null!;
    public int EventType { get; set; }
    public bool RequiresClaiming { get; set; }
    public bool IsClaimed { get; set; }
}