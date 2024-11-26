using AirTickets.Models;

namespace AirTickets.Services
{
    public interface IAuthenticationService
    {
        Customer? CurrentUser { get; }
        Task<bool> AuthenticateAsync(string phoneNumber, string password);
        Task<bool> RegisterAsync(string passportNumber, string phoneNumber, string password, string firstName, string lastName, string? baseCity = null);
        void Logout();
        bool IsLoggedIn();
    }
}
