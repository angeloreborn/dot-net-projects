using Stock_Manager_With_Search_Functionality.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock_Manager_With_Search_Functionality.Helpers
{
    public class ProductsSort
    {
        public IEnumerable<Product> products { get; set; }
        public string nameSort { get; set; }

        public IEnumerable<Product> result()
        {

            return products;
        }

 
    }
}
