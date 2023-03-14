using Bridge_Project.Data.Enums;
using Bridge_Project.Data.Models;

namespace Bridge_Project.Services.Services;

public interface ISourceEventService
{
    Task<IEnumerable<SourceEvent>> GetAllLockEvents(CancellationToken cancellationToken);
    Task<IEnumerable<SourceEvent>> GetAllBurnEvents(CancellationToken cancellationToken);
    Task<IEnumerable<SourceEvent>> GetAllByPublicKey(string publicKey, CancellationToken cancellationToken);
    Task<SourceEvent> GetByTransactionHash(string txHash, CancellationToken cancellationToken);
    Task<IEnumerable<SourceEvent>> GetAllByType(EventType eventType, CancellationToken cancellationToken);
}
