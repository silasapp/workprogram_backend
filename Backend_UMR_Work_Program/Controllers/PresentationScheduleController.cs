using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Backend_UMR_Work_Program.Models.GeneralModel;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PresentationScheduleController : ControllerBase
    {
        private Account _account;
        public WKP_DBContext _context;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        IHttpContextAccessor _httpContextAccessor;


        public PresentationScheduleController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController)
        {
            _context = context;
            _configuration = configuration;
            helpersController = new HelpersController(_context, configuration, _httpContextAccessor);
        }
        [HttpPost(Name = "PRESENTATION_SCRIBES")]
        public async Task<WebApiResponse> scribe(int[] Id, string emailStatus = null, string year = null)

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

        [HttpGet("SCRIBES_&_CHAIRMEN_YEARLIST")]
        public async Task<object> Get_Scribes_And_Chairmen_Yearlist() {
            var yearlist = await (from a in _context.ADMIN_DATETIME_PRESENTATIONs where a.YEAR != "" orderby a.YEAR select a.YEAR).Distinct().ToListAsync();
            return yearlist;
        }

        [HttpGet(Name = "SCRIBES_&_CHAIRMEN")]
        public async Task<WebApiResponse> scribes(string year = null)
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

                    details = _context.ADMIN_DATETIME_PRESENTATIONs.ToList();
                }
                else
                {
                    details = _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.COMPANY_ID == companyID).ToList();
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

        [HttpGet(Name = "GET_COMPANY_REP")]
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
        public async Task<object> Get_Divisional_Yearlist() {
            var yearlist = await (from a in _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs where a.YEAR != "" orderby a.YEAR select a.YEAR).Distinct().ToListAsync();
            return yearlist;
        }

        [HttpPost(Name = "UPDATE_COMPANY_REP")]
        public async Task<WebApiResponse> UpdateRep(int Id, int UpdateId)
        {
            var userRole = "Admin";
            int save = 0;
            var details = _context.ADMIN_DIVISION_REP_LISTs.FirstOrDefault(c => c.Id == Id);
            var compId = _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs.FirstOrDefault(n => n.Id == UpdateId);
            if (userRole == GeneralModel.Admin)
            {
                if (details != null && compId != null)
                {
                    try
                    {
                        compId.REPRESENTATIVE = details.DIV_REP_NAME;
                        compId.REPRESENTATIVE_EMAIL = details.DIV_REP_EMAIL;
                        save = _context.SaveChanges();
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
        [HttpPost(Name = "ACTIVATE/DEACTIVATE_EMAIL")]
        public async Task<WebApiResponse> EmailStatus(int UpdateId, string emailStatus)
        {
            var userRole = "Admin";
            var active = "Successful";
            string inactive = "Inactive";
            int save = 0;
            var compId = _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs.FirstOrDefault(n => n.Id == UpdateId);
            if (userRole == GeneralModel.Admin)
            {
                if (compId != null)
                {
                    try
                    {
                        if (emailStatus == "Active" && compId.EMAIL_REMARK != "Successful" || emailStatus == "Active" && compId.EMAIL_REMARK != null)
                            compId.EMAIL_REMARK = active;
                        else if (emailStatus == "Inactive" && compId.EMAIL_REMARK == active || emailStatus == "Inactive" && compId.EMAIL_REMARK == null)
                            compId.EMAIL_REMARK = inactive.ToUpper();
                        save = _context.SaveChanges();
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
