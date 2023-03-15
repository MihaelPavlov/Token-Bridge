using Bridge_Project.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bridge_Project.Data.Persistence;

public interface IBridgeContext
{
    public DbSet<BridgeEvent> BridgeEvents { get; set; }

    Task<int> SaveChangesAsync();
}
