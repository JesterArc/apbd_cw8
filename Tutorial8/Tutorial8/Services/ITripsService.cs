using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface ITripsService
{
    // returns a list of trips with the added countries that those trips take place in from Trip table
    public Task<List<TripDTO>> GetTrips();
    // returns a trip with the added countries that this trip takes place in from Trip table
    public Task<TripDTO> GetTripById(int id);
    // returns if this id is tied to a trip in Trip table
    public Task<bool> DoesTripExist(int id);
    // returns if this trip has at least 1 open slot left
    public Task<bool> CanTripFitOneMore(int tripId);
}