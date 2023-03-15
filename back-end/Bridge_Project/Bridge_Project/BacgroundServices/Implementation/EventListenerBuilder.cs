using Bridge_Project.Data;
using Bridge_Project.Data.Enums;
using Bridge_Project.Data.Models;
using Bridge_Project.EventsDTO;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Text.Json;

namespace Bridge_Project.BacgroundServices.Implementation;

public class EventListenerBuilder
{
    private readonly BridgeContext context;
    private readonly ILogger<BridgeContext> logger;
    private readonly IConfiguration configuration;

    public EventListenerBuilder(BridgeContext context, ILogger<BridgeContext> logger, IConfiguration configuration)
    {
        this.context = context;
        this.logger = logger;
        this.configuration = configuration;
    }

    public async Task CreateEventListenerService(string typeBridge,string abi, string contractAddressPath, CancellationToken stoppingToken)
    {
        logger.LogWarning($"Initialize {typeBridge} service");
        var privateKey = configuration.GetValue(typeof(string), "PrivateKey").ToString();
        var contractAddress = configuration.GetValue(typeof(string), contractAddressPath).ToString();
        var account = new Account(privateKey);
        var web3 = new Web3(account, @"https://sepolia.infura.io/v3/31d722098d4e48929c96519ba339b2d0");

        var contract = web3.Eth.GetContract(abi, contractAddress);
        var lockEventHandler = contract.GetEvent<LockEventDTO>();
        var unlockEventHandler = contract.GetEvent<UnlockEventDTO>();
        var mintEventHandler = contract.GetEvent<MintEventDTO>();
        var burnEventHandler = contract.GetEvent<BurnEventDTO>();

        var filterInput = lockEventHandler.CreateFilterInput();
        var filter = await lockEventHandler.CreateFilterAsync(filterInput);

        var filterInput1 = unlockEventHandler.CreateFilterInput();
        var filter1 = await unlockEventHandler.CreateFilterAsync(filterInput1);

        var filterInput2 = mintEventHandler.CreateFilterInput();
        var filter2 = await mintEventHandler.CreateFilterAsync(filterInput2);

        var filterInput3 = burnEventHandler.CreateFilterInput();
        var filter3 = await burnEventHandler.CreateFilterAsync(filterInput3);
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogWarning("Inside While Loop");

            try
            {
                logger.LogWarning("Collecting tasks");

                var tasks = new List<Task>();

                tasks.Add(ProcessEventChangesAsync(lockEventHandler, filter, HandleLockEvent, stoppingToken));
                tasks.Add(ProcessEventChangesAsync(unlockEventHandler, filter1, HandleUnlockEvent, stoppingToken));
                tasks.Add(ProcessEventChangesAsync(mintEventHandler, filter2, HandleMintEvent, stoppingToken));
                tasks.Add(ProcessEventChangesAsync(burnEventHandler, filter3, HandleBurnEvent, stoppingToken));

                await Task.WhenAll(tasks);

                await context.SaveChangesAsync();
                logger.LogWarning("Changes saved");

            }
            catch (Exception ex)
            {
                logger.LogWarning(ex.Message);
            }

            // If we don't put delay. 88 Line is throwing exception. And the background service stop working.
            await Task.Delay(60000, stoppingToken); // 1min
        }
    }

    private async Task ProcessEventChangesAsync<T>(Event<T> eventHandler, HexBigInteger filter, Func<T, BridgeEvent> handler, CancellationToken cancellationToken) where T : IEventDTO, new()
    {
        var changes = await eventHandler.GetFilterChangesAsync(filter);

        foreach (var change in changes)
        {
            logger.LogWarning("Event catched");
            var @event = handler(change.Event);
            var json = JsonSerializer.Serialize(@event);
            var model = JsonSerializer.Deserialize<BridgeEvent>(json);

            if (model is null)
                throw new ArgumentNullException();

            model.Id = change.Log.TransactionHash;
            await context.BridgeEvents.AddAsync(model, cancellationToken);
        }
    }

    private BridgeEvent HandleLockEvent(LockEventDTO lockEvent)
    {
        var jsonObj = new
        {
            lockEvent.Token,
            lockEvent.Sender,
            Amount = lockEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        return new BridgeEvent
        {
            PublicKeySender = lockEvent.Sender,
            EventType = (int)EventType.TokenLocked,
            RequiresClaiming = true,
            IsClaimed = false,
            JsonData = json
        };
    }

    private BridgeEvent HandleUnlockEvent(UnlockEventDTO unlockEvent)
    {
        var jsonObj = new
        {
            unlockEvent.Token,
            unlockEvent.To,
            Amount = unlockEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        return new BridgeEvent
        {
            EventType = (int)EventType.TokenUnlocked,
            RequiresClaiming = false,
            IsClaimed = false,
            JsonData = json
        };
    }

    private BridgeEvent HandleMintEvent(MintEventDTO mintEvent)
    {
        var jsonObj = new
        {
            mintEvent.Token,
            mintEvent.To,
            Amount = mintEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        return new BridgeEvent
        {
            EventType = (int)EventType.TokenMinted,
            RequiresClaiming = false,
            IsClaimed = false,
            JsonData = json
        };
    }

    private BridgeEvent HandleBurnEvent(BurnEventDTO burnEvent)
    {
        var jsonObj = new
        {
            burnEvent.Token,
            burnEvent.Sender,
            Amount = burnEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        return new BridgeEvent
        {
            PublicKeySender = burnEvent.Sender,
            EventType = (int)EventType.TokenBurned,
            RequiresClaiming = true,
            IsClaimed = false,
            JsonData = json
        };
    }
}
