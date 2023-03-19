using Bridge_Project.Data;
using Bridge_Project.Singleton;
using Microsoft.EntityFrameworkCore;

namespace Bridge_Project.BacgroundServices;

public class EventCheckerService : BackgroundService
{
    private readonly BridgeContext context;
    private readonly ILogger<EventCheckerService> logger;
    private readonly DbContextOptions<BridgeContext> options;

    public EventCheckerService(BridgeContext context, DbContextOptions<BridgeContext> options, ILogger<EventCheckerService> logger)
    {
        this.context = context;
        this.options = options;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogWarning("------------------Start Checking-----------------");

            try
            {
                var tracker = EventTracker.Instance;
                var data = tracker.GetEvents();
                using (var context = new BridgeContext(options))
                {
                    foreach (var eventInfo in data)
                    {
                        var dbEvent = await context.BridgeEvents.AsTracking().FirstOrDefaultAsync(x => x.Id == eventInfo.ForClaimTxHash, stoppingToken);

                        if (dbEvent != null)
                        {
                            dbEvent!.IsClaimed = true;
                            dbEvent!.ClaimedFromId = eventInfo.ClaimedFromTxHASH;
                            eventInfo.Tracked = true;
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }

            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.Message);
            }

            await Task.Delay(10000, stoppingToken); // 1min
        }
    }
}
