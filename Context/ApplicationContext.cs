using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PR8.Classes;

namespace PR8.Context
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Weather> Weathers { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Hour> Hours { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=10.0.201.112;User Id=ISP_22_1_1;Password=ck2PjQQBIo15_;Database=base1_ISP_22_1_1;Encrypt=false");
        }
    }
}
