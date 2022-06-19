using AutoMapper;
using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using static Backend_UMR_Work_Program.Models.ViewModel;

namespace Backend_UMR_Work_Program.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PresentationController : ControllerBase
    {
        private Presentation _presentation;
        private Account _account;
        public WKP_DBContext _context;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public PresentationController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController, Account account, IMapper mapper)
        {
            _account = account;
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor, _mapper);
        }

        private string? WKPUserId => "1";
        private string? WKPUserName => "Name";
        private string? WKPUserEmail => "adeola.kween123@gmail.com";
        private string? WKPUserRole => "Admin";
        //private string? WKPUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        //private string? WKPUserName => User.FindFirstValue(ClaimTypes.Name);
        //private string? WKPUserEmail => User.FindFirstValue(ClaimTypes.Email);
        //private string? WKPUserRole => User.FindFirstValue(ClaimTypes.Role);



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

        [HttpPost("SCHEDULEPRESENTATION")]
        public async Task<WebApiResponse> SCHEDULE_PRESENTATION_DATETIME(string time, DateTime date)
        {   
            var CurrentYear = DateTime.Now.Year.ToString();
            var checkCompanySchedule = _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.COMPANY_ID == WKPUserId && c.YEAR == CurrentYear).FirstOrDefault();

            if (checkCompanySchedule != null)
            {
                //schedule already exist for company
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "There is an existing presentation schedule for this year.", StatusCode = ResponseCodes.Failure };

            }
            else
            {

                //check if date has been scheduled by another company
                string Date_Conversion = date.ToString("dddd , dd MMMM yyyy");

                var checkSchedule = _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.wp_date == Date_Conversion && c.wp_time == time).FirstOrDefault();
                if (checkSchedule != null)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Sorry, date and time have already been selected. Kindly select another day and/or time.", StatusCode = ResponseCodes.Failure };
                }

                var presentation = new ADMIN_DATETIME_PRESENTATION()
                {
                    COMPANYNAME = WKPUserName,
                    COMPANYEMAIL = WKPUserEmail,
                    COMPANY_ID = WKPUserId,
                    YEAR = CurrentYear,
                    CREATED_BY = WKPUserEmail,
                    Submitted = "Not Yet",
                    wp_date = Date_Conversion,
                    DATE_TIME_TEXT = Date_Conversion,
                    wp_time = time,
                    Date_Created_BY_COMPANY = DateTime.Now.ToString(),
                    Date_Created = DateTime.Now,
                };

                _context.ADMIN_DATETIME_PRESENTATIONs.Add(presentation);

                if (await _context.SaveChangesAsync() > 0)
                {
                    var companyPresentations = _context.ADMIN_DATETIME_PRESENTATIONs.Where(x => x.COMPANY_ID == WKPUserId).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "A presentation schedule was created successfully.", Data = companyPresentations, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "An error occured while trying to create this presentation schedule.", StatusCode = ResponseCodes.Failure };

                }
            }
        }
        [HttpPost("UPLOADPRESENTATION")]
        public async Task<WebApiResponse> UPLOAD_PRESENTATION_DOCUMENT(string year, IFormFile document)
        {   
            
            var CurrentYear = DateTime.Now.Year.ToString();
            var checkCompanyPresentation = _context.PRESENTATION_UPLOADs.Where(c => c.COMPANY_ID == WKPUserId && c.Year_of_WP == year).FirstOrDefault();

            if (checkCompanyPresentation != null)
            {
                //schedule already exist for company
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "You have already uploaded a presentation for the selected year, kindly delete an existing document to upload another.", StatusCode = ResponseCodes.Failure };
            }
            else
            {

                //check if date has been scheduled by another company
                var system_date = DateTime.Now.ToString(); // 

                var uniqueFileName = "";
                var FileExtension = "";
                var newFileName = "";
                var documentPath = "";
                var fileName = "";
                //For image cover
                if (document != null)
                {
                    if (document.Length > 0)
                    {

                        var up = Path.Combine(Directory.GetCurrentDirectory(), "Documents");
                        //var docName = ContentDispositionHeaderValue.Parse(document.ContentDisposition).FileName.Trim();
                        //FileExtension = Path.GetExtension(docName).ToString().Replace("\n", ''');

                        var uploads = Path.Combine(up, "Presentations");
                        fileName = document.FileName.Split(".")[0].Trim();
                        uniqueFileName = Convert.ToString(Guid.NewGuid());
                        FileExtension = document.FileName.Split(".")[1].Trim();
                        //newFileName = uniqueFileName + FileExtension;
                        newFileName = fileName + "." + FileExtension;

                        // store path of folder in database
                        documentPath = "//Documents/Presentations/" + newFileName;
                        using (var s = new FileStream(Path.Combine(uploads, newFileName),
                             FileMode.Create))
                        {
                            document.CopyTo(s);
                        }

                    }


                    var presentation = new PRESENTATION_UPLOAD()
                    {
                        CompanyName = WKPUserName,
                        Companyemail = WKPUserEmail,
                        COMPANY_ID = WKPUserId,
                        Year_of_WP = CurrentYear,
                        uploaded_presentation = newFileName,
                        upload_extension = "." + FileExtension,
                        original_filemane = fileName,
                        Created_by = WKPUserEmail,
                        Date_Created = DateTime.Now,
                    };

                    _context.PRESENTATION_UPLOADs.Add(presentation);

                    if (await _context.SaveChangesAsync() > 0)
                    {
                        var companyPresentations = _context.PRESENTATION_UPLOADs.Where(x => x.COMPANY_ID == WKPUserId /*&& x.Year_of_WP == CurrentYear*/).ToListAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "File uploaded successfully.", Data = companyPresentations, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "An error occured while trying to create this presentation schedule.", StatusCode = ResponseCodes.Failure };

                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Sorry, document is empty.", StatusCode = ResponseCodes.Failure };

                }
            }
        }


        [HttpPost("PRESENTATION_SCRIBES")]
        public async Task<WebApiResponse> Presentation_scribe(int[] Id, string emailStatus, string year)
        {   
            int save = 0;
            //var newAdminPresentation = new Admin_Date_Presentations();
            var details = await _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.YEAR == year).ToListAsync();
            if (details.Count > 0)
            {
                try
                {
                    if (WKPUserRole == GeneralModel.Admin)
                    {
                        foreach (var select in Id)
                        {
                            var selected = details.Find(c => c.Id == select);
                            if (selected != null)
                            {
                                if (emailStatus == "Active" && selected.EMAIL_REMARK != "Successful" || emailStatus == "Active" && selected.EMAIL_REMARK != null)
                                {
                                    selected.EMAIL_REMARK = "Successful";
                                }
                                else if (emailStatus == "Inactive" && selected.EMAIL_REMARK == "Successful" || emailStatus == "Inactive" && selected.EMAIL_REMARK == null)
                                {
                                    selected.EMAIL_REMARK = emailStatus.ToUpper();
                                }
                            }
                        }
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

                    }
                    save = await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        string message = "Email updated successfully.";
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = message, Data = details, StatusCode = ResponseCodes.Success };
                    }
                }
                catch (Exception ex)
                {

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Badrequest };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };
            }
            else
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "No data found", StatusCode = ResponseCodes.RecordNotFound };

            }
        }

        [HttpGet("SCRIBES_YEARLIST")]
        public async Task<object> Get_Scribes_And_Chairmen_Yearlist() {
            var yearlist = await (from a in _context.ADMIN_DATETIME_PRESENTATIONs where a.YEAR != "" orderby a.YEAR select a.YEAR).Distinct().ToListAsync();
            return yearlist;
        }

        [HttpGet("SCRIBES_&_CHAIRMEN")]
        public async Task<WebApiResponse> scribes(string year)
        {   
            var presentYear = DateTime.Now.Year;

            var details = new List<ADMIN_DATETIME_PRESENTATION>();
            try
            {
                if (WKPUserRole == GeneralModel.Admin)
                {

                    details =await _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.YEAR == year).ToListAsync();
                }
                else
                {
                    details =await _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.YEAR == year && c.COMPANY_ID == WKPUserId).ToListAsync();
                }
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

        }

        [HttpGet("DIVISIONAL_SCHEDULE_LIST")]
        public async Task<WebApiResponse> Get_Divisional_Schedule_list(string year = null)
        {

            var details = new List<ADMIN_DIVISIONAL_REPS_PRESENTATION>();
            try
            {
                    details = await (from a in _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs select a).ToListAsync();
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details.OrderBy(x => x.YEAR), StatusCode = ResponseCodes.Success };

        }

        [HttpGet("DIVISIONAL_SCHEDULE_BY_YEAR")]
        public async Task<WebApiResponse> Get_Divisional_Schedule_By_Year(string year = null)
        {

            var details = new List<ADMIN_DIVISIONAL_REPS_PRESENTATION>();
            try
            {
                    details = await (from a in _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs where a.YEAR == year select a).ToListAsync();
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details.OrderBy(x => x.YEAR), StatusCode = ResponseCodes.Success };

        }

        [HttpGet("GET_COMPANY_REP")]
        public async Task<WebApiResponse> Comprep(int Id)
        {
            
            var details = _context.ADMIN_DATETIME_PRESENTATIONs.FirstOrDefault(c => c.Id == Id);
            if (WKPUserRole == GeneralModel.Admin)
            {
                try
                {
                    if (details != null)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };
                    }
                }
                catch (Exception ex)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = ex.Message, StatusCode = ResponseCodes.InternalError };
                }
            }
            return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound, Message = "No data found", StatusCode = ResponseCodes.RecordNotFound };
        }

        [HttpGet("DIVISIONAL_YEARLIST")]
        public async Task<object> Get_Divisional_Yearlist() {
            var yearlist = await (from a in _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs where a.YEAR != "" orderby a.YEAR select a.YEAR).Distinct().ToListAsync();
            return yearlist;
        }

        [HttpPost("UPDATE_COMPANY_REP")]
        public async Task<WebApiResponse> UpdateRep(int Id, int UpdateId)
        {
            
            int save = 0;
            var details = _context.ADMIN_DIVISION_REP_LISTs.FirstOrDefault(c => c.Id == Id);
            var compId = _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs.FirstOrDefault(n => n.Id == UpdateId);
            if (WKPUserRole == GeneralModel.Admin)
            {
                if (details != null && compId != null)
                {
                    try
                    {
                        compId.REPRESENTATIVE = details.DIV_REP_NAME;
                        compId.REPRESENTATIVE_EMAIL = details.DIV_REP_EMAIL;
                        save = await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string message = "Company admin representative updated sucessfully";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = message, Data = compId, StatusCode = ResponseCodes.Success };
                        }
                    }
                    catch (Exception ex)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = ex.Message, StatusCode = ResponseCodes.Failure };
                    }
                }
            }
            return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound, Message = "No data found", StatusCode = ResponseCodes.RecordNotFound };
        }
        [HttpPost("ACTIVATE/DEACTIVATE_EMAIL")]
        public async Task<WebApiResponse> EmailStatus(int UpdateId, string emailStatus)
        {
            
            var active = "Successful";
            string inactive = "Inactive";
            int save = 0;
            var compId = _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs.FirstOrDefault(n => n.Id == UpdateId);
            if (WKPUserRole == GeneralModel.Admin)
            {
                if (compId != null)
                {
                    try
                    {
                        if (emailStatus == "Active" && compId.EMAIL_REMARK != "Successful" || emailStatus == "Active" && compId.EMAIL_REMARK != null)
                            compId.EMAIL_REMARK = active;
                        else if (emailStatus == "Inactive" && compId.EMAIL_REMARK == active || emailStatus == "Inactive" && compId.EMAIL_REMARK == null)
                            compId.EMAIL_REMARK = inactive.ToUpper();
                        save = await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            string message = "Company admin representative updated sucessfully";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = message, Data = compId, StatusCode = ResponseCodes.Success };
                        }
                    }
                    catch (Exception ex)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = ex.Message, StatusCode = ResponseCodes.Failure };
                    }
                }
            }
            return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound, Message = "No data found", StatusCode = ResponseCodes.RecordNotFound };
        }
    }
}
