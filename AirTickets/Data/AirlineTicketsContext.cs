using System;
using System.Collections.Generic;
using AirTickets.Models;
using Microsoft.EntityFrameworkCore;

namespace AirTickets.Data;

public partial class AirlineTicketsContext : DbContext
{
    public AirlineTicketsContext()
    {
    }

    public AirlineTicketsContext(DbContextOptions<AirlineTicketsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<Plane> Planes { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.Iatacode).HasName("PK__Airports__EFD6F5BF56D3B4C4");

            entity.Property(e => e.Iatacode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("IATACode");
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.PassportNumber).HasName("PK__Customer__45809E70FE58A015");

            entity.HasIndex(e => e.PhoneNumber, "UQ__Customer__85FB4E3851A52C5A").IsUnique();

            entity.Property(e => e.PassportNumber).HasMaxLength(15);
            entity.Property(e => e.BaseCity).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Number).HasName("PK__Flights__78A1A19CB6CA1A84");

            entity.Property(e => e.Number).HasMaxLength(10);
            entity.Property(e => e.AircraftNumber).HasMaxLength(10);
            entity.Property(e => e.ArrivalAirport)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.DepartureAirport)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.AircraftNumberNavigation).WithMany(p => p.Flights)
                .HasForeignKey(d => d.AircraftNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flights__Aircraf__2E1BDC42");

            entity.HasOne(d => d.ArrivalAirportNavigation).WithMany(p => p.FlightArrivalAirportNavigations)
                .HasForeignKey(d => d.ArrivalAirport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flights__Arrival__300424B4");

            entity.HasOne(d => d.DepartureAirportNavigation).WithMany(p => p.FlightDepartureAirportNavigations)
                .HasForeignKey(d => d.DepartureAirport)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flights__Departu__2F10007B");
        });

        modelBuilder.Entity<Plane>(entity =>
        {
            entity.HasKey(e => e.TailNumber).HasName("PK__Planes__3F41D11A543C1A9C");

            entity.Property(e => e.TailNumber).HasMaxLength(10);
            entity.Property(e => e.ManufacturerName).HasMaxLength(20);
            entity.Property(e => e.ModelName).HasMaxLength(10);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => new { e.FlightNumber, e.SeatNumber }).HasName("PK_FlightNumber_SeatNumber");

            entity.Property(e => e.FlightNumber).HasMaxLength(10);
            entity.Property(e => e.SeatNumber).HasMaxLength(5);
            entity.Property(e => e.OwnerPassportNumber).HasMaxLength(15);
            entity.Property(e => e.Status).HasMaxLength(10);

            entity.HasOne(d => d.FlightNumberNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FlightNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__FlightN__31EC6D26");

            entity.HasOne(d => d.OwnerPassportNumberNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.OwnerPassportNumber)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__OwnerPa__32E0915F");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
