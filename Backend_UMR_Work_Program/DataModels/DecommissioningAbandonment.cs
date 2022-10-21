using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public partial class DecommissioningAbandonment
    {
        public int Id { get; set; }
        public string? Decommissioning { get; set; }
        public string? Abandonment { get; set; }
        public string? CAPEX { get; set; }
        public string? OPEX { get; set; }
        public int? CompanyNumber { get; set; }
        public int? ConcessionID { get; set; }
        public int? FieldID { get; set; }
        public int? DateCreated { get; set; }
        public int? Year { get; set; }
    }
}
