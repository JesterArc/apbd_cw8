using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface ITripsService
{
    public Task<List<TripDTO>> GetTrips();
}