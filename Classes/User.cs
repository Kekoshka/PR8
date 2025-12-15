using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PR8.Classes
{
    public class User
    {
        public int Id { get; set; }
        public IPAddress UserIp { get; set; }
        public int RequestCount { get; set; }
        public ICollection<Weather> Weathers { get; set; }
    }
}
