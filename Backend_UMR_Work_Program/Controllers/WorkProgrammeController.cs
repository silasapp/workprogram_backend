using Microsoft.AspNetCore.Mvc;
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
    public class WorkProgrammeController : ControllerBase
    {
        private Account _account;
        public WKP_DBContext _context;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private BlobService blobService;

        public WorkProgrammeController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController, IMapper mapper, BlobService blobservice)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor, _mapper);
            blobService = blobservice;
        }

        private string? WKPCompanyId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string? WKPCompanyName => User.FindFirstValue(ClaimTypes.Name);
        private string? WKPCompanyEmail => User.FindFirstValue(ClaimTypes.Email);
        private string? WKUserRole => User.FindFirstValue(ClaimTypes.Role);
        private int? WKPCompanyNumber=> Convert.ToInt32(User.FindFirstValue(ClaimTypes.PrimarySid));





        #region database tables actions

        [HttpPost("POST_CONCESSION_SITUATION")]
        public async Task<WebApiResponse> POST_CONCESSION_SITUATION([FromBody] CONCESSION_SITUATION concession_situation_model, string year, string omlName, string actionToDo = null)
        {

            int save = 0;
            var ConcessionCONCESSION_SITUATION_Model = new CONCESSION_SITUATION();
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo;

            try
            {
                #region Saving Concession Situations


                var concessionDbData = (from c in _context.CONCESSION_SITUATIONs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.Year == year select c).FirstOrDefault();
                ConcessionCONCESSION_SITUATION_Model = _mapper.Map<CONCESSION_SITUATION>(concession_situation_model);

                ConcessionCONCESSION_SITUATION_Model.Companyemail = WKPCompanyEmail;
                ConcessionCONCESSION_SITUATION_Model.CompanyName = WKPCompanyName;
                ConcessionCONCESSION_SITUATION_Model.COMPANY_ID = WKPCompanyId;
                ConcessionCONCESSION_SITUATION_Model.Date_Updated = DateTime.Now;
                ConcessionCONCESSION_SITUATION_Model.Updated_by = WKPCompanyEmail;
                ConcessionCONCESSION_SITUATION_Model.Year = year;
                ConcessionCONCESSION_SITUATION_Model.OML_Name = omlName.ToUpper();
                
                if (action == GeneralModel.Insert)
                {
                    if (concessionDbData == null)
                    {
                        ConcessionCONCESSION_SITUATION_Model.COMPANY_ID = WKPCompanyId;
                        ConcessionCONCESSION_SITUATION_Model.CompanyNumber = WKPCompanyNumber;
                        ConcessionCONCESSION_SITUATION_Model.Created_by = WKPCompanyEmail;
                        ConcessionCONCESSION_SITUATION_Model.Date_Created = DateTime.Now;
                        await _context.CONCESSION_SITUATIONs.AddAsync(ConcessionCONCESSION_SITUATION_Model);
                    }
                    else
                    {
                        _context.CONCESSION_SITUATIONs.Remove(concessionDbData);
                        ConcessionCONCESSION_SITUATION_Model.Created_by = WKPCompanyId;
                        ConcessionCONCESSION_SITUATION_Model.Date_Created = DateTime.Now;
                        await _context.CONCESSION_SITUATIONs.AddAsync(ConcessionCONCESSION_SITUATION_Model);
                    }
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.CONCESSION_SITUATIONs.Remove(concessionDbData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No CONCESSION_SITUATION_Model was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
       
        
        [HttpGet("GET_GEOPHYSICAL_ACTIVITIES_ACQUISITION")]
        public async Task<WebApiResponse> GET_GEOPHYSICAL_ACTIVITIES_ACQUISITION(string year, string omlName, string quarter)
        {

            try
            {
                var getGeophysicalActivitesData = (from c in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.QUATER == quarter && c.Year_of_WP == year select c).FirstOrDefault();
                
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getGeophysicalActivitesData, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }
        
        [HttpPost("POST_GEOPHYSICAL_ACTIVITIES_ACQUISITION")]
        public async Task<WebApiResponse> POST_GEOPHYSICAL_ACTIVITIES_ACQUISITION([FromBody] GEOPHYSICAL_ACTIVITIES_ACQUISITION geophysical_activities_acquisition_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {
                #region Saving Geophysical Activites
                if (geophysical_activities_acquisition_model != null)
                {
                        var getgeophysical_activities_acquisition_model = (from c in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.QUATER == geophysical_activities_acquisition_model.QUATER && c.Year_of_WP == year select c).FirstOrDefault();
                        
                        geophysical_activities_acquisition_model.Companyemail = WKPCompanyEmail;
                        geophysical_activities_acquisition_model.CompanyName = WKPCompanyName;
                        geophysical_activities_acquisition_model.COMPANY_ID = WKPCompanyId;
                        geophysical_activities_acquisition_model.CompanyNumber = WKPCompanyNumber;
                        geophysical_activities_acquisition_model.Date_Updated = DateTime.Now;
                        geophysical_activities_acquisition_model.Updated_by = WKPCompanyId;
                        geophysical_activities_acquisition_model.Year_of_WP = year;
                        geophysical_activities_acquisition_model.OML_Name = omlName.ToUpper();

                        if (action == GeneralModel.Insert)
                        {
                            if (getgeophysical_activities_acquisition_model == null)
                            {
                                geophysical_activities_acquisition_model.Date_Created = DateTime.Now;
                                geophysical_activities_acquisition_model.Created_by = WKPCompanyId;
                                await _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.AddAsync(geophysical_activities_acquisition_model);
                            }
                            else
                            {
                            geophysical_activities_acquisition_model.Date_Created = getgeophysical_activities_acquisition_model.Date_Created;
                            geophysical_activities_acquisition_model.Created_by = getgeophysical_activities_acquisition_model.Created_by;
                            geophysical_activities_acquisition_model.Date_Updated = DateTime.Now;
                            geophysical_activities_acquisition_model.Updated_by = WKPCompanyId;
                            await _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.AddAsync(geophysical_activities_acquisition_model);
                             }
                        }
                        else if (action == GeneralModel.Delete)
                        {
                            _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Remove(geophysical_activities_acquisition_model);
                        }

                        save += await _context.SaveChangesAsync();

                        if (save > 0)
                        {
                            string successMsg = "Form has been " + action + "D successfully.";
                            var All_geophysical_activities_acquisitions = await (from c in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_geophysical_activities_acquisitions, StatusCode = ResponseCodes.Success };
                        }
                        else
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                        }
                    }
                
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }


        [HttpGet("GET_GEOPHYSICAL_ACTIVITIES_PROCESSING")]
        public async Task<WebApiResponse> GET_GEOPHYSICAL_ACTIVITIES_PROCESSING(string year, string omlName, string quarter)
        {
            try
            {
                var getGeophysicalActivitesData = (from c in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.QUATER == quarter && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getGeophysicalActivitesData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_GEOPHYSICAL_ACTIVITIES_PROCESSING")]
        public async Task<WebApiResponse> POST_GEOPHYSICAL_ACTIVITIES_PROCESSING([FromBody] GEOPHYSICAL_ACTIVITIES_PROCESSING geophysical_activities_processing_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Geophysical Activites
                if (geophysical_activities_processing_model != null)
                {
                    var getGeophysicalActivitesData = (from c in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.QUATER == geophysical_activities_processing_model.QUATER && c.Year_of_WP == year select c).FirstOrDefault();

                    geophysical_activities_processing_model.Companyemail = WKPCompanyEmail;
                    geophysical_activities_processing_model.CompanyName = WKPCompanyName;
                    geophysical_activities_processing_model.COMPANY_ID = WKPCompanyId;
                    geophysical_activities_processing_model.CompanyNumber = int.Parse(WKPCompanyId);
                    geophysical_activities_processing_model.Date_Updated = DateTime.Now;
                    geophysical_activities_processing_model.Updated_by = WKPCompanyId;
                    geophysical_activities_processing_model.Year_of_WP = year;
                    geophysical_activities_processing_model.OML_Name = geophysical_activities_processing_model.OML_Name.ToUpper();

                    if (action == GeneralModel.Insert)
                    {
                        if (getGeophysicalActivitesData == null)
                        {
                            geophysical_activities_processing_model.Date_Created = DateTime.Now;
                            geophysical_activities_processing_model.Created_by = WKPCompanyId;
                            await _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.AddAsync(geophysical_activities_processing_model);
                        }
                        else
                        {
                            geophysical_activities_processing_model.Date_Created = getGeophysicalActivitesData.Date_Created;
                            geophysical_activities_processing_model.Created_by = getGeophysicalActivitesData.Created_by;
                            geophysical_activities_processing_model.Date_Updated = DateTime.Now;
                            geophysical_activities_processing_model.Updated_by = WKPCompanyId;
                            await _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.AddAsync(geophysical_activities_processing_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Remove(geophysical_activities_processing_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_DRILLING_OPERATIONS_CATEGORIES_OF_WELL")]
        public async Task<WebApiResponse> GET_DRILLING_OPERATIONS_CATEGORIES_OF_WELL(string year, string omlName)
        {

            try
            {
                var drillingCategory = await (from c in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.Year_of_WP == year select c).ToListAsync();
                var drillingWellCost = await (from c in _context.DRILLING_EACH_WELL_COSTs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.Year_of_WP == year select c).ToListAsync();
                var drillingWellProposed = await (from c in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.Year_of_WP == year select c).ToListAsync();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = new {drillingCategory = drillingCategory, drillingWellCost = drillingWellCost, drillingWellProposed = drillingWellProposed}, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }
        [HttpPost("POST_DRILLING_OPERATIONS_CATEGORIES_OF_WELL"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_DRILLING_OPERATIONS_CATEGORIES_OF_WELL([FromForm] DRILLING_OPERATIONS_CATEGORIES_OF_WELL drilling_operations_categories_of_well_model, string omlName, string year, string ActionToDo = null)
        {
            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving drilling data
                if (drilling_operations_categories_of_well_model != null)
                {
                    var getData = (from c in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.QUATER == drilling_operations_categories_of_well_model.QUATER && c.Year_of_WP == year select c).FirstOrDefault();

                    drilling_operations_categories_of_well_model.Companyemail = WKPCompanyEmail;
                    drilling_operations_categories_of_well_model.CompanyName = WKPCompanyName;
                    drilling_operations_categories_of_well_model.COMPANY_ID = WKPCompanyId;
                    drilling_operations_categories_of_well_model.CompanyNumber = WKPCompanyNumber;
                    drilling_operations_categories_of_well_model.Date_Updated = DateTime.Now;
                    drilling_operations_categories_of_well_model.Updated_by = WKPCompanyId;
                    drilling_operations_categories_of_well_model.Year_of_WP = year;
                    drilling_operations_categories_of_well_model.OML_Name = drilling_operations_categories_of_well_model.OML_Name.ToUpper();
                    #region file section
                    //UploadedDocument FieldDiscoveryUploadFile_File = null;
                    //UploadedDocument HydrocarbonCountUploadFile_File = null;
                    var file1 = Request.Form.Files[0];
                    var file2 = Request.Form.Files[1];
                    var blobname1 = blobService.Filenamer(file1);
                    var blobname2 = blobService.Filenamer(file2);

                    if (file1 != null)
                    {
                        string docName = "Field Discovery";
                        drilling_operations_categories_of_well_model.FieldDiscoveryUploadFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType,  $"FieldDiscoveryDocuments/{blobname1}");
                        if (drilling_operations_categories_of_well_model.FieldDiscoveryUploadFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                    }
                    if (file2 != null)
                    {
                        string docName = "Hydrocarbon Count";
                        drilling_operations_categories_of_well_model.HydrocarbonCountUploadFilePath = await blobService.UploadFileBlobAsync("documents", file2.OpenReadStream(), file2.ContentType, $"HydrocarbonCountDocuments/{blobname2}");
                        if (drilling_operations_categories_of_well_model.HydrocarbonCountUploadFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion

                    //drilling_operations_categories_of_well_model.FieldDiscoveryUploadFilePath = files[0] != null ? FieldDiscoveryUploadFile_File.filePath : null;
                    //drilling_operations_categories_of_well_model.HydrocarbonCountUploadFilePath = files[1] != null ? HydrocarbonCountUploadFile_File.filePath : null;

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            drilling_operations_categories_of_well_model.Date_Created = DateTime.Now;
                            drilling_operations_categories_of_well_model.Created_by = WKPCompanyId;
                            await _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.AddAsync(drilling_operations_categories_of_well_model);
                        }
                        else
                        {
                            drilling_operations_categories_of_well_model.Date_Created = getData.Date_Created;
                            drilling_operations_categories_of_well_model.Created_by = getData.Created_by;
                            drilling_operations_categories_of_well_model.Date_Updated = DateTime.Now;
                            drilling_operations_categories_of_well_model.Updated_by = WKPCompanyId;
                            //drilling_operations_categories_of_well_model.Id = null;
                            await _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.AddAsync(drilling_operations_categories_of_well_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Remove(drilling_operations_categories_of_well_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        
        [HttpGet("GET_DRILLING_EACH_WELL_COST")]
        public async Task<WebApiResponse> GET_DRILLING_EACH_WELL_COST(string year, string omlName, string quarter)
        {

            try
            {
                var getData = (from c in _context.DRILLING_EACH_WELL_COSTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.QUATER == quarter && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_DRILLING_EACH_WELL_COST")]
        public async Task<WebApiResponse> POST_DRILLING_EACH_WELL_COST([FromBody] DRILLING_EACH_WELL_COST drilling_each_well_cost_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving drilling data
                if (drilling_each_well_cost_model != null)
                {
                    var getData = (from c in _context.DRILLING_EACH_WELL_COSTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.QUATER == drilling_each_well_cost_model.QUATER && c.Year_of_WP == year select c).FirstOrDefault();

                    drilling_each_well_cost_model.Companyemail = WKPCompanyEmail;
                    drilling_each_well_cost_model.CompanyName = WKPCompanyName;
                    drilling_each_well_cost_model.COMPANY_ID = WKPCompanyId;
                    drilling_each_well_cost_model.CompanyNumber = int.Parse(WKPCompanyId);
                    drilling_each_well_cost_model.Date_Updated = DateTime.Now;
                    drilling_each_well_cost_model.Updated_by = WKPCompanyId;
                    drilling_each_well_cost_model.Year_of_WP = year;
                    drilling_each_well_cost_model.OML_Name = drilling_each_well_cost_model.OML_Name.ToUpper();

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            drilling_each_well_cost_model.Date_Created = DateTime.Now;
                            drilling_each_well_cost_model.Created_by = WKPCompanyId;
                            await _context.DRILLING_EACH_WELL_COSTs.AddAsync(drilling_each_well_cost_model);
                        }
                        else
                        {
                            drilling_each_well_cost_model.Date_Created = getData.Date_Created;
                            drilling_each_well_cost_model.Created_by = getData.Created_by;
                            drilling_each_well_cost_model.Date_Updated = DateTime.Now;
                            drilling_each_well_cost_model.Updated_by = WKPCompanyId;
                            await _context.DRILLING_EACH_WELL_COSTs.AddAsync(drilling_each_well_cost_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.DRILLING_EACH_WELL_COSTs.Remove(drilling_each_well_cost_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.DRILLING_EACH_WELL_COSTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_DRILLING_EACH_WELL_COST_PROPOSED")]
        public async Task<WebApiResponse> GET_DRILLING_EACH_WELL_COST_PROPOSED(string year, string omlName, string quarter)
        {

            try
            {
                var getData = (from c in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.QUATER == quarter && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_DRILLING_EACH_WELL_COST_PROPOSED")]
        public async Task<WebApiResponse> POST_DRILLING_EACH_WELL_COST_PROPOSED([FromBody] DRILLING_EACH_WELL_COST_PROPOSED drilling_each_well_cost_proposed_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving drilling data
                if (drilling_each_well_cost_proposed_model != null)
                {
                    var getData = (from c in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.QUATER == drilling_each_well_cost_proposed_model.QUATER && c.Year_of_WP == year select c).FirstOrDefault();

                    drilling_each_well_cost_proposed_model.Companyemail = WKPCompanyEmail;
                    drilling_each_well_cost_proposed_model.CompanyName = WKPCompanyName;
                    drilling_each_well_cost_proposed_model.COMPANY_ID = WKPCompanyId;
                    drilling_each_well_cost_proposed_model.CompanyNumber = int.Parse(WKPCompanyId);
                    drilling_each_well_cost_proposed_model.Date_Updated = DateTime.Now;
                    drilling_each_well_cost_proposed_model.Updated_by = WKPCompanyId;
                    drilling_each_well_cost_proposed_model.Year_of_WP = year;
                    drilling_each_well_cost_proposed_model.OML_Name = drilling_each_well_cost_proposed_model.OML_Name.ToUpper();

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            drilling_each_well_cost_proposed_model.Date_Created = DateTime.Now;
                            drilling_each_well_cost_proposed_model.Created_by = WKPCompanyId;
                            await _context.DRILLING_EACH_WELL_COST_PROPOSEDs.AddAsync(drilling_each_well_cost_proposed_model);
                        }
                        else
                        {
                            drilling_each_well_cost_proposed_model.Date_Created = getData.Date_Created;
                            drilling_each_well_cost_proposed_model.Created_by = getData.Created_by;
                            drilling_each_well_cost_proposed_model.Date_Updated = DateTime.Now;
                            drilling_each_well_cost_proposed_model.Updated_by = WKPCompanyId;
                            await _context.DRILLING_EACH_WELL_COST_PROPOSEDs.AddAsync(drilling_each_well_cost_proposed_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.DRILLING_EACH_WELL_COST_PROPOSEDs.Remove(drilling_each_well_cost_proposed_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_FIELD_DEVELOPMENT_PLAN")]
        public async Task<WebApiResponse> GET_FIELD_DEVELOPMENT_PLAN(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.FIELD_DEVELOPMENT_PLANs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_FIELD_DEVELOPMENT_PLAN")]
        public async Task<WebApiResponse> POST_FIELD_DEVELOPMENT_PLAN([FromBody] FIELD_DEVELOPMENT_PLAN field_development_plan_model, List<IFormFile> files, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving FDP data
                if (field_development_plan_model != null)
                {
                    var getData = (from c in _context.FIELD_DEVELOPMENT_PLANs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    field_development_plan_model.Companyemail = WKPCompanyEmail;
                    field_development_plan_model.CompanyName = WKPCompanyName;
                    field_development_plan_model.COMPANY_ID = WKPCompanyId;
                    field_development_plan_model.CompanyNumber = int.Parse(WKPCompanyId);
                    field_development_plan_model.Date_Updated = DateTime.Now;
                    field_development_plan_model.Updated_by = WKPCompanyId;
                    field_development_plan_model.Year_of_WP = year;
                    field_development_plan_model.OML_Name = field_development_plan_model.OML_Name.ToUpper();
                    #region file section
                    UploadedDocument approved_FDP_Document = null;

                    if (files[0] != null)
                    {
                        string docName = "Approved FDP";
                        approved_FDP_Document = _helpersController.UploadDocument(files[0], "FDPDocuments");
                        if (approved_FDP_Document == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion

                    field_development_plan_model.Uploaded_approved_FDP_Document = files[0] != null ? approved_FDP_Document.filePath : null;
                    field_development_plan_model.FDPDocumentFilename = files[0] != null ? approved_FDP_Document.fileName : null;

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            field_development_plan_model.Date_Created = DateTime.Now;
                            field_development_plan_model.Created_by = WKPCompanyId;
                            await _context.FIELD_DEVELOPMENT_PLANs.AddAsync(field_development_plan_model);
                        }
                        else
                        {
                            field_development_plan_model.Date_Created = getData.Date_Created;
                            field_development_plan_model.Created_by = getData.Created_by;
                            field_development_plan_model.Date_Updated = DateTime.Now;
                            field_development_plan_model.Updated_by = WKPCompanyId;
                            await _context.FIELD_DEVELOPMENT_PLANs.AddAsync(field_development_plan_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.FIELD_DEVELOPMENT_PLANs.Remove(field_development_plan_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.FIELD_DEVELOPMENT_PLANs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE")]
        public async Task<WebApiResponse> GET_FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE")]
        public async Task<WebApiResponse> POST_FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE([FromBody] FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERf field_development_plan_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving FDP data
                if (field_development_plan_model != null)
                {
                    var getData = (from c in _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    field_development_plan_model.Companyemail = WKPCompanyEmail;
                    field_development_plan_model.CompanyName = WKPCompanyName;
                    field_development_plan_model.COMPANY_ID = WKPCompanyId;
                    field_development_plan_model.CompanyNumber = int.Parse(WKPCompanyId);
                    field_development_plan_model.Date_Updated = DateTime.Now;
                    field_development_plan_model.Updated_by = WKPCompanyId;
                    field_development_plan_model.Year_of_WP = year;
                    field_development_plan_model.OML_Name = field_development_plan_model.OML_Name.ToUpper();
                   
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            field_development_plan_model.Date_Created = DateTime.Now;
                            field_development_plan_model.Created_by = WKPCompanyId;
                            await _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.AddAsync(field_development_plan_model);
                        }
                        else
                        {
                            field_development_plan_model.Date_Created = getData.Date_Created;
                            field_development_plan_model.Created_by = getData.Created_by;
                            field_development_plan_model.Date_Updated = DateTime.Now;
                            field_development_plan_model.Updated_by = WKPCompanyId;
                            await _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.AddAsync(field_development_plan_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.Remove(field_development_plan_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }


        [HttpGet("GET_FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP")]
        public async Task<WebApiResponse> GET_FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP")]
        public async Task<WebApiResponse> POST_FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP([FromBody] FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP field_development_plan_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving FDP data
                if (field_development_plan_model != null)
                {
                    var getData = (from c in _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    field_development_plan_model.Companyemail = WKPCompanyEmail;
                    field_development_plan_model.CompanyName = WKPCompanyName;
                    field_development_plan_model.COMPANY_ID = WKPCompanyId;
                    field_development_plan_model.CompanyNumber = int.Parse(WKPCompanyId);
                    field_development_plan_model.Date_Updated = DateTime.Now;
                    field_development_plan_model.Updated_by = WKPCompanyId;
                    field_development_plan_model.Year_of_WP = year;
                    field_development_plan_model.OML_Name = field_development_plan_model.OML_Name.ToUpper();

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            field_development_plan_model.Date_Created = DateTime.Now;
                            field_development_plan_model.Created_by = WKPCompanyId;
                            await _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs.AddAsync(field_development_plan_model);
                        }
                        else
                        {
                            field_development_plan_model.Date_Created = getData.Date_Created;
                            field_development_plan_model.Created_by = getData.Created_by;
                            field_development_plan_model.Date_Updated = DateTime.Now;
                            field_development_plan_model.Updated_by = WKPCompanyId;
                            await _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs.AddAsync(field_development_plan_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs.Remove(field_development_plan_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_FIELD_DEVELOPMENT_FIELDS_AND_STATUS")]
        public async Task<WebApiResponse> GET_FIELD_DEVELOPMENT_FIELDS_AND_STATUS(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_FIELD_DEVELOPMENT_FIELDS_AND_STATUS")]
        public async Task<WebApiResponse> POST_FIELD_DEVELOPMENT_FIELDS_AND_STATUS([FromBody] FIELD_DEVELOPMENT_FIELDS_AND_STATUS field_development_plan_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving FDP data
                if (field_development_plan_model != null)
                {
                    var getData = (from c in _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    field_development_plan_model.Companyemail = WKPCompanyEmail;
                    field_development_plan_model.CompanyName = WKPCompanyName;
                    field_development_plan_model.COMPANY_ID = WKPCompanyId;
                    field_development_plan_model.CompanyNumber = int.Parse(WKPCompanyId);
                    field_development_plan_model.Date_Updated = DateTime.Now;
                    field_development_plan_model.Updated_by = WKPCompanyId;
                    field_development_plan_model.Year_of_WP = year;
                    field_development_plan_model.OML_Name = field_development_plan_model.OML_Name.ToUpper();

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            field_development_plan_model.Date_Created = DateTime.Now;
                            field_development_plan_model.Created_by = WKPCompanyId;
                            await _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.AddAsync(field_development_plan_model);
                        }
                        else
                        {
                            field_development_plan_model.Date_Created = getData.Date_Created;
                            field_development_plan_model.Created_by = getData.Created_by;
                            field_development_plan_model.Date_Updated = DateTime.Now;
                            field_development_plan_model.Updated_by = WKPCompanyId;
                            await _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.AddAsync(field_development_plan_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.Remove(field_development_plan_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_INITIAL_WELL_COMPLETION_JOB")]
        public async Task<WebApiResponse> GET_INITIAL_WELL_COMPLETION_JOB(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.INITIAL_WELL_COMPLETION_JOBs1 where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_INITIAL_WELL_COMPLETION_JOB")]
        public async Task<WebApiResponse> POST_INITIAL_WELL_COMPLETION_JOB([FromBody] INITIAL_WELL_COMPLETION_JOB1 initial_well_completion_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving FDP data
                if (initial_well_completion_model != null)
                {
                    var getData = (from c in _context.INITIAL_WELL_COMPLETION_JOBs1 where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    initial_well_completion_model.Companyemail = WKPCompanyEmail;
                    initial_well_completion_model.CompanyName = WKPCompanyName;
                    initial_well_completion_model.COMPANY_ID = WKPCompanyId;
                    initial_well_completion_model.CompanyNumber = int.Parse(WKPCompanyId);
                    initial_well_completion_model.Date_Updated = DateTime.Now;
                    initial_well_completion_model.Updated_by = WKPCompanyId;
                    initial_well_completion_model.Year_of_WP = year;
                    initial_well_completion_model.OML_Name = initial_well_completion_model.OML_Name.ToUpper();

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            initial_well_completion_model.Date_Created = DateTime.Now;
                            initial_well_completion_model.Created_by = WKPCompanyId;
                            await _context.INITIAL_WELL_COMPLETION_JOBs1.AddAsync(initial_well_completion_model);
                        }
                        else
                        {
                            initial_well_completion_model.Date_Created = getData.Date_Created;
                            initial_well_completion_model.Created_by = getData.Created_by;
                            initial_well_completion_model.Date_Updated = DateTime.Now;
                            initial_well_completion_model.Updated_by = WKPCompanyId;
                            await _context.INITIAL_WELL_COMPLETION_JOBs1.AddAsync(initial_well_completion_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.INITIAL_WELL_COMPLETION_JOBs1.Remove(initial_well_completion_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.INITIAL_WELL_COMPLETION_JOBs1 where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_WORKOVERS_RECOMPLETION_JOB")]
        public async Task<WebApiResponse> GET_WORKOVERS_RECOMPLETION_JOB(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.WORKOVERS_RECOMPLETION_JOBs1 where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_WORKOVERS_RECOMPLETION_JOB")]
        public async Task<WebApiResponse> POST_WORKOVERS_RECOMPLETION_JOB([FromBody] WORKOVERS_RECOMPLETION_JOB1 workovers_recompletion_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving FDP data
                if (workovers_recompletion_model != null)
                {
                    var getData = (from c in _context.WORKOVERS_RECOMPLETION_JOBs1 where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    workovers_recompletion_model.Companyemail = WKPCompanyEmail;
                    workovers_recompletion_model.CompanyName = WKPCompanyName;
                    workovers_recompletion_model.COMPANY_ID = WKPCompanyId;
                    workovers_recompletion_model.CompanyNumber = int.Parse(WKPCompanyId);
                    workovers_recompletion_model.Date_Updated = DateTime.Now;
                    workovers_recompletion_model.Updated_by = WKPCompanyId;
                    workovers_recompletion_model.Year_of_WP = year;
                    workovers_recompletion_model.OML_Name = workovers_recompletion_model.OML_Name.ToUpper();

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            workovers_recompletion_model.Date_Created = DateTime.Now;
                            workovers_recompletion_model.Created_by = WKPCompanyId;
                            await _context.WORKOVERS_RECOMPLETION_JOBs1.AddAsync(workovers_recompletion_model);
                        }
                        else
                        {
                            workovers_recompletion_model.Date_Created = getData.Date_Created;
                            workovers_recompletion_model.Created_by = getData.Created_by;
                            workovers_recompletion_model.Date_Updated = DateTime.Now;
                            workovers_recompletion_model.Updated_by = WKPCompanyId;
                            await _context.WORKOVERS_RECOMPLETION_JOBs1.AddAsync(workovers_recompletion_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.WORKOVERS_RECOMPLETION_JOBs1.Remove(workovers_recompletion_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.WORKOVERS_RECOMPLETION_JOBs1 where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_OIL_CONDENSATE_PRODUCTION_ACTIVITY")]
        public async Task<WebApiResponse> GET_OIL_CONDENSATE_PRODUCTION_ACTIVITY(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITY")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITY([FromBody] OIL_CONDENSATE_PRODUCTION_ACTIVITy oil_condensate_activity_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Oil Condensate data
                if (oil_condensate_activity_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_condensate_activity_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_activity_model.CompanyName = WKPCompanyName;
                    oil_condensate_activity_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_activity_model.CompanyNumber = int.Parse(WKPCompanyId);
                    oil_condensate_activity_model.Date_Updated = DateTime.Now;
                    oil_condensate_activity_model.Updated_by = WKPCompanyId;
                    oil_condensate_activity_model.Year_of_WP = year;
                    oil_condensate_activity_model.OML_Name = oil_condensate_activity_model.OML_Name.ToUpper();

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            oil_condensate_activity_model.Date_Created = DateTime.Now;
                            oil_condensate_activity_model.Created_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.AddAsync(oil_condensate_activity_model);
                        }
                        else
                        {
                            oil_condensate_activity_model.Date_Created = getData.Date_Created;
                            oil_condensate_activity_model.Created_by = getData.Created_by;
                            oil_condensate_activity_model.Date_Updated = DateTime.Now;
                            oil_condensate_activity_model.Updated_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.AddAsync(oil_condensate_activity_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.Remove(oil_condensate_activity_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION")]
        public async Task<WebApiResponse> GET_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION([FromBody] OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION oil_condensate_unitisation_model, List<IFormFile> files, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Oil Condensate data
                if (oil_condensate_unitisation_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_condensate_unitisation_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_unitisation_model.CompanyName = WKPCompanyName;
                    oil_condensate_unitisation_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_unitisation_model.CompanyNumber = int.Parse(WKPCompanyId);
                    oil_condensate_unitisation_model.Date_Updated = DateTime.Now;
                    oil_condensate_unitisation_model.Updated_by = WKPCompanyId;
                    oil_condensate_unitisation_model.Year_of_WP = year;
                    oil_condensate_unitisation_model.OML_Name = oil_condensate_unitisation_model.OML_Name.ToUpper();
                    #region file section
                    UploadedDocument PUAUploadFile = null;
                    UploadedDocument UUOAUploadFile = null;

                    if (files[0] != null)
                    {
                        string docName = "PUA";
                        PUAUploadFile = _helpersController.UploadDocument(files[0], "PUADocuments");
                        if (PUAUploadFile == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    if (files[1] != null)
                    {
                        string docName = "UUOA";
                        UUOAUploadFile = _helpersController.UploadDocument(files[1], "UUOADocuments");
                        if (UUOAUploadFile == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion

                    oil_condensate_unitisation_model.PUAUploadFilePath = files[0] != null ? PUAUploadFile.filePath : null;
                    oil_condensate_unitisation_model.PUAUploadFilename = files[0] != null ? PUAUploadFile.fileName : null;
                    oil_condensate_unitisation_model.UUOAUploadFilePath = files[1] != null ? UUOAUploadFile.filePath : null;
                    oil_condensate_unitisation_model.UUOAUploadFilename = files[1] != null ? UUOAUploadFile.fileName : null;

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            oil_condensate_unitisation_model.Date_Created = DateTime.Now;
                            oil_condensate_unitisation_model.Created_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.AddAsync(oil_condensate_unitisation_model);
                        }
                        else
                        {
                            oil_condensate_unitisation_model.Date_Created = getData.Date_Created;
                            oil_condensate_unitisation_model.Created_by = getData.Created_by;
                            oil_condensate_unitisation_model.Date_Updated = DateTime.Now;
                            oil_condensate_unitisation_model.Updated_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.AddAsync(oil_condensate_unitisation_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.Remove(oil_condensate_unitisation_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_GAS_PRODUCTION_ACTIVITY")]
        public async Task<WebApiResponse> GET_GAS_PRODUCTION_ACTIVITY(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_GAS_PRODUCTION_ACTIVITY")]
        public async Task<WebApiResponse> POST_GAS_PRODUCTION_ACTIVITY([FromBody] GAS_PRODUCTION_ACTIVITy gas_production_model, List<IFormFile> files, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Oil Condensate data
                if (gas_production_model != null)
                {
                    var getData = (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    gas_production_model.Companyemail = WKPCompanyEmail;
                    gas_production_model.CompanyName = WKPCompanyName;
                    gas_production_model.COMPANY_ID = WKPCompanyId;
                    gas_production_model.CompanyNumber = int.Parse(WKPCompanyId);
                    gas_production_model.Date_Updated = DateTime.Now;
                    gas_production_model.Updated_by = WKPCompanyId;
                    gas_production_model.Year_of_WP = year;
                    gas_production_model.OML_Name = gas_production_model.OML_Name.ToUpper();

                    #region file section
                    UploadedDocument Upload_NDR_payment_receipt_File = null;

                    if (files[0] != null)
                    {
                        string docName = "NDR Payment Receipt";
                        Upload_NDR_payment_receipt_File = _helpersController.UploadDocument(files[0], "NDRPaymentReceipt");
                        if (Upload_NDR_payment_receipt_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion


                    gas_production_model.Upload_NDR_payment_receipt = files[0] != null ? Upload_NDR_payment_receipt_File.filePath : null;
                    gas_production_model.NDRFilename = files[0] != null ? Upload_NDR_payment_receipt_File.fileName : null;
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            gas_production_model.Date_Created = DateTime.Now;
                            gas_production_model.Created_by = WKPCompanyId;
                            await _context.GAS_PRODUCTION_ACTIVITIEs.AddAsync(gas_production_model);
                        }
                        else
                        {
                            gas_production_model.Date_Created = getData.Date_Created;
                            gas_production_model.Created_by = getData.Created_by;
                            gas_production_model.Date_Updated = DateTime.Now;
                            gas_production_model.Updated_by = WKPCompanyId;
                            await _context.GAS_PRODUCTION_ACTIVITIEs.AddAsync(gas_production_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.GAS_PRODUCTION_ACTIVITIEs.Remove(gas_production_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_NDR")]
        public async Task<WebApiResponse> GET_NDR(string year, string omlName)
        {
            try
            {
                var getData = (from c in _context.NDRs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_NDR")]
        public async Task<WebApiResponse> POST_NDR([FromBody] NDR ndr_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving NDR data
                if (ndr_model != null)
                {
                    var getData = (from c in _context.NDRs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    ndr_model.Companyemail = WKPCompanyEmail;
                    ndr_model.CompanyName = WKPCompanyName;
                    ndr_model.COMPANY_ID = WKPCompanyId;
                    ndr_model.CompanyNumber = int.Parse(WKPCompanyId);
                    ndr_model.Date_Updated = DateTime.Now;
                    ndr_model.Updated_by = WKPCompanyId;
                    ndr_model.Year_of_WP = year;
                    ndr_model.OML_Name = ndr_model.OML_Name.ToUpper();

                    var getGas_ProductionData = (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).FirstOrDefault();

                    if (getGas_ProductionData != null)
                    {
                        ndr_model.Upload_NDR_payment_receipt = getGas_ProductionData != null ? getGas_ProductionData.Upload_NDR_payment_receipt : null;
                        ndr_model.NDRFilename = getGas_ProductionData != null ? getGas_ProductionData.NDRFilename : null;

                    }
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            ndr_model.Date_Created = DateTime.Now;
                            ndr_model.Created_by = WKPCompanyId;
                            await _context.NDRs.AddAsync(ndr_model);
                        }
                        else
                        {
                            ndr_model.Date_Created = getData.Date_Created;
                            ndr_model.Created_by = getData.Created_by;
                            ndr_model.Date_Updated = DateTime.Now;
                            ndr_model.Updated_by = WKPCompanyId;
                            await _context.NDRs.AddAsync(ndr_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.NDRs.Remove(ndr_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.NDRs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }


        [HttpGet("GET_RESERVES_UPDATES_LIFE_INDEX")]
        public async Task<WebApiResponse> GET_RESERVES_UPDATES_LIFE_INDEX(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.RESERVES_UPDATES_LIFE_INDices where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_RESERVES_UPDATES_LIFE_INDEX")]
        public async Task<WebApiResponse> POST_RESERVES_UPDATES_LIFE_INDEX([FromBody] RESERVES_UPDATES_LIFE_INDEX reserves_life_index_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving RESERVES_UPDATES_LIFE_INDEX data
                if (reserves_life_index_model != null)
                {
                    var getData = (from c in _context.RESERVES_UPDATES_LIFE_INDices where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    reserves_life_index_model.Companyemail = WKPCompanyEmail;
                    reserves_life_index_model.CompanyName = WKPCompanyName;
                    reserves_life_index_model.COMPANY_ID = WKPCompanyId;
                    reserves_life_index_model.CompanyNumber = int.Parse(WKPCompanyId);
                    reserves_life_index_model.Date_Updated = DateTime.Now;
                    reserves_life_index_model.Updated_by = WKPCompanyId;
                    reserves_life_index_model.Year_of_WP = year;
                    reserves_life_index_model.OML_Name = reserves_life_index_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            reserves_life_index_model.Date_Created = DateTime.Now;
                            reserves_life_index_model.Created_by = WKPCompanyId;
                            await _context.RESERVES_UPDATES_LIFE_INDices.AddAsync(reserves_life_index_model);
                        }
                        else
                        {
                            reserves_life_index_model.Date_Created = getData.Date_Created;
                            reserves_life_index_model.Created_by = getData.Created_by;
                            reserves_life_index_model.Date_Updated = DateTime.Now;
                            reserves_life_index_model.Updated_by = WKPCompanyId;
                            await _context.RESERVES_UPDATES_LIFE_INDices.AddAsync(reserves_life_index_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_UPDATES_LIFE_INDices.Remove(reserves_life_index_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE")]
        public async Task<WebApiResponse> GET_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE")]
        public async Task<WebApiResponse> POST_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE([FromBody] RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE reserves_condensate_status_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE data
                if (reserves_condensate_status_model != null)
                {
                    var getData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    reserves_condensate_status_model.Companyemail = WKPCompanyEmail;
                    reserves_condensate_status_model.CompanyName = WKPCompanyName;
                    reserves_condensate_status_model.COMPANY_ID = WKPCompanyId;
                    reserves_condensate_status_model.CompanyNumber = int.Parse(WKPCompanyId);
                    reserves_condensate_status_model.Date_Updated = DateTime.Now;
                    reserves_condensate_status_model.Updated_by = WKPCompanyId;
                    reserves_condensate_status_model.Year_of_WP = year;
                    reserves_condensate_status_model.OML_Name = reserves_condensate_status_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            reserves_condensate_status_model.Date_Created = DateTime.Now;
                            reserves_condensate_status_model.Created_by = WKPCompanyId;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.AddAsync(reserves_condensate_status_model);
                        }
                        else
                        {
                            reserves_condensate_status_model.Date_Created = getData.Date_Created;
                            reserves_condensate_status_model.Created_by = getData.Created_by;
                            reserves_condensate_status_model.Date_Updated = DateTime.Now;
                            reserves_condensate_status_model.Updated_by = WKPCompanyId;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.AddAsync(reserves_condensate_status_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Remove(reserves_condensate_status_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection")]
        public async Task<WebApiResponse> GET_RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection")]
        public async Task<WebApiResponse> POST_RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection([FromBody] RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection reserves_condensate_status_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection data
                if (reserves_condensate_status_model != null)
                {
                    var getData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    reserves_condensate_status_model.Companyemail = WKPCompanyEmail;
                    reserves_condensate_status_model.CompanyName = WKPCompanyName;
                    reserves_condensate_status_model.COMPANY_ID = WKPCompanyId;
                    reserves_condensate_status_model.CompanyNumber = int.Parse(WKPCompanyId);
                    reserves_condensate_status_model.Date_Updated = DateTime.Now;
                    reserves_condensate_status_model.Updated_by = WKPCompanyId;
                    reserves_condensate_status_model.Year_of_WP = year;
                    reserves_condensate_status_model.OML_Name = reserves_condensate_status_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            reserves_condensate_status_model.Date_Created = DateTime.Now;
                            reserves_condensate_status_model.Created_by = WKPCompanyId;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections.AddAsync(reserves_condensate_status_model);
                        }
                        else
                        {
                            reserves_condensate_status_model.Date_Created = getData.Date_Created;
                            reserves_condensate_status_model.Created_by = getData.Created_by;
                            reserves_condensate_status_model.Date_Updated = DateTime.Now;
                            reserves_condensate_status_model.Updated_by = WKPCompanyId;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections.AddAsync(reserves_condensate_status_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections.Remove(reserves_condensate_status_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION")]
        public async Task<WebApiResponse> GET_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION([FromBody] OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION oil_condensate_fiveyears_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION data
                if (oil_condensate_fiveyears_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_condensate_fiveyears_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_fiveyears_model.CompanyName = WKPCompanyName;
                    oil_condensate_fiveyears_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_fiveyears_model.CompanyNumber = int.Parse(WKPCompanyId);
                    oil_condensate_fiveyears_model.Date_Updated = DateTime.Now;
                    oil_condensate_fiveyears_model.Updated_by = WKPCompanyId;
                    oil_condensate_fiveyears_model.Year_of_WP = year;
                    oil_condensate_fiveyears_model.OML_Name = oil_condensate_fiveyears_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            oil_condensate_fiveyears_model.Date_Created = DateTime.Now;
                            oil_condensate_fiveyears_model.Created_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.AddAsync(oil_condensate_fiveyears_model);
                        }
                        else
                        {
                            oil_condensate_fiveyears_model.Date_Created = getData.Date_Created;
                            oil_condensate_fiveyears_model.Created_by = getData.Created_by;
                            oil_condensate_fiveyears_model.Date_Updated = DateTime.Now;
                            oil_condensate_fiveyears_model.Updated_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.AddAsync(oil_condensate_fiveyears_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.Remove(oil_condensate_fiveyears_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION")]
        public async Task<WebApiResponse> GET_RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION")]
        public async Task<WebApiResponse> POST_RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION([FromBody] RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION reserves_update_production_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION data
                if (reserves_update_production_model != null)
                {
                    var getData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    reserves_update_production_model.Companyemail = WKPCompanyEmail;
                    reserves_update_production_model.CompanyName = WKPCompanyName;
                    reserves_update_production_model.COMPANY_ID = WKPCompanyId;
                    reserves_update_production_model.CompanyNumber = int.Parse(WKPCompanyId);
                    reserves_update_production_model.Date_Updated = DateTime.Now;
                    reserves_update_production_model.Updated_by = WKPCompanyId;
                    reserves_update_production_model.Year_of_WP = year;
                    reserves_update_production_model.OML_Name = reserves_update_production_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            reserves_update_production_model.Date_Created = DateTime.Now;
                            reserves_update_production_model.Created_by = WKPCompanyId;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.AddAsync(reserves_update_production_model);
                        }
                        else
                        {
                            reserves_update_production_model.Date_Created = getData.Date_Created;
                            reserves_update_production_model.Created_by = getData.Created_by;
                            reserves_update_production_model.Date_Updated = DateTime.Now;
                            reserves_update_production_model.Updated_by = WKPCompanyId;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.AddAsync(reserves_update_production_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.Remove(reserves_update_production_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE")]
        public async Task<WebApiResponse> GET_RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE")]
        public async Task<WebApiResponse> POST_RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE([FromBody] RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE reserves_update_decline_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE data
                if (reserves_update_decline_model != null)
                {
                    var getData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    reserves_update_decline_model.Companyemail = WKPCompanyEmail;
                    reserves_update_decline_model.CompanyName = WKPCompanyName;
                    reserves_update_decline_model.COMPANY_ID = WKPCompanyId;
                    reserves_update_decline_model.CompanyNumber = int.Parse(WKPCompanyId);
                    reserves_update_decline_model.Date_Updated = DateTime.Now;
                    reserves_update_decline_model.Updated_by = WKPCompanyId;
                    reserves_update_decline_model.Year_of_WP = year;
                    reserves_update_decline_model.OML_Name = reserves_update_decline_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            reserves_update_decline_model.Date_Created = DateTime.Now;
                            reserves_update_decline_model.Created_by = WKPCompanyId;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.AddAsync(reserves_update_decline_model);
                        }
                        else
                        {
                            reserves_update_decline_model.Date_Created = getData.Date_Created;
                            reserves_update_decline_model.Created_by = getData.Created_by;
                            reserves_update_decline_model.Date_Updated = DateTime.Now;
                            reserves_update_decline_model.Updated_by = WKPCompanyId;
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.AddAsync(reserves_update_decline_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.Remove(reserves_update_decline_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity([FromBody] OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity oil_condensate_reserves_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity data
                if (oil_condensate_reserves_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_condensate_reserves_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_reserves_model.CompanyName = WKPCompanyName;
                    oil_condensate_reserves_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_reserves_model.CompanyNumber = int.Parse(WKPCompanyId);
                    oil_condensate_reserves_model.Date_Updated = DateTime.Now;
                    oil_condensate_reserves_model.Updated_by = WKPCompanyId;
                    oil_condensate_reserves_model.Year_of_WP = year;
                    oil_condensate_reserves_model.OML_Name = oil_condensate_reserves_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            oil_condensate_reserves_model.Date_Created = DateTime.Now;
                            oil_condensate_reserves_model.Created_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.AddAsync(oil_condensate_reserves_model);
                        }
                        else
                        {
                            oil_condensate_reserves_model.Date_Created = getData.Date_Created;
                            oil_condensate_reserves_model.Created_by = getData.Created_by;
                            oil_condensate_reserves_model.Date_Updated = DateTime.Now;
                            oil_condensate_reserves_model.Updated_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.AddAsync(oil_condensate_reserves_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.Remove(oil_condensate_reserves_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_RESERVES_REPLACEMENT_RATIO")]
        public async Task<WebApiResponse> GET_RESERVES_REPLACEMENT_RATIO(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.RESERVES_REPLACEMENT_RATIOs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_RESERVES_REPLACEMENT_RATIO")]
        public async Task<WebApiResponse> POST_RESERVES_REPLACEMENT_RATIO([FromBody] RESERVES_REPLACEMENT_RATIO reserves_replacement_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving RESERVES_REPLACEMENT_RATIO data
                if (reserves_replacement_model != null)
                {
                    var getData = (from c in _context.RESERVES_REPLACEMENT_RATIOs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    reserves_replacement_model.Companyemail = WKPCompanyEmail;
                    reserves_replacement_model.CompanyName = WKPCompanyName;
                    reserves_replacement_model.COMPANY_ID = WKPCompanyId;
                    reserves_replacement_model.CompanyNumber = int.Parse(WKPCompanyId);
                    reserves_replacement_model.Date_Updated = DateTime.Now;
                    reserves_replacement_model.Updated_by = WKPCompanyId;
                    reserves_replacement_model.Year_of_WP = year;
                    reserves_replacement_model.OML_Name = reserves_replacement_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            reserves_replacement_model.Date_Created = DateTime.Now;
                            reserves_replacement_model.Created_by = WKPCompanyId;
                            await _context.RESERVES_REPLACEMENT_RATIOs.AddAsync(reserves_replacement_model);
                        }
                        else
                        {
                            reserves_replacement_model.Date_Created = getData.Date_Created;
                            reserves_replacement_model.Created_by = getData.Created_by;
                            reserves_replacement_model.Date_Updated = DateTime.Now;
                            reserves_replacement_model.Updated_by = WKPCompanyId;
                            await _context.RESERVES_REPLACEMENT_RATIOs.AddAsync(reserves_replacement_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_REPLACEMENT_RATIOs.Remove(reserves_replacement_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.RESERVES_REPLACEMENT_RATIOs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED")]
        public async Task<WebApiResponse> GET_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED([FromBody] OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED oil_condensate_monthly_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED data
                if (oil_condensate_monthly_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_condensate_monthly_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_monthly_model.CompanyName = WKPCompanyName;
                    oil_condensate_monthly_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_monthly_model.CompanyNumber = int.Parse(WKPCompanyId);
                    oil_condensate_monthly_model.Date_Updated = DateTime.Now;
                    oil_condensate_monthly_model.Updated_by = WKPCompanyId;
                    oil_condensate_monthly_model.Year_of_WP = year;
                    oil_condensate_monthly_model.OML_Name = oil_condensate_monthly_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            oil_condensate_monthly_model.Date_Created = DateTime.Now;
                            oil_condensate_monthly_model.Created_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.AddAsync(oil_condensate_monthly_model);
                        }
                        else
                        {
                            oil_condensate_monthly_model.Date_Created = getData.Date_Created;
                            oil_condensate_monthly_model.Created_by = getData.Created_by;
                            oil_condensate_monthly_model.Date_Updated = DateTime.Now;
                            oil_condensate_monthly_model.Updated_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.AddAsync(oil_condensate_monthly_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.Remove(oil_condensate_monthly_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_OIL_GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY")]
        public async Task<WebApiResponse> GET_OIL_GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_OIL_GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY")]
        public async Task<WebApiResponse> POST_OIL_GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY([FromBody] GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY oil_gas_domestic_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY data
                if (oil_gas_domestic_model != null)
                {
                    var getData = (from c in _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_gas_domestic_model.Companyemail = WKPCompanyEmail;
                    oil_gas_domestic_model.CompanyName = WKPCompanyName;
                    oil_gas_domestic_model.COMPANY_ID = WKPCompanyId;
                    oil_gas_domestic_model.CompanyNumber = int.Parse(WKPCompanyId);
                    oil_gas_domestic_model.Date_Updated = DateTime.Now;
                    oil_gas_domestic_model.Updated_by = WKPCompanyId;
                    oil_gas_domestic_model.Year_of_WP = year;
                    oil_gas_domestic_model.OML_Name = oil_gas_domestic_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            oil_gas_domestic_model.Date_Created = DateTime.Now;
                            oil_gas_domestic_model.Created_by = WKPCompanyId;
                            await _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.AddAsync(oil_gas_domestic_model);
                        }
                        else
                        {
                            oil_gas_domestic_model.Date_Created = getData.Date_Created;
                            oil_gas_domestic_model.Created_by = getData.Created_by;
                            oil_gas_domestic_model.Date_Updated = DateTime.Now;
                            oil_gas_domestic_model.Updated_by = WKPCompanyId;
                            await _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.AddAsync(oil_gas_domestic_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.Remove(oil_gas_domestic_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_BUDGET_ACTUAL_EXPENDITURE")]
        public async Task<WebApiResponse> GET_BUDGET_ACTUAL_EXPENDITURE(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.BUDGET_ACTUAL_EXPENDITUREs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_BUDGET_ACTUAL_EXPENDITURE")]
        public async Task<WebApiResponse> POST_BUDGET_ACTUAL_EXPENDITURE([FromBody] BUDGET_ACTUAL_EXPENDITURE budget_actual_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving BUDGET_ACTUAL_EXPENDITURE data
                if (budget_actual_model != null)
                {
                    var getData = (from c in _context.BUDGET_ACTUAL_EXPENDITUREs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_actual_model.Companyemail = WKPCompanyEmail;
                    budget_actual_model.CompanyName = WKPCompanyName;
                    budget_actual_model.COMPANY_ID = WKPCompanyId;
                    budget_actual_model.CompanyNumber = int.Parse(WKPCompanyId);
                    budget_actual_model.Date_Updated = DateTime.Now;
                    budget_actual_model.Updated_by = WKPCompanyId;
                    budget_actual_model.Year_of_WP = year;
                    budget_actual_model.OML_Name = budget_actual_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            budget_actual_model.Date_Created = DateTime.Now;
                            budget_actual_model.Created_by = WKPCompanyId;
                            await _context.BUDGET_ACTUAL_EXPENDITUREs.AddAsync(budget_actual_model);
                        }
                        else
                        {
                            budget_actual_model.Date_Created = getData.Date_Created;
                            budget_actual_model.Created_by = getData.Created_by;
                            budget_actual_model.Date_Updated = DateTime.Now;
                            budget_actual_model.Updated_by = WKPCompanyId;
                            _context.BUDGET_ACTUAL_EXPENDITUREs.Remove(getData);
                            await _context.BUDGET_ACTUAL_EXPENDITUREs.AddAsync(budget_actual_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.BUDGET_ACTUAL_EXPENDITUREs.Remove(budget_actual_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_ACTUAL_EXPENDITUREs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT")]
        public async Task<WebApiResponse> GET_BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT")]
        public async Task<WebApiResponse> POST_BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT([FromBody] BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT budget_proposal_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT data
                if (budget_proposal_model != null)
                {
                    var getData = (from c in _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_proposal_model.Companyemail = WKPCompanyEmail;
                    budget_proposal_model.CompanyName = WKPCompanyName;
                    budget_proposal_model.COMPANY_ID = WKPCompanyId;
                    budget_proposal_model.CompanyNumber = int.Parse(WKPCompanyId);
                    budget_proposal_model.Date_Updated = DateTime.Now;
                    budget_proposal_model.Updated_by = WKPCompanyId;
                    budget_proposal_model.Year_of_WP = year;
                    budget_proposal_model.OML_Name = budget_proposal_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            budget_proposal_model.Date_Created = DateTime.Now;
                            budget_proposal_model.Created_by = WKPCompanyId;
                            await _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs.AddAsync(budget_proposal_model);
                        }
                        else
                        {
                            budget_proposal_model.Date_Created = getData.Date_Created;
                            budget_proposal_model.Created_by = getData.Created_by;
                            budget_proposal_model.Date_Updated = DateTime.Now;
                            budget_proposal_model.Updated_by = WKPCompanyId;
                            await _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs.AddAsync(budget_proposal_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs.Remove(budget_proposal_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITY")]
        public async Task<WebApiResponse> GET_BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITY")]
        public async Task<WebApiResponse> POST_BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy([FromBody] BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy budget_exploratory_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy data
                if (budget_exploratory_model != null)
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_exploratory_model.Companyemail = WKPCompanyEmail;
                    budget_exploratory_model.CompanyName = WKPCompanyName;
                    budget_exploratory_model.COMPANY_ID = WKPCompanyId;
                    budget_exploratory_model.CompanyNumber = int.Parse(WKPCompanyId);
                    budget_exploratory_model.Date_Updated = DateTime.Now;
                    budget_exploratory_model.Updated_by = WKPCompanyId;
                    budget_exploratory_model.Year_of_WP = year;
                    budget_exploratory_model.OML_Name = budget_exploratory_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            budget_exploratory_model.Date_Created = DateTime.Now;
                            budget_exploratory_model.Created_by = WKPCompanyId;
                            await _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs.AddAsync(budget_exploratory_model);
                        }
                        else
                        {
                            budget_exploratory_model.Date_Created = getData.Date_Created;
                            budget_exploratory_model.Created_by = getData.Created_by;
                            budget_exploratory_model.Date_Updated = DateTime.Now;
                            budget_exploratory_model.Updated_by = WKPCompanyId;
                            _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs.Remove(getData);
                            await _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs.AddAsync(budget_exploratory_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs.Remove(budget_exploratory_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }


        [HttpGet("GET_BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITY")]
        public async Task<WebApiResponse> GET_BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITY")]
        public async Task<WebApiResponse> POST_BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy([FromBody] BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy budget_proposal_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy data
                if (budget_proposal_model != null)
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_proposal_model.Companyemail = WKPCompanyEmail;
                    budget_proposal_model.CompanyName = WKPCompanyName;
                    budget_proposal_model.COMPANY_ID = WKPCompanyId;
                    budget_proposal_model.CompanyNumber = int.Parse(WKPCompanyId);
                    budget_proposal_model.Date_Updated = DateTime.Now;
                    budget_proposal_model.Updated_by = WKPCompanyId;
                    budget_proposal_model.Year_of_WP = year;
                    budget_proposal_model.OML_Name = budget_proposal_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            budget_proposal_model.Date_Created = DateTime.Now;
                            budget_proposal_model.Created_by = WKPCompanyId;
                            await _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs.AddAsync(budget_proposal_model);
                        }
                        else
                        {
                            budget_proposal_model.Date_Created = getData.Date_Created;
                            budget_proposal_model.Created_by = getData.Created_by;
                            budget_proposal_model.Date_Updated = DateTime.Now;
                            budget_proposal_model.Updated_by = WKPCompanyId;
                            _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs.Remove(getData);
                            await _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs.AddAsync(budget_proposal_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs.Remove(budget_proposal_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        
        
        [HttpGet("GET_BUDGET_PERFORMANCE_PRODUCTION_COST")]
        public async Task<WebApiResponse> GET_BUDGET_PERFORMANCE_PRODUCTION_COST(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_BUDGET_PERFORMANCE_PRODUCTION_COST")]
        public async Task<WebApiResponse> POST_BUDGET_PERFORMANCE_PRODUCTION_COST([FromBody] BUDGET_PERFORMANCE_PRODUCTION_COST budget_performance_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving BUDGET_PERFORMANCE_PRODUCTION_COST data
                if (budget_performance_model != null)
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_performance_model.Companyemail = WKPCompanyEmail;
                    budget_performance_model.CompanyName = WKPCompanyName;
                    budget_performance_model.COMPANY_ID = WKPCompanyId;
                    budget_performance_model.CompanyNumber = int.Parse(WKPCompanyId);
                    budget_performance_model.Date_Updated = DateTime.Now;
                    budget_performance_model.Updated_by = WKPCompanyId;
                    budget_performance_model.Year_of_WP = year;
                    budget_performance_model.OML_Name = budget_performance_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            budget_performance_model.Date_Created = DateTime.Now;
                            budget_performance_model.Created_by = WKPCompanyId;
                            await _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs.AddAsync(budget_performance_model);
                        }
                        else
                        {
                            budget_performance_model.Date_Created = getData.Date_Created;
                            budget_performance_model.Created_by = getData.Created_by;
                            budget_performance_model.Date_Updated = DateTime.Now;
                            budget_performance_model.Updated_by = WKPCompanyId;
                            _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs.Remove(getData);
                            await _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs.AddAsync(budget_performance_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs.Remove(budget_performance_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT")]
        public async Task<WebApiResponse> GET_BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT")]
        public async Task<WebApiResponse> POST_BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT([FromBody] BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT budget_facilities_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT data
                if (budget_facilities_model != null)
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_facilities_model.Companyemail = WKPCompanyEmail;
                    budget_facilities_model.CompanyName = WKPCompanyName;
                    budget_facilities_model.COMPANY_ID = WKPCompanyId;
                    budget_facilities_model.CompanyNumber = int.Parse(WKPCompanyId);
                    budget_facilities_model.Date_Updated = DateTime.Now;
                    budget_facilities_model.Updated_by = WKPCompanyId;
                    budget_facilities_model.Year_of_WP = year;
                    budget_facilities_model.OML_Name = budget_facilities_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            budget_facilities_model.Date_Created = DateTime.Now;
                            budget_facilities_model.Created_by = WKPCompanyId;
                            await _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs.AddAsync(budget_facilities_model);
                        }
                        else
                        {
                            budget_facilities_model.Date_Created = getData.Date_Created;
                            budget_facilities_model.Created_by = getData.Created_by;
                            budget_facilities_model.Date_Updated = DateTime.Now;
                            budget_facilities_model.Updated_by = WKPCompanyId;
                            _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs.Remove(getData);

                            await _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs.AddAsync(budget_facilities_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs.Remove(budget_facilities_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE")]
        public async Task<WebApiResponse> GET_OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE")]
        public async Task<WebApiResponse> POST_OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE([FromBody] OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE oil_gas_facility_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE data
                if (oil_gas_facility_model != null)
                {
                    var getData = (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_gas_facility_model.Companyemail = WKPCompanyEmail;
                    oil_gas_facility_model.CompanyName = WKPCompanyName;
                    oil_gas_facility_model.COMPANY_ID = WKPCompanyId;
                    oil_gas_facility_model.CompanyNumber = int.Parse(WKPCompanyId);
                    oil_gas_facility_model.Date_Updated = DateTime.Now;
                    oil_gas_facility_model.Updated_by = WKPCompanyId;
                    oil_gas_facility_model.Year_of_WP = year;
                    oil_gas_facility_model.OML_Name = oil_gas_facility_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            oil_gas_facility_model.Date_Created = DateTime.Now;
                            oil_gas_facility_model.Created_by = WKPCompanyId;
                            await _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs.AddAsync(oil_gas_facility_model);
                        }
                        else
                        {
                            oil_gas_facility_model.Date_Created = getData.Date_Created;
                            oil_gas_facility_model.Created_by = getData.Created_by;
                            oil_gas_facility_model.Date_Updated = DateTime.Now;
                            oil_gas_facility_model.Updated_by = WKPCompanyId;
                            await _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs.AddAsync(oil_gas_facility_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs.Remove(oil_gas_facility_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_NEW_TECHNOLOGY_CONFORMITY_ASSESSMENT")]
        public async Task<WebApiResponse> GET_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_NEW_TECHNOLOGY_CONFORMITY_ASSESSMENT")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment([FromBody] OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment oil_condensate_assessment_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments data
                if (oil_condensate_assessment_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_condensate_assessment_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_assessment_model.CompanyName = WKPCompanyName;
                    oil_condensate_assessment_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_assessment_model.CompanyNumber = int.Parse(WKPCompanyId);
                    oil_condensate_assessment_model.Date_Updated = DateTime.Now;
                    oil_condensate_assessment_model.Updated_by = WKPCompanyId;
                    oil_condensate_assessment_model.Year_of_WP = year;
                    oil_condensate_assessment_model.OML_Name = oil_condensate_assessment_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            oil_condensate_assessment_model.Date_Created = DateTime.Now;
                            oil_condensate_assessment_model.Created_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.AddAsync(oil_condensate_assessment_model);
                        }
                        else
                        {
                            oil_condensate_assessment_model.Date_Created = getData.Date_Created;
                            oil_condensate_assessment_model.Created_by = getData.Created_by;
                            oil_condensate_assessment_model.Date_Updated = DateTime.Now;
                            oil_condensate_assessment_model.Updated_by = WKPCompanyId;
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.AddAsync(oil_condensate_assessment_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.Remove(oil_condensate_assessment_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT")]
        public async Task<WebApiResponse> GET_OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT")]
        public async Task<WebApiResponse> POST_OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT([FromBody] OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT oil_gas_facility_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs data
                if (oil_gas_facility_model != null)
                {
                    var getData = (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_gas_facility_model.Companyemail = WKPCompanyEmail;
                    oil_gas_facility_model.CompanyName = WKPCompanyName;
                    oil_gas_facility_model.COMPANY_ID = WKPCompanyId;
                    oil_gas_facility_model.CompanyNumber = int.Parse(WKPCompanyId);
                    oil_gas_facility_model.Date_Updated = DateTime.Now;
                    oil_gas_facility_model.Updated_by = WKPCompanyId;
                    oil_gas_facility_model.Year_of_WP = year;
                    oil_gas_facility_model.OML_Name = oil_gas_facility_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            oil_gas_facility_model.Date_Created = DateTime.Now;
                            oil_gas_facility_model.Created_by = WKPCompanyId;
                            await _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.AddAsync(oil_gas_facility_model);
                        }
                        else
                        {
                            oil_gas_facility_model.Date_Created = getData.Date_Created;
                            oil_gas_facility_model.Created_by = getData.Created_by;
                            oil_gas_facility_model.Date_Updated = DateTime.Now;
                            oil_gas_facility_model.Updated_by = WKPCompanyId;
                            await _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.AddAsync(oil_gas_facility_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Remove(oil_gas_facility_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_FACILITIES_PROJECT_PERFORMANCE")]
        public async Task<WebApiResponse> GET_FACILITIES_PROJECT_PERFORMANCE(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.FACILITIES_PROJECT_PERFORMANCEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_FACILITIES_PROJECT_PERFORMANCE")]
        public async Task<WebApiResponse> POST_FACILITIES_PROJECT_PERFORMANCE([FromBody] FACILITIES_PROJECT_PERFORMANCE facilities_project_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving FACILITIES_PROJECT_PERFORMANCEs data
                if (facilities_project_model != null)
                {
                    var getData = (from c in _context.FACILITIES_PROJECT_PERFORMANCEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    facilities_project_model.Companyemail = WKPCompanyEmail;
                    facilities_project_model.CompanyName = WKPCompanyName;
                    facilities_project_model.COMPANY_ID = WKPCompanyId;
                    facilities_project_model.CompanyNumber = int.Parse(WKPCompanyId);
                    facilities_project_model.Date_Updated = DateTime.Now;
                    facilities_project_model.Updated_by = WKPCompanyId;
                    facilities_project_model.Year_of_WP = year;
                    facilities_project_model.OML_Name = facilities_project_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            facilities_project_model.Date_Created = DateTime.Now;
                            facilities_project_model.Created_by = WKPCompanyId;
                            await _context.FACILITIES_PROJECT_PERFORMANCEs.AddAsync(facilities_project_model);
                        }
                        else
                        {
                            facilities_project_model.Date_Created = getData.Date_Created;
                            facilities_project_model.Created_by = getData.Created_by;
                            facilities_project_model.Date_Updated = DateTime.Now;
                            facilities_project_model.Updated_by = WKPCompanyId;
                            await _context.FACILITIES_PROJECT_PERFORMANCEs.AddAsync(facilities_project_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.FACILITIES_PROJECT_PERFORMANCEs.Remove(facilities_project_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.FACILITIES_PROJECT_PERFORMANCEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_BUDGET_CAPEX_OPEX")]
        public async Task<WebApiResponse> GET_BUDGET_CAPEX_OPEX(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.BUDGET_CAPEX_OPices where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_BUDGET_CAPEX_OPEX")]
        public async Task<WebApiResponse> POST_BUDGET_CAPEX_OPEX([FromBody] BUDGET_CAPEX_OPEX budget_capex_opex_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving BUDGET_CAPEX_OPEXs data
                if (budget_capex_opex_model != null)
                {
                    var getData = (from c in _context.BUDGET_CAPEX_OPices where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_capex_opex_model.Companyemail = WKPCompanyEmail;
                    budget_capex_opex_model.CompanyName = WKPCompanyName;
                    budget_capex_opex_model.COMPANY_ID = WKPCompanyId;
                    budget_capex_opex_model.CompanyNumber = int.Parse(WKPCompanyId);
                    budget_capex_opex_model.Date_Updated = DateTime.Now;
                    budget_capex_opex_model.Updated_by = WKPCompanyId;
                    budget_capex_opex_model.Year_of_WP = year;
                    budget_capex_opex_model.OML_Name = budget_capex_opex_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            budget_capex_opex_model.Date_Created = DateTime.Now;
                            budget_capex_opex_model.Created_by = WKPCompanyId;
                            await _context.BUDGET_CAPEX_OPices.AddAsync(budget_capex_opex_model);
                        }
                        else
                        {
                            budget_capex_opex_model.Date_Created = getData.Date_Created;
                            budget_capex_opex_model.Created_by = getData.Created_by;
                            budget_capex_opex_model.Date_Updated = DateTime.Now;
                            budget_capex_opex_model.Updated_by = WKPCompanyId;
                            await _context.BUDGET_CAPEX_OPices.AddAsync(budget_capex_opex_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.BUDGET_CAPEX_OPices.Remove(budget_capex_opex_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_CAPEX_OPices where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("BUDGET_CAPEX_OPEX")]
        public async Task<WebApiResponse> BUDGET_CAPEX_OPEX(BUDGET_CAPEX_OPEX_Model wkp, string year, string ActionToDo = null)
        {

            int save = 0;
            var BudgetCapexOpexData = new BUDGET_CAPEX_OPEX();
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving Budget Capex Opex

                var getBudgetCapexOpexData = (from c in _context.BUDGET_CAPEX_OPices where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).FirstOrDefault();

                BudgetCapexOpexData = getBudgetCapexOpexData != null ? getBudgetCapexOpexData : BudgetCapexOpexData;
                BudgetCapexOpexData = _mapper.Map<BUDGET_CAPEX_OPEX>(wkp);

                BudgetCapexOpexData.Companyemail = WKPCompanyEmail;
                BudgetCapexOpexData.CompanyName = WKPCompanyName;
                BudgetCapexOpexData.COMPANY_ID = WKPCompanyId;
                BudgetCapexOpexData.Date_Updated = DateTime.Now;
                BudgetCapexOpexData.Updated_by = WKPCompanyId;
                BudgetCapexOpexData.Year_of_WP = DateTime.Now.Year.ToString();
                if (getBudgetCapexOpexData == null)
                {
                    BudgetCapexOpexData.Created_by = WKPCompanyId;
                    BudgetCapexOpexData.Date_Created = DateTime.Now;
                    await _context.BUDGET_CAPEX_OPices.AddAsync(BudgetCapexOpexData);
                }
                else if (action == GeneralModel.Delete)
                {
                    _context.BUDGET_CAPEX_OPices.Remove(BudgetCapexOpexData);
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = wkp, StatusCode = ResponseCodes.Success };
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

        [HttpGet("GET_NIGERIA_CONTENT_TRAINING")]
        public async Task<WebApiResponse> GET_NIGERIA_CONTENT_Training(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.NIGERIA_CONTENT_Trainings where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_NIGERIA_CONTENT_TRAINING")]
        public async Task<WebApiResponse> POST_NIGERIA_CONTENT_Training([FromBody] NIGERIA_CONTENT_Training nigeria_content_training_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving NIGERIA_CONTENT_Trainings data
                if (nigeria_content_training_model != null)
                {
                    var getData = (from c in _context.NIGERIA_CONTENT_Trainings where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    nigeria_content_training_model.Companyemail = WKPCompanyEmail;
                    nigeria_content_training_model.CompanyName = WKPCompanyName;
                    nigeria_content_training_model.COMPANY_ID = WKPCompanyId;
                    nigeria_content_training_model.CompanyNumber = int.Parse(WKPCompanyId);
                    nigeria_content_training_model.Date_Updated = DateTime.Now;
                    nigeria_content_training_model.Updated_by = WKPCompanyId;
                    nigeria_content_training_model.Year_of_WP = year;
                    nigeria_content_training_model.OML_Name = nigeria_content_training_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            nigeria_content_training_model.Date_Created = DateTime.Now;
                            nigeria_content_training_model.Created_by = WKPCompanyId;
                            await _context.NIGERIA_CONTENT_Trainings.AddAsync(nigeria_content_training_model);
                        }
                        else
                        {
                            nigeria_content_training_model.Date_Created = getData.Date_Created;
                            nigeria_content_training_model.Created_by = getData.Created_by;
                            nigeria_content_training_model.Date_Updated = DateTime.Now;
                            nigeria_content_training_model.Updated_by = WKPCompanyId;
                            await _context.NIGERIA_CONTENT_Trainings.AddAsync(nigeria_content_training_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.NIGERIA_CONTENT_Trainings.Remove(nigeria_content_training_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.NIGERIA_CONTENT_Trainings where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_NIGERIA_CONTENT_UPLOAD_SUCCESSION_PLAN")]
        public async Task<WebApiResponse> GET_NIGERIA_CONTENT_UPLOAD_SUCCESSION_PLAN(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.NIGERIA_CONTENT_Upload_Succession_Plans where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_NIGERIA_CONTENT_UPLOAD_SUCCESSION_PLAN")]
        public async Task<WebApiResponse> POST_NIGERIA_CONTENT_UPLOAD_SUCCESSION_PLAN([FromBody] NIGERIA_CONTENT_Upload_Succession_Plan nigeria_content_succession_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving NIGERIA_CONTENT_Upload_Succession_Plans data
                if (nigeria_content_succession_model != null)
                {
                    var getData = (from c in _context.NIGERIA_CONTENT_Upload_Succession_Plans where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    nigeria_content_succession_model.Companyemail = WKPCompanyEmail;
                    nigeria_content_succession_model.CompanyName = WKPCompanyName;
                    nigeria_content_succession_model.COMPANY_ID = WKPCompanyId;
                    nigeria_content_succession_model.CompanyNumber = int.Parse(WKPCompanyId);
                    nigeria_content_succession_model.Date_Updated = DateTime.Now;
                    nigeria_content_succession_model.Updated_by = WKPCompanyId;
                    nigeria_content_succession_model.Year_of_WP = year;
                    nigeria_content_succession_model.OML_Name = nigeria_content_succession_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            nigeria_content_succession_model.Date_Created = DateTime.Now;
                            nigeria_content_succession_model.Created_by = WKPCompanyId;
                            await _context.NIGERIA_CONTENT_Upload_Succession_Plans.AddAsync(nigeria_content_succession_model);
                        }
                        else
                        {
                            nigeria_content_succession_model.Date_Created = getData.Date_Created;
                            nigeria_content_succession_model.Created_by = getData.Created_by;
                            nigeria_content_succession_model.Date_Updated = DateTime.Now;
                            nigeria_content_succession_model.Updated_by = WKPCompanyId;
                            await _context.NIGERIA_CONTENT_Upload_Succession_Plans.AddAsync(nigeria_content_succession_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.NIGERIA_CONTENT_Upload_Succession_Plans.Remove(nigeria_content_succession_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.NIGERIA_CONTENT_Upload_Succession_Plans where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_NIGERIA_CONTENT_QUESTION")]
        public async Task<WebApiResponse> GET_NIGERIA_CONTENT_QUESTION(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.NIGERIA_CONTENT_QUESTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_NIGERIA_CONTENT_QUESTION")]
        public async Task<WebApiResponse> POST_NIGERIA_CONTENT_QUESTION([FromBody] NIGERIA_CONTENT_QUESTION nigeria_content_question_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving NIGERIA_CONTENT_QUESTIONs data
                if (nigeria_content_question_model != null)
                {
                    var getData = (from c in _context.NIGERIA_CONTENT_QUESTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    nigeria_content_question_model.Companyemail = WKPCompanyEmail;
                    nigeria_content_question_model.CompanyName = WKPCompanyName;
                    nigeria_content_question_model.COMPANY_ID = WKPCompanyId;
                    nigeria_content_question_model.CompanyNumber = int.Parse(WKPCompanyId);
                    nigeria_content_question_model.Date_Updated = DateTime.Now;
                    nigeria_content_question_model.Updated_by = WKPCompanyId;
                    nigeria_content_question_model.Year_of_WP = year;
                    nigeria_content_question_model.OML_Name = nigeria_content_question_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            nigeria_content_question_model.Date_Created = DateTime.Now;
                            nigeria_content_question_model.Created_by = WKPCompanyId;
                            await _context.NIGERIA_CONTENT_QUESTIONs.AddAsync(nigeria_content_question_model);
                        }
                        else
                        {
                            nigeria_content_question_model.Date_Created = getData.Date_Created;
                            nigeria_content_question_model.Created_by = getData.Created_by;
                            nigeria_content_question_model.Date_Updated = DateTime.Now;
                            nigeria_content_question_model.Updated_by = WKPCompanyId;
                            await _context.NIGERIA_CONTENT_QUESTIONs.AddAsync(nigeria_content_question_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.NIGERIA_CONTENT_QUESTIONs.Remove(nigeria_content_question_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.NIGERIA_CONTENT_QUESTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_LEGAL_LITIGATION")]
        public async Task<WebApiResponse> GET_LEGAL_LITIGATION(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.LEGAL_LITIGATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_LEGAL_LITIGATION")]
        public async Task<WebApiResponse> POST_LEGAL_LITIGATION([FromBody] LEGAL_LITIGATION legal_litigation_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving LEGAL_LITIGATIONs data
                if (legal_litigation_model != null)
                {
                    var getData = (from c in _context.LEGAL_LITIGATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    legal_litigation_model.Companyemail = WKPCompanyEmail;
                    legal_litigation_model.CompanyName = WKPCompanyName;
                    legal_litigation_model.COMPANY_ID = WKPCompanyId;
                    legal_litigation_model.CompanyNumber = int.Parse(WKPCompanyId);
                    legal_litigation_model.Date_Updated = DateTime.Now;
                    legal_litigation_model.Updated_by = WKPCompanyId;
                    legal_litigation_model.Year_of_WP = year;
                    legal_litigation_model.OML_Name = legal_litigation_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            legal_litigation_model.Date_Created = DateTime.Now;
                            legal_litigation_model.Created_by = WKPCompanyId;
                            await _context.LEGAL_LITIGATIONs.AddAsync(legal_litigation_model);
                        }
                        else
                        {
                            legal_litigation_model.Date_Created = getData.Date_Created;
                            legal_litigation_model.Created_by = getData.Created_by;
                            legal_litigation_model.Date_Updated = DateTime.Now;
                            legal_litigation_model.Updated_by = WKPCompanyId;
                            await _context.LEGAL_LITIGATIONs.AddAsync(legal_litigation_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.LEGAL_LITIGATIONs.Remove(legal_litigation_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.LEGAL_LITIGATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_LEGAL_ARBITRATION")]
        public async Task<WebApiResponse> GET_LEGAL_ARBITRATION(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.LEGAL_ARBITRATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_LEGAL_ARBITRATION")]
        public async Task<WebApiResponse> POST_LEGAL_ARBITRATION([FromBody] LEGAL_ARBITRATION legal_arbitration_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving LEGAL_ARBITRATIONs data
                if (legal_arbitration_model != null)
                {
                    var getData = (from c in _context.LEGAL_ARBITRATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    legal_arbitration_model.Companyemail = WKPCompanyEmail;
                    legal_arbitration_model.CompanyName = WKPCompanyName;
                    legal_arbitration_model.COMPANY_ID = WKPCompanyId;
                    legal_arbitration_model.CompanyNumber = int.Parse(WKPCompanyId);
                    legal_arbitration_model.Date_Updated = DateTime.Now;
                    legal_arbitration_model.Updated_by = WKPCompanyId;
                    legal_arbitration_model.Year_of_WP = year;
                    legal_arbitration_model.OML_Name = legal_arbitration_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            legal_arbitration_model.Date_Created = DateTime.Now;
                            legal_arbitration_model.Created_by = WKPCompanyId;
                            await _context.LEGAL_ARBITRATIONs.AddAsync(legal_arbitration_model);
                        }
                        else
                        {
                            legal_arbitration_model.Date_Created = getData.Date_Created;
                            legal_arbitration_model.Created_by = getData.Created_by;
                            legal_arbitration_model.Date_Updated = DateTime.Now;
                            legal_arbitration_model.Updated_by = WKPCompanyId;
                            await _context.LEGAL_ARBITRATIONs.AddAsync(legal_arbitration_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.LEGAL_ARBITRATIONs.Remove(legal_arbitration_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.LEGAL_ARBITRATIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_STRATEGIC_PLANS_ON_COMPANY_BASES")]
        public async Task<WebApiResponse> GET_STRATEGIC_PLANS_ON_COMPANY_BASES(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.STRATEGIC_PLANS_ON_COMPANY_BAses where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_STRATEGIC_PLANS_ON_COMPANY_BASES")]
        public async Task<WebApiResponse> POST_STRATEGIC_PLANS_ON_COMPANY_BASI([FromBody] STRATEGIC_PLANS_ON_COMPANY_BASI strategic_plans_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving STRATEGIC_PLANS_ON_COMPANY_BASIs data
                if (strategic_plans_model != null)
                {
                    var getData = (from c in _context.STRATEGIC_PLANS_ON_COMPANY_BAses where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    strategic_plans_model.Companyemail = WKPCompanyEmail;
                    strategic_plans_model.CompanyName = WKPCompanyName;
                    strategic_plans_model.COMPANY_ID = WKPCompanyId;
                    strategic_plans_model.CompanyNumber = int.Parse(WKPCompanyId);
                    strategic_plans_model.Date_Updated = DateTime.Now;
                    strategic_plans_model.Updated_by = WKPCompanyId;
                    strategic_plans_model.Year_of_WP = year;
                    strategic_plans_model.OML_Name = strategic_plans_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            strategic_plans_model.Date_Created = DateTime.Now;
                            strategic_plans_model.Created_by = WKPCompanyId;
                            await _context.STRATEGIC_PLANS_ON_COMPANY_BAses.AddAsync(strategic_plans_model);
                        }
                        else
                        {
                            strategic_plans_model.Date_Created = getData.Date_Created;
                            strategic_plans_model.Created_by = getData.Created_by;
                            strategic_plans_model.Date_Updated = DateTime.Now;
                            strategic_plans_model.Updated_by = WKPCompanyId;
                            await _context.STRATEGIC_PLANS_ON_COMPANY_BAses.AddAsync(strategic_plans_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.STRATEGIC_PLANS_ON_COMPANY_BAses.Remove(strategic_plans_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.STRATEGIC_PLANS_ON_COMPANY_BAses where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_QUESTION")]
        public async Task<WebApiResponse> GET_HSE_QUESTION(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_QUESTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_QUESTION")]
        public async Task<WebApiResponse> POST_HSE_QUESTION([FromBody] HSE_QUESTION hse_question_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_QUESTIONs data
                if (hse_question_model != null)
                {
                    var getData = (from c in _context.HSE_QUESTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_question_model.Companyemail = WKPCompanyEmail;
                    hse_question_model.CompanyName = WKPCompanyName;
                    hse_question_model.COMPANY_ID = WKPCompanyId;
                    hse_question_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_question_model.Date_Updated = DateTime.Now;
                    hse_question_model.Updated_by = WKPCompanyId;
                    hse_question_model.Year_of_WP = year;
                    hse_question_model.OML_Name = hse_question_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_question_model.Date_Created = DateTime.Now;
                            hse_question_model.Created_by = WKPCompanyId;
                            await _context.HSE_QUESTIONs.AddAsync(hse_question_model);
                        }
                        else
                        {
                            hse_question_model.Date_Created = getData.Date_Created;
                            hse_question_model.Created_by = getData.Created_by;
                            hse_question_model.Date_Updated = DateTime.Now;
                            hse_question_model.Updated_by = WKPCompanyId;
                            await _context.HSE_QUESTIONs.AddAsync(hse_question_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_QUESTIONs.Remove(hse_question_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_QUESTIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_FATALITY")]
        public async Task<WebApiResponse> GET_HSE_FATALITY(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_FATALITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_FATALITY")]
        public async Task<WebApiResponse> POST_HSE_FATALITY([FromBody] HSE_FATALITy hse_fatality_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_FATALITY data
                if (hse_fatality_model != null)
                {
                    var getData = (from c in _context.HSE_FATALITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_fatality_model.Companyemail = WKPCompanyEmail;
                    hse_fatality_model.CompanyName = WKPCompanyName;
                    hse_fatality_model.COMPANY_ID = WKPCompanyId;
                    hse_fatality_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_fatality_model.Date_Updated = DateTime.Now;
                    hse_fatality_model.Updated_by = WKPCompanyId;
                    hse_fatality_model.Year_of_WP = year;
                    hse_fatality_model.OML_Name = hse_fatality_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_fatality_model.Date_Created = DateTime.Now;
                            hse_fatality_model.Created_by = WKPCompanyId;
                            await _context.HSE_FATALITIEs.AddAsync(hse_fatality_model);
                        }
                        else
                        {
                            hse_fatality_model.Date_Created = getData.Date_Created;
                            hse_fatality_model.Created_by = getData.Created_by;
                            hse_fatality_model.Date_Updated = DateTime.Now;
                            hse_fatality_model.Updated_by = WKPCompanyId;
                            await _context.HSE_FATALITIEs.AddAsync(hse_fatality_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_FATALITIEs.Remove(hse_fatality_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_FATALITIEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_DESIGNS_SAFETY")]
        public async Task<WebApiResponse> GET_HSE_DESIGNS_SAFETY(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_DESIGNS_SAFETies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_DESIGNS_SAFETY")]
        public async Task<WebApiResponse> POST_HSE_DESIGNS_SAFETY([FromBody] HSE_DESIGNS_SAFETY hse_designs_safety_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_DESIGNS_SAFETYs data
                if (hse_designs_safety_model != null)
                {
                    var getData = (from c in _context.HSE_DESIGNS_SAFETies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_designs_safety_model.Companyemail = WKPCompanyEmail;
                    hse_designs_safety_model.CompanyName = WKPCompanyName;
                    hse_designs_safety_model.COMPANY_ID = WKPCompanyId;
                    hse_designs_safety_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_designs_safety_model.Date_Updated = DateTime.Now;
                    hse_designs_safety_model.Updated_by = WKPCompanyId;
                    hse_designs_safety_model.Year_of_WP = year;
                    hse_designs_safety_model.OML_Name = hse_designs_safety_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_designs_safety_model.Date_Created = DateTime.Now;
                            hse_designs_safety_model.Created_by = WKPCompanyId;
                            await _context.HSE_DESIGNS_SAFETies.AddAsync(hse_designs_safety_model);
                        }
                        else
                        {
                            hse_designs_safety_model.Date_Created = getData.Date_Created;
                            hse_designs_safety_model.Created_by = getData.Created_by;
                            hse_designs_safety_model.Date_Updated = DateTime.Now;
                            hse_designs_safety_model.Updated_by = WKPCompanyId;
                            await _context.HSE_DESIGNS_SAFETies.AddAsync(hse_designs_safety_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_DESIGNS_SAFETies.Remove(hse_designs_safety_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_DESIGNS_SAFETies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_INSPECTION_AND_MAINTENANCE_NEW")]
        public async Task<WebApiResponse> GET_HSE_INSPECTION_AND_MAINTENANCE_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_INSPECTION_AND_MAINTENANCE_NEW")]
        public async Task<WebApiResponse> POST_HSE_INSPECTION_AND_MAINTENANCE_NEW([FromBody] HSE_INSPECTION_AND_MAINTENANCE_NEW hse_IM_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_INSPECTION_AND_MAINTENANCE_NEWs data
                if (hse_IM_model != null)
                {
                    var getData = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_IM_model.Companyemail = WKPCompanyEmail;
                    hse_IM_model.CompanyName = WKPCompanyName;
                    hse_IM_model.COMPANY_ID = WKPCompanyId;
                    hse_IM_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_IM_model.Date_Updated = DateTime.Now;
                    hse_IM_model.Updated_by = WKPCompanyId;
                    hse_IM_model.Year_of_WP = year;
                    hse_IM_model.OML_Name = hse_IM_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_IM_model.Date_Created = DateTime.Now;
                            hse_IM_model.Created_by = WKPCompanyId;
                            await _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs.AddAsync(hse_IM_model);
                        }
                        else
                        {
                            hse_IM_model.Date_Created = getData.Date_Created;
                            hse_IM_model.Created_by = getData.Created_by;
                            hse_IM_model.Date_Updated = DateTime.Now;
                            hse_IM_model.Updated_by = WKPCompanyId;
                            await _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs.AddAsync(hse_IM_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs.Remove(hse_IM_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW")]
        public async Task<WebApiResponse> GET_HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW")]
        public async Task<WebApiResponse> POST_HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW([FromBody] HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW hse_IM_facility_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs data
                if (hse_IM_facility_model != null)
                {
                    var getData = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_IM_facility_model.Companyemail = WKPCompanyEmail;
                    hse_IM_facility_model.CompanyName = WKPCompanyName;
                    hse_IM_facility_model.COMPANY_ID = WKPCompanyId;
                    hse_IM_facility_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_IM_facility_model.Date_Updated = DateTime.Now;
                    hse_IM_facility_model.Updated_by = WKPCompanyId;
                    hse_IM_facility_model.Year_of_WP = year;
                    hse_IM_facility_model.OML_Name = hse_IM_facility_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_IM_facility_model.Date_Created = DateTime.Now;
                            hse_IM_facility_model.Created_by = WKPCompanyId;
                            await _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.AddAsync(hse_IM_facility_model);
                        }
                        else
                        {
                            hse_IM_facility_model.Date_Created = getData.Date_Created;
                            hse_IM_facility_model.Created_by = getData.Created_by;
                            hse_IM_facility_model.Date_Updated = DateTime.Now;
                            hse_IM_facility_model.Updated_by = WKPCompanyId;
                            await _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.AddAsync(hse_IM_facility_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.Remove(hse_IM_facility_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW")]
        public async Task<WebApiResponse> GET_HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW")]
        public async Task<WebApiResponse> POST_HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW([FromBody] HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW hse_technical_safety_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs data
                if (hse_technical_safety_model != null)
                {
                    var getData = (from c in _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_technical_safety_model.Companyemail = WKPCompanyEmail;
                    hse_technical_safety_model.CompanyName = WKPCompanyName;
                    hse_technical_safety_model.COMPANY_ID = WKPCompanyId;
                    hse_technical_safety_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_technical_safety_model.Date_Updated = DateTime.Now;
                    hse_technical_safety_model.Updated_by = WKPCompanyId;
                    hse_technical_safety_model.Year_of_WP = year;
                    hse_technical_safety_model.OML_Name = hse_technical_safety_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_technical_safety_model.Date_Created = DateTime.Now;
                            hse_technical_safety_model.Created_by = WKPCompanyId;
                            await _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.AddAsync(hse_technical_safety_model);
                        }
                        else
                        {
                            hse_technical_safety_model.Date_Created = getData.Date_Created;
                            hse_technical_safety_model.Created_by = getData.Created_by;
                            hse_technical_safety_model.Date_Updated = DateTime.Now;
                            hse_technical_safety_model.Updated_by = WKPCompanyId;
                            await _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.AddAsync(hse_technical_safety_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.Remove(hse_technical_safety_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW")]
        public async Task<WebApiResponse> GET_HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW")]
        public async Task<WebApiResponse> POST_HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW([FromBody] HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW hse_asset_register_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs data
                if (hse_asset_register_model != null)
                {
                    var getData = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_asset_register_model.Companyemail = WKPCompanyEmail;
                    hse_asset_register_model.CompanyName = WKPCompanyName;
                    hse_asset_register_model.COMPANY_ID = WKPCompanyId;
                    hse_asset_register_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_asset_register_model.Date_Updated = DateTime.Now;
                    hse_asset_register_model.Updated_by = WKPCompanyId;
                    hse_asset_register_model.Year_of_WP = year;
                    hse_asset_register_model.OML_Name = hse_asset_register_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_asset_register_model.Date_Created = DateTime.Now;
                            hse_asset_register_model.Created_by = WKPCompanyId;
                            await _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs.AddAsync(hse_asset_register_model);
                        }
                        else
                        {
                            hse_asset_register_model.Date_Created = getData.Date_Created;
                            hse_asset_register_model.Created_by = getData.Created_by;
                            hse_asset_register_model.Date_Updated = DateTime.Now;
                            hse_asset_register_model.Updated_by = WKPCompanyId;
                            await _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs.AddAsync(hse_asset_register_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs.Remove(hse_asset_register_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_OIL_SPILL_REPORTING_NEW")]
        public async Task<WebApiResponse> GET_HSE_OIL_SPILL_REPORTING_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_OIL_SPILL_REPORTING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_OIL_SPILL_REPORTING_NEW")]
        public async Task<WebApiResponse> POST_HSE_OIL_SPILL_REPORTING_NEW([FromBody] HSE_OIL_SPILL_REPORTING_NEW hse_oil_spill_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_OIL_SPILL_REPORTING_NEWs data
                if (hse_oil_spill_model != null)
                {
                    var getData = (from c in _context.HSE_OIL_SPILL_REPORTING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_oil_spill_model.Companyemail = WKPCompanyEmail;
                    hse_oil_spill_model.CompanyName = WKPCompanyName;
                    hse_oil_spill_model.COMPANY_ID = WKPCompanyId;
                    hse_oil_spill_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_oil_spill_model.Date_Updated = DateTime.Now;
                    hse_oil_spill_model.Updated_by = WKPCompanyId;
                    hse_oil_spill_model.Year_of_WP = year;
                    hse_oil_spill_model.OML_Name = hse_oil_spill_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_oil_spill_model.Date_Created = DateTime.Now;
                            hse_oil_spill_model.Created_by = WKPCompanyId;
                            await _context.HSE_OIL_SPILL_REPORTING_NEWs.AddAsync(hse_oil_spill_model);
                        }
                        else
                        {
                            hse_oil_spill_model.Date_Created = getData.Date_Created;
                            hse_oil_spill_model.Created_by = getData.Created_by;
                            hse_oil_spill_model.Date_Updated = DateTime.Now;
                            hse_oil_spill_model.Updated_by = WKPCompanyId;
                            await _context.HSE_OIL_SPILL_REPORTING_NEWs.AddAsync(hse_oil_spill_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_OIL_SPILL_REPORTING_NEWs.Remove(hse_oil_spill_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_OIL_SPILL_REPORTING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW")]
        public async Task<WebApiResponse> GET_HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW")]
        public async Task<WebApiResponse> POST_HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW([FromBody] HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW hse_asset_register_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs data
                if (hse_asset_register_model != null)
                {
                    var getData = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_asset_register_model.Companyemail = WKPCompanyEmail;
                    hse_asset_register_model.CompanyName = WKPCompanyName;
                    hse_asset_register_model.COMPANY_ID = WKPCompanyId;
                    hse_asset_register_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_asset_register_model.Date_Updated = DateTime.Now;
                    hse_asset_register_model.Updated_by = WKPCompanyId;
                    hse_asset_register_model.Year_of_WP = year;
                    hse_asset_register_model.OML_Name = hse_asset_register_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_asset_register_model.Date_Created = DateTime.Now;
                            hse_asset_register_model.Created_by = WKPCompanyId;
                            await _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs.AddAsync(hse_asset_register_model);
                        }
                        else
                        {
                            hse_asset_register_model.Date_Created = getData.Date_Created;
                            hse_asset_register_model.Created_by = getData.Created_by;
                            hse_asset_register_model.Date_Updated = DateTime.Now;
                            hse_asset_register_model.Updated_by = WKPCompanyId;
                            await _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs.AddAsync(hse_asset_register_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs.Remove(hse_asset_register_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_ACCIDENT_INCIDENCE_REPORTING_NEW")]
        public async Task<WebApiResponse> GET_HSE_ACCIDENT_INCIDENCE_REPORTING_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_ACCIDENT_INCIDENCE_REPORTING_NEW")]
        public async Task<WebApiResponse> POST_HSE_ACCIDENT_INCIDENCE_REPORTING_NEW([FromBody] HSE_ACCIDENT_INCIDENCE_REPORTING_NEW hse_accident_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs data
                if (hse_accident_model != null)
                {
                    var getData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_accident_model.Companyemail = WKPCompanyEmail;
                    hse_accident_model.CompanyName = WKPCompanyName;
                    hse_accident_model.COMPANY_ID = WKPCompanyId;
                    hse_accident_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_accident_model.Date_Updated = DateTime.Now;
                    hse_accident_model.Updated_by = WKPCompanyId;
                    hse_accident_model.Year_of_WP = year;
                    hse_accident_model.OML_Name = hse_accident_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_accident_model.Date_Created = DateTime.Now;
                            hse_accident_model.Created_by = WKPCompanyId;
                            await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.AddAsync(hse_accident_model);
                        }
                        else
                        {
                            hse_accident_model.Date_Created = getData.Date_Created;
                            hse_accident_model.Created_by = getData.Created_by;
                            hse_accident_model.Date_Updated = DateTime.Now;
                            hse_accident_model.Updated_by = WKPCompanyId;
                            await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.AddAsync(hse_accident_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.Remove(hse_accident_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW")]
        public async Task<WebApiResponse> GET_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW")]
        public async Task<WebApiResponse> POST_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW([FromBody] HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW hse_accident_reporting_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs data
                if (hse_accident_reporting_model != null)
                {
                    var getData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_accident_reporting_model.Companyemail = WKPCompanyEmail;
                    hse_accident_reporting_model.CompanyName = WKPCompanyName;
                    hse_accident_reporting_model.COMPANY_ID = WKPCompanyId;
                    hse_accident_reporting_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_accident_reporting_model.Date_Updated = DateTime.Now;
                    hse_accident_reporting_model.Updated_by = WKPCompanyId;
                    hse_accident_reporting_model.Year_of_WP = year;
                    hse_accident_reporting_model.OML_Name = hse_accident_reporting_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_accident_reporting_model.Date_Created = DateTime.Now;
                            hse_accident_reporting_model.Created_by = WKPCompanyId;
                            await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.AddAsync(hse_accident_reporting_model);
                        }
                        else
                        {
                            hse_accident_reporting_model.Date_Created = getData.Date_Created;
                            hse_accident_reporting_model.Created_by = getData.Created_by;
                            hse_accident_reporting_model.Date_Updated = DateTime.Now;
                            hse_accident_reporting_model.Updated_by = WKPCompanyId;
                            await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.AddAsync(hse_accident_reporting_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.Remove(hse_accident_reporting_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW")]
        public async Task<WebApiResponse> GET_HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW")]
        public async Task<WebApiResponse> POST_HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW([FromBody] HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW hse_community_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs data
                if (hse_community_model != null)
                {
                    var getData = (from c in _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_community_model.Companyemail = WKPCompanyEmail;
                    hse_community_model.CompanyName = WKPCompanyName;
                    hse_community_model.COMPANY_ID = WKPCompanyId;
                    hse_community_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_community_model.Date_Updated = DateTime.Now;
                    hse_community_model.Updated_by = WKPCompanyId;
                    hse_community_model.Year_of_WP = year;
                    hse_community_model.OML_Name = hse_community_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_community_model.Date_Created = DateTime.Now;
                            hse_community_model.Created_by = WKPCompanyId;
                            await _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs.AddAsync(hse_community_model);
                        }
                        else
                        {
                            hse_community_model.Date_Created = getData.Date_Created;
                            hse_community_model.Created_by = getData.Created_by;
                            hse_community_model.Date_Updated = DateTime.Now;
                            hse_community_model.Updated_by = WKPCompanyId;
                            await _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs.AddAsync(hse_community_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs.Remove(hse_community_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_ENVIRONMENTAL_STUDIES_NEW")]
        public async Task<WebApiResponse> GET_HSE_ENVIRONMENTAL_STUDIES_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_ENVIRONMENTAL_STUDIES_NEW")]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_STUDIES_NEW([FromBody] HSE_ENVIRONMENTAL_STUDIES_NEW hse_environmental_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_ENVIRONMENTAL_STUDIES_NEWs data
                if (hse_environmental_model != null)
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_environmental_model.Companyemail = WKPCompanyEmail;
                    hse_environmental_model.CompanyName = WKPCompanyName;
                    hse_environmental_model.COMPANY_ID = WKPCompanyId;
                    hse_environmental_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_environmental_model.Date_Updated = DateTime.Now;
                    hse_environmental_model.Updated_by = WKPCompanyId;
                    hse_environmental_model.Year_of_WP = year;
                    hse_environmental_model.OML_Name = hse_environmental_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_environmental_model.Date_Created = DateTime.Now;
                            hse_environmental_model.Created_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_STUDIES_NEWs.AddAsync(hse_environmental_model);
                        }
                        else
                        {
                            hse_environmental_model.Date_Created = getData.Date_Created;
                            hse_environmental_model.Created_by = getData.Created_by;
                            hse_environmental_model.Date_Updated = DateTime.Now;
                            hse_environmental_model.Updated_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_STUDIES_NEWs.AddAsync(hse_environmental_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_STUDIES_NEWs.Remove(hse_environmental_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_WASTE_MANAGEMENT_NEW")]
        public async Task<WebApiResponse> GET_HSE_WASTE_MANAGEMENT_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_WASTE_MANAGEMENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_WASTE_MANAGEMENT_NEW")]
        public async Task<WebApiResponse> POST_HSE_WASTE_MANAGEMENT_NEW([FromBody] HSE_WASTE_MANAGEMENT_NEW hse_waste_management_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_WASTE_MANAGEMENT_NEWs data
                if (hse_waste_management_model != null)
                {
                    var getData = (from c in _context.HSE_WASTE_MANAGEMENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_waste_management_model.Companyemail = WKPCompanyEmail;
                    hse_waste_management_model.CompanyName = WKPCompanyName;
                    hse_waste_management_model.COMPANY_ID = WKPCompanyId;
                    hse_waste_management_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_waste_management_model.Date_Updated = DateTime.Now;
                    hse_waste_management_model.Updated_by = WKPCompanyId;
                    hse_waste_management_model.Year_of_WP = year;
                    hse_waste_management_model.OML_Name = hse_waste_management_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_waste_management_model.Date_Created = DateTime.Now;
                            hse_waste_management_model.Created_by = WKPCompanyId;
                            await _context.HSE_WASTE_MANAGEMENT_NEWs.AddAsync(hse_waste_management_model);
                        }
                        else
                        {
                            hse_waste_management_model.Date_Created = getData.Date_Created;
                            hse_waste_management_model.Created_by = getData.Created_by;
                            hse_waste_management_model.Date_Updated = DateTime.Now;
                            hse_waste_management_model.Updated_by = WKPCompanyId;
                            await _context.HSE_WASTE_MANAGEMENT_NEWs.AddAsync(hse_waste_management_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_WASTE_MANAGEMENT_NEWs.Remove(hse_waste_management_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_WASTE_MANAGEMENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW")]
        public async Task<WebApiResponse> GET_HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW")]
        public async Task<WebApiResponse> POST_HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW([FromBody] HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW hse_waste_management_facility_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs data
                if (hse_waste_management_facility_model != null)
                {
                    var getData = (from c in _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_waste_management_facility_model.Companyemail = WKPCompanyEmail;
                    hse_waste_management_facility_model.CompanyName = WKPCompanyName;
                    hse_waste_management_facility_model.COMPANY_ID = WKPCompanyId;
                    hse_waste_management_facility_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_waste_management_facility_model.Date_Updated = DateTime.Now;
                    hse_waste_management_facility_model.Updated_by = WKPCompanyId;
                    hse_waste_management_facility_model.Year_of_WP = year;
                    hse_waste_management_facility_model.OML_Name = hse_waste_management_facility_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_waste_management_facility_model.Date_Created = DateTime.Now;
                            hse_waste_management_facility_model.Created_by = WKPCompanyId;
                            await _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs.AddAsync(hse_waste_management_facility_model);
                        }
                        else
                        {
                            hse_waste_management_facility_model.Date_Created = getData.Date_Created;
                            hse_waste_management_facility_model.Created_by = getData.Created_by;
                            hse_waste_management_facility_model.Date_Updated = DateTime.Now;
                            hse_waste_management_facility_model.Updated_by = WKPCompanyId;
                            await _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs.AddAsync(hse_waste_management_facility_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs.Remove(hse_waste_management_facility_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_PRODUCED_WATER_MANAGEMENT_NEW")]
        public async Task<WebApiResponse> GET_HSE_PRODUCED_WATER_MANAGEMENT_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_PRODUCED_WATER_MANAGEMENT_NEW")]
        public async Task<WebApiResponse> POST_HSE_PRODUCED_WATER_MANAGEMENT_NEW([FromBody] HSE_PRODUCED_WATER_MANAGEMENT_NEW hse_produced_water_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_PRODUCED_WATER_MANAGEMENT_NEWs data
                if (hse_produced_water_model != null)
                {
                    var getData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_produced_water_model.Companyemail = WKPCompanyEmail;
                    hse_produced_water_model.CompanyName = WKPCompanyName;
                    hse_produced_water_model.COMPANY_ID = WKPCompanyId;
                    hse_produced_water_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_produced_water_model.Date_Updated = DateTime.Now;
                    hse_produced_water_model.Updated_by = WKPCompanyId;
                    hse_produced_water_model.Year_of_WP = year;
                    hse_produced_water_model.OML_Name = hse_produced_water_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_produced_water_model.Date_Created = DateTime.Now;
                            hse_produced_water_model.Created_by = WKPCompanyId;
                            await _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs.AddAsync(hse_produced_water_model);
                        }
                        else
                        {
                            hse_produced_water_model.Date_Created = getData.Date_Created;
                            hse_produced_water_model.Created_by = getData.Created_by;
                            hse_produced_water_model.Date_Updated = DateTime.Now;
                            hse_produced_water_model.Updated_by = WKPCompanyId;
                            await _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs.AddAsync(hse_produced_water_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs.Remove(hse_produced_water_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW")]
        public async Task<WebApiResponse> GET_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW")]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW([FromBody] HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW hse_compliance_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs data
                if (hse_compliance_model != null)
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_compliance_model.Companyemail = WKPCompanyEmail;
                    hse_compliance_model.CompanyName = WKPCompanyName;
                    hse_compliance_model.COMPANY_ID = WKPCompanyId;
                    hse_compliance_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_compliance_model.Date_Updated = DateTime.Now;
                    hse_compliance_model.Updated_by = WKPCompanyId;
                    hse_compliance_model.Year_of_WP = year;
                    hse_compliance_model.OML_Name = hse_compliance_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_compliance_model.Date_Created = DateTime.Now;
                            hse_compliance_model.Created_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs.AddAsync(hse_compliance_model);
                        }
                        else
                        {
                            hse_compliance_model.Date_Created = getData.Date_Created;
                            hse_compliance_model.Created_by = getData.Created_by;
                            hse_compliance_model.Date_Updated = DateTime.Now;
                            hse_compliance_model.Updated_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs.AddAsync(hse_compliance_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs.Remove(hse_compliance_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW")]
        public async Task<WebApiResponse> GET_HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW")]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW([FromBody] HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW hse_environmental_studies_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs data
                if (hse_environmental_studies_model != null)
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_environmental_studies_model.Companyemail = WKPCompanyEmail;
                    hse_environmental_studies_model.CompanyName = WKPCompanyName;
                    hse_environmental_studies_model.COMPANY_ID = WKPCompanyId;
                    hse_environmental_studies_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_environmental_studies_model.Date_Updated = DateTime.Now;
                    hse_environmental_studies_model.Updated_by = WKPCompanyId;
                    hse_environmental_studies_model.Year_of_WP = year;
                    hse_environmental_studies_model.OML_Name = hse_environmental_studies_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_environmental_studies_model.Date_Created = DateTime.Now;
                            hse_environmental_studies_model.Created_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs.AddAsync(hse_environmental_studies_model);
                        }
                        else
                        {
                            hse_environmental_studies_model.Date_Created = getData.Date_Created;
                            hse_environmental_studies_model.Created_by = getData.Created_by;
                            hse_environmental_studies_model.Date_Updated = DateTime.Now;
                            hse_environmental_studies_model.Updated_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs.AddAsync(hse_environmental_studies_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs.Remove(hse_environmental_studies_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL")]
        public async Task<WebApiResponse> GET_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL")]
        public async Task<WebApiResponse> POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL([FromBody] HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL hse_sustainable_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs data
                if (hse_sustainable_model != null)
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_sustainable_model.Companyemail = WKPCompanyEmail;
                    hse_sustainable_model.CompanyName = WKPCompanyName;
                    hse_sustainable_model.COMPANY_ID = WKPCompanyId;
                    hse_sustainable_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_sustainable_model.Date_Updated = DateTime.Now;
                    hse_sustainable_model.Updated_by = WKPCompanyId;
                    hse_sustainable_model.Year_of_WP = year;
                    hse_sustainable_model.OML_Name = hse_sustainable_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_sustainable_model.Date_Created = DateTime.Now;
                            hse_sustainable_model.Created_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs.AddAsync(hse_sustainable_model);
                        }
                        else
                        {
                            hse_sustainable_model.Date_Created = getData.Date_Created;
                            hse_sustainable_model.Created_by = getData.Created_by;
                            hse_sustainable_model.Date_Updated = DateTime.Now;
                            hse_sustainable_model.Updated_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs.AddAsync(hse_sustainable_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs.Remove(hse_sustainable_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED")]
        public async Task<WebApiResponse> GET_HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED")]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED([FromBody] HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED hse_environmental_studies_new_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs data
                if (hse_environmental_studies_new_model != null)
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_environmental_studies_new_model.Companyemail = WKPCompanyEmail;
                    hse_environmental_studies_new_model.CompanyName = WKPCompanyName;
                    hse_environmental_studies_new_model.COMPANY_ID = WKPCompanyId;
                    hse_environmental_studies_new_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_environmental_studies_new_model.Date_Updated = DateTime.Now;
                    hse_environmental_studies_new_model.Updated_by = WKPCompanyId;
                    hse_environmental_studies_new_model.Year_of_WP = year;
                    hse_environmental_studies_new_model.OML_Name = hse_environmental_studies_new_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_environmental_studies_new_model.Date_Created = DateTime.Now;
                            hse_environmental_studies_new_model.Created_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs.AddAsync(hse_environmental_studies_new_model);
                        }
                        else
                        {
                            hse_environmental_studies_new_model.Date_Created = getData.Date_Created;
                            hse_environmental_studies_new_model.Created_by = getData.Created_by;
                            hse_environmental_studies_new_model.Date_Updated = DateTime.Now;
                            hse_environmental_studies_new_model.Updated_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs.AddAsync(hse_environmental_studies_new_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs.Remove(hse_environmental_studies_new_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_OSP_REGISTRATIONS_NEW")]
        public async Task<WebApiResponse> GET_HSE_OSP_REGISTRATIONS_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_OSP_REGISTRATIONS_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_OSP_REGISTRATIONS_NEW")]
        public async Task<WebApiResponse> POST_HSE_OSP_REGISTRATIONS_NEW([FromBody] HSE_OSP_REGISTRATIONS_NEW hse_osp_registrations_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_OSP_REGISTRATIONS_NEWs data
                if (hse_osp_registrations_model != null)
                {
                    var getData = (from c in _context.HSE_OSP_REGISTRATIONS_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_osp_registrations_model.Companyemail = WKPCompanyEmail;
                    hse_osp_registrations_model.CompanyName = WKPCompanyName;
                    hse_osp_registrations_model.COMPANY_ID = WKPCompanyId;
                    hse_osp_registrations_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_osp_registrations_model.Date_Updated = DateTime.Now;
                    hse_osp_registrations_model.Updated_by = WKPCompanyId;
                    hse_osp_registrations_model.Year_of_WP = year;
                    hse_osp_registrations_model.OML_Name = hse_osp_registrations_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_osp_registrations_model.Date_Created = DateTime.Now;
                            hse_osp_registrations_model.Created_by = WKPCompanyId;
                            await _context.HSE_OSP_REGISTRATIONS_NEWs.AddAsync(hse_osp_registrations_model);
                        }
                        else
                        {
                            hse_osp_registrations_model.Date_Created = getData.Date_Created;
                            hse_osp_registrations_model.Created_by = getData.Created_by;
                            hse_osp_registrations_model.Date_Updated = DateTime.Now;
                            hse_osp_registrations_model.Updated_by = WKPCompanyId;
                            await _context.HSE_OSP_REGISTRATIONS_NEWs.AddAsync(hse_osp_registrations_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_OSP_REGISTRATIONS_NEWs.Remove(hse_osp_registrations_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_OSP_REGISTRATIONS_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED")]
        public async Task<WebApiResponse> GET_HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED")]
        public async Task<WebApiResponse> POST_HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED([FromBody] HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED hse_produced_water_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs data
                if (hse_produced_water_model != null)
                {
                    var getData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_produced_water_model.Companyemail = WKPCompanyEmail;
                    hse_produced_water_model.CompanyName = WKPCompanyName;
                    hse_produced_water_model.COMPANY_ID = WKPCompanyId;
                    hse_produced_water_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_produced_water_model.Date_Updated = DateTime.Now;
                    hse_produced_water_model.Updated_by = WKPCompanyId;
                    hse_produced_water_model.Year_of_WP = year;
                    hse_produced_water_model.OML_Name = hse_produced_water_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_produced_water_model.Date_Created = DateTime.Now;
                            hse_produced_water_model.Created_by = WKPCompanyId;
                            await _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.AddAsync(hse_produced_water_model);
                        }
                        else
                        {
                            hse_produced_water_model.Date_Created = getData.Date_Created;
                            hse_produced_water_model.Created_by = getData.Created_by;
                            hse_produced_water_model.Date_Updated = DateTime.Now;
                            hse_produced_water_model.Updated_by = WKPCompanyId;
                            await _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.AddAsync(hse_produced_water_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.Remove(hse_produced_water_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW")]
        public async Task<WebApiResponse> GET_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW")]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW([FromBody] HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW hse_chemical_usage_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs data
                if (hse_chemical_usage_model != null)
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_chemical_usage_model.Companyemail = WKPCompanyEmail;
                    hse_chemical_usage_model.CompanyName = WKPCompanyName;
                    hse_chemical_usage_model.COMPANY_ID = WKPCompanyId;
                    hse_chemical_usage_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_chemical_usage_model.Date_Updated = DateTime.Now;
                    hse_chemical_usage_model.Updated_by = WKPCompanyId;
                    hse_chemical_usage_model.Year_of_WP = year;
                    hse_chemical_usage_model.OML_Name = hse_chemical_usage_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_chemical_usage_model.Date_Created = DateTime.Now;
                            hse_chemical_usage_model.Created_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs.AddAsync(hse_chemical_usage_model);
                        }
                        else
                        {
                            hse_chemical_usage_model.Date_Created = getData.Date_Created;
                            hse_chemical_usage_model.Created_by = getData.Created_by;
                            hse_chemical_usage_model.Date_Updated = DateTime.Now;
                            hse_chemical_usage_model.Updated_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs.AddAsync(hse_chemical_usage_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs.Remove(hse_chemical_usage_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_CAUSES_OF_SPILL")]
        public async Task<WebApiResponse> GET_HSE_CAUSES_OF_SPILL(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_CAUSES_OF_SPILLs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_CAUSES_OF_SPILL")]
        public async Task<WebApiResponse> POST_HSE_CAUSES_OF_SPILL([FromBody] HSE_CAUSES_OF_SPILL hse_causes_of_spill_model, string omlName, string year, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_CAUSES_OF_SPILLs data
                if (hse_causes_of_spill_model != null)
                {
                    var getData = (from c in _context.HSE_CAUSES_OF_SPILLs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_causes_of_spill_model.Companyemail = WKPCompanyEmail;
                    hse_causes_of_spill_model.CompanyName = WKPCompanyName;
                    hse_causes_of_spill_model.COMPANY_ID = WKPCompanyId;
                    hse_causes_of_spill_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_causes_of_spill_model.Date_Updated = DateTime.Now;
                    hse_causes_of_spill_model.Updated_by = WKPCompanyId;
                    hse_causes_of_spill_model.Year_of_WP = year;
                    hse_causes_of_spill_model.OML_Name = hse_causes_of_spill_model.OML_Name.ToUpper();
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_causes_of_spill_model.Date_Created = DateTime.Now;
                            hse_causes_of_spill_model.Created_by = WKPCompanyId;
                            await _context.HSE_CAUSES_OF_SPILLs.AddAsync(hse_causes_of_spill_model);
                        }
                        else
                        {
                            hse_causes_of_spill_model.Date_Created = getData.Date_Created;
                            hse_causes_of_spill_model.Created_by = getData.Created_by;
                            hse_causes_of_spill_model.Date_Updated = DateTime.Now;
                            hse_causes_of_spill_model.Updated_by = WKPCompanyId;
                            await _context.HSE_CAUSES_OF_SPILLs.AddAsync(hse_causes_of_spill_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_CAUSES_OF_SPILLs.Remove(hse_causes_of_spill_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_CAUSES_OF_SPILLs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU")]
        public async Task<WebApiResponse> GET_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU")]
        public async Task<WebApiResponse> POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU([FromBody] HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU hse_MOU_model, string omlName, string year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs data
                if (hse_MOU_model != null)
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_MOU_model.Companyemail = WKPCompanyEmail;
                    hse_MOU_model.CompanyName = WKPCompanyName;
                    hse_MOU_model.COMPANY_ID = WKPCompanyId;
                    hse_MOU_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_MOU_model.Date_Updated = DateTime.Now;
                    hse_MOU_model.Updated_by = WKPCompanyId;
                    hse_MOU_model.Year_of_WP = year;
                    hse_MOU_model.OML_Name = hse_MOU_model.OML_Name.ToUpper();
                    #region file section
                    UploadedDocument MOUUploadFile_File = null;

                    if (files[0] != null)
                    {
                        string docName = "MOUU";
                        MOUUploadFile_File = _helpersController.UploadDocument(files[0], "MOUDocuments");
                        if (MOUUploadFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion

                    hse_MOU_model.MOUUploadFilePath = files[0] != null ? MOUUploadFile_File.filePath : null; ;
                    hse_MOU_model.MOUUploadFilename = files[0] != null ? MOUUploadFile_File.fileName : null; ;

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_MOU_model.Date_Created = DateTime.Now;
                            hse_MOU_model.Created_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs.AddAsync(hse_MOU_model);
                        }
                        else
                        {
                            hse_MOU_model.Date_Created = getData.Date_Created;
                            hse_MOU_model.Created_by = getData.Created_by;
                            hse_MOU_model.Date_Updated = DateTime.Now;
                            hse_MOU_model.Updated_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs.AddAsync(hse_MOU_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs.Remove(hse_MOU_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME")]
        public async Task<WebApiResponse> GET_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME")]
        public async Task<WebApiResponse> POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME([FromBody] HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME hse_scholarship_model, string omlName, string year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs data
                if (hse_scholarship_model != null)
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_scholarship_model.Companyemail = WKPCompanyEmail;
                    hse_scholarship_model.CompanyName = WKPCompanyName;
                    hse_scholarship_model.COMPANY_ID = WKPCompanyId;
                    hse_scholarship_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_scholarship_model.Date_Updated = DateTime.Now;
                    hse_scholarship_model.Updated_by = WKPCompanyId;
                    hse_scholarship_model.Year_of_WP = year;
                    hse_scholarship_model.OML_Name = hse_scholarship_model.OML_Name.ToUpper();
                    #region file section
                    UploadedDocument SSUploadFile_File = null;

                    if (files[0] != null)
                    {
                        string docName = "SS";
                        SSUploadFile_File = _helpersController.UploadDocument(files[0], "SSDocuments");
                        if (SSUploadFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion
                    hse_scholarship_model.SSUploadFilePath = files[0] != null ? SSUploadFile_File.filePath : null;
                    hse_scholarship_model.SSUploadFilename = files[0] != null ? SSUploadFile_File.fileName : null;

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_scholarship_model.Date_Created = DateTime.Now;
                            hse_scholarship_model.Created_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs.AddAsync(hse_scholarship_model);
                        }
                        else
                        {
                            hse_scholarship_model.Date_Created = getData.Date_Created;
                            hse_scholarship_model.Created_by = getData.Created_by;
                            hse_scholarship_model.Date_Updated = DateTime.Now;
                            hse_scholarship_model.Updated_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs.AddAsync(hse_scholarship_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs.Remove(hse_scholarship_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_MANAGEMENT_POSITION")]
        public async Task<WebApiResponse> GET_HSE_MANAGEMENT_POSITION(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_MANAGEMENT_POSITIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_MANAGEMENT_POSITION")]
        public async Task<WebApiResponse> POST_HSE_MANAGEMENT_POSITION([FromBody] HSE_MANAGEMENT_POSITION hse_management_model, string omlName, string year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_MANAGEMENT_POSITIONs data
                if (hse_management_model != null)
                {
                    var getData = (from c in _context.HSE_MANAGEMENT_POSITIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_management_model.Companyemail = WKPCompanyEmail;
                    hse_management_model.CompanyName = WKPCompanyName;
                    hse_management_model.COMPANY_ID = WKPCompanyId;
                    hse_management_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_management_model.Date_Updated = DateTime.Now;
                    hse_management_model.Updated_by = WKPCompanyId;
                    hse_management_model.Year_of_WP = year;
                    hse_management_model.OML_Name = hse_management_model.OML_Name.ToUpper();
                    #region file section
                    UploadedDocument PromotionLetterFile_File = null;
                    UploadedDocument OrganogramFile_File = null;

                    if (files[0] != null)
                    {
                        string docName = "Promotion Letter";
                        PromotionLetterFile_File = _helpersController.UploadDocument(files[0], "HRDocuments");
                        if (PromotionLetterFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    if (files[1] != null)
                    {
                        string docName = "Organogram";
                        OrganogramFile_File = _helpersController.UploadDocument(files[1], "OGDocuments");
                        if (OrganogramFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion

                    hse_management_model.PromotionLetterFilePath = files[0] != null ? PromotionLetterFile_File.filePath : null;
                    hse_management_model.PromotionLetterFilename = files[0] != null ? PromotionLetterFile_File.fileName : null;
                    hse_management_model.OrganogramrFilePath = files[1] != null ? OrganogramFile_File.filePath : null;
                    hse_management_model.OrganogramrFilename = files[1] != null ? OrganogramFile_File.fileName : null;

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_management_model.Date_Created = DateTime.Now;
                            hse_management_model.Created_by = WKPCompanyId;
                            await _context.HSE_MANAGEMENT_POSITIONs.AddAsync(hse_management_model);
                        }
                        else
                        {
                            hse_management_model.Date_Created = getData.Date_Created;
                            hse_management_model.Created_by = getData.Created_by;
                            hse_management_model.Date_Updated = DateTime.Now;
                            hse_management_model.Updated_by = WKPCompanyId;
                            await _context.HSE_MANAGEMENT_POSITIONs.AddAsync(hse_management_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_MANAGEMENT_POSITIONs.Remove(hse_management_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_MANAGEMENT_POSITIONs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_QUALITY_CONTROL")]
        public async Task<WebApiResponse> GET_HSE_QUALITY_CONTROL(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_QUALITY_CONTROLs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_QUALITY_CONTROL")]
        public async Task<WebApiResponse> POST_HSE_QUALITY_CONTROL([FromBody] HSE_QUALITY_CONTROL hse_quality_model, string omlName, string year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_QUALITY_CONTROLs data
                if (hse_quality_model != null)
                {
                    var getData = (from c in _context.HSE_QUALITY_CONTROLs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_quality_model.Companyemail = WKPCompanyEmail;
                    hse_quality_model.CompanyName = WKPCompanyName;
                    hse_quality_model.COMPANY_ID = WKPCompanyId;
                    hse_quality_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_quality_model.Date_Updated = DateTime.Now;
                    hse_quality_model.Updated_by = WKPCompanyId;
                    hse_quality_model.Year_of_WP = year;
                    hse_quality_model.OML_Name = hse_quality_model.OML_Name.ToUpper();

                    #region file section
                    UploadedDocument QualityControlFile_File = null;

                    if (files[0] != null)
                    {
                        string docName = "Quality Control";
                        QualityControlFile_File = _helpersController.UploadDocument(files[0], "COSDocuments");
                        if (QualityControlFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }

                    #endregion

                    hse_quality_model.QualityControlFilename = files[0] != null ? QualityControlFile_File.fileName : null;
                    hse_quality_model.QualityControlFilePath = files[0] != null ? QualityControlFile_File.filePath : null;
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_quality_model.Date_Created = DateTime.Now;
                            hse_quality_model.Created_by = WKPCompanyId;
                            await _context.HSE_QUALITY_CONTROLs.AddAsync(hse_quality_model);
                        }
                        else
                        {
                            hse_quality_model.Date_Created = getData.Date_Created;
                            hse_quality_model.Created_by = getData.Created_by;
                            hse_quality_model.Date_Updated = DateTime.Now;
                            hse_quality_model.Updated_by = WKPCompanyId;
                            await _context.HSE_QUALITY_CONTROLs.AddAsync(hse_quality_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_QUALITY_CONTROLs.Remove(hse_quality_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_QUALITY_CONTROLs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_CLIMATE_CHANGE_AND_AIR_QUALITY")]
        public async Task<WebApiResponse> GET_HSE_CLIMATE_CHANGE_AND_AIR_QUALITY(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_CLIMATE_CHANGE_AND_AIR_QUALITY")]
        public async Task<WebApiResponse> POST_HSE_CLIMATE_CHANGE_AND_AIR_QUALITY([FromBody] HSE_CLIMATE_CHANGE_AND_AIR_QUALITY hse_climate_model, string omlName, string year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_CLIMATE_CHANGE_AND_AIR_QUALITYs data
                if (hse_climate_model != null)
                {
                    var getData = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_climate_model.Companyemail = WKPCompanyEmail;
                    hse_climate_model.CompanyName = WKPCompanyName;
                    hse_climate_model.COMPANY_ID = WKPCompanyId;
                    hse_climate_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_climate_model.Date_Updated = DateTime.Now;
                    hse_climate_model.Updated_by = WKPCompanyId;
                    hse_climate_model.Year_of_WP = year;
                    hse_climate_model.OML_Name = hse_climate_model.OML_Name.ToUpper();
                    #region file section
                    UploadedDocument GHGFile_File = null;

                    if (files[0] != null)
                    {
                        string docName = "GHG";
                        GHGFile_File = _helpersController.UploadDocument(files[0], "GHGDocuments");
                        if (GHGFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }

                    #endregion

                    hse_climate_model.GHGFilename = files[0] != null ? GHGFile_File.fileName : null;
                    hse_climate_model.GHGFilePath = files[0] != null ? GHGFile_File.filePath : null;
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_climate_model.Date_Created = DateTime.Now;
                            hse_climate_model.Created_by = WKPCompanyId;
                            await _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.AddAsync(hse_climate_model);
                        }
                        else
                        {
                            hse_climate_model.Date_Created = getData.Date_Created;
                            hse_climate_model.Created_by = getData.Created_by;
                            hse_climate_model.Date_Updated = DateTime.Now;
                            hse_climate_model.Updated_by = WKPCompanyId;
                            await _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.AddAsync(hse_climate_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.Remove(hse_climate_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_SAFETY_CULTURE_TRAININGs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_SAFETY_CULTURE_TRAINING")]
        public async Task<WebApiResponse> GET_HSE_SAFETY_CULTURE_TRAINING(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_SAFETY_CULTURE_TRAININGs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_SAFETY_CULTURE_TRAINING")]
        public async Task<WebApiResponse> POST_HSE_SAFETY_CULTURE_TRAINING([FromBody] HSE_SAFETY_CULTURE_TRAINING hse_safety_culture_model, string omlName, string year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_SAFETY_CULTURE_TRAININGs data
                if (hse_safety_culture_model != null)
                {
                    var getData = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_safety_culture_model.Companyemail = WKPCompanyEmail;
                    hse_safety_culture_model.CompanyName = WKPCompanyName;
                    hse_safety_culture_model.COMPANY_ID = WKPCompanyId;
                    hse_safety_culture_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_safety_culture_model.Date_Updated = DateTime.Now;
                    hse_safety_culture_model.Updated_by = WKPCompanyId;
                    hse_safety_culture_model.Year_of_WP = year;
                    hse_safety_culture_model.OML_Name = hse_safety_culture_model.OML_Name.ToUpper();
                    #region file section
                    UploadedDocument SafetyCurrentYearFile_File = null;
                    UploadedDocument SafetyLast2YearsFile_File = null;
                    if (files[0] != null)
                    {
                        string docName = "Safety Current Year File";
                        SafetyCurrentYearFile_File = _helpersController.UploadDocument(files[0], "SafetyCurrentYearDocuments");
                        if (SafetyCurrentYearFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }

                    if (files[1] != null)
                    {
                        string docName = "Safety Last Two Years";
                        SafetyLast2YearsFile_File = _helpersController.UploadDocument(files[1], "SafetyLast2YearsDocuments");
                        if (SafetyLast2YearsFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }

                    #endregion


                    hse_safety_culture_model.SafetyCurrentYearFilename = files[0] != null ? SafetyCurrentYearFile_File.fileName : null;
                    hse_safety_culture_model.SafetyCurrentYearFilePath = files[0] != null ? SafetyCurrentYearFile_File.filePath : null;
                    hse_safety_culture_model.SafetyLast2YearsFilename = files[1] != null ? SafetyLast2YearsFile_File.fileName : null;
                    hse_safety_culture_model.SafetyLast2YearsFilePath = files[1] != null ? SafetyLast2YearsFile_File.filePath : null;
                    
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_safety_culture_model.Date_Created = DateTime.Now;
                            hse_safety_culture_model.Created_by = WKPCompanyId;
                            await _context.HSE_SAFETY_CULTURE_TRAININGs.AddAsync(hse_safety_culture_model);
                        }
                        else
                        {
                            hse_safety_culture_model.Date_Created = getData.Date_Created;
                            hse_safety_culture_model.Created_by = getData.Created_by;
                            hse_safety_culture_model.Date_Updated = DateTime.Now;
                            hse_safety_culture_model.Updated_by = WKPCompanyId;
                            await _context.HSE_SAFETY_CULTURE_TRAININGs.AddAsync(hse_safety_culture_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SAFETY_CULTURE_TRAININGs.Remove(hse_safety_culture_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_SAFETY_CULTURE_TRAININGs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_OCCUPATIONAL_HEALTH_MANAGEMENT")]
        public async Task<WebApiResponse> GET_HSE_OCCUPATIONAL_HEALTH_MANAGEMENT(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_OCCUPATIONAL_HEALTH_MANAGEMENT")]
        public async Task<WebApiResponse> POST_HSE_OCCUPATIONAL_HEALTH_MANAGEMENT([FromBody] HSE_OCCUPATIONAL_HEALTH_MANAGEMENT hse_occupational_model, string omlName, string year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs data
                if (hse_occupational_model != null)
                {
                    var getData = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_occupational_model.Companyemail = WKPCompanyEmail;
                    hse_occupational_model.CompanyName = WKPCompanyName;
                    hse_occupational_model.COMPANY_ID = WKPCompanyId;
                    hse_occupational_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_occupational_model.Date_Updated = DateTime.Now;
                    hse_occupational_model.Updated_by = WKPCompanyId;
                    hse_occupational_model.Year_of_WP = year;
                    hse_occupational_model.OML_Name = hse_occupational_model.OML_Name.ToUpper();
                    #region file section
                    UploadedDocument OHMplanFile_File = null;
                    UploadedDocument OHMplanCommunicationFile_File = null;

                    if (files[0] != null)
                    {
                        string docName = "Field Discovery";
                        OHMplanFile_File = _helpersController.UploadDocument(files[0], "FieldDiscoveryDocuments");
                        if (OHMplanFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    if (files[1] != null)
                    {
                        string docName = "Field Discovery";
                        OHMplanCommunicationFile_File = _helpersController.UploadDocument(files[1], "FieldDiscoveryDocuments");
                        if (OHMplanCommunicationFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }

                    #endregion
                    hse_occupational_model.OHMplanFilename = files[0] != null ? OHMplanFile_File.fileName : null;
                    hse_occupational_model.OHMplanFilePath = files[0] != null ? OHMplanFile_File.filePath : null;
                    hse_occupational_model.OHMplanCommunicationFilename = files[1] != null ? OHMplanCommunicationFile_File.fileName : null;
                    hse_occupational_model.OHMplanCommunicationFilePath = files[1] != null ? OHMplanCommunicationFile_File.filePath : null;

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_occupational_model.Date_Created = DateTime.Now;
                            hse_occupational_model.Created_by = WKPCompanyId;
                            await _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.AddAsync(hse_occupational_model);
                        }
                        else
                        {
                            hse_occupational_model.Date_Created = getData.Date_Created;
                            hse_occupational_model.Created_by = getData.Created_by;
                            hse_occupational_model.Date_Updated = DateTime.Now;
                            hse_occupational_model.Updated_by = WKPCompanyId;
                            await _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.AddAsync(hse_occupational_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.Remove(hse_occupational_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_WASTE_MANAGEMENT_SYSTEM")]
        public async Task<WebApiResponse> GET_HSE_WASTE_MANAGEMENT_SYSTEM(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_WASTE_MANAGEMENT_SYSTEMs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_WASTE_MANAGEMENT_SYSTEM")]
        public async Task<WebApiResponse> POST_HSE_WASTE_MANAGEMENT_SYSTEM([FromBody] HSE_WASTE_MANAGEMENT_SYSTEM hse_waste_model, string omlName, string year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_WASTE_MANAGEMENT_SYSTEMs data
                if (hse_waste_model != null)
                {
                    var getData = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_waste_model.Companyemail = WKPCompanyEmail;
                    hse_waste_model.CompanyName = WKPCompanyName;
                    hse_waste_model.COMPANY_ID = WKPCompanyId;
                    hse_waste_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_waste_model.Date_Updated = DateTime.Now;
                    hse_waste_model.Updated_by = WKPCompanyId;
                    hse_waste_model.Year_of_WP = year;
                    hse_waste_model.OML_Name = hse_waste_model.OML_Name.ToUpper();
                    #region file section
                    UploadedDocument DecomCertificateFile_File = null;
                    UploadedDocument WasteManagementPlanFile_File = null;

                    if (files[0] != null)
                    {
                        string docName = "Decom Certificate";
                        DecomCertificateFile_File = _helpersController.UploadDocument(files[0], "DecomCertificateDocuments");
                        if (DecomCertificateFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    if (files[1] != null)
                    {
                        string docName = "Waste Management Plan";
                        WasteManagementPlanFile_File = _helpersController.UploadDocument(files[1], "WasteManagementPlanDocuments");
                        if (WasteManagementPlanFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion

                    hse_waste_model.DecomCertificateFilePath = files[0] != null ? DecomCertificateFile_File.filePath : null;
                    hse_waste_model.DecomCertificateFilename = files[0] != null ? DecomCertificateFile_File.fileName : null;
                    hse_waste_model.WasteManagementPlanFilePath = files[1] != null ? WasteManagementPlanFile_File.filePath : null;
                    hse_waste_model.WasteManagementPlanFilename = files[1] != null ? WasteManagementPlanFile_File.fileName : null;

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_waste_model.Date_Created = DateTime.Now;
                            hse_waste_model.Created_by = WKPCompanyId;
                            await _context.HSE_WASTE_MANAGEMENT_SYSTEMs.AddAsync(hse_waste_model);
                        }
                        else
                        {
                            hse_waste_model.Date_Created = getData.Date_Created;
                            hse_waste_model.Created_by = getData.Created_by;
                            hse_waste_model.Date_Updated = DateTime.Now;
                            hse_waste_model.Updated_by = WKPCompanyId;
                            await _context.HSE_WASTE_MANAGEMENT_SYSTEMs.AddAsync(hse_waste_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_WASTE_MANAGEMENT_SYSTEMs.Remove(hse_waste_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_WASTE_MANAGEMENT_SYSTEMs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM")]
        public async Task<WebApiResponse> GET_HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM")]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM([FromBody] HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM hse_EMS_model, string omlName, string year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs data
                if (hse_EMS_model != null)
                {
                    var getData = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_EMS_model.Companyemail = WKPCompanyEmail;
                    hse_EMS_model.CompanyName = WKPCompanyName;
                    hse_EMS_model.COMPANY_ID = WKPCompanyId;
                    hse_EMS_model.CompanyNumber = int.Parse(WKPCompanyId);
                    hse_EMS_model.Date_Updated = DateTime.Now;
                    hse_EMS_model.Updated_by = WKPCompanyId;
                    hse_EMS_model.Year_of_WP = year;
                    hse_EMS_model.OML_Name = hse_EMS_model.OML_Name.ToUpper();
                    #region file section
                    UploadedDocument EMSFile_File = null;
                    UploadedDocument AUDITFile_File = null;

                    if (files[0] != null)
                    {
                        string docName = "EMS";
                        EMSFile_File = _helpersController.UploadDocument(files[0], "EMSDocuments");
                        if (EMSFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    if (files[1] != null)
                    {
                        string docName = "Audit File";
                        AUDITFile_File = _helpersController.UploadDocument(files[1], "AUDITDocuments");
                        if (AUDITFile_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion
                    hse_EMS_model.EMSFilename = files[0] != null ? EMSFile_File.fileName : null;
                    hse_EMS_model.EMSFilePath = files[0] != null ? EMSFile_File.filePath : null;
                    hse_EMS_model.AUDITFilename = files[1] != null ? AUDITFile_File.fileName : null;
                    hse_EMS_model.AUDITFilePath = files[1] != null ? AUDITFile_File.filePath : null;

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_EMS_model.Date_Created = DateTime.Now;
                            hse_EMS_model.Created_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs.AddAsync(hse_EMS_model);
                        }
                        else
                        {
                            hse_EMS_model.Date_Created = getData.Date_Created;
                            hse_EMS_model.Created_by = getData.Created_by;
                            hse_EMS_model.Date_Updated = DateTime.Now;
                            hse_EMS_model.Updated_by = WKPCompanyId;
                            await _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs.AddAsync(hse_EMS_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs.Remove(hse_EMS_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpGet("GET_PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT")]
        public async Task<WebApiResponse> GET_PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT(string year, string omlName)
        {

            try
            {
                var getData = (from c in _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = getData, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpPost("POST_PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT")]
        public async Task<WebApiResponse> POST_PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT([FromBody] PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT picture_upload_model, string omlName, string year, List<IFormFile> files, string ActionToDo = null)
        {

            int save = 0;
            string action = ActionToDo == null ? GeneralModel.Insert : ActionToDo;

            try
            {

                #region Saving PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs data
                if (picture_upload_model != null)
                {
                    var getData = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.CompanyNumber == int.Parse(WKPCompanyId) && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    picture_upload_model.Companyemail = WKPCompanyEmail;
                    picture_upload_model.CompanyName = WKPCompanyName;
                    picture_upload_model.COMPANY_ID = WKPCompanyId;
                    picture_upload_model.CompanyNumber = int.Parse(WKPCompanyId);
                    picture_upload_model.Date_Updated = DateTime.Now;
                    picture_upload_model.Updated_by = WKPCompanyId;
                    picture_upload_model.Year_of_WP = year;
                    picture_upload_model.OML_Name = picture_upload_model.OML_Name.ToUpper();
                    #region file section
                    UploadedDocument UploadedPresentation_File = null;

                    if (files[0] != null)
                    {
                        string docName = "Uploaded Presentation";
                        UploadedPresentation_File = _helpersController.UploadDocument(files[0], "Presentations");
                        if (UploadedPresentation_File == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion
                    picture_upload_model.uploaded_presentation = files[0] != null ? UploadedPresentation_File.filePath : null;

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            picture_upload_model.Date_Created = DateTime.Now;
                            picture_upload_model.Created_by = WKPCompanyId;
                            await _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs.AddAsync(picture_upload_model);
                        }
                        else
                        {
                            picture_upload_model.Date_Created = getData.Date_Created;
                            picture_upload_model.Created_by = getData.Created_by;
                            picture_upload_model.Date_Updated = DateTime.Now;
                            picture_upload_model.Updated_by = WKPCompanyId;
                            await _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs.AddAsync(picture_upload_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs.Remove(picture_upload_model);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs where c.CompanyNumber == int.Parse(WKPCompanyId) && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {ActionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        #endregion

        [HttpGet("PRESENTATION SCHEDULES")]
        public async Task<WebApiResponse> PRESENTATION_SCHEDULES(string year)
        {
            try
            {
                var schedules = (from sch in _context.ADMIN_DATETIME_PRESENTATIONs select sch).ToList();
                var viewYears = schedules.Select(x => x.YEAR).Distinct().ToList();

                if (year != null)
                {
                    schedules = schedules.Where(x => x.YEAR == year).ToList();
                }
                var presentationSchedules = new PresentationSchedules_Model()
                {
                    presentationSchedules = schedules,
                    Years = viewYears
                };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = presentationSchedules, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }

        [HttpGet("DIVISIONAL_PRESENTATIONS")]
        public async Task<WebApiResponse> DIVISIONAL_PRESENTATIONS(string year)
        {
            try
            {
                var presentations = (from sch in _context.ADMIN_DIVISIONAL_REPS_PRESENTATIONs select sch).ToList();
                var viewYears = presentations.Select(x => x.YEAR).Distinct().ToList();

                if (year != null)
                {
                    presentations = presentations.Where(x => x.YEAR == year).ToList();
                }
                var presentationDivision = new PresentationSchedules_Model()
                {
                    Divisionpresentations = presentations,
                    Years = viewYears
                };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = presentationDivision, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + e.Message, StatusCode = ResponseCodes.Success };

            }
        }

        #region work program admin     
        [HttpGet("OPL_RECALIBRATED_SCALED")]
        public async Task<WebApiResponse> opl_recalibrated(string year)
        {
            var details = new List<WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANy>();
            try
            {
                if (WKUserRole == GeneralModel.Admin)
                {

                    details = await _context.WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.Year_of_WP == year).ToListAsync();
                }
                else
                {
                    details = await _context.WP_OPL_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPCompanyName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
                }
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

        }
        [HttpGet("OPL_AGGREGATED_SCORE")]
        public async Task<WebApiResponse> opl_aggregated_score(string year)
        {
            var presentYear = DateTime.Now.Year;

            var details = new List<WP_OPL_Aggregated_Score_ALL_COMPANy>();
            try
            {
                if (WKUserRole == GeneralModel.Admin)
                {
                    details = await _context.WP_OPL_Aggregated_Score_ALL_COMPANIEs.Where(c => c.Year_of_WP == year).ToListAsync();

                }
                else
                {
                    details = await _context.WP_OPL_Aggregated_Score_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPCompanyName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
                }

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

        }
        [HttpGet("OML_RECALIBRATED_SCALED")]
        public async Task<WebApiResponse> OML_RECALIBRATED_SCALE(string year)
        {
            var details = new List<WP_OML_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANy>();
            try
            {
                if (WKUserRole == GeneralModel.Admin)
                {
                    details = await _context.WP_OML_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.Year_of_WP == year).ToListAsync();
                }
                else
                {
                    details = await _context.WP_OML_WEIGHTED_AND_RECALIBRATED_SCORE_UNION_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPCompanyName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
                }
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Success };
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

        }
        //[HttpGet("OML_AGGREGATED_SCORE")]
        //public async Task<WebApiResponse> oml_aggregated_score(string year)
        //{
        //    var presentYear = DateTime.Now.Year;

        //    var details = new List<WP_OML_Aggregated_Score_ALL_COMPANy>();
        //    try
        //    {
        //        if (WKUserRole == GeneralModel.Admin)
        //        {
        //            details = await _context.WP_OML_Aggregated_Score_ALL_COMPANIEs.Where(c => c.Year_of_WP == year).ToListAsync();

        //        }
        //        else
        //        {
        //            details = await _context.WP_OML_Aggregated_Score_ALL_COMPANIEs.Where(c => c.CompanyName.Trim().ToUpper() == WKPCompanyName.Trim().ToUpper() && c.Year_of_WP == year).ToListAsync();
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : " + ex.Message, StatusCode = ResponseCodes.Success };
        //    }

        //    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = details, StatusCode = ResponseCodes.Success };

        //}


        [HttpGet("GET_CONCESSION_HELD")]
        public async Task<object> Get_Concession_Held(string mycompanyId, string myyear)
        {
            var con = await (from a in _context.ADMIN_CONCESSIONS_INFORMATIONs where a.Company_ID == mycompanyId && a.Year == myyear && a.DELETED_STATUS == null select a.Concession_Held).Distinct().ToListAsync();
            return con;
        }


        [HttpGet("GET_FORM_ONE_CONCESSION")]
        public async Task<object> GET_FORM_ONE_CONCESSION(string omlName, string myyear)
        {   
            var concessionInfo = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.Company_ID == WKPCompanyId && d.Concession_Held == omlName && d.Year == myyear && d.DELETED_STATUS == null select d).ToListAsync();
            //var drillEachCost = await (from d in _context.DRILLING_EACH_WELL_COSTs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            //var drillEachCostProposed = await (from d in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            //var drillOperationCategoriesWell = await (from d in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            //var geoActivitiesAcquisition = await (from d in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            //var geoActivitiesProcessing = await (from d in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            var concessionSituation = await (from d in _context.CONCESSION_SITUATIONs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year == myyear select d).ToListAsync();
            //var concessionSituation1stJanuary = await (from d in _context.CONCESSION_SITUATIONs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year == myyear orderby d.Year select d).ToListAsync();
            return new {concessionSituation = concessionSituation, concessionInfo = concessionInfo};
        }

        [HttpGet("GET_FORM_ONE_GEOPHYSICAL")]
        public async Task<object> GET_FORM_ONE_GEOPHYSICAL(string omlName, string myyear)
        {   
            //var concessionInfo = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.Company_ID == WKPCompanyId && d.Concession_Held == omlName && d.Year == myyear && d.DELETED_STATUS == null select d).ToListAsync();
            //var drillEachCost = await (from d in _context.DRILLING_EACH_WELL_COSTs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            //var drillEachCostProposed = await (from d in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            //var drillOperationCategoriesWell = await (from d in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            var geoActivitiesAcquisition = await (from d in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            var geoActivitiesProcessing = await (from d in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
            //var concessionSituation = await (from d in _context.CONCESSION_SITUATIONs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year == myyear select d).ToListAsync();
            //var concessionSituation1stJanuary = await (from d in _context.CONCESSION_SITUATIONs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year == myyear orderby d.Year select d).ToListAsync();
            return new {geoActivitiesAcquisition = geoActivitiesAcquisition, geoActivitiesProcessing = geoActivitiesProcessing};
        }


        [HttpGet("GET_WPYEAR_LIST")]
        public async Task<object> Get_WPYear_List()
        {
            return await (from a in _context.ADMIN_CONCESSIONS_INFORMATIONs select a.Year).Distinct().ToListAsync();
        }

        #endregion

    }
}