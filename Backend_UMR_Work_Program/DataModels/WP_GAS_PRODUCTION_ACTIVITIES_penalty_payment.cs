using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public partial class WP_GAS_PRODUCTION_ACTIVITIES_penalty_payment
    {
        public string? CompanyName { get; set; }
        public string? Year_of_WP { get; set; }
        public int? Flared { get; set; }
        public int? penaltyfeepaid { get; set; }
        public int? Amount_Paid { get; set; }
    }
}
