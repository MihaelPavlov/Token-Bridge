using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

namespace Bridge_CLI.FunctionModels;

[Function("unlock")]
public class UnlockFunction : FunctionMessage
{
    [Parameter("string", "txHash", 1)]
    public string TxHash { get; set; } = null!;

    [Parameter("address", "originalToken", 2)]
    public string OriginalToken { get; set; } = null!;

    [Parameter("uint256", "amount", 3)]
    public BigInteger Amount { get; set; }
}
