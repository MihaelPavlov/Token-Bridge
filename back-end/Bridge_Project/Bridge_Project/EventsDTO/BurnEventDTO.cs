using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace Bridge_Project.EventsDTO;

[Event("TokenBurned")]
public class BurnEventDTO : IEventDTO
{
    [Parameter("address", "token", 1, true)]
    public string Token { get; set; } = null!;

    [Parameter("address", "sender", 2, true)]
    public string Sender { get; set; } = null!;

    [Parameter("uint256", "amount", 3, false)]
    public BigInteger Amount { get; set; }
}
