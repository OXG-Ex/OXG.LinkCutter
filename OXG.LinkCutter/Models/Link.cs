using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OXG.LinkCutter.Models
{
    public class Link
    {
        public int Id { get; set; }

        public string OriginalLink { get; set; }

        public string CuttedLink { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
