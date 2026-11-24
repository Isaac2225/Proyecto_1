using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class MyDBContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "User Id=alumne;Password=alumne;Data Source=10.2.97.150:1521/XEPDB1;";

            optionsBuilder.UseOracle(connectionString);
        }
    }
}
