using AirTickets.Services;
using Microsoft.AspNetCore.Mvc;

namespace AirTickets.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string phoneNumber, string password)
        {
            if (ModelState.IsValid && await _authenticationService.AuthenticateAsync(phoneNumber, password))
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid phone number or password.");
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string passportNumber, string phoneNumber, string password, string firstName, string lastName, string? baseCity)
        {
            if (ModelState.IsValid && await _authenticationService.RegisterAsync(passportNumber, phoneNumber, password, firstName, lastName, baseCity))
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Account with this passport number is already registered.");
            return View();
        }

        public IActionResult Logout()
        {
            _authenticationService.Logout();
            return RedirectToAction("Index", "Home");
        }
    }
}
