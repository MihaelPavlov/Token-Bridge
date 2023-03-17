namespace Bridge_Project.Data.Models;

public class BridgeEvent
{
    public string Id { get; set; } = null!;
    public string? PublicKeySender { get; set; }
    public string JsonData { get; set; } = null!;
    public int EventType { get; set; }
    public bool RequiresClaiming { get; set; }
    public bool IsClaimed { get; set; }
    public string BlockNumber { get; set; } = null!;
    public string ChainName { get; set; } = null!;
    public DateTime CreatedDate { get; set; }

    public string? ClaimedFromId { get; set; }
    public BridgeEvent? ClaimedFrom { get; set; }
}