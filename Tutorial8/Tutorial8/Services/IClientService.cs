using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientService
{
    // returns list of trips with extra information for Client with this id
    public Task<List<Client_TripDTO>> GetTripsByClientId(int id);
    // returns if this id is tied to a client in the Client table
    public Task<bool> DoesClientExist(int id);
    // adds a new client to the database
    public Task AddNewClientAsync(ClientDTO ClientDto);
    // adds a new clienttrip row with specified values
    public Task PutClientOntoATrip(int tripId, int id, int registeredAt);
    // removes a row from clienttrip with specified values
    public Task RemoveClientFromATrip(int tripId, int id);
    // returns if this id and tripId are tied to a row in client_trip table
    public Task<bool> DoesClientTripExist(int id, int tripId);
}