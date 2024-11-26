using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AirTickets.Models;

public partial class Airport
{
    [Display(Name = "IATA Code")]
    public string Iatacode { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual ICollection<Flight> FlightArrivalAirportNavigations { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightDepartureAirportNavigations { get; set; } = new List<Flight>();
}
