using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Stock_Manager_With_Search_Functionality.Models
{
    // National (South African Only)
    // Minimized (No Annotations) -> GoTo Course On Annotations
    
    public class Supplier
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public int PhoneNumber { get; set; }
    }



}
