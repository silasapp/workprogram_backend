﻿using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public partial class RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION
    {
        public int Id { get; set; }
        public string? OML_ID { get; set; }
        public string? OML_Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Companyemail { get; set; }
        public string? Year_of_WP { get; set; }
        public string? Company_Annual_Year { get; set; }
        public string? Company_Annual_Oil { get; set; }
        public string? Company_Annual_Condensate { get; set; }
        public string? Company_Annual_AG { get; set; }
        public string? Company_Annual_NAG { get; set; }
        public string? Created_by { get; set; }
        public string? Updated_by { get; set; }
        public DateTime? Date_Created { get; set; }
        public DateTime? Date_Updated { get; set; }
        public string? Terrain { get; set; }
        public string? Contract_Type { get; set; }
        public string? Consession_Type { get; set; }
        public string? COMPANY_ID { get; set; }
    }
}
