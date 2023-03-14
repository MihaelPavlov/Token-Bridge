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

    public async Task<IEnumerable<SourceEvent>> GetAllLockEvents(CancellationToken cancellationToken) => await this.context.SourceEvents.Where(x => !x.IsClaimed && x.EventType == (int)EventType.TokenLocked).ToListAsync(cancellationToken);
    public async Task<IEnumerable<SourceEvent>> GetAllBurnEvents(CancellationToken cancellationToken) => await this.context.SourceEvents.Where(x => !x.IsClaimed && x.EventType == (int)EventType.TokenBurned).ToListAsync(cancellationToken);

    public async Task<IEnumerable<SourceEvent>> GetAllByPublicKey(string publicKey, CancellationToken cancellationToken) => await this.context.SourceEvents.Where(x => !x.IsClaimed && x.EventType == (int)EventType.TokenLocked && x.PublicKeySender == publicKey).ToListAsync(cancellationToken);

    public async Task<IEnumerable<SourceEvent>> GetAllByType(EventType eventType, CancellationToken cancellationToken) => await this.context.SourceEvents.Where(x => x.EventType == (int)eventType).ToListAsync(cancellationToken);

    public async Task<SourceEvent> GetByTransactionHash(string txHash, CancellationToken cancellationToken) => await this.context.SourceEvents.FirstAsync(x => x.Id == txHash,cancellationToken);
}
