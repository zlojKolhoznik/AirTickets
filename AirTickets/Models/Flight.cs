using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace AirTickets.Models;

public partial class Flight
{
    public string Number { get; set; } = null!;

    public string AircraftNumber { get; set; } = null!;

    public string DepartureAirport { get; set; } = null!;

    public string ArrivalAirport { get; set; } = null!;

    public DateTime DepartureDateTime { get; set; }

    public DateTime ArrivalDateTime { get; set; }

    [ValidateNever]
    public virtual Plane AircraftNumberNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Airport ArrivalAirportNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Airport DepartureAirportNavigation { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
