using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace Backend_UMR_Work_Program.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class WorkProgrammeController : ControllerBase
    {
        private Account _account;
        public WKP_DBContext _context;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public WorkProgrammeController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor, _mapper);
        }

        //private string? WKPCompanyId => "1";
        //private string? WKPCompanyName => "Name";
        //private string? WKPCompanyEmail => "adeola.kween123@gmail.com";
        //private string? WKUserRole => "Admin";

        private string? WKPCompanyId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string? WKPCompanyName => User.FindFirstValue(ClaimTypes.Name);
        private string? WKPCompanyEmail => User.FindFirstValue(ClaimTypes.Email);
        private string? WKUserRole => User.FindFirstValue(ClaimTypes.Role);



        #region Form 1
        [HttpPost("POST_WORKPROGRAMME_1")]
        public async Task<WebApiResponse> Post_WORKPROGRAMME_1(WorkProgramme_Model_1 wkp, IFormFile FieldDiscoveryUploadFile, IFormFile HydrocarbonCountUploadFile)
        {

            int save = 0; int numberofTablesDataToSave = 0;

            try
            {
                #region save form data
                //Saving Concession Situations
                if (wkp.CONCESSION_SITUATION != null)
                {
                    numberofTablesDataToSave++;
                    Task<WebApiResponse> ConcessionData = POST_CONCESSION_SITUATION(wkp.CONCESSION_SITUATION, wkp.WorkProgramme_Year, "OML 21");
                    if (ConcessionData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Geophysical Activites
                    Task<WebApiResponse> GeophysicalActivitesData = GEOPHYSICAL_ACTIVITIES_ACQUISITION(wkp.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs, wkp.WorkProgramme_Year);
                    if (GeophysicalActivitesData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.GEOPHYSICAL_ACTIVITIES_PROCESSINGs != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Geophysical Activites Processing
                    Task<WebApiResponse> GeoActivitesProcessingData = GEOPHYSICAL_ACTIVITIES_PROCESSING(wkp.GEOPHYSICAL_ACTIVITIES_PROCESSINGs, wkp.WorkProgramme_Year);
                    if (GeoActivitesProcessingData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Drilling Operations
                    List<IFormFile> DriilingFiles = new List<IFormFile>();
                    DriilingFiles.Add(FieldDiscoveryUploadFile);
                    DriilingFiles.Add(HydrocarbonCountUploadFile);

                    Task<WebApiResponse> DrillingOperationsData = DRILLING_OPERATIONS_CATEGORIES_OF_WELL(wkp.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs, wkp.WorkProgramme_Year, DriilingFiles);
                    if (DrillingOperationsData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.DRILLING_EACH_WELL_COSTs != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Drilling Well Cost
                    Task<WebApiResponse> DrillingWellCostData = DRILLING_EACH_WELL_COST(wkp.DRILLING_EACH_WELL_COSTs, wkp.WorkProgramme_Year);
                    if (DrillingWellCostData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.DRILLING_EACH_WELL_COST_PROPOSEDs != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Geophysical Activites
                    Task<WebApiResponse> DrillingWellCostProposedData = DRILLING_EACH_WELL_COST_PROPOSED(wkp.DRILLING_EACH_WELL_COST_PROPOSEDs, wkp.WorkProgramme_Year);
                    if (DrillingWellCostProposedData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                #endregion

                if (save == numberofTablesDataToSave)
                {
                    string successMsg = "Work programme form 1 has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        #endregion

        #region Form 2
        [HttpPost("POST_WORKPROGRAMME_2")]
        public async Task<WebApiResponse> Post_WORKPROGRAMME_2(WorkProgramme_Model_2 wkp, IFormFile Uploaded_approved_FDP_Document, IFormFile PUAUploadFile_Document,
            IFormFile UUOAUploadFilePath_Document, IFormFile Upload_NDR_payment_receipt, IFormFile ProductionOilCondensateAGNAGUFile)
        {

            int save = 0; int numberofTablesDataToSave = 0;
            try
            {
                #region save form data
                if (wkp.Initial_Well_Completion_Job != null)
                {
                    numberofTablesDataToSave++;
                    //Saving Well Completion Job
                    Task<WebApiResponse> Well_Completion_JobData = INITIAL_WELL_COMPLETION_JOB1(wkp.Initial_Well_Completion_Job, wkp.WorkProgramme_Year);
                    if (Well_Completion_JobData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.WORKOVERS_RECOMPLETION_JOB1 != null)
                {
                    numberofTablesDataToSave++;
                    //Saving Well Completion Job
                    Task<WebApiResponse> Workover_JobData = WORKOVERS_RECOMPLETION_JOB1(wkp.WORKOVERS_RECOMPLETION_JOB1, wkp.WorkProgramme_Year);
                    if (Workover_JobData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE != null)
                {
                    numberofTablesDataToSave++;
                    // Saving FDP_Excess Reserve
                    Task<WebApiResponse> FDP_ExcessReserveData = FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE(wkp.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE, wkp.WorkProgramme_Year);
                    if (FDP_ExcessReserveData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP != null)
                {
                    numberofTablesDataToSave++;
                    // Saving FDP To Submit
                    Task<WebApiResponse> FDP_ToSubmitData = FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP(wkp.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP, wkp.WorkProgramme_Year);
                    if (FDP_ToSubmitData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.FIELD_DEVELOPMENT_FIELDS_AND_STATUS != null)
                {
                    numberofTablesDataToSave++;
                    // Saving FDP Status
                    Task<WebApiResponse> FDP_StatusData = FIELD_DEVELOPMENT_FIELDS_AND_STATUS(wkp.FIELD_DEVELOPMENT_FIELDS_AND_STATUS, wkp.WorkProgramme_Year);
                    if (FDP_StatusData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.RESERVES_UPDATES_LIFE_INDEX != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Reserve Update
                    Task<WebApiResponse> Reserve_UpdateData = RESERVES_UPDATES_LIFE_INDEX(wkp.RESERVES_UPDATES_LIFE_INDEX, wkp.WorkProgramme_Year);
                    if (Reserve_UpdateData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.FIELD_DEVELOPMENT_PLAN != null)
                {
                    numberofTablesDataToSave++;
                    // Saving FDP Data
                    List<IFormFile> FDPFiles = new List<IFormFile>();
                    FDPFiles.Add(Uploaded_approved_FDP_Document);

                    Task<WebApiResponse> FDP_Data = FIELD_DEVELOPMENT_PLAN(wkp.FIELD_DEVELOPMENT_PLAN, wkp.WorkProgramme_Year, FDPFiles);
                    if (FDP_Data.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITy != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Oil Production
                    Task<WebApiResponse> Oil_ProductionData = OIL_CONDENSATE_PRODUCTION_ACTIVITy(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITy, wkp.WorkProgramme_Year);
                    if (Oil_ProductionData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Oil Unitization
                    List<IFormFile> OilFiles = new List<IFormFile>();
                    OilFiles.Add(PUAUploadFile_Document);
                    OilFiles.Add(UUOAUploadFilePath_Document);

                    Task<WebApiResponse> OilUnitizationData = OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION, wkp.WorkProgramme_Year, OilFiles);
                    if (OilUnitizationData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.GAS_PRODUCTION_ACTIVITy != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Gas Production
                    List<IFormFile> GasFiles = new List<IFormFile>();
                    GasFiles.Add(Upload_NDR_payment_receipt);

                    Task<WebApiResponse> Gas_ProductionData = GAS_PRODUCTION_ACTIVITy(wkp.GAS_PRODUCTION_ACTIVITy, wkp.WorkProgramme_Year, GasFiles);
                    if (Gas_ProductionData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.NDR != null)
                {
                    numberofTablesDataToSave++;
                    // Saving NDR
                    List<IFormFile> NDRFiles = new List<IFormFile>();
                    NDRFiles.Add(Upload_NDR_payment_receipt);

                    Task<WebApiResponse> NDRData = NDR(wkp.NDR, wkp.WorkProgramme_Year, NDRFiles);
                    if (NDRData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Oil Condensate Reserve Status
                    Task<WebApiResponse> Oil_Condensate_ReserveStatusData = RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE(wkp.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE, wkp.WorkProgramme_Year);
                    if (Oil_Condensate_ReserveStatusData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Oil Condensate Reserve Update
                    Task<WebApiResponse> Oil_Condensate_ReserveProjectionData = RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection(wkp.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection, wkp.WorkProgramme_Year);
                    if (Oil_Condensate_ReserveProjectionData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Oil Condensate Production Activities
                    List<IFormFile> OilCondFiles = new List<IFormFile>();
                    OilCondFiles.Add(ProductionOilCondensateAGNAGUFile);
                    OilCondFiles.Add(ProductionOilCondensateAGNAGUFile);

                    Task<WebApiResponse> Oil_Condensate_ProjectionData = OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION, wkp.WorkProgramme_Year, OilCondFiles);
                    if (Oil_Condensate_ProjectionData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Oil Condensate Annual Data
                    Task<WebApiResponse> Oil_Condensate_AnnualData = RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION(wkp.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION, wkp.WorkProgramme_Year);
                    if (Oil_Condensate_AnnualData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Oil Condensate Decline Data
                    Task<WebApiResponse> Oil_Condensate_DeclineData = RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE(wkp.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE, wkp.WorkProgramme_Year);
                    if (Oil_Condensate_DeclineData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Oil Condensate Production Data
                    Task<WebApiResponse> Oil_Condensate_ProductionData = OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity, wkp.WorkProgramme_Year);
                    if (Oil_Condensate_ProductionData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.RESERVES_REPLACEMENT_RATIO != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Reserve Replacement Data
                    Task<WebApiResponse> Reserve_ReplacementData = RESERVES_REPLACEMENT_RATIO(wkp.RESERVES_REPLACEMENT_RATIO, wkp.WorkProgramme_Year);
                    if (Reserve_ReplacementData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Oil Monthly Data
                    Task<WebApiResponse> Oil_MonthlyData = OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED, wkp.WorkProgramme_Year);
                    if (Oil_MonthlyData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                if (wkp.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY != null)
                {
                    numberofTablesDataToSave++;
                    // Saving Oil Gas Activities Data
                    Task<WebApiResponse> Gas_ActivitiesData = GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY(wkp.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY, wkp.WorkProgramme_Year);
                    if (Gas_ActivitiesData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                #endregion
                if (save == numberofTablesDataToSave)
                {
                    string successMsg = "Work programme form 2 has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        #endregion


        #region Form 3

        [HttpPost("POST_WORKPROGRAMME_3")]
        public async Task<WebApiResponse> Post_WORKPROGRAMME_3(WorkProgramme_Model3 wkp)
        {

            int save = 0;
            try
            {
                #region Saving form data
                //Saving Budget Actual Expenditure
                Task<WebApiResponse> BudgetActualExData = BUDGET_ACTUAL_EXPENDITURE(wkp.BUDGET_ACTUAL_EXPENDITURE, wkp.WorkProgramme_Year);
                if (BudgetActualExData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Budget Proposal
                Task<WebApiResponse> BudgetProposalData = BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT(wkp.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT, wkp.WorkProgramme_Year);
                if (BudgetProposalData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Budget Performance Exploratory
                Task<WebApiResponse> BudgetPerformanceExpData = BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy(wkp.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy, wkp.WorkProgramme_Year);
                if (BudgetPerformanceExpData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Budget Performance Development 
                Task<WebApiResponse> BudgetPerformanceDevData = BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy(wkp.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy, wkp.WorkProgramme_Year);
                if (BudgetPerformanceDevData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Budget Performance Production
                Task<WebApiResponse> BudgetPerformanceProdData = BUDGET_PERFORMANCE_PRODUCTION_COST(wkp.BUDGET_PERFORMANCE_PRODUCTION_COST, wkp.WorkProgramme_Year);
                if (BudgetPerformanceProdData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Budget Performance Facility
                Task<WebApiResponse> BudgetPerformanceFacData = BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT(wkp.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT, wkp.WorkProgramme_Year);
                if (BudgetPerformanceFacData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Oil Gas Facility
                Task<WebApiResponse> OilGasFacData = OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE(wkp.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE, wkp.WorkProgramme_Year);
                if (OilGasFacData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Oil Gas Production
                Task<WebApiResponse> OilGasProdData = OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment, wkp.WorkProgramme_Year);
                if (OilGasProdData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Oil Gas Facilty Maintenance
                Task<WebApiResponse> OilGasFacProjectData = OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT(wkp.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT, wkp.WorkProgramme_Year);
                if (OilGasFacProjectData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Oil Gas Facilty Project
                Task<WebApiResponse> FacProjectData = FACILITIES_PROJECT_PERFORMANCE(wkp.FACILITIES_PROJECT_PERFORMANCE, wkp.WorkProgramme_Year);
                if (FacProjectData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }//Saving acilty Project
                Task<WebApiResponse> BudgetCapexOpexData = BUDGET_CAPEX_OPEX(wkp.BUDGET_CAPEX_OPEX, wkp.WorkProgramme_Year);
                if (BudgetCapexOpexData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                #endregion

                if (save == 11)
                {
                    string successMsg = "Work programme form 3 has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        #endregion


        #region Form 4

        [HttpPost("POST_WORKPROGRAMME_4")]
        public async Task<WebApiResponse> Post_WORKPROGRAMME_4(WorkProgramme_Model4 wkp)
        {

            int save = 0;
            try
            {
                #region Saving form data
                //Saving Nigeria Training
                Task<WebApiResponse> NigeriaTrainingData = NIGERIA_CONTENT_Training(wkp.NIGERIA_CONTENT_Training, wkp.WorkProgramme_Year);
                if (NigeriaTrainingData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Nigeria Content Upload
                Task<WebApiResponse> NigeriaUploadData = NIGERIA_CONTENT_Upload_Succession_Plan(wkp.NIGERIA_CONTENT_Upload_Succession_Plan, wkp.WorkProgramme_Year);
                if (NigeriaUploadData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Nigeria Content Upload
                Task<WebApiResponse> NigeriaQuestionData = NIGERIA_CONTENT_QUESTION(wkp.NIGERIA_CONTENT_QUESTION, wkp.WorkProgramme_Year);
                if (NigeriaQuestionData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Nigeria Content Upload
                Task<WebApiResponse> LegalLitigationData = LEGAL_LITIGATION(wkp.LEGAL_LITIGATION, wkp.WorkProgramme_Year);
                if (LegalLitigationData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Nigeria Content Upload
                Task<WebApiResponse> LegalArbitrationData = LEGAL_ARBITRATION(wkp.LEGAL_ARBITRATION, wkp.WorkProgramme_Year);
                if (LegalArbitrationData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Strategic Plans
                Task<WebApiResponse> StrategicPlansData = STRATEGIC_PLANS_ON_COMPANY_BASI(wkp.STRATEGIC_PLANS_ON_COMPANY_BASI, wkp.WorkProgramme_Year);
                if (StrategicPlansData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                #endregion
                if (save == 6)
                {
                    string successMsg = "Work programme form 4 has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        #endregion

        #region Form 5

        [HttpPost("POST_WORKPROGRAMME_5")]
        public async Task<WebApiResponse> Post_WORKPROGRAMME_5(WorkProgramme_Model5 wkp, IFormFile SMSFileUploadPath, IFormFile MOUUploadFile,
            IFormFile SSUploadFile, IFormFile OrganogramFile, IFormFile PromotionLetterFile, IFormFile QualityControlFile,
            IFormFile GHGFile, IFormFile SafetyCurrentYearFile, IFormFile SafetyLast2YearsFile, IFormFile OHMplanFile, IFormFile OHMplanCommunicationFile,
            IFormFile EMSFile, IFormFile AUDITFile, IFormFile UploadedPresentation, IFormFile WasteManagementPlanFile, IFormFile DecomCertificateFile)
        {
            int save = 0;
            int numberofTablesDataToSave = 0;
            try
            {

                #region save form data
                //Saving HSE Question
                if (wkp.HSE_QUESTION != null)
                {

                    numberofTablesDataToSave++;

                    Task<WebApiResponse> HseQuestionData = HSE_QUESTION(wkp.HSE_QUESTION, wkp.WorkProgramme_Year);
                    if (HseQuestionData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                //Saving HSE Fatality
                if (wkp.HSE_FATALITy != null)
                {
                    numberofTablesDataToSave++;

                    Task<WebApiResponse> HseFatalityData = HSE_FATALITy(wkp.HSE_FATALITy, wkp.WorkProgramme_Year);
                    if (HseFatalityData.Result.ToString() == AppResponseCodes.Success)
                    {
                        save++;
                    }
                }
                //Saving HSE Design Safety
                Task<WebApiResponse> HseDesignData = HSE_DESIGNS_SAFETY(wkp.HSE_DESIGNS_SAFETY, wkp.WorkProgramme_Year);
                if (HseDesignData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Safety 
                List<IFormFile> HSEFiles = new List<IFormFile>();
                HSEFiles.Add(SMSFileUploadPath);

                Task<WebApiResponse> HseSafetyData = HSE_SAFETY_STUDIES_NEW(wkp.HSE_SAFETY_STUDIES_NEW, wkp.WorkProgramme_Year, HSEFiles);
                if (HseSafetyData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Inspection Maintenance
                Task<WebApiResponse> HseInspectionMaintenanceData = HSE_INSPECTION_AND_MAINTENANCE_NEW(wkp.HSE_INSPECTION_AND_MAINTENANCE_NEW, wkp.WorkProgramme_Year);
                if (HseInspectionMaintenanceData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Inspection Maintenance and Facility
                Task<WebApiResponse> HseInspectionMaintenanceFacData = HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW(wkp.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW, wkp.WorkProgramme_Year);
                if (HseInspectionMaintenanceFacData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Technical Safety
                Task<WebApiResponse> HseTechnicalData = HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW(wkp.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW, wkp.WorkProgramme_Year);
                if (HseTechnicalData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Asset Register
                Task<WebApiResponse> HseAssetData = HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW(wkp.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW, wkp.WorkProgramme_Year);
                if (HseAssetData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Oil Spill
                Task<WebApiResponse> HseOilData = HSE_OIL_SPILL_REPORTING_NEW(wkp.HSE_OIL_SPILL_REPORTING_NEW, wkp.WorkProgramme_Year);
                if (HseOilData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Asset Register
                Task<WebApiResponse> HseAssetRegisterData = HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW(wkp.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW, wkp.WorkProgramme_Year);
                if (HseAssetRegisterData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Accident
                Task<WebApiResponse> HseAccidentData = HSE_ACCIDENT_INCIDENCE_REPORTING_NEW(wkp.HSE_ACCIDENT_INCIDENCE_REPORTING_NEW, wkp.WorkProgramme_Year);
                if (HseAccidentData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Accident Incidence
                Task<WebApiResponse> HseAccidentIncidenceData = HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW(wkp.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW, wkp.WorkProgramme_Year);
                if (HseAccidentIncidenceData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Community Disturbances
                Task<WebApiResponse> HseCommunityDisturbancesData = HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW(wkp.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW, wkp.WorkProgramme_Year);
                if (HseCommunityDisturbancesData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Environmental Studies
                Task<WebApiResponse> HseEnvironmentalData = HSE_ENVIRONMENTAL_STUDIES_NEW(wkp.HSE_ENVIRONMENTAL_STUDIES_NEW, wkp.WorkProgramme_Year);
                if (HseEnvironmentalData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Waste Management
                Task<WebApiResponse> HseWasteData = HSE_WASTE_MANAGEMENT_NEW(wkp.HSE_WASTE_MANAGEMENT_NEW, wkp.WorkProgramme_Year);
                if (HseWasteData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Waste Management Facility
                Task<WebApiResponse> HseWasteMgtData = HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW(wkp.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW, wkp.WorkProgramme_Year);
                if (HseWasteMgtData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Produced Water
                Task<WebApiResponse> HseWaterMgtData = HSE_PRODUCED_WATER_MANAGEMENT_NEW(wkp.HSE_PRODUCED_WATER_MANAGEMENT_NEW, wkp.WorkProgramme_Year);
                if (HseWaterMgtData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Environmental Compliance
                Task<WebApiResponse> HseEnvironmentalComplianceData = HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW(wkp.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW, wkp.WorkProgramme_Year);
                if (HseEnvironmentalComplianceData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Environmental Studies
                Task<WebApiResponse> HseEnvironmentalStudiesData = HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW(wkp.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW, wkp.WorkProgramme_Year);
                if (HseEnvironmentalStudiesData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Sustainable Development
                Task<WebApiResponse> HseSustainableData = HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL(wkp.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL, wkp.WorkProgramme_Year);
                if (HseSustainableData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Environmental Studies
                Task<WebApiResponse> HseEnvironmentalStudiesUpdatedData = HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED(wkp.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED, wkp.WorkProgramme_Year);
                if (HseEnvironmentalStudiesUpdatedData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE OSP Registrations
                Task<WebApiResponse> HseOSPData = HSE_OSP_REGISTRATIONS_NEW(wkp.HSE_OSP_REGISTRATIONS_NEW, wkp.WorkProgramme_Year);
                if (HseOSPData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE OSP Registrations
                Task<WebApiResponse> HseProducedWaterData = HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED(wkp.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED, wkp.WorkProgramme_Year);
                if (HseProducedWaterData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Environmental Monitoring Chemical
                Task<WebApiResponse> HseEnvironmentalMonitoringData = HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW(wkp.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW, wkp.WorkProgramme_Year);
                if (HseEnvironmentalMonitoringData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Environmental Monitoring Chemical
                Task<WebApiResponse> HseCausesData = HSE_CAUSES_OF_SPILL(wkp.HSE_CAUSES_OF_SPILL, wkp.WorkProgramme_Year);
                if (HseCausesData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Sustainable Development 
                List<IFormFile> HSESustainableDevelopmentFiles = new List<IFormFile>();
                HSESustainableDevelopmentFiles.Add(SMSFileUploadPath);

                Task<WebApiResponse> HseSustainableDevData = HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU(wkp.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU, wkp.WorkProgramme_Year, HSESustainableDevelopmentFiles);
                if (HseSustainableDevData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Sustainable Development Schemes
                List<IFormFile> HSESustainableCommunityFiles = new List<IFormFile>();
                HSESustainableCommunityFiles.Add(SSUploadFile);

                Task<WebApiResponse> HseSustainableComData = HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME(wkp.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME, wkp.WorkProgramme_Year, HSESustainableDevelopmentFiles);
                if (HseSustainableComData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Management Position
                List<IFormFile> HSEMgtPositionFiles = new List<IFormFile>();
                HSEMgtPositionFiles.Add(PromotionLetterFile);
                HSEMgtPositionFiles.Add(OrganogramFile);

                Task<WebApiResponse> HseManagmentData = HSE_MANAGEMENT_POSITION(wkp.HSE_MANAGEMENT_POSITION, wkp.WorkProgramme_Year, HSEMgtPositionFiles);
                if (HseManagmentData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Quality Control
                List<IFormFile> HSEQualityFiles = new List<IFormFile>();
                HSEQualityFiles.Add(QualityControlFile);

                Task<WebApiResponse> HseQualityData = HSE_QUALITY_CONTROL(wkp.HSE_QUALITY_CONTROL, wkp.WorkProgramme_Year, HSEQualityFiles);
                if (HseQualityData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Climate Change
                List<IFormFile> HSEClimateFiles = new List<IFormFile>();
                HSEClimateFiles.Add(GHGFile);

                Task<WebApiResponse> HseClimateData = HSE_CLIMATE_CHANGE_AND_AIR_QUALITY(wkp.HSE_CLIMATE_CHANGE_AND_AIR_QUALITY, wkp.WorkProgramme_Year, HSEClimateFiles);
                if (HseClimateData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Safety Culture
                List<IFormFile> HSESafetyFiles = new List<IFormFile>();
                HSESafetyFiles.Add(SafetyCurrentYearFile);
                HSESafetyFiles.Add(SafetyLast2YearsFile);

                Task<WebApiResponse> HseSafetyCultureData = HSE_SAFETY_CULTURE_TRAINING(wkp.HSE_SAFETY_CULTURE_TRAINING, wkp.WorkProgramme_Year, HSESafetyFiles);
                if (HseSafetyCultureData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Occupational
                List<IFormFile> HSEOccupationalFiles = new List<IFormFile>();
                HSEOccupationalFiles.Add(SafetyCurrentYearFile);
                HSEOccupationalFiles.Add(SafetyLast2YearsFile);

                Task<WebApiResponse> HseOccupationalData = HSE_OCCUPATIONAL_HEALTH_MANAGEMENT(wkp.HSE_OCCUPATIONAL_HEALTH_MANAGEMENT, wkp.WorkProgramme_Year, HSEOccupationalFiles);
                if (HseOccupationalData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Waste Management
                List<IFormFile> HSEWasteMgtFiles = new List<IFormFile>();
                HSEWasteMgtFiles.Add(DecomCertificateFile);
                HSEWasteMgtFiles.Add(WasteManagementPlanFile);

                Task<WebApiResponse> HseWasteMgtSysData = HSE_WASTE_MANAGEMENT_SYSTEM(wkp.HSE_WASTE_MANAGEMENT_SYSTEM, wkp.WorkProgramme_Year, HSEWasteMgtFiles);
                if (HseWasteMgtSysData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving HSE Waste Management
                List<IFormFile> HseEnvWasteFiles = new List<IFormFile>();
                HseEnvWasteFiles.Add(EMSFile);
                HseEnvWasteFiles.Add(AUDITFile);

                Task<WebApiResponse> HseEnvWasteData = HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM(wkp.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM, wkp.WorkProgramme_Year, HseEnvWasteFiles);
                if (HseEnvWasteData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Picture Upload
                List<IFormFile> PictureUploadFiles = new List<IFormFile>();
                PictureUploadFiles.Add(UploadedPresentation);

                Task<WebApiResponse> PictureUploadData = PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT(wkp.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT, wkp.WorkProgramme_Year, PictureUploadFiles);
                if (PictureUploadData.Result.ToString() == AppResponseCodes.Success)
                {
                    save++;
                }
                #endregion


                if (save == numberofTablesDataToSave)
                {
                    string successMsg = "Work programme form 5 has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }

        }
        #endregion


        #region database tables actions

        [HttpPost("POST_CONCESSION_SITUATION")]
        public async Task<WebApiResponse> POST_CONCESSION_SITUATION([FromBody] CONCESSION_SITUATION_Model concession_situation_model, string year, string omlName, string actionToDo = null)
        {

            int save = 0;
            var ConcessionCONCESSION_SITUATION_Model = new CONCESSION_SITUATION();
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo;

            try
            {
                #region Saving Concession Situations


                var concessionDbData = (from c in _context.CONCESSION_SITUATIONs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.Year == year select c).FirstOrDefault();
                ConcessionCONCESSION_SITUATION_Model = _mapper.Map<CONCESSION_SITUATION>(concession_situation_model);

                ConcessionCONCESSION_SITUATION_Model.Companyemail = WKPCompanyEmail;
                ConcessionCONCESSION_SITUATION_Model.CompanyName = WKPCompanyName;
                ConcessionCONCESSION_SITUATION_Model.COMPANY_ID = WKPCompanyId;
                ConcessionCONCESSION_SITUATION_Model.Date_Updated = DateTime.Now;
                ConcessionCONCESSION_SITUATION_Model.Updated_by = WKPCompanyId;
                ConcessionCONCESSION_SITUATION_Model.Year = year;
                if (action == GeneralModel.Insert)
                {
                    if (concessionDbData == null)
                    {
                        ConcessionCONCESSION_SITUATION_Model.Created_by = WKPCompanyId;
                        ConcessionCONCESSION_SITUATION_Model.Date_Created = DateTime.Now;
                        await _context.CONCESSION_SITUATIONs.AddAsync(ConcessionCONCESSION_SITUATION_Model);
                    }
                    else
                    {
                        _context.CONCESSION_SITUATIONs.Remove(concessionDbData);
                        ConcessionCONCESSION_SITUATION_Model.Created_by = WKPCompanyId;
                        ConcessionCONCESSION_SITUATION_Model.Date_Created = DateTime.Now;
                        await _context.CONCESSION_SITUATIONs.AddAsync(ConcessionCONCESSION_SITUATION_Model);
                    }
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.CONCESSION_SITUATIONs.Remove(concessionDbData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No CONCESSION_SITUATION_Model was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
       
        [HttpPost("GEOPHYSICAL_ACTIVITIES_ACQUISITION")]
        public async Task<WebApiResponse> GEOPHYSICAL_ACTIVITIES_ACQUISITION(List<GEOPHYSICAL_ACTIVITIES_ACQUISITION_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var GeophysicalActivitesData = new GEOPHYSICAL_ACTIVITIES_ACQUISITION();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Geophysical Activites
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                        var getGeophysicalActivitesData = (from c in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        GeophysicalActivitesData = getGeophysicalActivitesData != null ? getGeophysicalActivitesData : GeophysicalActivitesData;
                        GeophysicalActivitesData = _mapper.Map<GEOPHYSICAL_ACTIVITIES_ACQUISITION>(wkp);

                        GeophysicalActivitesData.Companyemail = WKPCompanyEmail;
                        GeophysicalActivitesData.CompanyName = WKPCompanyName;
                        GeophysicalActivitesData.COMPANY_ID = WKPCompanyId;
                        GeophysicalActivitesData.Date_Updated = DateTime.Now;
                        GeophysicalActivitesData.Updated_by = WKPCompanyId;
                        GeophysicalActivitesData.Year_of_WP = WorkProgramme_Year;
                        if (getGeophysicalActivitesData == null)
                        {
                            GeophysicalActivitesData.Date_Created = DateTime.Now;
                            GeophysicalActivitesData.Created_by = WKPCompanyId;
                            await _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.AddAsync(GeophysicalActivitesData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Remove(GeophysicalActivitesData);
                        }

                        save += await _context.SaveChangesAsync();

                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
       
        [HttpPost("GEOPHYSICAL_ACTIVITIES_PROCESSING")]
        public async Task<WebApiResponse> GEOPHYSICAL_ACTIVITIES_PROCESSING(List<GEOPHYSICAL_ACTIVITIES_PROCESSING_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var GeoActivitesProcessingData = new GEOPHYSICAL_ACTIVITIES_PROCESSING();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Geophysical Activites Processing
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                       
                        var getGeoActivitesProcessingData = (from c in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        GeoActivitesProcessingData = getGeoActivitesProcessingData != null ? getGeoActivitesProcessingData : GeoActivitesProcessingData;
                        GeoActivitesProcessingData = _mapper.Map<GEOPHYSICAL_ACTIVITIES_PROCESSING>(wkp);

                        GeoActivitesProcessingData.Companyemail = WKPCompanyEmail;
                        GeoActivitesProcessingData.CompanyName = WKPCompanyName;
                        GeoActivitesProcessingData.COMPANY_ID = WKPCompanyId;
                        GeoActivitesProcessingData.Date_Updated = DateTime.Now;
                        GeoActivitesProcessingData.Updated_by = WKPCompanyId;
                        GeoActivitesProcessingData.Year_of_WP = WorkProgramme_Year;
                        if (getGeoActivitesProcessingData == null)
                        {
                            GeoActivitesProcessingData.Created_by = WKPCompanyId;
                            GeoActivitesProcessingData.Date_Created = DateTime.Now;
                            await _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.AddAsync(GeoActivitesProcessingData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Remove(GeoActivitesProcessingData);
                        }

                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("DRILLING_OPERATIONS_CATEGORIES_OF_WELL")]
        public async Task<WebApiResponse> DRILLING_OPERATIONS_CATEGORIES_OF_WELL(List<DRILLING_OPERATIONS_CATEGORIES_OF_WELL_Model> data, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var DrillingOperationsData = new DRILLING_OPERATIONS_CATEGORIES_OF_WELL();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Drilling Operations
                if (data.Count() > 0)
                {
                  foreach (var wkp in data)
                  {
                    var getDrillingOperationsData = (from c in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                    DrillingOperationsData = getDrillingOperationsData != null ? getDrillingOperationsData : DrillingOperationsData;
                    DrillingOperationsData = _mapper.Map<DRILLING_OPERATIONS_CATEGORIES_OF_WELL>(wkp);

                    #region file section
                    UploadedDocument FieldDiscoveryUploadFile_File = null;
                    UploadedDocument HydrocarbonCountUploadFile_File = null;

                    if (files[0] != null)
                    {
                        string docName = "Field Discovery";
                        FieldDiscoveryUploadFile_File = _helpersController.UploadDocument(files[0], "FieldDiscoveryDocuments");
                        if (FieldDiscoveryUploadFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    if (files[1] != null)
                    {
                        string docName = "Hydrocarbon Count";
                        HydrocarbonCountUploadFile_File = _helpersController.UploadDocument(files[1], "HydrocarbonCountDocuments");
                        if (HydrocarbonCountUploadFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion

                    DrillingOperationsData.FieldDiscoveryUploadFilePath = files[0] != null ? FieldDiscoveryUploadFile_File.filePath : null;
                    DrillingOperationsData.HydrocarbonCountUploadFilePath = files[1] != null ? HydrocarbonCountUploadFile_File.filePath : null;

                    DrillingOperationsData.Companyemail = WKPCompanyEmail;
                    DrillingOperationsData.CompanyName = WKPCompanyName;
                    DrillingOperationsData.COMPANY_ID = WKPCompanyId;
                    DrillingOperationsData.Date_Updated = DateTime.Now;
                    DrillingOperationsData.Updated_by = WKPCompanyId;
                    DrillingOperationsData.Year_of_WP = WorkProgramme_Year;
                    if (DrillingOperationsData == null)
                    {
                        DrillingOperationsData.Date_Created = DateTime.Now;
                        DrillingOperationsData.Created_by = WKPCompanyId;
                        await _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.AddAsync(DrillingOperationsData);
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Remove(DrillingOperationsData);
                    }

                        save += await _context.SaveChangesAsync();

                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("DRILLING_EACH_WELL_COST")]
        public async Task<WebApiResponse> DRILLING_EACH_WELL_COST(List<DRILLING_EACH_WELL_COST_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var DrillingData = new DRILLING_EACH_WELL_COST();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Drilling Operations
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {

                        var getDrillingData = (from c in _context.DRILLING_EACH_WELL_COSTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        DrillingData = getDrillingData != null ? getDrillingData : DrillingData;
                        DrillingData = _mapper.Map<DRILLING_EACH_WELL_COST>(wkp);

                        DrillingData.Companyemail = WKPCompanyEmail;
                        DrillingData.CompanyName = WKPCompanyName;
                        DrillingData.COMPANY_ID = WKPCompanyId;
                        DrillingData.Date_Updated = DateTime.Now;
                        DrillingData.Updated_by = WKPCompanyId;
                        DrillingData.Year_of_WP = WorkProgramme_Year;
                        if (DrillingData == null)
                        {
                            DrillingData.Date_Created = DateTime.Now;
                            DrillingData.Created_by = WKPCompanyId;
                            await _context.DRILLING_EACH_WELL_COSTs.AddAsync(DrillingData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.DRILLING_EACH_WELL_COSTs.Remove(DrillingData);
                        }

                        save += await _context.SaveChangesAsync();

                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

                #endregion
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
       
        [HttpPost("DRILLING_EACH_WELL_COST_PROPOSED")]
        public async Task<WebApiResponse> DRILLING_EACH_WELL_COST_PROPOSED(List<DRILLING_EACH_WELL_COST_PROPOSED_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var DrillingData = new DRILLING_EACH_WELL_COST_PROPOSED();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Drilling Operations
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {

                        var getDrillingData = (from c in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        DrillingData = getDrillingData != null ? getDrillingData : DrillingData;
                        DrillingData = _mapper.Map<DRILLING_EACH_WELL_COST_PROPOSED>(wkp);

                        DrillingData.Companyemail = WKPCompanyEmail;
                        DrillingData.CompanyName = WKPCompanyName;
                        DrillingData.COMPANY_ID = WKPCompanyId;
                        DrillingData.Date_Updated = DateTime.Now;
                        DrillingData.Updated_by = WKPCompanyId;
                        DrillingData.Year_of_WP = WorkProgramme_Year;
                        if (DrillingData == null)
                        {
                            DrillingData.Date_Created = DateTime.Now;
                            DrillingData.Created_by = WKPCompanyId;
                            await _context.DRILLING_EACH_WELL_COST_PROPOSEDs.AddAsync(DrillingData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.DRILLING_EACH_WELL_COST_PROPOSEDs.Remove(DrillingData);
                        }

                        save += await _context.SaveChangesAsync();

                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("INITIAL_WELL_COMPLETION_JOB1")]
        public async Task<WebApiResponse> INITIAL_WELL_COMPLETION_JOB1(List<INITIAL_WELL_COMPLETION_JOB1_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Well_Completion_JobData = new INITIAL_WELL_COMPLETION_JOB1();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Well_Completion_JobData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {

                        var getWell_Completion_JobData = (from c in _context.INITIAL_WELL_COMPLETION_JOBs1 where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Well_Completion_JobData = getWell_Completion_JobData != null ? getWell_Completion_JobData : Well_Completion_JobData;
                        Well_Completion_JobData = _mapper.Map<INITIAL_WELL_COMPLETION_JOB1>(wkp);

                        Well_Completion_JobData.Companyemail = WKPCompanyEmail;
                        Well_Completion_JobData.CompanyName = WKPCompanyName;
                        Well_Completion_JobData.COMPANY_ID = WKPCompanyId;
                        Well_Completion_JobData.Date_Updated = DateTime.Now;
                        Well_Completion_JobData.Updated_by = WKPCompanyId;
                        Well_Completion_JobData.Year_of_WP = WorkProgramme_Year;
                        if (getWell_Completion_JobData == null)
                        {
                            Well_Completion_JobData.Created_by = WKPCompanyId;
                            Well_Completion_JobData.Date_Created = DateTime.Now;
                            await _context.INITIAL_WELL_COMPLETION_JOBs1.AddAsync(Well_Completion_JobData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.INITIAL_WELL_COMPLETION_JOBs1.Remove(Well_Completion_JobData);
                        }

                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("WORKOVERS_RECOMPLETION_JOB1")]
        public async Task<WebApiResponse> WORKOVERS_RECOMPLETION_JOB1(List<WORKOVERS_RECOMPLETION_JOB1_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Workover_JobData = new WORKOVERS_RECOMPLETION_JOB1();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Workover JobData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
             
                        var getWorkover_JobData = (from c in _context.WORKOVERS_RECOMPLETION_JOBs1 where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Workover_JobData = getWorkover_JobData != null ? getWorkover_JobData : Workover_JobData;
                        Workover_JobData = _mapper.Map<WORKOVERS_RECOMPLETION_JOB1>(wkp);

                        Workover_JobData.Companyemail = WKPCompanyEmail;
                        Workover_JobData.CompanyName = WKPCompanyName;
                        Workover_JobData.COMPANY_ID = WKPCompanyId;
                        Workover_JobData.Date_Updated = DateTime.Now;
                        Workover_JobData.Updated_by = WKPCompanyId;
                        Workover_JobData.Year_of_WP = WorkProgramme_Year;
                        if (getWorkover_JobData == null)
                        {
                            Workover_JobData.Created_by = WKPCompanyId;
                            Workover_JobData.Date_Created = DateTime.Now;
                            await _context.WORKOVERS_RECOMPLETION_JOBs1.AddAsync(Workover_JobData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.WORKOVERS_RECOMPLETION_JOBs1.Remove(Workover_JobData);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE")]
        public async Task<WebApiResponse> FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE(List<FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERf_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var FDP_ExcessReserveData = new FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERf();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Well_Completion_JobData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {

                        var getFDP_ExcessReserveData = (from c in _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        FDP_ExcessReserveData = getFDP_ExcessReserveData != null ? getFDP_ExcessReserveData : FDP_ExcessReserveData;
                        FDP_ExcessReserveData = _mapper.Map<FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERf>(wkp);

                        FDP_ExcessReserveData.Companyemail = WKPCompanyEmail;
                        FDP_ExcessReserveData.CompanyName = WKPCompanyName;
                        FDP_ExcessReserveData.COMPANY_ID = WKPCompanyId;
                        FDP_ExcessReserveData.Date_Updated = DateTime.Now;
                        FDP_ExcessReserveData.Updated_by = WKPCompanyId;
                        FDP_ExcessReserveData.Year_of_WP = WorkProgramme_Year;
                        if (getFDP_ExcessReserveData == null)
                        {
                            FDP_ExcessReserveData.Created_by = WKPCompanyId;
                            FDP_ExcessReserveData.Date_Created = DateTime.Now;
                            await _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.AddAsync(FDP_ExcessReserveData);
                        }

                        else if (action == GeneralModel.Delete)
                        {
                            _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.Remove(FDP_ExcessReserveData);
                        }

                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP")]
        public async Task<WebApiResponse> FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP(List<FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var FDP_ToSubmitData = new FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving FDP_ToSubmitData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                                var getFDP_ToSubmitData = (from c in _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                                FDP_ToSubmitData = getFDP_ToSubmitData != null ? getFDP_ToSubmitData : FDP_ToSubmitData;
                                FDP_ToSubmitData = _mapper.Map<FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP>(wkp);

                                FDP_ToSubmitData.Companyemail = WKPCompanyEmail;
                                FDP_ToSubmitData.CompanyName = WKPCompanyName;
                                FDP_ToSubmitData.COMPANY_ID = WKPCompanyId;
                                FDP_ToSubmitData.Date_Updated = DateTime.Now;
                                FDP_ToSubmitData.Updated_by = WKPCompanyId;
                                FDP_ToSubmitData.Year_of_WP = WorkProgramme_Year;
                                if (getFDP_ToSubmitData == null)
                                {
                                    FDP_ToSubmitData.Created_by = WKPCompanyId;
                                    FDP_ToSubmitData.Date_Created = DateTime.Now;
                                    await _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs.AddAsync(FDP_ToSubmitData);
                                }
                                else if (action == GeneralModel.Delete)
                                {
                                    _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs.Remove(FDP_ToSubmitData);
                                }
                                save = await _context.SaveChangesAsync();

                                if (save > 0)
                                {
                                    string successMsg = "Form has been " + action + "D successfully.";
                                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                                }
                                else
                                {
                                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                                }
                            }
                         }
                        #endregion
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

                    }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("FIELD_DEVELOPMENT_FIELDS_AND_STATUS")]
        public async Task<WebApiResponse> FIELD_DEVELOPMENT_FIELDS_AND_STATUS(List<FIELD_DEVELOPMENT_FIELDS_AND_STATUS_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var FDP_StatusData = new FIELD_DEVELOPMENT_FIELDS_AND_STATUS();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving FDP_StatusData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                        var getFDP_StatusData = (from c in _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        FDP_StatusData = getFDP_StatusData != null ? getFDP_StatusData : FDP_StatusData;
                        FDP_StatusData = _mapper.Map<FIELD_DEVELOPMENT_FIELDS_AND_STATUS>(wkp);

                        FDP_StatusData.Companyemail = WKPCompanyEmail;
                        FDP_StatusData.CompanyName = WKPCompanyName;
                        FDP_StatusData.COMPANY_ID = WKPCompanyId;
                        FDP_StatusData.Date_Updated = DateTime.Now;
                        FDP_StatusData.Updated_by = WKPCompanyId;
                        FDP_StatusData.Year_of_WP = WorkProgramme_Year;
                        if (getFDP_StatusData == null)
                        {
                            FDP_StatusData.Created_by = WKPCompanyId;
                            FDP_StatusData.Date_Created = DateTime.Now;
                            await _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.AddAsync(FDP_StatusData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.Remove(FDP_StatusData);
                        }
                        save += await _context.SaveChangesAsync();

                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                        #endregion
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure};

             }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("RESERVES_UPDATES_LIFE_INDEX")]
        public async Task<WebApiResponse> RESERVES_UPDATES_LIFE_INDEX(List<RESERVES_UPDATES_LIFE_INDEX_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Reserve_UpdateData = new RESERVES_UPDATES_LIFE_INDEX();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Reserve_UpdateData              
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                         var getReserve_UpdateData = (from c in _context.RESERVES_UPDATES_LIFE_INDices where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Reserve_UpdateData = getReserve_UpdateData != null ? getReserve_UpdateData : Reserve_UpdateData;
                        Reserve_UpdateData = _mapper.Map<RESERVES_UPDATES_LIFE_INDEX>(wkp);

                        Reserve_UpdateData.Companyemail = WKPCompanyEmail;
                        Reserve_UpdateData.CompanyName = WKPCompanyName;
                        Reserve_UpdateData.COMPANY_ID = WKPCompanyId;
                        Reserve_UpdateData.Date_Updated = DateTime.Now;
                        Reserve_UpdateData.Updated_by = WKPCompanyId;
                        Reserve_UpdateData.Year_of_WP = WorkProgramme_Year;
                        if (getReserve_UpdateData == null)
                        {
                            Reserve_UpdateData.Created_by = WKPCompanyId;
                            Reserve_UpdateData.Date_Created = DateTime.Now;
                            await _context.RESERVES_UPDATES_LIFE_INDices.AddAsync(Reserve_UpdateData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.RESERVES_UPDATES_LIFE_INDices.Remove(Reserve_UpdateData);
                        }

                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("FIELD_DEVELOPMENT_PLAN")]
        public async Task<WebApiResponse> FIELD_DEVELOPMENT_PLAN(List<FIELD_DEVELOPMENT_PLAN_Model> data, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var FDP_Data = new FIELD_DEVELOPMENT_PLAN();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving FDP_Data
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
              
                        var getFDP_Data = (from c in _context.FIELD_DEVELOPMENT_PLANs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        FDP_Data = getFDP_Data != null ? getFDP_Data : FDP_Data;
                        FDP_Data = _mapper.Map<FIELD_DEVELOPMENT_PLAN>(wkp);

                        #region file section
                        UploadedDocument approved_FDP_Document = null;

                        if (files[0] != null)
                        {
                            string docName = "Approved FDP";
                            approved_FDP_Document = _helpersController.UploadDocument(files[0], "FDPDocuments");
                            if (approved_FDP_Document == null)
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                        }
                        #endregion

                        FDP_Data.Uploaded_approved_FDP_Document = files[0] != null ? approved_FDP_Document.filePath : null;
                        FDP_Data.FDPDocumentFilename = files[0] != null ? approved_FDP_Document.fileName : null;
                        FDP_Data.Companyemail = WKPCompanyEmail;
                        FDP_Data.CompanyName = WKPCompanyName;
                        FDP_Data.COMPANY_ID = WKPCompanyId;
                        FDP_Data.Date_Updated = DateTime.Now;
                        FDP_Data.Updated_by = WKPCompanyId;
                        FDP_Data.Year_of_WP = WorkProgramme_Year;
                        if (getFDP_Data == null)
                        {
                            FDP_Data.Created_by = WKPCompanyId;
                            FDP_Data.Date_Created = DateTime.Now;
                            await _context.FIELD_DEVELOPMENT_PLANs.AddAsync(FDP_Data);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.FIELD_DEVELOPMENT_PLANs.Remove(FDP_Data);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure};

            }
             catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("OIL_CONDENSATE_PRODUCTION_ACTIVITy")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITy(List<OIL_CONDENSATE_PRODUCTION_ACTIVITy_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Oil_ProductionData = new OIL_CONDENSATE_PRODUCTION_ACTIVITy();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Oil_ProductionData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                            var getOil_ProductionData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                            Oil_ProductionData = getOil_ProductionData != null ? getOil_ProductionData : Oil_ProductionData;
                            Oil_ProductionData = _mapper.Map<OIL_CONDENSATE_PRODUCTION_ACTIVITy>(wkp);

                            Oil_ProductionData.Companyemail = WKPCompanyEmail;
                            Oil_ProductionData.CompanyName = WKPCompanyName;
                            Oil_ProductionData.COMPANY_ID = WKPCompanyId;
                            Oil_ProductionData.Date_Updated = DateTime.Now;
                            Oil_ProductionData.Updated_by = WKPCompanyId;
                            Oil_ProductionData.Year_of_WP = DateTime.Now.Year.ToString();
                            if (getOil_ProductionData == null)
                            {
                                Oil_ProductionData.Created_by = WKPCompanyId;
                                Oil_ProductionData.Date_Created = DateTime.Now;
                                await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.AddAsync(Oil_ProductionData);
                            }
                            else if (action == GeneralModel.Delete)
                            {
                                _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.Remove(Oil_ProductionData);
                            }

                            save += await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                  }
                }
            #endregion
            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

             }
             catch (Exception e)
            {
               return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }
        
        [HttpPost("OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION(List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION_Model> data, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var Oil_UnitizationData = new OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Oil_Unitization Data
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                        var getOil_UnitizationData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Oil_UnitizationData = getOil_UnitizationData != null ? getOil_UnitizationData : Oil_UnitizationData;
                        Oil_UnitizationData = _mapper.Map<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION>(wkp);
                        #region file section
                        UploadedDocument PUAUploadFile = null;
                        UploadedDocument UUOAUploadFile = null;

                        if (files[0] != null)
                        {
                            string docName = "PUA";
                            PUAUploadFile = _helpersController.UploadDocument(files[0], "PUADocuments");
                            if (PUAUploadFile == null)
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                        }
                        if (files[1] != null)
                        {
                            string docName = "UUOA";
                            UUOAUploadFile = _helpersController.UploadDocument(files[1], "UUOADocuments");
                            if (UUOAUploadFile == null)
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                        }
                        #endregion

                        Oil_UnitizationData.PUAUploadFilePath = files[0] != null ? PUAUploadFile.filePath : null;
                        Oil_UnitizationData.PUAUploadFilename = files[0] != null ? PUAUploadFile.fileName : null;
                        Oil_UnitizationData.UUOAUploadFilePath = files[1] != null ? UUOAUploadFile.filePath : null;
                        Oil_UnitizationData.UUOAUploadFilename = files[1] != null ? UUOAUploadFile.fileName : null;
                        Oil_UnitizationData.Companyemail = WKPCompanyEmail;
                        Oil_UnitizationData.CompanyName = WKPCompanyName;
                        Oil_UnitizationData.COMPANY_ID = WKPCompanyId;
                        Oil_UnitizationData.Date_Updated = DateTime.Now;
                        Oil_UnitizationData.Updated_by = WKPCompanyId;
                        Oil_UnitizationData.Year_of_WP = WorkProgramme_Year;
                        if (getOil_UnitizationData == null)
                        {
                            Oil_UnitizationData.Created_by = WKPCompanyId;
                            Oil_UnitizationData.Date_Created = DateTime.Now;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.AddAsync(Oil_UnitizationData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.Remove(Oil_UnitizationData);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                     }
                    }
                 #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
       
        [HttpPost("GAS_PRODUCTION_ACTIVITy")]
        public async Task<WebApiResponse> GAS_PRODUCTION_ACTIVITy(List<GAS_PRODUCTION_ACTIVITy_Model> data, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var Gas_ProductionData = new GAS_PRODUCTION_ACTIVITy();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Gas_ProductionData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                       var getGas_ProductionData = (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Gas_ProductionData = getGas_ProductionData != null ? getGas_ProductionData : Gas_ProductionData;
                        Gas_ProductionData = _mapper.Map<GAS_PRODUCTION_ACTIVITy>(wkp);

                        #region file section
                        UploadedDocument Upload_NDR_payment_receipt_File = null;

                        if (files[0] != null)
                        {
                            string docName = "NDR Payment Receipt";
                            Upload_NDR_payment_receipt_File = _helpersController.UploadDocument(files[0], "NDRPaymentReceipt");
                            if (Upload_NDR_payment_receipt_File == null)
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                        }
                        #endregion


                        Gas_ProductionData.Upload_NDR_payment_receipt = files[0] != null ? Upload_NDR_payment_receipt_File.filePath : null;
                        Gas_ProductionData.NDRFilename = files[0] != null ? Upload_NDR_payment_receipt_File.fileName : null;
                        Gas_ProductionData.Companyemail = WKPCompanyEmail;
                        Gas_ProductionData.CompanyName = WKPCompanyName;
                        Gas_ProductionData.COMPANY_ID = WKPCompanyId;
                        Gas_ProductionData.Date_Updated = DateTime.Now;
                        Gas_ProductionData.Updated_by = WKPCompanyId;
                        Gas_ProductionData.Year_of_WP = WorkProgramme_Year;
                        if (getGas_ProductionData == null)
                        {
                            Gas_ProductionData.Created_by = WKPCompanyId;
                            Gas_ProductionData.Date_Created = DateTime.Now;
                            await _context.GAS_PRODUCTION_ACTIVITIEs.AddAsync(Gas_ProductionData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.GAS_PRODUCTION_ACTIVITIEs.Remove(Gas_ProductionData);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

             }
            catch (Exception e)
            {
             return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("NDR")]
        public async Task<WebApiResponse> NDR(List<NDR_Model> data, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var NDRData = new NDR();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving NDRData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                        var getGas_ProductionData = (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        var getNDRData = (from c in _context.NDRs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        NDRData = getNDRData != null ? getNDRData : NDRData;
                        NDRData = _mapper.Map<NDR>(wkp);
                        if (getGas_ProductionData != null)
                        {
                            NDRData.Upload_NDR_payment_receipt = getGas_ProductionData != null ? getGas_ProductionData.Upload_NDR_payment_receipt : null;
                            NDRData.NDRFilename = getGas_ProductionData != null ? getGas_ProductionData.NDRFilename : null;

                        }
                        NDRData.Companyemail = WKPCompanyEmail;
                        NDRData.CompanyName = WKPCompanyName;
                        NDRData.COMPANY_ID = WKPCompanyId;
                        NDRData.Date_Updated = DateTime.Now;
                        NDRData.Updated_by = WKPCompanyId;
                        NDRData.Year_of_WP = WorkProgramme_Year;
                        if (getNDRData == null)
                        {
                            NDRData.Created_by = WKPCompanyId;
                            NDRData.Date_Created = DateTime.Now;
                            await _context.NDRs.AddAsync(NDRData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.NDRs.Remove(NDRData);
                        }

                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE")]
        public async Task<WebApiResponse> RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE(List<RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Oil_Condensate_ReserveStatusData = new RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Oil_Condensate_ReserveStatusData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                        var getOil_Condensate_ReserveStatusData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Oil_Condensate_ReserveStatusData = getOil_Condensate_ReserveStatusData != null ? getOil_Condensate_ReserveStatusData : Oil_Condensate_ReserveStatusData;
                        Oil_Condensate_ReserveStatusData = _mapper.Map<RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE>(wkp);

                        Oil_Condensate_ReserveStatusData.Companyemail = WKPCompanyEmail;
                        Oil_Condensate_ReserveStatusData.CompanyName = WKPCompanyName;
                        Oil_Condensate_ReserveStatusData.COMPANY_ID = WKPCompanyId;
                        Oil_Condensate_ReserveStatusData.Date_Updated = DateTime.Now;
                        Oil_Condensate_ReserveStatusData.Updated_by = WKPCompanyId;
                        Oil_Condensate_ReserveStatusData.Year_of_WP = WorkProgramme_Year;
                        if (getOil_Condensate_ReserveStatusData == null)
                        {
                            Oil_Condensate_ReserveStatusData.Created_by = WKPCompanyId;
                            Oil_Condensate_ReserveStatusData.Date_Created = DateTime.Now;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.AddAsync(Oil_Condensate_ReserveStatusData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Remove(Oil_Condensate_ReserveStatusData);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection")]
        public async Task<WebApiResponse> RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection(List<RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Oil_Condensate_ReserveProjectionData = new RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                        #region Saving Oil_Condensate_ReserveProjectionData
                        if (data.Count() > 0)
                        {
                            foreach (var wkp in data)
                            {
                        var getOil_Condensate_ReserveProjectionData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Oil_Condensate_ReserveProjectionData = getOil_Condensate_ReserveProjectionData != null ? getOil_Condensate_ReserveProjectionData : Oil_Condensate_ReserveProjectionData;
                        Oil_Condensate_ReserveProjectionData = _mapper.Map<RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection>(wkp);

                        Oil_Condensate_ReserveProjectionData.Companyemail = WKPCompanyEmail;
                        Oil_Condensate_ReserveProjectionData.CompanyName = WKPCompanyName;
                        Oil_Condensate_ReserveProjectionData.COMPANY_ID = WKPCompanyId;
                        Oil_Condensate_ReserveProjectionData.Date_Updated = DateTime.Now;
                        Oil_Condensate_ReserveProjectionData.Updated_by = WKPCompanyId;
                        Oil_Condensate_ReserveProjectionData.Year_of_WP = WorkProgramme_Year;
                        if (getOil_Condensate_ReserveProjectionData == null)
                        {
                            Oil_Condensate_ReserveProjectionData.Created_by = WKPCompanyId;
                            Oil_Condensate_ReserveProjectionData.Date_Created = DateTime.Now;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections.AddAsync(Oil_Condensate_ReserveProjectionData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections.Remove(Oil_Condensate_ReserveProjectionData);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION(List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION_Model> data, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var Oil_Condensate_ProjectionData = new OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Oil_Condensate_ProjectionData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                            var getOil_Condensate_ProjectionData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                            Oil_Condensate_ProjectionData = getOil_Condensate_ProjectionData != null ? getOil_Condensate_ProjectionData : Oil_Condensate_ProjectionData;
                            Oil_Condensate_ProjectionData = _mapper.Map<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION>(wkp);

                            #region file section
                            UploadedDocument ProductionOilCondensateAGNAGFile_File = null;

                            if (files[0] != null)
                            {
                                string docName = "ProductionOilCondensateAGNAG";
                                ProductionOilCondensateAGNAGFile_File = _helpersController.UploadDocument(files[0], "ProductionOilCondensateAGNAGDocument");
                                if (ProductionOilCondensateAGNAGFile_File == null)
                                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                            }
                            #endregion

                            Oil_Condensate_ProjectionData.ProductionOilCondensateAGNAGUFilename = files[0] != null ? ProductionOilCondensateAGNAGFile_File.fileName : null;
                            Oil_Condensate_ProjectionData.ProductionOilCondensateAGNAGUploadFilePath = files[0] != null ? ProductionOilCondensateAGNAGFile_File.filePath : null;
                            Oil_Condensate_ProjectionData.Companyemail = WKPCompanyEmail;
                            Oil_Condensate_ProjectionData.CompanyName = WKPCompanyName;
                            Oil_Condensate_ProjectionData.COMPANY_ID = WKPCompanyId;
                            Oil_Condensate_ProjectionData.Date_Updated = DateTime.Now;
                            Oil_Condensate_ProjectionData.Updated_by = WKPCompanyId;
                            Oil_Condensate_ProjectionData.Year_of_WP = DateTime.Now.Year.ToString();
                            if (getOil_Condensate_ProjectionData == null)
                            {
                                Oil_Condensate_ProjectionData.Created_by = WKPCompanyId;
                                Oil_Condensate_ProjectionData.Date_Created = DateTime.Now;
                                await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.AddAsync(Oil_Condensate_ProjectionData);
                            }
                            else if (action == GeneralModel.Delete)
                            {
                                _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.Remove(Oil_Condensate_ProjectionData);
                            }
                            save += await _context.SaveChangesAsync();
                            if (save > 0)
                            {
                                string successMsg = "Form has been " + action + "D successfully.";
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                            }
                            else
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                            }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
     
        [HttpPost("RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION")]
        public async Task<WebApiResponse> RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION(List<RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Oil_Condensate_AnnualData = new RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Oil_Condensate_AnnualData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                        var getOil_Condensate_AnnualData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Oil_Condensate_AnnualData = getOil_Condensate_AnnualData != null ? getOil_Condensate_AnnualData : Oil_Condensate_AnnualData;
                        Oil_Condensate_AnnualData = _mapper.Map<RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION>(wkp);

                        Oil_Condensate_AnnualData.Companyemail = WKPCompanyEmail;
                        Oil_Condensate_AnnualData.CompanyName = WKPCompanyName;
                        Oil_Condensate_AnnualData.COMPANY_ID = WKPCompanyId;
                        Oil_Condensate_AnnualData.Date_Updated = DateTime.Now;
                        Oil_Condensate_AnnualData.Updated_by = WKPCompanyId;
                        Oil_Condensate_AnnualData.Year_of_WP = DateTime.Now.Year.ToString();
                        if (getOil_Condensate_AnnualData == null)
                        {
                            Oil_Condensate_AnnualData.Created_by = WKPCompanyId;
                            Oil_Condensate_AnnualData.Date_Created = DateTime.Now;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.AddAsync(Oil_Condensate_AnnualData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.Remove(Oil_Condensate_AnnualData);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE")]
        public async Task<WebApiResponse> RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE(List<RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Oil_Condensate_DeclineData = new RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Oil_Condensate_DeclineData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                        
                        var getOil_Condensate_DeclineData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Oil_Condensate_DeclineData = getOil_Condensate_DeclineData != null ? getOil_Condensate_DeclineData : Oil_Condensate_DeclineData;
                        Oil_Condensate_DeclineData = _mapper.Map<RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE>(wkp);

                        Oil_Condensate_DeclineData.Companyemail = WKPCompanyEmail;
                        Oil_Condensate_DeclineData.CompanyName = WKPCompanyName;
                        Oil_Condensate_DeclineData.COMPANY_ID = WKPCompanyId;
                        Oil_Condensate_DeclineData.Date_Updated = DateTime.Now;
                        Oil_Condensate_DeclineData.Updated_by = WKPCompanyId;
                        Oil_Condensate_DeclineData.Year_of_WP = DateTime.Now.Year.ToString();
                        if (getOil_Condensate_DeclineData == null)
                        {
                            Oil_Condensate_DeclineData.Created_by = WKPCompanyId;
                            Oil_Condensate_DeclineData.Date_Created = DateTime.Now;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.AddAsync(Oil_Condensate_DeclineData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.Remove(Oil_Condensate_DeclineData);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity(List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Oil_Condensate_ProductionData = new OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Oil_Condensate_ProductionData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                         var getOil_Condensate_ProductionData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Oil_Condensate_ProductionData = getOil_Condensate_ProductionData != null ? getOil_Condensate_ProductionData : Oil_Condensate_ProductionData;
                        Oil_Condensate_ProductionData = _mapper.Map<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity>(wkp);

                        Oil_Condensate_ProductionData.Companyemail = WKPCompanyEmail;
                        Oil_Condensate_ProductionData.CompanyName = WKPCompanyName;
                        Oil_Condensate_ProductionData.COMPANY_ID = WKPCompanyId;
                        Oil_Condensate_ProductionData.Date_Updated = DateTime.Now;
                        Oil_Condensate_ProductionData.Updated_by = WKPCompanyId;
                        Oil_Condensate_ProductionData.Year_of_WP = DateTime.Now.Year.ToString();
                        if (getOil_Condensate_ProductionData == null)
                        {
                            Oil_Condensate_ProductionData.Created_by = WKPCompanyId;
                            Oil_Condensate_ProductionData.Date_Created = DateTime.Now;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.AddAsync(Oil_Condensate_ProductionData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.Remove(Oil_Condensate_ProductionData);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("RESERVES_REPLACEMENT_RATIO")]
        public async Task<WebApiResponse> RESERVES_REPLACEMENT_RATIO(List<RESERVES_REPLACEMENT_RATIO_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Reserve_ReplacementData = new RESERVES_REPLACEMENT_RATIO();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Reserve_ReplacementData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                        var getReserve_ReplacementData = (from c in _context.RESERVES_REPLACEMENT_RATIOs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Reserve_ReplacementData = getReserve_ReplacementData != null ? getReserve_ReplacementData : Reserve_ReplacementData;
                        Reserve_ReplacementData = _mapper.Map<RESERVES_REPLACEMENT_RATIO>(wkp);

                        Reserve_ReplacementData.Companyemail = WKPCompanyEmail;
                        Reserve_ReplacementData.CompanyName = WKPCompanyName;
                        Reserve_ReplacementData.COMPANY_ID = WKPCompanyId;
                        Reserve_ReplacementData.Date_Updated = DateTime.Now;
                        Reserve_ReplacementData.Updated_by = WKPCompanyId;
                        Reserve_ReplacementData.Year_of_WP = DateTime.Now.Year.ToString();
                        if (getReserve_ReplacementData == null)
                        {
                            Reserve_ReplacementData.Created_by = WKPCompanyId;
                            Reserve_ReplacementData.Date_Created = DateTime.Now;
                            await _context.RESERVES_REPLACEMENT_RATIOs.AddAsync(Reserve_ReplacementData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.RESERVES_REPLACEMENT_RATIOs.Remove(Reserve_ReplacementData);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED(List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Oil_MonthlyData = new OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Oil_MonthlyData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                        var getOil_MonthlyData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Oil_MonthlyData = getOil_MonthlyData != null ? getOil_MonthlyData : Oil_MonthlyData;
                        Oil_MonthlyData = _mapper.Map<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED>(wkp);

                        Oil_MonthlyData.Companyemail = WKPCompanyEmail;
                        Oil_MonthlyData.CompanyName = WKPCompanyName;
                        Oil_MonthlyData.COMPANY_ID = WKPCompanyId;
                        Oil_MonthlyData.Date_Updated = DateTime.Now;
                        Oil_MonthlyData.Updated_by = WKPCompanyId;
                        Oil_MonthlyData.Year_of_WP = DateTime.Now.Year.ToString();
                        if (getOil_MonthlyData == null)
                        {
                            Oil_MonthlyData.Created_by = WKPCompanyId;
                            Oil_MonthlyData.Date_Created = DateTime.Now;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.AddAsync(Oil_MonthlyData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.Remove(Oil_MonthlyData);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY")]
        public async Task<WebApiResponse> GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY(List<GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY_Model> data, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var Gas_ActivitiesData = new GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Gas_ActivitiesData
                if (data.Count() > 0)
                {
                    foreach (var wkp in data)
                    {
                        var getGas_ActivitiesData = (from c in _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                        Gas_ActivitiesData = getGas_ActivitiesData != null ? getGas_ActivitiesData : Gas_ActivitiesData;
                        Gas_ActivitiesData = _mapper.Map<GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY>(wkp);

                        Gas_ActivitiesData.Companyemail = WKPCompanyEmail;
                        Gas_ActivitiesData.CompanyName = WKPCompanyName;
                        Gas_ActivitiesData.COMPANY_ID = WKPCompanyId;
                        Gas_ActivitiesData.Date_Updated = DateTime.Now;
                        Gas_ActivitiesData.Updated_by = WKPCompanyId;
                        Gas_ActivitiesData.Year_of_WP = DateTime.Now.Year.ToString();
                        if (getGas_ActivitiesData == null)
                        {
                            Gas_ActivitiesData.Created_by = WKPCompanyId;
                            Gas_ActivitiesData.Date_Created = DateTime.Now;
                            await _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.AddAsync(Gas_ActivitiesData);
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.Remove(Gas_ActivitiesData);
                        }
                        save += await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                }
                #endregion
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }


        [HttpPost("BUDGET_ACTUAL_EXPENDITURE")]
        public async Task<WebApiResponse> BUDGET_ACTUAL_EXPENDITURE(BUDGET_ACTUAL_EXPENDITURE_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var BudgetActualExData = new BUDGET_ACTUAL_EXPENDITURE();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Gas_ActivitiesData
                var getBudgetActualExData = (from c in _context.BUDGET_ACTUAL_EXPENDITUREs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                BudgetActualExData = getBudgetActualExData != null ? getBudgetActualExData : BudgetActualExData;
                BudgetActualExData = _mapper.Map<BUDGET_ACTUAL_EXPENDITURE>(wkp);

                BudgetActualExData.Companyemail = WKPCompanyEmail;
                BudgetActualExData.CompanyName = WKPCompanyName;
                BudgetActualExData.COMPANY_ID = WKPCompanyId;
                BudgetActualExData.Date_Updated = DateTime.Now;
                BudgetActualExData.Updated_by = WKPCompanyId;
                BudgetActualExData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getBudgetActualExData == null)
                {
                    BudgetActualExData.Created_by = WKPCompanyId;
                    BudgetActualExData.Date_Created = DateTime.Now;
                    await _context.BUDGET_ACTUAL_EXPENDITUREs.AddAsync(BudgetActualExData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.BUDGET_ACTUAL_EXPENDITUREs.Remove(BudgetActualExData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT")]
        public async Task<WebApiResponse> BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT(BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var BudgetProposalData = new BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Budget Proposal

                var getBudgetProposalData = (from c in _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                BudgetProposalData = getBudgetProposalData != null ? getBudgetProposalData : BudgetProposalData;
                BudgetProposalData = _mapper.Map<BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT>(wkp);

                BudgetProposalData.Companyemail = WKPCompanyEmail;
                BudgetProposalData.CompanyName = WKPCompanyName;
                BudgetProposalData.COMPANY_ID = WKPCompanyId;
                BudgetProposalData.Date_Updated = DateTime.Now;
                BudgetProposalData.Updated_by = WKPCompanyId;
                BudgetProposalData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getBudgetProposalData == null)
                {
                    BudgetProposalData.Created_by = WKPCompanyId;
                    BudgetProposalData.Date_Created = DateTime.Now;

                    await _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs.AddAsync(BudgetProposalData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs.Remove(BudgetProposalData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy")]
        public async Task<WebApiResponse> BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy(BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var BudgetPerformanceExpData = new BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Budget Performance Exploratory

                var getBudgetPerformanceExpData = (from c in _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                BudgetPerformanceExpData = getBudgetPerformanceExpData != null ? getBudgetPerformanceExpData : BudgetPerformanceExpData;
                BudgetPerformanceExpData = _mapper.Map<BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy>(wkp);

                BudgetPerformanceExpData.Companyemail = WKPCompanyEmail;
                BudgetPerformanceExpData.CompanyName = WKPCompanyName;
                BudgetPerformanceExpData.COMPANY_ID = WKPCompanyId;
                BudgetPerformanceExpData.Date_Updated = DateTime.Now;
                BudgetPerformanceExpData.Updated_by = WKPCompanyId;
                BudgetPerformanceExpData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getBudgetPerformanceExpData == null)
                {
                    BudgetPerformanceExpData.Created_by = WKPCompanyId;
                    BudgetPerformanceExpData.Date_Created = DateTime.Now;
                    await _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs.AddAsync(BudgetPerformanceExpData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs.Remove(BudgetPerformanceExpData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy")]
        public async Task<WebApiResponse> BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy(BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var BudgetPerformanceDevData = new BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Budget Performance Development

                var getBudgetPerformanceDevData = (from c in _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                BudgetPerformanceDevData = getBudgetPerformanceDevData != null ? getBudgetPerformanceDevData : BudgetPerformanceDevData;
                BudgetPerformanceDevData = _mapper.Map<BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy>(wkp);

                BudgetPerformanceDevData.Companyemail = WKPCompanyEmail;
                BudgetPerformanceDevData.CompanyName = WKPCompanyName;
                BudgetPerformanceDevData.COMPANY_ID = WKPCompanyId;
                BudgetPerformanceDevData.Date_Updated = DateTime.Now;
                BudgetPerformanceDevData.Updated_by = WKPCompanyId;
                BudgetPerformanceDevData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getBudgetPerformanceDevData == null)
                {
                    BudgetPerformanceDevData.Created_by = WKPCompanyId;
                    BudgetPerformanceDevData.Date_Created = DateTime.Now;
                    await _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs.AddAsync(BudgetPerformanceDevData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs.Remove(BudgetPerformanceDevData);
                }
                save += await _context.SaveChangesAsync();
                #endregion


                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("BUDGET_PERFORMANCE_PRODUCTION_COST")]
        public async Task<WebApiResponse> BUDGET_PERFORMANCE_PRODUCTION_COST(BUDGET_PERFORMANCE_PRODUCTION_COST_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var BudgetPerformanceProdData = new BUDGET_PERFORMANCE_PRODUCTION_COST();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Budget Performance Production

                var getBudgetPerformanceProdData = (from c in _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                BudgetPerformanceProdData = getBudgetPerformanceProdData != null ? getBudgetPerformanceProdData : BudgetPerformanceProdData;
                BudgetPerformanceProdData = _mapper.Map<BUDGET_PERFORMANCE_PRODUCTION_COST>(wkp);

                BudgetPerformanceProdData.Companyemail = WKPCompanyEmail;
                BudgetPerformanceProdData.CompanyName = WKPCompanyName;
                BudgetPerformanceProdData.COMPANY_ID = WKPCompanyId;
                BudgetPerformanceProdData.Date_Updated = DateTime.Now;
                BudgetPerformanceProdData.Updated_by = WKPCompanyId;
                BudgetPerformanceProdData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getBudgetPerformanceProdData == null)
                {
                    BudgetPerformanceProdData.Created_by = WKPCompanyId;
                    BudgetPerformanceProdData.Date_Created = DateTime.Now;
                    await _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs.AddAsync(BudgetPerformanceProdData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs.Remove(BudgetPerformanceProdData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT")]
        public async Task<WebApiResponse> BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT(BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var BudgetPerformanceFacData = new BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Budget Performance Facility

                var getBudgetPerformanceFacData = (from c in _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                BudgetPerformanceFacData = getBudgetPerformanceFacData != null ? getBudgetPerformanceFacData : BudgetPerformanceFacData;
                BudgetPerformanceFacData = _mapper.Map<BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT>(wkp);

                BudgetPerformanceFacData.Companyemail = WKPCompanyEmail;
                BudgetPerformanceFacData.CompanyName = WKPCompanyName;
                BudgetPerformanceFacData.COMPANY_ID = WKPCompanyId;
                BudgetPerformanceFacData.Date_Updated = DateTime.Now;
                BudgetPerformanceFacData.Updated_by = WKPCompanyId;
                BudgetPerformanceFacData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getBudgetPerformanceFacData == null)
                {
                    BudgetPerformanceFacData.Created_by = WKPCompanyId;
                    BudgetPerformanceFacData.Date_Created = DateTime.Now;
                    await _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs.AddAsync(BudgetPerformanceFacData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs.Remove(BudgetPerformanceFacData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE")]
        public async Task<WebApiResponse> OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE(OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var OilGasFacData = new OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Oil & Gas Facility

                var getOilGasFacData = (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                OilGasFacData = getOilGasFacData != null ? getOilGasFacData : OilGasFacData;
                OilGasFacData = _mapper.Map<OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE>(wkp);

                OilGasFacData.Companyemail = WKPCompanyEmail;
                OilGasFacData.CompanyName = WKPCompanyName;
                OilGasFacData.COMPANY_ID = WKPCompanyId;
                OilGasFacData.Date_Updated = DateTime.Now;
                OilGasFacData.Updated_by = WKPCompanyId;
                OilGasFacData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getOilGasFacData == null)
                {
                    OilGasFacData.Created_by = WKPCompanyId;
                    OilGasFacData.Date_Created = DateTime.Now;
                    await _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs.AddAsync(OilGasFacData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs.Remove(OilGasFacData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment(OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var OilGasProdData = new OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Oil & Gas Production

                var getOilGasProdData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                OilGasProdData = getOilGasProdData != null ? getOilGasProdData : OilGasProdData;
                OilGasProdData = _mapper.Map<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment>(wkp);

                OilGasProdData.Companyemail = WKPCompanyEmail;
                OilGasProdData.CompanyName = WKPCompanyName;
                OilGasProdData.COMPANY_ID = WKPCompanyId;
                OilGasProdData.Date_Updated = DateTime.Now;
                OilGasProdData.Updated_by = WKPCompanyId;
                OilGasProdData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getOilGasProdData == null)
                {
                    OilGasProdData.Created_by = WKPCompanyId;
                    OilGasProdData.Date_Created = DateTime.Now;
                    await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.AddAsync(OilGasProdData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.Remove(OilGasProdData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT")]
        public async Task<WebApiResponse> OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT(OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var OilGasFacProjectData = new OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Oil Gas Facility Project

                var getOilGasFacProjectData = (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                OilGasFacProjectData = getOilGasFacProjectData != null ? getOilGasFacProjectData : OilGasFacProjectData;
                OilGasFacProjectData = _mapper.Map<OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT>(wkp);

                OilGasFacProjectData.Companyemail = WKPCompanyEmail;
                OilGasFacProjectData.CompanyName = WKPCompanyName;
                OilGasFacProjectData.COMPANY_ID = WKPCompanyId;
                OilGasFacProjectData.Date_Updated = DateTime.Now;
                OilGasFacProjectData.Updated_by = WKPCompanyId;
                OilGasFacProjectData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getOilGasFacProjectData == null)
                {
                    OilGasFacProjectData.Created_by = WKPCompanyId;
                    OilGasFacProjectData.Date_Created = DateTime.Now;
                    await _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.AddAsync(OilGasFacProjectData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Remove(OilGasFacProjectData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("FACILITIES_PROJECT_PERFORMANCE")]
        public async Task<WebApiResponse> FACILITIES_PROJECT_PERFORMANCE(FACILITIES_PROJECT_PERFORMANCE_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var FacProjectData = new FACILITIES_PROJECT_PERFORMANCE();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Facility Project

                var getFacProjectData = (from c in _context.FACILITIES_PROJECT_PERFORMANCEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                FacProjectData = getFacProjectData != null ? getFacProjectData : FacProjectData;
                FacProjectData = _mapper.Map<FACILITIES_PROJECT_PERFORMANCE>(wkp);

                FacProjectData.Companyemail = WKPCompanyEmail;
                FacProjectData.CompanyName = WKPCompanyName;
                FacProjectData.COMPANY_ID = WKPCompanyId;
                FacProjectData.Date_Updated = DateTime.Now;
                FacProjectData.Updated_by = WKPCompanyId;
                FacProjectData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getFacProjectData == null)
                {
                    FacProjectData.Created_by = WKPCompanyId;
                    FacProjectData.Date_Created = DateTime.Now;
                    await _context.FACILITIES_PROJECT_PERFORMANCEs.AddAsync(FacProjectData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.FACILITIES_PROJECT_PERFORMANCEs.Remove(FacProjectData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("BUDGET_CAPEX_OPEX")]
        public async Task<WebApiResponse> BUDGET_CAPEX_OPEX(BUDGET_CAPEX_OPEX_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var BudgetCapexOpexData = new BUDGET_CAPEX_OPEX();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Budget Capex Opex

                var getBudgetCapexOpexData = (from c in _context.BUDGET_CAPEX_OPices where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                BudgetCapexOpexData = getBudgetCapexOpexData != null ? getBudgetCapexOpexData : BudgetCapexOpexData;
                BudgetCapexOpexData = _mapper.Map<BUDGET_CAPEX_OPEX>(wkp);

                BudgetCapexOpexData.Companyemail = WKPCompanyEmail;
                BudgetCapexOpexData.CompanyName = WKPCompanyName;
                BudgetCapexOpexData.COMPANY_ID = WKPCompanyId;
                BudgetCapexOpexData.Date_Updated = DateTime.Now;
                BudgetCapexOpexData.Updated_by = WKPCompanyId;
                BudgetCapexOpexData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getBudgetCapexOpexData == null)
                {
                    BudgetCapexOpexData.Created_by = WKPCompanyId;
                    BudgetCapexOpexData.Date_Created = DateTime.Now;
                    await _context.BUDGET_CAPEX_OPices.AddAsync(BudgetCapexOpexData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.BUDGET_CAPEX_OPices.Remove(BudgetCapexOpexData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("NIGERIA_CONTENT_Training")]
        public async Task<WebApiResponse> NIGERIA_CONTENT_Training(NIGERIA_CONTENT_Training_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var NigeriaTrainingData = new NIGERIA_CONTENT_Training();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Nigeria Training
                var getNigeriaTrainingData = (from c in _context.NIGERIA_CONTENT_Trainings where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                NigeriaTrainingData = getNigeriaTrainingData != null ? getNigeriaTrainingData : NigeriaTrainingData;
                NigeriaTrainingData = _mapper.Map<NIGERIA_CONTENT_Training>(wkp);

                NigeriaTrainingData.Companyemail = WKPCompanyEmail;
                NigeriaTrainingData.CompanyName = WKPCompanyName;
                NigeriaTrainingData.COMPANY_ID = WKPCompanyId;
                NigeriaTrainingData.Date_Updated = DateTime.Now;
                NigeriaTrainingData.Updated_by = WKPCompanyId;
                NigeriaTrainingData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getNigeriaTrainingData == null)
                {
                    NigeriaTrainingData.Created_by = WKPCompanyId;
                    NigeriaTrainingData.Date_Created = DateTime.Now;
                    await _context.NIGERIA_CONTENT_Trainings.AddAsync(NigeriaTrainingData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.NIGERIA_CONTENT_Trainings.Remove(NigeriaTrainingData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("NIGERIA_CONTENT_Upload_Succession_Plan")]
        public async Task<WebApiResponse> NIGERIA_CONTENT_Upload_Succession_Plan(NIGERIA_CONTENT_Upload_Succession_Plan_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var NigeriaUploadData = new NIGERIA_CONTENT_Upload_Succession_Plan();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Nigeria Upload

                var getNigeriaUploadData = (from c in _context.NIGERIA_CONTENT_Upload_Succession_Plans where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                NigeriaUploadData = getNigeriaUploadData != null ? getNigeriaUploadData : NigeriaUploadData;
                NigeriaUploadData = _mapper.Map<NIGERIA_CONTENT_Upload_Succession_Plan>(wkp);

                NigeriaUploadData.Companyemail = WKPCompanyEmail;
                NigeriaUploadData.CompanyName = WKPCompanyName;
                NigeriaUploadData.COMPANY_ID = WKPCompanyId;
                NigeriaUploadData.Date_Updated = DateTime.Now;
                NigeriaUploadData.Updated_by = WKPCompanyId;
                NigeriaUploadData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getNigeriaUploadData == null)
                {
                    NigeriaUploadData.Created_by = WKPCompanyId;
                    NigeriaUploadData.Date_Created = DateTime.Now;
                    await _context.NIGERIA_CONTENT_Upload_Succession_Plans.AddAsync(NigeriaUploadData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.NIGERIA_CONTENT_Upload_Succession_Plans.Remove(NigeriaUploadData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("NIGERIA_CONTENT_QUESTION")]
        public async Task<WebApiResponse> NIGERIA_CONTENT_QUESTION(NIGERIA_CONTENT_QUESTION_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var NigeriaQuestionData = new NIGERIA_CONTENT_QUESTION();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Nigeria Question

                var getNigeriaQuestionData = (from c in _context.NIGERIA_CONTENT_QUESTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                NigeriaQuestionData = getNigeriaQuestionData != null ? getNigeriaQuestionData : NigeriaQuestionData;
                NigeriaQuestionData = _mapper.Map<NIGERIA_CONTENT_QUESTION>(wkp);

                NigeriaQuestionData.Companyemail = WKPCompanyEmail;
                NigeriaQuestionData.CompanyName = WKPCompanyName;
                NigeriaQuestionData.COMPANY_ID = WKPCompanyId;
                NigeriaQuestionData.Date_Updated = DateTime.Now;
                NigeriaQuestionData.Updated_by = WKPCompanyId;
                NigeriaQuestionData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getNigeriaQuestionData == null)
                {
                    NigeriaQuestionData.Created_by = WKPCompanyId;
                    NigeriaQuestionData.Date_Created = DateTime.Now;
                    await _context.NIGERIA_CONTENT_QUESTIONs.AddAsync(NigeriaQuestionData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.NIGERIA_CONTENT_QUESTIONs.Remove(NigeriaQuestionData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("LEGAL_LITIGATION")]
        public async Task<WebApiResponse> LEGAL_LITIGATION(LEGAL_LITIGATION_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var LegalLitigationData = new LEGAL_LITIGATION();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Legal Litigation

                var getLegalLitigationData = (from c in _context.LEGAL_LITIGATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                LegalLitigationData = getLegalLitigationData != null ? getLegalLitigationData : LegalLitigationData;
                LegalLitigationData = _mapper.Map<LEGAL_LITIGATION>(wkp);

                LegalLitigationData.Companyemail = WKPCompanyEmail;
                LegalLitigationData.CompanyName = WKPCompanyName;
                LegalLitigationData.COMPANY_ID = WKPCompanyId;
                LegalLitigationData.Date_Updated = DateTime.Now;
                LegalLitigationData.Updated_by = WKPCompanyId;
                LegalLitigationData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getLegalLitigationData == null)
                {
                    LegalLitigationData.Created_by = WKPCompanyId;
                    LegalLitigationData.Date_Created = DateTime.Now;
                    await _context.LEGAL_LITIGATIONs.AddAsync(LegalLitigationData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.LEGAL_LITIGATIONs.Remove(LegalLitigationData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("LEGAL_ARBITRATION")]
        public async Task<WebApiResponse> LEGAL_ARBITRATION(LEGAL_ARBITRATION_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var LegalArbitrationData = new LEGAL_ARBITRATION();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Legal Arbitration

                var getLegalArbitrationData = (from c in _context.LEGAL_ARBITRATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                LegalArbitrationData = getLegalArbitrationData != null ? getLegalArbitrationData : LegalArbitrationData;
                LegalArbitrationData = _mapper.Map<LEGAL_ARBITRATION>(wkp);

                LegalArbitrationData.Companyemail = WKPCompanyEmail;
                LegalArbitrationData.CompanyName = WKPCompanyName;
                LegalArbitrationData.COMPANY_ID = WKPCompanyId;
                LegalArbitrationData.Date_Updated = DateTime.Now;
                LegalArbitrationData.Updated_by = WKPCompanyId;
                LegalArbitrationData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getLegalArbitrationData == null)
                {
                    LegalArbitrationData.Created_by = WKPCompanyId;
                    LegalArbitrationData.Date_Created = DateTime.Now;
                    await _context.LEGAL_ARBITRATIONs.AddAsync(LegalArbitrationData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.LEGAL_ARBITRATIONs.Remove(LegalArbitrationData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("STRATEGIC_PLANS_ON_COMPANY_BASI")]
        public async Task<WebApiResponse> STRATEGIC_PLANS_ON_COMPANY_BASI(STRATEGIC_PLANS_ON_COMPANY_BASI_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var StrategicPlansData = new STRATEGIC_PLANS_ON_COMPANY_BASI();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Strategic Plans

                var getStrategicPlansData = (from c in _context.STRATEGIC_PLANS_ON_COMPANY_BAses where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                StrategicPlansData = getStrategicPlansData != null ? getStrategicPlansData : StrategicPlansData;
                StrategicPlansData = _mapper.Map<STRATEGIC_PLANS_ON_COMPANY_BASI>(wkp);

                StrategicPlansData.Companyemail = WKPCompanyEmail;
                StrategicPlansData.CompanyName = WKPCompanyName;
                StrategicPlansData.COMPANY_ID = WKPCompanyId;
                StrategicPlansData.Date_Updated = DateTime.Now;
                StrategicPlansData.Updated_by = WKPCompanyId;
                StrategicPlansData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getStrategicPlansData == null)
                {
                    StrategicPlansData.Created_by = WKPCompanyId;
                    StrategicPlansData.Date_Created = DateTime.Now;
                    await _context.STRATEGIC_PLANS_ON_COMPANY_BAses.AddAsync(StrategicPlansData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.STRATEGIC_PLANS_ON_COMPANY_BAses.Remove(StrategicPlansData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_QUESTION")]
        public async Task<WebApiResponse> HSE_QUESTION(HSE_QUESTION_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseQuestionData = new HSE_QUESTION();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Hse Question
                var getHseQuestionData = (from c in _context.HSE_QUESTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseQuestionData = getHseQuestionData != null ? getHseQuestionData : HseQuestionData;
                HseQuestionData = _mapper.Map<HSE_QUESTION>(wkp);

                HseQuestionData.Companyemail = WKPCompanyEmail;
                HseQuestionData.CompanyName = WKPCompanyName;
                HseQuestionData.COMPANY_ID = WKPCompanyId;
                HseQuestionData.Date_Updated = DateTime.Now;
                HseQuestionData.Updated_by = WKPCompanyId;
                HseQuestionData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseQuestionData == null)
                {
                    HseQuestionData.Created_by = WKPCompanyId;
                    HseQuestionData.Date_Created = DateTime.Now;
                    await _context.HSE_QUESTIONs.AddAsync(getHseQuestionData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_QUESTIONs.Remove(getHseQuestionData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_FATALITy")]
        public async Task<WebApiResponse> HSE_FATALITy(HSE_FATALITy_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseFatalityData = new HSE_FATALITy();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Hse Fatality
                var getHseFatalityData = (from c in _context.HSE_FATALITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseFatalityData = getHseFatalityData != null ? getHseFatalityData : HseFatalityData;
                HseFatalityData = _mapper.Map<HSE_FATALITy>(wkp);

                HseFatalityData.Companyemail = WKPCompanyEmail;
                HseFatalityData.CompanyName = WKPCompanyName;
                HseFatalityData.COMPANY_ID = WKPCompanyId;
                HseFatalityData.Date_Updated = DateTime.Now;
                HseFatalityData.Updated_by = WKPCompanyId;
                HseFatalityData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseFatalityData == null)
                {
                    HseFatalityData.Created_by = WKPCompanyId;
                    HseFatalityData.Date_Created = DateTime.Now;
                    await _context.HSE_FATALITIEs.AddAsync(getHseFatalityData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_FATALITIEs.Remove(getHseFatalityData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_DESIGNS_SAFETY")]
        public async Task<WebApiResponse> HSE_DESIGNS_SAFETY(HSE_DESIGNS_SAFETY_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseDesignData = new HSE_DESIGNS_SAFETY();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Hse Design Safety
                var getHseDesignData = (from c in _context.HSE_DESIGNS_SAFETies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseDesignData = getHseDesignData != null ? getHseDesignData : HseDesignData;
                HseDesignData = _mapper.Map<HSE_DESIGNS_SAFETY>(wkp);

                HseDesignData.Companyemail = WKPCompanyEmail;
                HseDesignData.CompanyName = WKPCompanyName;
                HseDesignData.COMPANY_ID = WKPCompanyId;
                HseDesignData.Date_Updated = DateTime.Now;
                HseDesignData.Updated_by = WKPCompanyId;
                HseDesignData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseDesignData == null)
                {
                    HseDesignData.Created_by = WKPCompanyId;
                    HseDesignData.Date_Created = DateTime.Now;
                    await _context.HSE_DESIGNS_SAFETies.AddAsync(getHseDesignData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_DESIGNS_SAFETies.Remove(getHseDesignData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_SAFETY_STUDIES_NEW")]
        public async Task<WebApiResponse> HSE_SAFETY_STUDIES_NEW(HSE_SAFETY_STUDIES_NEW_Model wkp, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var HseSafetyData = new HSE_SAFETY_STUDIES_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Hse Safety Studies
                var getHseSafetyData = (from c in _context.HSE_SAFETY_STUDIES_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseSafetyData = getHseSafetyData != null ? getHseSafetyData : HseSafetyData;
                HseSafetyData = _mapper.Map<HSE_SAFETY_STUDIES_NEW>(wkp);

                #region file section
                UploadedDocument SMSFileUploadPath_File = null;

                if (files[0] != null)
                {
                    string docName = "SMS File";
                    SMSFileUploadPath_File = _helpersController.UploadDocument(files[0], "SMSDocuments");
                    if (SMSFileUploadPath_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }
                #endregion

                HseSafetyData.SMSFileUploadPath = SMSFileUploadPath_File != null ? SMSFileUploadPath_File.filePath : null;
                HseSafetyData.Companyemail = WKPCompanyEmail;
                HseSafetyData.CompanyName = WKPCompanyName;
                HseSafetyData.COMPANY_ID = WKPCompanyId;
                HseSafetyData.Date_Updated = DateTime.Now;
                HseSafetyData.Updated_by = WKPCompanyId;
                HseSafetyData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseSafetyData == null)
                {
                    HseSafetyData.Created_by = WKPCompanyId;
                    HseSafetyData.Date_Created = DateTime.Now;
                    await _context.HSE_SAFETY_STUDIES_NEWs.AddAsync(getHseSafetyData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_SAFETY_STUDIES_NEWs.Remove(getHseSafetyData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_INSPECTION_AND_MAINTENANCE_NEW")]
        public async Task<WebApiResponse> HSE_INSPECTION_AND_MAINTENANCE_NEW(HSE_INSPECTION_AND_MAINTENANCE_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseInspectionMaintenanceData = new HSE_INSPECTION_AND_MAINTENANCE_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Hse Inspection Maintenance
                var getHseInspectionMaintenanceData = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseInspectionMaintenanceData = getHseInspectionMaintenanceData != null ? getHseInspectionMaintenanceData : HseInspectionMaintenanceData;
                HseInspectionMaintenanceData = _mapper.Map<HSE_INSPECTION_AND_MAINTENANCE_NEW>(wkp);

                HseInspectionMaintenanceData.Companyemail = WKPCompanyEmail;
                HseInspectionMaintenanceData.CompanyName = WKPCompanyName;
                HseInspectionMaintenanceData.COMPANY_ID = WKPCompanyId;
                HseInspectionMaintenanceData.Date_Updated = DateTime.Now;
                HseInspectionMaintenanceData.Updated_by = WKPCompanyId;
                HseInspectionMaintenanceData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseInspectionMaintenanceData == null)
                {
                    HseInspectionMaintenanceData.Created_by = WKPCompanyId;
                    HseInspectionMaintenanceData.Date_Created = DateTime.Now;
                    await _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs.AddAsync(getHseInspectionMaintenanceData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs.Remove(getHseInspectionMaintenanceData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW")]
        public async Task<WebApiResponse> HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW(HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseInspectionMaintenanceFacData = new HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Hse Inspection Maintenance and Facility
                var getHseInspectionMaintenanceFacData = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseInspectionMaintenanceFacData = getHseInspectionMaintenanceFacData != null ? getHseInspectionMaintenanceFacData : HseInspectionMaintenanceFacData;
                HseInspectionMaintenanceFacData = _mapper.Map<HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW>(wkp);

                HseInspectionMaintenanceFacData.Companyemail = WKPCompanyEmail;
                HseInspectionMaintenanceFacData.CompanyName = WKPCompanyName;
                HseInspectionMaintenanceFacData.COMPANY_ID = WKPCompanyId;
                HseInspectionMaintenanceFacData.Date_Updated = DateTime.Now;
                HseInspectionMaintenanceFacData.Updated_by = WKPCompanyId;
                HseInspectionMaintenanceFacData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseInspectionMaintenanceFacData == null)
                {
                    HseInspectionMaintenanceFacData.Created_by = WKPCompanyId;
                    HseInspectionMaintenanceFacData.Date_Created = DateTime.Now;
                    await _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.AddAsync(getHseInspectionMaintenanceFacData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.Remove(getHseInspectionMaintenanceFacData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW")]
        public async Task<WebApiResponse> HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW(HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseTechnicalData = new HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Hse Technical Safety
                var getHseTechnicalData = (from c in _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseTechnicalData = getHseTechnicalData != null ? getHseTechnicalData : HseTechnicalData;
                HseTechnicalData = _mapper.Map<HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW>(wkp);

                HseTechnicalData.Companyemail = WKPCompanyEmail;
                HseTechnicalData.CompanyName = WKPCompanyName;
                HseTechnicalData.COMPANY_ID = WKPCompanyId;
                HseTechnicalData.Date_Updated = DateTime.Now;
                HseTechnicalData.Updated_by = WKPCompanyId;
                HseTechnicalData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseTechnicalData == null)
                {
                    HseTechnicalData.Created_by = WKPCompanyId;
                    HseTechnicalData.Date_Created = DateTime.Now;
                    await _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.AddAsync(getHseTechnicalData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.Remove(getHseTechnicalData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW")]
        public async Task<WebApiResponse> HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW(HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseAssetData = new HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Hse Asset Register
                var getHseAssetData = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseAssetData = getHseAssetData != null ? getHseAssetData : HseAssetData;
                HseAssetData = _mapper.Map<HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW>(wkp);

                HseAssetData.Companyemail = WKPCompanyEmail;
                HseAssetData.CompanyName = WKPCompanyName;
                HseAssetData.COMPANY_ID = WKPCompanyId;
                HseAssetData.Date_Updated = DateTime.Now;
                HseAssetData.Updated_by = WKPCompanyId;
                HseAssetData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseAssetData == null)
                {
                    HseAssetData.Created_by = WKPCompanyId;
                    HseAssetData.Date_Created = DateTime.Now;
                    await _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs.AddAsync(getHseAssetData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs.Remove(getHseAssetData);
                }
                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_OIL_SPILL_REPORTING_NEW")]
        public async Task<WebApiResponse> HSE_OIL_SPILL_REPORTING_NEW(HSE_OIL_SPILL_REPORTING_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseOilData = new HSE_OIL_SPILL_REPORTING_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Oil Spill
                var getHseOilData = (from c in _context.HSE_OIL_SPILL_REPORTING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseOilData = getHseOilData != null ? getHseOilData : HseOilData;
                HseOilData = _mapper.Map<HSE_OIL_SPILL_REPORTING_NEW>(wkp);

                HseOilData.Companyemail = WKPCompanyEmail;
                HseOilData.CompanyName = WKPCompanyName;
                HseOilData.COMPANY_ID = WKPCompanyId;
                HseOilData.Date_Updated = DateTime.Now;
                HseOilData.Updated_by = WKPCompanyId;
                HseOilData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseOilData == null)
                {
                    HseOilData.Created_by = WKPCompanyId;
                    HseOilData.Date_Created = DateTime.Now;
                    await _context.HSE_OIL_SPILL_REPORTING_NEWs.AddAsync(getHseOilData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_OIL_SPILL_REPORTING_NEWs.Remove(getHseOilData);
                }
                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW")]
        public async Task<WebApiResponse> HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW(HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseAssetRegisterData = new HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Asset Register RBI
                var getHseAssetRegisterData = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseAssetRegisterData = getHseAssetRegisterData != null ? getHseAssetRegisterData : HseAssetRegisterData;
                HseAssetRegisterData = _mapper.Map<HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW>(wkp);

                HseAssetRegisterData.Companyemail = WKPCompanyEmail;
                HseAssetRegisterData.CompanyName = WKPCompanyName;
                HseAssetRegisterData.COMPANY_ID = WKPCompanyId;
                HseAssetRegisterData.Date_Updated = DateTime.Now;
                HseAssetRegisterData.Updated_by = WKPCompanyId;
                HseAssetRegisterData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseAssetRegisterData == null)
                {
                    HseAssetRegisterData.Created_by = WKPCompanyId;
                    HseAssetRegisterData.Date_Created = DateTime.Now;
                    await _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs.AddAsync(getHseAssetRegisterData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs.Remove(getHseAssetRegisterData);
                }

                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_ACCIDENT_INCIDENCE_REPORTING_NEW")]
        public async Task<WebApiResponse> HSE_ACCIDENT_INCIDENCE_REPORTING_NEW(HSE_ACCIDENT_INCIDENCE_REPORTING_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseAccidentData = new HSE_ACCIDENT_INCIDENCE_REPORTING_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Incidence Reporting
                var getHseAccidentData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseAccidentData = getHseAccidentData != null ? getHseAccidentData : HseAccidentData;
                HseAccidentData = _mapper.Map<HSE_ACCIDENT_INCIDENCE_REPORTING_NEW>(wkp);

                HseAccidentData.Companyemail = WKPCompanyEmail;
                HseAccidentData.CompanyName = WKPCompanyName;
                HseAccidentData.COMPANY_ID = WKPCompanyId;
                HseAccidentData.Date_Updated = DateTime.Now;
                HseAccidentData.Updated_by = WKPCompanyId;
                HseAccidentData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseAccidentData == null)
                {
                    HseAccidentData.Created_by = WKPCompanyId;
                    HseAccidentData.Date_Created = DateTime.Now;
                    await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.AddAsync(getHseAccidentData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.Remove(getHseAccidentData);
                }
                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW")]
        public async Task<WebApiResponse> HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW(HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseAccidentIncidenceData = new HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Incidence Reporting Type
                var getHseAccidentIncidenceData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseAccidentIncidenceData = getHseAccidentIncidenceData != null ? getHseAccidentIncidenceData : HseAccidentIncidenceData;
                HseAccidentIncidenceData = _mapper.Map<HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW>(wkp);

                HseAccidentIncidenceData.Companyemail = WKPCompanyEmail;
                HseAccidentIncidenceData.CompanyName = WKPCompanyName;
                HseAccidentIncidenceData.COMPANY_ID = WKPCompanyId;
                HseAccidentIncidenceData.Date_Updated = DateTime.Now;
                HseAccidentIncidenceData.Updated_by = WKPCompanyId;
                HseAccidentIncidenceData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseAccidentIncidenceData == null)
                {
                    HseAccidentIncidenceData.Created_by = WKPCompanyId;
                    HseAccidentIncidenceData.Date_Created = DateTime.Now;
                    await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.AddAsync(getHseAccidentIncidenceData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.Remove(getHseAccidentIncidenceData);
                }
                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW")]
        public async Task<WebApiResponse> HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW(HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseCommunityDisturbancesData = new HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Community Disturbances
                var getHseCommunityDisturbancesData = (from c in _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseCommunityDisturbancesData = getHseCommunityDisturbancesData != null ? getHseCommunityDisturbancesData : HseCommunityDisturbancesData;
                HseCommunityDisturbancesData = _mapper.Map<HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW>(wkp);

                HseCommunityDisturbancesData.Companyemail = WKPCompanyEmail;
                HseCommunityDisturbancesData.CompanyName = WKPCompanyName;
                HseCommunityDisturbancesData.COMPANY_ID = WKPCompanyId;
                HseCommunityDisturbancesData.Date_Updated = DateTime.Now;
                HseCommunityDisturbancesData.Updated_by = WKPCompanyId;
                HseCommunityDisturbancesData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseCommunityDisturbancesData == null)
                {
                    HseCommunityDisturbancesData.Created_by = WKPCompanyId;
                    HseCommunityDisturbancesData.Date_Created = DateTime.Now;
                    await _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs.AddAsync(getHseCommunityDisturbancesData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs.Remove(getHseCommunityDisturbancesData);
                }
                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_ENVIRONMENTAL_STUDIES_NEW")]
        public async Task<WebApiResponse> HSE_ENVIRONMENTAL_STUDIES_NEW(HSE_ENVIRONMENTAL_STUDIES_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseEnvironmentalData = new HSE_ENVIRONMENTAL_STUDIES_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Environmental Studies
                var getHseEnvironmentalData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseEnvironmentalData = getHseEnvironmentalData != null ? getHseEnvironmentalData : HseEnvironmentalData;
                HseEnvironmentalData = _mapper.Map<HSE_ENVIRONMENTAL_STUDIES_NEW>(wkp);

                HseEnvironmentalData.Companyemail = WKPCompanyEmail;
                HseEnvironmentalData.CompanyName = WKPCompanyName;
                HseEnvironmentalData.COMPANY_ID = WKPCompanyId;
                HseEnvironmentalData.Date_Updated = DateTime.Now;
                HseEnvironmentalData.Updated_by = WKPCompanyId;
                HseEnvironmentalData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseEnvironmentalData == null)
                {
                    HseEnvironmentalData.Created_by = WKPCompanyId;
                    HseEnvironmentalData.Date_Created = DateTime.Now;
                    await _context.HSE_ENVIRONMENTAL_STUDIES_NEWs.AddAsync(getHseEnvironmentalData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_ENVIRONMENTAL_STUDIES_NEWs.Remove(getHseEnvironmentalData);
                }

                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_WASTE_MANAGEMENT_NEW")]
        public async Task<WebApiResponse> HSE_WASTE_MANAGEMENT_NEW(HSE_WASTE_MANAGEMENT_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseWasteData = new HSE_WASTE_MANAGEMENT_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Waste Management
                var getHseWasteData = (from c in _context.HSE_WASTE_MANAGEMENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseWasteData = getHseWasteData != null ? getHseWasteData : HseWasteData;
                HseWasteData = _mapper.Map<HSE_WASTE_MANAGEMENT_NEW>(wkp);

                HseWasteData.Companyemail = WKPCompanyEmail;
                HseWasteData.CompanyName = WKPCompanyName;
                HseWasteData.COMPANY_ID = WKPCompanyId;
                HseWasteData.Date_Updated = DateTime.Now;
                HseWasteData.Updated_by = WKPCompanyId;
                HseWasteData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseWasteData == null)
                {
                    HseWasteData.Created_by = WKPCompanyId;
                    HseWasteData.Date_Created = DateTime.Now;
                    await _context.HSE_WASTE_MANAGEMENT_NEWs.AddAsync(getHseWasteData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_WASTE_MANAGEMENT_NEWs.Remove(getHseWasteData);
                }
                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW")]
        public async Task<WebApiResponse> HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW(HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseWasteMgtData = new HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Waste Management Facility
                var getHseWasteMgtData = (from c in _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseWasteMgtData = getHseWasteMgtData != null ? getHseWasteMgtData : HseWasteMgtData;
                HseWasteMgtData = _mapper.Map<HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW>(wkp);

                HseWasteMgtData.Companyemail = WKPCompanyEmail;
                HseWasteMgtData.CompanyName = WKPCompanyName;
                HseWasteMgtData.COMPANY_ID = WKPCompanyId;
                HseWasteMgtData.Date_Updated = DateTime.Now;
                HseWasteMgtData.Updated_by = WKPCompanyId;
                HseWasteMgtData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseWasteMgtData == null)
                {
                    HseWasteMgtData.Created_by = WKPCompanyId;
                    HseWasteMgtData.Date_Created = DateTime.Now;
                    await _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs.AddAsync(getHseWasteMgtData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs.Remove(getHseWasteMgtData);
                }
                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_PRODUCED_WATER_MANAGEMENT_NEW")]
        public async Task<WebApiResponse> HSE_PRODUCED_WATER_MANAGEMENT_NEW(HSE_PRODUCED_WATER_MANAGEMENT_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseWaterMgtData = new HSE_PRODUCED_WATER_MANAGEMENT_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Produced Water
                var getHseWaterMgtData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseWaterMgtData = getHseWaterMgtData != null ? getHseWaterMgtData : HseWaterMgtData;
                HseWaterMgtData = _mapper.Map<HSE_PRODUCED_WATER_MANAGEMENT_NEW>(wkp);

                HseWaterMgtData.Companyemail = WKPCompanyEmail;
                HseWaterMgtData.CompanyName = WKPCompanyName;
                HseWaterMgtData.COMPANY_ID = WKPCompanyId;
                HseWaterMgtData.Date_Updated = DateTime.Now;
                HseWaterMgtData.Updated_by = WKPCompanyId;
                HseWaterMgtData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseWaterMgtData == null)
                {
                    HseWaterMgtData.Created_by = WKPCompanyId;
                    HseWaterMgtData.Date_Created = DateTime.Now;
                    await _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs.AddAsync(getHseWaterMgtData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs.Remove(getHseWaterMgtData);
                }

                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW")]
        public async Task<WebApiResponse> HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW(HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseEnvironmentalComplianceData = new HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Environmental Compliance
                var getHseEnvironmentalComplianceData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseEnvironmentalComplianceData = getHseEnvironmentalComplianceData != null ? getHseEnvironmentalComplianceData : HseEnvironmentalComplianceData;
                HseEnvironmentalComplianceData = _mapper.Map<HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW>(wkp);

                HseEnvironmentalComplianceData.Companyemail = WKPCompanyEmail;
                HseEnvironmentalComplianceData.CompanyName = WKPCompanyName;
                HseEnvironmentalComplianceData.COMPANY_ID = WKPCompanyId;
                HseEnvironmentalComplianceData.Date_Updated = DateTime.Now;
                HseEnvironmentalComplianceData.Updated_by = WKPCompanyId;
                HseEnvironmentalComplianceData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseEnvironmentalComplianceData == null)
                {
                    HseEnvironmentalComplianceData.Created_by = WKPCompanyId;
                    HseEnvironmentalComplianceData.Date_Created = DateTime.Now;
                    await _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs.AddAsync(getHseEnvironmentalComplianceData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs.Remove(getHseEnvironmentalComplianceData);
                }

                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW")]
        public async Task<WebApiResponse> HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW(HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseEnvironmentalStudiesData = new HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Environmental Studies Five
                var getHseEnvironmentalStudiesData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseEnvironmentalStudiesData = getHseEnvironmentalStudiesData != null ? getHseEnvironmentalStudiesData : HseEnvironmentalStudiesData;
                HseEnvironmentalStudiesData = _mapper.Map<HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW>(wkp);

                HseEnvironmentalStudiesData.Companyemail = WKPCompanyEmail;
                HseEnvironmentalStudiesData.CompanyName = WKPCompanyName;
                HseEnvironmentalStudiesData.COMPANY_ID = WKPCompanyId;
                HseEnvironmentalStudiesData.Date_Updated = DateTime.Now;
                HseEnvironmentalStudiesData.Updated_by = WKPCompanyId;
                HseEnvironmentalStudiesData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseEnvironmentalStudiesData == null)
                {
                    HseEnvironmentalStudiesData.Created_by = WKPCompanyId;
                    HseEnvironmentalStudiesData.Date_Created = DateTime.Now;
                    await _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs.AddAsync(getHseEnvironmentalStudiesData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs.Remove(getHseEnvironmentalStudiesData);
                }

                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL")]
        public async Task<WebApiResponse> HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL(HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseSustainableData = new HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Sustainable Development
                var getHseSustainableData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseSustainableData = getHseSustainableData != null ? getHseSustainableData : HseSustainableData;
                HseSustainableData = _mapper.Map<HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL>(wkp);

                HseSustainableData.Companyemail = WKPCompanyEmail;
                HseSustainableData.CompanyName = WKPCompanyName;
                HseSustainableData.COMPANY_ID = WKPCompanyId;
                HseSustainableData.Date_Updated = DateTime.Now;
                HseSustainableData.Updated_by = WKPCompanyId;
                HseSustainableData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseSustainableData == null)
                {
                    HseSustainableData.Created_by = WKPCompanyId;
                    HseSustainableData.Date_Created = DateTime.Now;
                    await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs.AddAsync(HseSustainableData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs.Remove(HseSustainableData);
                }

                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED")]
        public async Task<WebApiResponse> HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED(HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseEnvironmentalStudiesUpdatedData = new HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Environmental Studies
                var getHseEnvironmentalStudiesUpdatedData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseEnvironmentalStudiesUpdatedData = getHseEnvironmentalStudiesUpdatedData != null ? getHseEnvironmentalStudiesUpdatedData : HseEnvironmentalStudiesUpdatedData;
                HseEnvironmentalStudiesUpdatedData = _mapper.Map<HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED>(wkp);

                HseEnvironmentalStudiesUpdatedData.Companyemail = WKPCompanyEmail;
                HseEnvironmentalStudiesUpdatedData.CompanyName = WKPCompanyName;
                HseEnvironmentalStudiesUpdatedData.COMPANY_ID = WKPCompanyId;
                HseEnvironmentalStudiesUpdatedData.Date_Updated = DateTime.Now;
                HseEnvironmentalStudiesUpdatedData.Updated_by = WKPCompanyId;
                HseEnvironmentalStudiesUpdatedData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseEnvironmentalStudiesUpdatedData == null)
                {
                    HseEnvironmentalStudiesUpdatedData.Created_by = WKPCompanyId;
                    HseEnvironmentalStudiesUpdatedData.Date_Created = DateTime.Now;
                    await _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs.AddAsync(getHseEnvironmentalStudiesUpdatedData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs.Remove(getHseEnvironmentalStudiesUpdatedData);
                }
                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_OSP_REGISTRATIONS_NEW")]
        public async Task<WebApiResponse> HSE_OSP_REGISTRATIONS_NEW(HSE_OSP_REGISTRATIONS_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseOSPData = new HSE_OSP_REGISTRATIONS_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse OSP Registrations
                var getHseOSPData = (from c in _context.HSE_OSP_REGISTRATIONS_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseOSPData = getHseOSPData != null ? getHseOSPData : HseOSPData;
                HseOSPData = _mapper.Map<HSE_OSP_REGISTRATIONS_NEW>(wkp);

                HseOSPData.Companyemail = WKPCompanyEmail;
                HseOSPData.CompanyName = WKPCompanyName;
                HseOSPData.COMPANY_ID = WKPCompanyId;
                HseOSPData.Date_Updated = DateTime.Now;
                HseOSPData.Updated_by = WKPCompanyId;
                HseOSPData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseOSPData == null)
                {
                    HseOSPData.Created_by = WKPCompanyId;
                    HseOSPData.Date_Created = DateTime.Now;
                    await _context.HSE_OSP_REGISTRATIONS_NEWs.AddAsync(getHseOSPData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_OSP_REGISTRATIONS_NEWs.Remove(getHseOSPData);
                }

                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED")]
        public async Task<WebApiResponse> HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED(HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseProducedWaterData = new HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Produced Water
                var getHseProducedWaterData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();
                HseProducedWaterData = getHseProducedWaterData != null ? getHseProducedWaterData : HseProducedWaterData;
                HseProducedWaterData = _mapper.Map<HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED>(wkp);

                HseProducedWaterData.Companyemail = WKPCompanyEmail;
                HseProducedWaterData.CompanyName = WKPCompanyName;
                HseProducedWaterData.COMPANY_ID = WKPCompanyId;
                HseProducedWaterData.Date_Updated = DateTime.Now;
                HseProducedWaterData.Updated_by = WKPCompanyId;
                HseProducedWaterData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseProducedWaterData == null)
                {
                    HseProducedWaterData.Created_by = WKPCompanyId;
                    HseProducedWaterData.Date_Created = DateTime.Now;
                    await _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.AddAsync(getHseProducedWaterData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.Remove(getHseProducedWaterData);
                }

                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW")]
        public async Task<WebApiResponse> HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW(HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseEnvironmentalMonitoringData = new HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Environmental Monitoring 
                var getHseEnvironmentalMonitoringData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();
                HseEnvironmentalMonitoringData = getHseEnvironmentalMonitoringData != null ? getHseEnvironmentalMonitoringData : HseEnvironmentalMonitoringData;
                HseEnvironmentalMonitoringData = _mapper.Map<HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW>(wkp);

                HseEnvironmentalMonitoringData.Companyemail = WKPCompanyEmail;
                HseEnvironmentalMonitoringData.CompanyName = WKPCompanyName;
                HseEnvironmentalMonitoringData.COMPANY_ID = WKPCompanyId;
                HseEnvironmentalMonitoringData.Date_Updated = DateTime.Now;
                HseEnvironmentalMonitoringData.Updated_by = WKPCompanyId;
                HseEnvironmentalMonitoringData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseEnvironmentalMonitoringData == null)
                {
                    HseEnvironmentalMonitoringData.Created_by = WKPCompanyId;
                    HseEnvironmentalMonitoringData.Date_Created = DateTime.Now;
                    await _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs.AddAsync(getHseEnvironmentalMonitoringData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs.Remove(getHseEnvironmentalMonitoringData);
                }
                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_CAUSES_OF_SPILL")]
        public async Task<WebApiResponse> HSE_CAUSES_OF_SPILL(HSE_CAUSES_OF_SPILL_Model wkp, string WorkProgramme_Year, string ActionToDo = null)
        {

            int save = 0;
            var HseCausesData = new HSE_CAUSES_OF_SPILL();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Causes of Spill
                var getHseCausesData = (from c in _context.HSE_CAUSES_OF_SPILLs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();
                HseCausesData = getHseCausesData != null ? getHseCausesData : HseCausesData;
                HseCausesData = _mapper.Map<HSE_CAUSES_OF_SPILL>(wkp);

                HseCausesData.Companyemail = WKPCompanyEmail;
                HseCausesData.CompanyName = WKPCompanyName;
                HseCausesData.COMPANY_ID = WKPCompanyId;
                HseCausesData.Date_Updated = DateTime.Now;
                HseCausesData.Updated_by = WKPCompanyId;
                HseCausesData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseCausesData == null)
                {
                    HseCausesData.Created_by = WKPCompanyId;
                    HseCausesData.Date_Created = DateTime.Now;
                    await _context.HSE_CAUSES_OF_SPILLs.AddAsync(getHseCausesData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_CAUSES_OF_SPILLs.Remove(getHseCausesData);
                }

                save += await _context.SaveChangesAsync();
                #endregion
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU")]
        public async Task<WebApiResponse> HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU(HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU_Model wkp, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var HseSustainableDevData = new HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Hse Sustainable Dev Community
                var getHseSustainableDevData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();
                HseSustainableDevData = getHseSustainableDevData != null ? getHseSustainableDevData : HseSustainableDevData;
                HseSustainableDevData = _mapper.Map<HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU>(wkp);
                #region file section
                UploadedDocument MOUUploadFile_File = null;

                if (files[0] != null)
                {
                    string docName = "MOUU";
                    MOUUploadFile_File = _helpersController.UploadDocument(files[0], "MOUDocuments");
                    if (MOUUploadFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }
                #endregion

                HseSustainableDevData.MOUUploadFilePath = files[0] != null ? MOUUploadFile_File.filePath : null; ;
                HseSustainableDevData.MOUUploadFilename = files[0] != null ? MOUUploadFile_File.fileName : null; ;
                HseSustainableDevData.Companyemail = WKPCompanyEmail;
                HseSustainableDevData.CompanyName = WKPCompanyName;
                HseSustainableDevData.COMPANY_ID = WKPCompanyId;
                HseSustainableDevData.Date_Updated = DateTime.Now;
                HseSustainableDevData.Updated_by = WKPCompanyId;
                HseSustainableDevData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseSustainableDevData == null)
                {
                    HseSustainableDevData.Created_by = WKPCompanyId;
                    HseSustainableDevData.Date_Created = DateTime.Now;
                    await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs.AddAsync(getHseSustainableDevData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs.Remove(getHseSustainableDevData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME")]
        public async Task<WebApiResponse> HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME(HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME_Model wkp, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var HseSustainableComData = new HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Hse Sustainable Dev Schemes
                var getHseSustainableComData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();
                HseSustainableComData = getHseSustainableComData != null ? getHseSustainableComData : HseSustainableComData;
                HseSustainableComData = _mapper.Map<HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME>(wkp);
                #region file section
                UploadedDocument SSUploadFile_File = null;

                if (files[0] != null)
                {
                    string docName = "SS";
                    SSUploadFile_File = _helpersController.UploadDocument(files[0], "SSDocuments");
                    if (SSUploadFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }
                #endregion
                HseSustainableComData.SSUploadFilePath = files[0] != null ? SSUploadFile_File.filePath : null;
                HseSustainableComData.SSUploadFilename = files[0] != null ? SSUploadFile_File.fileName : null;
                HseSustainableComData.Companyemail = WKPCompanyEmail;
                HseSustainableComData.CompanyName = WKPCompanyName;
                HseSustainableComData.COMPANY_ID = WKPCompanyId;
                HseSustainableComData.Date_Updated = DateTime.Now;
                HseSustainableComData.Updated_by = WKPCompanyId;
                HseSustainableComData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseSustainableComData == null)
                {
                    HseSustainableComData.Created_by = WKPCompanyId;
                    HseSustainableComData.Date_Created = DateTime.Now;
                    await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs.AddAsync(getHseSustainableComData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs.Remove(getHseSustainableComData);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_MANAGEMENT_POSITION")]
        public async Task<WebApiResponse> HSE_MANAGEMENT_POSITION(HSE_MANAGEMENT_POSITION_Model wkp, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var HseManagmentData = new HSE_MANAGEMENT_POSITION();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Management position
                var getHseManagmentData = (from c in _context.HSE_MANAGEMENT_POSITIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseManagmentData = getHseManagmentData != null ? getHseManagmentData : HseManagmentData;
                HseManagmentData = _mapper.Map<HSE_MANAGEMENT_POSITION>(wkp);
                #region file section
                UploadedDocument PromotionLetterFile_File = null;
                UploadedDocument OrganogramFile_File = null;

                if (files[0] != null)
                {
                    string docName = "Promotion Letter";
                    PromotionLetterFile_File = _helpersController.UploadDocument(files[0], "HRDocuments");
                    if (PromotionLetterFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }
                if (files[1] != null)
                {
                    string docName = "Organogram";
                    OrganogramFile_File = _helpersController.UploadDocument(files[1], "OGDocuments");
                    if (OrganogramFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }
                #endregion

                HseManagmentData.PromotionLetterFilePath = files[0] != null ? PromotionLetterFile_File.filePath : null;
                HseManagmentData.PromotionLetterFilename = files[0] != null ? PromotionLetterFile_File.fileName : null;
                HseManagmentData.OrganogramrFilePath = files[1] != null ? OrganogramFile_File.filePath : null;
                HseManagmentData.OrganogramrFilename = files[1] != null ? OrganogramFile_File.fileName : null;
                HseManagmentData.Companyemail = WKPCompanyEmail;
                HseManagmentData.CompanyName = WKPCompanyName;
                HseManagmentData.COMPANY_ID = WKPCompanyId;
                HseManagmentData.Date_Updated = DateTime.Now;
                HseManagmentData.Updated_by = WKPCompanyId;
                HseManagmentData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseManagmentData == null)
                {
                    HseManagmentData.Created_by = WKPCompanyId;
                    HseManagmentData.Date_Created = DateTime.Now;
                    await _context.HSE_MANAGEMENT_POSITIONs.AddAsync(getHseManagmentData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_MANAGEMENT_POSITIONs.Remove(getHseManagmentData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_QUALITY_CONTROL")]
        public async Task<WebApiResponse> HSE_QUALITY_CONTROL(HSE_QUALITY_CONTROL_Model wkp, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var HseQualityData = new HSE_QUALITY_CONTROL();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Quality
                var getHseQualityData = (from c in _context.HSE_QUALITY_CONTROLs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();

                HseQualityData = getHseQualityData != null ? getHseQualityData : HseQualityData;
                HseQualityData = _mapper.Map<HSE_QUALITY_CONTROL>(wkp);
                #region file section
                UploadedDocument QualityControlFile_File = null;

                if (files[0] != null)
                {
                    string docName = "Quality Control";
                    QualityControlFile_File = _helpersController.UploadDocument(files[0], "COSDocuments");
                    if (QualityControlFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }

                #endregion

                HseQualityData.QualityControlFilename = files[0] != null ? QualityControlFile_File.fileName : null;
                HseQualityData.QualityControlFilePath = files[0] != null ? QualityControlFile_File.filePath : null;
                HseQualityData.Companyemail = WKPCompanyEmail;
                HseQualityData.CompanyName = WKPCompanyName;
                HseQualityData.COMPANY_ID = WKPCompanyId;
                HseQualityData.Date_Updated = DateTime.Now;
                HseQualityData.Updated_by = WKPCompanyId;
                HseQualityData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseQualityData == null)
                {
                    HseQualityData.Created_by = WKPCompanyId;
                    HseQualityData.Date_Created = DateTime.Now;
                    await _context.HSE_QUALITY_CONTROLs.AddAsync(getHseQualityData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_QUALITY_CONTROLs.Remove(getHseQualityData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_CLIMATE_CHANGE_AND_AIR_QUALITY")]
        public async Task<WebApiResponse> HSE_CLIMATE_CHANGE_AND_AIR_QUALITY(HSE_CLIMATE_CHANGE_AND_AIR_QUALITY_Model wkp, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var HseClimateData = new HSE_CLIMATE_CHANGE_AND_AIR_QUALITY();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Climate
                var getHseClimateData = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();
                HseClimateData = getHseClimateData != null ? getHseClimateData : HseClimateData;
                HseClimateData = _mapper.Map<HSE_CLIMATE_CHANGE_AND_AIR_QUALITY>(wkp);
                #region file section
                UploadedDocument GHGFile_File = null;

                if (files[0] != null)
                {
                    string docName = "GHG";
                    GHGFile_File = _helpersController.UploadDocument(files[0], "GHGDocuments");
                    if (GHGFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }

                #endregion

                HseClimateData.GHGFilename = files[0] != null ? GHGFile_File.fileName : null;
                HseClimateData.GHGFilePath = files[0] != null ? GHGFile_File.filePath : null;
                HseClimateData.Companyemail = WKPCompanyEmail;
                HseClimateData.CompanyName = WKPCompanyName;
                HseClimateData.COMPANY_ID = WKPCompanyId;
                HseClimateData.Date_Updated = DateTime.Now;
                HseClimateData.Updated_by = WKPCompanyId;
                HseClimateData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getHseClimateData == null)
                {
                    HseClimateData.Created_by = WKPCompanyId;
                    HseClimateData.Date_Created = DateTime.Now;
                    await _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.AddAsync(getHseClimateData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.Remove(getHseClimateData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_SAFETY_CULTURE_TRAINING")]
        public async Task<WebApiResponse> HSE_SAFETY_CULTURE_TRAINING(HSE_SAFETY_CULTURE_TRAINING_Model wkp, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var HseSafetyCultureData = new HSE_SAFETY_CULTURE_TRAINING();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Safety Culture
                var getHseSafetyCultureData = (from c in _context.HSE_SAFETY_CULTURE_TRAININGs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();
                HseSafetyCultureData = getHseSafetyCultureData != null ? getHseSafetyCultureData : HseSafetyCultureData;
                HseSafetyCultureData = _mapper.Map<HSE_SAFETY_CULTURE_TRAINING>(wkp);
                #region file section
                UploadedDocument SafetyCurrentYearFile_File = null;
                UploadedDocument SafetyLast2YearsFile_File = null;
                if (files[0] != null)
                {
                    string docName = "Safety Current Year File";
                    SafetyCurrentYearFile_File = _helpersController.UploadDocument(files[0], "SafetyCurrentYearDocuments");
                    if (SafetyCurrentYearFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }

                if (files[1] != null)
                {
                    string docName = "Safety Last Two Years";
                    SafetyLast2YearsFile_File = _helpersController.UploadDocument(files[1], "SafetyLast2YearsDocuments");
                    if (SafetyLast2YearsFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }

                #endregion


                HseSafetyCultureData.SafetyCurrentYearFilename = files[0] != null ? SafetyCurrentYearFile_File.fileName : null;
                HseSafetyCultureData.SafetyCurrentYearFilePath = files[0] != null ? SafetyCurrentYearFile_File.filePath : null;
                HseSafetyCultureData.SafetyLast2YearsFilename = files[1] != null ? SafetyLast2YearsFile_File.fileName : null;
                HseSafetyCultureData.SafetyLast2YearsFilePath = files[1] != null ? SafetyLast2YearsFile_File.filePath : null;
                HseSafetyCultureData.Companyemail = WKPCompanyEmail;
                HseSafetyCultureData.CompanyName = WKPCompanyName;
                HseSafetyCultureData.COMPANY_ID = WKPCompanyId;
                HseSafetyCultureData.Date_Updated = DateTime.Now;
                HseSafetyCultureData.Updated_by = WKPCompanyId;
                HseSafetyCultureData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getHseSafetyCultureData == null)
                {
                    HseSafetyCultureData.Created_by = WKPCompanyId;
                    HseSafetyCultureData.Date_Created = DateTime.Now;
                    await _context.HSE_SAFETY_CULTURE_TRAININGs.AddAsync(getHseSafetyCultureData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_SAFETY_CULTURE_TRAININGs.Remove(getHseSafetyCultureData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_OCCUPATIONAL_HEALTH_MANAGEMENT")]
        public async Task<WebApiResponse> HSE_OCCUPATIONAL_HEALTH_MANAGEMENT(HSE_OCCUPATIONAL_HEALTH_MANAGEMENT_Model wkp, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var HseOccupationalData = new HSE_OCCUPATIONAL_HEALTH_MANAGEMENT();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Occupational 
                var getHseOccupationalData = (from c in _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();
                HseOccupationalData = getHseOccupationalData != null ? getHseOccupationalData : HseOccupationalData;
                HseOccupationalData = _mapper.Map<HSE_OCCUPATIONAL_HEALTH_MANAGEMENT>(wkp);

                #region file section
                UploadedDocument OHMplanFile_File = null;
                UploadedDocument OHMplanCommunicationFile_File = null;

                if (files[0] != null)
                {
                    string docName = "Field Discovery";
                    OHMplanFile_File = _helpersController.UploadDocument(files[0], "FieldDiscoveryDocuments");
                    if (OHMplanFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }
                if (files[1] != null)
                {
                    string docName = "Field Discovery";
                    OHMplanCommunicationFile_File = _helpersController.UploadDocument(files[1], "FieldDiscoveryDocuments");
                    if (OHMplanCommunicationFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }

                #endregion
                HseOccupationalData.OHMplanFilename = files[0] != null ? OHMplanFile_File.fileName : null;
                HseOccupationalData.OHMplanFilePath = files[0] != null ? OHMplanFile_File.filePath : null;
                HseOccupationalData.OHMplanCommunicationFilename = files[1] != null ? OHMplanCommunicationFile_File.fileName : null;
                HseOccupationalData.OHMplanCommunicationFilePath = files[1] != null ? OHMplanCommunicationFile_File.filePath : null;
                HseOccupationalData.Companyemail = WKPCompanyEmail;
                HseOccupationalData.CompanyName = WKPCompanyName;
                HseOccupationalData.COMPANY_ID = WKPCompanyId;
                HseOccupationalData.Date_Updated = DateTime.Now;
                HseOccupationalData.Updated_by = WKPCompanyId;
                HseOccupationalData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getHseOccupationalData == null)
                {
                    HseOccupationalData.Created_by = WKPCompanyId;
                    HseOccupationalData.Date_Created = DateTime.Now;
                    await _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.AddAsync(getHseOccupationalData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.Remove(getHseOccupationalData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_WASTE_MANAGEMENT_SYSTEM")]
        public async Task<WebApiResponse> HSE_WASTE_MANAGEMENT_SYSTEM(HSE_WASTE_MANAGEMENT_SYSTEM_Model wkp, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var HseWasteMgtSysData = new HSE_WASTE_MANAGEMENT_SYSTEM();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Waste Management
                var getHseWasteMgtSysData = (from c in _context.HSE_WASTE_MANAGEMENT_SYSTEMs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();
                HseWasteMgtSysData = getHseWasteMgtSysData != null ? getHseWasteMgtSysData : HseWasteMgtSysData;
                HseWasteMgtSysData = _mapper.Map<HSE_WASTE_MANAGEMENT_SYSTEM>(wkp);
                #region file section
                UploadedDocument DecomCertificateFile_File = null;
                UploadedDocument WasteManagementPlanFile_File = null;

                if (files[0] != null)
                {
                    string docName = "Decom Certificate";
                    DecomCertificateFile_File = _helpersController.UploadDocument(files[0], "DecomCertificateDocuments");
                    if (DecomCertificateFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }
                if (files[1] != null)
                {
                    string docName = "Waste Management Plan";
                    WasteManagementPlanFile_File = _helpersController.UploadDocument(files[1], "WasteManagementPlanDocuments");
                    if (WasteManagementPlanFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }
                #endregion

                HseWasteMgtSysData.DecomCertificateFilePath = files[0] != null ? DecomCertificateFile_File.filePath : null;
                HseWasteMgtSysData.DecomCertificateFilename = files[0] != null ? DecomCertificateFile_File.fileName : null;
                HseWasteMgtSysData.WasteManagementPlanFilePath = files[1] != null ? WasteManagementPlanFile_File.filePath : null;
                HseWasteMgtSysData.WasteManagementPlanFilename = files[1] != null ? WasteManagementPlanFile_File.fileName : null;
                HseWasteMgtSysData.Companyemail = WKPCompanyEmail;
                HseWasteMgtSysData.CompanyName = WKPCompanyName;
                HseWasteMgtSysData.COMPANY_ID = WKPCompanyId;
                HseWasteMgtSysData.Date_Updated = DateTime.Now;
                HseWasteMgtSysData.Updated_by = WKPCompanyId;
                HseWasteMgtSysData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getHseWasteMgtSysData == null)
                {
                    HseWasteMgtSysData.Created_by = WKPCompanyId;
                    HseWasteMgtSysData.Date_Created = DateTime.Now;
                    await _context.HSE_WASTE_MANAGEMENT_SYSTEMs.AddAsync(getHseWasteMgtSysData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_WASTE_MANAGEMENT_SYSTEMs.Remove(getHseWasteMgtSysData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM")]
        public async Task<WebApiResponse> HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM(HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM_Model wkp, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var HseEnvWasteData = new HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Hse Environmental Management
                var getHseEnvWasteData = (from c in _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();
                HseEnvWasteData = getHseEnvWasteData != null ? getHseEnvWasteData : HseEnvWasteData;
                HseEnvWasteData = _mapper.Map<HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM>(wkp);
                #region file section
                UploadedDocument EMSFile_File = null;
                UploadedDocument AUDITFile_File = null;

                if (files[0] != null)
                {
                    string docName = "EMS";
                    EMSFile_File = _helpersController.UploadDocument(files[0], "EMSDocuments");
                    if (EMSFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }
                if (files[1] != null)
                {
                    string docName = "Audit File";
                    AUDITFile_File = _helpersController.UploadDocument(files[1], "AUDITDocuments");
                    if (AUDITFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }
                #endregion


                HseEnvWasteData.EMSFilename = files[0] != null ? EMSFile_File.fileName : null;
                HseEnvWasteData.EMSFilePath = files[0] != null ? EMSFile_File.filePath : null;
                HseEnvWasteData.AUDITFilename = files[1] != null ? AUDITFile_File.fileName : null;
                HseEnvWasteData.AUDITFilePath = files[1] != null ? AUDITFile_File.filePath : null;
                HseEnvWasteData.Companyemail = WKPCompanyEmail;
                HseEnvWasteData.CompanyName = WKPCompanyName;
                HseEnvWasteData.COMPANY_ID = WKPCompanyId;
                HseEnvWasteData.Date_Updated = DateTime.Now;
                HseEnvWasteData.Updated_by = WKPCompanyId;
                HseEnvWasteData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getHseEnvWasteData == null)
                {
                    HseEnvWasteData.Created_by = WKPCompanyId;
                    HseEnvWasteData.Date_Created = DateTime.Now;
                    await _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs.AddAsync(getHseEnvWasteData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs.Remove(getHseEnvWasteData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT")]
        public async Task<WebApiResponse> PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT(PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT_Model wkp, string WorkProgramme_Year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            var PictureUploadData = new PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Picture Upload
                var getPictureUploadData = (from c in _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == WorkProgramme_Year select c).FirstOrDefault();
                PictureUploadData = getPictureUploadData != null ? getPictureUploadData : PictureUploadData;
                PictureUploadData = _mapper.Map<PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT>(wkp);
                #region file section
                UploadedDocument UploadedPresentation_File = null;

                if (files[0] != null)
                {
                    string docName = "Uploaded Presentation";
                    UploadedPresentation_File = _helpersController.UploadDocument(files[0], "Presentations");
                    if (UploadedPresentation_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                }
                #endregion
                PictureUploadData.uploaded_presentation = files[0] != null ? UploadedPresentation_File.filePath : null;
                PictureUploadData.Companyemail = WKPCompanyEmail;
                PictureUploadData.CompanyName = WKPCompanyName;
                PictureUploadData.COMPANY_ID = WKPCompanyId;
                PictureUploadData.Date_Updated = DateTime.Now;
                PictureUploadData.Updated_by = WKPCompanyId;
                PictureUploadData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getPictureUploadData == null)
                {
                    PictureUploadData.Created_by = WKPCompanyId;
                    PictureUploadData.Date_Created = DateTime.Now;
                    await _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs.AddAsync(getPictureUploadData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs.Remove(getPictureUploadData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }


        #endregion

        [HttpGet("PRESENTATION SCHEDULES")]
        public async Task<WebApiResponse> PRESENTATION_SCHEDULES(string year)
        {
            try
            {
                var schedules = (from sch in _context.ADMIN_DATETIME_PRESENTATIONs select sch).ToList();
                var viewYears = schedules.Select(x => x.YEAR).Distinct().ToList();

                if (year != null)
                {
                    schedules = schedules.Where(x => x.YEAR == year).ToList();
                }
                var presentationSchedules = new PresentationSchedules_Model()
                {
                    presentationSchedules = schedules,
                    Years = viewYears
                };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = presentationSchedules, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }

        [HttpGet("DIVISIONAL_PRESENTATIONS")]
        public async Task<WebApiResponse> DIVISIONAL_PRESENTATIONS(string year)
        {
            try
            {
                var presentations = (from sch in _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs select sch).ToList();
                var viewYears = presentations.Select(x => x.YEAR).Distinct().ToList();

                if (year != null)
                {
                    presentations = presentations.Where(x => x.YEAR == year).ToList();
                }
                var presentationDivision = new PresentationSchedules_Model()
                {
                    Divisionpresentations = presentations,
                    Years = viewYears
                };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = presentationDivision, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }

        #region work program admin     
        [HttpGet("OPL_RECALIBRATED_SCALED")]
        public async Task<WebApiResponse> opl_recalibrated(string year)
        {
            var details = new List<WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANy>();
            try
            {
                if (WKUserRole == GeneralModel.Admin)
                {

                    details = await _context.WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.Year_of_WP == year).ToListAsync();
                }
                else
                {
                    details = await _context.WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPCompanyName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
                }
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

        }
        [HttpGet("OPL_AGGREGATED_SCORE")]
        public async Task<WebApiResponse> opl_aggregated_score(string year)
        {
            var presentYear = DateTime.Now.Year;

            var details = new List<WP_OPL_Aggregated_Score_ALL_COMPANy>();
            try
            {
                if (WKUserRole == GeneralModel.Admin)
                {
                    details = await _context.WP_OPL_Aggregated_Score_ALL_COMPANIEs.Where(c => c.Year_of_WP == year).ToListAsync();

                }
                else
                {
                    details = await _context.WP_OPL_Aggregated_Score_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPCompanyName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
                }

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

        }
        [HttpGet("OML_RECALIBRATED_SCALED")]
        public async Task<WebApiResponse> OML_RECALIBRATED_SCALE(string year)
        {
            var details = new List<WP_OML_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANy>();
            try
            {
                if (WKUserRole == GeneralModel.Admin)
                {
                    details = await _context.WP_OML_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.Year_of_WP == year).ToListAsync();
                }
                else
                {
                    details = await _context.WP_OML_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPCompanyName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
                }
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

        }
        [HttpGet("OML_AGGREGATED_SCORE")]
        public async Task<WebApiResponse> oml_aggregated_score(string year)
        {
            var presentYear = DateTime.Now.Year;

            var details = new List<WP_OML_Aggregated_Score_ALL_COMPANy>();
            try
            {
                if (WKUserRole == GeneralModel.Admin)
                {
                    details = await _context.WP_OML_Aggregated_Score_ALL_COMPANIEs.Where(c => c.Year_of_WP == year).ToListAsync();

                }
                else
                {
                    details = await _context.WP_OML_Aggregated_Score_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPCompanyName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
                }

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

        }


        [HttpGet("GET_CONCESSION_HELD")]
        public async Task<object> Get_Concession_Held(string mycompanyId, string myyear)
        {
            return await (from a in _context.ADMIN_CONCESSIONS_INFORMATIONs where a.Company_ID == mycompanyId && a.Year == myyear && a.DELETED_STATUS == null select a.Concession_Held).Distinct().ToListAsync();
        }


        [HttpGet("GET_FORM_ONE")]
        public async Task<object> GET_FORM_ONE(string omlName, string myyear)
        {   
            var concessionInfo = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.Company_ID == WKPCompanyId && d.Concession_Held == omlName && d.Year == myyear && d.DELETED_STATUS == null select d).ToListAsync();
            var drillEachCost = await (from d in _context.DRILLING_EACH_WELL_COSTs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            var drillEachCostProposed = await (from d in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            var drillOperationCategoriesWell = await (from d in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            var geoActivitiesAcquisition = await (from d in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            var geoActivitiesProcessing = await (from d in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            var concessionSituation = await (from d in _context.CONCESSION_SITUATIONs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year == myyear select d).ToListAsync();
            var concessionSituation1stJanuary = await (from d in _context.CONCESSION_SITUATIONs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year == myyear orderby d.Year select d).ToListAsync();
            return new {concessionSituation = concessionSituation, concessionInfo = concessionInfo, drillEachCost = drillEachCost, drillEachCostProposed = drillEachCostProposed, drillOperationCategoriesWell = drillOperationCategoriesWell,
             geoActivitiesAcquisition = geoActivitiesAcquisition, geoActivitiesProcessing = geoActivitiesProcessing, concessionSituation1stJanuary = concessionSituation1stJanuary};
        }

        [HttpGet("GET_WPYEAR_LIST")]
        public async Task<object> Get_WPYear_List()
        {
            return await (from a in _context.ADMIN_CONCESSIONS_INFORMATIONs select a.Year).Distinct().ToListAsync();
        }

        #endregion

    }
}