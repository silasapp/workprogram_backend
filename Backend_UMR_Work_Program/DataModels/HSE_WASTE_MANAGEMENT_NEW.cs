using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public partial class HSE_WASTE_MANAGEMENT_NEW
    {
        public int Id { get; set; }
        public string? OML_ID { get; set; }
        public string? OML_Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Companyemail { get; set; }
        public string? Year_of_WP { get; set; }
        public string? ACTUAL_year { get; set; }
        public string? PROPOSED_year { get; set; }
        public string? Do_you_have_Waste_Management_facilities { get; set; }
        public string? If_YES_is_the_facility_registered { get; set; }
        public string? If_NO_give_reasons_for_not_being_registered { get; set; }
        public string? Created_by { get; set; }
        public string? Updated_by { get; set; }
        public DateTime? Date_Created { get; set; }
        public DateTime? Date_Updated { get; set; }
        public string? Consession_Type { get; set; }
        public string? Terrain { get; set; }
        public string? Contract_Type { get; set; }
        public string? COMPANY_ID { get; set; }
    }
}
