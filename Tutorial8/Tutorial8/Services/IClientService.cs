using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientService
{
    public Task<List<Client_TripDTO>> GetTripsByClientId(int id);
    public Task<bool> DoesClientExist(int id);
    public Task AddNewClientAsync(ClientDTO ClientDto);
    public Task PutClientOntoATrip(int tripId, int id, int registeredAt);
}