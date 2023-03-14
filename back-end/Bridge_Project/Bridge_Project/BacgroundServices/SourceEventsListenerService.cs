﻿using Bridge_Project.Data;
using Bridge_Project.Data.Enums;
using Bridge_Project.Data.Models;
using Bridge_Project.EventsDTO;
using Microsoft.EntityFrameworkCore;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
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

    public SourceEventsListenerService(BridgeContext context, ILogger<BridgeContext> logger, IConfiguration configuration)
    {
        this.context = context;
        this.logger = logger;
        this.configuration = configuration;
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

            // If we don't put delay. 88 Line is throwing exception. And the background service stop working.
            await Task.Delay(20000, stoppingToken); // 1min
        }
    }

    private async Task ProcessEventChangesAsync<T>(Event<T> eventHandler, HexBigInteger filter, Func<T, Task<SourceEvent>> handler, CancellationToken cancellationToken) where T : IEventDTO, new()
    {
        var changes = await eventHandler.GetFilterChangesAsync(filter);

        foreach (var change in changes)
        {
            logger.LogWarning("Source Event catched");
            var @event = await handler(change.Event);
            var json = JsonSerializer.Serialize(@event);
            var model = JsonSerializer.Deserialize<SourceEvent>(json);

            if (model is null)
                throw new ArgumentNullException();

            model.Id = change.Log.TransactionHash;
            await context.SourceEvents.AddAsync(model, cancellationToken);
        }
    }

    private async Task<SourceEvent> HandleLockEvent(LockEventDTO lockEvent)
    {
        var jsonObj = new
        {
            lockEvent.Token,
            lockEvent.Sender,
            Amount = lockEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        return new SourceEvent
        {
            PublicKeySender = lockEvent.Sender,
            EventType = (int)EventType.TokenLocked,
            RequiresClaiming = true,
            IsClaimed = false,
            JsonData = json
        };
    }

    private async Task<SourceEvent> HandleUnlockEvent(UnlockEventDTO unlockEvent)
    {
        var jsonObj = new
        {
            unlockEvent.Token,
            unlockEvent.To,
            Amount = unlockEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        var lockEvent = await this.context.DestinationEvents.AsTracking().FirstOrDefaultAsync(x => x.Id == unlockEvent.TxHash);

        lockEvent!.IsClaimed = true;

        return new SourceEvent
        {
            EventType = (int)EventType.TokenUnlocked,
            RequiresClaiming = false,
            IsClaimed = false,
            JsonData = json
        };
    }

    private async Task<SourceEvent> HandleMintEvent(MintEventDTO mintEvent)
    {
        var jsonObj = new
        {
            mintEvent.Token,
            mintEvent.To,
            Amount = mintEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        var lockEvent = await this.context.DestinationEvents.AsTracking().FirstOrDefaultAsync(x => x.Id == mintEvent.TxHash);

        lockEvent!.IsClaimed = true;

        return new SourceEvent
        {
            EventType = (int)EventType.TokenMinted,
            RequiresClaiming = false,
            IsClaimed = false,
            JsonData = json
        };
    }

    private async Task<SourceEvent> HandleBurnEvent(BurnEventDTO burnEvent)
    {
        var jsonObj = new
        {
            burnEvent.Token,
            burnEvent.Sender,
            Amount = burnEvent.Amount.ToString()
        };

        var json = JsonSerializer.Serialize(jsonObj);

        return new SourceEvent
        {
            PublicKeySender = burnEvent.Sender,
            EventType = (int)EventType.TokenBurned,
            RequiresClaiming = true,
            IsClaimed = false,
            JsonData = json
        };
    }
}
