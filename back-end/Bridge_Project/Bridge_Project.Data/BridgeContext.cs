using Bridge_Project.Data.Models;
using Bridge_Project.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bridge_Project.Data;

public class BridgeContext : DbContext, IBridgeContext
{
    public BridgeContext(DbContextOptions<BridgeContext> options)
        :base(options)
    {
    }

    public DbSet<SourceEvent> SourceEvents { get; set; }
    public DbSet<DestinationEvent> DestinationEvents { get; set; }

    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
}
