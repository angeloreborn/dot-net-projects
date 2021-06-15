using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock_Manager_With_Search_Functionality.Models
{
    // Minimized (No Annotations) -> GoTo Course On Annotations
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }

        public double Price { get; set; }

        public int Supplier { get; set; }

    }
}
