using System.Diagnostics;
using AirTickets.Data;
using AirTickets.Models;
using AirTickets.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AirTickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthenticationService _authenticationService;
        private readonly AirlineTicketsContext _context;

        public HomeController(ILogger<HomeController> logger, IAuthenticationService authenticationService, AirlineTicketsContext context)
        {
            _logger = logger;
            _authenticationService = authenticationService;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.IsAuthenticated = _authenticationService.IsLoggedIn();
            var city = _authenticationService.CurrentUser?.BaseCity;
            ViewBag.City = city;
            IEnumerable<Flight> flights = _context.Flights.Include(f => f.DepartureAirportNavigation)
                .Include(f => f.ArrivalAirportNavigation)
                .Include(f => f.AircraftNumberNavigation)
                .Where(f => f.DepartureAirportNavigation.City == city || city == null)
                .OrderBy(x => Guid.NewGuid())
                .Take(10)
                .ToList();
            return View(flights);
        }

        [Route("/search")]
        public async Task<IActionResult> Search(string query)
        {
            var comparison = StringComparison.InvariantCultureIgnoreCase;
            var flightsByDeparture = _context.Flights.Include(f => f.DepartureAirportNavigation)
                .Include(f => f.ArrivalAirportNavigation)
                .Include(f => f.AircraftNumberNavigation)
                .AsEnumerable()
                .Where(f => f.DepartureAirportNavigation.City.Contains(query, comparison) || string.Equals(f.DepartureAirport, query, comparison))
                .ToList();
            var flightsByArrival = _context.Flights.Include(f => f.DepartureAirportNavigation)
                .Include(f => f.ArrivalAirportNavigation)
                .Include(f => f.AircraftNumberNavigation)
                .AsEnumerable()
                .Where(f => f.ArrivalAirportNavigation.City.Contains(query, comparison) || string.Equals(f.ArrivalAirport, query, comparison))
                .ToList();
            var flights = flightsByDeparture.Concat(flightsByArrival).Distinct().ToList();
            return View(flights);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
