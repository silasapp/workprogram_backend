using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public partial class WP_TOTAL_INCIDENCE_AND_OIL_SPILL_AND_RECOVERED
    {
        public string? CompanyName { get; set; }
        public string? Year_of_WP { get; set; }
        public int? Frequency { get; set; }
        public int? Total_Quantity_Spilled { get; set; }
        public int? Total_Quantity_Recovered { get; set; }
    }
}
