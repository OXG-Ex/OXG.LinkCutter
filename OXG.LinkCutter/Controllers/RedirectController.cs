using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OXG.LinkCutter.Data;

namespace OXG.LinkCutter.Controllers
{
    public class RedirectController : Controller
    {

        private readonly CutterDbContext db;
        //Внедрение зависимостей
        public RedirectController(CutterDbContext context)
        {
            db = context;
        }

        public async Task<IActionResult> Rdrc(string id) 
        {
            var lnk = await db.Links.Where(l => l.CuttedLink == id).FirstOrDefaultAsync();
            if (lnk!= null)
            {
                return Redirect(lnk.OriginalLink);
            }
            else
            {
                return Content("404");
            }
        }
    }
}