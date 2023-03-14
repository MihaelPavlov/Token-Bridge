using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Bridge_Project.EventsDTO;

[Event("TokenUnlocked")]
public class UnlockEventDTO : IEventDTO
{
    [Parameter("string", "txHash", 1, false)]
    public string TxHash { get; set; } = null!;

    [Parameter("address", "token", 2, true)]
    public string Token { get; set; } = null!;

    [Parameter("address", "to", 3, true)]
    public string To { get; set; } = null!;

    [Parameter("uint256", "amount", 4, false)]
    public BigInteger Amount { get; set; }
}
