namespace Tutorial8.Models.DTOs;

public class Client_TripDTO
{
    public TripDTO trip { get; set; }
    public int RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}