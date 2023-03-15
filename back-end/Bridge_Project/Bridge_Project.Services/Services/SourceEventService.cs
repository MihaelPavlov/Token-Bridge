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

    public async Task<IEnumerable<BridgeEvent>> GetAllLockEvents(CancellationToken cancellationToken) => await this.context.BridgeEvents.Where(x => !x.IsClaimed && x.EventType == (int)EventType.TokenLocked).ToListAsync(cancellationToken);
    public async Task<IEnumerable<BridgeEvent>> GetAllBurnEvents(CancellationToken cancellationToken) => await this.context.BridgeEvents.Where(x => !x.IsClaimed && x.EventType == (int)EventType.TokenBurned).ToListAsync(cancellationToken);

    public async Task<IEnumerable<BridgeEvent>> GetAllByPublicKey(string publicKey, CancellationToken cancellationToken) => await this.context.BridgeEvents.Where(x => !x.IsClaimed && x.EventType == (int)EventType.TokenLocked && x.PublicKeySender == publicKey).ToListAsync(cancellationToken);

    public async Task<IEnumerable<BridgeEvent>> GetAllByType(EventType eventType, CancellationToken cancellationToken) => await this.context.BridgeEvents.Where(x => x.EventType == (int)eventType).ToListAsync(cancellationToken);

    public async Task<BridgeEvent> GetByTransactionHash(string txHash, CancellationToken cancellationToken) => await this.context.BridgeEvents.FirstAsync(x => x.Id == txHash, cancellationToken);

    public async Task<string> GetLastBlockNumberFromEvents(CancellationToken cancellationToken)
    {
        var result = await this.context.BridgeEvents.OrderByDescending(x => x.CreatedDate).FirstAsync(cancellationToken);

        return result.BlockNumber;
    }

    public async Task<bool> IsEventSaved(string txHash, CancellationToken cancellationToken)
    {
        return await this.context.BridgeEvents.AnyAsync(x => x.Id == txHash, cancellationToken);
    }
}
