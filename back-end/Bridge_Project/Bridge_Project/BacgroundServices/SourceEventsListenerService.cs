using Bridge_Project.BacgroundServices.Models;
using Bridge_Project.Data;
using Bridge_Project.Data.Enums;
using Bridge_Project.Data.Models;
using Bridge_Project.EventsDTO;
using Bridge_Project.Singleton;
using Microsoft.EntityFrameworkCore;
using Nethereum.ABI.FunctionEncoding.Attributes;
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
    const string Abi = @"
""[  {    \""anonymous\"": false,    \""inputs\"": [      {        \""indexed\"": true,        \""internalType\"": \""address\"",        \""name\"": \""token\"",        \""type\"": \""address\""      },      {        \""indexed\"": true,        \""internalType\"": \""address\"",        \""name\"": \""sender\"",        \""type\"": \""address\""      },      {        \""indexed\"": false,        \""internalType\"": \""uint256\"",        \""name\"": \""amount\"",        \""type\"": \""uint256\""      }    ],    \""name\"": \""Burn\"",    \""type\"": \""event\""  },  {    \""anonymous\"": false,    \""inputs\"": [      {        \""indexed\"": true,        \""internalType\"": \""address\"",        \""name\"": \""token\"",        \""type\"": \""address\""      },      {        \""indexed\"": true,        \""internalType\"": \""address\"",        \""name\"": \""sender\"",        \""type\"": \""address\""      },      {        \""indexed\"": false,        \""internalType\"": \""uint256\"",        \""name\"": \""amount\"",        \""type\"": \""uint256\""      }    ],    \""name\"": \""Lock\"",    \""type\"": \""event\""  },  {    \""anonymous\"": false,    \""inputs\"": [      {        \""indexed\"": false,        \""internalType\"": \""string\"",        \""name\"": \""txHash\"",        \""type\"": \""string\""      },      {        \""indexed\"": true,        \""internalType\"": \""address\"",        \""name\"": \""token\"",        \""type\"": \""address\""      },      {        \""indexed\"": true,        \""internalType\"": \""address\"",        \""name\"": \""to\"",        \""type\"": \""address\""      },      {        \""indexed\"": false,        \""internalType\"": \""uint256\"",        \""name\"": \""amount\"",        \""type\"": \""uint256\""      }    ],    \""name\"": \""Mint\"",    \""type\"": \""event\""  },  {    \""anonymous\"": false,    \""inputs\"": [      {        \""indexed\"": false,        \""internalType\"": \""string\"",        \""name\"": \""txHash\"",        \""type\"": \""string\""      },      {        \""indexed\"": true,        \""internalType\"": \""address\"",        \""name\"": \""token\"",        \""type\"": \""address\""      },      {        \""indexed\"": true,        \""internalType\"": \""address\"",        \""name\"": \""to\"",        \""type\"": \""address\""      },      {        \""indexed\"": false,        \""internalType\"": \""uint256\"",        \""name\"": \""amount\"",        \""type\"": \""uint256\""      }    ],    \""name\"": \""Unlock\"",    \""type\"": \""event\""  },  {    \""anonymous\"": false,    \""inputs\"": [      {        \""indexed\"": true,        \""internalType\"": \""address\"",        \""name\"": \""wrappedToken\"",        \""type\"": \""address\""      },      {        \""indexed\"": true,        \""internalType\"": \""address\"",        \""name\"": \""originalToken\"",        \""type\"": \""address\""      }    ],    \""name\"": \""WrappedTokenCreated\"",    \""type\"": \""event\""  },  {    \""inputs\"": [      {        \""internalType\"": \""address\"",        \""name\"": \""wrappedToken\"",        \""type\"": \""address\""      },      {        \""internalType\"": \""uint256\"",        \""name\"": \""amount\"",        \""type\"": \""uint256\""      }    ],    \""name\"": \""burn\"",    \""outputs\"": [          ],    \""stateMutability\"": \""nonpayable\"",    \""type\"": \""function\""  },  {    \""inputs\"": [      {        \""internalType\"": \""string\"",        \""name\"": \""\"",        \""type\"": \""string\""      }    ],    \""name\"": \""claimedTransactions\"",    \""outputs\"": [      {        \""internalType\"": \""bool\"",        \""name\"": \""\"",        \""type\"": \""bool\""      }    ],    \""stateMutability\"": \""view\"",    \""type\"": \""function\""  },  {    \""inputs\"": [      {        \""internalType\"": \""address\"",        \""name\"": \""sourceToken\"",        \""type\"": \""address\""      },      {        \""internalType\"": \""uint256\"",        \""name\"": \""amount\"",        \""type\"": \""uint256\""      }    ],    \""name\"": \""lock\"",    \""outputs\"": [          ],    \""stateMutability\"": \""nonpayable\"",    \""type\"": \""function\""  },  {    \""inputs\"": [      {        \""internalType\"": \""string\"",        \""name\"": \""txHash\"",        \""type\"": \""string\""      },      {        \""internalType\"": \""address\"",        \""name\"": \""sourceToken\"",        \""type\"": \""address\""      },      {        \""internalType\"": \""uint256\"",        \""name\"": \""amount\"",        \""type\"": \""uint256\""      },      {        \""internalType\"": \""string\"",        \""name\"": \""name\"",        \""type\"": \""string\""      },      {        \""internalType\"": \""string\"",        \""name\"": \""symbol\"",        \""type\"": \""string\""      },      {        \""internalType\"": \""uint8\"",        \""name\"": \""decimals\"",        \""type\"": \""uint8\""      }    ],    \""name\"": \""mint\"",    \""outputs\"": [          ],    \""stateMutability\"": \""nonpayable\"",    \""type\"": \""function\""  },  {    \""inputs\"": [      {        \""internalType\"": \""address\"",        \""name\"": \""\"",        \""type\"": \""address\""      }    ],    \""name\"": \""sourceToWrappedTokenMap\"",    \""outputs\"": [      {        \""internalType\"": \""address\"",        \""name\"": \""\"",        \""type\"": \""address\""      }    ],    \""stateMutability\"": \""view\"",    \""type\"": \""function\""  },  {    \""inputs\"": [      {        \""internalType\"": \""string\"",        \""name\"": \""txHash\"",        \""type\"": \""string\""      },      {        \""internalType\"": \""address\"",        \""name\"": \""originalToken\"",        \""type\"": \""address\""      },      {        \""internalType\"": \""uint256\"",        \""name\"": \""amount\"",        \""type\"": \""uint256\""      }    ],    \""name\"": \""unlock\"",    \""outputs\"": [          ],    \""stateMutability\"": \""nonpayable\"",    \""type\"": \""function\""  },  {    \""inputs\"": [      {        \""internalType\"": \""address\"",        \""name\"": \""\"",        \""type\"": \""address\""      }    ],    \""name\"": \""wrappedToSourceTokenMap\"",    \""outputs\"": [      {        \""internalType\"": \""address\"",        \""name\"": \""\"",        \""type\"": \""address\""      }    ],    \""stateMutability\"": \""view\"",    \""type\"": \""function\""  }]""
";
    readonly ILogger<BridgeContext> logger;
    readonly IConfiguration configuration;
    readonly DbContextOptions<BridgeContext> options;
    readonly EventTracker tracker;

    readonly Network network;

    public SourceEventsListenerService(ILogger<BridgeContext> logger, IConfiguration configuration, DbContextOptions<BridgeContext> options, EventTracker tracker)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.options = options;
        this.tracker = EventTracker.Instance;
        this.network = this.configuration.GetSection("Network_1").Get<Network>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.logger.LogInformation("Initialize Source Service");

        var account = new Account(this.network.PrivateKey);
        var web3 = new Web3(account, @"https://sepolia.infura.io/v3/74da96f16ad744b08ce0da374aee014c");

        var contract = web3.Eth.GetContract(Abi, this.network.BridgeContractAddress);

        #region Initialize Events
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
        #endregion Initialize Events

        #region Calculate the Block Timestamp
        var block1 = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(3076954));
        var block2 = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(3076955));

        var blockTimestamp = TimeSpan.FromSeconds((long)block2.Timestamp.Value) - TimeSpan.FromSeconds((long)block1.Timestamp.Value);
        var milliseconds = int.Parse(blockTimestamp.TotalMilliseconds.ToString());
        #endregion Calculate the Block Timestamp

        await CheckForEvents(web3, new List<NewFilterInput> { filterInput, filterInput1, filterInput2, filterInput3 }, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            this.logger.LogInformation("Inside While Loop 1");

            try
            {
                var tasks = new List<Task>();
                using (var context = new BridgeContext(options))
                {
                    tasks.Add(ProcessEventChangesAsync(lockEventHandler, filter, HandleLockEvent, context, stoppingToken));
                    tasks.Add(ProcessEventChangesAsync(unlockEventHandler, filter1, HandleUnlockEvent, context, stoppingToken));
                    tasks.Add(ProcessEventChangesAsync(mintEventHandler, filter2, HandleMintEvent, context, stoppingToken));
                    tasks.Add(ProcessEventChangesAsync(burnEventHandler, filter3, HandleBurnEvent, context, stoppingToken));

                    await Task.WhenAll(tasks);

                    await context.SaveChangesAsync();
                    this.logger.LogInformation("Events are saved");
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex.Message);
            }

            await Task.Delay(milliseconds, stoppingToken);
        }
    }

    /// <summary>
    /// Checking for events before start listening for new events.
    /// Case 1: if there are events in the database, we are getting the last saved event and use it as a starting point for fetching the new events.
    /// Case 2: if there are no events in the database, we start fetching events from the block number on which the bridge contract was created/deployed.
    /// </summary>
    private async Task CheckForEvents(Web3 web3, List<NewFilterInput> filterInputs, CancellationToken cancellationToken)
    {
        var latestBlock = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

        int count;
        List<FilterLog> filterLogs = new List<FilterLog>();

        using (var context = new BridgeContext(options))
        {
            count = await context.BridgeEvents.CountAsync(x => x.ChainName == this.network.Name, cancellationToken);
        }

        if (count != 0)
        {
            BridgeEvent lastSavedEvent;

            using (var context = new BridgeContext(options))
            {
                lastSavedEvent = await context.BridgeEvents.Where(x => x.ChainName == this.network.Name).OrderByDescending(x => x.BlockNumber).FirstAsync(cancellationToken);
            }

            await FetchEvents(batchSize: this.network.BatchSizeEvents,
              fromBlock: new BlockParameter(new HexBigInteger(BigInteger.Parse(lastSavedEvent.BlockNumber))),
              latestBlock: latestBlock,
              web3,
              filterInputs,
              filterLogs);

            await ProcessLogs(filterLogs, cancellationToken);
        }
        else
        {
            await FetchEvents(batchSize: this.network.BatchSizeEvents,
                fromBlock: new BlockParameter(new HexBigInteger(BigInteger.Parse(this.network.DeployedContractBlockNumber))),
                latestBlock: latestBlock,
                web3,
                filterInputs,
                filterLogs);

            await ProcessLogs(filterLogs, cancellationToken);
        }
    }

    /// <summary>
    /// Processes a list of event logs and saves them to a database.
    /// </summary>
    private async Task ProcessLogs(List<FilterLog> filterLogs, CancellationToken cancellationToken)
    {
        using (var context = new BridgeContext(options))
        {
            foreach (var item in filterLogs)
            {
                bool isEventSaved = await context.BridgeEvents.AnyAsync(x => x.Id == item.TransactionHash, cancellationToken);

                if (!isEventSaved)
                {
                    if (item.IsLogForEvent<LockEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<LockEventDTO>();

                        var newEvent = await HandleLockEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, context, cancellationToken);

                        this.logger.LogInformation("Lock Event is Processed");
                    }
                    else if (item.IsLogForEvent<UnlockEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<UnlockEventDTO>();

                        var newEvent = await HandleUnlockEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, context, cancellationToken);

                        this.logger.LogInformation("Unlock Event is Processed");
                    }
                    else if (item.IsLogForEvent<MintEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<MintEventDTO>();

                        var newEvent = await HandleMintEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, context, cancellationToken);

                        this.logger.LogInformation("Mint Event is Processed");
                    }
                    else if (item.IsLogForEvent<BurnEventDTO>())
                    {
                        var eventLog = item.DecodeEvent<BurnEventDTO>();

                        var newEvent = await HandleBurnEvent(eventLog.Event, eventLog.Log.TransactionHash);

                        await CreateEvent(newEvent, eventLog, context, cancellationToken);

                        this.logger.LogInformation("Burn Event is Processed");
                    }
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Allows for efficient retrieval of a large number of log events from the Ethereum blockchain in batches, by limiting the number of logs fetched in each call to the GetLogs method.
    /// </summary>
    /// <param name="batchSize">Determines the number of log events to be fetched in each batch when retrieving logs from a range of blocks.</param>
    /// <param name="fromBlock">Used to specify the starting block.</param>
    /// <param name="latestBlock">Used to specify the latest block.</param>
    private async Task FetchEvents(int batchSize, BlockParameter fromBlock, HexBigInteger latestBlock, Web3 web3, List<NewFilterInput> filterInputs, List<FilterLog> filterLogs)
    {
        for (var i = fromBlock.BlockNumber; i <= latestBlock.Value; i.Value += batchSize)
        {
            var batchEnd = i.Value + batchSize - 1;
            if (batchEnd > latestBlock.Value)
            {
                batchEnd = latestBlock.Value;
            }

            this.logger.LogInformation($"-- From block {fromBlock.BlockNumber} to {batchEnd} --");

            foreach (var filter in filterInputs)
            {
                filter.FromBlock = fromBlock;
                filter.ToBlock = new BlockParameter(batchEnd.ToHexBigInteger());

                var logs = await web3.Eth.Filters.GetLogs.SendRequestAsync(filter);

                filterLogs.AddRange(logs);

                this.logger.LogInformation($"Logs -> {logs.Count()}");
            }
        }
    }

    private async Task ProcessEventChangesAsync<T>(Event<T> eventHandler, HexBigInteger filter, Func<T, string, Task<BridgeEvent>> handler, BridgeContext context, CancellationToken cancellationToken) where T : IEventDTO, new()
    {
        var changes = await eventHandler.GetFilterChangesAsync(filter);

        logger.LogInformation($"Destination events {changes.Count}");

        foreach (var change in changes)
        {
            var @event = await handler(change.Event, change.Log.TransactionHash);

            await CreateEvent(@event, change, context, cancellationToken);
        }
    }

    private async Task CreateEvent<T>(BridgeEvent @event, EventLog<T> change, BridgeContext context, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(@event);
        var model = JsonSerializer.Deserialize<BridgeEvent>(json);

        if (model is null)
        {
            this.logger.LogError($"Deserialization unsuccessfully for event with transaction hash [{change.Log.TransactionHash}]");
            return;
        }

        model.Id = change.Log.TransactionHash;
        model.BlockNumber = change.Log.BlockNumber.ToString();
        model.ChainName = this.network.Name;
        model.CreatedDate = DateTime.Now;

        await context.BridgeEvents.AddAsync(model, cancellationToken);
    }

    #region Handlers
    private Task<BridgeEvent> HandleLockEvent(LockEventDTO lockEvent, string txHash)
    {
        var jsonObj = new
        {
            lockEvent.Token,
            lockEvent.Sender,
            Amount = lockEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        return Task.FromResult(new BridgeEvent
        {
            PublicKeySender = lockEvent.Sender,
            EventType = (int)EventType.TokenLocked,
            RequiresClaiming = true,
            IsClaimed = false,
            JsonData = json
        });
    }

    private Task<BridgeEvent> HandleUnlockEvent(UnlockEventDTO unlockEvent, string txHash)
    {
        var jsonObj = new
        {
            unlockEvent.Token,
            unlockEvent.To,
            Amount = unlockEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        this.tracker.AddEvent(unlockEvent.TxHash, txHash);

        return Task.FromResult(new BridgeEvent
        {
            PublicKeySender = unlockEvent.To,
            EventType = (int)EventType.TokenUnlocked,
            RequiresClaiming = false,
            IsClaimed = false,
            JsonData = json
        });
    }

    private Task<BridgeEvent> HandleMintEvent(MintEventDTO mintEvent, string txHash)
    {
        var jsonObj = new
        {
            mintEvent.Token,
            mintEvent.To,
            Amount = mintEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        this.tracker.AddEvent(mintEvent.TxHash, txHash);

        return Task.FromResult(new BridgeEvent
        {
            PublicKeySender = mintEvent.To,
            EventType = (int)EventType.TokenMinted,
            RequiresClaiming = false,
            IsClaimed = false,
            JsonData = json
        });
    }

    private Task<BridgeEvent> HandleBurnEvent(BurnEventDTO burnEvent, string txHash)
    {
        var jsonObj = new
        {
            burnEvent.Token,
            burnEvent.Sender,
            Amount = burnEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        return Task.FromResult(new BridgeEvent
        {
            PublicKeySender = burnEvent.Sender,
            EventType = (int)EventType.TokenBurned,
            RequiresClaiming = true,
            IsClaimed = false,
            JsonData = json
        });
    }
    #endregion Handlers
}
