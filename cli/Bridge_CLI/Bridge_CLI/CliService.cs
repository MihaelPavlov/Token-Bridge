using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Util;
using Nethereum.Web3;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Bridge_CLI
{
    //internal class CliService
    //{
    //    public async Task Initialize()
    //    {
    //        string privateKey = "0x...";
    //        string bridgeContractAddress = "0x...";
    //        string sourceTokenAddress = "0x...";
    //        string wrappedTokenAddress = "0x...";
    //        string txHash = "0x...";

    //        // Initialize the web3 object with the Infura endpoint and private key
    //        var web3 = new Web3("https://mainnet.infura.io/v3/<your-infura-project-id>");
    //        var account = new Nethereum.Web3.Accounts.Account(privateKey);

    //        // Load the contract
    //        var bridgeContract = new Contract(web3.Eth, BridgeContract.ABI, bridgeContractAddress);

    //        // Create the function calls
    //        var lockFunction = bridgeContract.GetFunction("lock");
    //        var unlockFunction = bridgeContract.GetFunction("unlock");
    //        var mintFunction = bridgeContract.GetFunction("mint");
    //        var burnFunction = bridgeContract.GetFunction("burn");

    //        // Prompt user for input and execute corresponding function call
    //        var menu = new SelectionPrompt<string>()
    //            .Title("Select an action:")
    //            .PageSize(4)
    //            .AddChoices(new[] { "Lock", "Unlock", "Mint", "Burn" });
    //        string choice = AnsiConsole.Prompt(menu);

    //        switch (choice)
    //        {
    //            case "Lock":
    //                decimal amountToLock = AnsiConsole.Prompt(new TextPrompt<decimal>("Enter the amount of tokens to lock:"));
    //                var gasPrice = await web3.Eth.GasPrice.SendRequestAsync();
    //                var gas = new HexBigInteger(1000000);
    //                var transactionInput = lockFunction.CreateTransactionInput(account.Address, new HexBigInteger(amountToLock * 1e18), gasPrice, gas);
    //                var transactionHash = await web3.Eth.TransactionManager.SendTransactionAsync(transactionInput);
    //                AnsiConsole.MarkupLine($"Lock transaction sent with hash [bold green]{transactionHash}[/]");
    //                break;
    //        }
    //    }

    //    public async Task Lock(Contract contract, string senderAddress, string privateKey)
    //    {
    //        var tokenAddress = AnsiConsole.Ask<string>("Enter the token address to lock: ");
    //        var amount = AnsiConsole.Ask<decimal>("Enter the amount to lock: ");

    //        var gas = new HexBigInteger(500000);

    //        var function = contract.GetFunction("lock");

    //        var transactionInput = function.CreateTransactionInput(senderAddress, gas, null, tokenAddress, new BigInteger(UnitConversion.Convert.ToWei(amount, 18)));

    //        var web3 = new Web3("YOUR_INFURA_URL_HERE");

    //        var signed = await web3.TransactionManager.SignTransactionAsync(transactionInput, senderAddress, new HexBigInteger(8000000000), new HexBigInteger(0));

    //        var transactionHash = await web3.Eth.Transactions.SendTransaction.SendRequestAsync(signed);

    //        AnsiConsole.MarkupLine($"[green]Transaction sent. Hash: {transactionHash}[/]");
    //    }

    //    static async Task Mint(Contract contract, string senderAddress, string privateKey)
    //    {
    //        var txHash = AnsiConsole.Ask<string>("Enter the transaction hash to mint: ");
    //        var tokenAddress = AnsiConsole.Ask<string>("Enter the token address of the original token: ");
    //        var amount = AnsiConsole.Ask<decimal>("Enter the amount to mint: ");
    //        var name = AnsiConsole.Ask<string>("Enter the name of the wrapped token: ");
    //        var symbol = AnsiConsole.Ask<string>("Enter the symbol of the wrapped token: ");

    //        var gas = new HexBigInteger(500000);

    //        var function = contract.GetFunction("mint");

    //        var transactionInput = function.CreateTransactionInput(senderAddress, gas, null, txHash, tokenAddress, new BigInteger(UnitConversion.Convert.ToWei(amount, 18)), name, symbol);

    //        var web3 = new Web3("YOUR_INFURA_URL_HERE");

    //        var signed = await web3.TransactionManager.SignTransactionAsync(transactionInput, senderAddress, new HexBigInteger(8000000000), new HexBigInteger(0));

    //        var transactionHash = await web3.Eth.Transactions.SendTransaction.SendRequestAsync(signed);

    //        AnsiConsole.MarkupLine($"[green]Transaction");
    //    }
    //}
}
