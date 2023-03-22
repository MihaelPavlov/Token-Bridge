using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;

namespace Bridge_CLI.FunctionModels;

[Function("burn")]
internal class BurnFunction : FunctionMessage
{
    [Parameter("address", "wrappedToken", 1)]
    public string WrappedToken { get; set; } = null!;

    [Parameter("uint256", "amount", 2)]
    public BigInteger Amount { get; set; }
}
