using Bridge_Project.Data.Enums;
using Bridge_Project.Data.Models;

namespace Bridge_Project.Services.Services;

public interface IDestinationEventService
{
    Task<IEnumerable<DestinationEvent>> GetAllLockEvents(CancellationToken cancellationToken);
    Task<IEnumerable<DestinationEvent>> GetAllBurnEvents(CancellationToken cancellationToken);
    Task<IEnumerable<DestinationEvent>> GetAllByPublicKey(string publicKey, CancellationToken cancellationToken);
    Task<DestinationEvent> GetByTransactionHash(string txHash, CancellationToken cancellationToken);
    Task<IEnumerable<DestinationEvent>> GetAllByType(EventType eventType, CancellationToken cancellationToken);
}
