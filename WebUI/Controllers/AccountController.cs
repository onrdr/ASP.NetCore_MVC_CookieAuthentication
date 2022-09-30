using DataAccess;
using Entities; 
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using System.Reflection;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public AccountController(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Login operations begins here
            }
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new()
                {
                    UserName = model.UserName,
                    Password = GetHashedPassword(model)
                };
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
            }
            return View(model);
        }

        public IActionResult Profile()
        {
            return View();
        }

        public string GetHashedPassword(RegisterViewModel model)
        {
            string md5Salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
            string saltedPassword = model.Password + md5Salt;
            string hashedPassword = saltedPassword.MD5();
            return hashedPassword;
        }
    }
}
