using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Backend_UMR_Work_Program.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]/[action]")]
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

        private string? WKPUserId => "1";
        private string? WKPUserName => "Name";
        private string? WKPUserEmail => "adeola.kween123@gmail.com";
        private string? WKUserRole => "Admin";
        // private string? WKPUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        //private string? WKPUserName => User.FindFirstValue(ClaimTypes.Name);
        //private string? WKPUserEmail => User.FindFirstValue(ClaimTypes.Email);
        //private string? WKUserRole => User.FindFirstValue(ClaimTypes.Role);


        [HttpPost("POST_WORKPROGRAMME_1")]
        public async Task<WebApiResponse> Post_WORKPROGRAMME_1(WorkProgramme_Model_1 wkp, IFormFile FieldDiscoveryUploadFile, IFormFile HydrocarbonCountUploadFile)
        {

            int save = 0;

            try
            {
                #region save form data
                //Saving Concession Situations
                WebApiResponse ConcessionData = _helpersController.CONCESSION_SITUATION(wkp.CONCESSION_SITUATION, wkp.WorkProgramme_Year);
                if (ConcessionData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Geophysical Activites
                WebApiResponse GeophysicalActivitesData = _helpersController.GEOPHYSICAL_ACTIVITIES_ACQUISITION(wkp.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs, wkp.WorkProgramme_Year);
                if (GeophysicalActivitesData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Geophysical Activites Processing
                WebApiResponse GeoActivitesProcessingData = _helpersController.GEOPHYSICAL_ACTIVITIES_PROCESSING(wkp.GEOPHYSICAL_ACTIVITIES_PROCESSINGs, wkp.WorkProgramme_Year);
                if (GeoActivitesProcessingData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Drilling Operations
                List<IFormFile> DriilingFiles = new List<IFormFile>();
                DriilingFiles.Add(FieldDiscoveryUploadFile);
                DriilingFiles.Add(HydrocarbonCountUploadFile);

                WebApiResponse DrillingOperationsData = _helpersController.DRILLING_OPERATIONS_CATEGORIES_OF_WELL(wkp.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs, wkp.WorkProgramme_Year, DriilingFiles);
                if (DrillingOperationsData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Drilling Well Cost
                WebApiResponse DrillingWellCostData = _helpersController.DRILLING_EACH_WELL_COST(wkp.DRILLING_EACH_WELL_COSTs, wkp.WorkProgramme_Year);
                if (DrillingWellCostData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Geophysical Activites
                WebApiResponse DrillingWellCostProposedData = _helpersController.DRILLING_EACH_WELL_COST_PROPOSED(wkp.DRILLING_EACH_WELL_COST_PROPOSEDs, wkp.WorkProgramme_Year);
                if (DrillingWellCostProposedData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                #endregion

                if (save == 6)
                {
                    string successMsg = "Work programme form 1 has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Badrequest };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }


        [HttpPost("POST_CONCESSION")]
        public async Task<WebApiResponse> Post_CONCESSION_SITUATION(CONCESSION_SITUATION_Model wkp, string WorkProgramme_Year)
        {
            try
            {
                WebApiResponse result = _helpersController.CONCESSION_SITUATION(wkp, WorkProgramme_Year);

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Badrequest };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }


        [HttpPost("POST_GEOPHYSICAL_ACQUISITION")]
        public async Task<WebApiResponse> Post_GEOPHYSICAL_ACTIVITIES_ACQUISITION(GEOPHYSICAL_ACTIVITIES_ACQUISITION_Model wkp, string WorkProgramme_Year)
        {

            try
            {
                WebApiResponse result = _helpersController.GEOPHYSICAL_ACTIVITIES_ACQUISITION(wkp, WorkProgramme_Year);

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Badrequest };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }


        [HttpPost("POST_GEOPHYSICAL_ACTIVITIES_PROCESSING")]
        public async Task<WebApiResponse> Post_GEOPHYSICAL_ACTIVITIES_PROCESSING(GEOPHYSICAL_ACTIVITIES_PROCESSING_Model wkp, string WorkProgramme_Year)
        {

            try
            {
                WebApiResponse result = _helpersController.GEOPHYSICAL_ACTIVITIES_PROCESSING(wkp, WorkProgramme_Year);

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Badrequest };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }

        }

        [HttpPost("DRILLING_OPERATIONS_CATEGORIES_OF_WELL")]
        public async Task<WebApiResponse> Post_DRILLING_OPERATIONS(DRILLING_OPERATIONS_CATEGORIES_OF_WELL_Model wkp, IFormFile FieldDiscoveryUploadFile, IFormFile HydrocarbonCountUploadFile, string WorkProgramme_Year)
        {
            try
            {
                // Saving Drilling Operations
                List<IFormFile> DriilingFiles = new List<IFormFile>();
                DriilingFiles.Add(FieldDiscoveryUploadFile);
                DriilingFiles.Add(HydrocarbonCountUploadFile);

                WebApiResponse result = _helpersController.DRILLING_OPERATIONS_CATEGORIES_OF_WELL(wkp, WorkProgramme_Year, DriilingFiles);

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Badrequest };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }

        }

        [HttpPost("POST_DRILLING_EACH_WELL_COST")]
        public async Task<WebApiResponse> Post_DRILLING_EACH_WELL_COST(DRILLING_EACH_WELL_COST_Model wkp, string WorkProgramme_Year)
        {
            try
            {
                WebApiResponse result = _helpersController.DRILLING_EACH_WELL_COST(wkp, WorkProgramme_Year);

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Badrequest };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_WELL_PROPOSED")]
        public async Task<WebApiResponse> Post_DRILLING_WELL_PROPOSED(DRILLING_EACH_WELL_COST_PROPOSED_Model wkp, string WorkProgramme_Year)
        {
            try
            {
                WebApiResponse result = _helpersController.DRILLING_EACH_WELL_COST_PROPOSED(wkp, WorkProgramme_Year);

                if (result != null)
                {
                    return result;
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Badrequest };
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        #region Form 2
        [HttpPost("POST_WORKPROGRAMME_2")]
        public async Task<WebApiResponse> Post_WORKPROGRAMME_2(WorkProgramme_Model_2 wkp, IFormFile Uploaded_approved_FDP_Document, IFormFile PUAUploadFile_Document,
            IFormFile UUOAUploadFilePath_Document, IFormFile Upload_NDR_payment_receipt, IFormFile ProductionOilCondensateAGNAGUFile)
        {

            int save = 0;
            try
            {
                #region save form data
                //Saving Well Completion Job
                WebApiResponse Well_Completion_JobData = _helpersController.INITIAL_WELL_COMPLETION_JOB1(wkp.Initial_Well_Completion_Job, wkp.WorkProgramme_Year);
                if (Well_Completion_JobData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Well Completion Job
                WebApiResponse Workover_JobData = _helpersController.WORKOVERS_RECOMPLETION_JOB1(wkp.WORKOVERS_RECOMPLETION_JOB1, wkp.WorkProgramme_Year);
                if (Workover_JobData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving FDP_Excess Reserve
                WebApiResponse FDP_ExcessReserveData = _helpersController.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE(wkp.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE, wkp.WorkProgramme_Year);
                if (FDP_ExcessReserveData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving FDP To Submit
                WebApiResponse FDP_ToSubmitData = _helpersController.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP(wkp.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP, wkp.WorkProgramme_Year);
                if (FDP_ToSubmitData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving FDP Status
                WebApiResponse FDP_StatusData = _helpersController.FIELD_DEVELOPMENT_FIELDS_AND_STATUS(wkp.FIELD_DEVELOPMENT_FIELDS_AND_STATUS, wkp.WorkProgramme_Year);
                if (FDP_StatusData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Reserve Update
                WebApiResponse Reserve_UpdateData = _helpersController.RESERVES_UPDATES_LIFE_INDEX(wkp.RESERVES_UPDATES_LIFE_INDEX, wkp.WorkProgramme_Year);
                if (Reserve_UpdateData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving FDP Data
                List<IFormFile> FDPFiles = new List<IFormFile>();
                FDPFiles.Add(Uploaded_approved_FDP_Document);

                WebApiResponse FDP_Data = _helpersController.FIELD_DEVELOPMENT_PLAN(wkp.FIELD_DEVELOPMENT_PLAN, wkp.WorkProgramme_Year, FDPFiles);
                if (FDP_Data.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Oil Production
                WebApiResponse Oil_ProductionData = _helpersController.OIL_CONDENSATE_PRODUCTION_ACTIVITy(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITy, wkp.WorkProgramme_Year);
                if (Oil_ProductionData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Oil Unitization
                List<IFormFile> OilFiles = new List<IFormFile>();
                OilFiles.Add(PUAUploadFile_Document);
                OilFiles.Add(UUOAUploadFilePath_Document);

                WebApiResponse OilUnitizationData = _helpersController.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION, wkp.WorkProgramme_Year, OilFiles);
                if (OilUnitizationData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Gas Production
                List<IFormFile> GasFiles = new List<IFormFile>();
                GasFiles.Add(Upload_NDR_payment_receipt);

                WebApiResponse Gas_ProductionData = _helpersController.GAS_PRODUCTION_ACTIVITy(wkp.GAS_PRODUCTION_ACTIVITy, wkp.WorkProgramme_Year, GasFiles);
                if (Gas_ProductionData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving NDR
                List<IFormFile> NDRFiles = new List<IFormFile>();
                NDRFiles.Add(Upload_NDR_payment_receipt);

                WebApiResponse NDRData = _helpersController.NDR(wkp.NDR, wkp.WorkProgramme_Year, NDRFiles);
                if (NDRData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Oil Condensate Reserve Status
                WebApiResponse Oil_Condensate_ReserveStatusData = _helpersController.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE(wkp.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE, wkp.WorkProgramme_Year);
                if (Oil_Condensate_ReserveStatusData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Oil Condensate Reserve Update
                WebApiResponse Oil_Condensate_ReserveProjectionData = _helpersController.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection(wkp.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection, wkp.WorkProgramme_Year);
                if (Oil_Condensate_ReserveProjectionData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Oil Condensate Production Activities
                List<IFormFile> OilCondFiles = new List<IFormFile>();
                OilCondFiles.Add(ProductionOilCondensateAGNAGUFile);
                OilCondFiles.Add(ProductionOilCondensateAGNAGUFile);

                WebApiResponse Oil_Condensate_ProjectionData = _helpersController.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION, wkp.WorkProgramme_Year, OilCondFiles);
                if (Oil_Condensate_ReserveProjectionData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Oil Condensate Annual Data
                WebApiResponse Oil_Condensate_AnnualData = _helpersController.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION(wkp.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION, wkp.WorkProgramme_Year);
                if (Oil_Condensate_AnnualData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Oil Condensate Decline Data
                WebApiResponse Oil_Condensate_DeclineData = _helpersController.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE(wkp.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE, wkp.WorkProgramme_Year);
                if (Oil_Condensate_DeclineData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Oil Condensate Production Data
                WebApiResponse Oil_Condensate_ProductionData = _helpersController.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity, wkp.WorkProgramme_Year);
                if (Oil_Condensate_ProductionData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Reserve Replacement Data
                WebApiResponse Reserve_ReplacementData = _helpersController.RESERVES_REPLACEMENT_RATIO(wkp.RESERVES_REPLACEMENT_RATIO, wkp.WorkProgramme_Year);
                if (Reserve_ReplacementData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Oil Monthly Data
                WebApiResponse Oil_MonthlyData = _helpersController.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED, wkp.WorkProgramme_Year);
                if (Oil_MonthlyData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                // Saving Oil Gas Activities Data
                WebApiResponse Gas_ActivitiesData = _helpersController.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY(wkp.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY, wkp.WorkProgramme_Year);
                if (Gas_ActivitiesData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }


                #endregion
                if (save == 20)
                {
                    string successMsg = "Work programme form 2 has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

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
                WebApiResponse BudgetActualExData = _helpersController.BUDGET_ACTUAL_EXPENDITURE(wkp.BUDGET_ACTUAL_EXPENDITURE, wkp.WorkProgramme_Year);
                if (BudgetActualExData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Budget Proposal
                WebApiResponse BudgetProposalData = _helpersController.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT(wkp.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT, wkp.WorkProgramme_Year);
                if (BudgetProposalData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Budget Performance Exploratory
                WebApiResponse BudgetPerformanceExpData = _helpersController.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy(wkp.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy, wkp.WorkProgramme_Year);
                if (BudgetPerformanceExpData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Budget Performance Development 
                WebApiResponse BudgetPerformanceDevData = _helpersController.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy(wkp.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy, wkp.WorkProgramme_Year);
                if (BudgetPerformanceDevData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Budget Performance Production
                WebApiResponse BudgetPerformanceProdData = _helpersController.BUDGET_PERFORMANCE_PRODUCTION_COST(wkp.BUDGET_PERFORMANCE_PRODUCTION_COST, wkp.WorkProgramme_Year);
                if (BudgetPerformanceProdData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Budget Performance Facility
                WebApiResponse BudgetPerformanceFacData = _helpersController.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT(wkp.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT, wkp.WorkProgramme_Year);
                if (BudgetPerformanceFacData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Oil Gas Facility
                WebApiResponse OilGasFacData = _helpersController.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE(wkp.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE, wkp.WorkProgramme_Year);
                if (OilGasFacData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Oil Gas Production
                WebApiResponse OilGasProdData = _helpersController.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment(wkp.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment, wkp.WorkProgramme_Year);
                if (OilGasProdData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Oil Gas Facilty Maintenance
                WebApiResponse OilGasFacProjectData = _helpersController.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT(wkp.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT, wkp.WorkProgramme_Year);
                if (OilGasFacProjectData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Oil Gas Facilty Project
                WebApiResponse FacProjectData = _helpersController.FACILITIES_PROJECT_PERFORMANCE(wkp.FACILITIES_PROJECT_PERFORMANCE, wkp.WorkProgramme_Year);
                if (FacProjectData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }//Saving acilty Project
                WebApiResponse BudgetCapexOpexData = _helpersController.BUDGET_CAPEX_OPEX(wkp.BUDGET_CAPEX_OPEX, wkp.WorkProgramme_Year);
                if (BudgetCapexOpexData.ResponseCode == AppResponseCodes.Success)
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
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Badrequest };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

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
                WebApiResponse NigeriaTrainingData = _helpersController.NIGERIA_CONTENT_Training(wkp.NIGERIA_CONTENT_Training, wkp.WorkProgramme_Year);
                if (NigeriaTrainingData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Nigeria Content Upload
                WebApiResponse NigeriaUploadData = _helpersController.NIGERIA_CONTENT_Upload_Succession_Plan(wkp.NIGERIA_CONTENT_Upload_Succession_Plan, wkp.WorkProgramme_Year);
                if (NigeriaUploadData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Nigeria Content Upload
                WebApiResponse NigeriaQuestionData = _helpersController.NIGERIA_CONTENT_QUESTION(wkp.NIGERIA_CONTENT_QUESTION, wkp.WorkProgramme_Year);
                if (NigeriaQuestionData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Nigeria Content Upload
                WebApiResponse LegalLitigationData = _helpersController.LEGAL_LITIGATION(wkp.LEGAL_LITIGATION, wkp.WorkProgramme_Year);
                if (LegalLitigationData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Nigeria Content Upload
                WebApiResponse LegalArbitrationData = _helpersController.LEGAL_ARBITRATION(wkp.LEGAL_ARBITRATION, wkp.WorkProgramme_Year);
                if (LegalArbitrationData.ResponseCode == AppResponseCodes.Success)
                {
                    save++;
                }
                //Saving Strategic Plans
                WebApiResponse StrategicPlansData = _helpersController.STRATEGIC_PLANS_ON_COMPANY_BASI(wkp.STRATEGIC_PLANS_ON_COMPANY_BASI, wkp.WorkProgramme_Year);
                if (StrategicPlansData.ResponseCode == AppResponseCodes.Success)
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
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Badrequest };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

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

            #region variable declaration
            int save = 0;
            var HseQuestionData = new HSE_QUESTION();
            var HseFatalityData = new HSE_FATALITy();
            var HseDesignData = new HSE_DESIGNS_SAFETY();
            var HseSafetyData = new HSE_SAFETY_STUDIES_NEW();
            var HseInspectionMaintenanceData = new HSE_INSPECTION_AND_MAINTENANCE_NEW();
            var HseInspectionMaintenanceFacData = new HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW();
            var HseTechnicalData = new HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW();
            var HseAssetData = new HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW();
            var HseOilData = new HSE_OIL_SPILL_REPORTING_NEW();
            var HseAssetRegisterData = new HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW();
            var HseAccidentData = new HSE_ACCIDENT_INCIDENCE_REPORTING_NEW();
            var HseAccidentIncidenceData = new HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW();
            var HseCommunityDisturbancesData = new HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW();
            var HseEnvironmentalData = new HSE_ENVIRONMENTAL_STUDIES_NEW();
            var HseWasteData = new HSE_WASTE_MANAGEMENT_NEW();
            var HseWasteMgtData = new HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW();
            var HseWaterMgtData = new HSE_PRODUCED_WATER_MANAGEMENT_NEW();
            var HseEnvironmentalComplianceData = new HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW();
            var HseEnvironmentalStudiesData = new HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW();
            var HseSustainableData = new HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL();
            var HseEnvironmentalStudiesUpdatedData = new HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED();
            var HseOSPData = new HSE_OSP_REGISTRATIONS_NEW();
            var HseProducedWaterData = new HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED();
            var HseEnvironmentalMonitoringData = new HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW();
            var HseCausesData = new HSE_CAUSES_OF_SPILL();
            var HseSustainableDevData = new HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU();
            var HseSustainableComData = new HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME();
            var HseManagmentData = new HSE_MANAGEMENT_POSITION();
            var HseQualityData = new HSE_QUALITY_CONTROL();
            var HseClimateData = new HSE_CLIMATE_CHANGE_AND_AIR_QUALITY();
            var HseSafetyCultureData = new HSE_SAFETY_CULTURE_TRAINING();
            var HseOccupationalData = new HSE_OCCUPATIONAL_HEALTH_MANAGEMENT();
            var HseWasteMgtSysData = new HSE_WASTE_MANAGEMENT_SYSTEM();
            var HseEnvWasteData = new HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM();
            var PictureUploadData = new PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT();
            #endregion
            try
            {
                #region Saving Hse Question
                var getHseQuestionData = (from c in _context.HSE_QUESTIONs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseQuestionData = HseQuestionData != null ? HseQuestionData : HseQuestionData;
                HseQuestionData = _mapper.Map<HSE_QUESTION>(wkp.HSE_QUESTION);

                HseQuestionData.Companyemail = WKPUserEmail;
                HseQuestionData.CompanyName = WKPUserName;
                HseQuestionData.COMPANY_ID = WKPUserId;
                HseQuestionData.Created_by = WKPUserId;
                HseQuestionData.Date_Created = DateTime.Now;
                HseQuestionData.Date_Updated = DateTime.Now;
                HseQuestionData.Updated_by = WKPUserId;
                HseQuestionData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseQuestionData == null)
                {
                    _context.HSE_QUESTIONs.AddAsync(getHseQuestionData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Fatality
                var getHseFatalityData = (from c in _context.HSE_FATALITIEs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseFatalityData = HseFatalityData != null ? HseFatalityData : HseFatalityData;
                HseFatalityData = _mapper.Map<HSE_FATALITy>(wkp.HSE_FATALITy);

                HseFatalityData.Companyemail = WKPUserEmail;
                HseFatalityData.CompanyName = WKPUserName;
                HseFatalityData.COMPANY_ID = WKPUserId;
                HseFatalityData.Created_by = WKPUserId;
                HseFatalityData.Date_Created = DateTime.Now;
                HseFatalityData.Date_Updated = DateTime.Now;
                HseFatalityData.Updated_by = WKPUserId;
                HseFatalityData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseFatalityData == null)
                {
                    _context.HSE_FATALITIEs.AddAsync(getHseFatalityData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Design Safety
                var getHseDesignData = (from c in _context.HSE_DESIGNS_SAFETies where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseDesignData = HseDesignData != null ? HseDesignData : HseDesignData;
                HseDesignData = _mapper.Map<HSE_DESIGNS_SAFETY>(wkp.HSE_DESIGNS_SAFETY);

                HseDesignData.Companyemail = WKPUserEmail;
                HseDesignData.CompanyName = WKPUserName;
                HseDesignData.COMPANY_ID = WKPUserId;
                HseDesignData.Created_by = WKPUserId;
                HseDesignData.Date_Created = DateTime.Now;
                HseDesignData.Date_Updated = DateTime.Now;
                HseDesignData.Updated_by = WKPUserId;
                HseDesignData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseDesignData == null)
                {
                    _context.HSE_DESIGNS_SAFETies.AddAsync(getHseDesignData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Safety Studies
                var getHseSafetyData = (from c in _context.HSE_SAFETY_STUDIES_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseSafetyData = HseSafetyData != null ? HseSafetyData : HseSafetyData;
                HseSafetyData = _mapper.Map<HSE_SAFETY_STUDIES_NEW>(wkp.HSE_SAFETY_STUDIES_NEW);

                #region file section
                UploadedDocument SMSFileUploadPath_File = null;

                if (SMSFileUploadPath != null)
                {
                    string docName = "SMS File";
                    SMSFileUploadPath_File = _helpersController.UploadDocument(SMSFileUploadPath, "SMSDocuments");
                    if (SMSFileUploadPath_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }
                #endregion

                HseSafetyData.SMSFileUploadPath = SMSFileUploadPath_File != null ? SMSFileUploadPath_File.filePath : null;
                HseSafetyData.Companyemail = WKPUserEmail;
                HseSafetyData.CompanyName = WKPUserName;
                HseSafetyData.COMPANY_ID = WKPUserId;
                HseSafetyData.Created_by = WKPUserId;
                HseSafetyData.Date_Created = DateTime.Now;
                HseSafetyData.Date_Updated = DateTime.Now;
                HseSafetyData.Updated_by = WKPUserId;
                HseSafetyData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseSafetyData == null)
                {
                    _context.HSE_SAFETY_STUDIES_NEWs.AddAsync(getHseSafetyData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Inspection Maintenance
                var getHseInspectionMaintenanceData = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseInspectionMaintenanceData = HseInspectionMaintenanceData != null ? HseInspectionMaintenanceData : HseInspectionMaintenanceData;
                HseInspectionMaintenanceData = _mapper.Map<HSE_INSPECTION_AND_MAINTENANCE_NEW>(wkp.HSE_QUESTION);

                HseInspectionMaintenanceData.Companyemail = WKPUserEmail;
                HseInspectionMaintenanceData.CompanyName = WKPUserName;
                HseInspectionMaintenanceData.COMPANY_ID = WKPUserId;
                HseInspectionMaintenanceData.Created_by = WKPUserId;
                HseInspectionMaintenanceData.Date_Created = DateTime.Now;
                HseInspectionMaintenanceData.Date_Updated = DateTime.Now;
                HseInspectionMaintenanceData.Updated_by = WKPUserId;
                HseInspectionMaintenanceData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseInspectionMaintenanceData == null)
                {
                    _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs.AddAsync(getHseInspectionMaintenanceData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Inspection Maintenance and Facility
                var getHseInspectionMaintenanceFacData = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseInspectionMaintenanceFacData = HseInspectionMaintenanceFacData != null ? HseInspectionMaintenanceFacData : HseInspectionMaintenanceFacData;
                HseInspectionMaintenanceFacData = _mapper.Map<HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW>(wkp.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW);

                HseInspectionMaintenanceFacData.Companyemail = WKPUserEmail;
                HseInspectionMaintenanceFacData.CompanyName = WKPUserName;
                HseInspectionMaintenanceFacData.COMPANY_ID = WKPUserId;
                HseInspectionMaintenanceFacData.Created_by = WKPUserId;
                HseInspectionMaintenanceFacData.Date_Created = DateTime.Now;
                HseInspectionMaintenanceFacData.Date_Updated = DateTime.Now;
                HseInspectionMaintenanceFacData.Updated_by = WKPUserId;
                HseInspectionMaintenanceFacData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseInspectionMaintenanceFacData == null)
                {
                    _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.AddAsync(getHseInspectionMaintenanceFacData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Technical Safety
                var getHseTechnicalData = (from c in _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseTechnicalData = HseTechnicalData != null ? HseTechnicalData : HseTechnicalData;
                HseTechnicalData = _mapper.Map<HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW>(wkp.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW);

                HseTechnicalData.Companyemail = WKPUserEmail;
                HseTechnicalData.CompanyName = WKPUserName;
                HseTechnicalData.COMPANY_ID = WKPUserId;
                HseTechnicalData.Created_by = WKPUserId;
                HseTechnicalData.Date_Created = DateTime.Now;
                HseTechnicalData.Date_Updated = DateTime.Now;
                HseTechnicalData.Updated_by = WKPUserId;
                HseTechnicalData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseTechnicalData == null)
                {
                    _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.AddAsync(getHseTechnicalData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Asset Register
                var getHseAssetData = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseAssetData = HseAssetData != null ? HseAssetData : HseAssetData;
                HseAssetData = _mapper.Map<HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW>(wkp.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW);

                HseAssetData.Companyemail = WKPUserEmail;
                HseAssetData.CompanyName = WKPUserName;
                HseAssetData.COMPANY_ID = WKPUserId;
                HseAssetData.Created_by = WKPUserId;
                HseAssetData.Date_Created = DateTime.Now;
                HseAssetData.Date_Updated = DateTime.Now;
                HseAssetData.Updated_by = WKPUserId;
                HseAssetData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseAssetData == null)
                {
                    _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs.AddAsync(getHseAssetData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Oil Spill
                var getHseOilData = (from c in _context.HSE_OIL_SPILL_REPORTING_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseOilData = HseOilData != null ? HseOilData : HseOilData;
                HseOilData = _mapper.Map<HSE_OIL_SPILL_REPORTING_NEW>(wkp.HSE_OIL_SPILL_REPORTING_NEW);

                HseOilData.Companyemail = WKPUserEmail;
                HseOilData.CompanyName = WKPUserName;
                HseOilData.COMPANY_ID = WKPUserId;
                HseOilData.Created_by = WKPUserId;
                HseOilData.Date_Created = DateTime.Now;
                HseOilData.Date_Updated = DateTime.Now;
                HseOilData.Updated_by = WKPUserId;
                HseOilData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseOilData == null)
                {
                    _context.HSE_OIL_SPILL_REPORTING_NEWs.AddAsync(getHseOilData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Asset Register RBI
                var getHseAssetRegisterData = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseAssetRegisterData = HseAssetRegisterData != null ? HseAssetRegisterData : HseAssetRegisterData;
                HseAssetRegisterData = _mapper.Map<HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW>(wkp.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW);

                HseAssetRegisterData.Companyemail = WKPUserEmail;
                HseAssetRegisterData.CompanyName = WKPUserName;
                HseAssetRegisterData.COMPANY_ID = WKPUserId;
                HseAssetRegisterData.Created_by = WKPUserId;
                HseAssetRegisterData.Date_Created = DateTime.Now;
                HseAssetRegisterData.Date_Updated = DateTime.Now;
                HseAssetRegisterData.Updated_by = WKPUserId;
                HseAssetRegisterData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseAssetRegisterData == null)
                {
                    _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs.AddAsync(getHseAssetRegisterData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Incidence Reporting
                var getHseAccidentData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseAccidentData = HseAccidentData != null ? HseAccidentData : HseAccidentData;
                HseAccidentData = _mapper.Map<HSE_ACCIDENT_INCIDENCE_REPORTING_NEW>(wkp.HSE_ACCIDENT_INCIDENCE_REPORTING_NEW);

                HseAccidentData.Companyemail = WKPUserEmail;
                HseAccidentData.CompanyName = WKPUserName;
                HseAccidentData.COMPANY_ID = WKPUserId;
                HseAccidentData.Created_by = WKPUserId;
                HseAccidentData.Date_Created = DateTime.Now;
                HseAccidentData.Date_Updated = DateTime.Now;
                HseAccidentData.Updated_by = WKPUserId;
                HseAccidentData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseAccidentData == null)
                {
                    _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.AddAsync(getHseAccidentData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Incidence Reporting Type
                var getHseAccidentIncidenceData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseAccidentIncidenceData = HseAccidentIncidenceData != null ? HseAccidentIncidenceData : HseAccidentIncidenceData;
                HseAccidentIncidenceData = _mapper.Map<HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW>(wkp.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW);

                HseAccidentIncidenceData.Companyemail = WKPUserEmail;
                HseAccidentIncidenceData.CompanyName = WKPUserName;
                HseAccidentIncidenceData.COMPANY_ID = WKPUserId;
                HseAccidentIncidenceData.Created_by = WKPUserId;
                HseAccidentIncidenceData.Date_Created = DateTime.Now;
                HseAccidentIncidenceData.Date_Updated = DateTime.Now;
                HseAccidentIncidenceData.Updated_by = WKPUserId;
                HseAccidentIncidenceData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseAccidentIncidenceData == null)
                {
                    _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.AddAsync(getHseAccidentIncidenceData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Community Disturbances
                var getHseCommunityDisturbancesData = (from c in _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseCommunityDisturbancesData = HseCommunityDisturbancesData != null ? HseCommunityDisturbancesData : HseCommunityDisturbancesData;
                HseCommunityDisturbancesData = _mapper.Map<HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW>(wkp.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW);

                HseCommunityDisturbancesData.Companyemail = WKPUserEmail;
                HseCommunityDisturbancesData.CompanyName = WKPUserName;
                HseCommunityDisturbancesData.COMPANY_ID = WKPUserId;
                HseCommunityDisturbancesData.Created_by = WKPUserId;
                HseCommunityDisturbancesData.Date_Created = DateTime.Now;
                HseCommunityDisturbancesData.Date_Updated = DateTime.Now;
                HseCommunityDisturbancesData.Updated_by = WKPUserId;
                HseCommunityDisturbancesData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseCommunityDisturbancesData == null)
                {
                    _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs.AddAsync(getHseCommunityDisturbancesData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Environmental Studies
                var getHseEnvironmentalData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseEnvironmentalData = HseEnvironmentalData != null ? HseEnvironmentalData : HseEnvironmentalData;
                HseEnvironmentalData = _mapper.Map<HSE_ENVIRONMENTAL_STUDIES_NEW>(wkp.HSE_ENVIRONMENTAL_STUDIES_NEW);

                HseEnvironmentalData.Companyemail = WKPUserEmail;
                HseEnvironmentalData.CompanyName = WKPUserName;
                HseEnvironmentalData.COMPANY_ID = WKPUserId;
                HseEnvironmentalData.Created_by = WKPUserId;
                HseEnvironmentalData.Date_Created = DateTime.Now;
                HseEnvironmentalData.Date_Updated = DateTime.Now;
                HseEnvironmentalData.Updated_by = WKPUserId;
                HseEnvironmentalData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseEnvironmentalData == null)
                {
                    _context.HSE_ENVIRONMENTAL_STUDIES_NEWs.AddAsync(getHseEnvironmentalData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Waste Management
                var getHseWasteData = (from c in _context.HSE_WASTE_MANAGEMENT_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseWasteData = HseWasteData != null ? HseWasteData : HseWasteData;
                HseWasteData = _mapper.Map<HSE_WASTE_MANAGEMENT_NEW>(wkp.HSE_WASTE_MANAGEMENT_NEW);

                HseWasteData.Companyemail = WKPUserEmail;
                HseWasteData.CompanyName = WKPUserName;
                HseWasteData.COMPANY_ID = WKPUserId;
                HseWasteData.Created_by = WKPUserId;
                HseWasteData.Date_Created = DateTime.Now;
                HseWasteData.Date_Updated = DateTime.Now;
                HseWasteData.Updated_by = WKPUserId;
                HseWasteData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseWasteData == null)
                {
                    _context.HSE_WASTE_MANAGEMENT_NEWs.AddAsync(getHseWasteData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Waste Management Facility
                var getHseWasteMgtData = (from c in _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseWasteMgtData = HseWasteMgtData != null ? HseWasteMgtData : HseWasteMgtData;
                HseWasteMgtData = _mapper.Map<HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW>(wkp.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW);

                HseWasteMgtData.Companyemail = WKPUserEmail;
                HseWasteMgtData.CompanyName = WKPUserName;
                HseWasteMgtData.COMPANY_ID = WKPUserId;
                HseWasteMgtData.Created_by = WKPUserId;
                HseWasteMgtData.Date_Created = DateTime.Now;
                HseWasteMgtData.Date_Updated = DateTime.Now;
                HseWasteMgtData.Updated_by = WKPUserId;
                HseWasteMgtData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseWasteMgtData == null)
                {
                    _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs.AddAsync(getHseWasteMgtData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Produced Water
                var getHseWaterMgtData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseWaterMgtData = HseWaterMgtData != null ? HseWaterMgtData : HseWaterMgtData;
                HseWaterMgtData = _mapper.Map<HSE_PRODUCED_WATER_MANAGEMENT_NEW>(wkp.HSE_PRODUCED_WATER_MANAGEMENT_NEW);

                HseWaterMgtData.Companyemail = WKPUserEmail;
                HseWaterMgtData.CompanyName = WKPUserName;
                HseWaterMgtData.COMPANY_ID = WKPUserId;
                HseWaterMgtData.Created_by = WKPUserId;
                HseWaterMgtData.Date_Created = DateTime.Now;
                HseWaterMgtData.Date_Updated = DateTime.Now;
                HseWaterMgtData.Updated_by = WKPUserId;
                HseWaterMgtData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseWaterMgtData == null)
                {
                    _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs.AddAsync(getHseWaterMgtData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Environmental Compliance
                var getHseEnvironmentalComplianceData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseEnvironmentalComplianceData = HseEnvironmentalComplianceData != null ? HseEnvironmentalComplianceData : HseEnvironmentalComplianceData;
                HseEnvironmentalComplianceData = _mapper.Map<HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW>(wkp.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW);

                HseEnvironmentalComplianceData.Companyemail = WKPUserEmail;
                HseEnvironmentalComplianceData.CompanyName = WKPUserName;
                HseEnvironmentalComplianceData.COMPANY_ID = WKPUserId;
                HseEnvironmentalComplianceData.Created_by = WKPUserId;
                HseEnvironmentalComplianceData.Date_Created = DateTime.Now;
                HseEnvironmentalComplianceData.Date_Updated = DateTime.Now;
                HseEnvironmentalComplianceData.Updated_by = WKPUserId;
                HseEnvironmentalComplianceData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseEnvironmentalComplianceData == null)
                {
                    _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs.AddAsync(getHseEnvironmentalComplianceData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Environmental Studies Five
                var getHseEnvironmentalStudiesData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseEnvironmentalStudiesData = HseEnvironmentalStudiesData != null ? HseEnvironmentalStudiesData : HseEnvironmentalStudiesData;
                HseEnvironmentalStudiesData = _mapper.Map<HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW>(wkp.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW);

                HseEnvironmentalStudiesData.Companyemail = WKPUserEmail;
                HseEnvironmentalStudiesData.CompanyName = WKPUserName;
                HseEnvironmentalStudiesData.COMPANY_ID = WKPUserId;
                HseEnvironmentalStudiesData.Created_by = WKPUserId;
                HseEnvironmentalStudiesData.Date_Created = DateTime.Now;
                HseEnvironmentalStudiesData.Date_Updated = DateTime.Now;
                HseEnvironmentalStudiesData.Updated_by = WKPUserId;
                HseEnvironmentalStudiesData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseEnvironmentalStudiesData == null)
                {
                    _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs.AddAsync(getHseEnvironmentalStudiesData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Sustainable Development
                var getHseSustainableData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseSustainableData = getHseSustainableData != null ? getHseSustainableData : HseSustainableData;
                HseSustainableData = _mapper.Map<HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL>(wkp.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL);

                HseSustainableData.Companyemail = WKPUserEmail;
                HseSustainableData.CompanyName = WKPUserName;
                HseSustainableData.COMPANY_ID = WKPUserId;
                HseSustainableData.Created_by = WKPUserId;
                HseSustainableData.Date_Created = DateTime.Now;
                HseSustainableData.Date_Updated = DateTime.Now;
                HseSustainableData.Updated_by = WKPUserId;
                HseSustainableData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseSustainableData == null)
                {
                    _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs.AddAsync(HseSustainableData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Environmental Studies
                var getHseEnvironmentalStudiesUpdatedData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseEnvironmentalStudiesUpdatedData = HseEnvironmentalStudiesUpdatedData != null ? HseEnvironmentalStudiesUpdatedData : HseEnvironmentalStudiesUpdatedData;
                HseEnvironmentalStudiesUpdatedData = _mapper.Map<HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED>(wkp.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED);

                HseEnvironmentalStudiesUpdatedData.Companyemail = WKPUserEmail;
                HseEnvironmentalStudiesUpdatedData.CompanyName = WKPUserName;
                HseEnvironmentalStudiesUpdatedData.COMPANY_ID = WKPUserId;
                HseEnvironmentalStudiesUpdatedData.Created_by = WKPUserId;
                HseEnvironmentalStudiesUpdatedData.Date_Created = DateTime.Now;
                HseEnvironmentalStudiesUpdatedData.Date_Updated = DateTime.Now;
                HseEnvironmentalStudiesUpdatedData.Updated_by = WKPUserId;
                HseEnvironmentalStudiesUpdatedData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseEnvironmentalStudiesUpdatedData == null)
                {
                    _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs.AddAsync(getHseEnvironmentalStudiesUpdatedData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse OSP Registrations
                var getHseOSPData = (from c in _context.HSE_OSP_REGISTRATIONS_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseOSPData = HseOSPData != null ? HseOSPData : HseOSPData;
                HseOSPData = _mapper.Map<HSE_OSP_REGISTRATIONS_NEW>(wkp.HSE_OSP_REGISTRATIONS_NEW);

                HseOSPData.Companyemail = WKPUserEmail;
                HseOSPData.CompanyName = WKPUserName;
                HseOSPData.COMPANY_ID = WKPUserId;
                HseOSPData.Created_by = WKPUserId;
                HseOSPData.Date_Created = DateTime.Now;
                HseOSPData.Date_Updated = DateTime.Now;
                HseOSPData.Updated_by = WKPUserId;
                HseOSPData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseOSPData == null)
                {
                    _context.HSE_OSP_REGISTRATIONS_NEWs.AddAsync(getHseOSPData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Produced Water
                var getHseProducedWaterData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();
                HseProducedWaterData = HseProducedWaterData != null ? HseProducedWaterData : HseProducedWaterData;
                HseProducedWaterData = _mapper.Map<HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED>(wkp.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED);

                HseProducedWaterData.Companyemail = WKPUserEmail;
                HseProducedWaterData.CompanyName = WKPUserName;
                HseProducedWaterData.COMPANY_ID = WKPUserId;
                HseProducedWaterData.Created_by = WKPUserId;
                HseProducedWaterData.Date_Created = DateTime.Now;
                HseProducedWaterData.Date_Updated = DateTime.Now;
                HseProducedWaterData.Updated_by = WKPUserId;
                HseProducedWaterData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseProducedWaterData == null)
                {
                    _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.AddAsync(getHseProducedWaterData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Environmental Monitoring 
                var getHseEnvironmentalMonitoringData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();
                HseEnvironmentalMonitoringData = HseEnvironmentalMonitoringData != null ? HseEnvironmentalMonitoringData : HseEnvironmentalMonitoringData;
                HseEnvironmentalMonitoringData = _mapper.Map<HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW>(wkp.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW);

                HseEnvironmentalMonitoringData.Companyemail = WKPUserEmail;
                HseEnvironmentalMonitoringData.CompanyName = WKPUserName;
                HseEnvironmentalMonitoringData.COMPANY_ID = WKPUserId;
                HseEnvironmentalMonitoringData.Created_by = WKPUserId;
                HseEnvironmentalMonitoringData.Date_Created = DateTime.Now;
                HseEnvironmentalMonitoringData.Date_Updated = DateTime.Now;
                HseEnvironmentalMonitoringData.Updated_by = WKPUserId;
                HseEnvironmentalMonitoringData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseEnvironmentalMonitoringData == null)
                {
                    _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs.AddAsync(getHseEnvironmentalMonitoringData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Causes of Spill
                var getHseCausesData = (from c in _context.HSE_CAUSES_OF_SPILLs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();
                HseCausesData = HseCausesData != null ? HseCausesData : HseCausesData;
                HseCausesData = _mapper.Map<HSE_CAUSES_OF_SPILL>(wkp.HSE_CAUSES_OF_SPILL);

                HseCausesData.Companyemail = WKPUserEmail;
                HseCausesData.CompanyName = WKPUserName;
                HseCausesData.COMPANY_ID = WKPUserId;
                HseCausesData.Created_by = WKPUserId;
                HseCausesData.Date_Created = DateTime.Now;
                HseCausesData.Date_Updated = DateTime.Now;
                HseCausesData.Updated_by = WKPUserId;
                HseCausesData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseCausesData == null)
                {
                    _context.HSE_CAUSES_OF_SPILLs.AddAsync(getHseCausesData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Sustainable Dev Community
                var getHseSustainableDevData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();
                HseSustainableDevData = HseSustainableDevData != null ? HseSustainableDevData : HseSustainableDevData;
                HseSustainableDevData = _mapper.Map<HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU>(wkp.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU);
                #region file section
                UploadedDocument MOUUploadFile_File = null;

                if (MOUUploadFile != null)
                {
                    string docName = "MOUU";
                    MOUUploadFile_File = _helpersController.UploadDocument(MOUUploadFile, "MOUDocuments");
                    if (MOUUploadFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }
                #endregion


                HseSustainableDevData.MOUUploadFilePath = MOUUploadFile != null ? MOUUploadFile_File.filePath : null; ;
                HseSustainableDevData.MOUUploadFilename = MOUUploadFile != null ? MOUUploadFile_File.fileName : null; ;
                HseSustainableDevData.Companyemail = WKPUserEmail;
                HseSustainableDevData.CompanyName = WKPUserName;
                HseSustainableDevData.COMPANY_ID = WKPUserId;
                HseSustainableDevData.Created_by = WKPUserId;
                HseSustainableDevData.Date_Created = DateTime.Now;
                HseSustainableDevData.Date_Updated = DateTime.Now;
                HseSustainableDevData.Updated_by = WKPUserId;
                HseSustainableDevData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseSustainableDevData == null)
                {
                    _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs.AddAsync(getHseSustainableDevData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Sustainable Dev Schemes
                var getHseSustainableComData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();
                HseSustainableComData = HseSustainableComData != null ? HseSustainableComData : HseSustainableComData;
                HseSustainableComData = _mapper.Map<HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME>(wkp.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME);
                #region file section
                UploadedDocument SSUploadFile_File = null;

                if (SSUploadFile != null)
                {
                    string docName = "SS";
                    SSUploadFile_File = _helpersController.UploadDocument(SSUploadFile, "SSDocuments");
                    if (SSUploadFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }
                #endregion
                HseSustainableComData.SSUploadFilePath = SSUploadFile != null ? SSUploadFile_File.filePath : null;
                HseSustainableComData.SSUploadFilename = SSUploadFile != null ? SSUploadFile_File.fileName : null;
                HseSustainableComData.Companyemail = WKPUserEmail;
                HseSustainableComData.CompanyName = WKPUserName;
                HseSustainableComData.COMPANY_ID = WKPUserId;
                HseSustainableComData.Created_by = WKPUserId;
                HseSustainableComData.Date_Created = DateTime.Now;
                HseSustainableComData.Date_Updated = DateTime.Now;
                HseSustainableComData.Updated_by = WKPUserId;
                HseSustainableComData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseSustainableComData == null)
                {
                    _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs.AddAsync(getHseSustainableComData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Management position
                var getHseManagmentData = (from c in _context.HSE_MANAGEMENT_POSITIONs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseManagmentData = HseManagmentData != null ? HseManagmentData : HseManagmentData;
                HseManagmentData = _mapper.Map<HSE_MANAGEMENT_POSITION>(wkp.HSE_MANAGEMENT_POSITION);
                #region file section
                UploadedDocument PromotionLetterFile_File = null;
                UploadedDocument OrganogramFile_File = null;

                if (PromotionLetterFile != null)
                {
                    string docName = "Promotion Letter";
                    PromotionLetterFile_File = _helpersController.UploadDocument(PromotionLetterFile, "HRDocuments");
                    if (PromotionLetterFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }
                if (OrganogramFile != null)
                {
                    string docName = "Organogram";
                    OrganogramFile_File = _helpersController.UploadDocument(OrganogramFile, "OGDocuments");
                    if (OrganogramFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }
                #endregion


                HseManagmentData.PromotionLetterFilePath = PromotionLetterFile != null ? PromotionLetterFile_File.filePath : null;
                HseManagmentData.PromotionLetterFilename = PromotionLetterFile != null ? PromotionLetterFile_File.fileName : null;
                HseManagmentData.OrganogramrFilePath = OrganogramFile != null ? OrganogramFile_File.filePath : null;
                HseManagmentData.OrganogramrFilename = OrganogramFile != null ? OrganogramFile_File.fileName : null;
                HseManagmentData.Companyemail = WKPUserEmail;
                HseManagmentData.CompanyName = WKPUserName;
                HseManagmentData.COMPANY_ID = WKPUserId;
                HseManagmentData.Created_by = WKPUserId;
                HseManagmentData.Date_Created = DateTime.Now;
                HseManagmentData.Date_Updated = DateTime.Now;
                HseManagmentData.Updated_by = WKPUserId;
                HseManagmentData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseManagmentData == null)
                {
                    _context.HSE_MANAGEMENT_POSITIONs.AddAsync(getHseManagmentData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Quality
                var getHseQualityData = (from c in _context.HSE_QUALITY_CONTROLs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();

                HseQualityData = HseQualityData != null ? HseQualityData : HseQualityData;
                HseQualityData = _mapper.Map<HSE_QUALITY_CONTROL>(wkp.HSE_QUALITY_CONTROL);
                #region file section
                UploadedDocument QualityControlFile_File = null;

                if (QualityControlFile != null)
                {
                    string docName = "Quality Control";
                    QualityControlFile_File = _helpersController.UploadDocument(QualityControlFile, "COSDocuments");
                    if (QualityControlFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }

                #endregion

                HseQualityData.QualityControlFilename = QualityControlFile != null ? QualityControlFile_File.fileName : null;
                HseQualityData.QualityControlFilePath = QualityControlFile != null ? QualityControlFile_File.filePath : null;
                HseQualityData.Companyemail = WKPUserEmail;
                HseQualityData.CompanyName = WKPUserName;
                HseQualityData.COMPANY_ID = WKPUserId;
                HseQualityData.Created_by = WKPUserId;
                HseQualityData.Date_Created = DateTime.Now;
                HseQualityData.Date_Updated = DateTime.Now;
                HseQualityData.Updated_by = WKPUserId;
                HseQualityData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseQualityData == null)
                {
                    _context.HSE_QUALITY_CONTROLs.AddAsync(getHseQualityData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Climate
                var getHseClimateData = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();
                HseClimateData = HseClimateData != null ? HseClimateData : HseClimateData;
                HseClimateData = _mapper.Map<HSE_CLIMATE_CHANGE_AND_AIR_QUALITY>(wkp.HSE_CLIMATE_CHANGE_AND_AIR_QUALITY);
                #region file section
                UploadedDocument GHGFile_File = null;

                if (GHGFile != null)
                {
                    string docName = "GHG";
                    GHGFile_File = _helpersController.UploadDocument(GHGFile, "GHGDocuments");
                    if (GHGFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }

                #endregion

                HseClimateData.GHGFilename = GHGFile != null ? GHGFile_File.fileName : null;
                HseClimateData.GHGFilePath = GHGFile != null ? GHGFile_File.filePath : null;
                HseClimateData.Companyemail = WKPUserEmail;
                HseClimateData.CompanyName = WKPUserName;
                HseClimateData.COMPANY_ID = WKPUserId;
                HseClimateData.Created_by = WKPUserId;
                HseClimateData.Date_Created = DateTime.Now;
                HseClimateData.Date_Updated = DateTime.Now;
                HseClimateData.Updated_by = WKPUserId;
                HseClimateData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseClimateData == null)
                {
                    _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.AddAsync(getHseClimateData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Safety Culture
                var getHseSafetyCultureData = (from c in _context.HSE_SAFETY_CULTURE_TRAININGs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();
                HseSafetyCultureData = HseSafetyCultureData != null ? HseSafetyCultureData : HseSafetyCultureData;
                HseSafetyCultureData = _mapper.Map<HSE_SAFETY_CULTURE_TRAINING>(wkp.HSE_SAFETY_CULTURE_TRAINING);
                #region file section
                UploadedDocument SafetyCurrentYearFile_File = null;
                UploadedDocument SafetyLast2YearsFile_File = null;
                if (SafetyCurrentYearFile != null)
                {
                    string docName = "Safety Current Year File";
                    SafetyCurrentYearFile_File = _helpersController.UploadDocument(SafetyCurrentYearFile, "SafetyCurrentYearDocuments");
                    if (SafetyCurrentYearFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }

                if (SafetyLast2YearsFile != null)
                {
                    string docName = "Safety Last Two Years";
                    SafetyLast2YearsFile_File = _helpersController.UploadDocument(SafetyLast2YearsFile, "SafetyLast2YearsDocuments");
                    if (SafetyLast2YearsFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }

                #endregion


                HseSafetyCultureData.SafetyCurrentYearFilename = SafetyLast2YearsFile != null ? SafetyCurrentYearFile_File.fileName : null;
                HseSafetyCultureData.SafetyCurrentYearFilePath = SafetyLast2YearsFile != null ? SafetyCurrentYearFile_File.filePath : null;
                HseSafetyCultureData.SafetyLast2YearsFilename = SafetyLast2YearsFile != null ? SafetyLast2YearsFile_File.fileName : null;
                HseSafetyCultureData.SafetyLast2YearsFilePath = SafetyLast2YearsFile != null ? SafetyLast2YearsFile_File.filePath : null;
                HseSafetyCultureData.Companyemail = WKPUserEmail;
                HseSafetyCultureData.CompanyName = WKPUserName;
                HseSafetyCultureData.COMPANY_ID = WKPUserId;
                HseSafetyCultureData.Created_by = WKPUserId;
                HseSafetyCultureData.Date_Created = DateTime.Now;
                HseSafetyCultureData.Date_Updated = DateTime.Now;
                HseSafetyCultureData.Updated_by = WKPUserId;
                HseSafetyCultureData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseSafetyCultureData == null)
                {
                    _context.HSE_SAFETY_CULTURE_TRAININGs.AddAsync(getHseSafetyCultureData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Occupational 
                var getHseOccupationalData = (from c in _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();
                HseOccupationalData = HseOccupationalData != null ? HseOccupationalData : HseOccupationalData;
                HseOccupationalData = _mapper.Map<HSE_OCCUPATIONAL_HEALTH_MANAGEMENT>(wkp.HSE_OCCUPATIONAL_HEALTH_MANAGEMENT);

                #region file section
                UploadedDocument OHMplanFile_File = null;
                UploadedDocument OHMplanCommunicationFile_File = null;

                if (OHMplanFile != null)
                {
                    string docName = "Field Discovery";
                    OHMplanFile_File = _helpersController.UploadDocument(OHMplanFile, "FieldDiscoveryDocuments");
                    if (OHMplanFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }
                if (OHMplanCommunicationFile != null)
                {
                    string docName = "Field Discovery";
                    OHMplanCommunicationFile_File = _helpersController.UploadDocument(OHMplanCommunicationFile, "FieldDiscoveryDocuments");
                    if (OHMplanCommunicationFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }

                #endregion
                HseOccupationalData.OHMplanFilename = OHMplanFile != null ? OHMplanFile_File.fileName : null;
                HseOccupationalData.OHMplanFilePath = OHMplanFile != null ? OHMplanFile_File.filePath : null;
                HseOccupationalData.OHMplanCommunicationFilename = OHMplanCommunicationFile != null ? OHMplanCommunicationFile_File.fileName : null;
                HseOccupationalData.OHMplanCommunicationFilePath = OHMplanCommunicationFile != null ? OHMplanCommunicationFile_File.filePath : null;
                HseOccupationalData.Companyemail = WKPUserEmail;
                HseOccupationalData.CompanyName = WKPUserName;
                HseOccupationalData.COMPANY_ID = WKPUserId;
                HseOccupationalData.Created_by = WKPUserId;
                HseOccupationalData.Date_Created = DateTime.Now;
                HseOccupationalData.Date_Updated = DateTime.Now;
                HseOccupationalData.Updated_by = WKPUserId;
                HseOccupationalData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseOccupationalData == null)
                {
                    _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.AddAsync(getHseOccupationalData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Waste Management
                var getHseWasteMgtSysData = (from c in _context.HSE_WASTE_MANAGEMENT_SYSTEMs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();
                HseWasteMgtSysData = HseWasteMgtSysData != null ? HseWasteMgtSysData : HseWasteMgtSysData;
                HseWasteMgtSysData = _mapper.Map<HSE_WASTE_MANAGEMENT_SYSTEM>(wkp.HSE_WASTE_MANAGEMENT_SYSTEM);
                #region file section
                UploadedDocument DecomCertificateFile_File = null;
                UploadedDocument WasteManagementPlanFile_File = null;

                if (DecomCertificateFile != null)
                {
                    string docName = "Decom Certificate";
                    DecomCertificateFile_File = _helpersController.UploadDocument(DecomCertificateFile, "DecomCertificateDocuments");
                    if (DecomCertificateFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }
                if (WasteManagementPlanFile != null)
                {
                    string docName = "Waste Management Plan";
                    WasteManagementPlanFile_File = _helpersController.UploadDocument(WasteManagementPlanFile, "WasteManagementPlanDocuments");
                    if (WasteManagementPlanFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }
                #endregion

                HseWasteMgtSysData.DecomCertificateFilePath = DecomCertificateFile != null ? DecomCertificateFile_File.filePath : null;
                HseWasteMgtSysData.DecomCertificateFilename = DecomCertificateFile != null ? DecomCertificateFile_File.fileName : null;
                HseWasteMgtSysData.WasteManagementPlanFilePath = WasteManagementPlanFile != null ? WasteManagementPlanFile_File.filePath : null;
                HseWasteMgtSysData.WasteManagementPlanFilename = WasteManagementPlanFile != null ? WasteManagementPlanFile_File.fileName : null;
                HseWasteMgtSysData.Companyemail = WKPUserEmail;
                HseWasteMgtSysData.CompanyName = WKPUserName;
                HseWasteMgtSysData.COMPANY_ID = WKPUserId;
                HseWasteMgtSysData.Created_by = WKPUserId;
                HseWasteMgtSysData.Date_Created = DateTime.Now;
                HseWasteMgtSysData.Date_Updated = DateTime.Now;
                HseWasteMgtSysData.Updated_by = WKPUserId;
                HseWasteMgtSysData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseWasteMgtSysData == null)
                {
                    _context.HSE_WASTE_MANAGEMENT_SYSTEMs.AddAsync(getHseWasteMgtSysData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Hse Environmental Management
                var getHseEnvWasteData = (from c in _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();
                HseEnvWasteData = HseEnvWasteData != null ? HseEnvWasteData : HseEnvWasteData;
                HseEnvWasteData = _mapper.Map<HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM>(wkp.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM);
                #region file section
                UploadedDocument EMSFile_File = null;
                UploadedDocument AUDITFile_File = null;

                if (EMSFile != null)
                {
                    string docName = "EMS";
                    EMSFile_File = _helpersController.UploadDocument(EMSFile, "EMSDocuments");
                    if (EMSFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }
                if (AUDITFile != null)
                {
                    string docName = "Audit File";
                    AUDITFile_File = _helpersController.UploadDocument(AUDITFile, "AUDITDocuments");
                    if (AUDITFile_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }
                #endregion


                HseEnvWasteData.EMSFilename = EMSFile != null ? EMSFile_File.fileName : null;
                HseEnvWasteData.EMSFilePath = EMSFile != null ? EMSFile_File.filePath : null;
                HseEnvWasteData.AUDITFilename = AUDITFile != null ? AUDITFile_File.fileName : null;
                HseEnvWasteData.AUDITFilePath = AUDITFile != null ? AUDITFile_File.filePath : null;
                HseEnvWasteData.Companyemail = WKPUserEmail;
                HseEnvWasteData.CompanyName = WKPUserName;
                HseEnvWasteData.COMPANY_ID = WKPUserId;
                HseEnvWasteData.Created_by = WKPUserId;
                HseEnvWasteData.Date_Created = DateTime.Now;
                HseEnvWasteData.Date_Updated = DateTime.Now;
                HseEnvWasteData.Updated_by = WKPUserId;
                HseEnvWasteData.Year_of_WP = DateTime.Now.Year.ToString();
                if (HseEnvWasteData == null)
                {
                    _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs.AddAsync(getHseEnvWasteData);
                }
                save += _context.SaveChanges();
                #endregion
                #region Saving Picture Upload
                var getPictureUploadData = (from c in _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs where c.COMPANY_ID == WKPUserId && c.Year_of_WP == wkp.WorkProgramme_Year select c).FirstOrDefault();
                PictureUploadData = PictureUploadData != null ? PictureUploadData : PictureUploadData;
                PictureUploadData = _mapper.Map<PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT>(wkp.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT);
                #region file section
                UploadedDocument UploadedPresentation_File = null;

                if (UploadedPresentation != null)
                {
                    string docName = "Uploaded Presentation";
                    UploadedPresentation_File = _helpersController.UploadDocument(UploadedPresentation, "Presentations");
                    if (UploadedPresentation_File == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to uploading " + docName + " document.", StatusCode = ResponseCodes.Success };

                }
                #endregion
                PictureUploadData.uploaded_presentation = UploadedPresentation != null ? UploadedPresentation_File.filePath : null;
                PictureUploadData.Companyemail = WKPUserEmail;
                PictureUploadData.CompanyName = WKPUserName;
                PictureUploadData.COMPANY_ID = WKPUserId;
                PictureUploadData.Created_by = WKPUserId;
                PictureUploadData.Date_Created = DateTime.Now;
                PictureUploadData.Date_Updated = DateTime.Now;
                PictureUploadData.Updated_by = WKPUserId;
                PictureUploadData.Year_of_WP = DateTime.Now.Year.ToString();
                if (PictureUploadData == null)
                {
                    _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs.AddAsync(getPictureUploadData);
                }
                save += _context.SaveChanges();
                #endregion


                if (save == 35)
                {
                    string successMsg = "Work programme form 5 has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Badrequest };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };
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
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

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
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }


    }
}