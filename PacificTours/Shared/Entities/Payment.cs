namespace PacificTours.Shared.Entities;

public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int Amount { get; set; }
    public DateTime DatePaid { get; set; }
    public string Status { get; set; }

    public Booking Booking { get; set; }
}