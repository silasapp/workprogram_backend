using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

//        private string? WKPCompanyId => User.FindFirstValue(ClaimTypes.NameIdentifier);
//        private string? WKPCompanyName => User.FindFirstValue(ClaimTypes.Name);
//        private string? WKPCompanyEmail => User.FindFirstValue(ClaimTypes.Email);
//        private string? WKPRole => User.FindFirstValue(ClaimTypes.Role);


//        [HttpPost("POST_WORKPROGRAMME")]
//        public async Task<WebApiResponse> Post_WORKPROGRAMME(WorkProgramme_Model wkp)
//        {

//            var userName = "test";
//            var userEmail = "test@mailinator.com";
//            var companyID = "NND/001";
//            int save = 0;
//            var ConcessionData = new CONCESSION_SITUATION();
//            var GeophysicalActivitesData = new GEOPHYSICAL_ACTIVITIES_ACQUISITION();
//            var GeoActivitesProcessingData = new GEOPHYSICAL_ACTIVITIES_PROCESSING();
//            var DrillingOperationsData = new DRILLING_OPERATIONS_CATEGORIES_OF_WELL();
//            var DrillingWellCostData = new DRILLING_EACH_WELL_COST();
//            var DrillingWellCostProposedData = new DRILLING_EACH_WELL_COST_PROPOSED();

//            try
//            {
//                # region Saving Concession Situations
//                var getConcessionData = (from c in _context.CONCESSION_SITUATIONs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                ConcessionData = getConcessionData != null ? getConcessionData : ConcessionData;
//                ConcessionData = _mapper.Map<CONCESSION_SITUATION>(wkp.CONCESSION_SITUATION);

//                ConcessionData.Companyemail = userEmail;
//                ConcessionData.CompanyName = userName;
//                ConcessionData.COMPANY_ID = companyID;
//                ConcessionData.Created_by = companyID;
//                ConcessionData.Date_Created = DateTime.Now;
//                ConcessionData.Date_Updated = DateTime.Now;
//                ConcessionData.Updated_by = companyID;
//                ConcessionData.Year = DateTime.Now.Year.ToString();
//                if (getConcessionData == null)
//                {
//                    _context.CONCESSION_SITUATIONs.Add(ConcessionData);
//                }
//                save = _context.SaveChanges();
//                #endregion

//                #region Saving Geophysical Activites

//                var getGeophysicalActivitesData = (from c in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                GeophysicalActivitesData = getGeophysicalActivitesData != null ? getGeophysicalActivitesData : GeophysicalActivitesData;
//                GeophysicalActivitesData = _mapper.Map<GEOPHYSICAL_ACTIVITIES_ACQUISITION>(wkp.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs);

//                GeophysicalActivitesData.Companyemail = userEmail;
//                GeophysicalActivitesData.CompanyName = userName;
//                GeophysicalActivitesData.COMPANY_ID = companyID;
//                GeophysicalActivitesData.Created_by = companyID;
//                GeophysicalActivitesData.Date_Created = DateTime.Now;
//                GeophysicalActivitesData.Date_Updated = DateTime.Now;
//                GeophysicalActivitesData.Updated_by = companyID;
//                GeophysicalActivitesData.Year_of_WP = DateTime.Now.Year.ToString();
//                if (getGeophysicalActivitesData == null)
//                {
//                    _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Add(GeophysicalActivitesData);
//                }
//                save = _context.SaveChanges();
//                #endregion

//                #region Saving Geophysical Activites Processing

//                var getGeoActivitesProcessingData = (from c in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                GeoActivitesProcessingData = getGeoActivitesProcessingData != null ? getGeoActivitesProcessingData : GeoActivitesProcessingData;
//                GeoActivitesProcessingData = _mapper.Map<GEOPHYSICAL_ACTIVITIES_PROCESSING>(wkp.GEOPHYSICAL_ACTIVITIES_PROCESSINGs);

//                GeoActivitesProcessingData.Companyemail = userEmail;
//                GeoActivitesProcessingData.CompanyName = userName;
//                GeoActivitesProcessingData.COMPANY_ID = companyID;
//                GeoActivitesProcessingData.Created_by = companyID;
//                GeoActivitesProcessingData.Date_Created = DateTime.Now;
//                GeoActivitesProcessingData.Date_Updated = DateTime.Now;
//                GeoActivitesProcessingData.Updated_by = companyID;
//                GeoActivitesProcessingData.Year_of_WP = DateTime.Now.Year.ToString();
//                if (getGeoActivitesProcessingData == null)
//                {
//                    _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Add(GeoActivitesProcessingData);
//                }
//                save = _context.SaveChanges();
//                #endregion

//                #region Saving Drilling Operations

//                var getDrillingOperationsData = (from c in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                DrillingOperationsData = getDrillingOperationsData != null ? getDrillingOperationsData : DrillingOperationsData;
//                DrillingOperationsData = _mapper.Map<DRILLING_OPERATIONS_CATEGORIES_OF_WELL>(wkp.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs);

//                DrillingOperationsData.Companyemail = userEmail;
//                DrillingOperationsData.CompanyName = userName;
//                DrillingOperationsData.COMPANY_ID = companyID;
//                DrillingOperationsData.Created_by = companyID;
//                DrillingOperationsData.Date_Created = DateTime.Now;
//                DrillingOperationsData.Date_Updated = DateTime.Now;
//                DrillingOperationsData.Updated_by = companyID;
//                DrillingOperationsData.Year_of_WP = DateTime.Now.Year.ToString();
//                if (DrillingOperationsData == null)
//                {
//                    _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Add(DrillingOperationsData);
//                }
//                save = _context.SaveChanges();
//                #endregion

//                #region Saving Drilling Well Costs

//                var getDrillingWellCostData = (from c in _context.DRILLING_EACH_WELL_COSTs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                DrillingWellCostData = getDrillingWellCostData != null ? getDrillingWellCostData : DrillingWellCostData;
//                DrillingWellCostData = _mapper.Map<DRILLING_EACH_WELL_COST>(wkp.DRILLING_EACH_WELL_COSTs);

//                DrillingWellCostData.Companyemail = userEmail;
//                DrillingWellCostData.CompanyName = userName;
//                DrillingWellCostData.COMPANY_ID = companyID;
//                DrillingWellCostData.Created_by = companyID;
//                DrillingWellCostData.Date_Created = DateTime.Now;
//                DrillingWellCostData.Date_Updated = DateTime.Now;
//                DrillingWellCostData.Updated_by = companyID;
//                DrillingWellCostData.Year_of_WP = DateTime.Now.Year.ToString();
//                if (DrillingWellCostData == null)
//                {
//                    _context.DRILLING_EACH_WELL_COSTs.Add(DrillingWellCostData);
//                }
//                save = _context.SaveChanges();
//                #endregion

//                #region Saving Drilling Well Proposed Costs

//                var getDrillingWellCostProposedData = (from c in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                DrillingWellCostProposedData = getDrillingWellCostProposedData != null ? getDrillingWellCostProposedData : DrillingWellCostProposedData;
//                DrillingWellCostProposedData = _mapper.Map<DRILLING_EACH_WELL_COST_PROPOSED>(wkp.DRILLING_EACH_WELL_COST_PROPOSEDs);

//                DrillingWellCostProposedData.Companyemail = userEmail;
//                DrillingWellCostProposedData.CompanyName = userName;
//                DrillingWellCostProposedData.COMPANY_ID = companyID;
//                DrillingWellCostProposedData.Created_by = companyID;
//                DrillingWellCostProposedData.Date_Created = DateTime.Now;
//                DrillingWellCostProposedData.Date_Updated = DateTime.Now;
//                DrillingWellCostProposedData.Updated_by = companyID;
//                DrillingWellCostProposedData.Year_of_WP = DateTime.Now.Year.ToString();
//                if (DrillingWellCostProposedData == null)
//                {
//                    _context.DRILLING_EACH_WELL_COST_PROPOSEDs.Add(DrillingWellCostProposedData);
//                }
//                save = _context.SaveChanges();
//                #endregion

//                if (save == 6)
//                {
//                    string successMsg = "Work programme form has been submitted successfully.";
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
//                }
//                else
//                {
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

//                }

//            }
//            catch (Exception e)
//            {
//                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

//            }
//        }

//        [HttpPost("POST_CONCESSION")]
//        public async Task<WebApiResponse> Post_CONCESSION_SITUATION(CONCESSION_SITUATION wkp)
//        {

//            var userName = "test";
//            var userEmail = "test@mailinator.com";
//            var companyID = "NND/001";
//            int save = 0;
//            var ConcessionData = new CONCESSION_SITUATION();

//            try
//            {
//                # region Saving Concession Situations
//                var getConcessionData = (from c in _context.CONCESSION_SITUATIONs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                ConcessionData = getConcessionData != null ? getConcessionData : ConcessionData;
//                ConcessionData = _mapper.Map<CONCESSION_SITUATION>(wkp);

//                ConcessionData.Companyemail = userEmail;
//                ConcessionData.CompanyName = userName;
//                ConcessionData.COMPANY_ID = companyID;
//                ConcessionData.Created_by = companyID;
//                ConcessionData.Date_Created = DateTime.Now;
//                ConcessionData.Date_Updated = DateTime.Now;
//                ConcessionData.Updated_by = companyID;
//                ConcessionData.Year = DateTime.Now.Year.ToString();
//                if (getConcessionData == null)
//                {
//                    _context.CONCESSION_SITUATIONs.Add(ConcessionData);
//                }
//                save = _context.SaveChanges();
//                #endregion


//                if (save > 0)
//                {
//                    string successMsg = "Form has been submitted successfully.";
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
//                }
//                else
//                {
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

//                }

//            }
//            catch (Exception e)
//            {
//                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

//            }
//        }


//        [HttpPost("POST_GEOPHYSICAL_ACQUISITION")]
//        public async Task<WebApiResponse> Post_GEOPHYSICAL_ACTIVITIES_ACQUISITION(GEOPHYSICAL_ACTIVITIES_ACQUISITION wkp)
//        {

//            var userName = "test";
//            var userEmail = "test@mailinator.com";
//            var companyID = "NND/001";
//            int save = 0;
//            var GeophysicalActivitesData = new GEOPHYSICAL_ACTIVITIES_ACQUISITION();

//            try
//            {

//                #region Saving Geophysical Activites

//                var getGeophysicalActivitesData = (from c in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                GeophysicalActivitesData = getGeophysicalActivitesData != null ? getGeophysicalActivitesData : GeophysicalActivitesData;
//                GeophysicalActivitesData = _mapper.Map<GEOPHYSICAL_ACTIVITIES_ACQUISITION>(wkp);

//                GeophysicalActivitesData.Companyemail = userEmail;
//                GeophysicalActivitesData.CompanyName = userName;
//                GeophysicalActivitesData.COMPANY_ID = companyID;
//                GeophysicalActivitesData.Created_by = companyID;
//                GeophysicalActivitesData.Date_Created = DateTime.Now;
//                GeophysicalActivitesData.Date_Updated = DateTime.Now;
//                GeophysicalActivitesData.Updated_by = companyID;
//                GeophysicalActivitesData.Year_of_WP = DateTime.Now.Year.ToString();
//                if (getGeophysicalActivitesData == null)
//                {
//                    _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Add(GeophysicalActivitesData);
//                }
//                save = _context.SaveChanges();
//                #endregion

//                if (save > 0)
//                {
//                    string successMsg = "Form has been submitted successfully.";
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
//                }
//                else
//                {
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

//                }

//            }
//            catch (Exception e)
//            {
//                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

//            }
//        }

//        [HttpPost("POST_GEOPHYSICAL_ACTIVITIES_PROCESSING")]
//        public async Task<WebApiResponse> Post_GEOPHYSICAL_ACTIVITIES_PROCESSING(GEOPHYSICAL_ACTIVITIES_PROCESSING wkp)
//        {

//            var userName = "test";
//            var userEmail = "test@mailinator.com";
//            var companyID = "NND/001";
//            int save = 0;
//            var GeoActivitesProcessingData = new GEOPHYSICAL_ACTIVITIES_PROCESSING();

//            try
//            {

//                #region Saving Geophysical Activites Processing

//                var getGeoActivitesProcessingData = (from c in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                GeoActivitesProcessingData = getGeoActivitesProcessingData != null ? getGeoActivitesProcessingData : GeoActivitesProcessingData;
//                GeoActivitesProcessingData = _mapper.Map<GEOPHYSICAL_ACTIVITIES_PROCESSING>(wkp);

//                GeoActivitesProcessingData.Companyemail = userEmail;
//                GeoActivitesProcessingData.CompanyName = userName;
//                GeoActivitesProcessingData.COMPANY_ID = companyID;
//                GeoActivitesProcessingData.Created_by = companyID;
//                GeoActivitesProcessingData.Date_Created = DateTime.Now;
//                GeoActivitesProcessingData.Date_Updated = DateTime.Now;
//                GeoActivitesProcessingData.Updated_by = companyID;
//                GeoActivitesProcessingData.Year_of_WP = DateTime.Now.Year.ToString();
//                if (getGeoActivitesProcessingData == null)
//                {
//                    _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Add(GeoActivitesProcessingData);
//                }
//                save = _context.SaveChanges();
//                #endregion


//                if (save > 0)
//                {
//                    string successMsg = "Form has been submitted successfully.";
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
//                }
//                else
//                {
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

//                }

//            }
//            catch (Exception e)
//            {
//                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

//            }
//        }

//        [HttpPost("DRILLING_OPERATIONS_CATEGORIES_OF_WELL")]
//        public async Task<WebApiResponse> Post_DRILLING_OPERATIONS(DRILLING_OPERATIONS_CATEGORIES_OF_WELL wkp)
//        {

//            var userName = "test";
//            var userEmail = "test@mailinator.com";
//            var companyID = "NND/001";
//            int save = 0;
//            var DrillingOperationsData = new DRILLING_OPERATIONS_CATEGORIES_OF_WELL();

//            try
//            {

//                #region Saving Drilling Operations

//                var getDrillingOperationsData = (from c in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                DrillingOperationsData = getDrillingOperationsData != null ? getDrillingOperationsData : DrillingOperationsData;
//                DrillingOperationsData = _mapper.Map<DRILLING_OPERATIONS_CATEGORIES_OF_WELL>(wkp);

//                DrillingOperationsData.Companyemail = userEmail;
//                DrillingOperationsData.CompanyName = userName;
//                DrillingOperationsData.COMPANY_ID = companyID;
//                DrillingOperationsData.Created_by = companyID;
//                DrillingOperationsData.Date_Created = DateTime.Now;
//                DrillingOperationsData.Date_Updated = DateTime.Now;
//                DrillingOperationsData.Updated_by = companyID;
//                DrillingOperationsData.Year_of_WP = DateTime.Now.Year.ToString();
//                if (DrillingOperationsData == null)
//                {
//                    _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Add(DrillingOperationsData);
//                }
//                save = _context.SaveChanges();
//                #endregion


//                if (save > 0)
//                {
//                    string successMsg = "Form has been submitted successfully.";
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
//                }
//                else
//                {
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

//                }

//            }
//            catch (Exception e)
//            {
//                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

//            }
//        }

//        [HttpPost("POST_DRILLING_EACH_WELL_COST")]
//        public async Task<WebApiResponse> Post_DRILLING_EACH_WELL_COST(DRILLING_EACH_WELL_COST wkp)
//        {

//            var userName = "test";
//            var userEmail = "test@mailinator.com";
//            var companyID = "NND/001";
//            int save = 0;
//            var DrillingWellCostData = new DRILLING_EACH_WELL_COST();

//            try
//            {

//                #region Saving Drilling Well Costs

//                var getDrillingWellCostData = (from c in _context.DRILLING_EACH_WELL_COSTs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                DrillingWellCostData = getDrillingWellCostData != null ? getDrillingWellCostData : DrillingWellCostData;
//                DrillingWellCostData = _mapper.Map<DRILLING_EACH_WELL_COST>(wkp);

//                DrillingWellCostData.Companyemail = userEmail;
//                DrillingWellCostData.CompanyName = userName;
//                DrillingWellCostData.COMPANY_ID = companyID;
//                DrillingWellCostData.Created_by = companyID;
//                DrillingWellCostData.Date_Created = DateTime.Now;
//                DrillingWellCostData.Date_Updated = DateTime.Now;
//                DrillingWellCostData.Updated_by = companyID;
//                DrillingWellCostData.Year_of_WP = DateTime.Now.Year.ToString();
//                if (DrillingWellCostData == null)
//                {
//                    _context.DRILLING_EACH_WELL_COSTs.Add(DrillingWellCostData);
//                }
//                save = _context.SaveChanges();
//                #endregion


//                if (save > 0)
//                {
//                    string successMsg = "Form has been submitted successfully.";
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
//                }
//                else
//                {
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

//                }

//            }
//            catch (Exception e)
//            {
//                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

//            }
//        }

//        [HttpPost("POST_WELL_PROPOSED")]
//        public async Task<WebApiResponse> Post_DRILLING_WELL_PROPOSED(WorkProgramme_Model wkp)
//        {

//            var userName = "test";
//            var userEmail = "test@mailinator.com";
//            var companyID = "NND/001";
//            int save = 0;
//            var DrillingWellCostProposedData = new DRILLING_EACH_WELL_COST_PROPOSED();

//            try
//            {

//                #region Saving Drilling Well Proposed Costs

//                var getDrillingWellCostProposedData = (from c in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where c.COMPANY_ID == companyID select c).FirstOrDefault();

//                DrillingWellCostProposedData = getDrillingWellCostProposedData != null ? getDrillingWellCostProposedData : DrillingWellCostProposedData;
//                DrillingWellCostProposedData = _mapper.Map<DRILLING_EACH_WELL_COST_PROPOSED>(wkp.DRILLING_EACH_WELL_COST_PROPOSEDs);

//                DrillingWellCostProposedData.Companyemail = userEmail;
//                DrillingWellCostProposedData.CompanyName = userName;
//                DrillingWellCostProposedData.COMPANY_ID = companyID;
//                DrillingWellCostProposedData.Created_by = companyID;
//                DrillingWellCostProposedData.Date_Created = DateTime.Now;
//                DrillingWellCostProposedData.Date_Updated = DateTime.Now;
//                DrillingWellCostProposedData.Updated_by = companyID;
//                DrillingWellCostProposedData.Year_of_WP = DateTime.Now.Year.ToString();
//                if (DrillingWellCostProposedData == null)
//                {
//                    _context.DRILLING_EACH_WELL_COST_PROPOSEDs.Add(DrillingWellCostProposedData);
//                }
//                save = _context.SaveChanges();
//                #endregion

//                if (save > 0)
//                {
//                    string successMsg = "Form has been submitted successfully.";
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
//                }
//                else
//                {
//                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

//                }

//            }
//            catch (Exception e)
//            {
//                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

//            }
//        }

//        [HttpGet("PRESENTATION SCHEDULES")]
//        public async Task<WebApiResponse> PRESENTATION_SCHEDULES(string year)
//        {
//            var userEmail = "test@mailinator.com";
//            var companyID = "NND/001";
//            try
//            {
//                var schedules = (from sch in _context.ADMIN_DATETIME_PRESENTATIONs select sch).ToList();
//                var viewYears = schedules.Select(x => x.YEAR).Distinct().ToList();

//                if (year != null)
//                {
//                    schedules = schedules.Where(x => x.YEAR == year).ToList();
//                }
//                var presentationSchedules = new PresentationSchedules_Model()
//                {
//                    presentationSchedules = schedules,
//                    Years = viewYears
//                };

//                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = presentationSchedules, StatusCode = ResponseCodes.Success };
//            }            
//            catch (Exception e)
//            {
//                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

//            }
//        }
//         [HttpGet("DIVISIONAL_PRESENTATIONS")]
//        public async Task<WebApiResponse> DIVISIONAL_PRESENTATIONS(string year)
//        {
//            var userEmail = "test@mailinator.com";
//            var companyID = "NND/001";
//            try
//            {
//                var presentations = (from sch in _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs select sch).ToList();
//                var viewYears = presentations.Select(x => x.YEAR).Distinct().ToList();

//                if (year != null)
//                {
//                    presentations = presentations.Where(x => x.YEAR == year).ToList();
//                }
//                var presentationDivision = new PresentationSchedules_Model()
//                {
//                    Divisionpresentations = presentations,
//                    Years = viewYears
//                };

//                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = presentationDivision, StatusCode = ResponseCodes.Success };
//            }            
//            catch (Exception e)
//            {
//                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

//            }
//        }

//        #region work program admin     
//        [HttpGet("OPL-RECLIBRATED-SCALED")]
//        public async Task<WebApiResponse> opl_reclibrated(string year = null)
//        {
//            var userRole = "Admin";
//            var userEmail = "test@mailinator.com";
//            var companyID = "NND/001";

//            var details = new List<WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANy>();
//            try
//            {
//                if (userRole == GeneralModel.Admin)
//                {

//                    details = _context.WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.ToList();
//                }
//                else
//                {
//                    //details = _context.WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.CompanyID == companyID).ToList();
//                }
//                if (year != null)
//                {
//                    details = details.Where(c => c.Year_of_WP == year).ToList();
//                }
//            }
//            catch (Exception ex)
//            {

//                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Success };
//            }

//            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };

//        }
//        [HttpGet("OPL-AGGREGATED-SCORE(%))")]
//        public async Task<WebApiResponse> opl_aggregated_score(string year = null)
//        {
//            var userRole = "Admin";
//            var userEmail = "test@mailinator.com";
//            var companyID = "NND/001";
//            var presentYear = DateTime.Now.Year;

//            var details = new List<WP_OPL_Aggregated_Score_ALL_COMPANy>();
//            try
//            {
//                if (userRole == GeneralModel.Admin)
//                {

//                    details = _context.WP_OPL_Aggregated_Score_ALL_COMPANIEs.ToList();
//                }
//                else
//                {
//                    //details = _context.WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.CompanyID == companyID).ToList();
//                }
//                if (year != null)
//                {
//                    details = details.Where(c => c.Year_of_WP == year).ToList();
//                }
//            }
//            catch (Exception ex)
//            {

//                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Success };
//            }

//            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };

//        }
//        //[HttpGet(Name = "OML-RECLIBRATED-SCALED")]
//        //public async Task<WebApiResponse> oml_reclibrated(string year = null)
//        //{
//        //    try
//        //    {

//        //    }
//        //    catch (Exception ex)
//        //    {

//        //        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Success };
//        //    }

//        //}


//        #endregion

    }
}
