using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using static Backend_UMR_Work_Program.Models.ViewModel;

namespace Backend_UMR_Work_Program.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class PresentationController : ControllerBase
    {
        private Presentation _presentation; public WKP_DBContext _context;

        public PresentationController(Presentation presentation, WKP_DBContext context)
        {
            _presentation = presentation;
            _context = context;
        }

        private string? WKPCompanyId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string? WKPCompanyName => User.FindFirstValue(ClaimTypes.Name);
        private string? WKPCompanyEmail => User.FindFirstValue(ClaimTypes.Email);
        private string? WKPRole => User.FindFirstValue(ClaimTypes.Role);


        [HttpGet("GetCompanyDetails")]
        public CompanyDetail CompanyDetails(string companyName, string companyEmail, string companyId)
        {
            var details = _presentation.CompanyDetails(companyName, companyEmail, companyId);

            return details;
        }

        [HttpPost("EditCompanyDetails")]
        public IActionResult EditCompanyDetails([FromBody] CompanyDetail myDetail)
        {
            _presentation.Insert_Company_Details_Contact_Person(myDetail.CompanyName, myDetail.CompanyEmail, myDetail.Address_of_Company, myDetail.Name_of_MD_CEO, myDetail.Phone_NO_of_MD_CEO, myDetail.Contact_Person, myDetail.Phone_No, myDetail.Email_Address);
            return Ok(myDetail);
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
