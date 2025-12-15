using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR8.Classes
{
    public class Day
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public ICollection<Hour> Hours { get; set; }
    }
}
