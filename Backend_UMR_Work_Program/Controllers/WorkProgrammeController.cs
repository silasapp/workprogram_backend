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
        private int? WKPCompanyNumber => Convert.ToInt32(User.FindFirstValue(ClaimTypes.PrimarySid));

        #region company concessions and fields management
        [HttpGet("GET_COMPANY_CONCESSIONS")]
        public async Task<object> GET_COMPANY_CONCESSIONS(int companyNumber)
        {
            int companyID = companyNumber > 0 ? companyNumber : (int)WKPCompanyNumber;
            var companyConcessions = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.CompanyNumber == WKPCompanyNumber && d.DELETED_STATUS != "DELETED" select d).ToListAsync();
            return new { CompanyConcessions = companyConcessions };
        }

        [HttpGet("GET_COMPANY_FIELDS")]
        public async Task<object> GET_COMPANY_FIELDS(int companyNumber)
        {
            int companyID = companyNumber > 0 ? companyNumber : int.Parse(WKPCompanyId);
            var concessionFields = await (from d in _context.COMPANY_FIELDs where d.CompanyNumber == companyID && d.DeletedStatus != true select d).ToListAsync();
            return new { ConcessionFields = concessionFields };
        }

        [HttpGet("GET_CONCESSIONS_FIELD")]
        public async Task<object> GET_CONCESSIONS_FIELD(int companyNumber)
        {
            int companyID = companyNumber > 0 ? companyNumber : (int)WKPCompanyNumber;
            var companyConcessions = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.Company_ID == WKPCompanyId && d.DELETED_STATUS == null select d).ToListAsync();
            var companyFields = await (from d in _context.COMPANY_FIELDs
                                       where d.CompanyNumber == WKPCompanyNumber && d.DeletedStatus != true
                                       select new
                                       {
                                           Field_Name = d.Field_Name,
                                           Field_ID = d.Field_ID,
                                           Concession_Name = _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(x => x.Consession_Id == d.Concession_ID && x.Company_ID == WKPCompanyId).FirstOrDefault().Concession_Held
                                       }).ToListAsync();

            return new { CompanyConcessions = companyConcessions, CompanyFields = companyFields };
        }


        [HttpPost("POST_ADMIN_CONCESSIONS_INFORMATION")]
        public async Task<WebApiResponse> POST_ADMIN_CONCESSIONS_INFORMATION([FromBody] ADMIN_CONCESSIONS_INFORMATION ADMIN_CONCESSIONS_INFORMATION_model, string id, string actionToDo)
        {
            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo;

            try
            {
                #region Saving Concession

                if (action == GeneralModel.Insert)
                {
                    var companyConcession = (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs
                                             where (d.ConcessionName == ADMIN_CONCESSIONS_INFORMATION_model.Concession_Held.TrimEnd().ToUpper() || d.Concession_Held == ADMIN_CONCESSIONS_INFORMATION_model.Concession_Held.TrimEnd().ToUpper())
                                                   && d.CompanyNumber == WKPCompanyNumber && d.DELETED_STATUS == null
                                             select d).FirstOrDefault();

                    if (companyConcession != null)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : Concession ({ADMIN_CONCESSIONS_INFORMATION_model.Concession_Held} is already existing and can not be duplicated.", StatusCode = ResponseCodes.Failure };
                    }
                    else
                    {
                        ADMIN_CONCESSIONS_INFORMATION_model.CompanyNumber = WKPCompanyNumber;
                        ADMIN_CONCESSIONS_INFORMATION_model.Date_Created = DateTime.Now;
                        ADMIN_CONCESSIONS_INFORMATION_model.Created_by = WKPCompanyEmail;
                        ADMIN_CONCESSIONS_INFORMATION_model.ConcessionName = ADMIN_CONCESSIONS_INFORMATION_model.Concession_Held.TrimEnd().ToUpper();
                        await _context.ADMIN_CONCESSIONS_INFORMATIONs.AddAsync(ADMIN_CONCESSIONS_INFORMATION_model);
                    }
                }
                else
                {
                    var companyConcession = (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs
                                             where d.Consession_Id == int.Parse(id)
                                                   && d.CompanyNumber == WKPCompanyNumber && d.DELETED_STATUS == null
                                             select d).FirstOrDefault();

                    if (action == GeneralModel.Update)
                    {

                        if (companyConcession == null)
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This concession details could not be found.", StatusCode = ResponseCodes.Failure };
                        }
                        else
                        {
                            ADMIN_CONCESSIONS_INFORMATION_model.Date_Updated = DateTime.Now;
                            ADMIN_CONCESSIONS_INFORMATION_model.Date_Created = companyConcession.Date_Created;
                            ADMIN_CONCESSIONS_INFORMATION_model.Updated_by = WKPCompanyEmail;
                            ADMIN_CONCESSIONS_INFORMATION_model.Concession_Held = ADMIN_CONCESSIONS_INFORMATION_model.Concession_Held.TrimEnd().ToUpper();
                            _context.ADMIN_CONCESSIONS_INFORMATIONs.Remove(companyConcession);
                            await _context.ADMIN_CONCESSIONS_INFORMATIONs.AddAsync(ADMIN_CONCESSIONS_INFORMATION_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.ADMIN_CONCESSIONS_INFORMATIONs.Remove(companyConcession);
                    }
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Concession has been " + action + "D successfully.";
                    var allConcessions = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs
                                                where d.CompanyNumber == WKPCompanyNumber && d.DELETED_STATUS == null
                                                select d).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = allConcessions, Message = successMsg, StatusCode = ResponseCodes.Success };
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


        [HttpGet("GET_CONCESSIONS_FIELDS")]
        public async Task<object> GET_CONCESSIONS_FIELDS(string concessionID, string companyID)
        {

            if (!string.IsNullOrEmpty(companyID) && companyID != "null")
            {
                var companyFields = await (from d in _context.COMPANY_FIELDs where d.CompanyNumber.ToString() == companyID && d.DeletedStatus != true select d).ToListAsync();
                return companyFields;
            }
            else
            {
                var concession = (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where (d.Consession_Id.ToString() == concessionID || d.Concession_Held == concessionID) && d.Company_ID == WKPCompanyId && d.DELETED_STATUS != "DELETED" select d).FirstOrDefault();


                var companyFields = await (from d in _context.COMPANY_FIELDs where d.Concession_ID == concession.Consession_Id && d.DeletedStatus != true select d).ToListAsync();
                return companyFields;
            }
        }

        //[HttpPost("POST_COMPANY_FIELD")]
        //public async Task<WebApiResponse> POST_COMPANY_FIELD([FromBody] COMPANY_FIELD company_field_model, string actionToDo = null)
        //{

        //    int save = 0;
        //    string action = actionToDo == null ? GeneralModel.Insert : actionToDo; 
        //    try
        //    {
        //        #region Saving Field

        //        if (action == GeneralModel.Insert)
        //        {
        //            var companyField = (from d in _context.COMPANY_FIELDs
        //                                where d.Field_Name == company_field_model.Field_Name.TrimEnd().ToUpper()
        //                                      && d.CompanyNumber == int.Parse(WKPCompanyId) && d.DeletedStatus != true
        //                                select d).FirstOrDefault();

        //            if (companyField != null)
        //            {
        //                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : Field ({company_field_model.Field_Name} is already existing and can not be duplicated.", StatusCode = ResponseCodes.Failure };
        //            }
        //            else
        //            {
        //                company_field_model.CompanyNumber = int.Parse(WKPCompanyId);
        //                company_field_model.Date_Created = DateTime.Now;
        //                company_field_model.Field_Name = company_field_model.Field_Name.TrimEnd().ToUpper();
        //                await _context.COMPANY_FIELDs.AddAsync(company_field_model);
        //            }
        //        }
        //        else if (action == GeneralModel.Update)
        //        {
        //            var companyField = (from d in _context.COMPANY_FIELDs
        //                                where d.Field_ID == company_field_model.Field_ID
        //                                      && d.CompanyNumber == int.Parse(WKPCompanyId) && d.DeletedStatus != true
        //                                select d).FirstOrDefault();

        //            if (companyField == null)
        //            {
        //                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This field details could not be found.", StatusCode = ResponseCodes.Failure };
        //            }
        //            else
        //            {
        //                company_field_model.Date_Updated = DateTime.Now;
        //                company_field_model.Field_Name = company_field_model.Field_Name.TrimEnd().ToUpper();
        //            }
        //        }
        //        else if (action == GeneralModel.Delete)
        //        {
        //            _context.COMPANY_FIELDs.Remove(company_field_model);
        //        }

        //        save += await _context.SaveChangesAsync();
        //        #endregion

        //        if (save > 0)
        //        {
        //            string successMsg = "Field has been " + action + "D successfully.";
        //            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, StatusCode = ResponseCodes.Success };
        //        }
        //        else
        //        {
        //            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

        //    }
        //}

        [HttpPost("POST_COMPANY_FIELD")]
        public async Task<WebApiResponse> POST_COMPANY_FIELD([FromBody] COMPANY_FIELD company_field_model, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo;
            try
            {
                #region Saving Field
                var concession = (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.Concession_Held == company_field_model.Concession_Name && d.Company_ID == WKPCompanyId && d.DELETED_STATUS == null select d).FirstOrDefault();

                if (action == GeneralModel.Insert)
                {
                    var companyField = (from d in _context.COMPANY_FIELDs
                                        where d.Field_Name == company_field_model.Field_Name.TrimEnd().ToUpper()
                                              && d.CompanyNumber == WKPCompanyNumber && d.DeletedStatus != true
                                        select d).FirstOrDefault();

                    if (companyField != null)
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : Field ({company_field_model.Field_Name} is already existing and can not be duplicated.", StatusCode = ResponseCodes.Failure };
                    }
                    else
                    {
                        company_field_model.Concession_ID = concession.Consession_Id;
                        company_field_model.CompanyNumber = WKPCompanyNumber;
                        company_field_model.Date_Created = DateTime.Now;
                        company_field_model.Field_Name = company_field_model.Field_Name.TrimEnd().ToUpper();
                        await _context.COMPANY_FIELDs.AddAsync(company_field_model);
                    }
                }
                else
                {
                    var companyField = (from d in _context.COMPANY_FIELDs
                                        where d.Field_ID.ToString() == id
                                              && d.CompanyNumber == WKPCompanyNumber && d.DeletedStatus != true
                                        select d).FirstOrDefault();

                    if (action == GeneralModel.Update)
                    {

                        if (companyField == null)
                        {
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : This field details could not be found.", StatusCode = ResponseCodes.Failure };
                        }
                        else
                        {
                            companyField.Concession_ID = concession.Consession_Id;
                            companyField.Date_Updated = DateTime.Now;
                            companyField.Field_Name = company_field_model.Field_Name.TrimEnd().ToUpper();
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.COMPANY_FIELDs.Remove(companyField);
                    }
                }
                save += await _context.SaveChangesAsync();
                #endregion

                if (save > 0)
                {
                    string successMsg = "Field has been " + action + "D successfully.";
                    var allFields = await (from d in _context.COMPANY_FIELDs
                                           where d.CompanyNumber == WKPCompanyNumber && d.DeletedStatus != true
                                           select new
                                           {
                                               Field_Name = d.Field_Name,
                                               Field_ID = d.Field_ID,
                                               Concession_Name =  _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(x => x.Consession_Id == d.Concession_ID && x.Company_ID == WKPCompanyId).FirstOrDefault().Concession_Held
                                           }).ToListAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = allFields, Message = successMsg, StatusCode = ResponseCodes.Success };
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

        #region database tables actions
        [HttpGet("GET_FORM_ONE_CONCESSION")]
        public async Task<object> GET_FORM_ONE_CONCESSION(string omlName, string fieldName,  string myyear)
        {
            var concessionInfo = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.Company_ID == WKPCompanyId && d.Concession_Held == omlName && d.Year == myyear && d.DELETED_STATUS == null select d).ToListAsync();
            var concessionSituation = await (from d in _context.CONCESSION_SITUATIONs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year == myyear select d).ToListAsync();
            return new { concessionSituation = concessionSituation, concessionInfo = concessionInfo };
        }

        [HttpGet("GET_FORM_ONE_GEOPHYSICAL")]
        public async Task<object> GET_FORM_ONE_GEOPHYSICAL(string omlName, string fieldName,  string myyear)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            if (concessionField.Consession_Type != "OPL" && int.Parse(myyear) > 2022)
            {
                var geoActivitiesAcquisition = await (from d in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where d.COMPANY_ID == WKPCompanyId && d.Field_ID == concessionField.Field_ID && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
                var geoActivitiesProcessing = await (from d in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where d.COMPANY_ID == WKPCompanyId && d.Field_ID == concessionField.Field_ID && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
                return new { geoActivitiesAcquisition = geoActivitiesAcquisition, geoActivitiesProcessing = geoActivitiesProcessing };
            }
            else 
            {
                var geoActivitiesAcquisition = await (from d in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
                var geoActivitiesProcessing = await (from d in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
                return new { geoActivitiesAcquisition = geoActivitiesAcquisition, geoActivitiesProcessing = geoActivitiesProcessing };
            }
        }
        [HttpGet("GET_FORM_ONE_DRILLING")]
        public async Task<object> GET_FORM_ONE_DRILLING(string omlName, string fieldName,  string myyear)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            if (concessionField.Consession_Type != "OPL" && int.Parse(myyear) > 2022)
            {
                var drillEachCost = await (from d in _context.DRILLING_EACH_WELL_COSTs where d.COMPANY_ID == WKPCompanyId && d.Field_ID == concessionField.Field_ID && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
                var drillEachCostProposed = await (from d in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where d.COMPANY_ID == WKPCompanyId && d.Field_ID == concessionField.Field_ID && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
                var drillOperationCategoriesWell = await (from d in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where d.COMPANY_ID == WKPCompanyId && d.Field_ID == concessionField.Field_ID && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
                return new { drillEachCost = drillEachCost, drillEachCostProposed = drillEachCostProposed, drillOperationCategoriesWell = drillOperationCategoriesWell };
            }
            else
            {
                var drillEachCost = await (from d in _context.DRILLING_EACH_WELL_COSTs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
                var drillEachCostProposed = await (from d in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
                var drillOperationCategoriesWell = await (from d in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs where d.COMPANY_ID == WKPCompanyId && d.OML_Name == omlName && d.Year_of_WP == myyear orderby d.QUATER select d).ToListAsync();
                return new { drillEachCost = drillEachCost, drillEachCostProposed = drillEachCostProposed, drillOperationCategoriesWell = drillOperationCategoriesWell };
            }
        }

        [HttpGet("GET_CONCESSION_FIELD")]
        public ConcessionField GET_CONCESSION_FIELD(string omlName, string fieldName)
        {
            var concession = (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.Company_ID == WKPCompanyId && d.Concession_Held == omlName && d.DELETED_STATUS == null select d).FirstOrDefault();
            var field = (from d in _context.COMPANY_FIELDs where d.Field_Name == fieldName && d.DeletedStatus != true select d).FirstOrDefault();
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

        [HttpGet("GET_FORM_TWO_INITIAL_WELL_COMPLETION_JOB")]
        public async Task<object> GET_INITIAL_WELL_COMPLETION_JOB(string year, string omlName, string fieldName)
        {

            try
            {
                var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

                if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
                {
                    var InitialWellCompletion = await (from c in _context.INITIAL_WELL_COMPLETION_JOBs1 where c.COMPANY_ID == WKPCompanyId && c.Field_ID == concessionField.Field_ID && c.Year_of_WP == year select c).ToListAsync();
                    return new { InitialWellCompletion = InitialWellCompletion };
                }
                else
                {
                    var InitialWellCompletion = await (from c in _context.INITIAL_WELL_COMPLETION_JOBs1 where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new { InitialWellCompletion = InitialWellCompletion };
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpGet("GET_FORM_TWO_WORKOVERS_RECOMPLETION_JOB")]
        public async Task<object> GET_WORKOVERS_RECOMPLETION_JOB(string year, string omlName, string fieldName)
        {

            try
            {

                var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

                if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
                {
                    var WorkoverRecompletion = await (from c in _context.WORKOVERS_RECOMPLETION_JOBs1 where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Field_ID == concessionField.Field_ID && c.Year_of_WP == year select c).ToListAsync();

                    return new { WorkoverRecompletion = WorkoverRecompletion };
                }
                else
                {
                    var WorkoverRecompletion = await (from c in _context.WORKOVERS_RECOMPLETION_JOBs1 where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();

                    return new { WorkoverRecompletion = WorkoverRecompletion };

                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        [HttpGet("GET_FORM_TWO_FDP_UNITISATION")]
        public async Task<object> GET_FORM_TWO_FDP_UNITISATION(string omlName, string fieldName,  string year)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {
                var FDP = await (from c in _context.FIELD_DEVELOPMENT_PLANs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Field_ID == concessionField.Field_ID && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var FDPExcessiveReserves = await (from c in _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Field_ID == concessionField.Field_ID && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var FieldsToSubmitFDP = await (from c in _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Field_ID == concessionField.Field_ID && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var FDPFieldsAndStatus = await (from c in _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Field_ID == concessionField.Field_ID && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var Unitization = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Field_ID == concessionField.Field_ID && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                return new { FDP = FDP, FDPExcessiveReserves = FDPExcessiveReserves, FieldsToSubmitFDP = FieldsToSubmitFDP, FDPFieldsAndStatus = FDPFieldsAndStatus, Unitization = Unitization };

            }
            else
            {
                var FDP = (from c in _context.FIELD_DEVELOPMENT_PLANs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();
                var FDPExcessiveReserves = (from c in _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();
                var FieldsToSubmitFDP = (from c in _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();
                var FDPFieldsAndStatus = (from c in _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();
                var Unitization = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();
                return new { FDP = FDP, FDPExcessiveReserves = FDPExcessiveReserves, FieldsToSubmitFDP = FieldsToSubmitFDP, FDPFieldsAndStatus = FDPFieldsAndStatus, Unitization = Unitization };
            }
        }
        [HttpGet("GET_FORM_TWO_RESERVES")]
        public async Task<object> GET_FORM_TWO_RESERVES(string omlName, string fieldName,  string year)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {
                var ReservesUpdate = (from c in _context.RESERVES_UPDATES_LIFE_INDices where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year select c).FirstOrDefault();
                var StatusOfReservesPreceeding = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year && c.FLAG1 == "COMPANY_RESERVE_OF_PRECEDDING_YEAR" select c).FirstOrDefault();
                var StatusOfReservesCurrent = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year && c.FLAG1 == "COMPANY_CURRENT_RESERVE" select c).FirstOrDefault();
                var FiveYearProjection = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year select c).FirstOrDefault();
                var CompanyAnnualProduction = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year select c).FirstOrDefault();
                var ReservesAddition = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_Additions where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var ReservesDecline = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year select c).FirstOrDefault();
                var ReservesReplacementRatio = (from c in _context.RESERVES_REPLACEMENT_RATIOs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year select c).FirstOrDefault();
                var OilCondensateFiveYears = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                return new
                {
                    ReservesUpdate = ReservesUpdate,
                    StatusOfReservesPreceeding = StatusOfReservesPreceeding,
                    StatusOfReservesCurrent = StatusOfReservesCurrent,
                    FiveYearProjection = FiveYearProjection,
                    CompanyAnnualProduction = CompanyAnnualProduction,
                    ReservesAddition = ReservesAddition,
                    ReservesDecline = ReservesDecline,
                    ReservesReplacementRatio = ReservesReplacementRatio,
                    OilCondensateFiveYears = OilCondensateFiveYears
                };
            }
            else
            {
                var ReservesUpdate = await (from c in _context.RESERVES_UPDATES_LIFE_INDices where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var StatusOfReservesPreceeding = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year && c.FLAG1 == "COMPANY_RESERVE_OF_PRECEDDING_YEAR" select c).FirstOrDefaultAsync();
                var StatusOfReservesCurrent = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year && c.FLAG1 == "COMPANY_CURRENT_RESERVE" select c).FirstOrDefaultAsync();
                var FiveYearProjection = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var CompanyAnnualProduction = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var ReservesAddition = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_Additions where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var ReservesDecline = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var ReservesReplacementRatio = await (from c in _context.RESERVES_REPLACEMENT_RATIOs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var OilCondensateFiveYears = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();

                return new
                {
                    ReservesUpdate = ReservesUpdate,
                    StatusOfReservesPreceeding = StatusOfReservesPreceeding,
                    StatusOfReservesCurrent = StatusOfReservesCurrent,
                    FiveYearProjection = FiveYearProjection,
                    CompanyAnnualProduction = CompanyAnnualProduction,
                    ReservesAddition = ReservesAddition,
                    ReservesDecline = ReservesDecline,
                    ReservesReplacementRatio = ReservesReplacementRatio,
                    OilCondensateFiveYears = OilCondensateFiveYears
                };
            }
        }
        [HttpGet("GET_FORM_TWO_OIL_PRODUCTION")]
        public async Task<object> GET_FORM_TWO_OIL_PRODUCTION(string omlName, string fieldName,  string year)
        {

            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {
                var OilCondensateProduction = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                //var Unitization = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Field_ID == concessionField.Field_ID && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var OilCondensateProductionMonthly = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year select c).ToListAsync();
                var OilCondensateProductionMonthlyProposed = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year select c).ToListAsync();
                var OilCondensateFiveYears = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.OML_Name.ToUpper() == omlName && c.Year_of_WP == year select c).ToListAsync();

                return new
                {
                    OilCondensateProduction = OilCondensateProduction,
                    //Unitization = Unitization,
                    OilCondensateProductionMonthly = OilCondensateProductionMonthly,
                    OilCondensateProductionMonthlyProposed = OilCondensateProductionMonthlyProposed,
                    OilCondensateFiveYears = OilCondensateFiveYears
                };
            }
            else
            {
                var OilCondensateProduction = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                //var Unitization = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Field_ID == concessionField.Field_ID && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var OilCondensateProductionMonthly = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var OilCondensateProductionMonthlyProposed = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var OilCondensateFiveYears = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();

                return new
                {
                    OilCondensateProduction = OilCondensateProduction,
                    //Unitization = Unitization,
                    OilCondensateProductionMonthly = OilCondensateProductionMonthly,
                    OilCondensateProductionMonthlyProposed = OilCondensateProductionMonthlyProposed,
                    OilCondensateFiveYears = OilCondensateFiveYears
                };
            }
        }
        [HttpGet("GET_FORM_TWO_GAS_PRODUCTION")]
        public async Task<object> GET_FORM_TWO_GAS_PRODUCTION(string omlName, string fieldName,  string year)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {
                var GasProduction = await (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var GasProductionDomestic = await (from c in _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                return new
                {
                    GasProductionActivity = GasProduction,
                    GasProductionDomestic = GasProductionDomestic
                };
            }
            else
            {
                var GasProduction = await (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                var GasProductionDomestic = await (from c in _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();
                return new
                {
                    GasProductionActivity = GasProduction,
                    GasProductionDomestic = GasProductionDomestic
                };
            }
        }
        [HttpGet("GET_FORM_THREE_BUDGET_PERFORMANCE")]
        public async Task<object> GET_FORM_THREE_BUDGET_PERFORMANCE(string omlName, string fieldName,  string year)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {

                var BudgetActualExpenditure = await(from c in _context.BUDGET_ACTUAL_EXPENDITUREs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var BudgetPerformanceExploratory = await(from c in _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var BudgetPerformanceDevelopment = await(from c in _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var BudgetPerformanceProductionCost = await(from c in _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var BudgetPerformanceFacilityDevProjects = await(from c in _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                return new
                {
                    BudgetActualExpenditure = BudgetActualExpenditure,
                    BudgetPerformanceExploratory = BudgetPerformanceExploratory,
                    BudgetPerformanceDevelopment = BudgetPerformanceDevelopment,
                    BudgetPerformanceProductionCost = BudgetPerformanceProductionCost,
                    BudgetPerformanceFacilityDevProjects = BudgetPerformanceFacilityDevProjects
                };
            }
            else
            {
                var BudgetActualExpenditure = await(from c in _context.BUDGET_ACTUAL_EXPENDITUREs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var BudgetPerformanceExploratory = await(from c in _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var BudgetPerformanceDevelopment = await(from c in _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var BudgetPerformanceProductionCost = await(from c in _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var BudgetPerformanceFacilityDevProjects = await(from c in _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                return new
                {
                    BudgetActualExpenditure = BudgetActualExpenditure,
                    BudgetPerformanceExploratory = BudgetPerformanceExploratory,
                    BudgetPerformanceDevelopment = BudgetPerformanceDevelopment,
                    BudgetPerformanceProductionCost = BudgetPerformanceProductionCost,
                    BudgetPerformanceFacilityDevProjects = BudgetPerformanceFacilityDevProjects
                };
            }
        }

        [HttpGet("GET_FORM_THREE_BUDGET_PROPOSAL_IN_NAIRA_DOLLAR")]
        public async Task<object> GET_FORM_THREE_BUDGET_PROPOSAL_IN_NAIRA_DOLLAR(string omlName, string fieldName,  string year)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {
                var BudgetProposalComponents = await(from c in _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var BudgetCapexOpex = await(from c in _context.BUDGET_CAPEX_OPices where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                return new
                {
                    BudgetProposalComponents = BudgetProposalComponents,
                    BudgetCapexOpex = BudgetCapexOpex
                };
            }
            else
            {
                var BudgetProposalComponents = await(from c in _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var BudgetCapexOpex = await(from c in _context.BUDGET_CAPEX_OPices where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                return new
                {
                    BudgetProposalComponents = BudgetProposalComponents,
                    BudgetCapexOpex = BudgetCapexOpex
                };
            }
        }

        [HttpGet("GET_FORM_THREE_OIL_GAS_FACILITY_MAINTENANCE")]
        public async Task<object> GET_FORM_THREE_OIL_GAS_FACILITY_MAINTENANCE(string omlName, string fieldName,  string year)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {
                var OilAndGasExpenditure = await(from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var OilCondensateTechnologyAssessment = await(from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var OilAndGasProjects = await(from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var FacilitiesProjetPerformance = await(from c in _context.FACILITIES_PROJECT_PERFORMANCEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();

                return new
                {
                    OilAndGasExpenditure = OilAndGasExpenditure,
                    OilCondensateTechnologyAssessment = OilCondensateTechnologyAssessment,
                    OilAndGasProjects = OilAndGasProjects,
                    FacilitiesProjetPerformance = FacilitiesProjetPerformance,
                };
            }
            else
            {
                var OilAndGasExpenditure = await(from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var OilCondensateTechnologyAssessment = await(from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var OilAndGasProjects = await(from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var FacilitiesProjetPerformance = await(from c in _context.FACILITIES_PROJECT_PERFORMANCEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();

                return new
                {
                    OilAndGasExpenditure = OilAndGasExpenditure,
                    OilCondensateTechnologyAssessment = OilCondensateTechnologyAssessment,
                    OilAndGasProjects = OilAndGasProjects,
                    FacilitiesProjetPerformance = FacilitiesProjetPerformance,
                };
            }
        }

        [HttpGet("GET_FORM_FOUR_NIGERIA_CONTENT")]
        public async Task<object> GET_FORM_FOUR_NIGERIA_CONTENT(string omlName, string fieldName,  string year)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {
                var NigeriaContent = await(from c in _context.NIGERIA_CONTENT_Trainings where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var NigeriaContentUploadSuccession = await(from c in _context.NIGERIA_CONTENT_Upload_Succession_Plans where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var NigeriaContentQuestion = await(from c in _context.NIGERIA_CONTENT_QUESTIONs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();

                return new
                {
                    NigeriaContent = NigeriaContent,
                    NigeriaContentUploadSuccession = NigeriaContentUploadSuccession,
                    NigeriaContentQuestion = NigeriaContentQuestion
                };
            }
            else
            {
                var NigeriaContent = await(from c in _context.NIGERIA_CONTENT_Trainings where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var NigeriaContentUploadSuccession = await(from c in _context.NIGERIA_CONTENT_Upload_Succession_Plans where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var NigeriaContentQuestion = await(from c in _context.NIGERIA_CONTENT_QUESTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();

                return new
                {
                    NigeriaContent = NigeriaContent,
                    NigeriaContentUploadSuccession = NigeriaContentUploadSuccession,
                    NigeriaContentQuestion = NigeriaContentQuestion
                };
            }
        }

        [HttpGet("GET_FORM_FOUR_STRATEGIC_PLANS")]
        public async Task<object> GET_FORM_FOUR_STRATEGIC_PLANS(string omlName, string fieldName,  string year)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {
                var StrategicPlans = await(from c in _context.STRATEGIC_PLANS_ON_COMPANY_BAses where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                return new
                {
                    StrategicPlans = StrategicPlans
                };
            }
            else
            {
                var StrategicPlans = await(from c in _context.STRATEGIC_PLANS_ON_COMPANY_BAses where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                return new
                {
                    StrategicPlans = StrategicPlans
                };
            }
        }

        [HttpGet("GET_FORM_FOUR_LEGAL_PROCEEDINGS")]
        public async Task<object> GET_FORM_FOUR_LEGAL_PROCEEDINGS(string omlName, string fieldName,  string year)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {
                var LegalLitigation = await(from c in _context.LEGAL_LITIGATIONs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var LegalArbitration = await(from c in _context.LEGAL_ARBITRATIONs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                return new
                {
                    LegalLitigation = LegalLitigation,
                    LegalArbitration = LegalArbitration
                };
            }
            else
            {
                var LegalLitigation = await(from c in _context.LEGAL_LITIGATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                var LegalArbitration = await(from c in _context.LEGAL_ARBITRATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                return new
                {
                    LegalLitigation = LegalLitigation,
                    LegalArbitration = LegalArbitration
                };
            }
        }

        [HttpGet("GET_FORM_FIVE_HSE")]
        public async Task<object> GET_FORM_FIVE_HSE(string omlName, string fieldName,  string year)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {
                var HSEQuestion = (from c in _context.HSE_QUESTIONs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEFatality = (from c in _context.HSE_FATALITIEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEDesignSafety = (from c in _context.HSE_DESIGNS_SAFETies where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEInspectionMaintenance = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEInspectionMaintenanceFacility = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSETechnicalSafety = (from c in _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESafetyStudies = (from c in _context.HSE_SAFETY_STUDIES_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();

                var HSEAssetRegister = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEOilSpill = (from c in _context.HSE_OIL_SPILL_REPORTING_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEAssetRegisterRBI = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();

                var HSEAccidentIncidence = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEAccidentIncidenceType = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var accidentModel = (from a1 in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs
                                     join a2 in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs on a1.COMPANY_ID equals a2.COMPANY_ID
                                     where a1.COMPANY_ID == WKPCompanyId && a1.OML_Name == omlName && a1.Year_of_WP == year
                                     && a2.COMPANY_ID == WKPCompanyId && a2.OML_Name == omlName && a2.Year_of_WP == year
                                     select new HSE_ACCIDENT_INCIDENCE_MODEL
                                     {
                                         Was_there_any_accident_incidence = a1.Was_there_any_accident_incidence,
                                         If_YES_were_they_reported = a1.If_YES_were_they_reported,
                                         Cause = a2.Cause,
                                         Type_of_Accident_Incidence = a2.Type_of_Accident_Incidence,
                                         Consequence = a2.Consequence,
                                         Frequency = a2.Frequency,
                                         Investigation = a2.Investigation,
                                         Lesson_Learnt = a2.Lesson_Learnt,
                                         Location = a2.Location,
                                         Date_ = a2.Date_
                                     }).ToList();

                var HSECommunityDisturbance = (from c in _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEEnvironmentalStudies = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEWasteManagement = (from c in _context.HSE_WASTE_MANAGEMENT_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEWasteManagementType = (from c in _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEProducedWaterMgt = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEEnvironmentalCompliance = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();


                var HSEEnvironmentalFiveYears = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainableDev = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEEnvironmentalStudiesUpdated = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEOSPRegistrations = (from c in _context.HSE_OSP_REGISTRATIONS_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEProducedWaterMgtUpdated = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEEnvironmentalComplianceChemical = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSECausesOfSpill = (from c in _context.HSE_CAUSES_OF_SPILLs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainableDevMOU = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainableDevScheme = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEManagementPosition = (from c in _context.HSE_MANAGEMENT_POSITIONs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEQualityControl = (from c in _context.HSE_QUALITY_CONTROLs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEClimateChange = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESafetyCulture = (from c in _context.HSE_SAFETY_CULTURE_TRAININGs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEOccupationalHealth = (from c in _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEWasteManagementSystems = (from c in _context.HSE_WASTE_MANAGEMENT_SYSTEMs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSEEnvironmentalManagementSystems = (from c in _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();

                return new
                {
                    HSETechnicalSafety = HSETechnicalSafety,
                    HSESafetyStudies = HSESafetyStudies,
                    HSEQualityControl = HSEQualityControl,
                    HSEInspectionMaintenance = HSEInspectionMaintenance,
                    HSEAssetRegister = HSEAssetRegister,
                    HSEOilSpill = HSEOilSpill,
                    HSECausesOfSpill = HSECausesOfSpill,
                    HSEAssetRegisterRBI = HSEAssetRegisterRBI,
                    HSEAccidentModel = accidentModel,
                    HSEAccidentIncidence = HSEAccidentIncidence,
                    HSEOSPRegistrations = HSEOSPRegistrations,
                    HSEAccidentIncidenceType = HSEAccidentIncidenceType,
                    HSECommunityDisturbance = HSECommunityDisturbance,

                    HSEQuestion = HSEQuestion,
                    HSEFatality = HSEFatality,
                    HSEDesignSafety = HSEDesignSafety,
                    HSEInspectionMaintenanceFacility = HSEInspectionMaintenanceFacility,
                    HSEEnvironmentalStudies = HSEEnvironmentalStudies,
                    HSEWasteManagement = HSEWasteManagement,
                    HSEWasteManagementType = HSEWasteManagementType,
                    HSEProducedWaterMgt = HSEProducedWaterMgt,
                    HSEEnvironmentalCompliance = HSEEnvironmentalCompliance,
                    HSEEnvironmentalFiveYears = HSEEnvironmentalFiveYears,
                    HSEEnvironmentalStudiesUpdated = HSEEnvironmentalStudiesUpdated,
                    HSEProducedWaterMgtUpdated = HSEProducedWaterMgtUpdated,
                    HSEEnvironmentalComplianceChemical = HSEEnvironmentalComplianceChemical,
                    HSEManagementPosition = HSEManagementPosition,
                    HSEClimateChange = HSEClimateChange,
                    HSESafetyCulture = HSESafetyCulture,
                    HSEOccupationalHealth = HSEOccupationalHealth,
                    HSEWasteManagementSystems = HSEWasteManagementSystems,
                    HSEEnvironmentalManagementSystems = HSEEnvironmentalManagementSystems,
                };
            }
            else
            {
                    var HSEQuestion = (from c in _context.HSE_QUESTIONs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEFatality = (from c in _context.HSE_FATALITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEDesignSafety = (from c in _context.HSE_DESIGNS_SAFETies where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEInspectionMaintenance = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEInspectionMaintenanceFacility = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSETechnicalSafety = (from c in _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSESafetyStudies = (from c in _context.HSE_SAFETY_STUDIES_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();

                    var HSEAssetRegister = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEOilSpill = (from c in _context.HSE_OIL_SPILL_REPORTING_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEAssetRegisterRBI = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();

                    var HSEAccidentIncidence = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEAccidentIncidenceType = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var accidentModel = (from a1 in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs
                                         join a2 in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs on a1.COMPANY_ID equals a2.COMPANY_ID
                                         where a1.COMPANY_ID == WKPCompanyId && a1.OML_Name == omlName && a1.Year_of_WP == year
                                         && a2.COMPANY_ID == WKPCompanyId && a2.OML_Name == omlName && a2.Year_of_WP == year
                                         select new HSE_ACCIDENT_INCIDENCE_MODEL
                                         {
                                             Was_there_any_accident_incidence = a1.Was_there_any_accident_incidence,
                                             If_YES_were_they_reported = a1.If_YES_were_they_reported,
                                             Cause = a2.Cause,
                                             Type_of_Accident_Incidence = a2.Type_of_Accident_Incidence,
                                             Consequence = a2.Consequence,
                                             Frequency = a2.Frequency,
                                             Investigation = a2.Investigation,
                                             Lesson_Learnt = a2.Lesson_Learnt,
                                             Location = a2.Location,
                                             Date_ = a2.Date_
                                         }).ToList();

                    var HSECommunityDisturbance = (from c in _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEEnvironmentalStudies = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEWasteManagement = (from c in _context.HSE_WASTE_MANAGEMENT_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEWasteManagementType = (from c in _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEProducedWaterMgt = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEEnvironmentalCompliance = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEEnvironmentalFiveYears = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSESustainableDev = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEEnvironmentalStudiesUpdated = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEOSPRegistrations = (from c in _context.HSE_OSP_REGISTRATIONS_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEProducedWaterMgtUpdated = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEEnvironmentalComplianceChemical = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSECausesOfSpill = (from c in _context.HSE_CAUSES_OF_SPILLs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSESustainableDevMOU = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSESustainableDevScheme = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEManagementPosition = (from c in _context.HSE_MANAGEMENT_POSITIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEQualityControl = (from c in _context.HSE_QUALITY_CONTROLs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEClimateChange = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSESafetyCulture = (from c in _context.HSE_SAFETY_CULTURE_TRAININGs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEOccupationalHealth = (from c in _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEWasteManagementSystems = (from c in _context.HSE_WASTE_MANAGEMENT_SYSTEMs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                    var HSEEnvironmentalManagementSystems = (from c in _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();

                    return new
                    {
                        HSETechnicalSafety = HSETechnicalSafety,
                        HSESafetyStudies = HSESafetyStudies,
                        HSEQualityControl = HSEQualityControl,
                        HSEInspectionMaintenance = HSEInspectionMaintenance,
                        HSEAssetRegister = HSEAssetRegister,
                        HSEOilSpill = HSEOilSpill,
                        HSECausesOfSpill = HSECausesOfSpill,
                        HSEAssetRegisterRBI = HSEAssetRegisterRBI,
                        HSEAccidentModel = accidentModel,
                        HSEAccidentIncidence = HSEAccidentIncidence,
                        HSEOSPRegistrations = HSEOSPRegistrations,
                        HSEAccidentIncidenceType = HSEAccidentIncidenceType,
                        HSECommunityDisturbance = HSECommunityDisturbance,

                        HSEQuestion = HSEQuestion,
                        HSEFatality = HSEFatality,
                        HSEDesignSafety = HSEDesignSafety,
                        HSEInspectionMaintenanceFacility = HSEInspectionMaintenanceFacility,
                        HSEEnvironmentalStudies = HSEEnvironmentalStudies,
                        HSEWasteManagement = HSEWasteManagement,
                        HSEWasteManagementType = HSEWasteManagementType,
                        HSEProducedWaterMgt = HSEProducedWaterMgt,
                        HSEEnvironmentalCompliance = HSEEnvironmentalCompliance,
                        HSEEnvironmentalFiveYears = HSEEnvironmentalFiveYears,
                        HSEEnvironmentalStudiesUpdated = HSEEnvironmentalStudiesUpdated,
                        HSEProducedWaterMgtUpdated = HSEProducedWaterMgtUpdated,
                        HSEEnvironmentalComplianceChemical = HSEEnvironmentalComplianceChemical,
                        HSEManagementPosition = HSEManagementPosition,
                        HSEClimateChange = HSEClimateChange,
                        HSESafetyCulture = HSESafetyCulture,
                        HSEOccupationalHealth = HSEOccupationalHealth,
                        HSEWasteManagementSystems = HSEWasteManagementSystems,
                        HSEEnvironmentalManagementSystems = HSEEnvironmentalManagementSystems,
                    };
                }
        }

        [HttpGet("GET_ROYALTY")]
        public async Task<object> GET_ROYALTY(string omlName, string myyear)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, "");
            var royalty = await (from d in _context.Royalties where d.CompanyNumber == WKPCompanyNumber && d.Concession_ID == concessionField.Concession_ID && d.Year == myyear select d).ToListAsync();
            return new { royalty = royalty };
        }
        
        [HttpPost("POST_ROYALTY")]
        public async Task<WebApiResponse> POST_ROYALTY([FromBody] Royalty royalty_model, string year, string omlName, string id, string actionToDo)
        {
            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; 
            var concessionField = GET_CONCESSION_FIELD(omlName, "");
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.Royalties where c.Royalty_ID == int.Parse(id) select c).FirstOrDefault();
                    if (action == GeneralModel.Delete)
                        _context.Royalties.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (royalty_model != null)
                {
                    var getData = (from d in _context.Royalties where d.CompanyNumber == WKPCompanyNumber && d.Concession_ID == concessionField.Concession_ID && d.Year == year select d).FirstOrDefault();
                    royalty_model.CompanyNumber = WKPCompanyNumber;
                    royalty_model.Field_ID = concessionField.Concession_ID;
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            royalty_model.Date_Created = DateTime.Now;
                            await _context.Royalties.AddAsync(royalty_model);
                        }
                        else
                        {
                            royalty_model.Date_Created = getData.Date_Created;
                            _context.Royalties.Remove(getData);
                            await _context.Royalties.AddAsync(royalty_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.Royalties.Remove(getData);
                    }
                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.Royalties where c.Concession_ID == concessionField.Concession_ID && c.CompanyNumber == WKPCompanyNumber && c.Year == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpGet("GET_FORM_FIVE_SDCP")]
        public async Task<object> GET_FORM_FIVE_SDCP(string omlName, string fieldName,  string year)
        {
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            if (concessionField.Consession_Type != "OPL" && int.Parse(year) > 2022)
            {
                var HSESustainable_CSR = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_Question = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTIONs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_MOU = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_Capital = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_Schorlarship_CSR = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_Schorlarship = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_Training_CSR = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_TrainingDetails_CSR = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEMEs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var PictureUpload = (from c in _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs where c.Field_ID == concessionField.Field_ID && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();

                return new
                {
                    HSESustainable_CSR = HSESustainable_CSR,
                    HSESustainable_Question = HSESustainable_Question,
                    HSESustainable_Schorlarship_CSR = HSESustainable_Schorlarship_CSR,
                    HSESustainable_MOU = HSESustainable_MOU,
                    HSESustainable_Schorlarship = HSESustainable_Schorlarship,
                    HSESustainable_Training_CSR = HSESustainable_Training_CSR,
                    HSESustainable_TrainingDetails_CSR = HSESustainable_TrainingDetails_CSR,
                    PictureUpload = PictureUpload
                };
            }
            else
            {
                var HSESustainable_CSR = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_Question = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_MOU = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_Capital = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_Schorlarship_CSR = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_Schorlarship = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_Training_CSR = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var HSESustainable_TrainingDetails_CSR = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEMEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();
                var PictureUpload = (from c in _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();

                return new
                {
                    HSESustainable_CSR = HSESustainable_CSR,
                    HSESustainable_Question = HSESustainable_Question,
                    HSESustainable_Schorlarship_CSR = HSESustainable_Schorlarship_CSR,
                    HSESustainable_MOU = HSESustainable_MOU,
                    HSESustainable_Schorlarship = HSESustainable_Schorlarship,
                    HSESustainable_Training_CSR = HSESustainable_Training_CSR,
                    HSESustainable_TrainingDetails_CSR = HSESustainable_TrainingDetails_CSR,
                    PictureUpload = PictureUpload
                };
            }
        }

        [HttpPost("POST_CONCESSION_SITUATION")]
        public async Task<WebApiResponse> POST_CONCESSION_SITUATION([FromBody] CONCESSION_SITUATION concession_situation_model, string year, string omlName, string fieldName,  string actionToDo = null)
        {

            int save = 0;
            var ConcessionCONCESSION_SITUATION_Model = new CONCESSION_SITUATION();
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; 
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            try
            {
                #region Saving Concession Situations

                var concessionDbData = (from c in _context.CONCESSION_SITUATIONs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.Year == year select c).FirstOrDefault();

                ConcessionCONCESSION_SITUATION_Model.Companyemail = WKPCompanyEmail;
                ConcessionCONCESSION_SITUATION_Model.CompanyName = WKPCompanyName;
                ConcessionCONCESSION_SITUATION_Model.COMPANY_ID = WKPCompanyId;
                ConcessionCONCESSION_SITUATION_Model.Date_Updated = DateTime.Now;
                ConcessionCONCESSION_SITUATION_Model.Updated_by = WKPCompanyEmail;
                ConcessionCONCESSION_SITUATION_Model.Year = year;
                ConcessionCONCESSION_SITUATION_Model.OML_Name = omlName;
                ConcessionCONCESSION_SITUATION_Model.Field_ID = concessionField?.Field_ID;
                ConcessionCONCESSION_SITUATION_Model.Field_Name = concessionField?.Field_Name;

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

        [HttpPost("POST_GEOPHYSICAL_ACTIVITIES_ACQUISITION")]
        public async Task<WebApiResponse> POST_GEOPHYSICAL_ACTIVITIES_ACQUISITION([FromBody] GEOPHYSICAL_ACTIVITIES_ACQUISITION geophysical_activities_acquisition_model, string omlName, string fieldName, string year, string actionToDo = null)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo;
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
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
                    geophysical_activities_acquisition_model.OML_Name = omlName;
                    geophysical_activities_acquisition_model.Field_ID = concessionField?.Field_ID;
                    geophysical_activities_acquisition_model.Actual_year = year;
                    geophysical_activities_acquisition_model.proposed_year = (int.Parse(year) + 1).ToString();

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
                            _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Remove(getgeophysical_activities_acquisition_model);
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
                        var All_geophysical_activities_acquisitions = await (from c in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_geophysical_activities_acquisitions, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("POST_GEOPHYSICAL_ACTIVITIES_PROCESSING")]
        public async Task<WebApiResponse> POST_GEOPHYSICAL_ACTIVITIES_PROCESSING([FromBody] GEOPHYSICAL_ACTIVITIES_PROCESSING geophysical_activities_processing_model, string omlName, string fieldName, string year, string actionToDo = null)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            try
            {
                #region Saving Geophysical Activites
                if (geophysical_activities_processing_model != null)
                {
                    var getGeophysicalActivitesData = (from c in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.QUATER == geophysical_activities_processing_model.QUATER && c.Year_of_WP == year select c).FirstOrDefault();

                    geophysical_activities_processing_model.Companyemail = WKPCompanyEmail;
                    geophysical_activities_processing_model.CompanyName = WKPCompanyName;
                    geophysical_activities_processing_model.COMPANY_ID = WKPCompanyId;
                    geophysical_activities_processing_model.CompanyNumber = WKPCompanyNumber;
                    geophysical_activities_processing_model.Date_Updated = DateTime.Now;
                    geophysical_activities_processing_model.Updated_by = WKPCompanyId;
                    geophysical_activities_processing_model.Year_of_WP = year;
                    geophysical_activities_processing_model.OML_Name = omlName;
                    geophysical_activities_processing_model.Field_ID = concessionField.Field_ID;
                    geophysical_activities_processing_model.Actual_year = year;
                    geophysical_activities_processing_model.proposed_year = (int.Parse(year) + 1).ToString();

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
                            _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Remove(getGeophysicalActivitesData);
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
                        var All_Data = await (from c in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs where c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }
        
        [HttpPost("POST_DRILLING_OPERATIONS_CATEGORIES_OF_WELL"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_DRILLING_OPERATIONS_CATEGORIES_OF_WELL([FromForm] DRILLING_OPERATIONS_CATEGORIES_OF_WELL drilling_operations_categories_of_well_model, string omlName, string fieldName, string year, string actionToDo = null)
        {
            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; 
            var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

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
                    drilling_operations_categories_of_well_model.OML_Name = omlName;
                    drilling_operations_categories_of_well_model.Field_ID = concessionField.Field_ID;
                    drilling_operations_categories_of_well_model.Actual_year = year;
                    drilling_operations_categories_of_well_model.proposed_year = (int.Parse(year) + 1).ToString();

                    #region file section
                    var file1 = Request.Form.Files[0];
                    var file2 = Request.Form.Files[1];
                    var blobname1 = blobService.Filenamer(file1);
                    var blobname2 = blobService.Filenamer(file2);

                    if (file1 != null)
                    {
                        string docName = "Field Discovery";
                        drilling_operations_categories_of_well_model.FieldDiscoveryUploadFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"FieldDiscoveryDocuments/{blobname1}");
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
                            _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Remove(getData);
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

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_DRILLING_EACH_WELL_COST")]
        public async Task<WebApiResponse> POST_DRILLING_EACH_WELL_COST([FromBody] DRILLING_EACH_WELL_COST drilling_each_well_cost_model, string omlName, string fieldName, string year, string actionToDo = null)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            try
            {

                #region Saving drilling data
                if (drilling_each_well_cost_model != null)
                {
                    var getData = (from c in _context.DRILLING_EACH_WELL_COSTs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.QUATER == drilling_each_well_cost_model.QUATER && c.Year_of_WP == year select c).FirstOrDefault();

                    drilling_each_well_cost_model.Companyemail = WKPCompanyEmail;
                    drilling_each_well_cost_model.CompanyName = WKPCompanyName;
                    drilling_each_well_cost_model.COMPANY_ID = WKPCompanyId;
                    drilling_each_well_cost_model.CompanyNumber = WKPCompanyNumber;
                    drilling_each_well_cost_model.Date_Updated = DateTime.Now;
                    drilling_each_well_cost_model.Updated_by = WKPCompanyId;
                    drilling_each_well_cost_model.Year_of_WP = year;
                    drilling_each_well_cost_model.OML_Name = omlName;
                    drilling_each_well_cost_model.Field_ID = concessionField.Field_ID;
                    
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
                            _context.DRILLING_EACH_WELL_COSTs.Remove(getData);
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
                        var All_Data = await (from c in _context.DRILLING_EACH_WELL_COSTs where c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpPost("POST_DRILLING_EACH_WELL_COST_PROPOSED")]
        public async Task<WebApiResponse> POST_DRILLING_EACH_WELL_COST_PROPOSED([FromBody] DRILLING_EACH_WELL_COST_PROPOSED drilling_each_well_cost_proposed_model, string omlName, string fieldName, string year, string actionToDo = null)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                #region Saving drilling data
                if (drilling_each_well_cost_proposed_model != null)
                {
                    var getData = (from c in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.QUATER == drilling_each_well_cost_proposed_model.QUATER && c.Year_of_WP == year select c).FirstOrDefault();

                    drilling_each_well_cost_proposed_model.Companyemail = WKPCompanyEmail;
                    drilling_each_well_cost_proposed_model.CompanyName = WKPCompanyName;
                    drilling_each_well_cost_proposed_model.COMPANY_ID = WKPCompanyId;
                    drilling_each_well_cost_proposed_model.CompanyNumber = WKPCompanyNumber;
                    drilling_each_well_cost_proposed_model.Date_Updated = DateTime.Now;
                    drilling_each_well_cost_proposed_model.Updated_by = WKPCompanyId;
                    drilling_each_well_cost_proposed_model.Year_of_WP = year;
                    drilling_each_well_cost_proposed_model.OML_Name = omlName;
                    drilling_each_well_cost_proposed_model.Field_ID = concessionField.Field_ID;
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
                            _context.DRILLING_EACH_WELL_COST_PROPOSEDs.Remove(getData);
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
                        var All_Data = await (from c in _context.DRILLING_EACH_WELL_COST_PROPOSEDs where c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_FIELD_DEVELOPMENT_PLAN")]
        public async Task<WebApiResponse> POST_FIELD_DEVELOPMENT_PLAN([FromBody] FIELD_DEVELOPMENT_PLAN field_development_plan_model, List<IFormFile> files, string omlName, string fieldName, string year, string actionToDo = null)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving FDP data
                if (field_development_plan_model != null)
                {
                    var getData = (from c in _context.FIELD_DEVELOPMENT_PLANs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    field_development_plan_model.Companyemail = WKPCompanyEmail;
                    field_development_plan_model.CompanyName = WKPCompanyName;
                    field_development_plan_model.COMPANY_ID = WKPCompanyId;
                    field_development_plan_model.CompanyNumber = WKPCompanyNumber;
                    field_development_plan_model.Date_Updated = DateTime.Now;
                    field_development_plan_model.Updated_by = WKPCompanyId;
                    field_development_plan_model.Year_of_WP = year;
                    field_development_plan_model.OML_Name = field_development_plan_model.OML_Name.ToUpper();
                    field_development_plan_model.Field_ID = concessionField.Field_ID;
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
                        var All_Data = await (from c in _context.FIELD_DEVELOPMENT_PLANs where c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }
        [HttpPost("POST_FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE")]
        public async Task<WebApiResponse> POST_FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVE([FromBody] FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERf field_development_plan_model, string omlName, string fieldName, string year, string actionToDo = null)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving FDP data
                if (field_development_plan_model != null)
                {
                    var getData = (from c in _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    field_development_plan_model.Companyemail = WKPCompanyEmail;
                    field_development_plan_model.CompanyName = WKPCompanyName;
                    field_development_plan_model.COMPANY_ID = WKPCompanyId;
                    field_development_plan_model.CompanyNumber = WKPCompanyNumber;
                    field_development_plan_model.Date_Updated = DateTime.Now;
                    field_development_plan_model.Updated_by = WKPCompanyId;
                    field_development_plan_model.Year_of_WP = year;
                    field_development_plan_model.OML_Name = omlName;
                    field_development_plan_model.Field_ID = concessionField.Field_ID;
                    field_development_plan_model.Field_Name = concessionField.Field_Name;

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
                            _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.Remove(getData);
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
                        var All_Data = await (from c in _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs where c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
        
        [HttpPost("POST_FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP")]
        public async Task<WebApiResponse> POST_FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP([FromBody] FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDP field_development_plan_model, string omlName, string fieldName, string year, string actionToDo = null)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving FDP data
                if (field_development_plan_model != null)
                {
                    var getData = (from c in _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs where c.COMPANY_ID == WKPCompanyId && c.OML_Name == omlName && c.Year_of_WP == year select c).FirstOrDefault();

                    field_development_plan_model.Companyemail = WKPCompanyEmail;
                    field_development_plan_model.CompanyName = WKPCompanyName;
                    field_development_plan_model.COMPANY_ID = WKPCompanyId;
                    field_development_plan_model.CompanyNumber = WKPCompanyNumber;
                    field_development_plan_model.Date_Updated = DateTime.Now;
                    field_development_plan_model.Updated_by = WKPCompanyId;
                    field_development_plan_model.Year_of_WP = year;
                    field_development_plan_model.OML_Name = field_development_plan_model.OML_Name.ToUpper();
                    field_development_plan_model.Field_Name = concessionField.Field_Name;
                    field_development_plan_model.Field_ID = concessionField.Field_ID;

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
                        var All_Data = await (from c in _context.FIELD_DEVELOPMENT_FIELDS_TO_SUBMIT_FDPs where c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_FIELD_DEVELOPMENT_FIELDS_AND_STATUS")]
        public async Task<WebApiResponse> POST_FIELD_DEVELOPMENT_FIELDS_AND_STATUS([FromBody] FIELD_DEVELOPMENT_FIELDS_AND_STATUS field_development_plan_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving FDP data
                if (field_development_plan_model != null)
                {
                    var getData = (from c in _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    field_development_plan_model.Companyemail = WKPCompanyEmail;
                    field_development_plan_model.CompanyName = WKPCompanyName;
                    field_development_plan_model.COMPANY_ID = WKPCompanyId;
                    field_development_plan_model.CompanyNumber = WKPCompanyNumber;
                    field_development_plan_model.Date_Updated = DateTime.Now;
                    field_development_plan_model.Updated_by = WKPCompanyId;
                    field_development_plan_model.Year_of_WP = year;
                    field_development_plan_model.OML_Name = field_development_plan_model.OML_Name.ToUpper();
                    field_development_plan_model.Field_ID = concessionField.Field_ID;
                    field_development_plan_model.Field_Name = concessionField.Field_Name;
                    field_development_plan_model.OML_ID = concessionField.Concession_ID.ToString();

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
                            _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.Remove(getData);
                            await _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.AddAsync(field_development_plan_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_INITIAL_WELL_COMPLETION_JOB")]
        public async Task<WebApiResponse> POST_INITIAL_WELL_COMPLETION_JOB([FromBody] INITIAL_WELL_COMPLETION_JOB1 initial_well_completion_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving FDP data
                if (initial_well_completion_model != null)
                {
                    var getData = await (from c in _context.INITIAL_WELL_COMPLETION_JOBs1 where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year && c.QUATER == initial_well_completion_model.QUATER select c).FirstOrDefaultAsync();

                    initial_well_completion_model.Companyemail = WKPCompanyEmail;
                    initial_well_completion_model.CompanyName = WKPCompanyName;
                    initial_well_completion_model.COMPANY_ID = WKPCompanyId;
                    initial_well_completion_model.CompanyNumber = WKPCompanyNumber;
                    initial_well_completion_model.Date_Updated = DateTime.Now;
                    initial_well_completion_model.Updated_by = WKPCompanyId;
                    initial_well_completion_model.Year_of_WP = year;
                    initial_well_completion_model.OML_Name = omlName.ToUpper();
                    initial_well_completion_model.Field_ID = concessionField.Field_ID;
                    initial_well_completion_model.Actual_year = year;
                    initial_well_completion_model.proposed_year =(int.Parse(year) + 1).ToString();

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
                            _context.INITIAL_WELL_COMPLETION_JOBs1.Remove(getData);
                            await _context.INITIAL_WELL_COMPLETION_JOBs1.AddAsync(initial_well_completion_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.INITIAL_WELL_COMPLETION_JOBs1.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.INITIAL_WELL_COMPLETION_JOBs1 where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpPost("POST_WORKOVERS_RECOMPLETION_JOB")]
        public async Task<WebApiResponse> POST_WORKOVERS_RECOMPLETION_JOB([FromBody] WORKOVERS_RECOMPLETION_JOB1 workovers_recompletion_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving FDP data
                if (workovers_recompletion_model != null)
                {
                    var getData = await (from c in _context.WORKOVERS_RECOMPLETION_JOBs1 where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year && c.QUATER == workovers_recompletion_model.QUATER select c).FirstOrDefaultAsync();

                    workovers_recompletion_model.Companyemail = WKPCompanyEmail;
                    workovers_recompletion_model.CompanyName = WKPCompanyName;
                    workovers_recompletion_model.COMPANY_ID = WKPCompanyId;
                    workovers_recompletion_model.CompanyNumber = WKPCompanyNumber;
                    workovers_recompletion_model.Date_Updated = DateTime.Now;
                    workovers_recompletion_model.Updated_by = WKPCompanyId;
                    workovers_recompletion_model.Year_of_WP = year;
                    workovers_recompletion_model.OML_Name = omlName.ToUpper();
                    workovers_recompletion_model.Field_ID = concessionField.Field_ID;
                    workovers_recompletion_model.Actual_year = year;
                    workovers_recompletion_model.proposed_year = (int.Parse(year) + 1).ToString();

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
                            _context.WORKOVERS_RECOMPLETION_JOBs1.Remove(getData);
                            await _context.WORKOVERS_RECOMPLETION_JOBs1.AddAsync(workovers_recompletion_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.WORKOVERS_RECOMPLETION_JOBs1.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.WORKOVERS_RECOMPLETION_JOBs1 where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITY")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITY([FromBody] OIL_CONDENSATE_PRODUCTION_ACTIVITy oil_condensate_activity_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving Oil Condensate data
                if (oil_condensate_activity_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_condensate_activity_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_activity_model.CompanyName = WKPCompanyName;
                    oil_condensate_activity_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_activity_model.CompanyNumber = WKPCompanyNumber;
                    oil_condensate_activity_model.Date_Updated = DateTime.Now;
                    oil_condensate_activity_model.Updated_by = WKPCompanyId;
                    oil_condensate_activity_model.Year_of_WP = year;
                    oil_condensate_activity_model.OML_Name = oil_condensate_activity_model.OML_Name.ToUpper();
                    oil_condensate_activity_model.Field_ID = concessionField.Field_ID;
                    oil_condensate_activity_model.Actual_year = year;
                    oil_condensate_activity_model.proposed_year = (int.Parse(year) + 1).ToString();


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
                            _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.Remove(getData);
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.AddAsync(oil_condensate_activity_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION([FromBody] OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION oil_condensate_unitisation_model, List<IFormFile> files, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving Oil Condensate data
                if (oil_condensate_unitisation_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_condensate_unitisation_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_unitisation_model.CompanyName = WKPCompanyName;
                    oil_condensate_unitisation_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_unitisation_model.CompanyNumber = WKPCompanyNumber;
                    oil_condensate_unitisation_model.Date_Updated = DateTime.Now;
                    oil_condensate_unitisation_model.Updated_by = WKPCompanyId;
                    oil_condensate_unitisation_model.Year_of_WP = year;
                    oil_condensate_unitisation_model.OML_Name = omlName;
                    oil_condensate_unitisation_model.Field_ID = concessionField.Field_ID;
                    oil_condensate_unitisation_model.Actual_year = year;
                    oil_condensate_unitisation_model.proposed_year = (int.Parse(year) + 1).ToString();

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
                            _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.Remove(getData);
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.AddAsync(oil_condensate_unitisation_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_GAS_PRODUCTION_ACTIVITY")]
        public async Task<WebApiResponse> POST_GAS_PRODUCTION_ACTIVITY([FromBody] GAS_PRODUCTION_ACTIVITy gas_production_model, List<IFormFile> files, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving Oil Condensate data
                if (gas_production_model != null)
                {
                    var getData = (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    gas_production_model.Companyemail = WKPCompanyEmail;
                    gas_production_model.CompanyName = WKPCompanyName;
                    gas_production_model.COMPANY_ID = WKPCompanyId;
                    gas_production_model.CompanyNumber = WKPCompanyNumber;
                    gas_production_model.Date_Updated = DateTime.Now;
                    gas_production_model.Updated_by = WKPCompanyId;
                    gas_production_model.Year_of_WP = year;
                    gas_production_model.OML_Name = omlName;
                    gas_production_model.Field_ID = concessionField.Field_ID;
                    gas_production_model.Actual_year = year;
                    gas_production_model.proposed_year = (int.Parse(year) + 1).ToString();

                    #region file section
                    //UploadedDocument Upload_NDR_payment_receipt_File = null;

                    // if (files[0] != null)
                    // {
                    //     string docName = "NDR Payment Receipt";
                    //     Upload_NDR_payment_receipt_File = _helpersController.UploadDocument(files[0], "NDRPaymentReceipt");
                    //     if (Upload_NDR_payment_receipt_File == null)
                    //         return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    // }
                    #endregion


                    //gas_production_model.Upload_NDR_payment_receipt = files[0] != null ? Upload_NDR_payment_receipt_File.filePath : null;
                    //gas_production_model.NDRFilename = files[0] != null ? Upload_NDR_payment_receipt_File.fileName : null;
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
                            _context.GAS_PRODUCTION_ACTIVITIEs.Remove(getData);
                            await _context.GAS_PRODUCTION_ACTIVITIEs.AddAsync(gas_production_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.GAS_PRODUCTION_ACTIVITIEs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };
                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_NDR")]
        public async Task<WebApiResponse> POST_NDR([FromBody] NDR ndr_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving NDR data
                if (ndr_model != null)
                {
                    var getData = (from c in _context.NDRs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    ndr_model.Companyemail = WKPCompanyEmail;
                    ndr_model.CompanyName = WKPCompanyName;
                    ndr_model.COMPANY_ID = WKPCompanyId;
                    ndr_model.CompanyNumber = WKPCompanyNumber;
                    ndr_model.Date_Updated = DateTime.Now;
                    ndr_model.Updated_by = WKPCompanyId;
                    ndr_model.Year_of_WP = year;
                    ndr_model.OML_Name = omlName;
                    ndr_model.Field_ID = concessionField.Field_ID;
                    var getGas_ProductionData = (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

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
                            _context.NDRs.Remove(getData);
                            await _context.NDRs.AddAsync(ndr_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.NDRs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.NDRs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_RESERVES_UPDATES_LIFE_INDEX")]
        public async Task<WebApiResponse> POST_RESERVES_UPDATES_LIFE_INDEX([FromBody] RESERVES_UPDATES_LIFE_INDEX reserves_life_index_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving RESERVES_UPDATES_LIFE_INDEX data
                if (reserves_life_index_model != null)
                {
                    var getData = (from c in _context.RESERVES_UPDATES_LIFE_INDices where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    reserves_life_index_model.Companyemail = WKPCompanyEmail;
                    reserves_life_index_model.CompanyName = WKPCompanyName;
                    reserves_life_index_model.COMPANY_ID = WKPCompanyId;
                    reserves_life_index_model.CompanyNumber = WKPCompanyNumber;
                    reserves_life_index_model.Date_Updated = DateTime.Now;
                    reserves_life_index_model.Updated_by = WKPCompanyId;
                    reserves_life_index_model.Year_of_WP = year;
                    reserves_life_index_model.OML_Name = omlName;
                    reserves_life_index_model.Field_ID = concessionField.Field_ID;
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
                            _context.RESERVES_UPDATES_LIFE_INDices.Remove(getData);
                            await _context.RESERVES_UPDATES_LIFE_INDices.AddAsync(reserves_life_index_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_UPDATES_LIFE_INDices.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_PRECEEDING")]
        public async Task<WebApiResponse> POST_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_PRECEEDING([FromBody] RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE reserves_condensate_status_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                #region Saving RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE data
                if (reserves_condensate_status_model != null)
                {
                    var getData = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year && c.FLAG1 == "COMPANY_RESERVE_OF_PRECEDDING_YEAR" select c).FirstOrDefaultAsync();

                    reserves_condensate_status_model.Companyemail = WKPCompanyEmail;
                    reserves_condensate_status_model.CompanyName = WKPCompanyName;
                    reserves_condensate_status_model.COMPANY_ID = WKPCompanyId;
                    reserves_condensate_status_model.CompanyNumber = WKPCompanyNumber;
                    reserves_condensate_status_model.Date_Updated = DateTime.Now;
                    reserves_condensate_status_model.Updated_by = WKPCompanyId;
                    reserves_condensate_status_model.Year_of_WP = year;
                    reserves_condensate_status_model.OML_Name = omlName;
                    reserves_condensate_status_model.Field_ID = concessionField.Field_ID;
                    reserves_condensate_status_model.FLAG1 = "COMPANY_RESERVE_OF_PRECEDDING_YEAR";

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
                            _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Remove(getData);
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.AddAsync(reserves_condensate_status_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }


        [HttpPost("POST_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_CURRENT")]
        public async Task<WebApiResponse> POST_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_CURRENT([FromBody] RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE reserves_condensate_status_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                #region Saving RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE data
                if (reserves_condensate_status_model != null)
                {
                    var getData = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year && c.FLAG1 == "COMPANY_CURRENT_RESERVE" select c).FirstOrDefaultAsync();

                    reserves_condensate_status_model.Companyemail = WKPCompanyEmail;
                    reserves_condensate_status_model.CompanyName = WKPCompanyName;
                    reserves_condensate_status_model.COMPANY_ID = WKPCompanyId;
                    reserves_condensate_status_model.CompanyNumber = WKPCompanyNumber;
                    reserves_condensate_status_model.Date_Updated = DateTime.Now;
                    reserves_condensate_status_model.Updated_by = WKPCompanyId;
                    reserves_condensate_status_model.Year_of_WP = year;
                    reserves_condensate_status_model.OML_Name = omlName;
                    reserves_condensate_status_model.Field_ID = concessionField.Field_ID;
                    reserves_condensate_status_model.FLAG1 = "COMPANY_CURRENT_RESERVE";

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
                            _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Remove(getData);
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.AddAsync(reserves_condensate_status_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_RESERVES_UPDATES_OIL_CONDENSATE_FIVEYEARS_PROJECTION")]
        public async Task<WebApiResponse> POST_RESERVES_UPDATES_OIL_CONDENSATE_FIVEYEARS_PROJECTION([FromBody] RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection reserves_condensate_status_model, string omlName, string fieldName,  string year, string actionToDo)
        {
            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projection data
                if (reserves_condensate_status_model != null)
                {
                    var getData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    reserves_condensate_status_model.Companyemail = WKPCompanyEmail;
                    reserves_condensate_status_model.CompanyName = WKPCompanyName;
                    reserves_condensate_status_model.COMPANY_ID = WKPCompanyId;
                    reserves_condensate_status_model.CompanyNumber = WKPCompanyNumber;
                    reserves_condensate_status_model.Date_Updated = DateTime.Now;
                    reserves_condensate_status_model.Updated_by = WKPCompanyId;
                    reserves_condensate_status_model.Year_of_WP = year;
                    reserves_condensate_status_model.OML_Name = omlName;
                    reserves_condensate_status_model.Field_ID = concessionField.Field_ID;
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
                            _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections.Remove(getData);
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections.AddAsync(reserves_condensate_status_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Fiveyear_Projections where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION([FromForm] OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION oil_condensate_fiveyears_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION data
                if (oil_condensate_fiveyears_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year && c.Fiveyear_Timeline == oil_condensate_fiveyears_model.Fiveyear_Timeline select c).FirstOrDefault();

                    oil_condensate_fiveyears_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_fiveyears_model.CompanyName = WKPCompanyName;
                    oil_condensate_fiveyears_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_fiveyears_model.CompanyNumber = WKPCompanyNumber;
                    oil_condensate_fiveyears_model.Date_Updated = DateTime.Now;
                    oil_condensate_fiveyears_model.Updated_by = WKPCompanyId;
                    oil_condensate_fiveyears_model.Year_of_WP = year;
                    oil_condensate_fiveyears_model.OML_Name = omlName;
                    oil_condensate_fiveyears_model.Field_ID = concessionField.Field_ID;
                    oil_condensate_fiveyears_model.Actual_year = year;
                    oil_condensate_fiveyears_model.proposed_year = (int.Parse(year) + 1).ToString();


                    #region file section
                    var file1 = Request.Form.Files[0];
                    var blobname1 = blobService.Filenamer(file1);

                    if (file1 != null)
                    {
                        string docName = "Production Oil Condensate AGNAG Document";
                        oil_condensate_fiveyears_model.ProductionOilCondensateAGNAGUFilename = blobname1;
                        oil_condensate_fiveyears_model.ProductionOilCondensateAGNAGUploadFilePath= await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"ProductionOilCondensateAGNAGDocument/{blobname1}");
                        if (oil_condensate_fiveyears_model.ProductionOilCondensateAGNAGUploadFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                    }
                    #endregion

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
                            _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.Remove(getData);
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.AddAsync(oil_condensate_fiveyears_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_RESERVES_UPDATES_OIL_CONDENSATE_COMPANY_ANNUAL_PRODUCTION")]
        public async Task<WebApiResponse> POST_RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION([FromBody] RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION reserves_update_production_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION data
                if (reserves_update_production_model != null)
                {
                    var getData = (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    reserves_update_production_model.Companyemail = WKPCompanyEmail;
                    reserves_update_production_model.CompanyName = WKPCompanyName;
                    reserves_update_production_model.COMPANY_ID = WKPCompanyId;
                    reserves_update_production_model.CompanyNumber = WKPCompanyNumber;
                    reserves_update_production_model.Date_Updated = DateTime.Now;
                    reserves_update_production_model.Updated_by = WKPCompanyId;
                    reserves_update_production_model.Year_of_WP = year;
                    reserves_update_production_model.OML_Name = omlName;
                    reserves_update_production_model.Field_ID = concessionField.Field_ID;
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
                            _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.Remove(getData);
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.AddAsync(reserves_update_production_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_RESERVES_UPDATES_OIL_CONDENSATE_RESERVES_DECLINE")]
        public async Task<WebApiResponse> POST_RESERVES_UPDATES_OIL_CONDENSATE_RESERVES_DECLINE([FromBody] RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE reserves_update_decline_model, string omlName, string fieldName,  string year, string actionToDo)
        {
            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                #region Saving RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE data
                if (reserves_update_decline_model != null)
                {
                    var getData = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefaultAsync();

                    reserves_update_decline_model.Companyemail = WKPCompanyEmail;
                    reserves_update_decline_model.CompanyName = WKPCompanyName;
                    reserves_update_decline_model.COMPANY_ID = WKPCompanyId;
                    reserves_update_decline_model.CompanyNumber = WKPCompanyNumber;
                    reserves_update_decline_model.Date_Updated = DateTime.Now;
                    reserves_update_decline_model.Updated_by = WKPCompanyId;
                    reserves_update_decline_model.Year_of_WP = year;
                    reserves_update_decline_model.OML_Name = omlName;
                    reserves_update_decline_model.Field_ID = concessionField.Field_ID;
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
                            _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.Remove(getData);
                            await _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.AddAsync(reserves_update_decline_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };
                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_MONTHLY_ACTIVITY")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_MONTHLY_ACTIVITY([FromBody] OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity oil_condensate_reserves_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity data
                if (oil_condensate_reserves_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year && c.Production_month == oil_condensate_reserves_model.Production_month select c).FirstOrDefault();

                    oil_condensate_reserves_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_reserves_model.CompanyName = WKPCompanyName;
                    oil_condensate_reserves_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_reserves_model.CompanyNumber = WKPCompanyNumber;
                    oil_condensate_reserves_model.Date_Updated = DateTime.Now;
                    oil_condensate_reserves_model.Updated_by = WKPCompanyId;
                    oil_condensate_reserves_model.Year_of_WP = year;
                    oil_condensate_reserves_model.OML_Name = omlName;
                    oil_condensate_reserves_model.Field_ID = concessionField.Field_ID;

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
                            _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.Remove(getData);
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.AddAsync(oil_condensate_reserves_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_RESERVES_REPLACEMENT_RATIO")]
        public async Task<WebApiResponse> POST_RESERVES_REPLACEMENT_RATIO([FromBody] RESERVES_REPLACEMENT_RATIO reserves_replacement_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                #region Saving RESERVES_REPLACEMENT_RATIO data
                if (reserves_replacement_model != null)
                {
                    var getData = (from c in _context.RESERVES_REPLACEMENT_RATIOs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    reserves_replacement_model.Companyemail = WKPCompanyEmail;
                    reserves_replacement_model.CompanyName = WKPCompanyName;
                    reserves_replacement_model.COMPANY_ID = WKPCompanyId;
                    reserves_replacement_model.CompanyNumber = WKPCompanyNumber;
                    reserves_replacement_model.Date_Updated = DateTime.Now;
                    reserves_replacement_model.Updated_by = WKPCompanyId;
                    reserves_replacement_model.Year_of_WP = year;
                    reserves_replacement_model.OML_Name = omlName;
                    reserves_replacement_model.Field_ID = concessionField.Field_ID;
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
                            _context.RESERVES_REPLACEMENT_RATIOs.Remove(getData);
                            await _context.RESERVES_REPLACEMENT_RATIOs.AddAsync(reserves_replacement_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.RESERVES_REPLACEMENT_RATIOs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.RESERVES_REPLACEMENT_RATIOs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_MONTHLY_ACTIVITIES_PROPOSED")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_MONTHLY_ACTIVITIES_PROPOSED([FromBody] OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED oil_condensate_monthly_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED data
                if (oil_condensate_monthly_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year && c.Production_month == oil_condensate_monthly_model.Production_month select c).FirstOrDefault();

                    oil_condensate_monthly_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_monthly_model.CompanyName = WKPCompanyName;
                    oil_condensate_monthly_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_monthly_model.CompanyNumber = WKPCompanyNumber;
                    oil_condensate_monthly_model.Date_Updated = DateTime.Now;
                    oil_condensate_monthly_model.Updated_by = WKPCompanyId;
                    oil_condensate_monthly_model.Year_of_WP = year;
                    oil_condensate_monthly_model.OML_Name = omlName;
                    oil_condensate_monthly_model.Field_ID = concessionField.Field_ID;
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
                            _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.Remove(getData);
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.AddAsync(oil_condensate_monthly_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully." + "2022";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };
                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_OIL_GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY")]
        public async Task<WebApiResponse> POST_OIL_GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY([FromBody] GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY oil_gas_domestic_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY data
                if (oil_gas_domestic_model != null)
                {
                    var getData = (from c in _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_gas_domestic_model.Companyemail = WKPCompanyEmail;
                    oil_gas_domestic_model.CompanyName = WKPCompanyName;
                    oil_gas_domestic_model.COMPANY_ID = WKPCompanyId;
                    oil_gas_domestic_model.CompanyNumber = WKPCompanyNumber;
                    oil_gas_domestic_model.Date_Updated = DateTime.Now;
                    oil_gas_domestic_model.Updated_by = WKPCompanyId;
                    oil_gas_domestic_model.Year_of_WP = year;
                    oil_gas_domestic_model.OML_Name = omlName;
                    oil_gas_domestic_model.Field_ID = concessionField.Field_ID;
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
                            _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.Remove(getData);
                            await _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.AddAsync(oil_gas_domestic_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_BUDGET_ACTUAL_EXPENDITURE")]
        public async Task<WebApiResponse> POST_BUDGET_ACTUAL_EXPENDITURE([FromBody] BUDGET_ACTUAL_EXPENDITURE budget_actual_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.BUDGET_ACTUAL_EXPENDITUREs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.BUDGET_ACTUAL_EXPENDITUREs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (budget_actual_model != null)
                {
                    var getData = (from c in _context.BUDGET_ACTUAL_EXPENDITUREs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_actual_model.Companyemail = WKPCompanyEmail;
                    budget_actual_model.CompanyName = WKPCompanyName;
                    budget_actual_model.COMPANY_ID = WKPCompanyId;
                    budget_actual_model.CompanyNumber = WKPCompanyNumber;
                    budget_actual_model.Date_Updated = DateTime.Now;
                    budget_actual_model.Updated_by = WKPCompanyId;
                    budget_actual_model.Year_of_WP = year;
                    budget_actual_model.OML_Name = omlName;
                    budget_actual_model.Field_ID = concessionField.Field_ID;
                    budget_actual_model.Actual_year = year;
                    budget_actual_model.Proposed_year = (int.Parse(year) + 1).ToString();
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
                        _context.BUDGET_ACTUAL_EXPENDITUREs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_ACTUAL_EXPENDITUREs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT")]
        public async Task<WebApiResponse> POST_BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT([FromBody] BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENT budget_proposal_model, string omlName, string fieldName,  string year, string id,string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (budget_proposal_model != null)
                {
                    var getData = (from c in _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_proposal_model.Companyemail = WKPCompanyEmail;
                    budget_proposal_model.CompanyName = WKPCompanyName;
                    budget_proposal_model.COMPANY_ID = WKPCompanyId;
                    budget_proposal_model.CompanyNumber = WKPCompanyNumber;
                    budget_proposal_model.Date_Updated = DateTime.Now;
                    budget_proposal_model.Updated_by = WKPCompanyId;
                    budget_proposal_model.Year_of_WP = year;
                    budget_proposal_model.OML_Name = omlName;
                    budget_proposal_model.Field_ID = concessionField.Field_ID;
                    budget_proposal_model.Actual_year = year;
                    budget_proposal_model.Proposed_year = (int.Parse(year) + 1).ToString();
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
                            _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs.Remove(getData);
                            await _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs.AddAsync(budget_proposal_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_PROPOSAL_IN_NAIRA_AND_DOLLAR_COMPONENTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITY")]
        public async Task<WebApiResponse> POST_BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy([FromBody] BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITy budget_exploratory_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (budget_exploratory_model != null)
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_exploratory_model.Companyemail = WKPCompanyEmail;
                    budget_exploratory_model.CompanyName = WKPCompanyName;
                    budget_exploratory_model.COMPANY_ID = WKPCompanyId;
                    budget_exploratory_model.CompanyNumber = WKPCompanyNumber;
                    budget_exploratory_model.Date_Updated = DateTime.Now;
                    budget_exploratory_model.Updated_by = WKPCompanyId;
                    budget_exploratory_model.Year_of_WP = year;
                    budget_exploratory_model.OML_Name = omlName;
                    budget_exploratory_model.Field_ID = concessionField.Field_ID;
                    if (action == GeneralModel.Insert) if (action == GeneralModel.Insert)
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
                            _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs.Remove(getData);
                        }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITY")]
        public async Task<WebApiResponse> POST_BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy([FromBody] BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITy budget_proposal_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (budget_proposal_model != null)
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_proposal_model.Companyemail = WKPCompanyEmail;
                    budget_proposal_model.CompanyName = WKPCompanyName;
                    budget_proposal_model.COMPANY_ID = WKPCompanyId;
                    budget_proposal_model.CompanyNumber = WKPCompanyNumber;
                    budget_proposal_model.Date_Updated = DateTime.Now;
                    budget_proposal_model.Updated_by = WKPCompanyId;
                    budget_proposal_model.Year_of_WP = year;
                    budget_proposal_model.OML_Name = omlName;
                    budget_proposal_model.Field_ID = concessionField.Field_ID;
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
                        _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_BUDGET_PERFORMANCE_PRODUCTION_COST")]
        public async Task<WebApiResponse> POST_BUDGET_PERFORMANCE_PRODUCTION_COST([FromBody] BUDGET_PERFORMANCE_PRODUCTION_COST budget_performance_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (budget_performance_model != null)
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_performance_model.Companyemail = WKPCompanyEmail;
                    budget_performance_model.CompanyName = WKPCompanyName;
                    budget_performance_model.COMPANY_ID = WKPCompanyId;
                    budget_performance_model.CompanyNumber = WKPCompanyNumber;
                    budget_performance_model.Date_Updated = DateTime.Now;
                    budget_performance_model.Updated_by = WKPCompanyId;
                    budget_performance_model.Year_of_WP = year;
                    budget_performance_model.OML_Name = omlName;
                    budget_performance_model.Field_ID = concessionField.Field_ID;
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
                        _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT")]
        public async Task<WebApiResponse> POST_BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT([FromBody] BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT budget_facilities_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (budget_facilities_model != null)
                {
                    var getData = (from c in _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_facilities_model.Companyemail = WKPCompanyEmail;
                    budget_facilities_model.CompanyName = WKPCompanyName;
                    budget_facilities_model.COMPANY_ID = WKPCompanyId;
                    budget_facilities_model.CompanyNumber = WKPCompanyNumber;
                    budget_facilities_model.Date_Updated = DateTime.Now;
                    budget_facilities_model.Updated_by = WKPCompanyId;
                    budget_facilities_model.Year_of_WP = year;
                    budget_facilities_model.OML_Name = omlName;
                    budget_facilities_model.Field_ID = concessionField.Field_ID;

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
                        _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE")]
        public async Task<WebApiResponse> POST_OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE([FromBody] OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITURE oil_gas_facility_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (oil_gas_facility_model != null)
                {
                    var getData = (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_gas_facility_model.Companyemail = WKPCompanyEmail;
                    oil_gas_facility_model.CompanyName = WKPCompanyName;
                    oil_gas_facility_model.COMPANY_ID = WKPCompanyId;
                    oil_gas_facility_model.CompanyNumber = WKPCompanyNumber;
                    oil_gas_facility_model.Date_Updated = DateTime.Now;
                    oil_gas_facility_model.Updated_by = WKPCompanyId;
                    oil_gas_facility_model.Year_of_WP = year;
                    oil_gas_facility_model.OML_Name = omlName;
                    oil_gas_facility_model.Field_ID = concessionField.Field_ID;
                    oil_gas_facility_model.Actual_year = year;
                    oil_gas_facility_model.Proposed_year = (int.Parse(year) + 1).ToString();

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
                            _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs.Remove(getData);
                            await _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs.AddAsync(oil_gas_facility_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_EXPENDITUREs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_NEW_TECHNOLOGY_CONFORMITY_ASSESSMENT")]
        public async Task<WebApiResponse> POST_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment([FromBody] OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment oil_condensate_assessment_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (oil_condensate_assessment_model != null)
                {
                    var getData = (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_condensate_assessment_model.Companyemail = WKPCompanyEmail;
                    oil_condensate_assessment_model.CompanyName = WKPCompanyName;
                    oil_condensate_assessment_model.COMPANY_ID = WKPCompanyId;
                    oil_condensate_assessment_model.CompanyNumber = WKPCompanyNumber;
                    oil_condensate_assessment_model.Date_Updated = DateTime.Now;
                    oil_condensate_assessment_model.Updated_by = WKPCompanyId;
                    oil_condensate_assessment_model.Year_of_WP = year;
                    oil_condensate_assessment_model.OML_Name = omlName;
                    oil_condensate_assessment_model.Field_ID = concessionField.Field_ID;

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
                            _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.Remove(getData);
                            await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.AddAsync(oil_condensate_assessment_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT")]
        public async Task<WebApiResponse> POST_OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT([FromBody] OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT oil_gas_facility_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (oil_gas_facility_model != null)
                {
                    var getData = (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    oil_gas_facility_model.Companyemail = WKPCompanyEmail;
                    oil_gas_facility_model.CompanyName = WKPCompanyName;
                    oil_gas_facility_model.COMPANY_ID = WKPCompanyId;
                    oil_gas_facility_model.CompanyNumber = WKPCompanyNumber;
                    oil_gas_facility_model.Date_Updated = DateTime.Now;
                    oil_gas_facility_model.Updated_by = WKPCompanyId;
                    oil_gas_facility_model.Year_of_WP = year;
                    oil_gas_facility_model.OML_Name = omlName;
                    oil_gas_facility_model.Field_ID = concessionField.Field_ID;
                    oil_gas_facility_model.Actual_year = year;
                    oil_gas_facility_model.Proposed_year = (int.Parse(year) + 1).ToString();

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
                            _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Remove(getData);
                            await _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.AddAsync(oil_gas_facility_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_FACILITIES_PROJECT_PERFORMANCE")]
        public async Task<WebApiResponse> POST_FACILITIES_PROJECT_PERFORMANCE([FromBody] FACILITIES_PROJECT_PERFORMANCE facilities_project_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.FACILITIES_PROJECT_PERFORMANCEs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.FACILITIES_PROJECT_PERFORMANCEs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (facilities_project_model != null)
                {
                    var getData = (from c in _context.FACILITIES_PROJECT_PERFORMANCEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    facilities_project_model.Companyemail = WKPCompanyEmail;
                    facilities_project_model.CompanyName = WKPCompanyName;
                    facilities_project_model.COMPANY_ID = WKPCompanyId;
                    facilities_project_model.CompanyNumber = WKPCompanyNumber;
                    facilities_project_model.Date_Updated = DateTime.Now;
                    facilities_project_model.Updated_by = WKPCompanyId;
                    facilities_project_model.Year_of_WP = year;
                    facilities_project_model.OML_Name = facilities_project_model.OML_Name.ToUpper();
                    facilities_project_model.OML_Name = omlName;
                    facilities_project_model.Field_ID = concessionField.Field_ID;

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
                            _context.FACILITIES_PROJECT_PERFORMANCEs.Remove(getData);
                            await _context.FACILITIES_PROJECT_PERFORMANCEs.AddAsync(facilities_project_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.FACILITIES_PROJECT_PERFORMANCEs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.FACILITIES_PROJECT_PERFORMANCEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_BUDGET_CAPEX_OPEX")]
        public async Task<WebApiResponse> POST_BUDGET_CAPEX_OPEX([FromBody] BUDGET_CAPEX_OPEX budget_capex_opex_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.BUDGET_CAPEX_OPices where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.BUDGET_CAPEX_OPices.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (budget_capex_opex_model != null)
                {
                    var getData = (from c in _context.BUDGET_CAPEX_OPices where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    budget_capex_opex_model.Companyemail = WKPCompanyEmail;
                    budget_capex_opex_model.CompanyName = WKPCompanyName;
                    budget_capex_opex_model.COMPANY_ID = WKPCompanyId;
                    budget_capex_opex_model.CompanyNumber = WKPCompanyNumber;
                    budget_capex_opex_model.Date_Updated = DateTime.Now;
                    budget_capex_opex_model.Updated_by = WKPCompanyId;
                    budget_capex_opex_model.Year_of_WP = year;
                    budget_capex_opex_model.OML_Name = omlName;
                    budget_capex_opex_model.Field_ID = concessionField.Field_ID;

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
                            _context.BUDGET_CAPEX_OPices.Remove(getData);
                            await _context.BUDGET_CAPEX_OPices.AddAsync(budget_capex_opex_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.BUDGET_CAPEX_OPices.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.BUDGET_CAPEX_OPices where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_NIGERIA_CONTENT_TRAINING")]
        public async Task<WebApiResponse> POST_NIGERIA_CONTENT_Training([FromBody] NIGERIA_CONTENT_Training nigeria_content_training_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving NIGERIA_CONTENT_Trainings data
                if (nigeria_content_training_model != null)
                {
                    var getData = (from c in _context.NIGERIA_CONTENT_Trainings where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    nigeria_content_training_model.Companyemail = WKPCompanyEmail;
                    nigeria_content_training_model.CompanyName = WKPCompanyName;
                    nigeria_content_training_model.COMPANY_ID = WKPCompanyId;
                    nigeria_content_training_model.CompanyNumber = WKPCompanyNumber;
                    nigeria_content_training_model.Date_Updated = DateTime.Now;
                    nigeria_content_training_model.Updated_by = WKPCompanyId;
                    nigeria_content_training_model.Year_of_WP = year;
                    nigeria_content_training_model.OML_Name = omlName;
                    nigeria_content_training_model.Field_ID = concessionField.Field_ID;
                    nigeria_content_training_model.Actual_Proposed_Year = (int.Parse(year) + 1).ToString();

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
                            _context.NIGERIA_CONTENT_Trainings.Remove(getData);
                            await _context.NIGERIA_CONTENT_Trainings.AddAsync(nigeria_content_training_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.NIGERIA_CONTENT_Trainings.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.NIGERIA_CONTENT_Trainings where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_NIGERIA_CONTENT_UPLOAD_SUCCESSION_PLAN")]
        public async Task<WebApiResponse> POST_NIGERIA_CONTENT_UPLOAD_SUCCESSION_PLAN([FromBody] NIGERIA_CONTENT_Upload_Succession_Plan nigeria_content_succession_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving NIGERIA_CONTENT_Upload_Succession_Plans data
                if (nigeria_content_succession_model != null)
                {
                    var getData = (from c in _context.NIGERIA_CONTENT_Upload_Succession_Plans where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    nigeria_content_succession_model.Companyemail = WKPCompanyEmail;
                    nigeria_content_succession_model.CompanyName = WKPCompanyName;
                    nigeria_content_succession_model.COMPANY_ID = WKPCompanyId;
                    nigeria_content_succession_model.CompanyNumber = WKPCompanyNumber;
                    nigeria_content_succession_model.Date_Updated = DateTime.Now;
                    nigeria_content_succession_model.Updated_by = WKPCompanyId;
                    nigeria_content_succession_model.Year_of_WP = year;
                    nigeria_content_succession_model.OML_Name = omlName;
                    nigeria_content_succession_model.Field_ID = concessionField.Field_ID;
                    nigeria_content_succession_model.Actual_Proposed_Year = (int.Parse(year) + 1).ToString();

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
                            _context.NIGERIA_CONTENT_Upload_Succession_Plans.Remove(getData);
                            await _context.NIGERIA_CONTENT_Upload_Succession_Plans.AddAsync(nigeria_content_succession_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.NIGERIA_CONTENT_Upload_Succession_Plans.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.NIGERIA_CONTENT_Upload_Succession_Plans where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_NIGERIA_CONTENT_QUESTION")]
        public async Task<WebApiResponse> POST_NIGERIA_CONTENT_QUESTION([FromBody] NIGERIA_CONTENT_QUESTION nigeria_content_question_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving NIGERIA_CONTENT_QUESTIONs data
                if (nigeria_content_question_model != null)
                {
                    var getData = (from c in _context.NIGERIA_CONTENT_QUESTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    nigeria_content_question_model.Companyemail = WKPCompanyEmail;
                    nigeria_content_question_model.CompanyName = WKPCompanyName;
                    nigeria_content_question_model.COMPANY_ID = WKPCompanyId;
                    nigeria_content_question_model.CompanyNumber = WKPCompanyNumber;
                    nigeria_content_question_model.Date_Updated = DateTime.Now;
                    nigeria_content_question_model.Updated_by = WKPCompanyId;
                    nigeria_content_question_model.Year_of_WP = year;
                    nigeria_content_question_model.OML_Name = nigeria_content_question_model.OML_Name.ToUpper();
                    nigeria_content_question_model.OML_Name = omlName;
                    nigeria_content_question_model.Field_ID = concessionField.Field_ID;
                    
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
                            _context.NIGERIA_CONTENT_QUESTIONs.Remove(getData);
                            await _context.NIGERIA_CONTENT_QUESTIONs.AddAsync(nigeria_content_question_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.NIGERIA_CONTENT_QUESTIONs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.NIGERIA_CONTENT_QUESTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_LEGAL_LITIGATION")]
        public async Task<WebApiResponse> POST_LEGAL_LITIGATION([FromBody] LEGAL_LITIGATION legal_litigation_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving LEGAL_LITIGATIONs data
                if (legal_litigation_model != null)
                {
                    var getData = (from c in _context.LEGAL_LITIGATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    legal_litigation_model.Companyemail = WKPCompanyEmail;
                    legal_litigation_model.CompanyName = WKPCompanyName;
                    legal_litigation_model.COMPANY_ID = WKPCompanyId;
                    legal_litigation_model.CompanyNumber = WKPCompanyNumber;
                    legal_litigation_model.Date_Updated = DateTime.Now;
                    legal_litigation_model.Updated_by = WKPCompanyId;
                    legal_litigation_model.Year_of_WP = year;
                    legal_litigation_model.OML_Name = omlName;
                    legal_litigation_model.Field_ID = concessionField.Field_ID;
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
                            _context.LEGAL_LITIGATIONs.Remove(getData);
                            await _context.LEGAL_LITIGATIONs.AddAsync(legal_litigation_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.LEGAL_LITIGATIONs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.LEGAL_LITIGATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_LEGAL_ARBITRATION")]
        public async Task<WebApiResponse> POST_LEGAL_ARBITRATION([FromBody] LEGAL_ARBITRATION legal_arbitration_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving LEGAL_ARBITRATIONs data
                if (legal_arbitration_model != null)
                {
                    var getData = (from c in _context.LEGAL_ARBITRATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    legal_arbitration_model.Companyemail = WKPCompanyEmail;
                    legal_arbitration_model.CompanyName = WKPCompanyName;
                    legal_arbitration_model.COMPANY_ID = WKPCompanyId;
                    legal_arbitration_model.CompanyNumber = WKPCompanyNumber;
                    legal_arbitration_model.Date_Updated = DateTime.Now;
                    legal_arbitration_model.Updated_by = WKPCompanyId;
                    legal_arbitration_model.Year_of_WP = year;
                    legal_arbitration_model.OML_Name = omlName;
                    legal_arbitration_model.Field_ID = concessionField.Field_ID;
                    
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
                            _context.LEGAL_ARBITRATIONs.Remove(getData);
                            await _context.LEGAL_ARBITRATIONs.AddAsync(legal_arbitration_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.LEGAL_ARBITRATIONs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.LEGAL_ARBITRATIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_STRATEGIC_PLANS_ON_COMPANY_BASES")]
        public async Task<WebApiResponse> POST_STRATEGIC_PLANS_ON_COMPANY_BASI([FromBody] STRATEGIC_PLANS_ON_COMPANY_BASI strategic_plans_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving STRATEGIC_PLANS_ON_COMPANY_BASIs data
                if (strategic_plans_model != null)
                {
                    var getData = (from c in _context.STRATEGIC_PLANS_ON_COMPANY_BAses where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    strategic_plans_model.Companyemail = WKPCompanyEmail;
                    strategic_plans_model.CompanyName = WKPCompanyName;
                    strategic_plans_model.COMPANY_ID = WKPCompanyId;
                    strategic_plans_model.CompanyNumber = WKPCompanyNumber;
                    strategic_plans_model.Date_Updated = DateTime.Now;
                    strategic_plans_model.Updated_by = WKPCompanyId;
                    strategic_plans_model.Year_of_WP = year;
                    strategic_plans_model.OML_Name = omlName;
                    strategic_plans_model.Field_ID = concessionField.Field_ID;
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
                            _context.STRATEGIC_PLANS_ON_COMPANY_BAses.Remove(getData);
                            await _context.STRATEGIC_PLANS_ON_COMPANY_BAses.AddAsync(strategic_plans_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.STRATEGIC_PLANS_ON_COMPANY_BAses.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.STRATEGIC_PLANS_ON_COMPANY_BAses where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_HSE_QUESTION")]
        public async Task<WebApiResponse> POST_HSE_QUESTION([FromBody] HSE_QUESTION hse_question_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving HSE_QUESTIONs data
                if (hse_question_model != null)
                {
                    var getData = (from c in _context.HSE_QUESTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_question_model.Companyemail = WKPCompanyEmail;
                    hse_question_model.CompanyName = WKPCompanyName;
                    hse_question_model.COMPANY_ID = WKPCompanyId;
                    hse_question_model.CompanyNumber = WKPCompanyNumber;
                    hse_question_model.Date_Updated = DateTime.Now;
                    hse_question_model.Updated_by = WKPCompanyId;
                    hse_question_model.Year_of_WP = year;
                    hse_question_model.OML_Name = omlName;
                    hse_question_model.Field_ID = concessionField.Field_ID;
                   
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
                            _context.HSE_QUESTIONs.Remove(getData);
                            await _context.HSE_QUESTIONs.AddAsync(hse_question_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_QUESTIONs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_QUESTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_HSE_FATALITY")]
        public async Task<WebApiResponse> POST_HSE_FATALITY([FromBody] HSE_FATALITy hse_fatality_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving HSE_FATALITY data
                if (hse_fatality_model != null)
                {
                    var getData = (from c in _context.HSE_FATALITIEs where c.Fatalities_Type == hse_fatality_model.Fatalities_Type && c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_fatality_model.Companyemail = WKPCompanyEmail;
                    hse_fatality_model.CompanyName = WKPCompanyName;
                    hse_fatality_model.COMPANY_ID = WKPCompanyId;
                    hse_fatality_model.CompanyNumber = WKPCompanyNumber;
                    hse_fatality_model.Date_Updated = DateTime.Now;
                    hse_fatality_model.Updated_by = WKPCompanyId;
                    hse_fatality_model.Year_of_WP = year;
                    hse_fatality_model.OML_Name = omlName;
                    hse_fatality_model.Field_ID = concessionField.Field_ID;
                    
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
                            _context.HSE_FATALITIEs.Remove(getData);
                            await _context.HSE_FATALITIEs.AddAsync(hse_fatality_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_FATALITIEs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_FATALITIEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_HSE_DESIGNS_SAFETY")]
        public async Task<WebApiResponse> POST_HSE_DESIGNS_SAFETY([FromBody] HSE_DESIGNS_SAFETY hse_designs_safety_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving HSE_DESIGNS_SAFETYs data
                if (hse_designs_safety_model != null)
                {
                    var getData = (from c in _context.HSE_DESIGNS_SAFETies where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_designs_safety_model.Companyemail = WKPCompanyEmail;
                    hse_designs_safety_model.CompanyName = WKPCompanyName;
                    hse_designs_safety_model.COMPANY_ID = WKPCompanyId;
                    hse_designs_safety_model.CompanyNumber = WKPCompanyNumber;
                    hse_designs_safety_model.Date_Updated = DateTime.Now;
                    hse_designs_safety_model.Updated_by = WKPCompanyId;
                    hse_designs_safety_model.Year_of_WP = year;
                    hse_designs_safety_model.OML_Name = omlName;
                    hse_designs_safety_model.Field_ID = concessionField.Field_ID;
                    
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
                            _context.HSE_DESIGNS_SAFETies.Remove(getData);
                            await _context.HSE_DESIGNS_SAFETies.AddAsync(hse_designs_safety_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_DESIGNS_SAFETies.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_DESIGNS_SAFETies where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_HSE_INSPECTION_AND_MAINTENANCE_NEW")]
        public async Task<WebApiResponse> POST_HSE_INSPECTION_AND_MAINTENANCE_NEW([FromBody] HSE_INSPECTION_AND_MAINTENANCE_NEW hse_IM_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving HSE_INSPECTION_AND_MAINTENANCE_NEWs data
                if (hse_IM_model != null)
                {
                    var getData = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_IM_model.Companyemail = WKPCompanyEmail;
                    hse_IM_model.CompanyName = WKPCompanyName;
                    hse_IM_model.COMPANY_ID = WKPCompanyId;
                    hse_IM_model.CompanyNumber = WKPCompanyNumber;
                    hse_IM_model.Date_Updated = DateTime.Now;
                    hse_IM_model.Updated_by = WKPCompanyId;
                    hse_IM_model.Year_of_WP = year;
                    hse_IM_model.OML_Name = omlName;
                    hse_IM_model.Field_ID = concessionField.Field_ID;
                    hse_IM_model.ACTUAL_year = year;
                    hse_IM_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs.Remove(getData);
                            await _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs.AddAsync(hse_IM_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW")]
        public async Task<WebApiResponse> POST_HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW([FromBody] HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW hse_IM_facility_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs data
                if (hse_IM_facility_model != null)
                {
                    var getData = (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_IM_facility_model.Companyemail = WKPCompanyEmail;
                    hse_IM_facility_model.CompanyName = WKPCompanyName;
                    hse_IM_facility_model.COMPANY_ID = WKPCompanyId;
                    hse_IM_facility_model.CompanyNumber = WKPCompanyNumber;
                    hse_IM_facility_model.Date_Updated = DateTime.Now;
                    hse_IM_facility_model.Updated_by = WKPCompanyId;
                    hse_IM_facility_model.Year_of_WP = year;
                    hse_IM_facility_model.OML_Name = omlName;
                    hse_IM_facility_model.Field_ID = concessionField.Field_ID;
                    hse_IM_facility_model.ACTUAL_year = year;
                    hse_IM_facility_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.Remove(getData);
                            await _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.AddAsync(hse_IM_facility_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW")]
        public async Task<WebApiResponse> POST_HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW([FromBody] HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW hse_technical_safety_model,
            string omlName, string fieldName,  string year, string id, string actionToDo)
        {
            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_technical_safety_model != null)
                {
                    var getData = await (from c in _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();

                    hse_technical_safety_model.Companyemail = WKPCompanyEmail;
                    hse_technical_safety_model.CompanyName = WKPCompanyName;
                    hse_technical_safety_model.COMPANY_ID = WKPCompanyId;
                    hse_technical_safety_model.CompanyNumber = WKPCompanyNumber;
                    hse_technical_safety_model.Date_Updated = DateTime.Now;
                    hse_technical_safety_model.Updated_by = WKPCompanyId;
                    hse_technical_safety_model.Year_of_WP = year;
                    hse_technical_safety_model.OML_Name = omlName;
                    hse_technical_safety_model.Field_ID = concessionField.Field_ID;
                    
                    if (action == GeneralModel.Insert)
                    {
                        if (getData.Count() <= 0)
                        {
                            hse_technical_safety_model.Date_Created = DateTime.Now;
                            hse_technical_safety_model.Created_by = WKPCompanyId;
                            await _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.AddAsync(hse_technical_safety_model);
                        }
                        else
                        {
                            hse_technical_safety_model.Date_Created = getData.FirstOrDefault().Date_Created;
                            hse_technical_safety_model.Created_by = getData.FirstOrDefault().Created_by;
                            hse_technical_safety_model.Date_Updated = DateTime.Now;
                            hse_technical_safety_model.Updated_by = WKPCompanyId;
                            getData.ForEach(x =>
                            {
                                _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.Remove(x);
                                save += _context.SaveChanges();

                            });
                            await _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.AddAsync(hse_technical_safety_model);
                        }
                    }

                    save += await _context.SaveChangesAsync();
                }

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_HSE_SAFETY_STUDIES_NEW"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_HSE_SAFETY_STUDIES_NEW([FromForm] HSE_SAFETY_STUDIES_NEW hse_safety_studies_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_SAFETY_STUDIES_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_SAFETY_STUDIES_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_safety_studies_model != null)
                {
                    var getData = (from c in _context.HSE_SAFETY_STUDIES_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();

                    hse_safety_studies_model.Companyemail = WKPCompanyEmail;
                    hse_safety_studies_model.CompanyName = WKPCompanyName;
                    hse_safety_studies_model.COMPANY_ID = WKPCompanyId;
                    hse_safety_studies_model.CompanyNumber = WKPCompanyNumber;
                    hse_safety_studies_model.Date_Updated = DateTime.Now;
                    hse_safety_studies_model.Updated_by = WKPCompanyId;
                    hse_safety_studies_model.Year_of_WP = year;
                    hse_safety_studies_model.OML_Name = omlName;
                    hse_safety_studies_model.Field_ID = concessionField.Field_ID;
                    hse_safety_studies_model.ACTUAL_year = year;
                    hse_safety_studies_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

                    var file1 = Request.Form.Files[0];
                    var blobname1 = blobService.Filenamer(file1);

                    if (file1 != null)
                    {
                        string docName = "SMS";
                        hse_safety_studies_model.SMSFileUploadPath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"SMSDocuments/{blobname1}");
                        if (hse_safety_studies_model.SMSFileUploadPath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                    }
                    if (action == GeneralModel.Insert)
                    {
                        if (getData.Count <= 0)
                        {
                            hse_safety_studies_model.Date_Created = DateTime.Now;
                            hse_safety_studies_model.Created_by = WKPCompanyId;
                            await _context.HSE_SAFETY_STUDIES_NEWs.AddAsync(hse_safety_studies_model);
                        }
                        else
                        {
                            hse_safety_studies_model.Date_Created = getData.FirstOrDefault().Date_Created;
                            hse_safety_studies_model.Created_by = getData.FirstOrDefault().Created_by;
                            hse_safety_studies_model.Date_Updated = DateTime.Now;
                            hse_safety_studies_model.Updated_by = WKPCompanyId;
                            getData.ForEach(x =>
                            {
                                _context.HSE_SAFETY_STUDIES_NEWs.Remove(x);
                                save += _context.SaveChanges();
                            });
                            await _context.HSE_SAFETY_STUDIES_NEWs.AddAsync(hse_safety_studies_model);
                        }
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_SAFETY_STUDIES_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW")]
        public async Task<WebApiResponse> POST_HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW([FromBody] HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEW hse_asset_register_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs data
                if (hse_asset_register_model != null)
                {
                    var getData = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_asset_register_model.Companyemail = WKPCompanyEmail;
                    hse_asset_register_model.CompanyName = WKPCompanyName;
                    hse_asset_register_model.COMPANY_ID = WKPCompanyId;
                    hse_asset_register_model.CompanyNumber = WKPCompanyNumber;
                    hse_asset_register_model.Date_Updated = DateTime.Now;
                    hse_asset_register_model.Updated_by = WKPCompanyId;
                    hse_asset_register_model.Year_of_WP = year;
                    hse_asset_register_model.OML_Name = omlName;
                    hse_asset_register_model.Field_ID = concessionField.Field_ID;
                    
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
                            _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs.Remove(getData);
                            await _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs.AddAsync(hse_asset_register_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_PRESCRIPTIVE_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_HSE_OIL_SPILL_REPORTING_NEW")]
        public async Task<WebApiResponse> POST_HSE_OIL_SPILL_REPORTING_NEW([FromBody] HSE_OIL_SPILL_REPORTING_NEW hse_oil_spill_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_OIL_SPILL_REPORTING_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_OIL_SPILL_REPORTING_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_oil_spill_model != null)
                {
                    var getData = (from c in _context.HSE_OIL_SPILL_REPORTING_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_oil_spill_model.Companyemail = WKPCompanyEmail;
                    hse_oil_spill_model.CompanyName = WKPCompanyName;
                    hse_oil_spill_model.COMPANY_ID = WKPCompanyId;
                    hse_oil_spill_model.CompanyNumber = WKPCompanyNumber;
                    hse_oil_spill_model.Date_Updated = DateTime.Now;
                    hse_oil_spill_model.Updated_by = WKPCompanyId;
                    hse_oil_spill_model.Year_of_WP = year;
                    hse_oil_spill_model.OML_Name = omlName;
                    hse_oil_spill_model.Field_ID = concessionField.Field_ID;
                    
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
                            _context.HSE_OIL_SPILL_REPORTING_NEWs.Remove(getData);
                            _context.SaveChanges();
                            await _context.HSE_OIL_SPILL_REPORTING_NEWs.AddAsync(hse_oil_spill_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_OIL_SPILL_REPORTING_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();


                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_OIL_SPILL_REPORTING_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW")]
        public async Task<WebApiResponse> POST_HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW([FromBody] HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEW hse_asset_register_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_asset_register_model != null)
                {
                    var getData = (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_asset_register_model.Companyemail = WKPCompanyEmail;
                    hse_asset_register_model.CompanyName = WKPCompanyName;
                    hse_asset_register_model.COMPANY_ID = WKPCompanyId;
                    hse_asset_register_model.CompanyNumber = WKPCompanyNumber;
                    hse_asset_register_model.Date_Updated = DateTime.Now;
                    hse_asset_register_model.Updated_by = WKPCompanyId;
                    hse_asset_register_model.Year_of_WP = year;
                    hse_asset_register_model.OML_Name = omlName;
                    hse_asset_register_model.Field_ID = concessionField.Field_ID;
                    
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
                            _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs.Remove(getData);
                            _context.SaveChanges();
                            await _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs.AddAsync(hse_asset_register_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_ASSET_REGISTER_TEMPLATE_RBI_EQUIPMENT_INSPECTION_STRATEGY_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_ACCIDENT_INCIDENCE")]
        public async Task<WebApiResponse> POST_HSE_ACCIDENT_INCIDENCE_REPORTING_NEW([FromBody] HSE_ACCIDENT_INCIDENCE_MODEL hse_accident_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            var HSE_Accident_Incidence = new HSE_ACCIDENT_INCIDENCE_REPORTING_NEW();
            var HSE_Accident_Incidence_Type = new HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW();
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else
                if (hse_accident_model != null)
                {
                    #region Saving HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs data
                    var getData = await (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();

                    HSE_Accident_Incidence.Was_there_any_accident_incidence = hse_accident_model.Was_there_any_accident_incidence;
                    HSE_Accident_Incidence.If_YES_were_they_reported = hse_accident_model.If_YES_were_they_reported;
                    HSE_Accident_Incidence.Companyemail = WKPCompanyEmail;
                    HSE_Accident_Incidence.CompanyName = WKPCompanyName;
                    HSE_Accident_Incidence.COMPANY_ID = WKPCompanyId;
                    HSE_Accident_Incidence.CompanyNumber = WKPCompanyNumber;
                    HSE_Accident_Incidence.Date_Updated = DateTime.Now;
                    HSE_Accident_Incidence.Updated_by = WKPCompanyId;
                    HSE_Accident_Incidence.Year_of_WP = year;
                    HSE_Accident_Incidence.OML_Name = omlName;
                    if (action == GeneralModel.Insert)
                    {
                        if (getData.Count() <= 0)
                        {
                            HSE_Accident_Incidence.Date_Created = DateTime.Now;
                            HSE_Accident_Incidence.Created_by = WKPCompanyId;
                            await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.AddAsync(HSE_Accident_Incidence);
                        }
                        else
                        {
                            HSE_Accident_Incidence.Date_Created = getData.FirstOrDefault().Date_Created;
                            HSE_Accident_Incidence.Created_by = getData.FirstOrDefault().Created_by;
                            HSE_Accident_Incidence.Date_Updated = DateTime.Now;
                            HSE_Accident_Incidence.Updated_by = WKPCompanyId;
                            getData.ForEach(x =>
                            {
                                _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.Remove(x);
                                save += _context.SaveChanges();
                            });
                            await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.AddAsync(HSE_Accident_Incidence);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        getData.ForEach(x =>
                        {
                            _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.Remove(x);
                            save += _context.SaveChanges();
                        });
                    }

                    save += await _context.SaveChangesAsync();

                    #endregion

                    #region Saving HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs data

                    var getData2 = await (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();

                    HSE_Accident_Incidence_Type.Cause = hse_accident_model.Cause;
                    HSE_Accident_Incidence_Type.Frequency = hse_accident_model.Frequency;
                    HSE_Accident_Incidence_Type.Type_of_Accident_Incidence = hse_accident_model.Type_of_Accident_Incidence;
                    HSE_Accident_Incidence_Type.Location = hse_accident_model.Location;
                    HSE_Accident_Incidence_Type.Investigation = hse_accident_model.Investigation;
                    HSE_Accident_Incidence_Type.Lesson_Learnt = hse_accident_model.Lesson_Learnt;
                    HSE_Accident_Incidence_Type.Consequence = hse_accident_model.Consequence;
                    HSE_Accident_Incidence_Type.Date_ = hse_accident_model.Date_;

                    HSE_Accident_Incidence_Type.Companyemail = WKPCompanyEmail;
                    HSE_Accident_Incidence_Type.CompanyName = WKPCompanyName;
                    HSE_Accident_Incidence_Type.COMPANY_ID = WKPCompanyId;
                    HSE_Accident_Incidence_Type.CompanyNumber = WKPCompanyNumber;
                    HSE_Accident_Incidence_Type.Date_Updated = DateTime.Now;
                    HSE_Accident_Incidence_Type.Updated_by = WKPCompanyId;
                    HSE_Accident_Incidence_Type.Year_of_WP = year;
                    HSE_Accident_Incidence_Type.OML_Name = omlName;
                    if (action == GeneralModel.Insert)
                    {
                        if (getData2.Count() <= 0)
                        {
                            HSE_Accident_Incidence_Type.Date_Created = DateTime.Now;
                            HSE_Accident_Incidence_Type.Created_by = WKPCompanyId;
                            await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.AddAsync(HSE_Accident_Incidence_Type);
                        }
                        else
                        {
                            HSE_Accident_Incidence_Type.Date_Created = getData2.FirstOrDefault().Date_Created;
                            HSE_Accident_Incidence_Type.Created_by = getData2.FirstOrDefault().Created_by;
                            HSE_Accident_Incidence_Type.Date_Updated = DateTime.Now;
                            HSE_Accident_Incidence_Type.Updated_by = WKPCompanyId;
                            getData2.ForEach(x =>
                            {
                                _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.Remove(x);
                                save += _context.SaveChanges();
                            });
                            await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.AddAsync(HSE_Accident_Incidence_Type);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        getData2.ForEach(x =>
                        {
                            _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.Remove(x);
                            save += _context.SaveChanges();
                        });
                    }

                    save += await _context.SaveChangesAsync();
                    #endregion


                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";

                    var All_Data = await (from a1 in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs
                                          join a2 in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs on a1.COMPANY_ID equals a2.COMPANY_ID
                                          where a1.COMPANY_ID == WKPCompanyId && a1.OML_Name == omlName && a1.Year_of_WP == year
                                          && a2.COMPANY_ID == WKPCompanyId && a2.OML_Name == omlName && a2.Year_of_WP == year

                                          select new HSE_ACCIDENT_INCIDENCE_MODEL
                                          {
                                              Was_there_any_accident_incidence = a1.Was_there_any_accident_incidence,
                                              If_YES_were_they_reported = a1.If_YES_were_they_reported,
                                              Cause = a2.Cause,
                                              Type_of_Accident_Incidence = a2.Type_of_Accident_Incidence,
                                              Consequence = a2.Consequence,
                                              Frequency = a2.Frequency,
                                              Investigation = a2.Investigation,
                                              Lesson_Learnt = a2.Lesson_Learnt,
                                              Location = a2.Location,
                                              Date_ = a2.Date_
                                          }).ToListAsync();


                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_ACCIDENT_INCIDENCE_REPORTING_NEW")]
        public async Task<WebApiResponse> POST_HSE_ACCIDENT_INCIDENCE_REPORTING_NEW([FromBody] HSE_ACCIDENT_INCIDENCE_REPORTING_NEW hse_accident_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_accident_model != null)
                {
                    var getData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_accident_model.Companyemail = WKPCompanyEmail;
                    hse_accident_model.CompanyName = WKPCompanyName;
                    hse_accident_model.COMPANY_ID = WKPCompanyId;
                    hse_accident_model.CompanyNumber = WKPCompanyNumber;
                    hse_accident_model.Date_Updated = DateTime.Now;
                    hse_accident_model.Updated_by = WKPCompanyId;
                    hse_accident_model.Year_of_WP = year;
                    hse_accident_model.OML_Name = omlName;
                    hse_accident_model.Field_ID = concessionField.Field_ID;
                    hse_accident_model.ACTUAL_year = year;
                    hse_accident_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.Remove(hse_accident_model);
                            await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.AddAsync(hse_accident_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW")]
        public async Task<WebApiResponse> POST_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW([FromBody] HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW hse_accident_reporting_model, string omlName, string fieldName,  string year, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                #region Saving HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs data
                if (hse_accident_reporting_model != null)
                {
                    var getData = (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_accident_reporting_model.Companyemail = WKPCompanyEmail;
                    hse_accident_reporting_model.CompanyName = WKPCompanyName;
                    hse_accident_reporting_model.COMPANY_ID = WKPCompanyId;
                    hse_accident_reporting_model.CompanyNumber = WKPCompanyNumber;
                    hse_accident_reporting_model.Date_Updated = DateTime.Now;
                    hse_accident_reporting_model.Updated_by = WKPCompanyId;
                    hse_accident_reporting_model.Year_of_WP = year;
                    hse_accident_reporting_model.OML_Name = omlName;
                    hse_accident_reporting_model.Field_ID = concessionField.Field_ID;
                    hse_accident_reporting_model.ACTUAL_year = year;
                    hse_accident_reporting_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.Remove(getData);
                            await _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.AddAsync(hse_accident_reporting_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW")]
        public async Task<WebApiResponse> POST_HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW([FromBody] HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEW hse_community_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_community_model != null)
                {
                    var getData = (from c in _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_community_model.Companyemail = WKPCompanyEmail;
                    hse_community_model.CompanyName = WKPCompanyName;
                    hse_community_model.COMPANY_ID = WKPCompanyId;
                    hse_community_model.CompanyNumber = WKPCompanyNumber;
                    hse_community_model.Date_Updated = DateTime.Now;
                    hse_community_model.Updated_by = WKPCompanyId;
                    hse_community_model.Year_of_WP = year;
                    hse_community_model.OML_Name = omlName;
                    hse_community_model.Field_ID = concessionField.Field_ID;
                    hse_community_model.ACTUAL_year = year;
                    hse_community_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs.Remove(getData);
                            await _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs.AddAsync(hse_community_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }


                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_COMMUNITY_DISTURBANCES_AND_OIL_SPILL_COST_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_ENVIRONMENTAL_STUDIES_NEW")]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_STUDIES_NEW([FromBody] HSE_ENVIRONMENTAL_STUDIES_NEW hse_environmental_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_ENVIRONMENTAL_STUDIES_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_environmental_model != null)
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_environmental_model.Companyemail = WKPCompanyEmail;
                    hse_environmental_model.CompanyName = WKPCompanyName;
                    hse_environmental_model.COMPANY_ID = WKPCompanyId;
                    hse_environmental_model.CompanyNumber = WKPCompanyNumber;
                    hse_environmental_model.Date_Updated = DateTime.Now;
                    hse_environmental_model.Updated_by = WKPCompanyId;
                    hse_environmental_model.Year_of_WP = year;
                    hse_environmental_model.OML_Name = omlName;
                    hse_environmental_model.Field_ID = concessionField.Field_ID;
                    hse_environmental_model.ACTUAL_year = year;
                    hse_environmental_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_ENVIRONMENTAL_STUDIES_NEWs.Remove(getData);
                            await _context.HSE_ENVIRONMENTAL_STUDIES_NEWs.AddAsync(hse_environmental_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_STUDIES_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_WASTE_MANAGEMENT_NEW")]
        public async Task<WebApiResponse> POST_HSE_WASTE_MANAGEMENT_NEW([FromBody] HSE_WASTE_MANAGEMENT_NEW hse_waste_management_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_WASTE_MANAGEMENT_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_WASTE_MANAGEMENT_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_waste_management_model != null)
                {
                    var getData = (from c in _context.HSE_WASTE_MANAGEMENT_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_waste_management_model.Companyemail = WKPCompanyEmail;
                    hse_waste_management_model.CompanyName = WKPCompanyName;
                    hse_waste_management_model.COMPANY_ID = WKPCompanyId;
                    hse_waste_management_model.CompanyNumber = WKPCompanyNumber;
                    hse_waste_management_model.Date_Updated = DateTime.Now;
                    hse_waste_management_model.Updated_by = WKPCompanyId;
                    hse_waste_management_model.Year_of_WP = year;
                    hse_waste_management_model.OML_Name = omlName;
                    hse_waste_management_model.Field_ID = concessionField.Field_ID;
                    hse_waste_management_model.ACTUAL_year = year;
                    hse_waste_management_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_WASTE_MANAGEMENT_NEWs.Remove(getData);
                            await _context.HSE_WASTE_MANAGEMENT_NEWs.AddAsync(hse_waste_management_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_WASTE_MANAGEMENT_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_WASTE_MANAGEMENT_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW")]
        public async Task<WebApiResponse> POST_HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW([FromBody] HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEW hse_waste_management_facility_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_waste_management_facility_model != null)
                {
                    var getData = (from c in _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_waste_management_facility_model.Companyemail = WKPCompanyEmail;
                    hse_waste_management_facility_model.CompanyName = WKPCompanyName;
                    hse_waste_management_facility_model.COMPANY_ID = WKPCompanyId;
                    hse_waste_management_facility_model.CompanyNumber = WKPCompanyNumber;
                    hse_waste_management_facility_model.Date_Updated = DateTime.Now;
                    hse_waste_management_facility_model.Updated_by = WKPCompanyId;
                    hse_waste_management_facility_model.Year_of_WP = year;
                    hse_waste_management_facility_model.OML_Name = omlName;
                    hse_waste_management_facility_model.Field_ID = concessionField.Field_ID;
                    hse_waste_management_facility_model.ACTUAL_year = year;
                    hse_waste_management_facility_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs.Remove(getData);
                            await _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs.AddAsync(hse_waste_management_facility_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_WASTE_MANAGEMENT_TYPE_OF_FACILITY_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_PRODUCED_WATER_MANAGEMENT_NEW")]
        public async Task<WebApiResponse> POST_HSE_PRODUCED_WATER_MANAGEMENT_NEW([FromBody] HSE_PRODUCED_WATER_MANAGEMENT_NEW hse_produced_water_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_produced_water_model != null)
                {
                    var getData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_produced_water_model.Companyemail = WKPCompanyEmail;
                    hse_produced_water_model.CompanyName = WKPCompanyName;
                    hse_produced_water_model.COMPANY_ID = WKPCompanyId;
                    hse_produced_water_model.CompanyNumber = WKPCompanyNumber;
                    hse_produced_water_model.Date_Updated = DateTime.Now;
                    hse_produced_water_model.Updated_by = WKPCompanyId;
                    hse_produced_water_model.Year_of_WP = year;
                    hse_produced_water_model.OML_Name = omlName;
                    hse_produced_water_model.Field_ID = concessionField.Field_ID;
                    hse_produced_water_model.ACTUAL_year = year;
                    hse_produced_water_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs.Remove(getData);
                            await _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs.AddAsync(hse_produced_water_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                    if (save > 0)
                    {
                        string successMsg = "Form has been " + action + "D successfully.";
                        var All_Data = await (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
                    }
                    else
                    {
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error : An error occured while trying to submit this form.", StatusCode = ResponseCodes.Failure };

                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                #endregion

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };

            }
        }

        [HttpPost("POST_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW")]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW([FromBody] HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEW hse_compliance_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_compliance_model != null)
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_compliance_model.Companyemail = WKPCompanyEmail;
                    hse_compliance_model.CompanyName = WKPCompanyName;
                    hse_compliance_model.COMPANY_ID = WKPCompanyId;
                    hse_compliance_model.CompanyNumber = WKPCompanyNumber;
                    hse_compliance_model.Date_Updated = DateTime.Now;
                    hse_compliance_model.Updated_by = WKPCompanyId;
                    hse_compliance_model.Year_of_WP = year;
                    hse_compliance_model.OML_Name = omlName;
                    hse_compliance_model.Field_ID = concessionField.Field_ID;
                    hse_compliance_model.ACTUAL_year = year;
                    hse_compliance_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs.Remove(getData);
                            await _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs.AddAsync(hse_compliance_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW")]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW([FromBody] HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEW hse_environmental_studies_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {


                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_environmental_studies_model != null)
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs where c.YEAR_ == hse_environmental_studies_model.YEAR_ && c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_environmental_studies_model.Companyemail = WKPCompanyEmail;
                    hse_environmental_studies_model.CompanyName = WKPCompanyName;
                    hse_environmental_studies_model.COMPANY_ID = WKPCompanyId;
                    hse_environmental_studies_model.CompanyNumber = WKPCompanyNumber;
                    hse_environmental_studies_model.Date_Updated = DateTime.Now;
                    hse_environmental_studies_model.Updated_by = WKPCompanyId;
                    hse_environmental_studies_model.Year_of_WP = year;
                    hse_environmental_studies_model.OML_Name = omlName;
                    hse_environmental_studies_model.Field_ID = concessionField.Field_ID;
                    hse_environmental_studies_model.ACTUAL_year = year;
                    hse_environmental_studies_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs.Remove(getData);
                            await _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs.AddAsync(hse_environmental_studies_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_STUDIES_FIVE_YEAR_STRATEGIC_PLAN_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTION"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTION([FromForm] HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTION hse_sustainable_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTIONs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTIONs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_sustainable_model != null)
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_sustainable_model.Companyemail = WKPCompanyEmail;
                    hse_sustainable_model.CompanyName = WKPCompanyName;
                    hse_sustainable_model.COMPANY_ID = WKPCompanyId;
                    hse_sustainable_model.CompanyNumber = WKPCompanyNumber;
                    hse_sustainable_model.Date_Updated = DateTime.Now;
                    hse_sustainable_model.Updated_by = WKPCompanyId;
                    hse_sustainable_model.Year_of_WP = year;
                    hse_sustainable_model.OML_Name = omlName;
                    hse_sustainable_model.Field_ID = concessionField.Field_ID;
                    
                    #region file section
                    var file1 = Request.Form.Files[0] != null ? Request.Form.Files[0] : null;
                    var file2 = Request.Form.Files[1] != null ? Request.Form.Files[1] : null;
                    var blobname1 = blobService.Filenamer(file1);
                    var blobname2 = blobService.Filenamer(file2);

                    if (file1 != null)
                    {
                        string docName = "MOU Responder";
                        hse_sustainable_model.MOUResponderFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"MOUResponderDocuments/{blobname1}");
                        if (hse_sustainable_model.MOUResponderFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_sustainable_model.MOUResponderFilename = blobname1;
                    }
                    if (file2 != null)
                    {
                        string docName = "OSCP";
                        hse_sustainable_model.MOUOSCPFilePath = await blobService.UploadFileBlobAsync("documents", file2.OpenReadStream(), file2.ContentType, $"MOUOSCPDocuments/{blobname2}");
                        if (hse_sustainable_model.MOUOSCPFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_sustainable_model.MOUOSCPFilename = blobname2;

                    }
                    #endregion

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_sustainable_model.Date_Created = DateTime.Now;
                            hse_sustainable_model.Created_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTIONs.AddAsync(hse_sustainable_model);
                        }
                        else
                        {
                            hse_sustainable_model.Date_Created = getData.Date_Created;
                            hse_sustainable_model.Created_by = getData.Created_by;
                            hse_sustainable_model.Date_Updated = DateTime.Now;
                            hse_sustainable_model.Updated_by = WKPCompanyId;
                            _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTIONs.Remove(getData);
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTIONs.AddAsync(hse_sustainable_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTIONs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_QUESTIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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
        [HttpPost("POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU([FromForm] HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOU hse_sustainable_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_sustainable_model != null)
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_sustainable_model.Companyemail = WKPCompanyEmail;
                    hse_sustainable_model.CompanyName = WKPCompanyName;
                    hse_sustainable_model.COMPANY_ID = WKPCompanyId;
                    hse_sustainable_model.CompanyNumber = WKPCompanyNumber;
                    hse_sustainable_model.Date_Updated = DateTime.Now;
                    hse_sustainable_model.Updated_by = WKPCompanyId;
                    hse_sustainable_model.Year_of_WP = year;
                    hse_sustainable_model.OML_Name = omlName;
                    hse_sustainable_model.Field_ID = concessionField.Field_ID;
                   
                    #region file section
                    var file1 = Request.Form.Files[0] != null ? Request.Form.Files[0] : null;
                    var blobname1 = blobService.Filenamer(file1);

                    if (file1 != null)
                    {
                        string docName = "GMOU";
                        hse_sustainable_model.MOUUploadFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"MOUDocuments/{blobname1}");
                        if (hse_sustainable_model.MOUUploadFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_sustainable_model.Date_Created = DateTime.Now;
                            hse_sustainable_model.Created_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs.AddAsync(hse_sustainable_model);
                        }
                        else
                        {
                            hse_sustainable_model.Date_Created = getData.Date_Created;
                            hse_sustainable_model.Created_by = getData.Created_by;
                            hse_sustainable_model.Date_Updated = DateTime.Now;
                            hse_sustainable_model.Updated_by = WKPCompanyId;
                            _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs.Remove(getData);
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs.AddAsync(hse_sustainable_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_MOUs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW")]
        public async Task<WebApiResponse> POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW([FromBody] HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW hse_sustainable_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_sustainable_model != null)
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_sustainable_model.Companyemail = WKPCompanyEmail;
                    hse_sustainable_model.CompanyName = WKPCompanyName;
                    hse_sustainable_model.COMPANY_ID = WKPCompanyId;
                    hse_sustainable_model.CompanyNumber = WKPCompanyNumber;
                    hse_sustainable_model.Date_Updated = DateTime.Now;
                    hse_sustainable_model.Updated_by = WKPCompanyId;
                    hse_sustainable_model.Year_of_WP = year;
                    hse_sustainable_model.OML_Name = omlName;
                    hse_sustainable_model.Field_ID = concessionField.Field_ID;
                    hse_sustainable_model.Actual_Proposed_Year= (int.Parse(year) + 1).ToString();

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_sustainable_model.Date_Created = DateTime.Now;
                            hse_sustainable_model.Created_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs.AddAsync(hse_sustainable_model);
                        }
                        else
                        {
                            hse_sustainable_model.Date_Created = getData.Date_Created;
                            hse_sustainable_model.Created_by = getData.Created_by;
                            hse_sustainable_model.Date_Updated = DateTime.Now;
                            hse_sustainable_model.Updated_by = WKPCompanyId;
                            _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs.Remove(getData);
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs.AddAsync(hse_sustainable_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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
        [HttpPost("POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL"), DisableRequestSizeLimitAttribute]
        public async Task<WebApiResponse> POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL([FromForm] HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUAL hse_sustainable_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {


                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_sustainable_model != null)
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_sustainable_model.Companyemail = WKPCompanyEmail;
                    hse_sustainable_model.CompanyName = WKPCompanyName;
                    hse_sustainable_model.COMPANY_ID = WKPCompanyId;
                    hse_sustainable_model.CompanyNumber = WKPCompanyNumber;
                    hse_sustainable_model.Date_Updated = DateTime.Now;
                    hse_sustainable_model.Updated_by = WKPCompanyId;
                    hse_sustainable_model.Year_of_WP = year;
                    hse_sustainable_model.OML_Name = omlName;
                    hse_sustainable_model.Field_ID = concessionField.Field_ID;
                    hse_sustainable_model.ACTUAL_year = year;
                    hse_sustainable_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs.Remove(getData);
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs.AddAsync(hse_sustainable_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_PLANNED_AND_ACTUALs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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
        [HttpPost("POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisition")]
        public async Task<WebApiResponse> POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisition([FromBody] HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisition hse_sustainable_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_sustainable_model != null)
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_sustainable_model.Companyemail = WKPCompanyEmail;
                    hse_sustainable_model.CompanyName = WKPCompanyName;
                    hse_sustainable_model.COMPANY_ID = WKPCompanyId;
                    hse_sustainable_model.CompanyNumber = WKPCompanyNumber;
                    hse_sustainable_model.Date_Updated = DateTime.Now;
                    hse_sustainable_model.Updated_by = WKPCompanyId;
                    hse_sustainable_model.Year_of_WP = year;
                    hse_sustainable_model.OML_Name = omlName;
                    hse_sustainable_model.Field_ID = concessionField.Field_ID;
                    hse_sustainable_model.Actual_Proposed_Year = (int.Parse(year) + 1).ToString();

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_sustainable_model.Date_Created = DateTime.Now;
                            hse_sustainable_model.Created_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions.AddAsync(hse_sustainable_model);
                        }
                        else
                        {
                            hse_sustainable_model.Date_Created = getData.Date_Created;
                            hse_sustainable_model.Created_by = getData.Created_by;
                            hse_sustainable_model.Date_Updated = DateTime.Now;
                            hse_sustainable_model.Updated_by = WKPCompanyId;
                            _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions.Remove(getData);
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions.AddAsync(hse_sustainable_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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
        [HttpPost("POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME"), DisableRequestSizeLimitAttribute]
        public async Task<WebApiResponse> POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME([FromForm] HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEME hse_sustainable_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEMEs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEMEs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_sustainable_model != null)
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEMEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_sustainable_model.Companyemail = WKPCompanyEmail;
                    hse_sustainable_model.CompanyName = WKPCompanyName;
                    hse_sustainable_model.COMPANY_ID = WKPCompanyId;
                    hse_sustainable_model.CompanyNumber = WKPCompanyNumber;
                    hse_sustainable_model.Date_Updated = DateTime.Now;
                    hse_sustainable_model.Updated_by = WKPCompanyId;
                    hse_sustainable_model.Year_of_WP = year;
                    hse_sustainable_model.OML_Name = omlName;
                    hse_sustainable_model.Field_ID = concessionField.Field_ID;
                   
                    #region file section
                    var file1 = Request.Form.Files[0] != null ? Request.Form.Files[0] : null;
                    var blobname1 = blobService.Filenamer(file1);

                    if (file1 != null)
                    {
                        string docName = "TS";
                        hse_sustainable_model.TSUploadFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"TSDocuments/{blobname1}");
                        if (hse_sustainable_model.TSUploadFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };

                    }
                    #endregion
                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_sustainable_model.Date_Created = DateTime.Now;
                            hse_sustainable_model.Created_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEMEs.AddAsync(hse_sustainable_model);
                        }
                        else
                        {
                            hse_sustainable_model.Date_Created = getData.Date_Created;
                            hse_sustainable_model.Created_by = getData.Created_by;
                            hse_sustainable_model.Date_Updated = DateTime.Now;
                            hse_sustainable_model.Updated_by = WKPCompanyId;
                            _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEMEs.Remove(getData);
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEMEs.AddAsync(hse_sustainable_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEMEs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_TRAINING_SCHEMEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarship")]
        public async Task<WebApiResponse> POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarship([FromBody] HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarship hse_sustainable_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_sustainable_model != null)
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_sustainable_model.Companyemail = WKPCompanyEmail;
                    hse_sustainable_model.CompanyName = WKPCompanyName;
                    hse_sustainable_model.COMPANY_ID = WKPCompanyId;
                    hse_sustainable_model.CompanyNumber = WKPCompanyNumber;
                    hse_sustainable_model.Date_Updated = DateTime.Now;
                    hse_sustainable_model.Updated_by = WKPCompanyId;
                    hse_sustainable_model.Year_of_WP = year;
                    hse_sustainable_model.OML_Name = omlName;
                    hse_sustainable_model.Field_ID = concessionField.Field_ID;
                    hse_sustainable_model.Actual_Proposed_Year = (int.Parse(year) + 1).ToString();

                    if (action == GeneralModel.Insert)
                    {
                        if (getData == null)
                        {
                            hse_sustainable_model.Date_Created = DateTime.Now;
                            hse_sustainable_model.Created_by = WKPCompanyId;
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships.AddAsync(hse_sustainable_model);
                        }
                        else
                        {
                            hse_sustainable_model.Date_Created = getData.Date_Created;
                            hse_sustainable_model.Created_by = getData.Created_by;
                            hse_sustainable_model.Date_Updated = DateTime.Now;
                            hse_sustainable_model.Updated_by = WKPCompanyId;
                            _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships.Remove(getData);
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships.AddAsync(hse_sustainable_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED")]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED([FromBody] HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATED hse_environmental_studies_new_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {


                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_environmental_studies_new_model != null)
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_environmental_studies_new_model.Companyemail = WKPCompanyEmail;
                    hse_environmental_studies_new_model.CompanyName = WKPCompanyName;
                    hse_environmental_studies_new_model.COMPANY_ID = WKPCompanyId;
                    hse_environmental_studies_new_model.CompanyNumber = WKPCompanyNumber;
                    hse_environmental_studies_new_model.Date_Updated = DateTime.Now;
                    hse_environmental_studies_new_model.Updated_by = WKPCompanyId;
                    hse_environmental_studies_new_model.Year_of_WP = year;
                    hse_environmental_studies_new_model.OML_Name = omlName;
                    hse_environmental_studies_new_model.Field_ID = concessionField.Field_ID;
                    
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
                            _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs.Remove(getData);
                            await _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs.AddAsync(hse_environmental_studies_new_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_STUDIES_NEW_UPDATEDs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_OSP_REGISTRATIONS_NEW")]
        public async Task<WebApiResponse> POST_HSE_OSP_REGISTRATIONS_NEW([FromBody] HSE_OSP_REGISTRATIONS_NEW hse_osp_registrations_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_OSP_REGISTRATIONS_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_OSP_REGISTRATIONS_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_osp_registrations_model != null)
                {
                    var getData = (from c in _context.HSE_OSP_REGISTRATIONS_NEWs where c.VALUES_ == hse_osp_registrations_model.VALUES_ && c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_osp_registrations_model.Companyemail = WKPCompanyEmail;
                    hse_osp_registrations_model.CompanyName = WKPCompanyName;
                    hse_osp_registrations_model.COMPANY_ID = WKPCompanyId;
                    hse_osp_registrations_model.CompanyNumber = WKPCompanyNumber;
                    hse_osp_registrations_model.Date_Updated = DateTime.Now;
                    hse_osp_registrations_model.Updated_by = WKPCompanyId;
                    hse_osp_registrations_model.Year_of_WP = year;
                    hse_osp_registrations_model.OML_Name = omlName;
                    hse_osp_registrations_model.Field_ID = concessionField.Field_ID;
                    
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
                            _context.HSE_OSP_REGISTRATIONS_NEWs.Remove(getData);
                            await _context.HSE_OSP_REGISTRATIONS_NEWs.AddAsync(hse_osp_registrations_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_OSP_REGISTRATIONS_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_OSP_REGISTRATIONS_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED")]
        public async Task<WebApiResponse> POST_HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED([FromBody] HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATED hse_produced_water_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_produced_water_model != null)
                {
                    var getData = (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_produced_water_model.Companyemail = WKPCompanyEmail;
                    hse_produced_water_model.CompanyName = WKPCompanyName;
                    hse_produced_water_model.COMPANY_ID = WKPCompanyId;
                    hse_produced_water_model.CompanyNumber = WKPCompanyNumber;
                    hse_produced_water_model.Date_Updated = DateTime.Now;
                    hse_produced_water_model.Updated_by = WKPCompanyId;
                    hse_produced_water_model.Year_of_WP = year;
                    hse_produced_water_model.OML_Name = omlName;
                    hse_produced_water_model.Field_ID = concessionField.Field_ID;
                    
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
                            _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.Remove(getData);
                            await _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.AddAsync(hse_produced_water_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW")]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW([FromBody] HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEW hse_chemical_usage_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_chemical_usage_model != null)
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_chemical_usage_model.Companyemail = WKPCompanyEmail;
                    hse_chemical_usage_model.CompanyName = WKPCompanyName;
                    hse_chemical_usage_model.COMPANY_ID = WKPCompanyId;
                    hse_chemical_usage_model.CompanyNumber = WKPCompanyNumber;
                    hse_chemical_usage_model.Date_Updated = DateTime.Now;
                    hse_chemical_usage_model.Updated_by = WKPCompanyId;
                    hse_chemical_usage_model.Year_of_WP = year;
                    hse_chemical_usage_model.OML_Name = omlName;
                    hse_chemical_usage_model.Field_ID = concessionField.Field_ID;
                    hse_chemical_usage_model.ACTUAL_year = year;
                    hse_chemical_usage_model.PROPOSED_year = (int.Parse(year) + 1).ToString();

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
                            _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs.Remove(getData);
                            await _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs.AddAsync(hse_chemical_usage_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_COMPLIANCE_MONITORING_CHEMICAL_USAGE_NEWs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_CAUSES_OF_SPILL")]
        public async Task<WebApiResponse> POST_HSE_CAUSES_OF_SPILL([FromBody] HSE_CAUSES_OF_SPILL hse_causes_of_spill_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_CAUSES_OF_SPILLs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_CAUSES_OF_SPILLs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_causes_of_spill_model != null)
                {
                    var getData = (from c in _context.HSE_CAUSES_OF_SPILLs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_causes_of_spill_model.Companyemail = WKPCompanyEmail;
                    hse_causes_of_spill_model.CompanyName = WKPCompanyName;
                    hse_causes_of_spill_model.COMPANY_ID = WKPCompanyId;
                    hse_causes_of_spill_model.CompanyNumber = WKPCompanyNumber;
                    hse_causes_of_spill_model.Date_Updated = DateTime.Now;
                    hse_causes_of_spill_model.Updated_by = WKPCompanyId;
                    hse_causes_of_spill_model.Year_of_WP = year;
                    hse_causes_of_spill_model.OML_Name = omlName;
                    hse_causes_of_spill_model.Field_ID = concessionField.Field_ID;
                   
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
                            _context.HSE_CAUSES_OF_SPILLs.Remove(getData);
                            await _context.HSE_CAUSES_OF_SPILLs.AddAsync(hse_causes_of_spill_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_CAUSES_OF_SPILLs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_CAUSES_OF_SPILLs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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


        [HttpPost("POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME([FromForm] HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEME hse_scholarship_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_scholarship_model != null)
                {
                    var getData = (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_scholarship_model.Companyemail = WKPCompanyEmail;
                    hse_scholarship_model.CompanyName = WKPCompanyName;
                    hse_scholarship_model.COMPANY_ID = WKPCompanyId;
                    hse_scholarship_model.CompanyNumber = WKPCompanyNumber;
                    hse_scholarship_model.Date_Updated = DateTime.Now;
                    hse_scholarship_model.Updated_by = WKPCompanyId;
                    hse_scholarship_model.Year_of_WP = year;
                    hse_scholarship_model.OML_Name = omlName;
                    hse_scholarship_model.Field_ID = concessionField.Field_ID;
                    
                    #region file section
                    var file1 = Request.Form.Files[0] != null ? Request.Form.Files[0] : null;
                    var blobname1 = blobService.Filenamer(file1);

                    if (file1 != null)
                    {
                        string docName = "SS";
                        hse_scholarship_model.SSUploadFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"SSDocuments/{blobname1}");
                        if (hse_scholarship_model.SSUploadFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_scholarship_model.SSUploadFilename = blobname1;
                    }

                    #endregion
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
                            _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs.Remove(getData);
                            await _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs.AddAsync(hse_scholarship_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_SCHOLASHIP_SCHEMEs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_MANAGEMENT_POSITION"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_HSE_MANAGEMENT_POSITION([FromForm] HSE_MANAGEMENT_POSITION hse_management_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_MANAGEMENT_POSITIONs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_MANAGEMENT_POSITIONs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_management_model != null)
                {
                    var getData = (from c in _context.HSE_MANAGEMENT_POSITIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();

                    hse_management_model.Companyemail = WKPCompanyEmail;
                    hse_management_model.CompanyName = WKPCompanyName;
                    hse_management_model.COMPANY_ID = WKPCompanyId;
                    hse_management_model.CompanyNumber = WKPCompanyNumber;
                    hse_management_model.Date_Updated = DateTime.Now;
                    hse_management_model.Updated_by = WKPCompanyId;
                    hse_management_model.Year_of_WP = year;
                    hse_management_model.OML_Name = omlName;
                    hse_management_model.Field_ID = concessionField.Field_ID;
                    
                    #region file section
                    var file1 = Request.Form.Files[0] != null ? Request.Form.Files[0] : null;
                    var file2 = Request.Form.Files[1] != null ? Request.Form.Files[1] : null;
                    var blobname1 = blobService.Filenamer(file1);
                    var blobname2 = blobService.Filenamer(file2);

                    if (file1 != null)
                    {
                        string docName = "Promotion Letter";
                        hse_management_model.PromotionLetterFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"HRDocuments/{blobname1}");
                        if (hse_management_model.PromotionLetterFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_management_model.PromotionLetterFilename = blobname1;
                    }
                    if (file2 != null)
                    {
                        string docName = "Organogram";
                        hse_management_model.OrganogramrFilePath = await blobService.UploadFileBlobAsync("documents", file2.OpenReadStream(), file2.ContentType, $"OGDocuments/{blobname2}");
                        if (hse_management_model.OrganogramrFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_management_model.OrganogramrFilename = blobname2;

                    }
                    #endregion

                    if (action == GeneralModel.Insert)
                    {
                        if (getData.Count <= 0)
                        {
                            hse_management_model.Date_Created = DateTime.Now;
                            hse_management_model.Created_by = WKPCompanyId;
                            await _context.HSE_MANAGEMENT_POSITIONs.AddAsync(hse_management_model);
                        }
                        else
                        {
                            hse_management_model.Date_Created = getData.FirstOrDefault().Date_Created;
                            hse_management_model.Created_by = getData.FirstOrDefault().Created_by;
                            hse_management_model.Date_Updated = DateTime.Now;
                            hse_management_model.Updated_by = WKPCompanyId;
                            getData.ForEach(x =>
                            {
                                _context.HSE_MANAGEMENT_POSITIONs.Remove(x);
                                save += _context.SaveChanges();

                            });
                            await _context.HSE_MANAGEMENT_POSITIONs.AddAsync(hse_management_model);
                        }
                    }

                    save += await _context.SaveChangesAsync();

                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_MANAGEMENT_POSITIONs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_SAFETY_CULTURE_TRAINING"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_HSE_SAFETY_CULTURE_TRAINING([FromForm] HSE_SAFETY_CULTURE_TRAINING hse_safety_culture_model, string year, string omlName, string fieldName,  string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_SAFETY_CULTURE_TRAININGs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_SAFETY_CULTURE_TRAININGs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_safety_culture_model != null)
                {
                    var getData = (from c in _context.HSE_SAFETY_CULTURE_TRAININGs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToList();

                    hse_safety_culture_model.Companyemail = WKPCompanyEmail;
                    hse_safety_culture_model.CompanyName = WKPCompanyName;
                    hse_safety_culture_model.COMPANY_ID = WKPCompanyId;
                    hse_safety_culture_model.CompanyNumber = WKPCompanyNumber;
                    hse_safety_culture_model.Date_Updated = DateTime.Now;
                    hse_safety_culture_model.Updated_by = WKPCompanyId;
                    hse_safety_culture_model.Year_of_WP = year;
                    hse_safety_culture_model.OML_Name = omlName;
                    hse_safety_culture_model.Field_ID = concessionField.Field_ID;
                    
                    #region file section
                    var file1 = Request.Form.Files[0];
                    var file2 = Request.Form.Files[1];
                    var blobname1 = blobService.Filenamer(file1);
                    var blobname2 = blobService.Filenamer(file2);

                    if (file1 != null)
                    {
                        string docName = "Safety Current Year";
                        hse_safety_culture_model.SafetyCurrentYearFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"SafetyCurrentYearDocuments/{blobname1}");
                        if (hse_safety_culture_model.SafetyCurrentYearFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_safety_culture_model.SafetyCurrentYearFilename = blobname1;
                    }
                    if (file2 != null)
                    {
                        string docName = "Safety Last Two Years";
                        hse_safety_culture_model.SafetyLast2YearsFilePath = await blobService.UploadFileBlobAsync("documents", file2.OpenReadStream(), file2.ContentType, $"SafetyLast2YearsDocuments/{blobname2}");
                        if (hse_safety_culture_model.SafetyLast2YearsFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_safety_culture_model.SafetyLast2YearsFilename = blobname2;
                    }
                    #endregion

                    if (action == GeneralModel.Insert)
                    {
                        if (getData.Count <= 0)
                        {
                            hse_safety_culture_model.Date_Created = DateTime.Now;
                            hse_safety_culture_model.Created_by = WKPCompanyId;
                            await _context.HSE_SAFETY_CULTURE_TRAININGs.AddAsync(hse_safety_culture_model);
                        }
                        else
                        {
                            hse_safety_culture_model.Date_Created = getData.FirstOrDefault().Date_Created;
                            hse_safety_culture_model.Created_by = getData.FirstOrDefault().Created_by;
                            hse_safety_culture_model.Date_Updated = DateTime.Now;
                            hse_safety_culture_model.Updated_by = WKPCompanyId;
                            hse_safety_culture_model.CompanyNumber = WKPCompanyNumber;
                            getData.ForEach(x =>
                            {
                                _context.HSE_SAFETY_CULTURE_TRAININGs.Remove(x);
                                save += _context.SaveChanges();

                            });
                            await _context.HSE_SAFETY_CULTURE_TRAININGs.AddAsync(hse_safety_culture_model);
                        }
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

                }

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_SAFETY_CULTURE_TRAININGs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_QUALITY_CONTROL"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_HSE_QUALITY_CONTROL([FromForm] HSE_QUALITY_CONTROL hse_quality_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_QUALITY_CONTROLs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_QUALITY_CONTROLs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_quality_model != null)
                {

                    var getData = (from c in _context.HSE_QUALITY_CONTROLs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_quality_model.Companyemail = WKPCompanyEmail;
                    hse_quality_model.CompanyName = WKPCompanyName;
                    hse_quality_model.COMPANY_ID = WKPCompanyId;
                    hse_quality_model.CompanyNumber = WKPCompanyNumber;
                    hse_quality_model.Date_Updated = DateTime.Now;
                    hse_quality_model.Updated_by = WKPCompanyId;
                    hse_quality_model.Year_of_WP = year;
                    hse_quality_model.OML_Name = omlName;
                    hse_quality_model.Field_ID = concessionField.Field_ID;
                   
                    #region file section
                    var file1 = Request.Form.Files[0];
                    var blobname1 = blobService.Filenamer(file1);

                    if (file1 != null)
                    {
                        string docName = "Quality Control";
                        hse_quality_model.QualityControlFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"COSDocuments/{blobname1}");
                        if (hse_quality_model.QualityControlFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_quality_model.QualityControlFilename = blobname1;
                    }
                    #endregion

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
                            _context.HSE_QUALITY_CONTROLs.RemoveRange(getData);
                            save += await _context.SaveChangesAsync();

                            await _context.HSE_QUALITY_CONTROLs.AddAsync(hse_quality_model);
                        }
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };

                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_QUALITY_CONTROLs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_CLIMATE_CHANGE_AND_AIR_QUALITY"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_HSE_CLIMATE_CHANGE_AND_AIR_QUALITY([FromForm] HSE_CLIMATE_CHANGE_AND_AIR_QUALITY hse_climate_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {

                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_climate_model != null)
                {
                    var getData = (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_climate_model.Companyemail = WKPCompanyEmail;
                    hse_climate_model.CompanyName = WKPCompanyName;
                    hse_climate_model.COMPANY_ID = WKPCompanyId;
                    hse_climate_model.CompanyNumber = WKPCompanyNumber;
                    hse_climate_model.Date_Updated = DateTime.Now;
                    hse_climate_model.Updated_by = WKPCompanyId;
                    hse_climate_model.Year_of_WP = year;
                    hse_climate_model.OML_Name = omlName;
                    hse_climate_model.Field_ID = concessionField.Field_ID;
                   
                    #region file section
                    var file1 = Request.Form.Files[0];
                    var blobname1 = blobService.Filenamer(file1);

                    if (file1 != null)
                    {
                        string docName = "GHG";
                        hse_climate_model.GHGFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"GHGDocuments/{blobname1}");
                        if (hse_climate_model.GHGFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_climate_model.GHGFilename = blobname1;
                    }

                    #endregion
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
                            _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.Remove(getData);
                            await _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.AddAsync(hse_climate_model);
                        }
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }

                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_OCCUPATIONAL_HEALTH_MANAGEMENT"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_HSE_OCCUPATIONAL_HEALTH_MANAGEMENT([FromForm] HSE_OCCUPATIONAL_HEALTH_MANAGEMENT hse_occupational_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_occupational_model != null)
                {
                    var getData = (from c in _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_occupational_model.Companyemail = WKPCompanyEmail;
                    hse_occupational_model.CompanyName = WKPCompanyName;
                    hse_occupational_model.COMPANY_ID = WKPCompanyId;
                    hse_occupational_model.CompanyNumber = WKPCompanyNumber;
                    hse_occupational_model.Date_Updated = DateTime.Now;
                    hse_occupational_model.Updated_by = WKPCompanyId;
                    hse_occupational_model.Year_of_WP = year;
                    hse_occupational_model.OML_Name = omlName;
                    hse_occupational_model.Field_ID = concessionField.Field_ID;
                  
                    #region file section
                    var file1 = Request.Form.Files[0];
                    var file2 = Request.Form.Files[1];
                    var blobname1 = blobService.Filenamer(file1);
                    var blobname2 = blobService.Filenamer(file2);

                    if (file1 != null)
                    {
                        string docName = "submission of OHM plan";
                        hse_occupational_model.OHMplanFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"FieldDiscoveryDocuments/{blobname1}");
                        if (hse_occupational_model.OHMplanFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_occupational_model.OHMplanFilename = blobname1;
                    }
                    if (file2 != null)
                    {
                        string docName = "submission of OHM plan";
                        hse_occupational_model.OHMplanCommunicationFilePath = await blobService.UploadFileBlobAsync("documents", file2.OpenReadStream(), file2.ContentType, $"FieldDiscoveryDocuments/{blobname2}");
                        if (hse_occupational_model.OHMplanCommunicationFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_occupational_model.OHMplanCommunicationFilename = blobname2;
                    }
                    #endregion

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
                            _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.Remove(getData);
                            await _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.AddAsync(hse_occupational_model);
                        }
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_WASTE_MANAGEMENT_SYSTEM"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_HSE_WASTE_MANAGEMENT_SYSTEM([FromForm] HSE_WASTE_MANAGEMENT_SYSTEM hse_waste_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {


                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_WASTE_MANAGEMENT_SYSTEMs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_WASTE_MANAGEMENT_SYSTEMs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_waste_model != null)
                {
                    var getData = (from c in _context.HSE_WASTE_MANAGEMENT_SYSTEMs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_waste_model.Companyemail = WKPCompanyEmail;
                    hse_waste_model.CompanyName = WKPCompanyName;
                    hse_waste_model.COMPANY_ID = WKPCompanyId;
                    hse_waste_model.CompanyNumber = WKPCompanyNumber;
                    hse_waste_model.Date_Updated = DateTime.Now;
                    hse_waste_model.Updated_by = WKPCompanyId;
                    hse_waste_model.Year_of_WP = year;
                    hse_waste_model.OML_Name = omlName;
                    hse_waste_model.Field_ID = concessionField.Field_ID;
                   
                    #region file section
                    var file1 = Request.Form.Files[0] != null ? Request.Form.Files[0] : null;
                    var file2 = Request.Form.Files[1] != null ? Request.Form.Files[1] : null;
                    var blobname1 = blobService.Filenamer(file1);
                    var blobname2 = blobService.Filenamer(file2);

                    if (file1 != null)
                    {
                        string docName = "Decom Certificate";
                        hse_waste_model.DecomCertificateFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"DecomCertificateDocuments/{blobname1}");
                        if (hse_waste_model.DecomCertificateFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_waste_model.DecomCertificateFilename = blobname1;
                    }
                    if (file2 != null)
                    {
                        string docName = "Waste Management Plan";
                        hse_waste_model.WasteManagementPlanFilePath = await blobService.UploadFileBlobAsync("documents", file2.OpenReadStream(), file2.ContentType, $"WasteManagementPlanDocuments/{blobname2}");
                        if (hse_waste_model.WasteManagementPlanFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_waste_model.WasteManagementPlanFilename = blobname1;
                    }

                    #endregion

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
                            _context.HSE_WASTE_MANAGEMENT_SYSTEMs.Remove(getData);
                            await _context.HSE_WASTE_MANAGEMENT_SYSTEMs.AddAsync(hse_waste_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_WASTE_MANAGEMENT_SYSTEMs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_WASTE_MANAGEMENT_SYSTEMs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM([FromForm] HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEM hse_EMS_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (hse_EMS_model != null)
                {
                    var getData = (from c in _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    hse_EMS_model.Companyemail = WKPCompanyEmail;
                    hse_EMS_model.CompanyName = WKPCompanyName;
                    hse_EMS_model.COMPANY_ID = WKPCompanyId;
                    hse_EMS_model.CompanyNumber = WKPCompanyNumber;
                    hse_EMS_model.Date_Updated = DateTime.Now;
                    hse_EMS_model.Updated_by = WKPCompanyId;
                    hse_EMS_model.Year_of_WP = year;
                    hse_EMS_model.OML_Name = omlName;
                    hse_EMS_model.Field_ID = concessionField.Field_ID;
                    
                    #region file section
                    var file1 = Request.Form.Files[0] != null ? Request.Form.Files[0] : null;
                    var file2 = Request.Form.Files[1] != null ? Request.Form.Files[1] : null;
                    var blobname1 = blobService.Filenamer(file1);
                    var blobname2 = blobService.Filenamer(file2);

                    if (file1 != null)
                    {
                        string docName = "EMS";
                        hse_EMS_model.EMSFilePath = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"EMSDocuments/{blobname1}");
                        if (hse_EMS_model.EMSFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_EMS_model.EMSFilename = blobname1;
                    }
                    if (file2 != null)
                    {
                        string docName = "Audit File";
                        hse_EMS_model.AUDITFilePath = await blobService.UploadFileBlobAsync("documents", file2.OpenReadStream(), file2.ContentType, $"AUDITDocuments/{blobname2}");
                        if (hse_EMS_model.AUDITFilePath == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                        else
                            hse_EMS_model.AUDITFilename = blobname1;
                    }
                    #endregion

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
                            _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs.Remove(getData);
                            await _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs.AddAsync(hse_EMS_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.HSE_ENVIRONMENTAL_MANAGEMENT_SYSTEMs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpPost("POST_PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT"), DisableRequestSizeLimit]
        public async Task<WebApiResponse> POST_PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT([FromForm] PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECT picture_upload_model, string omlName, string fieldName,  string year, string id, string actionToDo)
        {

            int save = 0;
            string action = actionToDo == null ? GeneralModel.Insert : actionToDo; var concessionField = GET_CONCESSION_FIELD(omlName, fieldName);

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var getData = (from c in _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs where c.Id == int.Parse(id) select c).FirstOrDefault();

                    if (action == GeneralModel.Delete)
                        _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs.Remove(getData);
                    save += _context.SaveChanges();
                }
                else if (picture_upload_model != null)
                {
                    var getData = (from c in _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).FirstOrDefault();

                    picture_upload_model.Companyemail = WKPCompanyEmail;
                    picture_upload_model.CompanyName = WKPCompanyName;
                    picture_upload_model.COMPANY_ID = WKPCompanyId;
                    picture_upload_model.CompanyNumber = WKPCompanyNumber;
                    picture_upload_model.Date_Updated = DateTime.Now;
                    picture_upload_model.Updated_by = WKPCompanyId;
                    picture_upload_model.Year_of_WP = year;
                    picture_upload_model.OML_Name = omlName;
                    picture_upload_model.Field_ID = concessionField.Field_ID;
                    
                    #region file section
                    var file1 = Request.Form.Files[0] != null ? Request.Form.Files[0] : null;
                    var blobname1 = blobService.Filenamer(file1);

                    if (file1 != null)
                    {
                        string docName = "Uploaded Presentation";
                        picture_upload_model.uploaded_presentation = await blobService.UploadFileBlobAsync("documents", file1.OpenReadStream(), file1.ContentType, $"Presentations/{blobname1}");
                        if (picture_upload_model.uploaded_presentation == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : An error occured while trying to upload " + docName + " document.", StatusCode = ResponseCodes.Badrequest };
                    }
                    #endregion

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
                            _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs.Remove(getData);
                            await _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs.AddAsync(picture_upload_model);
                        }
                    }
                    else if (action == GeneralModel.Delete)
                    {
                        _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs.Remove(getData);
                    }

                    save += await _context.SaveChangesAsync();
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = $"Error : No data was passed for {actionToDo} process to be completed.", StatusCode = ResponseCodes.Failure };
                }
                if (save > 0)
                {
                    string successMsg = "Form has been " + action + "D successfully.";
                    var All_Data = await (from c in _context.PICTURE_UPLOAD_COMMUNITY_DEVELOPMENT_PROJECTs where c.OML_Name == omlName && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year select c).ToListAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = successMsg, Data = All_Data, StatusCode = ResponseCodes.Success };
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

        [HttpGet("GET_WPYEAR_LIST")]
        public async Task<object> Get_WPYear_List()
        {
            return await (from a in _context.ADMIN_CONCESSIONS_INFORMATIONs select a.Year).Distinct().ToListAsync();
        }

        #endregion

    }
}