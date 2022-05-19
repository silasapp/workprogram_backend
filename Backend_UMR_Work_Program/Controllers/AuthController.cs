using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend_UMR_Work_Program.Controllers;
using Backend_UMR_Work_Program.Models;
using LpgLicense.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Backend_UMR_Work_Program.Helpers;

namespace Backend_UMR_Work_Program.Controllers.Authentications
{
    public class AuthController : Controller
    {
        private readonly WKP_DBContext _context;
        IHttpContextAccessor _httpContextAccessor;
        public IConfiguration _configuration;
      
        ElpsResponse elpsResponse = new ElpsResponse();
        ElpsServices elpsServices = new ElpsServices();
        RestSharpServices _restService = new RestSharpServices();
        Helpers.Authentications auth = new Helpers.Authentications();

        HelpersController _helpersController;

        //session
        public const string sessionRelieveStaff = "_SessionRelieveStaff";
        public const string sessionEmail = "_sessionEmail";
        public const string sessionUserID = "_sessionUserID";
        public const string sessionFieldID = "_sessionFieldID";

        public const string sessionRoleID = "_sessionRoleID";
        public const string sessionLogin = "_sessionLogin";
        public const string sessionElpsID = "_sessionElpsID";
        public const string sessionRoleName = "_sessionRoleName";
        public const string sessionUserName = "_sessionUserName";
        public const string sessionTheme = "_sessionTheme";
        public const string sessionCompanyName = "_sessionCompanyName";
      
        public AuthController(WKP_DBContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
            //_helpersController = new HelpersController(_context, _configuration);

        }


        public async Task<IActionResult> UserAuth(string email, string code, string relievestaff=null)
        {
            var isSuccess = elpsServices.CodeCheck(email, code);

            if (isSuccess == true || isSuccess != true)
            {
                var log = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


                if (string.IsNullOrWhiteSpace(email))
                {
                    elpsResponse.message = "Oops... No result gotten from ELPS.";
                }
                else
                {
                    // Loginig from elps
                    var paramData = _restService.parameterData("staffEmail", email);
                    var response = await _restService.Response("/api/Accounts/Staff/{staffEmail}/{email}/{apiHash}", paramData); // GET

                    if (response.ErrorException != null)
                    {
                        elpsResponse.message = _restService.ErrorResponse(response);
                    }
                    else
                    {
                        var res = JsonConvert.DeserializeObject<LpgLicense.Models.Staff>(response.Content);
                        // checking local staff database for staff

                        if (res == null)
                        {
                            // no staff check for company
                            var paramData2 = _restService.parameterData("compemail", email);
                            var response2 = await _restService.Response("/api/company/{compemail}/{email}/{apiHash}", paramData2); // GET

                            if (response2.ErrorException != null)
                            {
                                elpsResponse.message = _restService.ErrorResponse(response2);
                            }
                            else
                            {
                                // checck company
                                var res2 = JsonConvert.DeserializeObject<CompanyDetail>(response2.Content);

                                if (res2 != null)
                                {

                                    string address = "";

                                    if (res2.registered_Address_Id != null || res2.registered_Address_Id != "0")
                                    {
                                        address = res2.registered_Address_Id;
                                    }
                                    else if (res2.operational_Address_Id != null || res2.operational_Address_Id != "0")
                                    {
                                        address = res2.operational_Address_Id;
                                    }

                                    // check address
                                    var paramDatas2 = _restService.parameterData("Id", address);
                                    var responses = await _restService.Response("/api/Address/ById/{Id}/{email}/{apiHash}", paramDatas2); // GET


                                    var _company = (from s in _context.ADMIN_COMPANY_DETAILs where s.Id == res2.id select s);

                                    if (_company.Count() > 0)
                                    {
                                        elpsResponse.message = email + " Company Found On Local DB.";
                                        elpsResponse.value = _company;

                                        if (responses != null)
                                        {
                                            var com = JsonConvert.DeserializeObject<Address>(responses.Content);

                                            if (com != null)
                                            {
                                                //save address
                                                //var codes = _helpersController.GetStateShortName(com.stateName.ToUpper(), "00" + _company.FirstOrDefault().Id);
                                                //_company.FirstOrDefault().CompanyCode = codes;
                                                _company.FirstOrDefault().Address_of_Company = com.address_1;
                                            }

                                            // address not found
                                            _company.FirstOrDefault().COMPANY_NAME = res2.name;
                                            _company.FirstOrDefault().EMAIL = res2.user_Id;
                                            _context.SaveChanges();
                                        }
                                        else
                                        {
                                            _company.FirstOrDefault().COMPANY_NAME = res2.name;
                                            _company.FirstOrDefault().EMAIL = res2.user_Id;
                                            _context.SaveChanges();
                                        }

                                        string roleID = ""; string roleName = "";
                                        var getRoleID = (from u in _context.Roles where u.Description == "Company" select u).FirstOrDefault();
                                        if (getRoleID != null)
                                            roleID = getRoleID.RoleId; roleName = getRoleID.Description;

                                        //Logins
                                        //Logins logins = new Logins()
                                        //{
                                        //    UserID = _company.FirstOrDefault().id,
                                        //    RoleID = roleID,
                                        //    HostName = auth.GetHostName(),
                                        //    MacAddress = auth.GetMACAddress(),
                                        //    Local_Ip = auth.GetLocal_IpAddress(),
                                        //    Remote_Ip = HttpContext.GetRemote_IpAddress().ToString(),
                                        //    UserAgent = Request.Headers["User-Agent"].ToString(),
                                        //    LoginTime = DateTime.Now,
                                        //    LoginStatus = "Logged in",
                                        //};

                                        //_context.Logins.Add(logins);
                                        //_context.SaveChanges();


                                        //int lastLoginID = logins.LoginID;


                                        var identity = new ClaimsIdentity(new[]
                                                   {
                                            new Claim(ClaimTypes.Role, roleName),

                                        }, CookieAuthenticationDefaults.AuthenticationScheme);

                                        var authProperties = new AuthenticationProperties
                                        {
                                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                                        };

                                        var principal = new ClaimsPrincipal(identity);

                                        HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();

                                        var getin = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                                        HttpContext.Session.SetString(sessionEmail, _helpersController.Encrypt(email));
                                        HttpContext.Session.SetString(sessionUserID, _helpersController.Encrypt(_company.FirstOrDefault().Id.ToString()));
                                        HttpContext.Session.SetString(sessionRoleID, _helpersController.Encrypt(roleID));
                                        //HttpContext.Session.SetString(sessionElpsID, _helpersController.Encrypt(_company.FirstOrDefault().elps_id.ToString()));
                                        HttpContext.Session.SetString(sessionRoleName, _helpersController.Encrypt(roleName));
                                        HttpContext.Session.SetString(sessionUserName, _helpersController.Encrypt(_company.FirstOrDefault().COMPANY_NAME));
                                        return RedirectToAction("Dashboard", "Companies");
                                    }
                                }
                                else
                                {
                                    elpsResponse.message = email + " This company was not found on WORK PROGRAMME PORTAL.... Creating company now.";

                                    ADMIN_COMPANY_INFORMATION companies = new ADMIN_COMPANY_INFORMATION()
                                    {
                                        //elps_id = res2.id,
                                        // user_id = res2.user_Id.Trim(),
                                        // name = res2.name.ToUpper(),
                                        // CompanyEmail=email,
                                        // business_type=res2.business_Type,
                                        // CompanyCode=res2.postal_Code,
                                        // contact_firstname=res2.contact_FirstName,
                                        // contact_lastname=res2.contact_LastName,
                                        // contact_phone=res2.contact_Phone,
                                        // City=res2.city,
                                        // StateName=res2.stateName,

                                        // //locat = _context.Location.Where(x => x.LocationName.Contains("CUS")).Select(x => x.LocationId).FirstOrDefault(),
                                        // RoleId = 0,
                                        // LocationId = 0,
                                        //// RoleId = _context.UserRoles.Where(x => x.RoleName.Contains("COMPANY")).Select(x => x.RoleId).FirstOrDefault(),
                                        // ActiveStatus = true,
                                        // CreatedAt = DateTime.Now,
                                        // DeleteStatus = false,
                                        // isFirstTime = true
                                    };

                                    _context.ADMIN_COMPANY_INFORMATIONs.Add(companies);

                                    int done = _context.SaveChanges();

                                    if (done > 0)
                                    {
                                        //    _helpersController.LogMessages(email + " Company Successfully created. Logging user in", companies.CompanyEmail.ToString());

                                        return RedirectToAction("UserAuth", "Auth", new { email = email });
                                    }
                                    else
                                    {
                                        elpsResponse.message = email + " Something went wrong trying to create your company on WEB PROGRAMME portal. please try again.";
                                    }

                                }
                                //else
                                //{
                                //    elpsResponse.message = email + " company was not found on ELPS and WORK PROGRAMME portal....";
                                //}
                            }
                        }

                        else
                        {
                            //    string elpsStaffEmail = res.email.Split('@')[0];

                            //    var _staff = (from s in _context.STAFF where s.StaffEmail.Contains(elpsStaffEmail.Trim()) select s);
                            //    var _staff2 = (from s in _context.Staff where s.StaffElpsID == res.userId select s);

                            //    if (_staff.Count() > 0)
                            //    {
                            //        elpsResponse.message = email + " Staff Found On Local DB.";
                            //        elpsResponse.value = _staff;

                            //        _staff.FirstOrDefault().StaffEmail = res.email;
                            //        _staff.FirstOrDefault().StaffElpsID = res.userId;
                            //        _context.SaveChanges();

                            //        if ((!string.IsNullOrWhiteSpace(_staff.FirstOrDefault().StaffEmail)) && _staff.FirstOrDefault().DeleteStatus == true)
                            //        {
                            //            elpsResponse.message = email + " You have been deleted from this portal. Please contact administrator.";
                            //            // redirect to deleted page page
                            //        }
                            //        else if ((!string.IsNullOrWhiteSpace(_staff.FirstOrDefault().StaffEmail)) && _staff.FirstOrDefault().ActiveStatus != true)
                            //        {
                            //            elpsResponse.message = email + " You have been deactivated on this portal. Please contact administrator.";
                            //            //redirect to deactivated page
                            //        }
                            //        else
                            //        {

                            //            Logins logins = new Logins()
                            //            {
                            //                UserID = _staff.FirstOrDefault().StaffID,
                            //                RoleID = (int)_staff.FirstOrDefault().RoleID,
                            //                HostName = auth.GetHostName(),
                            //                MacAddress = auth.GetMACAddress(),
                            //                Local_Ip = auth.GetLocal_IpAddress(),
                            //                Remote_Ip = HttpContext.GetRemote_IpAddress().ToString(),
                            //                UserAgent = Request.Headers["User-Agent"].ToString(),
                            //                LoginTime = DateTime.Now,
                            //                LoginStatus = "Logged in",
                            //            };

                            //            _context.Logins.Add(logins);
                            //            _context.SaveChanges();


                            //            int lastLoginID = logins.LoginID;

                            //            // get role name
                            //            var roleName = _context.UserRoles.Where(x => x.Role_id == _staff.FirstOrDefault().RoleID);

                            //            var identity = new ClaimsIdentity(new[]
                            //                       {
                            //                    new Claim(ClaimTypes.Role, roleName.FirstOrDefault().RoleName.ToString()),

                            //                }, CookieAuthenticationDefaults.AuthenticationScheme);

                            //            ClaimsPrincipal principal = new ClaimsPrincipal(identity);


                            //            var authProperties = new AuthenticationProperties
                            //            {
                            //                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                            //            };

                            //            //HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).Wait();
                            //            var getin = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                            //            if (relievestaff != null)
                            //            {
                            //                HttpContext.Session.SetString(sessionRelieveStaff, _helpersController.Encrypt(relievestaff));
                            //            }
                            //            HttpContext.Session.SetString(sessionEmail, _helpersController.Encrypt(res.email));
                            //            HttpContext.Session.SetString(sessionUserID, _helpersController.Encrypt(_staff.FirstOrDefault().StaffID.ToString()));
                            //            HttpContext.Session.SetString(sessionRoleID, _helpersController.Encrypt(_staff.FirstOrDefault().RoleID.ToString()));
                            //            HttpContext.Session.SetString(sessionFieldID, _helpersController.Encrypt(_staff.FirstOrDefault().FieldOfficeID.ToString()));
                            //            HttpContext.Session.SetString(sessionElpsID, _helpersController.Encrypt(_staff.FirstOrDefault().StaffElpsID.ToString()));
                            //            HttpContext.Session.SetString(sessionLogin, _helpersController.Encrypt(lastLoginID.ToString()));
                            //            HttpContext.Session.SetString(sessionRoleName, _helpersController.Encrypt(roleName.FirstOrDefault().RoleName));
                            //            HttpContext.Session.SetString(sessionUserName, _helpersController.Encrypt(_staff.FirstOrDefault().FirstName));
                            //            HttpContext.Session.SetString(sessionTheme, _helpersController.Encrypt(_staff.FirstOrDefault().Theme));
                            //            HttpContext.Session.SetString(sessionTheme, _helpersController.Encrypt(_staff.FirstOrDefault().Theme));

                            //            if (identity.Claims.FirstOrDefault().Value.ToString().Contains(roleName.FirstOrDefault().RoleName.ToString()))
                            //            {
                            //                _helpersController.LogMessages(email + " Staff Successfully logged in", _helpersController.getSessionEmail());

                            //                return RedirectToAction("Dashboard", "Staffs");
                            //            }
                            //        }
                            //    }
                            //    else
                            //    {
                            //        Backend_UMR_Work_Program.Models.Staff staff = new Backend_UMR_Work_Program.Models.Staff()
                            //        {

                            //            StaffElpsID = res.userId.Trim(),
                            //            FieldOfficeID = 0,
                            //            RoleID = 0,
                            //            LocationID = 0,
                            //            UpdatedBy = 0,
                            //            StaffEmail = res.email.Trim(),
                            //            FirstName = res.firstName.ToUpper(),
                            //            LastName = res.lastName.ToUpper(),
                            //            CreatedAt = DateTime.Now,
                            //            ActiveStatus = false,
                            //            DeleteStatus = false,
                            //            Theme = "Light"
                            //        };

                            //        _context.Staff.Add(staff);
                            //        int saved = _context.SaveChanges();

                            //        int lastStaffID = staff.StaffID;

                            //        var updateStaff = from s in _context.Staff where s.StaffID == lastStaffID select s;
                            //        updateStaff.FirstOrDefault().CreatedBy = lastStaffID;
                            //        int save2 = _context.SaveChanges(); // updating self creating staff.

                            //        if (saved > 0)
                            //        {
                            //            elpsResponse.message = email + " Staff successfully created from elps but not active";
                            //        }
                            //        else
                            //        {
                            //            elpsResponse.message = email + " Staff not created. Try again later.";
                            //        }
                            //    }
                        }
                    }
                }
            }
                return RedirectToAction("Error", "Home", new { message = _helpersController.Encrypt(elpsResponse.message) });
            
          }




        public ContentResult Logout()
        {
            string publicKey = _configuration.GetSection("ElpsKeys").GetSection("PK").Value.ToString();
            var elpsLogoff = ElpsServices._elpsBaseUrl + "Account/RemoteLogOff";
            var returnUrl = Url.Action("Index", "Home", null, Request.Scheme);

            var frm = "<form action='" + elpsLogoff + "' id='frmTest' method='post'>" +
                    "<input type='hidden' name='returnUrl' value='" + returnUrl + "' />" +
                    "<input type='hidden' name='appId' value='" + publicKey + "' />" +
                    "</form>" +
                    "<script>document.getElementById('frmTest').submit();</script>";
            var login = _helpersController.getSessionLogin().ToString();

            if (string.IsNullOrWhiteSpace(login))
            {
                return Content(frm, "text/html");
            }
            else
            {
                //var logins = _context.Logins.Where(x => x.LoginId == _helpersController.getSessionLogin());

                //logins.FirstOrDefault().LoginStatus = "Logout";
                //logins.FirstOrDefault().LogoutTime = DateTime.Now;
                //_context.SaveChanges();

              //  _helpersController.LogMessages(_helpersController.getSessionEmail() + " logged out successfully...", _helpersController.getSessionEmail());

                HttpContext.Session.Remove(sessionEmail);
                HttpContext.Session.Remove(sessionUserID);
                HttpContext.Session.Remove(sessionRoleID);
                HttpContext.Session.Remove(sessionElpsID);
                HttpContext.Session.Remove(sessionLogin);
                HttpContext.Session.Remove(sessionRoleName);
                HttpContext.Session.Remove(sessionUserName);
                HttpContext.Session.Remove(sessionTheme);

                var log = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return Content(frm, "text/html");
            }
        }




        public IActionResult ChangePassword()
        {
            return View();
        }





        public async Task<JsonResult> ChangePasswordAction(string OldPassword, string NewPassword, string ConfirmPassword)
        {
            string result = "";

            LpgLicense.Models.ChangePassword changePassword = new ChangePassword()
            {
                oldPassword = OldPassword,
                newPassword = NewPassword,
                confirmPassword = ConfirmPassword
            };

            var paramData = _restService.parameterData("useremail", _helpersController.getSessionEmail());
            var response = await _restService.Response("/api/Accounts/ChangePassword/{useremail}/{email}/{apiHash}", paramData, "POST", changePassword); // GET

            if (response.ErrorException != null)
            {
                result = _restService.ErrorResponse(response);
            }
            else
            {
                var res = JsonConvert.DeserializeObject<LpgLicense.Models.ChangePasswordResponse>(response.Content);

                if (res.code == 1)
                {
                    result = "Password Changed";
                }
                else
                {
                    result = "Password not change. " + res.msg;
                }
            }

            //_helpersController.LogMessages("Result for user changing password " + result, _helpersController.getSessionEmail());

            return Json(result);

        }

    }
}