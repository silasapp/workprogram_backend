using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_UMR_Work_Program.Models
{
    public class GeneralModel
    {
        public class AppMessage
        {
            public object Subject { get; set; }
            public object Content { get; set; }
            public object RefNo { get; set; }

            public object Status { get; set; }
            public object Stage { get; set; }
            public object TotalAmount { get; set; }
            public object Seen { get; set; }
            public string GeneratedNo { get; set; }
            public object CompanyName { get; set; }
            public object CategoryName { get; set; }
            public object PhaseName { get; set; }
            public object FacilityName { get; set; }
            public object StatutoryLicenceFee { get; set; }
            public object ServiceCharge { get; set; }
            public object TotalAmountDue { get; set; }
            public object ApplicationPeriod { get; set; }
            public object DateSubmitted { get; set; }

        }

        public class WebApiResponse
        {
            public string ResponseCode { get; set; }
            public string UserStatus { get; set; }
            public string Message { get; set; }
            public int StatusCode { get; set; }
            public Object Data { get; set; }
        }

    }
}
