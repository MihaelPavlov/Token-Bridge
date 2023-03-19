using Bridge_Project.Singleton.Models;

namespace Bridge_Project.Singleton;

public sealed class EventTracker
{
    private static EventTracker instance = null!;
    private static readonly object padlock = new object();
    private ICollection<EventInfo> data;

    public EventTracker()
    {
        data = new HashSet<EventInfo>();
    }

    public static EventTracker Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new EventTracker();
                }
                return instance;
            }
        }
    }

    public void AddEvent(string forClaimTxHash, string claimedFromTxHash)
    {
        data.Add(new EventInfo { ForClaimTxHash = forClaimTxHash, ClaimedFromTxHASH = claimedFromTxHash });
    }

    public ICollection<EventInfo> GetEvents()
    {
        return data.Where(x => x.Tracked == false).ToList();
    }
}
