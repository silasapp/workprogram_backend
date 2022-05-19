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


    [HttpGet(Name = "Get_ADMIN_CONCESSIONS_INFORMATION_BY_CURRENT_YEAR")]
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


    }

}