﻿using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore;

namespace Backend_UMR_Work_Program.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]

    public class AdminController : Controller
    {
        private Account _account;
        public WKP_DBContext _context;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AdminController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController, IMapper mapper)
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

        #region Company Codes

        [HttpGet("GET_COMPANYCODES")]
        public async Task<WebApiResponse> Get_CompanyCodes()
        {
            try
            {
                var getCodes = (from c in _context.ADMIN_COMPANY_CODEs
                                select c).ToList();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getCodes, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpGet("GET_COMPANYCODE_UPDATE")]
        public async Task<WebApiResponse> Get_CompanyCodeUpdate(int Id)
        {
            try
            {
                var getCode = (from c in _context.ADMIN_COMPANY_CODEs where c.Id == Id select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getCode, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("POST_COMPANYCODESTATUS_UPDATE")]
        public async Task<WebApiResponse> Get_CompanyCodeStatusUpdate(int Id, string Status)
        {
            try
            {
                var getCode = (from c in _context.ADMIN_COMPANY_CODEs where c.Id == Id select c).FirstOrDefault();
                if(getCode != null)
                {
                    getCode.IsActive = Status;
                    await _context.SaveChangesAsync();

                    var getCCode = (from c in _context.ADMIN_COMPANY_CODEs where c.Id == Id select c).FirstOrDefault();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getCCode, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : Company code details was not found.", StatusCode = ResponseCodes.Badrequest };
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("UPLOAD_COMPANYCODE")]
        public async Task<WebApiResponse>Upload_CompanyCode(string CompanyName, string CompanyCode, string CompanyEmail)
        {
            try
            {
                int save = 0;
                List<string> companyCodes = new List<string> { CompanyName, CompanyCode };
                //List<string> companyCodes = new List<string> { CompanyName, CompanyCode, CompanyEmail };
                foreach(var companyCode in companyCodes)
                {
                    var getCode = (from c in _context.ADMIN_COMPANY_CODEs where c.CompanyCode == CompanyCode && c.IsActive == "YES" select c).FirstOrDefault();
                    if (getCode == null)
                    {
                        var company = (from c in _context.ADMIN_COMPANY_INFORMATIONs
                                       where c.NAME.ToLower() == CompanyName.ToLower() /*|| c.EMAIL.ToLower() == CompanyEmail.ToLower()*/
                                       select c).FirstOrDefault();
                        if (company != null)
                        {
                            getCode.CompanyName = CompanyName.ToUpper().Trim();
                            getCode.Email = CompanyEmail.ToLower().Trim();
                            //getCode.CompanyNumber = company.Id;
                            getCode.CompanyCode = CompanyCode.Trim();
                            getCode.IsActive = "YES";
                            await _context.ADMIN_COMPANY_CODEs.AddAsync(getCode);
                            save = await _context.SaveChangesAsync();
                        }
                    }
                }
                if (save > 0)
                {
                    var getCodes = (from c in _context.ADMIN_COMPANY_CODEs select c).ToList();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getCodes, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : Company code details was not found.", StatusCode = ResponseCodes.Badrequest };
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        #endregion

        #region User Administration

        [HttpGet("GET_USERS")]
        public async Task<WebApiResponse> Get_Users()
        {
            try
            {
                var companyInformation = (from c in _context.ADMIN_COMPANY_INFORMATIONs
                                          where c.COMPANY_NAME != GeneralModel.Admin && c.DELETED_STATUS == null
                                          select c).ToList();
                var staffInformation = (from c in _context.ADMIN_COMPANY_INFORMATIONs
                                          where c.COMPANY_NAME == GeneralModel.Admin && c.DELETED_STATUS == null
                                          select c).ToList();
                var userRoles =        _context.ROLES_s.ToList();

                var returnData = new UserModel()
                {
                  companiesList =  companyInformation, staffList = staffInformation, roles = userRoles
                };
                    
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = returnData, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }      
        
        [HttpGet("GET_COMPANIES")]
        public async Task<WebApiResponse> Get_Companies()
        {
            try
            {
                var companyInformation = (from c in _context.ADMIN_COMPANY_INFORMATIONs
                                          where c.COMPANY_NAME != GeneralModel.Admin && c.DELETED_STATUS == null
                                          select c).ToList();
             
                    
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = companyInformation, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("CREATE_USER")]
        public async Task<WebApiResponse> CreateUser(ADMIN_COMPANY_INFORMATION_Model userModel)
        {
            try
            {
                var checkUser = (from c in _context.ADMIN_COMPANY_INFORMATIONs
                                 where c.EMAIL.ToLower() == userModel.EMAIL.ToLower()
                                 select c).FirstOrDefault();
                if (checkUser != null)
                {
                    bool deleted = checkUser.DELETED_STATUS == "DELETED" ? true : false;
                    bool activated = checkUser.STATUS_ == "Activated" ? true : false;
                    string errMsg = $"User details with '{userModel.EMAIL}' is already existing on the portal.";

                    if (deleted == true)
                        errMsg = $"User details with '{userModel.EMAIL}' is already existing, but the account has been deleted on the portal, kindly restore account information.";

                    if (activated != true)
                        errMsg = $"User details with '{userModel.EMAIL}' is already existing, but the account has been de-activated on the portal, kindly activate account information.";

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + errMsg, StatusCode = ResponseCodes.Failure };

                }
                else
                {
                    var data = _mapper.Map<ADMIN_COMPANY_INFORMATION>(userModel);
                    data.EMAIL = userModel.EMAIL.ToLower();
                    data.PASSWORDS = _helpersController.Encrypt(userModel.PASSWORDS);
                    data.STATUS_ = "Activated";
                    data.Date_Created = DateTime.Now;
                    data.Created_by = WKPCompanyEmail;
                    await _context.ADMIN_COMPANY_INFORMATIONs.AddAsync(data);
                    int save = await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"{userModel.EMAIL} has been added successfully", Data = userModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "An error occured while adding this user.", StatusCode = ResponseCodes.Failure };
                    }
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpGet("GET_UPDATEUSER")]
        public async Task<WebApiResponse> Get_UpdateUser(int id)
        {
            try
            {
                var checkUser = (from c in _context.ADMIN_COMPANY_INFORMATIONs
                                 where c.Id == id
                                 select c).FirstOrDefault();
                if (checkUser == null)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: User information was not found.", StatusCode = ResponseCodes.Failure };

                }
                else
                {
                    var data = _mapper.Map<ADMIN_COMPANY_INFORMATION_Model>(checkUser);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"Success", Data = data, StatusCode = ResponseCodes.Success };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("UPDATE_USER")]
        public async Task<WebApiResponse> UpdateUser(ADMIN_COMPANY_INFORMATION_Model userModel)
        {
            try
            {
                var checkUser = (from c in _context.ADMIN_COMPANY_INFORMATIONs
                                 where c.Id == userModel.Id
                                 select c).FirstOrDefault();
                if (checkUser == null)
                {   
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: User information was not found for {userModel.EMAIL}", StatusCode = ResponseCodes.Failure };

                }
                else
                {
                    var data = _mapper.Map<ADMIN_COMPANY_INFORMATION>(userModel);
                    data.EMAIL = userModel.EMAIL.ToLower();
                    data.PASSWORDS = _helpersController.Encrypt(userModel.PASSWORDS);
                    data.STATUS_ = "Activated";
                    data.UPDATED_DATE = DateTime.Now.ToString();
                    data.UPDATED_BY = WKPCompanyEmail;
                    int save = await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"User information has been updated successfully", Data = userModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "An error occured while adding this user.", StatusCode = ResponseCodes.Failure };
                    }
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("DELETE_USER")]
        public async Task<WebApiResponse> DeleteUser(int Id)
        {
            try
            {
                var checkUser = (from c in _context.ADMIN_COMPANY_INFORMATIONs
                                 where c.Id == Id
                                 select c).FirstOrDefault();
                if (checkUser == null)
                {   
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: User information was not found.", StatusCode = ResponseCodes.Failure };

                }
                else
                {
                    checkUser.DELETED_STATUS = "DELETED";
                    checkUser.DELETED_DATE = DateTime.Now.ToString();
                    checkUser.DELETED_BY = WKPCompanyEmail;
                    int save = await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"User information has been updated successfully", StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "An error occured while adding this user.", StatusCode = ResponseCodes.Failure };
                    }
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("RESTORE_USER")]
        public async Task<WebApiResponse> RestoreUser(int Id)
        {
            try
            {
                var checkUser = (from c in _context.ADMIN_COMPANY_INFORMATIONs
                                 where c.Id == Id
                                 select c).FirstOrDefault();
                if (checkUser == null)
                {   
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: User information was not found.", StatusCode = ResponseCodes.Failure };

                }
                else
                {
                    checkUser.DELETED_STATUS = null;
                    checkUser.DELETED_DATE = null;
                    checkUser.DELETED_BY = null;
                    int save = await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"User information has been updated successfully", StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "An error occured while adding this user.", StatusCode = ResponseCodes.Failure };
                    }
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        #endregion

        #region Concession Administration

        [HttpGet("GET_CONCESSIONS")]
        public async Task<WebApiResponse> Get_Concessions(string year)
        {
            try
            {
                var data = (from c in _context.ADMIN_CONCESSIONS_INFORMATIONs
                                          where c.DELETED_STATUS == null select c).GroupBy(x => x.Concession_Unique_ID).Select(x => x.FirstOrDefault()).ToList();
                if(year != null)
                {
                    data = data.Where(d => d.Year == year.Trim()).ToList();
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = data, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
       
        [HttpPost("CREATE_CONCESSION")]
        public async Task<WebApiResponse> CreateConcession(ADMIN_CONCESSIONS_INFORMATION_Model concessionModel)
        {
            try
            {
                var checkConcession = (from c in _context.ADMIN_CONCESSIONS_INFORMATIONs
                                       where c.Concession_Unique_ID.ToLower() == concessionModel.Concession_Unique_ID.ToLower()
                                 select c).FirstOrDefault();
                if (checkConcession != null)
                {
                    bool deleted = checkConcession.DELETED_STATUS == "DELETED" ? true : false;
                    string errMsg = $"Concession details with '{concessionModel.Concession_Unique_ID}' is already existing on the portal.";

                    if (deleted == true)
                        errMsg = $"Concession details with '{concessionModel.Concession_Unique_ID}' is already existing, but the information has been deleted on the portal, kindly restore concession information.";

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + errMsg, StatusCode = ResponseCodes.Failure };

                }
                else
                {
                     var company = (from c in _context.ADMIN_COMPANY_INFORMATIONs
                                        where c.EMAIL.ToLower() == concessionModel.COMPANY_EMAIL.ToLower()
                                       select c).FirstOrDefault();
                    if (company != null)
                    {
                        var data = _mapper.Map<ADMIN_CONCESSIONS_INFORMATION>(concessionModel);
                        data.CompanyNumber = company.CompanyNumber;
                        data.Company_ID = company.COMPANY_ID;
                        data.Date_Created = DateTime.Now;
                        data.Created_by = WKPCompanyEmail;
                        await _context.ADMIN_CONCESSIONS_INFORMATIONs.AddAsync(data);
                        int save = await _context.SaveChangesAsync();
                        if (save > 0)
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"{concessionModel.Concession_Unique_ID} has been added successfully", Data = concessionModel, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "An error occured while adding this concession.", StatusCode = ResponseCodes.Failure };
                        }
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "Concession owner/company details was not found.", StatusCode = ResponseCodes.Failure };
                    }
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpGet("GET_UPDATECONCESSION")]
        public async Task<WebApiResponse> Get_UpdateConcession(int id)
        {
            try
            {
                var checkConcession = (from c in _context.ADMIN_CONCESSIONS_INFORMATIONs
                                       where c.Consession_Id == id
                                 select c).FirstOrDefault();
                if (checkConcession == null)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: Concession information was not found.", StatusCode = ResponseCodes.Failure };

                }
                else
                {
                    var data = _mapper.Map<ADMIN_CONCESSIONS_INFORMATION_Model>(checkConcession);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"Success", Data = data, StatusCode = ResponseCodes.Success };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("UPDATE_CONCESSION")]
        public async Task<WebApiResponse> UpdateConcession(ADMIN_CONCESSIONS_INFORMATION_Model concessionModel)
        {
            try
            {
                var checkConcession = (from c in _context.ADMIN_CONCESSIONS_INFORMATIONs
                                 where c.Consession_Id == concessionModel.Consession_Id
                                 select c).FirstOrDefault();
                if (checkConcession == null)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: Concession information was not found for {concessionModel.Concession_Unique_ID}", StatusCode = ResponseCodes.Failure };

                }
                else
                {
                    var data = _mapper.Map<ADMIN_COMPANY_INFORMATION>(concessionModel);
                    data.EMAIL = concessionModel.COMPANY_EMAIL.ToLower();
                    data.UPDATED_DATE = DateTime.Now.ToString();
                    data.UPDATED_BY = WKPCompanyEmail;
                    int save = await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"Concession information has been updated successfully.", Data = concessionModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "An error occured while adding this concession.", StatusCode = ResponseCodes.Failure };
                    }
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("DELETE_CONCESSION")]
        public async Task<WebApiResponse> DeleteConcession(int Id)
        {
            try
            {
                var checkConcession = (from c in _context.ADMIN_CONCESSIONS_INFORMATIONs
                                 where c.Consession_Id == Id
                                 select c).FirstOrDefault();
                if (checkConcession == null)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: Concession information was not found.", StatusCode = ResponseCodes.Failure };

                }
                else
                {
                    checkConcession.DELETED_STATUS = "DELETED";
                    checkConcession.DELETED_DATE = DateTime.Now.ToString();
                    checkConcession.DELETED_BY = WKPCompanyEmail;
                    int save = await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"Concession information has been updated successfully", StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "An error occured while adding this concession.", StatusCode = ResponseCodes.Failure };
                    }
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("RESTORE_CONCESSION")]
        public async Task<WebApiResponse> RestoreConcession(int Id)
        {
            try
            {
                var checkConcession = (from c in _context.ADMIN_CONCESSIONS_INFORMATIONs
                                 where c.Consession_Id == Id
                                 select c).FirstOrDefault();
                if (checkConcession == null)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: Concession information was not found.", StatusCode = ResponseCodes.Failure };

                }
                else
                {
                    checkConcession.DELETED_STATUS = null;
                    checkConcession.DELETED_DATE = null;
                    checkConcession.DELETED_BY = null;
                    int save = await _context.SaveChangesAsync();
                    if (save > 0)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"Concession information has been updated successfully", StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "An error occured while adding this concession.", StatusCode = ResponseCodes.Failure };
                    }
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        #endregion

        [HttpGet("GET_COMPANYLIST")]
        public async Task<WebApiResponse> Get_CompanyList()
        {
            try
            {
                var data = (from c in _context.ADMIN_COMPANY_DETAILs select c).ToList();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = data, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        #region Report Editor Section
        [HttpGet("GET_REPORTS")]
        public async Task<WebApiResponse> Get_Reports()
        {
            try
            {
                var data = _context.ADMIN_WORK_PROGRAM_REPORTs.Where(x => x.Id <= 5).ToList();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = data, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
      
        [HttpPost("UPDATE_REPORTS")]
        public async Task<WebApiResponse> UpdateReports(List<ADMIN_WORK_PROGRAM_REPORT> reports)
        {
            try
            {
                if (reports == null && reports.Count > 0)
                {
                    foreach (var x in reports)
                    {
                        var checkReport = (from c in _context.ADMIN_WORK_PROGRAM_REPORTs
                                           where c.Id == x.Id
                                           select c).FirstOrDefault();
                        if (checkReport == null)
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: Report details could not found on the portal.", StatusCode = ResponseCodes.Failure };
                        }
                        else
                        {
                            checkReport.Report_Content = x.Report_Content;
                            checkReport.Date_Updated = DateTime.Now;
                            checkReport.Updated_by = WKPCompanyEmail;
                            int save = await _context.SaveChangesAsync();
                            if (save > 0)
                            {
                                var data = _context.ADMIN_WORK_PROGRAM_REPORTs.Where(x => x.Id <= 5).ToList();
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"Report(s) have been updated successfully.", Data = data, StatusCode = ResponseCodes.Success };
                            }
                            else
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "An error occured while adding this concession.", StatusCode = ResponseCodes.Failure };
                            }
                        }
                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "Report(s) data is null/empty.", StatusCode = ResponseCodes.Failure };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error: " + "Report(s) data is null/empty.", StatusCode = ResponseCodes.Failure };
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        #endregion

        #region Parameters Configuration
        [HttpGet("GET_PARAMETERSCONFIGURATION")]
        public async Task<WebApiResponse> Get_ParametersConfiguration()
        {
            try
            {
                var adminCategories =        _context.ADMIN_CATEGORIEs.ToList();
                var dataTypes =              _context.Data_Types.ToList();
                var wellCategories =         _context.ADMIN_WELL_CATEGORIEs.ToList();
                var startEndDate =           _context.ADMIN_WP_START_END_DATEs.ToList();
                var startEndDateUpload =     _context.ADMIN_WP_START_END_DATE_DATA_UPLOADs.ToList();
                var penalties =              _context.ADMIN_WP_PENALTIEs.ToList();
                var emailDays =              _context.ADMIN_EMAIL_DAYs.Where(x=> x.Deleted_status == null).ToList();
                var superAdmins =            _context.ROLES_SUPER_ADMINs.Where(x=> x.Deleted_status == null).OrderBy(x=> x.Date_Created).ToList();
                var presentationCategories = _context.ADMIN_PRESENTATION_CATEGORIEs.ToList();
                var meetingRooms =           _context.ADMIN_MEETING_ROOMs.ToList();

                var returnData = new parameterConfigModel()
                {
                    adminCategories = adminCategories,
                    dataTypes = dataTypes,  
                    wellCategories = wellCategories,
                    startEndDate = startEndDate,
                    startEndDateUpload = startEndDateUpload,
                    penalties = penalties,
                    emailDays = emailDays,
                    superAdmins = superAdmins,
                    presentationCategories = presentationCategories,
                    meetingRooms = meetingRooms,
                };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = returnData, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("ADMIN_CATEGORIES")]
        public async Task<WebApiResponse> Admin_Categories(ADMIN_CATEGORy model,string action)
        {
            try
            {
                int save = 0;
                if (model != null)
                {
                    switch (action) {
                        case "INSERT":  
                            var getdata = _context.ADMIN_CATEGORIEs.Where(x => x.categories.ToLower() == model.categories.Trim().ToLower()).FirstOrDefault();
                            if (getdata != null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: {model.categories} is already existing on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                var data = new ADMIN_CATEGORy()
                                {
                                    categories = model.categories,
                                    categories_Desc = model.categories_Desc,
                                    Date_Created = DateTime.Now,
                                    Created_by = WKPCompanyEmail
                                };
                                await _context.ADMIN_CATEGORIEs.AddAsync(data);
                                save = await _context.SaveChangesAsync();

                            }
                            break;
                      
                        default:
                            var checkdata = _context.ADMIN_CATEGORIEs.Where(x => x.Id == model.Id).FirstOrDefault();
                            if (checkdata == null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: {model.categories} details could not be found on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                if (action == GeneralModel.Update)
                                {
                                    checkdata.categories = model.categories;
                                    checkdata.categories_Desc = model.categories_Desc;
                                    checkdata.Date_Updated = DateTime.Now;
                                    checkdata.Updated_by = WKPCompanyEmail;
                                }
                                else if (action == GeneralModel.Delete)
                                {
                                   _context.ADMIN_CATEGORIEs.Remove(checkdata);
                                }
                                save = await _context.SaveChangesAsync();
                               
                            }
                            
                         break;
                      }
                   
                    if (save > 0)
                    {
                        var returnModel = _context.ADMIN_CATEGORIEs.ToList();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"{model.categories} has been {action}D successfully", Data = returnModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: An error occured while trying to {action} {model.categories} category.", StatusCode = ResponseCodes.Failure };
                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: model data was not passed correctly.", StatusCode = ResponseCodes.Failure };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("DATA_TYPES")]
        public async Task<WebApiResponse> Data_Types(Data_Type model,string action)
        {
            try
            {
                int save = 0;
                if (model != null)
                {
                    switch (action) {
                        case "INSERT":  
                            var getdata = _context.Data_Types.Where(x => x.DataType.ToLower() == model.DataType.Trim().ToLower()).FirstOrDefault();
                            if (getdata != null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: {model.DataType} is already existing on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                var data = new Data_Type()
                                {
                                    DataType = model.DataType, 
                                    Date_Created = DateTime.Now,
                                    Created_by = WKPCompanyEmail
                                };
                                await _context.Data_Types.AddAsync(data);
                                save = await _context.SaveChangesAsync();

                            }
                            break;
                      
                        default:
                            var checkdata = _context.Data_Types.Where(x => x.DataTypeId == model.DataTypeId).FirstOrDefault();
                            if (checkdata == null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: {model.DataType} details could not be found on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                if (action == GeneralModel.Update)
                                {
                                    checkdata.DataType = model.DataType;
                                }
                                else if (action == GeneralModel.Delete)
                                {
                                   _context.Data_Types.Remove(checkdata);
                                }
                                save = await _context.SaveChangesAsync();
                               
                            }
                            
                         break;
                      }
                   
                    if (save > 0)
                    {
                        var returnModel = _context.Data_Types.ToList();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"{model.DataType} has been {action}D successfully", Data = returnModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: An error occured while trying to {action} {model.DataType} data type.", StatusCode = ResponseCodes.Failure };
                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: model data was not passed correctly.", StatusCode = ResponseCodes.Failure };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("ADMIN_WELL_CATEGORIES")]
        public async Task<WebApiResponse> Admin_Well_Categories(ADMIN_WELL_CATEGORy model,string action)
        {
            try
            {
                int save = 0;
                if (model != null)
                {
                    switch (action) {
                        case "INSERT":  
                            var getdata = _context.ADMIN_WELL_CATEGORIEs.Where(x => x.welltype.ToLower() == model.welltype.Trim().ToLower()).FirstOrDefault();
                            if (getdata != null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: {model.welltype} is already existing on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                var data = new ADMIN_WELL_CATEGORy()
                                {
                                    welltype = model.welltype,
                                    categories_Desc = model.categories_Desc,
                                    Date_Created = DateTime.Now,
                                    Created_by = WKPCompanyEmail
                                };
                                await _context.ADMIN_WELL_CATEGORIEs.AddAsync(data);
                                save = await _context.SaveChangesAsync();

                            }
                            break;
                      
                        default:
                            var checkdata = _context.ADMIN_WELL_CATEGORIEs.Where(x => x.Id == model.Id).FirstOrDefault();
                            if (checkdata == null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: {model.welltype} details could not be found on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                if (action == GeneralModel.Update)
                                {
                                    checkdata.welltype = model.welltype;
                                    checkdata.categories_Desc = model.categories_Desc;
                                    checkdata.Date_Updated = DateTime.Now;
                                    checkdata.Updated_by = WKPCompanyEmail;
                                }
                                else if (action == GeneralModel.Delete)
                                {
                                   _context.ADMIN_WELL_CATEGORIEs.Remove(checkdata);
                                }
                                save = await _context.SaveChangesAsync();
                               
                            }
                            
                         break;
                      }
                   
                    if (save > 0)
                    {
                        var returnModel = _context.ADMIN_CATEGORIEs.ToList();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"{model.welltype} has been {action}D successfully", Data = returnModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: An error occured while trying to {action} {model.welltype} category.", StatusCode = ResponseCodes.Failure };
                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: model data was not passed correctly.", StatusCode = ResponseCodes.Failure };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("ADMIN_START_END_DATES")]
        public async Task<WebApiResponse> Admin_StartEnd_Date(ADMIN_WP_START_END_DATE model,string action)
        {
            try
            {
                int save = 0;
                if (model != null)
                {
                    switch (action) {
                        case "INSERT":  
                            var getdata = _context.ADMIN_WP_START_END_DATEs.Where(x => x.start_date == model.start_date && x.end_date == model.end_date).FirstOrDefault();
                            if (getdata != null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: start & end dates are already existing on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                var data = new ADMIN_WP_START_END_DATE()
                                {
                                    start_date = model.start_date,
                                    end_date = model.end_date,
                                    categories_Desc = model.categories_Desc,
                                    Date_Created = DateTime.Now,
                                    Created_by = WKPCompanyEmail
                                };
                                await _context.ADMIN_WP_START_END_DATEs.AddAsync(data);
                                save = await _context.SaveChangesAsync();

                            }
                            break;
                      
                        default:
                            var checkdata = _context.ADMIN_WP_START_END_DATEs.Where(x => x.Id == model.Id).FirstOrDefault();
                            if (checkdata == null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: start & end dates details could not be found on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                if (action == GeneralModel.Update)
                                {
                                    checkdata.start_date = model.start_date;
                                    checkdata.end_date = model.end_date;
                                    checkdata.categories_Desc = model.categories_Desc;
                                    checkdata.Date_Updated = DateTime.Now;
                                    checkdata.Updated_by = WKPCompanyEmail;
                                }
                                else if (action == GeneralModel.Delete)
                                {
                                   _context.ADMIN_WP_START_END_DATEs.Remove(checkdata);
                                }
                                save = await _context.SaveChangesAsync();
                               
                            }
                            
                         break;
                      }
                   
                    if (save > 0)
                    {
                        var returnModel = _context.ADMIN_WP_START_END_DATEs.ToList();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"WKP start & end dates have been {action}D successfully", Data = returnModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: An error occured while trying to {action} start & end dates.", StatusCode = ResponseCodes.Failure };
                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: model data was not passed correctly.", StatusCode = ResponseCodes.Failure };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("ADMIN_START_END_DATE_DATA_UPLOADS")]
        public async Task<WebApiResponse> Admin_StartEnd_Date(ADMIN_WP_START_END_DATE_DATA_UPLOAD model,string action)
        {
            try
            {
                int save = 0;
                if (model != null)
                {
                    switch (action) {
                        case "INSERT":  
                            var getdata = _context.ADMIN_WP_START_END_DATE_DATA_UPLOADs.Where(x => x.start_date == model.start_date && x.end_date == model.end_date).FirstOrDefault();
                            if (getdata != null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: start & end data uploads are already existing on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                var data = new ADMIN_WP_START_END_DATE()
                                {
                                    start_date = model.start_date,
                                    end_date = model.end_date,
                                    categories_Desc = model.categories_Desc,
                                    Date_Created = DateTime.Now,
                                    Created_by = WKPCompanyEmail
                                };
                                await _context.ADMIN_WP_START_END_DATEs.AddAsync(data);
                                save = await _context.SaveChangesAsync();

                            }
                            break;
                      
                        default:
                            var checkdata = _context.ADMIN_WP_START_END_DATEs.Where(x => x.Id == model.Id).FirstOrDefault();
                            if (checkdata == null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: start & end dates details could not be found on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                if (action == GeneralModel.Update)
                                {
                                    checkdata.start_date = model.start_date;
                                    checkdata.end_date = model.end_date;
                                    checkdata.categories_Desc = model.categories_Desc;
                                    checkdata.Date_Updated = DateTime.Now;
                                    checkdata.Updated_by = WKPCompanyEmail;
                                }
                                else if (action == GeneralModel.Delete)
                                {
                                   _context.ADMIN_WP_START_END_DATEs.Remove(checkdata);
                                }
                                save = await _context.SaveChangesAsync();
                               
                            }
                            
                         break;
                      }
                   
                    if (save > 0)
                    {
                        var returnModel = _context.ADMIN_WP_START_END_DATEs.ToList();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"WKP start & end dates have been {action}D successfully", Data = returnModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: An error occured while trying to {action} start & end dates.", StatusCode = ResponseCodes.Failure };
                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: model data was not passed correctly.", StatusCode = ResponseCodes.Failure };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("ADMIN_PENALTIES")]
        public async Task<WebApiResponse> Admin_Penalties(ADMIN_WP_PENALTy model,string action)
        {
            try
            {
                int save = 0;
                if (model != null)
                {
                    switch (action) {
                        case "INSERT":  
                            var getdata = _context.ADMIN_WP_PENALTIEs.Where(x => x.NO_SUBMISSION == model.NO_SUBMISSION && x.NO_SHOW == model.NO_SHOW ).FirstOrDefault();
                            if (getdata != null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: penalty config is already existing on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                var data = new ADMIN_WP_PENALTy()
                                {
                                    NO_SHOW = model.NO_SHOW,
                                    NO_SUBMISSION = model.NO_SUBMISSION,
                                    categories_Desc = model.categories_Desc,
                                    Date_Created = DateTime.Now,
                                    Created_by = WKPCompanyEmail
                                };
                                await _context.ADMIN_WP_PENALTIEs.AddAsync(data);
                                save = await _context.SaveChangesAsync();

                            }
                            break;
                      
                        default:
                            var checkdata = _context.ADMIN_WP_PENALTIEs.Where(x => x.Id == model.Id).FirstOrDefault();
                            if (checkdata == null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: penalty details could not be found on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                if (action == GeneralModel.Update)
                                {
                                    checkdata.NO_SHOW = model.NO_SHOW;
                                    checkdata.NO_SUBMISSION = model.NO_SUBMISSION;
                                    checkdata.categories_Desc = model.categories_Desc;
                                    checkdata.Date_Updated = DateTime.Now;
                                    checkdata.Updated_by = WKPCompanyEmail;
                                }
                                else if (action == GeneralModel.Delete)
                                {
                                   _context.ADMIN_WP_PENALTIEs.Remove(checkdata);
                                }
                                save = await _context.SaveChangesAsync();
                               
                            }
                            
                         break;
                      }
                   
                    if (save > 0)
                    {
                        var returnModel = _context.ADMIN_WP_PENALTIEs.ToList();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"Penalty has been {action}D successfully", Data = returnModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: An error occured while trying to {action} penalty.", StatusCode = ResponseCodes.Failure };
                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: model data was not passed correctly.", StatusCode = ResponseCodes.Failure };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("ADMIN_EMAIL_DAYS")]
        public async Task<WebApiResponse> Admin_EmailDays(ADMIN_EMAIL_DAY model,string action)
        {
            try
            {
                int save = 0;
                if (model != null)
                {
                    switch (action) {
                        case "INSERT":  
                            var getdata = _context.ADMIN_EMAIL_DAYs.Where(x => x.Email_.ToLower() == model.Email_.Trim().ToLower() && x.DAYS_ == model.DAYS_ && x.Deleted_status == null).FirstOrDefault();
                            if (getdata != null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: admin email days config is already existing on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                var data = new ADMIN_EMAIL_DAY()
                                {
                                    Email_ = model.Email_.ToLower().Trim(),
                                    DAYS_ = model.DAYS_.Trim(),
                                    Description = model.Description,
                                    Date_Created = DateTime.Now,
                                    Created_by = WKPCompanyEmail
                                };
                                await _context.ADMIN_EMAIL_DAYs.AddAsync(data);
                                save = await _context.SaveChangesAsync();

                            }
                            break;
                      
                        default:
                            var checkdata = _context.ADMIN_EMAIL_DAYs.Where(x => x.SN == model.SN && x.Deleted_status == null).FirstOrDefault();
                            if (checkdata == null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: start & end dates details could not be found on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                if (action == GeneralModel.Update)
                                {
                                    checkdata.Email_ = model.Email_.Trim().ToLower();
                                    checkdata.DAYS_ = model.DAYS_.Trim();
                                    checkdata.Description = model.Description;
                                    checkdata.Date_Updated = DateTime.Now;
                                    checkdata.Updated_by = WKPCompanyEmail;
                                }
                                else if (action == GeneralModel.Delete)
                                {
                                   _context.ADMIN_EMAIL_DAYs.Remove(checkdata);
                                }
                                save = await _context.SaveChangesAsync();
                               
                            }
                            
                         break;
                      }
                   
                    if (save > 0)
                    {
                        var returnModel = _context.ADMIN_EMAIL_DAYs.ToList();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"Admin email days config have been {action}D successfully", Data = returnModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: An error occured while trying to {action} admin email days config.", StatusCode = ResponseCodes.Failure };
                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: model data was not passed correctly.", StatusCode = ResponseCodes.Failure };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("ROLES_SUPER_ADMINS")]
        public async Task<WebApiResponse> Super_Admins(ROLES_SUPER_ADMIN model,string action)
        {
            try
            {
                int save = 0;
                if (model != null)
                {
                    switch (action) {
                        case "INSERT":  
                            var getdata = _context.ROLES_SUPER_ADMINs.Where(x => x.Email_.ToLower() == model.Email_.Trim().ToLower() && x.Deleted_status == null).FirstOrDefault();
                            if (getdata != null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: super admin email is already existing on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                var data = new ROLES_SUPER_ADMIN()
                                {
                                    
                                    Email_ = model.Email_.ToLower().Trim(),
                                    Role_ = GeneralModel.SuperAdmin,
                                    Description = model.Description,
                                    Date_Created = DateTime.Now,
                                    Created_by = WKPCompanyEmail
                                };
                                await _context.ROLES_SUPER_ADMINs.AddAsync(data);
                                save = await _context.SaveChangesAsync();

                            }
                            break;
                      
                        default:
                            var checkdata = _context.ROLES_SUPER_ADMINs.Where(x => x.SN == model.SN && x.Deleted_status == null).FirstOrDefault();
                            if (checkdata == null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: super admin details could not be found on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                if (action == GeneralModel.Update)
                                {
                                    checkdata.Email_ = model.Email_.Trim().ToLower();
                                    checkdata.Role_ = model.Role_.Trim();
                                    checkdata.Description = model.Description;
                                    checkdata.Date_Updated = DateTime.Now;
                                    checkdata.Updated_by = WKPCompanyEmail;
                                }
                                else if (action == GeneralModel.Delete)
                                {
                                   _context.ROLES_SUPER_ADMINs.Remove(checkdata);
                                }
                                save = await _context.SaveChangesAsync();
                               
                            }
                            
                         break;
                      }
                   
                    if (save > 0)
                    {
                        var returnModel = _context.ROLES_SUPER_ADMINs.ToList();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"Super admin email has been {action}D successfully", Data = returnModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: An error occured while trying to {action} super admin email.", StatusCode = ResponseCodes.Failure };
                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: model data was not passed correctly.", StatusCode = ResponseCodes.Failure };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("ADMIN_PRESENTATION_CATEGORIES")]
        public async Task<WebApiResponse> Admin_Presentation_Categories(ADMIN_PRESENTATION_CATEGORy model, string action)
        {
            try
            {
                int save = 0;
                if (model != null)
                {
                    switch (action)
                    {
                        case "INSERT":
                            var getdata = _context.ADMIN_PRESENTATION_CATEGORIEs.Where(x => x.categories.ToLower() == model.categories.Trim().ToLower()).FirstOrDefault();
                            if (getdata != null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: {model.categories} is already existing on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                var data = new ADMIN_PRESENTATION_CATEGORy()
                                {
                                    categories = model.categories,
                                    categories_Desc = model.categories_Desc,
                                    Date_Created = DateTime.Now,
                                    Created_by = WKPCompanyEmail
                                };
                                await _context.ADMIN_PRESENTATION_CATEGORIEs.AddAsync(data);
                                save = await _context.SaveChangesAsync();

                            }
                            break;

                        default:
                            var checkdata = _context.ADMIN_PRESENTATION_CATEGORIEs.Where(x => x.Id == model.Id).FirstOrDefault();
                            if (checkdata == null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: {model.categories} details could not be found on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                if (action == GeneralModel.Update)
                                {
                                    checkdata.categories = model.categories;
                                    checkdata.categories_Desc = model.categories_Desc;
                                    checkdata.Date_Updated = DateTime.Now;
                                    checkdata.Updated_by = WKPCompanyEmail;
                                }
                                else if (action == GeneralModel.Delete)
                                {
                                    _context.ADMIN_PRESENTATION_CATEGORIEs.Remove(checkdata);
                                }
                                save = await _context.SaveChangesAsync();

                            }

                            break;
                    }

                    if (save > 0)
                    {
                        var returnModel = _context.ADMIN_PRESENTATION_CATEGORIEs.ToList();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"{model.categories} has been {action}D successfully", Data = returnModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: An error occured while trying to {action} {model.categories} presentation category.", StatusCode = ResponseCodes.Failure };
                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: model data was not passed correctly.", StatusCode = ResponseCodes.Failure };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("ADMIN_MEETING_ROOMS")]
        public async Task<WebApiResponse> Admin_Meeting_Rooms(ADMIN_MEETING_ROOM model, string action)
        {
            try
            {
                int save = 0;
                if (model != null)
                {
                    switch (action)
                    {
                        case "INSERT":
                            var getdata = _context.ADMIN_MEETING_ROOMs.Where(x => x.MEETING_ROOM.ToLower() == model.MEETING_ROOM.Trim().ToLower()).FirstOrDefault();
                            if (getdata != null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: {model.MEETING_ROOM} is already existing on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                var data = new ADMIN_MEETING_ROOM()
                                {
                                    MEETING_ROOM = model.MEETING_ROOM.Trim().ToUpper(),
                                    categories_Desc = model.categories_Desc,
                                    Date_Created = DateTime.Now,
                                    Created_by = WKPCompanyEmail
                                };
                                await _context.ADMIN_MEETING_ROOMs.AddAsync(data);
                                save = await _context.SaveChangesAsync();

                            }
                            break;

                        default:
                            var checkdata = _context.ADMIN_MEETING_ROOMs.Where(x => x.Id == model.Id).FirstOrDefault();
                            if (checkdata == null)
                            {
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: {model.MEETING_ROOM} details could not be found on the portal. ", StatusCode = ResponseCodes.Failure };
                            }
                            else
                            {
                                if (action == GeneralModel.Update)
                                {
                                    checkdata.MEETING_ROOM = model.MEETING_ROOM;
                                    checkdata.categories_Desc = model.categories_Desc;
                                    checkdata.Date_Updated = DateTime.Now;
                                    checkdata.Updated_by = WKPCompanyEmail;
                                }
                                else if (action == GeneralModel.Delete)
                                {
                                    _context.ADMIN_MEETING_ROOMs.Remove(checkdata);
                                }
                                save = await _context.SaveChangesAsync();

                            }

                            break;
                    }

                    if (save > 0)
                    {
                        var returnModel = _context.ADMIN_MEETING_ROOMs.ToList();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"{model.MEETING_ROOM} has been {action}D successfully", Data = returnModel, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: An error occured while trying to {action} {model.MEETING_ROOM} meeting room.", StatusCode = ResponseCodes.Failure };
                    }
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error: model data was not passed correctly.", StatusCode = ResponseCodes.Failure };
                }

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        #endregion
        #region company concessions and fields management
        [HttpGet("GET_ADMIN_CONCESSIONS_INFORMATIONs")]
        public async Task<object> GET_ADMIN_CONCESSIONS_INFORMATIONs(int companyNumber)
        {
            int companyID = companyNumber > 0 ? companyNumber : int.Parse(WKPCompanyId);
            var companyConcessions = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.CompanyNumber == companyID && d.DELETED_STATUS != "true" select d).ToListAsync();
            return new { CompanyConcessions = companyConcessions };
        }

        [HttpGet("GET_COMPANY_FIELDS")]
        public async Task<object> GET_COMPANY_FIELDS(int companyNumber)
        {
            int companyID = companyNumber > 0 ? companyNumber : int.Parse(WKPCompanyId);
            var concessionFields = await (from d in _context.COMPANY_FIELDs where d.CompanyNumber == companyID && d.DeletedStatus != true select d).ToListAsync();
            return new { ConcessionFields = concessionFields };
        }
        [HttpGet("GET_CONCESSIONS_FIELDS")]
        public async Task<object> GET_CONCESSIONS_FIELDS(int concessionID)
        {
            var companyFields = await (from d in _context.COMPANY_FIELDs where d.Concession_ID == concessionID && d.DeletedStatus != true select d).ToListAsync();
            return new { CompanyFields = companyFields };
        }


        [HttpPost("POST_COMPANY_FIELD")]
        public async Task<WebApiResponse> POST_COMPANY_FIELD([FromBody] COMPANY_FIELD company_field_model, string actionToDo = null)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo;

            try
            {
                #region Saving Field

                if (action == GeneralModel.Insert)
                {
                    var companyField = (from d in _context.COMPANY_FIELDs
                                        where d.Field_Name == company_field_model.Field_Name.TrimEnd().ToUpper()
                                              && d.CompanyNumber == int.Parse(WKPCompanyId) && d.DeletedStatus != true
                                        select d).FirstOrDefault();

                    if (companyField != null)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : Field ({company_field_model.Field_Name} is already existing and can not be duplicated.", StatusCode = ResponseCodes.Failure };
                    }
                    else
                    {
                        company_field_model.CompanyNumber = int.Parse(WKPCompanyId);
                        company_field_model.Date_Created = DateTime.Now;
                        company_field_model.Field_Name = company_field_model.Field_Name.TrimEnd().ToUpper();
                        await _context.COMPANY_FIELDs.AddAsync(company_field_model);
                    }
                }
                else if (action == GeneralModel.Update)
                {
                    var companyField = (from d in _context.COMPANY_FIELDs
                                        where d.Field_ID == company_field_model.Field_ID
                                              && d.CompanyNumber == int.Parse(WKPCompanyId) && d.DeletedStatus != true
                                        select d).FirstOrDefault();

                    if (companyField == null)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This field details could not be found.", StatusCode = ResponseCodes.Failure };
                    }
                    else
                    {
                        company_field_model.Date_Updated = DateTime.Now;
                        company_field_model.Field_Name = company_field_model.Field_Name.TrimEnd().ToUpper();
                    }
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.COMPANY_FIELDs.Remove(company_field_model);
                }

                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Field has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        #endregion

    }


}