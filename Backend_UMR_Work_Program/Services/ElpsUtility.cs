using Backend_UMR_Work_Program.DataModels;
using Backend_UMR_Work_Program.Helpers;
using Backend_UMR_Work_Program.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using static Backend_UMR_Work_Program.Models.GeneralModel;

namespace Backend_UMR_Work_Program.Services
{
	public class ElpsUtility
	{
		//private  = new WKP_DBContext();
		//private readonly AppSettings _appSettings;
		//= new ();
		public ElpsUtility()
		{
			//_appSettings=appSettings;
		}
		public static async Task<WebApiResponse> ValidateLogin(string email, string code, WKP_DBContext _context, AppSettings appSettings, WebApiResponse webApiResponse)
		{
			var company = new ADMIN_COMPANY_INFORMATION();
			var response = new WebApiResponse();
			try
			{
				if (!string.IsNullOrEmpty(code))
				{
					response = GetCompanyDetailByEmail(email, appSettings, webApiResponse);
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
						response = GetStaff(email, appSettings, webApiResponse);
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

		public static WebApiResponse GetCompanyDetailByEmail(string email, AppSettings _appSettings, WebApiResponse webApiResponse)
		{
			try
			{
				var encrpt = $"{_appSettings.AppEmail}{_appSettings.SecreteKey}";
				var apiHash = MyUtils.GenerateSha512(encrpt);
				var request = new RestRequest("api/company/{compemail}/{email}/{apiHash}", Method.Get);
				request.AddUrlSegment("compemail", email);
				request.AddUrlSegment("email", _appSettings.AppEmail);
				request.AddUrlSegment("apiHash", apiHash);

				var client = new RestClient(_appSettings.elpsBaseUrl);
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

		public static WebApiResponse GetStaff(string email, AppSettings _appSettings, WebApiResponse webApiResponse)
		{
			try
			{
				var encrpt = $"{_appSettings.AppEmail}{_appSettings.SecreteKey}";
				var apiHash = MyUtils.GenerateSha512(encrpt);
				var request = new RestRequest($"/api/Accounts/Staff/{email}/{_appSettings.AppEmail}/{apiHash}", Method.Get);
				;

				var client = new RestClient(_appSettings.elpsBaseUrl);
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
	}
}
