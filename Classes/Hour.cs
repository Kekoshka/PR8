using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PR8.Classes
{
    public class Hour
    {
        public Guid Id { get; set; }
        public TimeOnly Time { get; set; }
        public int Temperature { get; set; }
        public int Humidity { get; set; }
        public int Pressure { get; set; }
    }
}
