using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
            //Инициализация БД
            if (db.Users.Count() < 1)
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
            //фабрика сокращённых ссылок
            var manager = new LinkManager();
            
            //Новая ссылка
            var link = manager.Cut(originalLink);//

            if (link == null)//Проверка на соответствие ссылки формату
            {
                ViewBag.Message = "Ошибка: Введённая строка не соответствует формату URL";
                return View("Index");
            }

            if (!User.Identity.IsAuthenticated)//Если пользователь не аутентифицирован
            {
                link.User = await db.Users.FirstOrDefaultAsync();//Используется анонимный пользователь
            }
            else
            {
                link.User = await db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync(); //Иначе ссылка связывается с пользователем
            }
            await db.Links.AddAsync(link);
            await db.SaveChangesAsync();

            return View(link);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> RemoveLink(int id)
        {
            var lnk = await db.Links.Include(l => l.User).Where(l => l.Id == id).FirstOrDefaultAsync();//Получение ссылки

            var user = await db.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();

            if (lnk.User.Email == User.Identity.Name || user.Role == "ADMIN")//Если ссылка принадлежит пользователю или пользователь в роли Администратора
            {
                db.Links.Remove(lnk); //Удаление ссылки из БД
                await db.SaveChangesAsync();
            }
            else
            {
                ViewBag.Message = "404";
                return View("Index");
            }
            return RedirectToAction("Index","Account");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
