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

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CutLink(string originalLink)
        {
            //var user = new User() { Email = "anonymous@user.com",PasswordHash="", Role="User" };
            //await db.Users.AddAsync(user);
            //await db.SaveChangesAsync();

            //TODO: добавить проверку корректности. manager может вернуть null если ссылка не соответсвует регулярному выражению

            var manager = new LinkManager();
            var link = manager.Cut(originalLink);
            if (!User.Identity.IsAuthenticated)
            {
                link.User = await db.Users.FirstOrDefaultAsync();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
