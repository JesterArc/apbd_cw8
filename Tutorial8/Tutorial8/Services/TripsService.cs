using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    // Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=APBD;Integrated Security=True;
    private readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
    
    
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();
        string command = """
                         SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name FROM Trip t 
                         INNER JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip 
                         INNER JOIN Country c ON ct.IdCountry = c.IdCountry
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var id = reader.GetInt32(0);
                    var trip = trips.FirstOrDefault(e => e.Id.Equals(id));
                    if (trip == null)
                    {
                        trip = new TripDTO()
                        {
                            Id = id,
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            DateFrom = reader.GetDateTime(3),
                            DateTo = reader.GetDateTime(4),
                            MaxPeople = reader.GetInt32(5),
                            Countries = new List<CountryDTO>(),
                        };
                        trips.Add(trip);
                    }
                    String name = reader.GetString(6);
                    var country = trip.Countries.FirstOrDefault(e => e.Name.Equals(name));
                    if (country is null)
                    {
                        country = new CountryDTO()
                        {
                            Name = name
                        };
                        trip.Countries.Add(country);
                    }
                }
            }
            
        }
        return trips;
    }
}