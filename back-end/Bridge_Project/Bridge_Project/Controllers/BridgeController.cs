using Microsoft.AspNetCore.Mvc;
using Bridge_Project.Services.Services;
using Bridge_Project.Data.Models;

namespace Bridge_Project.Controllers;

[ApiController]
[Route("[controller]")]
public class BridgeController : Controller
{
    private readonly ISourceEventService eventService;

    public BridgeController(ISourceEventService eventService)
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
}
