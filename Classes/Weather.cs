using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PR8.Classes
{
    public class Weather
    {
        public Guid Id { get; set; }
        public Position Position { get; set; }
        public ICollection<Day> Days { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
