using Bridge_Project.Data.Enums;
using Bridge_Project.Data.Models;
using Bridge_Project.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Bridge_Project.Services.Services;

internal class DestinationEventService : IDestinationEventService
{
    private readonly BridgeContext context;
    private readonly ILogger<BridgeContext> logger;

    public DestinationEventService(BridgeContext context, ILogger<BridgeContext> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public async Task<IEnumerable<DestinationEvent>> GetAllLockEvents(CancellationToken cancellationToken) => await this.context.DestinationEvents.Where(x => !x.IsClaimed && x.EventType == (int)EventType.TokenLocked).ToListAsync(cancellationToken);
    public async Task<IEnumerable<DestinationEvent>> GetAllBurnEvents(CancellationToken cancellationToken) => await this.context.DestinationEvents.Where(x => !x.IsClaimed && x.EventType == (int)EventType.TokenBurned).ToListAsync(cancellationToken);

    public async Task<IEnumerable<DestinationEvent>> GetAllByPublicKey(string publicKey, CancellationToken cancellationToken) => await this.context.DestinationEvents.Where(x => !x.IsClaimed && x.EventType == (int)EventType.TokenLocked && x.PublicKeySender == publicKey).ToListAsync(cancellationToken);

    public async Task<IEnumerable<DestinationEvent>> GetAllByType(EventType eventType, CancellationToken cancellationToken) => await this.context.DestinationEvents.Where(x => x.EventType == (int)eventType).ToListAsync(cancellationToken);

    public async Task<DestinationEvent> GetByTransactionHash(string txHash, CancellationToken cancellationToken) => await this.context.DestinationEvents.FirstAsync(x => x.Id == txHash, cancellationToken);
}
