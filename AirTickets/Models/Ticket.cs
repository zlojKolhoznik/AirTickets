using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace AirTickets.Models;

public partial class Ticket
{
    public string FlightNumber { get; set; } = null!;

    public string SeatNumber { get; set; } = null!;

    public string OwnerPassportNumber { get; set; } = null!;

    [ValidateNever]
    public string Status { get; set; } = null!;

    [ValidateNever]
    public virtual Flight FlightNumberNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual Customer OwnerPassportNumberNavigation { get; set; } = null!;
}
