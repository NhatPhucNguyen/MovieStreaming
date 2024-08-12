using Lab3_NhatNguyen_WebApp.Models;
using Lab3_NhatNguyen_WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace Lab3_NhatNguyen_WebApp.Controllers
{
    public class SignupController : Controller
    {
        public MovieWebAppContext _context;
        public SignupController(MovieWebAppContext context)
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
        public async Task<IActionResult> Index(User user,int month,int day,int year)
        {
            try
            {
                var foundUser = await _context.Users.FindAsync(user.Email);
                if(foundUser != null)
                {
                    ViewBag.Message = "Email is already in user. Please try another one";
                    return View();
                }
                user.DateOfBirth = new DateTime(year, month, day);
                string hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password, 13);
                user.Password = hashPassword;
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                ViewBag.Message = e.Message;
                return View();
            }
            return Redirect("/login");
        }
    }
}
