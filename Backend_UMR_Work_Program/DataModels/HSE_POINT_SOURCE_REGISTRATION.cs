using System;
using System.Collections.Generic;
namespace Backend_UMR_Work_Program.Models
{
    public class HSE_POINT_SOURCE_REGISTRATION
    {
        public int Id { get; set; }
        public string? OML_ID { get; set; }
        public string? OML_Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Companyemail { get; set; }
        public string? Year_of_WP { get; set; }
        public string? Company_ID { get; set; }
        public string? Company_Number { get; set; }
        public string? areTherePointSourcePermit { get; set; }
        public string? evidenceOfPSPFilename { get; set; }
        public string? evidenceOfPSPPath { get; set; }
        public string? reasonForNoPSP { get; set; }

    }
}
