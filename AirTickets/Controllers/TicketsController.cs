using AirTickets.Data;
using AirTickets.Models;
using AirTickets.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirTickets.Controllers
{
    public class TicketsController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly AirlineTicketsContext _context;

        public TicketsController(AirlineTicketsContext context, IAuthenticationService authenticationService)
        {
            _context = context;
            _authenticationService = authenticationService;
        }

        public IActionResult Index()
        {
            if (!_authenticationService.IsLoggedIn())
            {
                return RedirectToAction("Login", "Authentication");
            }

            var tickets = _context.Tickets
                .Include(t => t.FlightNumberNavigation)
                .Where(t => t.OwnerPassportNumber == _authenticationService.CurrentUser!.PassportNumber)
                .ToList();
            return View(tickets);
        }

        // GET: /book/LAX434
        [Route("/book/{flightNumber}")]
        public async Task<IActionResult> BookAsync(string flightNumber)
        {
            if (!_authenticationService.IsLoggedIn())
            {
                return RedirectToAction("Login", "Authentication");
            }

            var flight = await _context.Flights.Include(f => f.AircraftNumberNavigation).FirstOrDefaultAsync(f => f.Number == flightNumber);
            if (flight is null)
            {
                return NotFound("Flight with this number doest not exist");
            }

            ViewBag.FlightNumber = flightNumber;
            ViewBag.OwnerPassportNumber = _authenticationService.CurrentUser!.PassportNumber;
            ViewBag.AvailableSeats = Enumerable.Range(1, flight.AircraftNumberNavigation.SeatsCount).Select(p => p.ToString())
                .Except(_context.Tickets.Where(t => t.FlightNumber == flightNumber && t.Status != "Returned").Select(t => t.SeatNumber)).ToList();
            return View();
        }

        // POST: /book/LAX434
        [HttpPost]
        [Route("/book/{flightNumber}")]
        public async Task<IActionResult> BookAsync([Bind("FlightNumber,OwnerPassportNumber,SeatNumber")] Ticket ticket, [FromRoute] string flightNumber)
        {
            if (ModelState.IsValid)
            {
                ticket.Status = "Booked";
                await _context.Tickets.AddAsync(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var flight = await _context.Flights.Include(f => f.AircraftNumberNavigation).FirstOrDefaultAsync(f => f.Number == flightNumber);
            if (flight is null)
            {
                return NotFound("Flight with this number doest not exist");
            }

            ViewBag.AvailableSeats = Enumerable.Range(1, flight.AircraftNumberNavigation.SeatsCount).Select(p => p.ToString())
                .Except(_context.Tickets.Where(t => t.FlightNumber == flightNumber).Select(t => t.SeatNumber)).ToList();
            return View();
        }

        // GET: /return/LAX434
        [Route("/return/{flightNumber}")]
        public async Task<IActionResult> ReturnAsync(string flightNumber)
        {
            if (!_authenticationService.IsLoggedIn())
            {
                return RedirectToAction("Login", "Authentication");
            }
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.FlightNumber == flightNumber && t.OwnerPassportNumber == _authenticationService.CurrentUser!.PassportNumber);
            if (ticket is null)
            {
                return NotFound("Ticket with this flight number doest not exist");
            }
            ticket.Status = "Returned";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: /pay/LAX434
        [Route("/pay/{flightNumber}")]
        public async Task<IActionResult> PayAsync(string flightNumber)
        {
            if (!_authenticationService.IsLoggedIn())
            {
                return RedirectToAction("Login", "Authentication");
            }
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.FlightNumber == flightNumber && t.OwnerPassportNumber == _authenticationService.CurrentUser!.PassportNumber);
            if (ticket is null)
            {
                return NotFound("Ticket with this flight number doest not exist");
            }
            if (ticket.Status == "Bought")
            {
                return BadRequest("Ticket is already paid for");
            }
            ticket.Status = "Bought";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
