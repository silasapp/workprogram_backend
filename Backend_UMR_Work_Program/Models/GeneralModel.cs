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
        public static string Presented = "Presented";
        public static string FailedToShow = "Failed to show up";
        public static string NotInvited = "Not invited";
        public static string ShowedButNoPresentation = "Showed up but could not present";
        public static string ThreeD = "3D";
        public static string TwoD = "2D";
        public static string Exploration = "Exploration";
        public static string Development = "Development";
        public static string FirstAppraisal = "1st Appraisal";
        public static string SecondAppraisal = "Second Appraisal";
        public static string Appraisal = "Appraisal";
        public static string OrdinaryAppraisal = "Ordinary Appraisal";
        public static string JV = "JV";
        public static string PSC = "PSC";
        public static string MF = "MF";
        public static string SR = "SR";
        public static string ContinentalShelf = "Continental Shelf";
        public static string DeepOffshore = "Deep Offshore";
        public static string Onshore = "Onshore";
        public static string Fatality = "FATALITY";
        public static string Sabotage = "SABOTAGE";
        public static string HumanError = "HUMAN ERROR";
        public static string MysterySpills = "MYSTERY SPILLS";
        public static string EquipmentFailure = "EQUIPMENT_FAILURE";

        public class WorkProgramme_Model
        {
            public CONCESSION_SITUATION_Model CONCESSION_SITUATION { get; set; }
            public GEOPHYSICAL_ACTIVITIES_ACQUISITION_Model GEOPHYSICAL_ACTIVITIES_ACQUISITIONs { get; set; }
            public GEOPHYSICAL_ACTIVITIES_PROCESSING_Model GEOPHYSICAL_ACTIVITIES_PROCESSINGs { get; set; }
            public DRILLING_OPERATIONS_CATEGORIES_OF_WELL_Model DRILLING_OPERATIONS_CATEGORIES_OF_WELLs { get; set; }
            public DRILLING_EACH_WELL_COST_Model DRILLING_EACH_WELL_COSTs { get; set; }
            public DRILLING_EACH_WELL_COST_PROPOSED_Model DRILLING_EACH_WELL_COST_PROPOSEDs { get; set; }
            
        }
        public class CONCESSION_SITUATION_Model
        {
            public int Id { get; set; }
            public string? OML_ID { get; set; }
            public string? OML_Name { get; set; }
            public string? CompanyName { get; set; }
            public string? Companyemail { get; set; }
            public string? Year { get; set; }
            public string? Concession_Held { get; set; }
            public string? Area { get; set; }
            public string? No_of_discovered_field { get; set; }
            public string? No_of_field_producing { get; set; }
            public string? Name_of_Company { get; set; }
            public string? Equity_distribution { get; set; }
            public string? Contract_Type { get; set; }
            public string? Geological_location { get; set; }
            public string? Has_Signature_Bonus_been_paid { get; set; }
            public string? If_No_why_sig { get; set; }
            public string? Has_the_Concession_Rentals_been_paid { get; set; }
            public string? If_No_why_concession { get; set; }
            public string? Is_there_an_application_for_renewal { get; set; }
            public string? If_No_why_renewal { get; set; }
            public string? Budget_actual_for_license_or_lease { get; set; }
            public string? proposed_budget_for_each_license_lease { get; set; }
            public string? Five_year_proposal { get; set; }
            public string? Did_you_meet_the_minimum_work_programme { get; set; }
            public string? Comment { get; set; }
            public string? Created_by { get; set; }
            public string? Updated_by { get; set; }
            public DateTime? Date_Created { get; set; }
            public DateTime? Date_Updated { get; set; }
            public DateTime? Date_of_Grant_Expiration { get; set; }
            public string? Terrain { get; set; }
            public string? Consession_Type { get; set; }
            public DateTime? Date_of_Expiration { get; set; }
            public string? How_Much_Signature_Bonus_have_been_paid_USD { get; set; }
            public string? How_Much_Concession_Rental_have_been_paid_USD { get; set; }
            public string? How_Much_Renewal_Bonus_have_been_paid_USD { get; set; }
            public string? Has_Assignment_of_Interest_Fee_been_paid { get; set; }
            public string? relinquishment_retention { get; set; }
            public string? area_in_square_meter_based_on_company_records { get; set; }
            public string? COMPANY_ID { get; set; }
        }

        public class GEOPHYSICAL_ACTIVITIES_ACQUISITION_Model
        {
            public string? Geo_acquired_geophysical_data { get; set; }
            public string? Geo_area_of_coverage { get; set; }
            public string? Geo_method_of_acquisition { get; set; }
            public string? Geo_type_of_data_acquired { get; set; }
            public string? Geo_Record_Length_of_Data { get; set; }
            public string? Geo_Completion_Status { get; set; }
            public string? Quantum { get; set; }
            public string? Quantum_carry_forward { get; set; }
            public string? Geo_Activity_Timeline { get; set; }
            public string? Remarks { get; set; }
            public string? Actual_year_aquired_data { get; set; }
            public string? proposed_year_data { get; set; }
            public string? Budeget_Allocation { get; set; }
            public string? Actual_year { get; set; }
            public string? proposed_year { get; set; }
            public string? OML_ID { get; set; }
            public string? OML_Name { get; set; }
            public string? CompanyName { get; set; }
            public string? Companyemail { get; set; }
            public string? Year_of_WP { get; set; }
            public string? Budeget_Allocation_NGN { get; set; }
            public string? Budeget_Allocation_USD { get; set; }
            public string? Name_of_Contractor { get; set; }
            public string? Quantum_Approved { get; set; }
            public string? Contract_Type { get; set; }
            public string? Terrain { get; set; }
            public string? Consession_Type { get; set; }
            public string? Quantum_Planned { get; set; }
            public string? Gas_flare_Royalty_payment { get; set; }
            public string? Gas_Sales_Royalty_Payment { get; set; }
            public string? QUATER { get; set; }
            public string? COMPANY_ID { get; set; }
        }

        public class GEOPHYSICAL_ACTIVITIES_PROCESSING_Model
        {
            public string? Geo_Any_Ongoing_Processing_Project { get; set; }
            public string? Geo_Type_of_Data_being_Processed { get; set; }
            public string? Geo_Quantum_of_Data { get; set; }
            public string? Geo_Quantum_of_Data_carry_over { get; set; }
            public string? Geo_Completion_Status { get; set; }
            public string? Geo_Activity_Timeline { get; set; }
            public string? Remarks { get; set; }
            public string? Actual_year_aquired_data { get; set; }
            public string? proposed_year_data { get; set; }
            public string? Budeget_Allocation { get; set; }
            public string? Actual_year { get; set; }
            public string? proposed_year { get; set; }
            public string? OML_ID { get; set; }
            public string? OML_Name { get; set; }
            public string? CompanyName { get; set; }
            public string? Companyemail { get; set; }
            public string? Year_of_WP { get; set; }
            public string? Budeget_Allocation_USD { get; set; }
            public string? Budeget_Allocation_NGN { get; set; }
            public string? Processed_Actual { get; set; }
            public string? Processed_Proposed { get; set; }
            public string? Reprocessed_Actual { get; set; }
            public string? Reprocessed_Proposed { get; set; }
            public string? Interpreted_Actual { get; set; }
            public string? Interpreted_Proposed { get; set; }
            public string? Name_of_Contractor { get; set; }
            public string? Quantum_Approved { get; set; }
            public string? Contract_Type { get; set; }
            public string? Terrain { get; set; }
            public string? Quantum_Planned { get; set; }
            public string? Consession_Type { get; set; }
            public string? QUATER { get; set; }
            public string? COMPANY_ID { get; set; }
        }

        public class DRILLING_OPERATIONS_CATEGORIES_OF_WELL_Model
        {
            public string? OML_ID { get; set; }
            public string? OML_Name { get; set; }
            public string? CompanyName { get; set; }
            public string? Companyemail { get; set; }
            public string? Year_of_WP { get; set; }
            public string? Category { get; set; }
            public string? Actual_No_Drilled_in_Current_Year { get; set; }
            public string? Proposed_No_Drilled { get; set; }
            public string? Processing_Fees_Paid { get; set; }
            public string? Comments { get; set; }
            public string? No_of_wells_cored { get; set; }
            public string? Actual_year { get; set; }
            public string? proposed_year { get; set; }
            public string? Created_by { get; set; }
            public string? Updated_by { get; set; }
            public DateTime? Date_Created { get; set; }
            public DateTime? Date_Updated { get; set; }
            public string? well_type { get; set; }
            public string? well_trajectory { get; set; }
            public DateTime? spud_date { get; set; }
            public string? well_cost { get; set; }
            public string? Number_of_Days_to_Total_Depth { get; set; }
            public string? Well_Status_and_Depth { get; set; }
            public string? Contract_Type { get; set; }
            public string? Terrain { get; set; }
            public string? well_name { get; set; }
            public string? Consession_Type { get; set; }
            public string? QUATER { get; set; }
            public string? Any_New_Discoveries { get; set; }
            public string? Hydrocarbon_Counts { get; set; }
            public string? State_the_field_where_Discovery_was_made { get; set; }
            public string? Core_Cost_USD { get; set; }
            public string? Core_Depth_Interval { get; set; }
            public string? Propose_well_names { get; set; }
            public string? Actual_wells_name { get; set; }
            public string? Terrain_Drill { get; set; }
            public string? Water_depth { get; set; }
            public string? True_vertical_depth { get; set; }
            public string? Depth_refrence { get; set; }
            public string? Rig_type { get; set; }
            public string? Rig_Name { get; set; }
            public string? Target_reservoir { get; set; }
            public string? Surface_cordinates_for_each_well_in_degrees { get; set; }
            public string? Location_name { get; set; }
            public string? Proposed_cost_per_well { get; set; }
            public string? Basin { get; set; }
            public string? Measured_depth { get; set; }
            public string? FieldDiscoveryUploadFilePath { get; set; }
            public string? HydrocarbonCountUploadFilePath { get; set; }
            public string? COMPANY_ID { get; set; }
            public string? Cored { get; set; }
            public string? Actual_Proposed { get; set; }
            public string? WellName { get; set; }
        }

        public class DRILLING_EACH_WELL_COST_Model
        {
            public string? OML_ID { get; set; }
            public string? OML_Name { get; set; }
            public string? CompanyName { get; set; }
            public string? Companyemail { get; set; }
            public string? Year_of_WP { get; set; }
            public string? well_name { get; set; }
            public string? well_cost { get; set; }
            public string? Consession_Type { get; set; }
            public string? QUATER { get; set; }
            public string? Surface_cordinates_for_each_well_in_degrees { get; set; }
            public string? COMPANY_ID { get; set; }
        }

        public class DRILLING_EACH_WELL_COST_PROPOSED_Model
        {
            public string? OML_ID { get; set; }
            public string? OML_Name { get; set; }
            public string? CompanyName { get; set; }
            public string? Companyemail { get; set; }
            public string? Year_of_WP { get; set; }
            public string? well_name { get; set; }
            public string? well_cost { get; set; }
            public string? Surface_cordinates_for_each_well_in_degrees { get; set; }
            public string? Consession_Type { get; set; }
            public string? QUATER { get; set; }
            public string? COMPANY_ID { get; set; }
        }

        public class UserMasterModel
        {
            public int UserId { get; set; }
            public string? UserEmail { get; set; }
            public string? UserType { get; set; }
            public string? FirstName { get; set; }
            public string? LastName { get; set; }
            public string? UserRole { get; set; }
            public string? Status { get; set; }
            public int? LoginCount { get; set; }
            public string? PasswordHash { get; set; }
            public string? PhoneNumber { get; set; }
            public string? CompanyName { get; set; }
            public int? DivisionId { get; set; }
            public string? Comment { get; set; }

        }

        public class PresentationSchedules_Model
        {
            public List<ADMIN_DATETIME_PRESENTATION> presentationSchedules { get; set; }
            public List<ADMIN_DIVISIONAL_REPS_PRESENTATION> Divisionpresentations { get; set; }
            public List<string> Years { get; set; }
        }
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
