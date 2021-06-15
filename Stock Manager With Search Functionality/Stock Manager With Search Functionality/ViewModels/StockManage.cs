using Stock_Manager_With_Search_Functionality.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock_Manager_With_Search_Functionality.ViewModels
{
    public class StockManage
    {
        public string ProductName { get; set; }
        public string ProductCatagory { get; set; }
        public double ProductPrice { get; set; }
        public int ProductID { get; set; }
        public int SupplierID { get; set; }
        public int StockID { get; set; }
        public int StockQuantity { get; set; }

        public DateTime DateUpdated { get; set; }
      
    }
}
