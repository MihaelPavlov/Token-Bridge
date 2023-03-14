using Bridge_Project.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Bridge_Project.Data.Persistence;

public interface IBridgeContext
{
    public DbSet<SourceEvent> SourceEvents { get; set; }
    public DbSet<DestinationEvent> DestinationEvents { get; set; }
    Task<int> SaveChangesAsync();
}
