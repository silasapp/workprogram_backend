﻿using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using Newtonsoft.Json;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using static Backend_UMR_Work_Program.Models.ViewModel;
using AutoMapper;
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
        
        private readonly IMapper _mapper;

        public AccountController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController, Account account, IMapper mapper)
        {
            //_httpContextAccessor = httpContextAccessor;
            _account = account;
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor, _mapper);
        }

        [HttpGet("GetData")]
        public object GetData()
        {
            try {
                var table = _account.GetData();
                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(table);
                return JSONString;
            }
            catch(Exception ex) {
                return new {message = ex.Message, trace = ex.StackTrace};
            }
        }

        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] Logine logine)
        {
            try { 
            var tokenData = await _account.isAutheticate(logine.email, logine.password);
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(tokenData);
            return Ok(tokenData);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }


        [HttpGet("VerifyCompanyCode")]
        public async Task<IActionResult> VerifyCompanyCode(string companyCode)
        {
            try
            {
                var isAvailable = await _account.VerifyCompanyCode(companyCode);
                return Ok(isAvailable);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }


        [HttpPost("CheckNewPinCode")]
        public async Task<IActionResult> CheckNewPinCode(string oldCompanyCode, string email, string newCompanyCode)
        {
            try { 
            var isAvailable = await _account.CheckNewPinCode(oldCompanyCode, email, newCompanyCode);
            return Ok(isAvailable);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        [HttpGet("GetCompanyResource")]
        public async Task<IActionResult> GetCompanyResource(string companyCode)
        {
            try { 
            var isAvailable = await _account.GetCompanyResource(companyCode);
            return Ok(isAvailable);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        [HttpPost("CreateCompanyResource")]
        public async Task<IActionResult> CreateCompanyResource([FromBody] CreateUser user)
        {
            try { 
            var isAvailable = await _account.CheckIfUserExistBeforeCreating(user.companyName, user.companyCode, user.name, user.designation, user.phone, user.email, user.password);
            return Ok(isAvailable);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        [HttpPost("DeleteCompanyResource")]
        public async Task<IActionResult> DeleteCompanyResource(string id, string companyCode)
        {
            try { 
            var isAvailable = await _account.DeleteUser(companyCode, id);
            return Ok(isAvailable);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        [HttpPost("ReturnPasswordInfo")]
        public async Task<IActionResult> ReturnPasswordInfo(string email)
        {
            try { 
            var isAvailable = await _account.ReturnPasswordInfo(email);
            return Ok(isAvailable);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<WebApiResponse> ResetPassword(string email, string currentPassword, string newPassword)
        {
            try { 
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
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

         [HttpPost("Decrypt")]
        public string Decrypt(string text)
        {
            var data =  _account.Decrypt(text);
            return data;
        }

    }
}
