using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using Newtonsoft.Json;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using static Backend_UMR_Work_Program.Models.ViewModel;
//using static Backend_UMR_Work_Program.Helpers.GeneralClass;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]")]


    public class AccountController : ControllerBase
    {
        private Account _account;
        public WKP_DBContext _context;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        IHttpContextAccessor _httpContextAccessor;
        public AccountController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController, Account account)
        {
            //_httpContextAccessor = httpContextAccessor;
            _account = account;
            _context = context;
            _configuration = configuration;

            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
            //_helpersController = new HelpersController(_context, _configuration);
        }

        [HttpGet("GetData")]
        public object GetData()
        {
            var table = _account.GetData();
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Logine logine)
        {
            var tokenData = await _account.isAutheticate(logine.email, logine.password);
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(tokenData);
            return Ok(tokenData);
        }


        [HttpGet("VerifyCompanyCode")]
        public async Task<IActionResult> VerifyCompanyCode(string companyCode)
        {
            var isAvailable = await _account.VerifyCompanyCode(companyCode);
            return Ok(isAvailable);
        }


        [HttpPost("CheckNewPinCode")]
        public async Task<IActionResult> CheckNewPinCode(string oldCompanyCode, string email, string newCompanyCode)
        {
            var isAvailable = await _account.CheckNewPinCode(oldCompanyCode, email, newCompanyCode);
            return Ok(isAvailable);
        }

        [HttpGet("GetCompanyResource")]
        public async Task<IActionResult> GetCompanyResource(string companyCode)
        {
            var isAvailable = await _account.GetCompanyResource(companyCode);
            return Ok(isAvailable);
        }

        [HttpPost("CreateCompanyResource")]
        public async Task<IActionResult> CreateCompanyResource([FromBody] CreateUser user)
        {
            var isAvailable = await _account.CheckIfUserExistBeforeCreating(user.companyName, user.companyCode, user.name, user.designation, user.phone, user.email, user.password);
            return Ok(isAvailable);
        }

        [HttpPost("DeleteCompanyResource")]
        public async Task<IActionResult> DeleteCompanyResource(string id, string companyCode)
        {
            var isAvailable = await _account.DeleteUser(companyCode, id);
            return Ok(isAvailable);
        }

        [HttpPost(Name = "ResetPassword")]
        public async Task<WebApiResponse> ResetPassword(string email, string currentPassword, string newPassword)
        {
            string encryptCP = _helpersController.Encrypt(currentPassword);

            var getUser = (from u in _context.ADMIN_COMPANY_INFORMATIONs where u.EMAIL == email.Trim() && u.PASSWORDS == encryptCP select u).FirstOrDefault();
            if (getUser != null)
            {
                getUser.PASSWORDS = _helpersController.Encrypt(newPassword);
                getUser.UPDATED_BY = getUser.Id.ToString();
                getUser.UPDATED_DATE = DateTime.Now.ToString();

                if (await _context.SaveChangesAsync() > 0)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Password updated successfully", StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "An error occured while updating this password.", StatusCode = ResponseCodes.Failure };
                }
            }
            else
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Unable to fetch user details from Work Program.", StatusCode = ResponseCodes.RecordNotFound };
            }

        }

    }
}
