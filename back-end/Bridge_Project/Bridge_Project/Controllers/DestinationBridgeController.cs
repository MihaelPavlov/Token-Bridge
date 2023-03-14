using Bridge_Project.Data.Enums;
using Bridge_Project.Data.Models;
using Bridge_Project.Services.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bridge_Project.Controllers;

public class DestinationBridgeController : Controller
{
    private readonly IDestinationEventService eventService;

    public DestinationBridgeController(IDestinationEventService eventService)
    {
        this.eventService = eventService;
    }

    [HttpGet("get-unclaimed-locked-events")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DestinationEvent>))]
    public async Task<IActionResult> GetUnclaimedLockEvents(CancellationToken cancellationToken)
    {
        var result = await this.eventService.GetAllLockEvents(cancellationToken);
        return this.Ok(result);
    }

    [HttpGet("get-unclaimed-burned-events")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DestinationEvent>))]
    public async Task<IActionResult> GetUnclaimedBurnEvents(CancellationToken cancellationToken)
    {
        var result = await this.eventService.GetAllBurnEvents(cancellationToken);
        return this.Ok(result);
    }

    [HttpGet("get-by-public-key-events")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DestinationEvent>))]
    public async Task<IActionResult> GetUnclaimedBurnEvents(string publicKey, CancellationToken cancellationToken)
    {
        var result = await this.eventService.GetAllByPublicKey(publicKey, cancellationToken);
        return this.Ok(result);
    }

    [HttpGet("get-by-type-events")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DestinationEvent>))]
    public async Task<IActionResult> GetUnclaimedBurnEvents(EventType type, CancellationToken cancellationToken)
    {
        var result = await this.eventService.GetAllByType(type, cancellationToken);
        return this.Ok(result);
    }
}
