using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using PacificTours.Server.Services;
using PacificTours.Shared.Entities;
using PacificTours.Shared;

namespace PacificTours.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : ControllerBase
{
    
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public PaymentsController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpPost("ReserveBooking")]
    public async Task ReserveBooking(PaymentInfoDto paymentInfo)
    {
        var bookings = await _context.Bookings.Where(b => b.UserId == _userManager.GetUserId(User) && b.Status == "In Progress").ToListAsync();
        var booking = bookings.FirstOrDefault();

        var payment = new Payment
        {
            BookingId = booking.Id,
            Amount = paymentInfo.Amount,
            DatePaid = DateTime.Now,
            Status = "Reservation"
        };

        booking.TotalCost -= paymentInfo.Amount;
        booking.Status = "Reserved";

        _context.Payments.Add(payment);
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }
    
    [HttpPost("ConfirmBooking")]
    public async Task ConfirmBooking(PaymentInfoDto paymentInfo)
    {
        var bookings = await _context.Bookings.Where(b => b.UserId == _userManager.GetUserId(User) && b.Status == "Reserved").ToListAsync();
        var booking = bookings.FirstOrDefault();

        var payment = new Payment
        {
            BookingId = booking.Id,
            Amount = paymentInfo.Amount,
            DatePaid = DateTime.Now,
            Status = "Confirmation"
        };

        booking.TotalCost -= paymentInfo.Amount;
        booking.Status = "Confirmed";

        _context.Payments.Add(payment);
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    [HttpGet("GetPayments")]
    public async Task<ActionResult<List<Payment>>> GetPayments(int id)
    {
        var payments = await _context.Payments.Where(p => p.BookingId == id).ToListAsync();
        return Ok(payments);
    }
        
}