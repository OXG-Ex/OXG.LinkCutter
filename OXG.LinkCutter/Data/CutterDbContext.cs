using Microsoft.EntityFrameworkCore;
using OXG.LinkCutter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OXG.LinkCutter.Data
{
    public class CutterDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Link> Links { get; set; }

        public CutterDbContext(DbContextOptions<CutterDbContext> options): base(options)
        {
            Database.EnsureCreated();
        }
    }
}
