using Bridge_CLI.FunctionModels;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Spectre.Console;

namespace Bridge_CLI;

public class BridgeCLI
{
    public async Task Init()
    {
        string Abi = @"
""[	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""burn\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""lock\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""message\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""LogMessage\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			},			{				\""internalType\"": \""string\"",				\""name\"": \""name\"",				\""type\"": \""string\""			},			{				\""internalType\"": \""string\"",				\""name\"": \""symbol\"",				\""type\"": \""string\""			}		],		\""name\"": \""mint\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""mint\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""sourceToken\"",				\""type\"": \""address\""			}		],		\""name\"": \""setWrappedToSourceToken\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""sender\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenBurned\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""sender\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenLocked\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""to\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenMinted\"",		\""type\"": \""event\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": false,				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""to\"",				\""type\"": \""address\""			},			{				\""indexed\"": false,				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""TokenUnlocked\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""txHash\"",				\""type\"": \""bytes32\""			},			{				\""internalType\"": \""address\"",				\""name\"": \""originalToken\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""uint256\"",				\""name\"": \""amount\"",				\""type\"": \""uint256\""			}		],		\""name\"": \""unlock\"",		\""outputs\"": [],		\""stateMutability\"": \""nonpayable\"",		\""type\"": \""function\""	},	{		\""anonymous\"": false,		\""inputs\"": [			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""wrappedToken\"",				\""type\"": \""address\""			},			{				\""indexed\"": true,				\""internalType\"": \""address\"",				\""name\"": \""originalToken\"",				\""type\"": \""address\""			}		],		\""name\"": \""WrappedTokenCreated\"",		\""type\"": \""event\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""token\"",				\""type\"": \""address\""			}		],		\""name\"": \""getBytes\"",		\""outputs\"": [			{				\""internalType\"": \""bytes32\"",				\""name\"": \""\"",				\""type\"": \""bytes32\""			}		],		\""stateMutability\"": \""pure\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			},			{				\""internalType\"": \""bytes32\"",				\""name\"": \""\"",				\""type\"": \""bytes32\""			}		],		\""name\"": \""lockedTokens\"",		\""outputs\"": [			{				\""internalType\"": \""uint256\"",				\""name\"": \""\"",				\""type\"": \""uint256\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""name\"": \""sourceToWrappedTokenMap\"",		\""outputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	},	{		\""inputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""name\"": \""wrappedToSourceTokenMap\"",		\""outputs\"": [			{				\""internalType\"": \""address\"",				\""name\"": \""\"",				\""type\"": \""address\""			}		],		\""stateMutability\"": \""view\"",		\""type\"": \""function\""	}]""
";
        var contractAddress = string.Empty;

        Account account;

        Web3 web3 = null!;

        Contract contract = null!;

        while (true)
        {
            AnsiConsole.MarkupLine("[bold green]Select chain:[/]");
            var action1 = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(4)
                    .AddChoices("Source Chain", "Target Chain")
                    .Title("Action:")
            );
            switch (action1)
            {
                case "Source Chain":
                    {
                        AnsiConsole.MarkupLine("Selected Chain: [bold blue]Ethereum Sepolia[/]");
                        var privateKey = AnsiConsole.Prompt(new TextPrompt<string>("Enter You Private Key: Ethereum").PromptStyle("red").Secret());
                        contractAddress = "0xE8cA4610F0d109d5409FCBB52a072B82E90aFE0D";
                        account = new Account(privateKey);
                        web3 = new Web3(account, @"https://sepolia.infura.io/v3/31d722098d4e48929c96519ba339b2d0");
                        contract = web3.Eth.GetContract(Abi, contractAddress);
                    }
                    break;
                case "Target Chain":
                    {
                        AnsiConsole.MarkupLine("Selected Chain: [bold yellow]Binance Smart Chain Testnet[/]");
                        var privateKey = AnsiConsole.Prompt(new TextPrompt<string>("Enter You Private Key: BSC Testnet").PromptStyle("red").Secret());
                        contractAddress = "0xF68dA2aA23a703e5E6e10c96dFC74248e91c55A3";
                        account = new Account(privateKey);
                        web3 = new Web3(account, @"https://data-seed-prebsc-1-s1.binance.org:8545/");
                        contract = web3.Eth.GetContract(Abi, contractAddress);
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(contractAddress))
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
                    case "Unlock":
                        await Unlock(web3, contract);
                        break;
                    case "Mint":
                        await Mint(web3, contract);
                        break;
                    case "Burn":
                        await Burn(web3, contract);
                        break;
                    case "Exit":
                        break;
                }
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

        try
        {
            TransactionReceipt transactionReceipt = new TransactionReceipt();
            await AnsiConsole.Status()
            .StartAsync("Waithing...", async ctx =>
            {
                var result = await transferHandler.SignTransactionAsync(contract.Address,lockObj);

                transactionReceipt = await transferHandler.SendRequestAndWaitForReceiptAsync(contract.Address, lockObj);
            });

            if (transactionReceipt.Succeeded())
                AnsiConsole.MarkupLine($"[bold green]Succeded[/] Transaction sent: [bold yellow]{transactionReceipt.TransactionHash}[/]");
            else if (transactionReceipt.Failed())
                AnsiConsole.MarkupLine($"[bold red]Transaction Failed[/]");

        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Error --> {ex.Message}[/]");
        }
    }

    public async Task Unlock(Web3 web3, Contract contract)
    {
        AnsiConsole.MarkupLine("[bold green]Unlock[/]");

        var txHash = AnsiConsole.Ask<string>("Transaction Hash:");
        var originalToken = AnsiConsole.Ask<string>("Original Token:");
        var amount = AnsiConsole.Ask<int>("Amount:");

        var transferHandler = web3.Eth.GetContractTransactionHandler<UnlockFunction>();
        var unlockObj = new UnlockFunction
        {
            TxHash = txHash,
            OriginalToken = originalToken,
            Amount = amount
        };
        unlockObj.GasPrice = Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei);
        try
        {
            TransactionReceipt transactionReceipt = new TransactionReceipt();
            await AnsiConsole.Status()
            .StartAsync("Waithing...", async ctx =>
            {
                transactionReceipt = await transferHandler.SendRequestAndWaitForReceiptAsync(contract.Address, unlockObj);
            });

            if (transactionReceipt.Succeeded())
                AnsiConsole.MarkupLine($"[bold green]Succeded[/] Transaction sent: [bold yellow]{transactionReceipt.TransactionHash}[/]");
            else if (transactionReceipt.Failed())
                AnsiConsole.MarkupLine($"[bold red]Transaction Failed[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Error --> {ex.Message}[/]");
        }
    }

    public async Task Mint(Web3 web3, Contract contract)
    {
        AnsiConsole.MarkupLine("[bold green]Mint[/]");

        var txHash = AnsiConsole.Ask<string>("Transaction Hash:");
        var sourceToken = AnsiConsole.Ask<string>("Source Token:");
        var amount = AnsiConsole.Ask<int>("Amount:");
        var tokenName = AnsiConsole.Ask<string>("Token Name:");
        var tokenSymbol = AnsiConsole.Ask<string>("Token Symbol:");

        var transferHandler = web3.Eth.GetContractTransactionHandler<MintFunction>();
        var mintObj = new MintFunction
        {
            TxHash = txHash,
            SourceToken = sourceToken,
            Amount = amount,
            Name = tokenName,
            Symbol = tokenSymbol
        };

        mintObj.GasPrice = Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei);

        try
        {
            TransactionReceipt transactionReceipt = new TransactionReceipt();
            await AnsiConsole.Status()
            .StartAsync("Waithing...", async ctx =>
            {
                transactionReceipt = await transferHandler.SendRequestAndWaitForReceiptAsync(contract.Address, mintObj);
            });

            if (transactionReceipt.Succeeded())
                AnsiConsole.MarkupLine($"[bold green]Succeded[/] Transaction sent: [bold yellow]{transactionReceipt.TransactionHash}[/]");
            else if (transactionReceipt.Failed())
                AnsiConsole.MarkupLine($"[bold red]Transaction Failed[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Error --> {ex.Message}[/]");
        }
    }

    public async Task Burn(Web3 web3, Contract contract)
    {
        AnsiConsole.MarkupLine("[bold green]Burn[/]");

        var wrappedToken = AnsiConsole.Ask<string>("Wrapped Token:");
        var amount = AnsiConsole.Ask<int>("Amount:");

        var transferHandler = web3.Eth.GetContractTransactionHandler<BurnFunction>();
        var mintObj = new BurnFunction
        {
            WrappedToken = wrappedToken,
            Amount = amount,
        };

        mintObj.GasPrice = Web3.Convert.ToWei(25, UnitConversion.EthUnit.Gwei);

        try
        {
            TransactionReceipt transactionReceipt = new TransactionReceipt();
            await AnsiConsole.Status()
            .StartAsync("Waithing...", async ctx =>
            {
                transactionReceipt = await transferHandler.SendRequestAndWaitForReceiptAsync(contract.Address, mintObj);
            });

            if (transactionReceipt.Succeeded())
                AnsiConsole.MarkupLine($"[bold green]Succeded[/] Transaction sent: [bold yellow]{transactionReceipt.TransactionHash}[/]");
            else if (transactionReceipt.Failed())
                AnsiConsole.MarkupLine($"[bold red]Transaction Failed[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Error --> {ex.Message}[/]");
        }
    }
}
