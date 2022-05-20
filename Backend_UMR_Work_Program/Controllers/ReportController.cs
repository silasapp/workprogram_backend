using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using static Backend_UMR_Work_Program.Models.GeneralModel;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ReportController : ControllerBase
    {
    private Account _account;
    public WKP_DBContext _context;
    public IConfiguration _configuration;
    HelpersController _helpersController;
    IHttpContextAccessor _httpContextAccessor;
    public ReportController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController, Account account)
    {
        //_httpContextAccessor = httpContextAccessor;
        _account = account;
        _context = context;
        _configuration = configuration;

        _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        //_helpersController = new HelpersController(_context, _configuration);
    }


        [HttpGet(Name = "CONCESSIONSINFORMATION")]
        public async Task<WebApiResponse> Get_ADMIN_CONCESSIONS_INFORMATION_BY_CURRENT_YEAR(string year = null)
        {

            //var userRole = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionRoleName));
            //var userEmail = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionEmail));
            //var companyID = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionUserID));

            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var dateYear = DateTime.Now.AddYears(0).ToString("yyyy");
            var ConcessionsInformation = new List<ADMIN_CONCESSIONS_INFORMATION>();

            if (userRole == GeneralModel.Admin)
            {
                ConcessionsInformation = _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(c => c.Year == dateYear && c.DELETED_STATUS == null).ToList();
            }
            else
            {
                ConcessionsInformation = _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(c => c.Year == dateYear && c.Company_ID == companyID && c.DELETED_STATUS == null).ToList();
            }

            if (year != null)
            {
                ConcessionsInformation = _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(c => c.Year == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ConcessionsInformation, StatusCode = ResponseCodes.Success };

        }

        [HttpGet(Name = "CONCESSIONSITUATION")]
        public async Task<WebApiResponse> Get_CONCESSION_SITUATION(string year)
        {

            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ConcessionSituation = new List<CONCESSION_SITUATION>();

            if (userRole == GeneralModel.Admin)
            {
                ConcessionSituation = _context.CONCESSION_SITUATIONs.Where(c => c.Year == year).ToList();
            }
            else
            {
                ConcessionSituation = _context.CONCESSION_SITUATIONs.Where(c => c.Year == year && c.COMPANY_ID == companyID).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ConcessionSituation.OrderBy(x => x.Year), StatusCode = ResponseCodes.Success };

        }
        
        [HttpGet(Name = "GEOPHYSICALACTIVITIES")]
        public async Task<WebApiResponse> Get_GEOPHYSICAL_ACTIVITIES_ACQUISITION(string year)
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var GEOPHYSICALACTIVITIES = new List<GEOPHYSICAL_ACTIVITIES_ACQUISITION>();

            if (userRole == GeneralModel.Admin)
            {
                GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == companyID).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GEOPHYSICALACTIVITIES.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };

        }
      
      
        [HttpGet(Name = "GEOPHYSICALPROCESSING")]
        public async Task<WebApiResponse> Get_GEOPHYSICAL_ACTIVITIES_PROCESSING(string year)
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var GEOPHYSICALACTIVITIES = new List<GEOPHYSICAL_ACTIVITIES_PROCESSING>();

            if (userRole == GeneralModel.Admin)
            {
                GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == companyID).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GEOPHYSICALACTIVITIES.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };

        }

        [HttpGet(Name = "DRILLING-OPERATIONS")]
        public async Task<WebApiResponse> Get_DRILLING_OPERATIONS_CATEGORIES_OF_WELLS(string year)
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var DRILLING_OPERATIONS = new List<DRILLING_OPERATIONS_CATEGORIES_OF_WELL>();

            if (userRole == GeneralModel.Admin)
            {
                DRILLING_OPERATIONS = _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                DRILLING_OPERATIONS = _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == companyID).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = DRILLING_OPERATIONS.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };

        }
        
        [HttpGet(Name = "WORKOVERS_RECOMPLETION")]
        public async Task<WebApiResponse> Get_WORKOVERS_RECOMPLETION_JOBs(string year)
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var WORKOVERS_RECOMPLETION = new List<WORKOVERS_RECOMPLETION_JOB1>();

            if (userRole == GeneralModel.Admin)
            {
                WORKOVERS_RECOMPLETION = _context.WORKOVERS_RECOMPLETION_JOBs1.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                WORKOVERS_RECOMPLETION = _context.WORKOVERS_RECOMPLETION_JOBs1.Where(c => c.Year_of_WP == year && c.COMPANY_ID == companyID).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = WORKOVERS_RECOMPLETION.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };

        }
         
        [HttpGet(Name = "INITIAL_WELLCOMPLETION")]
        public async Task<WebApiResponse> Get_INITIAL_WELL_COMPLETION(string year)
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var INITIAL_WELLCOMPLETION = new List<INITIAL_WELL_COMPLETION_JOB1>();

            if (userRole == GeneralModel.Admin)
            {
                INITIAL_WELLCOMPLETION = _context.INITIAL_WELL_COMPLETION_JOBs1.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                INITIAL_WELLCOMPLETION = _context.INITIAL_WELL_COMPLETION_JOBs1.Where(c => c.Year_of_WP == year && c.COMPANY_ID == companyID).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = INITIAL_WELLCOMPLETION.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };

        }



        }

    }