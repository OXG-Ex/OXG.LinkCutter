using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OXG.LinkCutter.Data;
using OXG.LinkCutter.Models;

namespace OXG.LinkCutter.Controllers
{
    public class AccountController : Controller
    {
        private readonly CutterDbContext db;
        //Внедрение зависимостей
        public AccountController(CutterDbContext context)
        {
            db = context;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            IQueryable model;
            
            if (user.Role == "ADMIN")
            {
                 model = db.Links.Include(l => l.User);
            }
            else
            {
                 model = db.Links.Include(l => l.User).Where(l => l.User.Email == user.Email);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string pass)
        {
            User user = await db.Users.Where(u => u.Email == email && u.PasswordHash == pass.GetHashCode()).FirstOrDefaultAsync();
            if (user != null)
            {
                await Authenticate(email); // аутентификация
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string pass, string passConfirm)
        {
            //TODO: Валидация данных
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                // добавляем пользователя в бд
                db.Users.Add(new User { Email = email, PasswordHash = pass.GetHashCode(), Role = "user" });

                await db.SaveChangesAsync();

                await Authenticate(email); // аутентификация

                return RedirectToAction("Index", "Home");
            }
            else
            {
                //TODO: Валидация данных
            }
            return RedirectToAction("Index", "Home");
        }

        private async Task Authenticate(string userName)
        {
            
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            //объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}