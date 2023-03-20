using Bridge_Project.Data;
using Bridge_Project.Data.Enums;
using Bridge_Project.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Bridge_Project.Services.Services;

public class SourceEventService : ISourceEventService
{
    private readonly BridgeContext context;
    private readonly ILogger<BridgeContext> logger;

    public SourceEventService(BridgeContext context, ILogger<BridgeContext> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<IEnumerable<BridgeEvent>> GetAll(CancellationToken cancellationToken)
    {
        return await this.context.BridgeEvents.Include(x => x.ClaimedFrom).Where(x => x.ClaimedFromId != null).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BridgeEvent>> GetAllByPublicKey(string publicKey, CancellationToken cancellationToken)
    {
        return await this.context.BridgeEvents.Include(x => x.ClaimedFrom).Where(x => x.ClaimedFromId != null && x.PublicKeySender == publicKey).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BridgeEvent>> GetAllEventsForClaiming(CancellationToken cancellationToken)
    {
        return await this.context.BridgeEvents.Where(x => x.RequiresClaiming == true && x.IsClaimed == false && x.EventType == (int)EventType.TokenLocked).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BridgeEvent>> GetAllEventsForReleasing(CancellationToken cancellationToken)
    {
        return await this.context.BridgeEvents.Where(x => x.RequiresClaiming == true && x.IsClaimed == false && x.EventType == (int)EventType.TokenBurned).ToListAsync(cancellationToken);
    }
}
