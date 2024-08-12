using Lab3_NhatNguyen_WebApp.Models;
using Lab3_NhatNguyen_WebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lab3_NhatNguyen_WebApp.Controllers
{
    public class LoginController : Controller
    {
        public MovieWebAppContext _context;
        public LoginController(MovieWebAppContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var claimUser = HttpContext.User;
            if (claimUser.Identity.IsAuthenticated)
            {
                return Redirect("/movies");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(User user)
        {
            try
            {
                var foundUser = await _context.Users.FindAsync(user.Email);
                if (foundUser != null && PasswordCompare(user.Password,foundUser.Password))
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,foundUser.Email),
                        new Claim(ClaimTypes.Name,foundUser.Displayname)
                    };
                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                    };
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), properties);

                    return Redirect("/movies");
                }
                else
                {
                    ViewBag.Message = "Incorrect username or password. Please try again";
                    return View();
                }
            }
            catch (Exception)
            {
                throw;
                ViewBag.Message = "Something went wrong with server";
                return View();
            }

        }
        private bool PasswordCompare(string inputPassword, string userPassword)
        {
            var isMatch = BCrypt.Net.BCrypt.EnhancedVerify(inputPassword,userPassword);
            return isMatch;
        }
    }
    
}
