using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OXG.LinkCutter.Data;
using OXG.LinkCutter.Models;

namespace OXG.LinkCutter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CutterDbContext db;
        private readonly IWebHostEnvironment env;

        public HomeController(ILogger<HomeController> logger, CutterDbContext context, IWebHostEnvironment environment)
        {
            _logger = logger;
            db = context;
            env = environment;
        }

        public async Task<IActionResult> Index()
        {
            var test = ("admin123").GetHashCode();
            //Инициализация БД
            if (db.Users.Count() == 0)
            {
                var user = new User() { Email = "anonymous@user.com", PasswordHash = 0, Role = "user" };//Анонимный пользователь, к нему будут привязываться ссылки от пользователей без аккаунтов
                var admin = new User() { Email = "admin@admin.com", PasswordHash = ("admin123").GetHashCode(), Role = "ADMIN" };//Аккаунт Администратора
                await db.Users.AddAsync(user);
                await db.Users.AddAsync(admin);
                await db.SaveChangesAsync();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CutLink(string originalLink)
        {
            //TODO: добавить проверку корректности. manager может вернуть null если ссылка не соответсвует регулярному выражению
            //TODO: добавить автоматическое создание анонимного пользователя
            var manager = new LinkManager();
            var link = manager.Cut(originalLink);
            if (!User.Identity.IsAuthenticated)
            {
                link.User = await db.Users.FirstOrDefaultAsync();
            }
            else
            {
                link.User = await db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync(); 
            }
            await db.Links.AddAsync(link);
            await db.SaveChangesAsync();

            var s = env.ApplicationName;

            return View(link);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> RemoveLink(int id)
        {
            var lnk = await db.Links.Where(l => l.Id == id).FirstOrDefaultAsync();
            db.Links.Remove(lnk);
            await db.SaveChangesAsync();
            return RedirectToAction("Index","Account");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
