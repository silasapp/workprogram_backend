﻿using AutoMapper;
using Backend_UMR_Work_Program.DataModels;
using Backend_UMR_Work_Program.Models;
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
	[ApiController]

	public class PresentationController : ControllerBase
	{
		private Presentation _presentation;
		public WKP_DBContext _context;
		public IConfiguration _configuration;
		HelpersController _helpersController;
		IHttpContextAccessor _httpContextAccessor;
		IMapper _mapper;
		private BlobService blobService;
		public PresentationController(Presentation presentation, WKP_DBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper, BlobService blobservice)
		{
			_presentation = presentation;
			_context = context;
			_configuration = configuration;
			_mapper = mapper;
			_helpersController = new HelpersController(_context, configuration, _httpContextAccessor, _mapper);
			this.blobService = blobservice;
		}

		//private string? WKPUserId => "1";
		//private string? WKPUserName => "Name";
		//private string? WKPUserEmail => "adeola.kween123@gmail.com";
		//private string? WKPUserRole => "Admin";
		private string? WKPUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
		private string? WKPUserName => User.FindFirstValue(ClaimTypes.Name);
		private string? WKPUserEmail => User.FindFirstValue(ClaimTypes.Email);
		private string? WKPUserRole => User.FindFirstValue(ClaimTypes.Role);
		private int? WKPUserNumber => Convert.ToInt32(User.FindFirstValue(ClaimTypes.PrimarySid));



		[HttpGet("GetCompanyDetails")]
		public object CompanyDetails(string companyName, string companyEmail, string companyId)
		{
			try
			{

				var details = _context.ADMIN_COMPANY_DETAILs.Where(q => q.COMPANY_NAME==companyName).FirstOrDefault();
				//var details = _presentation.CompanyDetails(companyName, companyEmail, companyId);


				return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = details, StatusCode = ResponseCodes.Success };

			}
			catch (Exception ex)
			{
				return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.InternalError };
			}
		}

		[HttpPost("EditCompanyDetails")]
		public object EditCompanyDetails([FromBody] CompanyDetail myDetail)
		{
			try
			{

				var details = _context.ADMIN_COMPANY_DETAILs.Where(a => a.COMPANY_NAME == myDetail.CompanyName).FirstOrDefault();
				if (details != null)
				{


					//Contact Person's Number
					//dami
					//Contact Person's Email
					details.Address_of_Company=myDetail.Address_of_Company;
					details.Phone_NO_of_MD_CEO= myDetail.Phone_NO_of_MD_CEO;
					details.Name_of_MD_CEO = myDetail.Name_of_MD_CEO;
					details.Contact_Person=myDetail.Contact_Person;
					details.Email_Address=myDetail.Email_Address;
					details.Phone_No = myDetail.Phone_No;


					// _context.ADMIN_COMPANY_DETAILs.Remove(details);
					//_context.SaveChanges();
					// _context.ADMIN_COMPANY_DETAILs.Add(_detail);
					//    _presentation.Insert_Company_Details_Contact_Person(myDetail.CompanyName, myDetail.CompanyEmail, myDetail.Address_of_Company, myDetail.Name_of_MD_CEO, myDetail.Phone_NO_of_MD_CEO, myDetail.Contact_Person, myDetail.Phone_No, myDetail.Email_Address);
					if (_context.SaveChanges() > 0)
					{
						return Ok(details);

					}
				}

				return null;

			}
			catch (Exception ex)
			{
				return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.InternalError };
			}
		}

		[HttpGet("GET_PRESENTATION_DATETIME")]
		public async Task<object> GET_PRESENTATION_DATETIME()
		{
			var schedule = await _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.COMPANY_ID == WKPUserId && c.YEAR == DateTime.Now.Year.ToString()).FirstOrDefaultAsync();
			return schedule;
		}

		[HttpGet("GET_PRESENTATION_DATETIMES")]
		public async Task<object> GET_PRESENTATION_DATETIMES()
		{
			var schedule = await _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.COMPANY_ID == WKPUserId && c.YEAR == DateTime.Now.Year.ToString()).ToListAsync();
			return schedule;
		}
		//Added by Musa 
		[HttpGet("Get_StartAndEndDates")]
		public async Task<StartAndEndDateDto> Get_StartAndEndDates()
		{
			try
			{
				var data = await _context.ADMIN_WP_START_END_DATEs.Where(x => x.Created_by == WKPUserEmail)?.FirstOrDefaultAsync();

				if (data==null)
				{
					return null;
				}

				var startnEndDate = new StartAndEndDateDto()
				{
					start_date=data.start_date,
					end_date=data.end_date
				};
				//var response = new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = data, StatusCode = ResponseCodes.Success };
				return startnEndDate;
			}
			catch (Exception e)
			{
				throw e;
				//return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };

			}
		}
		[HttpPost("SCHEDULEPRESENTATION")]
		public async Task<WebApiResponse> SCHEDULE_PRESENTATION_DATETIME(string time, DateTime date)
		{
			try
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
			catch (Exception ex)
			{
				return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.InternalError };
			}
		}
		[HttpPost("UPLOADPRESENTATION")]
		public async Task<WebApiResponse> UPLOAD_PRESENTATION_DOCUMENT(string year, IFormFile document)
		{
			try
			{
				var CurrentYear = DateTime.Now.Year.ToString();
				var checkCompanyPresentation = _context.PRESENTATION_UPLOADs.Where(c => c.COMPANY_ID == WKPUserId && c.Year_of_WP == year).FirstOrDefault();

				//Commented by Musa Sule to enable multiple uploads

				//if (checkCompanyPresentation != null)
				//{
				//schedule already exist for company
				//return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "You have already uploaded a presentation for the selected year, kindly delete an existing document to upload another.", StatusCode = ResponseCodes.Failure };
				//}
				//else
				//{

				//check if date has been scheduled by another company
				var system_date = DateTime.Now.ToString(); // 
				if (document != null)
				{

					var blobname1 = blobService.Filenamer(document);
					var uploadedDocument = await blobService.UploadFileBlobAsync("documents", document.OpenReadStream(), document.ContentType, $"Presentations/{blobname1}", "PRESENTATION", (int)WKPUserNumber, int.Parse(year));

					if (uploadedDocument == null)
					{
						return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "An error occured while trying to upload presentation document.", StatusCode = ResponseCodes.Failure };
					}


					string document_FileExtension = document.FileName.Split(".")[1].Trim();

					var PPTX = "pptx";
					var PPT = "ppt";

					if (document_FileExtension.ToLower()!=PPTX.ToLower() /*|| document_FileExtension.ToLower()!=PPT.ToLower()*/)
					{
						return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Only pptx or ppt are allowed. Please select Presentation file", StatusCode = ResponseCodes.Failure };
					}


					var presentation = new PRESENTATION_UPLOAD()
					{
						CompanyName = WKPUserName,
						Companyemail = WKPUserEmail,
						COMPANY_ID = WKPUserId,
						Year_of_WP = year,
						uploaded_presentation = uploadedDocument,
						upload_extension = "." + document_FileExtension,
						original_filemane = document.Name,
						Created_by = WKPUserEmail,
						Date_Created = DateTime.Now,
					};

					_context.PRESENTATION_UPLOADs.Add(presentation);

					if (await _context.SaveChangesAsync() > 0)
					{
						var companyPresentations = await _context.PRESENTATION_UPLOADs.Where(x => x.COMPANY_ID == WKPUserId /*&& x.Year_of_WP == CurrentYear*/).ToListAsync();

						return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "File uploaded successfully.", Data = companyPresentations, StatusCode = ResponseCodes.Success };
					}
					//else
					//{
					return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "An error occured while trying to create this presentation schedule.", StatusCode = ResponseCodes.Failure };

					//}
				}
				//else
				//{
				return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Sorry, document is empty.", StatusCode = ResponseCodes.Failure };

				//}
				//}

			}
			catch (Exception ex)
			{
				return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.InternalError };
			}
		}


		[HttpPost("PRESENTATION_SCRIBES")]
		public async Task<WebApiResponse> Presentation_scribe(int[] Id, string emailStatus, string year)
		{
			int save = 0;
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

					return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Badrequest };
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
			try
			{
				var yearlist = await (from a in _context.ADMIN_DATETIME_PRESENTATIONs where a.YEAR != "" orderby a.YEAR select a.YEAR).Distinct().ToListAsync();
				return yearlist;
			}
			catch (Exception ex)
			{
				return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.InternalError };
			}
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
				return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };
			}
			catch (Exception ex)
			{

				return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Success };
			}


		}

		[HttpGet("DIVISIONAL_SCHEDULE_LIST")]
		public async Task<WebApiResponse> Get_Divisional_Schedule_list(string year = null)
		{

			var details = new List<ADMIN_DIVISIONAL_REPS_PRESENTATION>();
			try
			{
				details = await (from a in _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs select a).ToListAsync();
				return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details.OrderBy(x => x.YEAR), StatusCode = ResponseCodes.Success };
			}
			catch (Exception ex)
			{

				return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Success };
			}


		}

		[HttpGet("DIVISIONAL_SCHEDULE_BY_YEAR")]
		public async Task<WebApiResponse> Get_Divisional_Schedule_By_Year(string year = null)
		{

			var details = new List<ADMIN_DIVISIONAL_REPS_PRESENTATION>();
			try
			{
				details = await (from a in _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs where a.YEAR == year select a).ToListAsync();
				return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details.OrderBy(x => x.YEAR), StatusCode = ResponseCodes.Success };

			}
			catch (Exception ex)
			{

				return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Success };
			}


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
					return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ex.Message, StatusCode = ResponseCodes.InternalError };
				}
			}
			return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound, Message = "No data found", StatusCode = ResponseCodes.RecordNotFound };
		}

		[HttpGet("DIVISIONAL_YEARLIST")]
		public async Task<object> Get_Divisional_Yearlist()
		{
			try
			{
				var yearlist = await (from a in _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs where a.YEAR != "" orderby a.YEAR select a.YEAR).Distinct().ToListAsync();
				return yearlist;
			}
			catch (Exception ex)
			{
				return new WebApiResponse
				{
					ResponseCode = AppResponseCodes.InternalError,
					Message = "Error : " + ex.Message,
					StatusCode = ResponseCodes.InternalError
				};
			}
		}

		[HttpGet("COMPANY_REPS")]
		public async Task<object> Get_Company_Replist()
		{
			return await _context.ADMIN_DIVISION_REP_LISTs.ToListAsync();

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
						return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ex.Message, StatusCode = ResponseCodes.InternalError };
					}
				}
			}
			return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound, Message = "No data found", StatusCode = ResponseCodes.RecordNotFound };
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
			if (WKPUserRole == GeneralModel.Admin)
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
				catch (Exception ex)
				{
					return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error:" + ex.Message, StatusCode = ResponseCodes.InternalError };

				}

			}
			return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound, Message = "Not Successful", StatusCode = ResponseCodes.RecordNotFound };

		}


		[HttpPost("UPLOAD_MOM")]
		public async Task<WebApiResponse> UPLOADMOM(int compId, IFormFile document)
		{

			var userRole = WKPUserRole;
			var userName = WKPUserName;
			var userEmail = WKPUserEmail;
			var companyID = WKPUserId;
			var CurrentYear = DateTime.Now.Year.ToString();
			string folderToSave = "";
			var checkUploadedMom = _context.ADMIN_DATETIME_PRESENTATIONs.Where(c => c.Id == compId).FirstOrDefault();
			try
			{
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
			catch (Exception ex)
			{
				return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error:" + ex.Message, StatusCode = ResponseCodes.InternalError };

			}
		}
	}
}
