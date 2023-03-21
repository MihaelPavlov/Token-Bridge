using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Contracts.CQS;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Spectre.Console;
using System.Numerics;

namespace Bridge_CLI;

public class BridgeCLI
{
    public async Task Init()
    {
        string Abi = @"
""[	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""burn\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""lock\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""message\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""LogMessage\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			},			{				\""internalType\"": \""string\"",				\""name\"": \""name\"",				\""type\"": \""string\""			},			{				\""internalType\"": \""string\"",				\""name\"": \""symbol\"",				\""type\"": \""string\""			}		],		\""name\"": \""mint\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""mint\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			}		],		\""name\"": \""setWrappedToSourceToken\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""sender\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenBurned\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""sender\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenLocked\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""to\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenMinted\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""to\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenUnlocked\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""originalToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""unlock\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""originalToken\"",				\""type\"": \""address\""			}		],		\""name\"": \""WrappedTokenCreated\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			}		],		\""name\"": \""getBytes\"",		\""outputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""\"",				\""type\"": \""bytes32\""			}		],		\""stateMutability\"": \""pure\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""bytes32\"",				\""name\"": \""\"",				\""type\"": \""bytes32\""			}		],		\""name\"": \""lockedTokens\"",		\""outputs\"": [			{				\""internalType\"": \""uint256\"",				\""name\"": \""\"",				\""type\"": \""uint256\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""name\"": \""sourceToWrappedTokenMap\"",		\""outputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""name\"": \""wrappedToSourceTokenMap\"",		\""outputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	}]""
";
        var contractAddress = "0xF68dA2aA23a703e5E6e10c96dFC74248e91c55A3";

        var account = new Account("a61ded91802937b4690567a62077f2cca4cb1342a9be3cd69fd81689ad349c04");

        var web3 = new Web3(account, @"https://data-seed-prebsc-1-s1.binance.org:8545/");

        var contract = web3.Eth.GetContract(Abi, contractAddress);

        while (true)
        {
            AnsiConsole.MarkupLine("[bold green]Select an action:[/]");
            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .AddChoices("Lock", "Unlock", "Mint", "Burn", "Exit")
                    .Title("Action:")
            );

            switch (action)
            {
                case "Lock":
                    await Lock(web3, contract);
                    break;
                //case "Unlock":
                //    await Unlock(web3, contract);
                //    break;
                //case "Mint":
                //    await Mint(web3, contract);
                //    break;
                //case "Burn":
                //    await Burn(web3, contract);
                //    break;
                case "Exit":
                    break;
            }
        }
    }

    public async Task Lock(Web3 web3, Contract contract)
    {
        AnsiConsole.MarkupLine("[bold green]Lock[/]");

        var sourceToken = AnsiConsole.Ask<string>("Source Token:");
        var amount = AnsiConsole.Ask<int>("Amount:");
        
        var transferHandler = web3.Eth.GetContractTransactionHandler<LockFunction>();
        var lockObj = new LockFunction
        {
            SourceToken = sourceToken,
            Amount = amount
        };
        lockObj.GasPrice = Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei);
        var transactionReceipt = await transferHandler.SendRequestAndWaitForReceiptAsync(contract.Address, lockObj);
        //var transactionInput = lockFunction.CreateTransactionInput("0x8eAF7fCBf069BE4D1e89Cf8eC24503b49bA69828",
        //    new object[] { sourceToken, amount }
        //);

        //var txHash = await web3.Eth.TransactionManager.SendTransactionAsync(transactionInput);
        AnsiConsole.MarkupLine($"Transaction sent: [bold yellow]{transactionReceipt.TransactionHash}[/]");
    }
}

[Function("lock")]
public class LockFunction : FunctionMessage
{
    [Parameter("address", "sourceToken", 1)]
    public string SourceToken { get; set; } = null!;

    [Parameter("uint256", "amount", 2)]
    public BigInteger Amount { get; set; }
}