﻿using AutoMapper;
using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using static Backend_UMR_Work_Program.Models.ViewModel;

namespace Backend_UMR_Work_Program.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class PresentationController : ControllerBase
    {
        private Presentation _presentation;
        public WKP_DBContext _context;
        public IConfiguration _configuration;
        private HelpersController helpersController;
        HelpersController _helpersController;
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private string UserWorkRole = "Admin";
        public PresentationController(Presentation presentation, WKP_DBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _presentation = presentation;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            helpersController = new HelpersController(_context, configuration, _httpContextAccessor, _mapper);

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
        [HttpPost("SCHEDULEPRESENTATION")]
        public async Task<WebApiResponse> SCHEDULE_PRESENTATION_DATETIME(string time, DateTime date)
        {
            //var userRole = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionRoleName));
            //var userEmail = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionEmail));
            //var companyID = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionUserID));

            var userRole = "Admin";
            var userName = "testname";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";
            var CurrentYear = DateTime.Now.Year.ToString();
            var checkCompanySchedule = _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.COMPANY_ID == companyID && c.YEAR == CurrentYear).FirstOrDefault();

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
                    COMPANYNAME = userName,
                    COMPANYEMAIL = userEmail,
                    COMPANY_ID = companyID,
                    YEAR = CurrentYear,
                    CREATED_BY = userEmail,
                    Submitted = "Not Yet",
                    wp_date = Date_Conversion,
                    DATE_TIME_TEXT = Date_Conversion,
                    wp_time = time,
                    Date_Created_BY_COMPANY = DateTime.Now.ToString(),
                    Date_Created = DateTime.Now,
                };

                _context.ADMIN_DATETIME_PRESENTATIONs.Add(presentation);

                if (_context.SaveChanges() > 0)
                {
                    var companyPresentations = _context.ADMIN_DATETIME_PRESENTATIONs.Where(x => x.COMPANY_ID == companyID).ToList();
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
            //var userRole = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionRoleName));
            //var userEmail = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionEmail));
            //var companyID = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionUserID));

            var userRole = "Admin";
            var userName = "testname";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";
            var CurrentYear = DateTime.Now.Year.ToString();
            var checkCompanyPresentation = _context.PRESENTATION_UPLOADs.Where(c => c.COMPANY_ID == companyID && c.Year_of_WP == year).FirstOrDefault();

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
                        CompanyName = userName,
                        Companyemail = userEmail,
                        COMPANY_ID = companyID,
                        Year_of_WP = CurrentYear,
                        uploaded_presentation = newFileName,
                        upload_extension = "." + FileExtension,
                        original_filemane = fileName,
                        Created_by = userEmail,
                        Date_Created = DateTime.Now,
                    };

                    _context.PRESENTATION_UPLOADs.Add(presentation);

                    if (_context.SaveChanges() > 0)
                    {
                        var companyPresentations = _context.PRESENTATION_UPLOADs.Where(x => x.COMPANY_ID == companyID /*&& x.Year_of_WP == CurrentYear*/).ToList();

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
        [HttpPost("UPLOAD_MOM")]
        public async Task<WebApiResponse> UPLOADMOM(int compId, IFormFile document)
        {
            //var userRole = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionRoleName));
            //var userEmail = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionEmail));
            //var companyID = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionUserID));

            var userRole = "Admin";
            var userName = "testname";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";
            var CurrentYear = DateTime.Now.Year.ToString();
            string folderToSave = "";
            var checkUploadedMom = _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.Id == compId).FirstOrDefault();

            if (checkUploadedMom.MOM_UPLOAD_DATE != null)
            {
                //schedule already exist for company
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "You have already uploaded minutes for the selected year, kindly delete an existing document to upload another.", StatusCode = ResponseCodes.Failure };
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
                var save = 0;
                //For image cover
                if (document != null)
                {
                    if (document.Length > 0)
                    {
                        var up = Path.Combine(Directory.GetCurrentDirectory(), "Documents");
                        var uploads = Path.Combine(up, "UploadMOMs");
                        fileName = document.FileName.Split(".")[0].Trim();
                        uniqueFileName = Convert.ToString(Guid.NewGuid());
                        FileExtension = document.FileName.Split(".")[1].Trim();

                        newFileName = fileName + "." + FileExtension;


                        documentPath = "//Documents/UploadMOMs/" + newFileName;
                        using (var s = new FileStream(Path.Combine(uploads, newFileName),
                             FileMode.Create))
                        {
                            document.CopyTo(s);
                        }
                    }
                    checkUploadedMom.MOM = documentPath;
                    checkUploadedMom.MOM_UPLOADED_BY = userEmail;
                    checkUploadedMom.MOM_UPLOAD_DATE = DateTime.Now;
                    save = _context.SaveChanges();
                    if (save > 0)
                    {
                        var companyUploadedmom = _context.ADMIN_DATETIME_PRESENTATIONs.Where(x => x.Id == compId).ToList();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "File uploaded successfully.", Data = companyUploadedmom, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "An error occured while trying to upload this minutes.", StatusCode = ResponseCodes.Failure };

                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Sorry, document is empty.", StatusCode = ResponseCodes.Failure };

                }
            }
        }
        [HttpPost("ACTIVATE_DEACTIVATE_EMAIL")]
        public async Task<WebApiResponse> UpdateEmailStatus(int compId, string option)
        {
            string optionA = "Active";
            string optionB = "Inactive";
            string active = "Successful";
            string inactive = "Inactive";
            int save = 0;
            var dateTimeId = _context.ADMIN_DATETIME_PRESENTATIONs.FirstOrDefault(x => x.Id == compId);
            if (dateTimeId.EMAIL_REMARK == null)
            {
                dateTimeId.EMAIL_REMARK = inactive;
            }
            if (UserWorkRole == GeneralModel.Admin)
            {
                try
                {
                    if (dateTimeId != null)
                    {
                        if (dateTimeId != null && option == optionA && dateTimeId.EMAIL_REMARK.ToLower() != active.ToLower())
                        {
                            dateTimeId.EMAIL_REMARK = active;

                        }
                        if (dateTimeId != null && option == optionB && dateTimeId.EMAIL_REMARK.ToLower() != inactive.ToLower())
                        {
                            dateTimeId.EMAIL_REMARK = inactive.ToUpper();

                        }
                        save = _context.SaveChanges();
                        if (save > 0)
                        {
                            string message = "Company admin representative updated sucessfully";
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = message, Data = compId, StatusCode = ResponseCodes.Success };
                        }
                    }
                }
                catch (Exception)
                {

                    return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound, Message = "Error Occured", StatusCode = ResponseCodes.RecordNotFound };

                }

            }
            return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound, Message = "Not Successful", StatusCode = ResponseCodes.RecordNotFound };

        }
        [HttpPost("UPDATE_COMPANY_REP")]
        public async Task<WebApiResponse> UpdateRep(int compId, [FromBody] Admin_Reps model)
        {
            if (UserWorkRole == GeneralModel.Admin)
            {
                try
                {
                    int save = 0;
                    var _comp = _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs.FirstOrDefault(n => n.Id == compId);
                    if (_comp != null)
                    {
                        _comp.REPRESENTATIVE = model.REPRESENTATIVE;
                        _comp.REPRESENTATIVE_EMAIL = model.REPRESENTATIVE_EMAIL;
                        _comp.Date_Updated = DateTime.Now;

                        save = _context.SaveChanges();
                    }
                    if (save > 0)
                    {
                        string message = "Company representative updated sucessfully";
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = message, Data = _comp, StatusCode = ResponseCodes.Success };
                    }
                }
                catch (Exception ex)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound, Message = "Error occured", StatusCode = ResponseCodes.RecordNotFound };

                }
            }
            return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound, Message = "No data found", StatusCode = ResponseCodes.RecordNotFound };
        }
        [HttpGet("COMPANY_REPS")]
        public async Task<object> Get_Company_Replist()
        {
            return await _context.ADMIN_DIVISION_REP_LISTs.ToListAsync();
        }
        [HttpPost("PRESENTATION_SCRIBES")]
        public async Task<WebApiResponse> Presentation_scribe(int[] Id, string emailStatus = null, string year = null)

        {
            var userRole = "Admin";
            var userName = "test";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";
            int save = 0;
            //var newAdminPresentation = new Admin_Date_Presentations();
            var details = await _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.YEAR == year).ToListAsync();
            if (details.Count > 0)
            {
                try
                {
                    if (userRole == GeneralModel.Admin)
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
                    save = _context.SaveChanges();
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
        public async Task<object> Get_Scribes_And_Chairmen_Yearlist()
        {
            var yearlist = await (from a in _context.ADMIN_DATETIME_PRESENTATIONs where a.YEAR != "" orderby a.YEAR select a.YEAR).Distinct().ToListAsync();
            return yearlist;
        }

        [HttpGet("SCRIBES_&_CHAIRMEN")]
        public async Task<WebApiResponse> scribes(string? year)
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";
            var presentYear = DateTime.Now.Year;

            var details = new List<ADMIN_DATETIME_PRESENTATION>();
            try
            {
                if (userRole == GeneralModel.Admin)
                {

                    details = await _context.ADMIN_DATETIME_PRESENTATIONs.ToListAsync();
                }
                else
                {
                    details = await _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.COMPANY_ID == companyID).ToListAsync();
                }
                if (year != null)
                {
                    details = details.Where(c => c.YEAR == year).ToList();
                }
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details.OrderBy(x => x.YEAR), StatusCode = ResponseCodes.Success };

        }

        [HttpGet("DIVISIONAL_SCHEDULE_LIST")]
        public async Task<WebApiResponse> Get_Divisional_Schedule_list()
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
        public async Task<WebApiResponse> Get_Divisional_Schedule_By_Year(string year)
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
            var userRole = "Admin";
            var details = _context.ADMIN_DATETIME_PRESENTATIONs.FirstOrDefault(c => c.Id == Id);
            if (userRole == GeneralModel.Admin)
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
        public async Task<object> Get_Divisional_Yearlist()
        {
            var yearlist = await (from a in _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs where a.YEAR != "" orderby a.YEAR select a.YEAR).Distinct().ToListAsync();
            return yearlist;
        }



    }
}
