using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public partial class WP_GEOPHYSICAL_ACTIVITIES_ACQUISITION_sum_and_count
    {
        public int? Count_Contract_Type { get; set; }
        public string? Contract_Type { get; set; }
        public string? Year_of_WP { get; set; }
        public string? Geo_type_of_data_acquired { get; set; }
        public int? Actual_year_aquired_data { get; set; }
        public int? proposed_year_data { get; set; }
    }
}
