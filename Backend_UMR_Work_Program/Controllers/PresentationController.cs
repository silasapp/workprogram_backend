using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Backend_UMR_Work_Program.Models.ViewModel;

namespace Backend_UMR_Work_Program.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class PresentationController : ControllerBase
    {
        private Presentation _presentation;
        public PresentationController(Presentation presentation)
        {
            _presentation = presentation;
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

    }
}
