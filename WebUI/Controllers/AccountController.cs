using DataAccess;
using Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
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

        #region Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = GetUserIfLoginInformationIsCorrect(model);
            if (user is null)
            {
                ModelState.AddModelError("", "Username or password is incorrect!!!");
                return View(model);
            }

            if (user.Locked)
            {
                ModelState.AddModelError(nameof(model.UserName), "User is locked");
                return View(model);
            }

            SetClaimsAndCookiesThenSignIn(user);
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (UsernameExists(model))
            {
                ModelState.AddModelError(nameof(model.UserName), "Username already exists!!!");
                return View(model);
            }

            await SaveUserToDB(model);
            return RedirectToAction(nameof(Login));
        }

        #endregion

        #region Profile
        [Authorize]
        public IActionResult Profile()
        {
            ProfileInfoLoader();
            return View();
        }

        [HttpPost]
        public IActionResult ProfileChangeFullName([Required][StringLength(50)] string? fullname)
        {
            if (ModelState.IsValid)
            {
                ChangeFullName(fullname);
                return RedirectToAction(nameof(Profile));
            }
            ProfileInfoLoader();
            return View(nameof(Profile));
        }


        [HttpPost]
        public IActionResult ProfileChangePassword([Required][MinLength(6)][MaxLength(16)] string? password)
        {
            if (ModelState.IsValid)
            {
                ChangePassword(password);
                ViewData["result"] = "PasswordChanged";
            }
            ProfileInfoLoader();
            return View(nameof(Profile));
        }


        #endregion

        #region Logout
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
        #endregion

        #region Functions
        private async Task SaveUserToDB(RegisterViewModel model)
        {
            User user = new()
            {
                UserName = model.UserName,
                Password = GetHashedPassword(model.Password)
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        private void ChangeFullName(string? fullname)
        {
            Guid userId = new(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _dbContext.Users.SingleOrDefault(u => u.Id == userId);
            user.FullName = fullname;
            _dbContext.SaveChanges();
        }

        private void ChangePassword(string? password)
        {
            Guid userId = new(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _dbContext.Users.SingleOrDefault(u => u.Id == userId);
            var hashedPassword = GetHashedPassword(password);
            user.Password = hashedPassword;
            _dbContext.SaveChanges();
        }

        private void ProfileInfoLoader()
        {
            Guid userId = new(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _dbContext.Users.SingleOrDefault(u => u.Id == userId);
            ViewData["FullName"] = user.FullName;
        }

        public string GetHashedPassword(string str)
        {
            string md5Salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
            string salted = str + md5Salt;
            string hashed = salted.MD5();
            return hashed;
        }

        public bool UsernameExists(BaseLoginRegister model)
        {
            return _dbContext.Users.Any(u => u.UserName.ToLower().Equals(model.UserName.ToLower()));
        }

        public User GetUserIfLoginInformationIsCorrect(BaseLoginRegister model)
        {
            string hashedPassword = GetHashedPassword(model.Password);
            User user = _dbContext.Users.SingleOrDefault(u => u.UserName.ToLower().Equals(model.UserName.ToLower())
                && u.Password.Equals(hashedPassword));

            return user;
        }

        private void SetClaimsAndCookiesThenSignIn(User? user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Username", user.UserName ?? string.Empty)
            };

            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new(identity);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
        #endregion
    }
}
