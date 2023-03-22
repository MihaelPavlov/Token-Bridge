using Nethereum.Contracts;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Bridge_CLI.FunctionModels;

[Function("lock")]
public class LockFunction : FunctionMessage
{
    [Parameter("address", "sourceToken", 1)]
    public string SourceToken { get; set; } = null!;

    [Parameter("uint256", "amount", 2)]
    public BigInteger Amount { get; set; }
}
