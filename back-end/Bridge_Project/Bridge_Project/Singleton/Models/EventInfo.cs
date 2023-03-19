namespace Bridge_Project.Singleton.Models;

public class EventInfo
{
    public string ForClaimTxHash { get; set; } = null!;
    public string ClaimedFromTxHASH { get; set; } = null!;
    public bool Tracked { get; set; } = false;

}