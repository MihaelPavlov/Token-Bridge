using Bridge_Project.Data.Enums;
using Bridge_Project.Data.Models;

namespace Bridge_Project.Services.Services;

public interface ISourceEventService
{
    Task<IEnumerable<BridgeEvent>> GetAllLockEvents(CancellationToken cancellationToken);
    Task<IEnumerable<BridgeEvent>> GetAllBurnEvents(CancellationToken cancellationToken);
    Task<IEnumerable<BridgeEvent>> GetAllByPublicKey(string publicKey, CancellationToken cancellationToken);
    Task<BridgeEvent> GetByTransactionHash(string txHash, CancellationToken cancellationToken);
    Task<IEnumerable<BridgeEvent>> GetAllByType(EventType eventType, CancellationToken cancellationToken);
    Task<string> GetLastBlockNumberFromEvents(CancellationToken cancellationToken);
    Task<bool> IsEventSaved(string txHash, CancellationToken cancellationToken);
}
