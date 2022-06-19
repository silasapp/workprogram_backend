using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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

        private string? WKPUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string? WKPUserName => User.FindFirstValue(ClaimTypes.Name);
        private string? WKPUserEmail => User.FindFirstValue(ClaimTypes.Email);
        private string? WKPUserRole => User.FindFirstValue(ClaimTypes.Role);


        [HttpPost("POST_WORKPROGRAMME")]
        public async Task<WebApiResponse> Post_WORKPROGRAMME(WorkProgramme_Model wkp)
        {   
            int save = 0;
            var ConcessionData = new CONCESSION_SITUATION();
            var GeophysicalActivitesData = new GEOPHYSICAL_ACTIVITIES_ACQUISITION();
            var GeoActivitesProcessingData = new GEOPHYSICAL_ACTIVITIES_PROCESSING();
            var DrillingOperationsData = new DRILLING_OPERATIONS_CATEGORIES_OF_WELL();
            var DrillingWellCostData = new DRILLING_EACH_WELL_COST();
            var DrillingWellCostProposedData = new DRILLING_EACH_WELL_COST_PROPOSED();

            try
            {
                # region Saving Concession Situations
                var getConcessionData = (from c in _context.CONCESSION_SITUATIONs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                ConcessionData = getConcessionData != null ? getConcessionData : ConcessionData;
                ConcessionData = _mapper.Map<CONCESSION_SITUATION>(wkp.CONCESSION_SITUATION);

                ConcessionData.Companyemail = WKPUserEmail;
                ConcessionData.CompanyName = WKPUserName;
                ConcessionData.COMPANY_ID = WKPUserId;
                ConcessionData.Created_by = WKPUserId;
                ConcessionData.Date_Created = DateTime.Now;
                ConcessionData.Date_Updated = DateTime.Now;
                ConcessionData.Updated_by = WKPUserId;
                ConcessionData.Year = DateTime.Now.Year.ToString();
                if (getConcessionData == null)
                {
                    _context.CONCESSION_SITUATIONs.Add(ConcessionData);
                }
                save = _context.SaveChanges();
                #endregion

                #region Saving Geophysical Activites

                var getGeophysicalActivitesData = (from c in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                GeophysicalActivitesData = getGeophysicalActivitesData != null ? getGeophysicalActivitesData : GeophysicalActivitesData;
                GeophysicalActivitesData = _mapper.Map<GEOPHYSICAL_ACTIVITIES_ACQUISITION>(wkp.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs);

                GeophysicalActivitesData.Companyemail = WKPUserEmail;
                GeophysicalActivitesData.CompanyName = WKPUserName;
                GeophysicalActivitesData.COMPANY_ID = WKPUserId;
                GeophysicalActivitesData.Created_by = WKPUserId;
                GeophysicalActivitesData.Date_Created = DateTime.Now;
                GeophysicalActivitesData.Date_Updated = DateTime.Now;
                GeophysicalActivitesData.Updated_by = WKPUserId;
                GeophysicalActivitesData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getGeophysicalActivitesData == null)
                {
                    _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Add(GeophysicalActivitesData);
                }
                save = _context.SaveChanges();
                #endregion

                #region Saving Geophysical Activites Processing

                var getGeoActivitesProcessingData = (from c in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                GeoActivitesProcessingData = getGeoActivitesProcessingData != null ? getGeoActivitesProcessingData : GeoActivitesProcessingData;
                GeoActivitesProcessingData = _mapper.Map<GEOPHYSICAL_ACTIVITIES_PROCESSING>(wkp.GEOPHYSICAL_ACTIVITIES_PROCESSINGs);

                GeoActivitesProcessingData.Companyemail = WKPUserEmail;
                GeoActivitesProcessingData.CompanyName = WKPUserName;
                GeoActivitesProcessingData.COMPANY_ID = WKPUserId;
                GeoActivitesProcessingData.Created_by = WKPUserId;
                GeoActivitesProcessingData.Date_Created = DateTime.Now;
                GeoActivitesProcessingData.Date_Updated = DateTime.Now;
                GeoActivitesProcessingData.Updated_by = WKPUserId;
                GeoActivitesProcessingData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getGeoActivitesProcessingData == null)
                {
                    _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Add(GeoActivitesProcessingData);
                }
                save = _context.SaveChanges();
                #endregion

                #region Saving Drilling Operations

                var getDrillingOperationsData = (from c in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                DrillingOperationsData = getDrillingOperationsData != null ? getDrillingOperationsData : DrillingOperationsData;
                DrillingOperationsData = _mapper.Map<DRILLING_OPERATIONS_CATEGORIES_OF_WELL>(wkp.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs);

                DrillingOperationsData.Companyemail = WKPUserEmail;
                DrillingOperationsData.CompanyName = WKPUserName;
                DrillingOperationsData.COMPANY_ID = WKPUserId;
                DrillingOperationsData.Created_by = WKPUserId;
                DrillingOperationsData.Date_Created = DateTime.Now;
                DrillingOperationsData.Date_Updated = DateTime.Now;
                DrillingOperationsData.Updated_by = WKPUserId;
                DrillingOperationsData.Year_of_WP = DateTime.Now.Year.ToString();
                if (DrillingOperationsData == null)
                {
                    _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Add(DrillingOperationsData);
                }
                save = _context.SaveChanges();
                #endregion

                #region Saving Drilling Well Costs

                var getDrillingWellCostData = (from c in _context.DRILLING_EACH_WELL_COSTs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                DrillingWellCostData = getDrillingWellCostData != null ? getDrillingWellCostData : DrillingWellCostData;
                DrillingWellCostData = _mapper.Map<DRILLING_EACH_WELL_COST>(wkp.DRILLING_EACH_WELL_COSTs);

                DrillingWellCostData.Companyemail = WKPUserEmail;
                DrillingWellCostData.CompanyName = WKPUserName;
                DrillingWellCostData.COMPANY_ID = WKPUserId;
                DrillingWellCostData.Created_by = WKPUserId;
                DrillingWellCostData.Date_Created = DateTime.Now;
                DrillingWellCostData.Date_Updated = DateTime.Now;
                DrillingWellCostData.Updated_by = WKPUserId;
                DrillingWellCostData.Year_of_WP = DateTime.Now.Year.ToString();
                if (DrillingWellCostData == null)
                {
                    _context.DRILLING_EACH_WELL_COSTs.Add(DrillingWellCostData);
                }
                save = _context.SaveChanges();
                #endregion

                #region Saving Drilling Well Proposed Costs

                var getDrillingWellCostProposedData = (from c in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                DrillingWellCostProposedData = getDrillingWellCostProposedData != null ? getDrillingWellCostProposedData : DrillingWellCostProposedData;
                DrillingWellCostProposedData = _mapper.Map<DRILLING_EACH_WELL_COST_PROPOSED>(wkp.DRILLING_EACH_WELL_COST_PROPOSEDs);

                DrillingWellCostProposedData.Companyemail = WKPUserEmail;
                DrillingWellCostProposedData.CompanyName = WKPUserName;
                DrillingWellCostProposedData.COMPANY_ID = WKPUserId;
                DrillingWellCostProposedData.Created_by = WKPUserId;
                DrillingWellCostProposedData.Date_Created = DateTime.Now;
                DrillingWellCostProposedData.Date_Updated = DateTime.Now;
                DrillingWellCostProposedData.Updated_by = WKPUserId;
                DrillingWellCostProposedData.Year_of_WP = DateTime.Now.Year.ToString();
                if (DrillingWellCostProposedData == null)
                {
                    _context.DRILLING_EACH_WELL_COST_PROPOSEDs.Add(DrillingWellCostProposedData);
                }
                save = _context.SaveChanges();
                #endregion

                if (save == 6)
                {
                    string successMsg = "Work programme form has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data =wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }
        
        [HttpPost("POST_CONCESSION")]
        public async Task<WebApiResponse> Post_CONCESSION_SITUATION(CONCESSION_SITUATION_Model wkp)
        {   
            int save = 0;
            var ConcessionData = new CONCESSION_SITUATION();
           
            try
            {
                # region Saving Concession Situations
                var getConcessionData = (from c in _context.CONCESSION_SITUATIONs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                ConcessionData = getConcessionData != null ? getConcessionData : ConcessionData;
                ConcessionData = _mapper.Map<CONCESSION_SITUATION>(wkp);

                ConcessionData.Companyemail = WKPUserEmail;
                ConcessionData.CompanyName = WKPUserName;
                ConcessionData.COMPANY_ID = WKPUserId;
                ConcessionData.Created_by = WKPUserId;
                ConcessionData.Date_Created = DateTime.Now;
                ConcessionData.Date_Updated = DateTime.Now;
                ConcessionData.Updated_by = WKPUserId;
                ConcessionData.Year = DateTime.Now.Year.ToString();
                if (getConcessionData == null)
                {
                    _context.CONCESSION_SITUATIONs.Add(ConcessionData);
                }
                save = _context.SaveChanges();
                #endregion

                
                if (save > 0)
                {
                    string successMsg = "Form has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data =wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }

        [HttpPost("POST_GEOPHYSICAL_ACQUISITION")]
        public async Task<WebApiResponse> Post_GEOPHYSICAL_ACTIVITIES_ACQUISITION(GEOPHYSICAL_ACTIVITIES_ACQUISITION_Model wkp)
        {   
            int save = 0;
            var GeophysicalActivitesData = new GEOPHYSICAL_ACTIVITIES_ACQUISITION();
           
            try
            {

                #region Saving Geophysical Activites

                var getGeophysicalActivitesData = (from c in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                GeophysicalActivitesData = getGeophysicalActivitesData != null ? getGeophysicalActivitesData : GeophysicalActivitesData;
                GeophysicalActivitesData = _mapper.Map<GEOPHYSICAL_ACTIVITIES_ACQUISITION>(wkp);

                GeophysicalActivitesData.Companyemail = WKPUserEmail;
                GeophysicalActivitesData.CompanyName = WKPUserName;
                GeophysicalActivitesData.COMPANY_ID = WKPUserId;
                GeophysicalActivitesData.Created_by = WKPUserId;
                GeophysicalActivitesData.Date_Created = DateTime.Now;
                GeophysicalActivitesData.Date_Updated = DateTime.Now;
                GeophysicalActivitesData.Updated_by = WKPUserId;
                GeophysicalActivitesData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getGeophysicalActivitesData == null)
                {
                    _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Add(GeophysicalActivitesData);
                }
                save = _context.SaveChanges();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data =wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }

        [HttpPost("POST_GEOPHYSICAL_ACTIVITIES_PROCESSING")]
        public async Task<WebApiResponse> Post_GEOPHYSICAL_ACTIVITIES_PROCESSING(GEOPHYSICAL_ACTIVITIES_PROCESSING_Model wkp)
        {   
            int save = 0;
            var GeoActivitesProcessingData = new GEOPHYSICAL_ACTIVITIES_PROCESSING();
         
            try
            {
               
                #region Saving Geophysical Activites Processing

                var getGeoActivitesProcessingData = (from c in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                GeoActivitesProcessingData = getGeoActivitesProcessingData != null ? getGeoActivitesProcessingData : GeoActivitesProcessingData;
                GeoActivitesProcessingData = _mapper.Map<GEOPHYSICAL_ACTIVITIES_PROCESSING>(wkp);

                GeoActivitesProcessingData.Companyemail = WKPUserEmail;
                GeoActivitesProcessingData.CompanyName = WKPUserName;
                GeoActivitesProcessingData.COMPANY_ID = WKPUserId;
                GeoActivitesProcessingData.Created_by = WKPUserId;
                GeoActivitesProcessingData.Date_Created = DateTime.Now;
                GeoActivitesProcessingData.Date_Updated = DateTime.Now;
                GeoActivitesProcessingData.Updated_by = WKPUserId;
                GeoActivitesProcessingData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getGeoActivitesProcessingData == null)
                {
                    _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Add(GeoActivitesProcessingData);
                }
                save = _context.SaveChanges();
                #endregion


                if (save > 0)
                {
                    string successMsg = "Form has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data =wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }

        [HttpPost("DRILLING_OPERATIONS_CATEGORIES_OF_WELL")]
        public async Task<WebApiResponse> Post_DRILLING_OPERATIONS(DRILLING_OPERATIONS_CATEGORIES_OF_WELL_Model wkp)
        {   
            int save = 0;
            var DrillingOperationsData = new DRILLING_OPERATIONS_CATEGORIES_OF_WELL();
           
            try
            {

                #region Saving Drilling Operations

                var getDrillingOperationsData = (from c in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                DrillingOperationsData = getDrillingOperationsData != null ? getDrillingOperationsData : DrillingOperationsData;
                DrillingOperationsData = _mapper.Map<DRILLING_OPERATIONS_CATEGORIES_OF_WELL>(wkp);

                DrillingOperationsData.Companyemail = WKPUserEmail;
                DrillingOperationsData.CompanyName = WKPUserName;
                DrillingOperationsData.COMPANY_ID = WKPUserId;
                DrillingOperationsData.Created_by = WKPUserId;
                DrillingOperationsData.Date_Created = DateTime.Now;
                DrillingOperationsData.Date_Updated = DateTime.Now;
                DrillingOperationsData.Updated_by = WKPUserId;
                DrillingOperationsData.Year_of_WP = DateTime.Now.Year.ToString();
                if (DrillingOperationsData == null)
                {
                    _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Add(DrillingOperationsData);
                }
                save = _context.SaveChanges();
                #endregion

                
                if (save > 0)
                {
                    string successMsg = "Form has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data =wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }

        [HttpPost("POST_DRILLING_EACH_WELL_COST")]
        public async Task<WebApiResponse> Post_DRILLING_EACH_WELL_COST(DRILLING_EACH_WELL_COST_Model wkp)
        {   
            int save = 0;
            var DrillingWellCostData = new DRILLING_EACH_WELL_COST();

            try
            {

                #region Saving Drilling Well Costs

                var getDrillingWellCostData = (from c in _context.DRILLING_EACH_WELL_COSTs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                DrillingWellCostData = getDrillingWellCostData != null ? getDrillingWellCostData : DrillingWellCostData;
                DrillingWellCostData = _mapper.Map<DRILLING_EACH_WELL_COST>(wkp);

                DrillingWellCostData.Companyemail = WKPUserEmail;
                DrillingWellCostData.CompanyName = WKPUserName;
                DrillingWellCostData.COMPANY_ID = WKPUserId;
                DrillingWellCostData.Created_by = WKPUserId;
                DrillingWellCostData.Date_Created = DateTime.Now;
                DrillingWellCostData.Date_Updated = DateTime.Now;
                DrillingWellCostData.Updated_by = WKPUserId;
                DrillingWellCostData.Year_of_WP = DateTime.Now.Year.ToString();
                if (DrillingWellCostData == null)
                {
                    _context.DRILLING_EACH_WELL_COSTs.Add(DrillingWellCostData);
                }
                save = _context.SaveChanges();
                #endregion


                if (save > 0)
                {
                    string successMsg = "Form has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data =wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }

        [HttpPost("POST_WELL_PROPOSED")]
        public async Task<WebApiResponse> Post_DRILLING_WELL_PROPOSED(WorkProgramme_Model wkp)
        {

            int save = 0;
            var DrillingWellCostProposedData = new DRILLING_EACH_WELL_COST_PROPOSED();

            try
            {
            
                #region Saving Drilling Well Proposed Costs

                var getDrillingWellCostProposedData = (from c in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where c.COMPANY_ID == WKPUserId select c).FirstOrDefault();

                DrillingWellCostProposedData = getDrillingWellCostProposedData != null ? getDrillingWellCostProposedData : DrillingWellCostProposedData;
                DrillingWellCostProposedData = _mapper.Map<DRILLING_EACH_WELL_COST_PROPOSED>(wkp.DRILLING_EACH_WELL_COST_PROPOSEDs);

                DrillingWellCostProposedData.Companyemail = WKPUserEmail;
                DrillingWellCostProposedData.CompanyName = WKPUserName;
                DrillingWellCostProposedData.COMPANY_ID = WKPUserId;
                DrillingWellCostProposedData.Created_by = WKPUserId;
                DrillingWellCostProposedData.Date_Created = DateTime.Now;
                DrillingWellCostProposedData.Date_Updated = DateTime.Now;
                DrillingWellCostProposedData.Updated_by = WKPUserId;
                DrillingWellCostProposedData.Year_of_WP = DateTime.Now.Year.ToString();
                if (DrillingWellCostProposedData == null)
                {
                    _context.DRILLING_EACH_WELL_COST_PROPOSEDs.Add(DrillingWellCostProposedData);
                }
                save = _context.SaveChanges();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data =wkp, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Success };

                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }

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
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

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
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }

        #region work program admin     
        [HttpGet("OPL_RECALIBRATED_SCALED")]
        public async Task<WebApiResponse> opl_recalibrated(string year)
        {
            var details = new List<WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANy>();
            try
            {
                if (WKPUserRole == GeneralModel.Admin)
                {

                    details =await  _context.WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c=> c.Year_of_WP == year).ToListAsync();
                }
                else
                {
                    details =await  _context.WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPUserName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
                }
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Success };
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
                if (WKPUserRole == GeneralModel.Admin)
                {
                    details =await  _context.WP_OPL_Aggregated_Score_ALL_COMPANIEs.Where(c => c.Year_of_WP == year).ToListAsync();

                }
                else
                {
                    details =await  _context.WP_OPL_Aggregated_Score_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPUserName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
                }
                
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

        }
        [HttpGet("OML_RECALIBRATED_SCALED")]
        public async Task<WebApiResponse> OML_RECALIBRATED_SCALE(string year)
        {
            var details = new List<WP_OML_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANy>();
            try
            {
                if (WKPUserRole == GeneralModel.Admin)
                {   
                    details =await  _context.WP_OML_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c=> c.Year_of_WP == year).ToListAsync();
                }
                else
                {
                    details =await  _context.WP_OML_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPUserName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
                }
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Success };
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
                if (WKPUserRole == GeneralModel.Admin)
                {
                    details =await  _context.WP_OML_Aggregated_Score_ALL_COMPANIEs.Where(c => c.Year_of_WP == year).ToListAsync();

                }
                else
                {
                    details =await  _context.WP_OML_Aggregated_Score_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPUserName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
                }
                
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

        }
        
       
        #endregion

    }
}
