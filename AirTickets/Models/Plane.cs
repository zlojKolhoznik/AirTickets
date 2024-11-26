using System;
using System.Collections.Generic;

namespace AirTickets.Models;

public partial class Plane
{
    public string TailNumber { get; set; } = null!;

    public string ManufacturerName { get; set; } = null!;

    public string ModelName { get; set; } = null!;

    public int SeatsCount { get; set; }

    public DateTime LastInspectionDate { get; set; }

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
