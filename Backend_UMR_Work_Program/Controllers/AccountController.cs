using AutoMapper;
using Backend_UMR_Work_Program.DataModels;
using Backend_UMR_Work_Program.Helpers;
using Backend_UMR_Work_Program.Models;
using Backend_UMR_Work_Program.Services;
using LinqToDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Security.Claims;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using static Backend_UMR_Work_Program.Models.ViewModel;
//using static Backend_UMR_Work_Program.Helpers.GeneralClass;

namespace Backend_UMR_Work_Program.Controllers
{
	// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	public class AccountController : ControllerBase
	{
		private Account _account;
		public WKP_DBContext _context;
		public IConfiguration _configuration;
		HelpersController _helpersController;
		IHttpContextAccessor _httpContextAccessor;
		private readonly AppSettings _appSettings;
		WebApiResponse webApiResponse = new WebApiResponse();
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
			try
			{
				var table = _account.GetData();
				string JSONString = string.Empty;
				JSONString = JsonConvert.SerializeObject(table);
				return JSONString;
			}
			catch (Exception ex)
			{
				return new { message = ex.Message, trace = ex.StackTrace };
			}
		}
		[HttpPost]
		[Route("login")]

		//LoginParam
		//public async Task<IActionResult> LoginRedirect(string email, string code)
		public async Task<IActionResult> Login(LoginParam loginParam)
		{
			var email = loginParam.Email;
			var code = loginParam.Code;
			var login = await ValidateLogin(email, code);
			if (login.ResponseCode.Equals("00"))
				return Redirect($"{_appSettings.LoginUrl}/home?id={login.Data}");

			return Redirect($"{_appSettings.LoginUrl}/home");
		}

		public WebApiResponse GetCompanyDetailByEmail(string email)
		{
			try
			{
				var encrpt = $"{_appSettings.AppEmail}{_appSettings.SecreteKey}";
				var apiHash = MyUtils.GenerateSha512(encrpt);
				var request = new RestRequest("api/company/{compemail}/{email}/{apiHash}", Method.Get);
				request.AddUrlSegment("compemail", email);
				request.AddUrlSegment("email", _appSettings.AppEmail);
				request.AddUrlSegment("apiHash", apiHash);

				var client = new RestClient(_appSettings.ElpsUrl);
				RestResponse response = client.Execute(request);

				if (response.ErrorException != null)
				{
					webApiResponse.Message = response.ErrorMessage;
				}
				else if (response.ResponseStatus != ResponseStatus.Completed)
				{
					webApiResponse.Message = response.ResponseStatus.ToString();
				}
				else if (response.StatusCode != HttpStatusCode.OK)
				{
					webApiResponse.Message = response.StatusCode.ToString();
				}
				else
				{
					webApiResponse.Message = "SUCCESS";
					webApiResponse.Data = JsonConvert.DeserializeObject<LpgLicense.Models.CompanyDetail>(response.Content);
				}
			}
			catch (Exception ex)
			{
				//_generalLogger.LogRequest($"{"Last Exception =>" + ex.Message}{" - "}{DateTime.Now}", true, directory);

				webApiResponse.Message = ex.Message;
			}
			finally
			{
				//_generalLogger.LogRequest($"{"About to Return with Message => " + webApiResponse.Message}{" - "}{DateTime.Now}", false, directory);

			}
			return webApiResponse;
		}
		public WebApiResponse GetStaff(string email)
		{
			try
			{
				var encrpt = $"{_appSettings.AppEmail}{_appSettings.SecreteKey}";
				var apiHash = MyUtils.GenerateSha512(encrpt);
				var request = new RestRequest($"/api/Accounts/Staff/{email}/{_appSettings.AppEmail}/{apiHash}", Method.Get);
				;

				var client = new RestClient(_appSettings.ElpsUrl);
				//_generalLogger.LogRequest($"{"About to GetCompanyDetail On Elps with Email => "}{" "}{" - "}{DateTime.Now}", false, directory);
				RestResponse response = client.Execute(request);
				//_generalLogger.LogRequest($"{"Response Exception =>" + response.ErrorException + "\r\nResponse Status =>" + response.ResponseStatus + "\r\nStatus Code =>" + response.StatusCode}{" "}{" - "}{DateTime.Now}", false, directory);
				if (response.ErrorException != null)
				{
					webApiResponse.Message = response.ErrorMessage;
				}

				else if (response.ResponseStatus != ResponseStatus.Completed)
				{
					webApiResponse.Message = response.ResponseStatus.ToString();
				}

				else if (response.StatusCode != HttpStatusCode.OK)
				{
					webApiResponse.Message = response.StatusCode.ToString();
				}
				else
				{
					webApiResponse.Data = JsonConvert.DeserializeObject<StaffResponseDto>(response.Content);
					webApiResponse.Message = "SUCCESS";
				}
			}
			catch (Exception ex)
			{
				//_generalLogger.LogRequest($"{"Last Exception =>" + ex.ToString()}{" - "}{DateTime.Now}", true, directory);
				webApiResponse.Message = ex.Message;
			}
			finally
			{
				//_generalLogger.LogRequest($"{"About to Return with Message => " + webApiResponse.Message}{" - "}{DateTime.Now}", true, directory);

			}

			return webApiResponse;
		}

		public async Task<WebApiResponse> ValidateLogin(string email, string code)
		{
			var company = new ADMIN_COMPANY_INFORMATION();
			var response = new WebApiResponse();
			try
			{
				if (!string.IsNullOrEmpty(code))
				{
					response = GetCompanyDetailByEmail(email);
					if (response.Message == "SUCCESS")
					{
						var companyDetail = (LpgLicense.Models.CompanyDetail)response.Data;

						company = _context.ADMIN_COMPANY_INFORMATIONs.FirstOrDefault(x => x.EMAIL == email);


						if (company == null)
							company = new ADMIN_COMPANY_INFORMATION
							{
								EMAIL = email,
								NAME = companyDetail.name,
								PHONE_NO = companyDetail.contact_Phone,
								ELPS_ID = companyDetail.id,
								Created_by = "System",
								Date_Created = DateTime.UtcNow,
								STATUS_ = "True",
								COMPANY_NAME = companyDetail.name,
							};
						else
						{
							if (!company.EMAIL.ToLower().Equals(email.ToLower()))
							{
								company.EMAIL = email;
								company.COMPANY_NAME = companyDetail.name;
							}

							_context.ADMIN_COMPANY_INFORMATIONs.Update(company);
							var save = await _context.SaveChangesAsync();
						}
					}
					else
					{
						response = GetStaff(email);
						if (response.Message == "SUCCESS")
						{
							var staff = (StaffResponseDto)response.Data;

							company = _context.ADMIN_COMPANY_INFORMATIONs.FirstOrDefault(x => x.EMAIL == staff.email);

							if (company != null)
							{
								if (!company.EMAIL.ToLower().Equals(email.ToLower()))
								{
									company.EMAIL = email;
									company.NAME = email;
								}
								//user.FirstName = staff.firstName;
								company.PHONE_NO = staff?.phoneNo?.ToString();
								_context.ADMIN_COMPANY_INFORMATIONs.Update(company);
								var save = await _context.SaveChangesAsync();
								//await _userManager.UpdateAsync(user);
							}
						}
					}
					if (response.Message.ToLower().Equals("success"))
					{
						response = new WebApiResponse
						{
							ResponseCode = AppResponseCodes.Success,
							Message = "Successful",
							StatusCode = ResponseCodes.Success,
							Data = company.Id
						};
					}
					else
					{
						response = new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Unable to fetch user details from Celps with the email " + email, StatusCode = ResponseCodes.RecordNotFound };
					}
				}
				else
				{
					company= await _context.ADMIN_COMPANY_INFORMATIONs.FirstOrDefaultAsync(x => x.EMAIL.Equals(email));

					if (company != null)
					{

						response = new WebApiResponse
						{
							ResponseCode = AppResponseCodes.Success,
							Message = "Successful",
							Data = new
							{
								UserId = company.EMAIL,
								//UserType = user.UserType,
								ElpsId = company.ELPS_ID,
								//CaCNumber = user?.Company?.CacNumber,
								CompantName = company.COMPANY_NAME,
								CreatedBy = company.Created_by,
								CreatedOn = company.Date_Created,
								//Status = company.STATUS_,
							},
							StatusCode = ResponseCodes.Success
						};
					}
				}
			}
			catch (Exception ex)
			{

				response = new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Internal error occured " + ex.ToString(), StatusCode = ResponseCodes.InternalError };

			}
			return response;
		}

		[HttpPost("Authenticate")]
		public async Task<IActionResult> Authenticate([FromBody] Logine logine)
		{
			try
			{
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
			try
			{
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
			try
			{
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
			try
			{
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
			try
			{
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
			try
			{
				var isAvailable = await _account.ReturnPasswordInfo(email);
				return Ok(isAvailable);
			}
			catch (Exception e)
			{
				return Ok(e.Message);
			}
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpPost("ResetPassword")]
		public async Task<WebApiResponse> ResetPassword(string currentPassword, string newPassword)
		{
			try
			{
				string encryptCP = _helpersController.Encrypt(currentPassword);
				string email = User.FindFirstValue(ClaimTypes.Email);
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
			var data = _account.Decrypt(text);
			return data;
		}

	}
}
