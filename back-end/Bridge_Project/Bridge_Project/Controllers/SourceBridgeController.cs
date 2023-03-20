using Microsoft.AspNetCore.Mvc;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Bridge_Project.Deploy;
using Bridge_Project.FunctionMessages;
using Bridge_Project.EventsDTO;
using Bridge_Project.Services.Services;
using Bridge_Project.Data.Models;
using Bridge_Project.Data.Enums;

namespace Bridge_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class SourceBridgeController : Controller
{
    private string ContractAddress = "0xbc04519E66ce894b50587b9C8ac2aAB07F24fFB9";

    private readonly ISourceEventService eventService;

    public SourceBridgeController(ISourceEventService eventService)
    {
        this.eventService = eventService;
    }

    [HttpGet("get-all")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BridgeEvent>))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await this.eventService.GetAll(cancellationToken);
        return this.Ok(result);
    }

    [HttpGet("get-by-public-key")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BridgeEvent>))]
    public async Task<IActionResult> GetAllByPublicKey(string publicKey, CancellationToken cancellationToken)
    {
        var result = await this.eventService.GetAllByPublicKey(publicKey, cancellationToken);
        return this.Ok(result);
    }

    [HttpGet("get-all-for-claiming")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BridgeEvent>))]
    public async Task<IActionResult> GetAllEventsForClaiming(CancellationToken cancellationToken)
    {
        var result = await this.eventService.GetAllEventsForClaiming(cancellationToken);
        return this.Ok(result);
    }

    [HttpGet("get-all-for-releasing")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BridgeEvent>))]
    public async Task<IActionResult> GetAllEventsForReleasing(CancellationToken cancellationToken)
    {
        var result = await this.eventService.GetAllEventsForReleasing(cancellationToken);
        return this.Ok(result);
    }

    //[HttpGet]
    //public async Task<IActionResult> Index([FromQuery] string sourceToken, [FromQuery] int amount)
    //{
    //    var privateKey = @"901c60d49816230df335301b17e9aa81f89dd340b1ad90eb06b008df8a54a4b1";

    //    var account = new Account(privateKey);
    //    var web3 = new Web3(account, @"https://sepolia.infura.io/v3/31d722098d4e48929c96519ba339b2d0");

    //    var lockFunctionMessage = new LockFunction
    //    {
    //        SourceToken = sourceToken,
    //        Amount = 1000000000,
    //    };

    //    var lockHandler = web3.Eth.GetContractTransactionHandler<LockFunction>();
    //    var receipt = await lockHandler.SendRequestAndWaitForReceiptAsync(ContractAddress, lockFunctionMessage);

    //    var even = receipt.DecodeAllEvents<LockEventDTO>();
    //    return this.Ok(even);
    //}

    //[HttpGet("Deploy")]
    //public async Task<IActionResult> DeployBridgeToEthereum()
    //{
    //    var privateKey = "7b5652f8815e3fcd3f939dc89177fbec0e71cfd3d2cf4d0dd322c4d855aba35c";

    //    var account = new Account(privateKey);
    //    var web3 = new Web3(account, @"https://sepolia.infura.io/v3/31d722098d4e48929c96519ba339b2d0");

    //    var deployMessage = new StandardTokenDeployment();

    //    var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardTokenDeployment>();
    //    var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deployMessage);

    //    ContractAddress = transactionReceipt.ContractAddress;

    //    return this.Ok(transactionReceipt.ContractAddress);
    //}
}
