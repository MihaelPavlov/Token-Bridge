using Bridge_Project.Data.Models;

namespace Bridge_Project.Services.Services;

public interface ISourceEventService
{
    Task<IEnumerable<BridgeEvent>> GetAllEventsForClaiming(CancellationToken cancellationToken);
    Task<IEnumerable<BridgeEvent>> GetAllEventsForReleasing(CancellationToken cancellationToken);
    Task<IEnumerable<BridgeEvent>> GetAllByPublicKey(string publicKey, CancellationToken cancellationToken);
    Task<IEnumerable<BridgeEvent>> GetAll( CancellationToken cancellationToken);
}
