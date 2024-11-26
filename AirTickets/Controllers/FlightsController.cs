using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AirTickets.Data;
using AirTickets.Models;

namespace AirTickets.Controllers
{
    public class FlightsController : Controller
    {
        private readonly AirlineTicketsContext _context;

        public FlightsController(AirlineTicketsContext context)
        {
            _context = context;
        }

        // GET: Flights
        public async Task<IActionResult> Index()
        {
            var airlineTicketsContext = _context.Flights.Include(f => f.AircraftNumberNavigation).Include(f => f.ArrivalAirportNavigation).Include(f => f.DepartureAirportNavigation);
            return View(await airlineTicketsContext.ToListAsync());
        }

        // GET: Flights/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.AircraftNumberNavigation)
                .Include(f => f.ArrivalAirportNavigation)
                .Include(f => f.DepartureAirportNavigation)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // GET: Flights/Create
        public IActionResult Create()
        {
            ViewData["AircraftNumber"] = _context.Planes
                .Include(p => p.Flights)
                .AsEnumerable()
                .Where(p => p.Flights.All(f => (DateTime.UtcNow - f.ArrivalDateTime).Hours > 12))
                .Select(p => new SelectListItem { Text = p.TailNumber + " " + p.ManufacturerName + " " + p.ModelName, Value = p.TailNumber });
            ViewData["ArrivalAirport"] = new SelectList(_context.Airports.Where(a => a.Status == "Functioning"), "Iatacode", "Iatacode");
            ViewData["DepartureAirport"] = new SelectList(_context.Airports.Where(a => a.Status == "Functioning"), "Iatacode", "Iatacode");
            return View();
        }

        // POST: Flights/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Number,AircraftNumber,DepartureAirport,ArrivalAirport,DepartureDateTime,ArrivalDateTime")] Flight flight)
        {
            if (ModelState.IsValid)
            {
                flight.DepartureDateTime = flight.DepartureDateTime.ToUniversalTime();
                flight.ArrivalDateTime = flight.ArrivalDateTime.ToUniversalTime();
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AircraftNumber"] = _context.Planes
                .Include(p => p.Flights)
                .AsEnumerable()
                .Where(p => p.Flights.All(f => (DateTime.UtcNow - f.ArrivalDateTime).Hours > 12))
                .Select(p => new SelectListItem { Text = p.TailNumber + " " + p.ManufacturerName + " " + p.ModelName, Value = p.TailNumber, Selected = flight.AircraftNumber == p.TailNumber });
            ViewData["ArrivalAirport"] = new SelectList(_context.Airports.Where(a => a.Status == "Functioning"), "Iatacode", "Iatacode", flight.ArrivalAirport);
            ViewData["DepartureAirport"] = new SelectList(_context.Airports.Where(a => a.Status == "Functioning"), "Iatacode", "Iatacode", flight.DepartureAirport);
            return View(flight);
        }

        // GET: Flights/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound();
            }
            ViewData["AircraftNumber"] = _context.Planes
                .Include(p => p.Flights)
                .AsEnumerable()
                .Where(p => p.Flights.All(f => (DateTime.UtcNow - f.ArrivalDateTime).Hours > 12))
                .Select(p => new SelectListItem { Text = p.TailNumber + " " + p.ManufacturerName + " " + p.ModelName, Value = p.TailNumber, Selected = flight.AircraftNumber == p.TailNumber });
            ViewData["ArrivalAirport"] = new SelectList(_context.Airports.Where(a => a.Status == "Functioning"), "Iatacode", "Iatacode", flight.ArrivalAirport);
            ViewData["DepartureAirport"] = new SelectList(_context.Airports.Where(a => a.Status == "Functioning"), "Iatacode", "Iatacode", flight.DepartureAirport);
            return View(flight);
        }

        // POST: Flights/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Number,AircraftNumber,DepartureAirport,ArrivalAirport,DepartureDateTime,ArrivalDateTime")] Flight flight)
        {
            if (id != flight.Number)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlightExists(flight.Number))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AircraftNumber"] = _context.Planes
                .Include(p => p.Flights)
                .AsEnumerable()
                .Where(p => p.Flights.All(f => (DateTime.UtcNow - f.ArrivalDateTime).Hours > 12))
                .Select(p => new SelectListItem { Text = p.TailNumber + " " + p.ManufacturerName + " " + p.ModelName, Value = p.TailNumber, Selected = flight.AircraftNumber == p.TailNumber });
            ViewData["ArrivalAirport"] = new SelectList(_context.Airports.Where(a => a.Status == "Functioning"), "Iatacode", "Iatacode", flight.ArrivalAirport);
            ViewData["DepartureAirport"] = new SelectList(_context.Airports.Where(a => a.Status == "Functioning"), "Iatacode", "Iatacode", flight.DepartureAirport);
            return View(flight);
        }

        // GET: Flights/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.AircraftNumberNavigation)
                .Include(f => f.ArrivalAirportNavigation)
                .Include(f => f.DepartureAirportNavigation)
                .FirstOrDefaultAsync(m => m.Number == id);
            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        // POST: Flights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FlightExists(string id)
        {
            return _context.Flights.Any(e => e.Number == id);
        }
    }
}
