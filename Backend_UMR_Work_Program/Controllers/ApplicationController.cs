using AutoMapper;
using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Backend_UMR_Work_Program.Models.GeneralModel;

namespace Backend_UMR_Work_Program.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]

    public class ApplicationController : Controller
    {
        public WKP_DBContext _context;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ApplicationController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor, _mapper);
        }
        private string? WKPCompanyId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string? WKPCompanyName => User.FindFirstValue(ClaimTypes.Name);
        private string? WKPCompanyEmail => User.FindFirstValue(ClaimTypes.Email);
        private string? WKUserRole => User.FindFirstValue(ClaimTypes.Role);
        private int? WKPCompanyNumber => Convert.ToInt32(User.FindFirstValue(ClaimTypes.PrimarySid));

        [HttpGet("GetDashboardStuff")]
        public async Task<object> GetDashboardStuff()
        {
            try
            {
                var deskCount = 0;
                var getStaff = (from stf in _context.staff
                                join admin in _context.ADMIN_COMPANY_INFORMATIONs on stf.AdminCompanyInfo_ID equals admin.Id
                                where stf.AdminCompanyInfo_ID == WKPCompanyNumber && stf.DeleteStatus != true
                                select stf).FirstOrDefault();
                if (getStaff != null)
                {
                    deskCount = await _context.MyDesks.Where(x => x.StaffID == getStaff.StaffID && x.HasWork != true).CountAsync();
                }
                var allApplicationsCount = await _context.Applications.Where(x => x.Status == GeneralModel.Processing).CountAsync();
                var allProcessingCount = await _context.Applications.CountAsync();
                var allApprovalsCount = await _context.PermitApprovals.CountAsync();
                return new
                {
                    deskCount = deskCount,
                    allApplicationsCount = allApplicationsCount,
                    allProcessingCount = allProcessingCount,
                    allApprovalsCount = allApprovalsCount
                };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpGet("GetAppsOnMyDesk")]
        public async Task<WebApiResponse> GetAppsOnMyDesk()
        {
            try
            {
                var applications = await (from dsk in _context.MyDesks
                                          join app in _context.Applications on dsk.AppId equals app.Id
                                          join comp in _context.ADMIN_COMPANY_INFORMATIONs on app.CompanyID equals comp.Id
                                          join stf in _context.staff on dsk.StaffID equals stf.StaffID
                                          join admin in _context.ADMIN_COMPANY_INFORMATIONs on stf.AdminCompanyInfo_ID equals admin.Id
                                          join con in _context.ADMIN_CONCESSIONS_INFORMATIONs on app.ConcessionID equals con.Consession_Id
                                          where admin.Id == WKPCompanyNumber && dsk.HasWork != true
                                          select new Application_Model
                                          {
                                              Id= app.Id,
                                              FieldID = app.FieldID,
                                              ConcessionID = app.ConcessionID,
                                              ConcessionName = con.Concession_Held,
                                              FieldName = _context.COMPANY_FIELDs.Where(x => x.Field_ID == app.FieldID).FirstOrDefault().Field_Name,
                                              ReferenceNo = app.ReferenceNo,
                                              CreatedAt = app.CreatedAt,
                                              SubmittedAt = app.SubmittedAt,
                                              CompanyName = comp.COMPANY_NAME,
                                              Status = app.Status,
                                              PaymentStatus = app.PaymentStatus,
                                              YearOfWKP = app.YearOfWKP
                                          }).ToListAsync();
                return new WebApiResponse { Data= applications, ResponseCode = AppResponseCodes.Success, Message = "Success", StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }
        [HttpGet("All-Applications")] //For general application view
        public async Task<WebApiResponse> AllApplications()
        {
            try
            {
                var applications = await (from app in _context.Applications
                                          join comp in _context.ADMIN_COMPANY_INFORMATIONs on app.CompanyID equals comp.Id
                                          join field in _context.COMPANY_FIELDs on app.FieldID equals field.Field_ID
                                          join con in _context.ADMIN_CONCESSIONS_INFORMATIONs on app.ConcessionID equals con.Consession_Id
                                          select new Application_Model
                                          {
                                              Id= app.Id,
                                              FieldID = app.FieldID,
                                              ConcessionID = app.ConcessionID,
                                              ConcessionName = con.ConcessionName,
                                              FieldName = field.Field_Name,
                                              ReferenceNo = app.ReferenceNo,
                                              CreatedAt = app.CreatedAt,
                                              SubmittedAt = app.SubmittedAt,
                                              CompanyName = comp.COMPANY_NAME,
                                              Status = app.Status,
                                              PaymentStatus = app.PaymentStatus,
                                              YearOfWKP = app.YearOfWKP
                                          }).ToListAsync();
                return new WebApiResponse { Data= applications, ResponseCode = AppResponseCodes.Success, Message = "Success", StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpGet("ViewApplication")] //For general application view
        public async Task<WebApiResponse> ViewApplication(int appID)
        {
            try
            {
                var application = (from ap in _context.Applications where ap.Id == appID && ap.DeleteStatus != true select ap).FirstOrDefault();

                if (application == null)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Sorry, this application details could not be found.", StatusCode = ResponseCodes.Failure };
                }
                var field = await _context.COMPANY_FIELDs.Where(x => x.Field_ID == application.FieldID).FirstOrDefaultAsync();
                var concession = await _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(x => x.Consession_Id == application.ConcessionID).FirstOrDefaultAsync();
                var company = await _context.ADMIN_COMPANY_INFORMATIONs.Where(x => x.Id == application.CompanyID).FirstOrDefaultAsync();
                var appHistory = await (from his in _context.ApplicationDeskHistories
                                        join stf in _context.staff on his.StaffID equals stf.StaffID
                                        join admin in _context.ADMIN_COMPANY_INFORMATIONs on stf.AdminCompanyInfo_ID equals admin.Id
                                        join rol in _context.Roles on stf.RoleID equals rol.id
                                        join sbu in _context.StrategicBusinessUnits on stf.Staff_SBU equals sbu.Id
                                        select new ApplicationDeskHistory_Model
                                        {
                                            ID = his.Id,
                                            Staff_Name = stf.FirstName + " " + stf.LastName,
                                            Staff_Email = stf.StaffEmail,
                                            Staff_SBU = sbu.SBU_Name,
                                            Staff_Role = rol.RoleName,
                                            Comment = his.Comment
                                        }).ToListAsync();
                var staffDesk = (from dsk in _context.MyDesks
                                 join stf in _context.staff on dsk.StaffID equals stf.StaffID
                                 join admin in _context.ADMIN_COMPANY_INFORMATIONs on stf.AdminCompanyInfo_ID equals admin.Id
                                 join rol in _context.Roles on stf.RoleID equals rol.id
                                 join sbu in _context.StrategicBusinessUnits on stf.Staff_SBU equals sbu.Id
                                 where dsk.HasWork!= true && stf.ActiveStatus != false && stf.DeleteStatus != true
                                 select new Staff_Model
                                 {
                                     Staff_Name = stf.FirstName +" "+ stf.LastName,
                                     Staff_Email = stf.StaffEmail,
                                     Staff_SBU = sbu.SBU_Name,
                                     Staff_Role = rol.RoleName
                                 }).ToList();
                var documents = await _context.SubmittedDocuments.Where(x => x.CreatedBy == application.CompanyID.ToString() && x.YearOfWKP == application.YearOfWKP).Take(10).ToListAsync();
                var appDetails = new ApplicationDetailsModel
                {
                    Application = application,
                    Field = field,
                    Concession = concession,
                    Company = company,
                    Staff = staffDesk,
                    Application_History = appHistory.OrderByDescending(x => x.ID).Take(3).ToList(),
                    Document = documents
                };
                return new WebApiResponse { Data= appDetails, ResponseCode = AppResponseCodes.Success, Message = "Success", StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpGet("ProcessApplication")] //For processing application view
        public async Task<WebApiResponse> ProcessApplication(int appID)
        {
            try
            {
                var application = (from ap in _context.Applications where ap.Id == appID && ap.DeleteStatus != true select ap).FirstOrDefault();

                if (application == null)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Sorry, this application details could not be found.", StatusCode = ResponseCodes.Failure };
                }
                var field = await _context.COMPANY_FIELDs.Where(x => x.Field_ID == application.FieldID).FirstOrDefaultAsync();
                var concession = await _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(x => x.Consession_Id == application.ConcessionID).FirstOrDefaultAsync();
                var company = await _context.ADMIN_COMPANY_INFORMATIONs.Where(x => x.Id == application.CompanyID).FirstOrDefaultAsync();
                var appHistory = await (from his in _context.ApplicationDeskHistories
                                        join stf in _context.staff on his.StaffID equals stf.StaffID
                                        join admin in _context.ADMIN_COMPANY_INFORMATIONs on stf.AdminCompanyInfo_ID equals admin.Id
                                        join rol in _context.Roles on stf.RoleID equals rol.id
                                        join sbu in _context.StrategicBusinessUnits on stf.Staff_SBU equals sbu.Id
                                        select new ApplicationDeskHistory_Model
                                        {
                                            ID = his.Id,
                                            Staff_Name = stf.FirstName + " " + stf.LastName,
                                            Staff_Email = stf.StaffEmail,
                                            Staff_SBU = sbu.SBU_Name,
                                            Staff_Role = rol.RoleName,
                                            Comment = his.Comment
                                        }).ToListAsync();

                var staffDesk = (from dsk in _context.MyDesks
                                 join stf in _context.staff on dsk.StaffID equals stf.StaffID
                                 join admin in _context.ADMIN_COMPANY_INFORMATIONs on stf.AdminCompanyInfo_ID equals admin.Id
                                 join rol in _context.Roles on stf.RoleID equals rol.id
                                 join sbu in _context.StrategicBusinessUnits on stf.Staff_SBU equals sbu.Id
                                 where dsk.StaffID == WKPCompanyNumber && dsk.HasWork != true && stf.ActiveStatus != false && stf.DeleteStatus != true
                                 select new Staff_Model
                                 {
                                     Staff_Name = stf.FirstName + " " + stf.LastName,
                                     Staff_Email = stf.StaffEmail,
                                     Staff_SBU = sbu.SBU_Name,
                                     Staff_Role = rol.RoleName,
                                     Sort = dsk.Sort
                                 }).ToList();
                var documents = await _context.SubmittedDocuments.Where(x => x.CreatedBy == application.CompanyID.ToString() && x.YearOfWKP == application.YearOfWKP).Take(10).ToListAsync();
                var appDetails = new ApplicationDetailsModel
                {
                    Application = application,
                    Field = field,
                    Concession = concession,
                    Company = company,
                    Staff = staffDesk,
                    Application_History = appHistory.OrderByDescending(x => x.ID).Take(3).ToList(),
                    Document = documents
                };
                return new WebApiResponse { Data = appDetails, ResponseCode = AppResponseCodes.Success, Message = "Success", StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        //year: year, omlName: omlName, fieldName: fieldName
        //public async Task<WebApiResponse> SubmitApplication(string year, int concessionID, int fieldID)
        [HttpPost("SubmitApplication")]
        public async Task<WebApiResponse> SubmitApplication(string year, string omlName, string fieldName)
        {
            try
            {
                int yearID = Convert.ToInt32(year);
                var concession = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.Concession_Held.ToLower() == omlName.ToLower() select d).FirstOrDefaultAsync();
                var field = await (from d in _context.COMPANY_FIELDs where d.Field_Name.ToLower() == fieldName.ToLower() || d.Field_ID.ToString() == fieldName select d).FirstOrDefaultAsync();
                var checkApplication = (from ap in _context.Applications
                                        where ap.YearOfWKP == yearID && ap.ConcessionID == concession.Consession_Id
                                         && ap.DeleteStatus != true
                                        select ap).FirstOrDefault();
                if (field != null)
                {
                    checkApplication = (from ap in _context.Applications
                                        where ap.YearOfWKP == yearID && ap.ConcessionID == concession.Consession_Id
                                                        && ap.FieldID == field.Field_ID && ap.DeleteStatus != true
                                        select ap).FirstOrDefault();

                }
                if (checkApplication != null)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Sorry, this application details could not be found.", StatusCode = ResponseCodes.Failure };
                }

                Task<List<ApplicationProcessModel>> getApplicationProcess = _helpersController.GetApplicationProccess(GeneralModel.New, 0);

                if (getApplicationProcess.Result.Count <= 0)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "An error occured while trying to get process flow for this application.", StatusCode = ResponseCodes.Failure };
                }

                Application application = new Application();
                application.ReferenceNo = _helpersController.Generate_Reference_Number();
                application.YearOfWKP = yearID;
                application.ConcessionID = concession.Consession_Id;
                application.FieldID = field?.Field_ID;
                application.CompanyID = (int)WKPCompanyNumber;
                application.CategoryID = _context.ApplicationCategories.Where(x => x.Name == GeneralModel.New).FirstOrDefault().Id;
                application.Status = GeneralModel.Processing;
                application.PaymentStatus = GeneralModel.PaymentPending;
                application.CurrentDesk = getApplicationProcess.Result.FirstOrDefault().StaffId; //to change
                application.Submitted = true;
                application.CreatedAt = DateTime.Now;
                application.SubmittedAt = DateTime.Now;
                _context.Applications.Add(application);

                if (_context.SaveChanges() > 0)
                {
                    string subject2 = $"{year} submission of WORK PROGRAM application for {WKPCompanyName} field - {field?.Field_Name} : {application.ReferenceNo}";

                    foreach (var staff in getApplicationProcess.Result.ToList())
                    {
                        int saveStaffDesk = _helpersController.RecordStaffDesk(application.Id, staff);

                        if (saveStaffDesk > 0)
                        {
                            _helpersController.SaveHistory(application.Id, staff.StaffId, "Submitted", "Application submitted and landed on staff desk");

                            //send mail to staff
                            //var getStaff = (from stf in _context.staff where stf.StaffID == staff.StaffId select stf).FirstOrDefault();
                            var getStaff = (from stf in _context.staff
                                            join admin in _context.ADMIN_COMPANY_INFORMATIONs on stf.AdminCompanyInfo_ID equals admin.Id
                                            where stf.StaffID == staff.StaffId && stf.DeleteStatus != true
                                            select stf).FirstOrDefault();
                            string content2 = $"{WKPCompanyName} have submitted their WORK PROGRAM application for year {year}.";
                            var emailMsg2 = _helpersController.SaveMessage(application.Id, getStaff.StaffID, subject2, content2, "Staff");
                            var sendEmail2 = _helpersController.SendEmailMessage(getStaff.StaffEmail, getStaff.FirstName, emailMsg2, null);

                            _helpersController.LogMessages("Submission of application with REF : " + application.ReferenceNo, WKPCompanyEmail);

                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "An error occured while trying to submit this application to a staff.", StatusCode = ResponseCodes.Failure };
                        }
                    }
                    //send mail to company
                    string subject = $"{year} submission of WORK PROGRAM application for field - {field?.Field_Name} : {application.ReferenceNo}";
                    string content = $"You have successfully submitted your WORK PROGRAM application for year {year}, and it is currently being reviewed.";
                    var emailMsg = _helpersController.SaveMessage(application.Id, (int)WKPCompanyNumber, subject, content, "Company");
                    var sendEmail = _helpersController.SendEmailMessage(WKPCompanyEmail, WKPCompanyName, emailMsg, null);
                    var responseMsg = field != null ? $"{year} Application for field {field?.Field_Name} has been submitted successfully." : $"{year} Application for concession: ({concession.ConcessionName}) has been submitted successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = responseMsg, StatusCode = ResponseCodes.Success };

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "An error occured while trying to save this application record.", StatusCode = ResponseCodes.Failure };
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpPost("PushApplication")]
        public async Task<WebApiResponse> PushApplication(int deskID, string comment, string[] selectedApps)
        {
            var responseMessage = "";
            try
            {
                if (selectedApps != null)
                {
                    foreach (var b in selectedApps)
                    {
                        int appId = Convert.ToInt16(b);
                        //get current staff desk
                        var staffDesk = _context.MyDesks.Where(a => a.DeskID == deskID && a.AppId == appId && a.StaffID == int.Parse(WKPCompanyId)).FirstOrDefault();
                        var application = _context.Applications.Where(a => a.Id == appId).FirstOrDefault();
                        var Company = _context.ADMIN_COMPANY_INFORMATIONs.Where(p => p.Id == application.CompanyID).FirstOrDefault();
                        var field = _context.COMPANY_FIELDs.Where(p => p.Field_ID == application.FieldID).FirstOrDefault();

                        //update staff desk
                        staffDesk.HasPushed = true;
                        staffDesk.HasWork = true;
                        staffDesk.UpdatedAt = DateTime.Now;
                        _context.SaveChanges();

                        Task<List<ApplicationProcessModel>> getApplicationProcess = _helpersController.GetApplicationProccess(GeneralModel.New, 0);

                        if (getApplicationProcess.Result.Count > 0)
                        {
                            foreach (var staff in getApplicationProcess.Result.ToList())
                            {
                                int saveStaffDesk = _helpersController.RecordStaffDesk(application.Id, staff);

                                if (saveStaffDesk > 0)
                                {
                                    _helpersController.SaveHistory(application.Id, staff.StaffId, "Moved", "Application was pushed to staff desk");

                                    //send mail to staff
                                    var getStaff = (from stf in _context.staff
                                                    join admin in _context.ADMIN_COMPANY_INFORMATIONs on stf.AdminCompanyInfo_ID equals admin.Id
                                                    where stf.StaffID == staff.StaffId && stf.DeleteStatus != true
                                                    select stf).FirstOrDefault();

                                    string subject = $"Push for WORK PROGRAM application with ref: {application.ReferenceNo} ({field.Field_Name} - {application.YearOfWKP}).";
                                    string content = $"{WKPCompanyName} have submitted their WORK PROGRAM application for year {application.YearOfWKP}.";
                                    var emailMsg = _helpersController.SaveMessage(application.Id, getStaff.StaffID, subject, content, "Staff");
                                    var sendEmail = _helpersController.SendEmailMessage(getStaff.StaffEmail, getStaff.FirstName, emailMsg, null);

                                    _helpersController.LogMessages("Submission of application with REF : " + application.ReferenceNo, WKPCompanyEmail);
                                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"Application for field {field?.Field_Name} has been pushed successfully.", StatusCode = ResponseCodes.Success };
                                }
                                else
                                {
                                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "An error occured while trying to submit this application to a staff.", StatusCode = ResponseCodes.Failure };
                                }

                            }
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "An error occured while trying to get process flow for this application.", StatusCode = ResponseCodes.Failure };
                        }
                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: No application ID was passed for this action to be completed.", StatusCode = ResponseCodes.InternalError };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: No application ID was passed for this action to be completed.", StatusCode = ResponseCodes.InternalError };
            }
            catch (Exception x)
            {
                _helpersController.LogMessages($"Approve Error:: {x.Message.ToString()}");
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = $"An error occured while pushing application to staff."+ x.Message.ToString(), StatusCode = ResponseCodes.InternalError };
            }

        }

        [HttpPost("ApproveApplication")]
        public async Task<WebApiResponse> ApproveApplication(int deskID, string comment, string[] selectedApps)
        {
            var responseMessage = "";
            try
            {
                foreach (var b in selectedApps)
                {
                    int appId = Convert.ToInt16(b);
                    //get current staff desk
                    var staffDesk = _context.MyDesks.Where(a => a.DeskID == deskID && a.AppId == appId && a.StaffID == int.Parse(WKPCompanyId)).FirstOrDefault();

                    var application = _context.Applications.Where(a => a.Id == appId).FirstOrDefault();
                    var Company = _context.ADMIN_COMPANY_INFORMATIONs.Where(p => p.Id == application.CompanyID).FirstOrDefault();
                    var field = _context.COMPANY_FIELDs.Where(p => p.Field_ID == application.FieldID).FirstOrDefault();

                    //Update Staff Desk
                    staffDesk.HasPushed = true;
                    staffDesk.HasWork = true;
                    staffDesk.UpdatedAt = DateTime.Now;
                    _context.SaveChanges();

                    var p = _helpersController.CreatePermit(application);

                    responseMessage += "You have APPROVED this application (" + application.ReferenceNo + ")  and approval has been generated. Approval No: " + p + Environment.NewLine;
                    //var staff = _context.staff.Where(x => x.StaffID == int.Parse(WKPCompanyId) && x.DeleteStatus != true).FirstOrDefault();
                    var staff = (from stf in _context.staff
                                 join admin in _context.ADMIN_COMPANY_INFORMATIONs on stf.AdminCompanyInfo_ID equals admin.Id
                                 where stf.StaffID == int.Parse(WKPCompanyId) && stf.DeleteStatus != true
                                 select stf).FirstOrDefault();

                    if (!p.ToLower().Contains("error"))
                    {

                        string body = "";
                        var up = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        string file = up + @"\\Templates\" + "InternalMemo.txt";
                        using (var sr = new StreamReader(file))
                        {
                            body = sr.ReadToEnd();
                        }

                        //send email to staff approver
                        string subject = $"Approval For Application With REF: {application.ReferenceNo}";
                        string content = $"An approval has been generated for application with reference: " + application.ReferenceNo + " for " + field.Field_Name + "(" + Company.NAME + ").";

                        var emailMsg = _helpersController.SaveMessage(appId, staff.StaffID, subject, content, "Staff");
                        var sendEmail = _helpersController.SendEmailMessage(staff.StaffEmail, staff.FirstName, emailMsg, null);

                        _helpersController.LogMessages("Approval generated successfully for field => " + field.Field_Name + ". Application Reference : " + application.ReferenceNo, WKPCompanyEmail);
                        _helpersController.SaveHistory(appId, staff.StaffID, GeneralModel.Approved, staff.StaffEmail + "Final Approval For Application With Ref: " + application.ReferenceNo);

                        responseMessage = "Application(s) has been approved and permit approval generated successfully.";

                    }
                    else
                    {
                        responseMessage += "An error occured while trying to generate approval ref for this application." + Environment.NewLine;

                        //Update My Process
                        staffDesk.HasPushed = false;
                        staffDesk.HasWork = false;
                        staffDesk.UpdatedAt = DateTime.Now;
                        _context.SaveChanges();
                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = responseMessage, StatusCode = ResponseCodes.Failure };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = responseMessage, StatusCode = ResponseCodes.Success };
            }
            catch (Exception x)
            {
                _helpersController.LogMessages($"Approve Error:: {x.ToString()}");
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = $"An error occured while approving application(s).", StatusCode = ResponseCodes.InternalError };
            }

        }

        [HttpGet("All-Companies")]
        public async Task<WebApiResponse> AllCompanies()
        {
            try
            {
                var companies = await _context.ADMIN_COMPANY_INFORMATIONs.Where(ap => ap.DESIGNATION != "Admin").ToListAsync();

                return new WebApiResponse { Data = companies, ResponseCode = AppResponseCodes.Success, Message = "Success", StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        #region Minimum Requirement
        [HttpGet("Get_Planning_Requirement")]
        public async Task<object> Get_Planning_Requirement(string year, string omlName, string fieldName, string actionToDo)
        {
            try
            {
                var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
                var getData = await (from d in _context.Planning_MinimumRequirements
                                     where
                                    d.CompanyNumber == WKPCompanyNumber && d.Year == int.Parse(year) &&
                                    d.ConcessionID == concessionField.Result.Concession_ID
                                     select d).FirstOrDefaultAsync();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = getData, StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpPost("Post_Planning_Requirement")]
        public async Task<WebApiResponse> Post_Planning_Requirement([FromBody] Planning_MinimumRequirement model, string year, string omlName, string id, string actionToDo)
        {

            int save = 0;
            var concessionField = GET_CONCESSION_FIELD(omlName, "");
            string action = (id == "undefined" || actionToDo == null) ? GeneralModel.Insert : actionToDo;
            try
            {
                #region Saving Field

                model.ConcessionID = concessionField.Result.Concession_ID;
                model.CompanyNumber = WKPCompanyNumber;
                model.Year = int.Parse(year);

                if (action == GeneralModel.Insert)
                {
                    var data = await (from c in _context.Planning_MinimumRequirements where c.CompanyNumber == WKPCompanyNumber && c.ConcessionID == concessionField.Result.Concession_ID && c.Year == int.Parse(year) select c).FirstOrDefaultAsync();

                    if (data != null)
                    {

                        _context.Planning_MinimumRequirements.Remove(data);
                        model.DateCreated = DateTime.Now;

                        await _context.Planning_MinimumRequirements.AddAsync(model);


                        //return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This data is already existing and can not be duplicated.", StatusCode = ResponseCodes.Failure };
                    }
                    else
                    {
                        model.DateCreated = DateTime.Now;
                        await _context.Planning_MinimumRequirements.AddAsync(model);
                    }
                }
                else
                {
                    var data = await (from d in _context.Planning_MinimumRequirements
                                      where d.Id.ToString() == id && d.CompanyNumber == WKPCompanyNumber
                                      select d).FirstOrDefaultAsync();

                    if (action == GeneralModel.Update)
                    {
                        if (data == null)
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This details could not be found.", StatusCode = ResponseCodes.Failure };
                        }
                        else
                        {
                            _context.Planning_MinimumRequirements.Remove(data);
                            model.DateCreated = DateTime.Now;
                            await _context.Planning_MinimumRequirements.AddAsync(model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.Planning_MinimumRequirements.Remove(data);
                    }
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Data has been " + action + "D successfully.";
                    var allData = await (from d in _context.Planning_MinimumRequirements
                                         where d.CompanyNumber == WKPCompanyNumber && d.ConcessionID == concessionField.Result.Concession_ID
                                         select d).ToListAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = allData, Message = successMsg, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : An error occured while trying to {actionToDo} this form.", StatusCode = ResponseCodes.Failure };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("Get_HSE_Requirement")]
        public async Task<object> Get_HSE_Requirement(string year, string omlName, string fieldName, string actionToDo)
        {
            try
            {
                var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

                var getData = _context.HSE_MinimumRequirements.FirstOrDefault(d => d.CompanyNumber==WKPCompanyNumber && d.Year==int.Parse(year) && d.ConcessionID==concessionField.Result.Concession_ID);



                //var getData = await (from d in _context.HSE_MinimumRequirements
                //                     where
                //                    d.CompanyNumber == WKPCompanyNumber && d.Year == int.Parse(year) &&
                //                    d.ConcessionID == concessionField.Result.Concession_ID
                //                     select d).FirstOrDefaultAsync();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = getData, StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpPost("Post_HSE_Requirement")]
        public async Task<WebApiResponse> Post_HSE_Requirement([FromBody] HSE_MinimumRequirement model, string year, string omlName, string id, string actionToDo)
        {

            int save = 0;
            var concessionField = GET_CONCESSION_FIELD(omlName, "");

            string action = (id == "undefined" || actionToDo == null) ? GeneralModel.Insert : actionToDo;
            try
            {
                #region Saving Field

                model.ConcessionID = concessionField.Result.Concession_ID;

                model.CompanyNumber = WKPCompanyNumber;
                model.Year = int.Parse(year);

                if (action == GeneralModel.Insert)
                {
                    var data = await (from c in _context.HSE_MinimumRequirements where c.CompanyNumber == WKPCompanyNumber && c.ConcessionID == concessionField.Result.Concession_ID && c.Year == int.Parse(year) select c).FirstOrDefaultAsync();

                    if (data != null)
                    {

                        _context.HSE_MinimumRequirements.Remove(data);
                        model.DateCreated = DateTime.Now;
                        await _context.HSE_MinimumRequirements.AddAsync(model);

                        //return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This data is already existing and can not be duplicated.", StatusCode = ResponseCodes.Failure };
                    }
                    else
                    {
                        model.DateCreated = DateTime.Now;
                        await _context.HSE_MinimumRequirements.AddAsync(model);
                    }
                }
                else
                {
                    var data = await (from d in _context.HSE_MinimumRequirements
                                      where d.Id.ToString() == id && d.CompanyNumber == WKPCompanyNumber
                                      select d).FirstOrDefaultAsync();

                    if (action == GeneralModel.Update)
                    {
                        if (data == null)
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This details could not be found.", StatusCode = ResponseCodes.Failure };
                        }
                        else
                        {
                            _context.HSE_MinimumRequirements.Remove(data);
                            model.DateCreated = DateTime.Now;
                            await _context.HSE_MinimumRequirements.AddAsync(model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_MinimumRequirements.Remove(data);
                    }
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Data has been " + action + "D successfully.";

                    var allData = await (from d in _context.HSE_MinimumRequirements
                                         where d.CompanyNumber == WKPCompanyNumber && d.ConcessionID == concessionField.Result.Concession_ID
                                         select d).ToListAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = allData, Message = successMsg, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : An error occured while trying to {actionToDo} this form.", StatusCode = ResponseCodes.Failure };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("Get_DecommisioningAbandonment")]
        public async Task<object> Get_DecommisioningAbandonment(string year, string omlName, string fieldName, string actionToDo)
        {
            try
            {
                var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
                var getData = await (from d in _context.DecommissioningAbandonments
                                     where
                                    d.CompanyNumber == WKPCompanyNumber && d.Year == int.Parse(year) &&
                                    d.ConcessionID == concessionField.Result.Concession_ID
                                     select d).FirstOrDefaultAsync();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = getData, StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpPost("Post_DecommisioningAbandonment")]
        public async Task<WebApiResponse> Post_DecommisioningAbandonment([FromBody] DecommissioningAbandonment model, string year, string omlName, string id, string actionToDo)
        {

            int save = 0;
            var concessionField = GET_CONCESSION_FIELD(omlName, "");
            string action = (id == "undefined" || actionToDo == null) ? GeneralModel.Insert : actionToDo;
            try
            {
                #region Saving Data

                model.ConcessionID = concessionField.Result.Concession_ID;
                model.CompanyNumber = WKPCompanyNumber;
                model.Year = int.Parse(year);

                if (action == GeneralModel.Insert)
                {
                    var data = await (from c in _context.DecommissioningAbandonments where c.CompanyNumber == WKPCompanyNumber && c.ConcessionID == concessionField.Result.Concession_ID && c.Year == int.Parse(year) select c).FirstOrDefaultAsync();

                    if (data != null)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This data is already existing and can not be duplicated.", StatusCode = ResponseCodes.Failure };
                    }
                    else
                    {
                        model.DateCreated = DateTime.Now;
                        await _context.DecommissioningAbandonments.AddAsync(model);
                    }
                }
                else
                {
                    var data = await (from d in _context.DecommissioningAbandonments
                                      where d.Id.ToString() == id && d.CompanyNumber == WKPCompanyNumber
                                      select d).FirstOrDefaultAsync();

                    if (action == GeneralModel.Update)
                    {
                        if (data == null)
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This details could not be found.", StatusCode = ResponseCodes.Failure };
                        }
                        else
                        {
                            _context.DecommissioningAbandonments.Remove(data);
                            model.DateCreated = DateTime.Now;
                            await _context.DecommissioningAbandonments.AddAsync(model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.DecommissioningAbandonments.Remove(data);
                    }
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Data has been " + action + "D successfully.";
                    var allData = await (from d in _context.DecommissioningAbandonments
                                         where d.CompanyNumber == WKPCompanyNumber && d.ConcessionID == concessionField.Result.Concession_ID
                                         select d).ToListAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = allData, Message = successMsg, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : An error occured while trying to {actionToDo} this form.", StatusCode = ResponseCodes.Failure };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("Get_Development_Production")]
        public async Task<object> Get_Development_Production(string year, string omlName, string fieldName)
        {
            try
            {
                var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
                var getData = await (from d in _context.Development_And_Productions
                                     where
                                    d.CompanyNumber == WKPCompanyNumber && d.Year == int.Parse(year) &&
                                    d.ConcessionID == concessionField.Result.Concession_ID
                                     select d).FirstOrDefaultAsync();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = getData, StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpPost("Post_Development_Production")]
        public async Task<WebApiResponse> Post_Development_Production([FromBody] Development_And_Production model, string year, string omlName, string id, string actionToDo)
        {

            int save = 0;
            var concessionField = GET_CONCESSION_FIELD(omlName, "");
            string action = (id == "undefined" || actionToDo == null) ? GeneralModel.Insert : actionToDo;
            try
            {
                #region Saving Data

                model.ConcessionID = concessionField.Result.Concession_ID;
                model.CompanyNumber = WKPCompanyNumber;
                model.Year = int.Parse(year);

                if (action == GeneralModel.Insert)
                {
                    var data = await (from c in _context.Development_And_Productions where c.CompanyNumber == WKPCompanyNumber && c.ConcessionID == concessionField.Result.Concession_ID && c.Year == int.Parse(year) select c).FirstOrDefaultAsync();

                    if (data != null)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This data is already existing and can not be duplicated.", StatusCode = ResponseCodes.Failure };
                    }
                    else
                    {
                        model.DateCreated = DateTime.Now;
                        await _context.Development_And_Productions.AddAsync(model);
                    }
                }
                else
                {
                    var data = await (from d in _context.Development_And_Productions
                                      where d.Id.ToString() == id && d.CompanyNumber == WKPCompanyNumber
                                      select d).FirstOrDefaultAsync();

                    if (action == GeneralModel.Update)
                    {
                        if (data == null)
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This details could not be found.", StatusCode = ResponseCodes.Failure };
                        }
                        else
                        {
                            _context.Development_And_Productions.Remove(data);
                            model.DateCreated = DateTime.Now;
                            await _context.Development_And_Productions.AddAsync(model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.Development_And_Productions.Remove(data);
                    }
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Data has been " + action + "D successfully.";
                    var allData = await (from d in _context.Development_And_Productions
                                         where d.CompanyNumber == WKPCompanyNumber && d.ConcessionID == concessionField.Result.Concession_ID
                                         select d).ToListAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = allData, Message = successMsg, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : An error occured while trying to {actionToDo} this form.", StatusCode = ResponseCodes.Failure };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_CONCESSION_FIELD")]
        public async Task<ConcessionField> GET_CONCESSION_FIELD(string omlName, string fieldName)
        {
            try
            {
                var concession = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.Company_ID == WKPCompanyId && d.Concession_Held == omlName && d.DELETED_STATUS == null select d).FirstOrDefaultAsync();
                var field = await (from d in _context.COMPANY_FIELDs where d.Field_Name == fieldName && d.DeletedStatus != true select d).FirstOrDefaultAsync();

                return new ConcessionField
                {
                    Concession_ID = concession.Consession_Id,
                    Concession_Name = concession.ConcessionName,
                    Consession_Type = concession.Consession_Type,
                    Terrain = concession.Terrain,
                    Field_Name = field?.Field_Name,
                    Field_ID = field?.Field_ID,
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        #endregion

    }
}
