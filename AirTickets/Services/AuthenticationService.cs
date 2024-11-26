
using AirTickets.Data;
using AirTickets.Models;
using Microsoft.EntityFrameworkCore;

namespace AirTickets.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IServiceProvider _services;

        public AuthenticationService(IServiceProvider services)
        {
            _services = services;
        }

        public Customer? CurrentUser { get; private set; }

        public async Task<bool> AuthenticateAsync(string phoneNumber, string password)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AirlineTicketsContext>();
            var customer = await context.Customers.FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber && c.PasswordHash == password);
            if (customer is not null)
            {
                CurrentUser = customer;
                return true;
            }

            return false;
        }

        public async Task<bool> RegisterAsync(string passportNumber, string phoneNumber, string password, string firstName, string lastName, string? baseCity = null)
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AirlineTicketsContext>();
            if (await context.Customers.AnyAsync(c => c.PassportNumber == passportNumber))
            {
                return false;
            }

            var customer = new Customer
            {
                PassportNumber = passportNumber,
                PhoneNumber = phoneNumber,
                PasswordHash = password,
                FirstName = firstName,
                LastName = lastName,
                BaseCity = baseCity
            };
            await context.Customers.AddAsync(customer);
            await context.SaveChangesAsync();
            CurrentUser = customer;
            return true;
        }

        public bool IsLoggedIn() => CurrentUser is not null;

        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
