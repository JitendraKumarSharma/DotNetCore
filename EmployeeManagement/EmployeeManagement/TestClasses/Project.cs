using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.TestClasses
{
    public class Project
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("Clients")]
        public int ClientId { get; set; }
        public Client Clients { get; set; } // Project HAS-a client. Name of Field will be [PropertyName] _[PropertyIDName] “Clients_ID”  
    }
}
