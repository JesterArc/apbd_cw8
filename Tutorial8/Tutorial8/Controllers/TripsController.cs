using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tutorial8.Services;

namespace Tutorial8.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TripsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public TripsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }
        
    [HttpGet]
    public async Task<IActionResult> GetTrips()
    // returns all info about all trips
    {
        var trips = await _tripsService.GetTrips();
        return Ok(trips);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTrip(int id)
    // returns info about specific trip
    {
        if (!await _tripsService.DoesTripExist(id)){
            return NotFound("There is no trip with id = " + id);
        }
        var trip = await _tripsService.GetTripById(id);
        return Ok(trip);
    }
}