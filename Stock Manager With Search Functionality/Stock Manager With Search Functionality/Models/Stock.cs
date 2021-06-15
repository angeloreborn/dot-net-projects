using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock_Manager_With_Search_Functionality.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime DateCreated { get; set; }


    }
}
