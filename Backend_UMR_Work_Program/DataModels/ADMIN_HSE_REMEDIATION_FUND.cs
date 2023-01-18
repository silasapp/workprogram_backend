using System;
using System.Collections.Generic;

namespace Backend_UMR_Work_Program.Models
{
    public class ADMIN_HSE_REMEDIATION_FUND
    {
        public int Id { get; set; }
        public string? OML_ID { get; set; }
        public string? OML_Name { get; set; }
        public string? CompanyName { get; set; }
        public string? Companyemail { get; set; }
        public string? Year_of_WP { get; set; }
        public string? Company_ID { get; set; }
        public string? Company_Number { get; set; }
        public string? Evidence_Of_Payment_Filename { get; set; }
        public string? Evidence_Of_Payment_Path { get; set; }
        public string? Reason_For_No_Remediation { get; set; }
       
    }
}