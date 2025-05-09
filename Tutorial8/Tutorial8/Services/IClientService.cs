using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientService
{
    public Task<List<Client_TripDTO>> GetTripsByClientId(int id);
    public Task<bool> DoesClientExist(int id);
}