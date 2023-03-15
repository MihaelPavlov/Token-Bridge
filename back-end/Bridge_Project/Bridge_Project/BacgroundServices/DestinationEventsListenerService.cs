using Bridge_Project.Data;
using Bridge_Project.Data.Enums;
using Bridge_Project.Data.Models;
using Bridge_Project.EventsDTO;
using Bridge_Project.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System.Numerics;
using System.Text.Json;
using System.Threading;

namespace Bridge_Project.BacgroundServices;

public class DestinationEventsListenerService : BackgroundService
{
    private const string Abi = @"
""[	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""burn\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""lock\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""message\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""LogMessage\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			},			{				\""internalType\"": \""string\"",				\""name\"": \""name\"",				\""type\"": \""string\""			},			{				\""internalType\"": \""string\"",				\""name\"": \""symbol\"",				\""type\"": \""string\""			}		],		\""name\"": \""mint\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""mint\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			}		],		\""name\"": \""setWrappedToSourceToken\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""sender\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenBurned\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""sender\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenLocked\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""to\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenMinted\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""to\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenUnlocked\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""originalToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""unlock\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""originalToken\"",				\""type\"": \""address\""			}		],		\""name\"": \""WrappedTokenCreated\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			}		],		\""name\"": \""getBytes\"",		\""outputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""\"",				\""type\"": \""bytes32\""			}		],		\""stateMutability\"": \""pure\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""bytes32\"",				\""name\"": \""\"",				\""type\"": \""bytes32\""			}		],		\""name\"": \""lockedTokens\"",		\""outputs\"": [			{				\""internalType\"": \""uint256\"",				\""name\"": \""\"",				\""type\"": \""uint256\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""name\"": \""sourceToWrappedTokenMap\"",		\""outputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""name\"": \""wrappedToSourceTokenMap\"",		\""outputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	}]""
";
    private readonly BridgeContext context;
    private readonly ILogger<BridgeContext> logger;
    private readonly IConfiguration configuration;
    public DestinationEventsListenerService(BridgeContext context, ILogger<BridgeContext> logger, IConfiguration configuration)
    {
        this.context = context;
        this.logger = logger;
        this.configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogWarning("Initialzie Destination Service");
        var privateKey = configuration.GetValue(typeof(string), "DestinationPrivateKey").ToString();
        var contractAddress = configuration.GetValue(typeof(string), "DestinationBridgeContractAddress").ToString();
        var account = new Account(privateKey);
        var web3 = new Web3(account, @"https://data-seed-prebsc-1-s1.binance.org:8545/");

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

        var block1 = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(28074053));
        var block2 = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(28074054));

        var blockTimestamp = TimeSpan.FromSeconds((long)block2.Timestamp.Value) - TimeSpan.FromSeconds((long)block1.Timestamp.Value);
        var milliseconds = int.Parse(blockTimestamp.TotalMilliseconds.ToString());

        await CheckForEvents(web3, filterInput, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogWarning("Inside While Loop");

            try
            {
                var tasks = new List<Task>();

                tasks.Add(ProcessEventChangesAsync(lockEventHandler, filter, HandleLockEvent, stoppingToken));
                tasks.Add(ProcessEventChangesAsync(unlockEventHandler, filter1, HandleUnlockEvent, stoppingToken));
                tasks.Add(ProcessEventChangesAsync(mintEventHandler, filter2, HandleMintEventAsync, stoppingToken));
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

    private async Task CheckForEvents(Web3 web3, NewFilterInput filterInput, CancellationToken stoppingToken)
    {
        var latestBlock = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

        FilterLog[] result;
        if (await this.context.BridgeEvents.CountAsync(stoppingToken) != 0)
        {
            // if we don't have records we can maybe have the bridge deployed block number

            var lastSavedBlockNumber = await this.context.BridgeEvents.OrderByDescending(x => x.CreatedDate).FirstAsync(stoppingToken);
            filterInput.FromBlock = new BlockParameter(new HexBigInteger(BigInteger.Parse(lastSavedBlockNumber.BlockNumber)));
            filterInput.ToBlock = new BlockParameter(latestBlock);
            result = await web3.Eth.Filters.GetLogs.SendRequestAsync(filterInput);

            foreach (var item in result)
            {
                var isEventSaved = await this.context.BridgeEvents.AnyAsync(x => x.Id == item.TransactionHash, stoppingToken);

                //TODO: Extract that logic
                //TODO: Check and for the other events
                if (!isEventSaved)
                {
                    if (item.IsLogForEvent<LockEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<LockEventDTO>();

                        var jsonObj = new
                        {
                            eventLog.Event.Token,
                            eventLog.Event.Sender,
                            Amount = eventLog.Event.Amount.ToString()
                        };

                        var json = JsonSerializer.Serialize(jsonObj);

                        var newEvent = new BridgeEvent
                        {
                            PublicKeySender = eventLog.Event.Sender,
                            EventType = (int)EventType.TokenLocked,
                            RequiresClaiming = true,
                            IsClaimed = false,
                            JsonData = json
                        };

                        var jsonModel = JsonSerializer.Serialize(newEvent);
                        var model = JsonSerializer.Deserialize<BridgeEvent>(jsonModel);

                        if (model is null)
                            throw new ArgumentNullException();

                        model.Id = eventLog.Log.TransactionHash;
                        model.BlockNumber = eventLog.Log.BlockNumber.ToString();
                        model.ChainName = "BSC";
                        model.CreatedDate = DateTime.Now;
                        await context.BridgeEvents.AddAsync(model, stoppingToken);

                        Console.WriteLine("LockEvent");
                    }
                }

                Console.WriteLine(item.TransactionHash);
            }

            await context.SaveChangesAsync(stoppingToken);
        }
    }

    private async Task ProcessEventChangesAsync<T>(Event<T> eventHandler, HexBigInteger filter, Func<T, Task<BridgeEvent>> handler, CancellationToken cancellationToken) where T : IEventDTO, new()
    {
        var changes = await eventHandler.GetFilterChangesAsync(filter);

        foreach (var change in changes)
        {
            logger.LogWarning("Destination Event catched");
            var @event = await handler(change.Event);
            var json = JsonSerializer.Serialize(@event);
            var model = JsonSerializer.Deserialize<BridgeEvent>(json);

            if (model is null)
                throw new ArgumentNullException();

            model.Id = change.Log.TransactionHash;
            model.BlockNumber = change.Log.BlockNumber.ToString();
            model.ChainName = "BSC";
            model.CreatedDate = DateTime.Now;
            await context.BridgeEvents.AddAsync(model, cancellationToken);
        }
    }

    private async Task<BridgeEvent> HandleLockEvent(LockEventDTO lockEvent)
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

    private async Task<BridgeEvent> HandleUnlockEvent(UnlockEventDTO unlockEvent)
    {
        var jsonObj = new
        {
            unlockEvent.Token,
            unlockEvent.To,
            Amount = unlockEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        var lockEvent = await this.context.BridgeEvents.AsTracking().FirstOrDefaultAsync(x => x.Id == unlockEvent.TxHash);

        lockEvent!.IsClaimed = true;

        return new BridgeEvent
        {
            EventType = (int)EventType.TokenUnlocked,
            RequiresClaiming = false,
            IsClaimed = false,
            JsonData = json
        };
    }

    private async Task<BridgeEvent> HandleMintEventAsync(MintEventDTO mintEvent)
    {
        var jsonObj = new
        {
            mintEvent.Token,
            mintEvent.To,
            Amount = mintEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        var lockEvent = await this.context.BridgeEvents.AsTracking().FirstOrDefaultAsync(x => x.Id == mintEvent.TxHash);
        lockEvent!.IsClaimed = true;

        return new BridgeEvent
        {
            EventType = (int)EventType.TokenMinted,
            RequiresClaiming = false,
            IsClaimed = false,
            JsonData = json
        };
    }

    private async Task<BridgeEvent> HandleBurnEvent(BurnEventDTO burnEvent)
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
