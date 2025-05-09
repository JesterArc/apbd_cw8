using Microsoft.AspNetCore.Mvc;
using Tutorial8.Models.DTOs;
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
    // returns a list of trips (with extra information) that client with this id is on
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

    [HttpPost]
    public async Task<IActionResult> CreateClient([FromBody] ClientDTO clientDto)
    // if FromBody parameters are valid, creates a new client
    {
        if (!ValidateStringLength(clientDto.FirstName, 1, 120))
            return BadRequest("First name must be 120 characters or fewer but not zero");
        if (!ValidateStringLength(clientDto.LastName, 1, 120))
            return BadRequest("Last name must be 120 characters or fewer but not zero");
        // writing email validation makes my head hurt
        if (!ValidateStringLength(clientDto.Email, 1, 120))
            return BadRequest("Email must be 120 characters or fewer but not zero");
        // there are many standards for telephone numbers, so i decieded not to check for specific length
        if (!ValidateStringLength(clientDto.Telephone, 1, 120))
        {
            return BadRequest("Telephone number must be 120 characters or fewer but not zero");
        }
        if (!ValidatePesel(clientDto.Pesel))
            return BadRequest("Pesel is invalid");
        try
        {
            await _clientService.AddNewClientAsync(clientDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        return CreatedAtAction(nameof(CreateClient), new {IdClient = clientDto.IdClient}, clientDto);
    }

    [HttpPut("{id}/trips/{tripId}")]
    public async Task<IActionResult> AddClientToTrip(int id, int tripId)
    // if there is room on a trip, adds a client (id) onto it (tripid)
    {
        if (!await _clientService.DoesClientExist(id))
        {
            return NotFound("There is no client with id = " + id);
        }
        // to avoid repeating the next 2 methods in ClientContructor, I just created a new instance of TripsService and called them on it
        var tripsService = new TripsService();
        if (!await tripsService.DoesTripExist(tripId))
        {
            return NotFound("There is no trip with id = " + tripId);
        }

        if (!await tripsService.CanTripFitOneMore(tripId))
        {
            return BadRequest("No more room on that trip");
        }
        var year = DateTime.Now.Year;
        var month = DateTime.Now.Month;
        var day = DateTime.Now.Day;
        var registerTime = year * 10000 + month * 100 + day;
        try
        {
            await _clientService.PutClientOntoATrip(tripId, id, registerTime);
            return CreatedAtAction(nameof(AddClientToTrip), new {IdClient = id, IdTrip = tripId, registeredAt = registerTime});
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}/trips/{tripId}")]
    public async Task<IActionResult> RemoveClientFromTrip(int id, int tripId)
    // removes client from a trip, returns a message if delete was successful, but removed nothing
    {
        if (!await _clientService.DoesClientTripExist(id, tripId))
        {
            return Ok("There is no client with id = " + id + " going on a trip with id = " + tripId);
        }
        await _clientService.RemoveClientFromATrip(id, tripId);
        return NoContent();
    }
    
    public static bool ValidatePesel(string pesel)
    {
        if (pesel.Length != 11)
            return false;
        if (!pesel.Any(char.IsDigit))
        {
            return false;
        }

        var sum = 0;
        List<int> peselNumbers = [1, 3, 7, 9]; 
        for (int i = 0; i < 10; i++)
        {
            sum += (pesel[i] - '0') * peselNumbers[i%4];
        }
        return (10 - sum % 10 == pesel[10] - '0');
    }
    public static bool ValidateStringLength(string name, int minLength, int maxLength)
    // added because I would have written these lines more times than it takes to make this method
    {
        if (name.Length < minLength || name.Length > maxLength)
            return false;
        return true;
    }
}