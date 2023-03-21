namespace Bridge_Project.BacgroundServices.Models;

public class Network
{
    public string PrivateKey { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string BridgeContractAddress { get; set; } = null!;
    public string DeployedContractBlockNumber { get; set; } = null!;
    public int BatchSizeEvents { get; set; }
}
