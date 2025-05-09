using System.Data.Common;
using Microsoft.Data.SqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class ClientService : IClientService
{
    private readonly string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=apbd;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
    
    public async Task<List<Client_TripDTO>> GetTripsByClientId(int clientId)
    {
        var trips = new List<Client_TripDTO>();
        string command = """
                         Select t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name, ct.RegisteredAt, ISNULL(ct.PaymentDate, 0) as PaymentDate from Trip t
                         JOIN dbo.Client_Trip ct on t.IdTrip = ct.IdTrip
                         JOIN dbo.Country_Trip ctr on t.IdTrip = ctr.IdTrip
                         JOIN dbo.Country c on ctr.IdCountry = c.IdCountry
                         WHERE ct.IdClient = @IdClient
                         """;
        
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@IdClient", clientId);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var id = reader.GetInt32(0);
                    var clientTrip = trips.FirstOrDefault(e => e.trip.Id.Equals(id));
                    if (clientTrip == null)
                    {
                        clientTrip = new Client_TripDTO()
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
                            },
                            RegisteredAt = reader.GetInt32(7),
                            PaymentDate = reader.GetInt32(8)
                        };
                        trips.Add(clientTrip);
                    }
                    String name = reader.GetString(6);
                    var country = clientTrip.trip.Countries.FirstOrDefault(e => e.Name.Equals(name));
                    if (country is null)
                    {
                        country = new CountryDTO()
                        {
                            Name = name
                        };
                        clientTrip.trip.Countries.Add(country);
                    }
                }
            }
        }
        return trips;
    }

    public async Task<bool> DoesClientExist(int id)
    {
        var quantity = 0;
        string command = "Select count(1) from Client where IdClient = @IdClient";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@IdClient", id);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    quantity = reader.GetInt32(0);
                }
            }
        }
        return quantity > 0;
    }

    public async Task AddNewClientAsync(ClientDTO clientDto)
    {
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        try
        {
            command.CommandText = @"Insert into Client (FirstName, LastName, Email, Telephone, Pesel) 
                                    values (@Name, @Surname, @Email, @Telephone, @Pesel)";
            command.Parameters.AddWithValue("@Name", clientDto.FirstName);
            command.Parameters.AddWithValue("@Surname", clientDto.LastName);
            command.Parameters.AddWithValue("@Email", clientDto.Email);
            command.Parameters.AddWithValue("@Telephone", clientDto.Telephone);
            command.Parameters.AddWithValue("@Pesel", clientDto.Pesel);
            
            await command.ExecuteNonQueryAsync();
            command.Parameters.Clear();
            command.CommandText = @"Select @@Identity";
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    clientDto.IdClient = (int) reader.GetDecimal(0);
                }
            }
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task PutClientOntoATrip(int tripId, int id, int registeredAt)
    {
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        try
        {
            command.CommandText = @"Insert into Client_Trip Values (@id, @tripId , @registeredAt, null)";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@tripId", tripId);
            command.Parameters.AddWithValue("@registeredAt", registeredAt);
            await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }

    public async Task RemoveClientFromATrip(int id, int tripId)
    {
        await using SqlConnection connection = new SqlConnection(_connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;
        try
        {
            command.CommandText = @"Delete from Client_Trip where IdTrip = @tripId and IdClient = @id";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@tripId", tripId);
            await command.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<bool> DoesClientTripExist(int id, int tripId)
    {
        var quantity = 0;
        var command = @"Select Count(1) from Client_Trip where IdClient = @Id and IdTrip = @tripId";
        using (SqlConnection conn = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("@tripId", tripId);
            cmd.Parameters.AddWithValue("@Id", id);
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    quantity = reader.GetInt32(0);
                }
            }
        }
        return quantity > 0;
    }
}