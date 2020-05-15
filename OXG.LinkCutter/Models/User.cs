using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OXG.LinkCutter.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public int PasswordHash { get; set; }

        public string Role { get; set; }

        public List<Link> Links { get; set; }
    }
}
