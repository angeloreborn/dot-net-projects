using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stock_Manager_With_Search_Functionality.Models;

namespace Stock_Manager_With_Search_Functionality.Data
{
    public class Stock_Manager_With_Search_FunctionalityContext : DbContext
    {
        public Stock_Manager_With_Search_FunctionalityContext (DbContextOptions<Stock_Manager_With_Search_FunctionalityContext> options)
            : base(options)
        {
        }

        public DbSet<Stock_Manager_With_Search_Functionality.Models.Supplier> Supplier { get; set; }

        public DbSet<Stock_Manager_With_Search_Functionality.Models.Product> Product { get; set; }

        public DbSet<Stock_Manager_With_Search_Functionality.Models.Stock> Stock { get; set; }
    }
}
