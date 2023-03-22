using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

namespace Bridge_CLI.FunctionModels;

[Function("mint")]
public class MintFunction : FunctionMessage
{
    [Parameter("string", "txHash", 1)]
    public string TxHash { get; set; } = null!;

    [Parameter("address", "sourceToken", 2)]
    public string SourceToken { get; set; } = null!;

    [Parameter("uint256", "amount", 3)]
    public BigInteger Amount { get; set; }

    [Parameter("string", "name", 4)]
    public string Name { get; set; } = null!;

    [Parameter("string", "symbol", 5)]
    public string Symbol { get; set; } = null!;
}
