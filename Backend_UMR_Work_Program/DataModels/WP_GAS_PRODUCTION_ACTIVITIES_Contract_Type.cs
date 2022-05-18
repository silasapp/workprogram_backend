using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public partial class WP_GAS_PRODUCTION_ACTIVITIES_Contract_Type
    {
        public string? Year_of_WP { get; set; }
        public string? Contract_Type { get; set; }
        public int? Actual_Total_Gas_Produced { get; set; }
        public int? Utilized_Gas_Produced { get; set; }
        public int? Flared_Gas_Produced { get; set; }
    }
}
