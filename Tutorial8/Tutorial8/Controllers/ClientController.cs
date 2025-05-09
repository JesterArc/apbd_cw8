using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetTrips(int id)
    {
        if (!await _clientService.DoesClientExist(id)){
            return NotFound("There is no client with id = " + id);
        }
        var trip = await _clientService.GetTripsByClientId(id);
        if (trip.Count == 0)
        {
            // "Client has no associated trips"
            return NoContent();
        }
        return Ok(trip);
    }
}