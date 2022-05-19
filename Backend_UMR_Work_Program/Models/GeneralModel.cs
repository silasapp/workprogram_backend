using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_UMR_Work_Program.Models
{
    public class GeneralModel
    {
        public static string Company = "Company";
        public static string Admin = "Admin";
        public static string SuperAdmin = "SuperAdmin";

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

        public class AppResponseCodes
        {
            public const string Success = "00";
            public const string InternalError = "02";
            public const string Failed = "03";
            public const string DuplicateEmail = "04";
            public const string RecordNotFound = "05";
            public const string InvalidLogin = "06";
            public const string UserAlreadyExist = "07";
            public const string DuplicateUserDetails = "08";
            public const string InvalidAccountNo = "09";
            public const string UserNotFound = "10";
            public const string TransactionFailed = "11";
            public const string TransactionProcessed = "12";
            public const string AccountIsLocked = "13";
            public const string DuplicatePassword = "14";
            public const string OtpExpired = "15";
            public const string ExtraPaymentAlreadyExist = "16";


        }
        public class ResponseCodes
        {
            public const int Success = 200;
            public const int Failure = 300;
            public const int Badrequest = 400;
            public const int RecordNotFound = 404;
            public const int Duplicate = 409;
            public const int InternalError = 500;
        }
    }
}
