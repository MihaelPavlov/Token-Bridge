using Bridge_Project.Data;
using Bridge_Project.Data.Enums;
using Bridge_Project.Data.Models;
using Bridge_Project.EventsDTO;
using Microsoft.EntityFrameworkCore;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;
using System.Text.Json;

namespace Bridge_Project.BacgroundServices;

public class SourceEventsListenerService : BackgroundService
{
    private const string Abi = @"
""[	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""burn\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""lock\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""message\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""LogMessage\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			},			{				\""internalType\"": \""string\"",				\""name\"": \""name\"",				\""type\"": \""string\""			},			{				\""internalType\"": \""string\"",				\""name\"": \""symbol\"",				\""type\"": \""string\""			}		],		\""name\"": \""mint\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""mint\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			}		],		\""name\"": \""setWrappedToSourceToken\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""sender\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenBurned\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""sender\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenLocked\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""to\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenMinted\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""to\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenUnlocked\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""originalToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""unlock\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""originalToken\"",				\""type\"": \""address\""			}		],		\""name\"": \""WrappedTokenCreated\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			}		],		\""name\"": \""getBytes\"",		\""outputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""\"",				\""type\"": \""bytes32\""			}		],		\""stateMutability\"": \""pure\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""bytes32\"",				\""name\"": \""\"",				\""type\"": \""bytes32\""			}		],		\""name\"": \""lockedTokens\"",		\""outputs\"": [			{				\""internalType\"": \""uint256\"",				\""name\"": \""\"",				\""type\"": \""uint256\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""name\"": \""sourceToWrappedTokenMap\"",		\""outputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""name\"": \""wrappedToSourceTokenMap\"",		\""outputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	}]""
";
    private readonly BridgeContext context;
    private readonly ILogger<BridgeContext> logger;
    private readonly IConfiguration configuration;
    private readonly DbContextOptions<BridgeContext> _options;

    public SourceEventsListenerService(BridgeContext context, ILogger<BridgeContext> logger, IConfiguration configuration, DbContextOptions<BridgeContext> options)
    {
        this.context = context;
        this.logger = logger;
        this.configuration = configuration;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogWarning("Initialize Source Service");
        var privateKey = configuration.GetValue(typeof(string), "PrivateKey").ToString();
        var contractAddress = configuration.GetValue(typeof(string), "SourceBridgeContractAddress").ToString();
        var account = new Account(privateKey);
        var web3 = new Web3(account, @"https://sepolia.infura.io/v3/31d722098d4e48929c96519ba339b2d0");

        var contract = web3.Eth.GetContract(Abi, contractAddress);
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

        await CheckForEvents(web3, new List<NewFilterInput> { filterInput, filterInput1, filterInput2, filterInput3 }, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogWarning("Inside While Loop");

            try
            {
                logger.LogWarning("Collecting Tasks");

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

            await Task.Delay(10000, stoppingToken); // 1min
        }
    }

    private async Task CheckForEvents(Web3 web3, List<NewFilterInput> filterInputs, CancellationToken cancellationToken)
    {
        var latestBlock = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

        List<FilterLog> result = new List<FilterLog>(); ;
        BridgeEvent lastSavedBlockNumber;

        var count = 0;
        using (var context = new BridgeContext(_options))
        {
            count = await context.BridgeEvents.Where(x=>x.ChainName == "Ethereum").CountAsync(cancellationToken);
        }

        if (count != 0)
        {
            // if we don't have records we can maybe have the bridge deployed block number
            using (var context = new BridgeContext(_options))
            {
                lastSavedBlockNumber = await context.BridgeEvents.Where(x=>x.ChainName == "Ethereum").OrderByDescending(x => x.BlockNumber).FirstAsync(cancellationToken);
            }

            var batchSize = 5000;
            var fromBlock = new BlockParameter(new HexBigInteger(BigInteger.Parse(lastSavedBlockNumber.BlockNumber)));
            for (var i = fromBlock.BlockNumber; i <= latestBlock.Value; i.Value += batchSize)
            {
                var batchEnd = i.Value + batchSize - 1;
                if (batchEnd > latestBlock.Value)
                {
                    batchEnd = latestBlock.Value;
                }

                Console.WriteLine($"From block {fromBlock.BlockNumber} to {batchEnd}");
                foreach (var filter in filterInputs)
                {
                    filter.FromBlock = fromBlock;
                    filter.ToBlock = new BlockParameter(batchEnd.ToHexBigInteger());
                    var logs = await web3.Eth.Filters.GetLogs.SendRequestAsync(filter);
                    result.AddRange(logs);
                    Console.WriteLine($"Count events -> {logs.Count()}");

                }
            }

            foreach (var item in result)
            {
                bool isEventSaved = false;
                using (var context = new BridgeContext(_options))
                {
                    isEventSaved = await this.context.BridgeEvents.AnyAsync(x => x.Id == item.TransactionHash, cancellationToken);
                }

                if (!isEventSaved)
                {
                    if (item.IsLogForEvent<LockEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<LockEventDTO>();

                        var newEvent = await HandleLockEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, cancellationToken);

                        Console.WriteLine("LockEvent");
                    }
                    else if (item.IsLogForEvent<UnlockEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<UnlockEventDTO>();

                        var newEvent = await HandleUnlockEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, cancellationToken);

                        Console.WriteLine("UnlockEvent");
                    }
                    else if (item.IsLogForEvent<MintEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<MintEventDTO>();

                        var newEvent = await HandleMintEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, cancellationToken);

                        Console.WriteLine("MintEvent");
                    }
                    else if (item.IsLogForEvent<BurnEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<BurnEventDTO>();

                        var newEvent = await HandleBurnEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, cancellationToken);

                        Console.WriteLine("BurnEvent");
                    }
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            // if we don't have any records start doingsomething like pagination
            var batchSize = 1000;
            var fromBlock = new BlockParameter(new HexBigInteger(BigInteger.Parse("3076964")));
            for (var i = fromBlock.BlockNumber; i <= latestBlock.Value; i.Value += batchSize)
            {
                var batchEnd = i.Value + batchSize - 1;
                if (batchEnd > latestBlock.Value)
                {
                    batchEnd = latestBlock.Value;
                }

                Console.WriteLine($"From block {fromBlock.BlockNumber} to {batchEnd}");
                foreach (var filter in filterInputs)
                {
                    filter.FromBlock = fromBlock;
                    filter.ToBlock = new BlockParameter(batchEnd.ToHexBigInteger());
                    var logs = await web3.Eth.Filters.GetLogs.SendRequestAsync(filter);
                    result.AddRange(logs);
                    Console.WriteLine($"Count events -> {logs.Count()}");

                }
            }

            foreach (var item in result)
            {
                var isEventSaved = await this.context.BridgeEvents.AnyAsync(x => x.Id == item.TransactionHash, cancellationToken);

                if (!isEventSaved)
                {
                    if (item.IsLogForEvent<LockEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<LockEventDTO>();

                        var newEvent = await HandleLockEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, cancellationToken);

                        Console.WriteLine("LockEvent");
                    }
                    else if (item.IsLogForEvent<UnlockEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<UnlockEventDTO>();

                        var newEvent = await HandleUnlockEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, cancellationToken);

                        Console.WriteLine("UnlockEvent");
                    }
                    else if (item.IsLogForEvent<MintEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<MintEventDTO>();

                        var newEvent = await HandleMintEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, cancellationToken);

                        Console.WriteLine("MintEvent");
                    }
                    else if (item.IsLogForEvent<BurnEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<BurnEventDTO>();

                        var newEvent = await HandleBurnEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, cancellationToken);

                        Console.WriteLine("BurnEvent");
                    }
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task ProcessEventChangesAsync<T>(Event<T> eventHandler, HexBigInteger filter, Func<T, string, Task<BridgeEvent>> handler, CancellationToken cancellationToken) where T : IEventDTO, new()
    {
        var changes = await eventHandler.GetFilterChangesAsync(filter);

        foreach (var change in changes)
        {
            logger.LogWarning("Destination Event catched");
            var @event = await handler(change.Event, change.Log.TransactionHash);

            await CreateEvent(@event, change, cancellationToken);
        }
    }

    private async Task CreateEvent<T>(BridgeEvent @event, EventLog<T> change, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(@event);
        var model = JsonSerializer.Deserialize<BridgeEvent>(json);

        if (model is null)
            throw new ArgumentNullException();

        model.Id = change.Log.TransactionHash;
        model.BlockNumber = change.Log.BlockNumber.ToString();
        model.ChainName = "Ethereum";
        model.CreatedDate = DateTime.Now;
        await context.BridgeEvents.AddAsync(model, cancellationToken);
    }

    private async Task<BridgeEvent> HandleLockEvent(LockEventDTO lockEvent, string txHash)
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

    private async Task<BridgeEvent> HandleUnlockEvent(UnlockEventDTO unlockEvent, string txHash)
    {
        var jsonObj = new
        {
            unlockEvent.Token,
            unlockEvent.To,
            Amount = unlockEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        var lockEvent = await this.context.BridgeEvents.AsTracking().FirstOrDefaultAsync(x => x.Id == unlockEvent.TxHash);
        if (lockEvent is not null)
        {
            lockEvent!.IsClaimed = true;
            lockEvent.ClaimedFromId = txHash;
        }

        return new BridgeEvent
        {
            PublicKeySender = unlockEvent.To,
            EventType = (int)EventType.TokenUnlocked,
            RequiresClaiming = false,
            IsClaimed = false,
            JsonData = json
        };
    }

    private async Task<BridgeEvent> HandleMintEvent(MintEventDTO mintEvent, string txHash)
    {
        var jsonObj = new
        {
            mintEvent.Token,
            mintEvent.To,
            Amount = mintEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        var lockEvent = await this.context.BridgeEvents.AsTracking().FirstOrDefaultAsync(x => x.Id == mintEvent.TxHash);
        if (lockEvent is not null)
        {
            lockEvent!.IsClaimed = true;
            lockEvent.ClaimedFromId = txHash;
        }

        return new BridgeEvent
        {
            PublicKeySender = mintEvent.To,
            EventType = (int)EventType.TokenMinted,
            RequiresClaiming = false,
            IsClaimed = false,
            JsonData = json
        };
    }

    private async Task<BridgeEvent> HandleBurnEvent(BurnEventDTO burnEvent, string txHash)
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
