using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessCardWebAPI.Core.Data
{
    public class BusinessCardDbContext:DbContext
    {
        public BusinessCardDbContext(DbContextOptions options):base(options) 
        {
        
        }

        public DbSet<Users> Users { get; set; }
    }
}
