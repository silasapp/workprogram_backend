using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Backend_UMR_Work_Program.Controllers
{
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
    private Account _account;
    public WKP_DBContext _context;
    public IConfiguration _configuration;
    HelpersController _helpersController;
    IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

        public ReportController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController, Account account, IMapper mapper)
    {
        _httpContextAccessor = _httpContextAccessor;
        _account = account;
        _context = context;
        _configuration = configuration;
         _mapper = mapper;
        _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor, _mapper);
    }
        //private string? WKPCompanyId => "221";
        //private string? WKPCompanyName => "Name";
        //private string? WKPCompanyEmail => "adeola.kween123@gmail.com";
        //private string? WKUserRole => "Admin";

        private string? WKPCompanyId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string? WKPCompanyName => User.FindFirstValue(ClaimTypes.Name);
        private string? WKPCompanyEmail => User.FindFirstValue(ClaimTypes.Email);
        private string? WKUserRole => User.FindFirstValue(ClaimTypes.Role);


        #region General Report Section
        [HttpGet("GENERAL_WORKPROGRAMME_REPORT")]
        public async Task<WebApiResponse> GENERAL_WORKPROGRAMME(string year)
        {
            try
            {

                WorkProgrammeReport_Model GeneralReport = new WorkProgrammeReport_Model();

                WorkProgrammeReport1_Model WorkProgrammeReport = Get_General_SummaryReport(year);
                WorkProgrammeReport2_Model WorkProgrammeReport2 = Get_General_Report(year);

                GeneralReport.WorkProgrammeReport1_Model = WorkProgrammeReport;
                GeneralReport.WorkProgrammeReport2_Model = WorkProgrammeReport2;

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GeneralReport, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }
        [HttpGet("GENERAL_SUMMARYREPORT")]
        public async Task<WebApiResponse> GENERAL_SUMMARYREPORT(string year)
        {
            try
            {

                WorkProgrammeReport1_Model WorkProgrammeReport = Get_General_SummaryReport(year);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = WorkProgrammeReport, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }
        [HttpGet("GENERAL_REPORT")]
        public async Task<WebApiResponse> GENERAL_REPORT(string year)
        {
            try
            {

                WorkProgrammeReport2_Model WorkProgrammeReport2 = Get_General_Report(year);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = WorkProgrammeReport2, StatusCode = ResponseCodes.Success };

            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }
        [HttpGet("Get_General_SummaryReport")]
        public WorkProgrammeReport1_Model Get_General_SummaryReport(string year)
        {
            WorkProgrammeReport1_Model WorkProgrammeReport = new WorkProgrammeReport1_Model();

            try
            {
                var ADMIN_WORK_PROGRAM_REPORTs_Model= new List<ADMIN_WORK_PROGRAM_REPORT>();
  

                string previousYear = year !=null ? (int.Parse(year) - 1).ToString(): "";

                var WKP_Report = _context.ADMIN_WORK_PROGRAM_REPORTs.Where(x => x.Id <= 5).ToList();
                var Seismic_Data_Acquisition_Activities = WKP_Report.Where(x => x.Id == 2).FirstOrDefault();
                if (year != null)
                {
                    var WP_COUNT = _context.WP_COUNT_ADMIN_DATETIME_PRESENTATION_BY_YEAR_PRESENTED_CATEGORies.Where(x => x.Year == year).ToList();
                    var E_and_P_companies = _context.WP_COUNT_ADMIN_DATETIME_PRESENTATION_BY_TOTAL_COUNT_YEARLies.Where(x => x.YEAR == year).ToList();

                    
                    var WP_COUNT_GEOPHYSICAL_ACTIVITIES_ACQUISITION = (from o in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs
                                                                      where o.Year_of_WP == year && o.Geo_type_of_data_acquired == GeneralModel.ThreeD
                                                                      group o by new
                                                                      {
                                                                          o.Geo_type_of_data_acquired, o.Year_of_WP
                                                                      }
                                                                        into g
                                                                      select new WP_GEOPHYSICAL_ACTIVITIES_ACQUISITION
                                                                      {
                                                                          Geo_type_of_data_acquired = g.FirstOrDefault().Geo_type_of_data_acquired,
                                                                          Actual_year_aquired_data = g.Sum(x => Convert.ToInt32(Convert.ToDouble(x.Actual_year_aquired_data))),
                                                                          Year_of_WP = g.FirstOrDefault().Year_of_WP,

                                                                      }).ToList();

                    
                    var WP_COUNT_GEOPHYSICAL_ACTIVITIES_PROCESSING = _context.WP_GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(x => x.Year_of_WP == year && (x.Geo_Type_of_Data_being_Processed == GeneralModel.ThreeD || x.Geo_Type_of_Data_being_Processed == GeneralModel.TwoD)).ToList();


                    var WP_COUNT_WELLS = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && (x.Category == GeneralModel.Exploration || x.Category == GeneralModel.Development)).ToList();

                    var WP_SUM_APPRAISAL_WELLS = (from o in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs
                                                  where o.Category.Contains(GeneralModel.Appraisal) && o.Year_of_WP == year
                                                  group o by new
                                                  {
                                                      o.Category
                                                  }
                                                    into g
                                                  select new DRILLING_OPERATIONS_CATEGORIES_OF_WELL
                                                  {
                                                      Actual_No_Drilled_in_Current_Year = g.Sum(x => Convert.ToInt32(x.Actual_No_Drilled_in_Current_Year)).ToString(),
                                                      Proposed_No_Drilled = g.Sum(x => Convert.ToInt32(x.Proposed_No_Drilled)).ToString(),
                                                      Year_of_WP = g.FirstOrDefault().Year_of_WP,

                                                  }).ToList();


                    var WP_SUM_WORKOVERS_RECOMPLETION = _context.WP_SUM_INITIAL_WELL_COMPLETION_JOBS_WORKOVERS_RECOMPLETIONs.Where(x => x.Year_of_WP_I == year).ToList();

                    var WP_COUNT_Appraisal = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category.Contains(GeneralModel.Appraisal)).ToList();

                    var WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES =  (from u in _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Total_reconciled_crude_oils
                                                                    where u.Year_of_WP == year
                                                                    select u).ToList();

                    var WP_JV_Contract_Type =  (from u in _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Contract_Types
                                               where u.Year_of_WP == previousYear /*&& u.Contract_Type == GeneralModel.JV*/
                                               select u).GroupBy(x => x.Contract_Type).Select(x => x.FirstOrDefault()).ToList();

                    var WP_GAS_PRODUCTION_ACTIVITIES_Percentages =  (from u in _context.WP_GAS_PRODUCTION_ACTIVITIES_Percentages
                                                                     where u.Year_of_WP == previousYear select u).ToList();
                    var WP_GAS_PRODUCTION_ACTIVITIES_Percentages_CY =  (from u in _context.WP_GAS_PRODUCTION_ACTIVITIES_Percentages
                                                                     where u.Year_of_WP == year select u).ToList();
                    
                    var WP_GAS_PRODUCTION_ACTIVITIES_Contract_Type =  (from u in _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types
                                                                       where u.Year_of_WP == previousYear select u).ToList();



                    var WP_CRUDE_OIL = (from u in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs where u.Year_of_WP == previousYear || u.Year_of_WP == year
                                        group u by new{ u.Year_of_WP } into g select new {
                                                         Year = g.Key,
                                                         Total_Reconciled_National_Crude_Oil_Production = g.Sum(c=> Convert.ToInt64(Convert.ToDouble(c.Total_Reconciled_National_Crude_Oil_Production))),
                                                     }).ToList();
                    var WP_CRUDE_OIL_PY = WP_CRUDE_OIL.Where(x => x.Year.ToString() == previousYear).ToList();
                    var WP_CRUDE_OIL_CY = WP_CRUDE_OIL.Where(x => x.Year.ToString() == year).ToList();
                    
                    var WP_OIL_PRODUCTION_total_barrel = (from u in _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPEs
                                                          where u.Year_of_WP == year select u).GroupBy(x => x.Contract_Type).Select(x => x.FirstOrDefault()).ToList();

                    var WP_Terrain_Continental = (from u in _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrains
                                                  where u.Year_of_WP == year select u).GroupBy(x => x.Terrain).Select(x => x.FirstOrDefault()).ToList();
                    #region Gas Production Activities For Previous Year & Current Year
                    
                     var GAS_ACTIVITIES = (from u in _context.GAS_PRODUCTION_ACTIVITIEs
                                             select u).ToList();

                    var GAS_ACTIVITIES_YEAR = (from u in _context.GAS_PRODUCTION_ACTIVITIEs
                                             select u).GroupBy(x=> x.Year_of_WP).Select(x=> x.FirstOrDefault()).ToList();

                    var WP_GAS_ACTIVITIES = (from g in GAS_ACTIVITIES_YEAR
                                             select new
                                             {
                                                 Actual_Total_Gas_Produced = Convert.ToInt64(g.Current_Actual_Year),
                                                 Utilized_Gas_Produced = Convert.ToInt64(g.Utilized),
                                                 Flared_Gas_Produced = Convert.ToInt64(Convert.ToDouble(g.Flared)),
                                                 Year_of_WP = g.Year_of_WP,
                                                 Percentage_Utilized= (Convert.ToDecimal(g.Utilized)/Convert.ToDecimal(g.Actual_year))*100
                                             }).ToList();

                    var PY_GAS_ACTIVITIES = WP_GAS_ACTIVITIES.Where(x=> x.Year_of_WP.ToString() == previousYear).ToList();
                    var CY_GAS_ACTIVITIES = WP_GAS_ACTIVITIES.Where(x=> x.Year_of_WP.ToString() == year).ToList();

                    var GAS_ACTIVITIES_CONTRACTTYPES = (from u in _context.GAS_PRODUCTION_ACTIVITIEs
                                               select u).GroupBy(x => x.Contract_Type).Select(x => x.FirstOrDefault()).ToList();

                  
                    #endregion

                    var OIL_CONDENSATE_MMBBL = _context.WP_RESERVES_UPDATES_OIL_CONDENSATE_MMBBLs.Where(x => x.Year_of_WP == year).ToList();

                    #region HSE Accident
                    var HSE_ACCIDENT_Consequences = _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_by_consequences.Where(x => x.Year_of_WP == year && x.Consequence == GeneralModel.Fatality).ToList();
                    //string Sum_accident = HSE_ACCIDENT_Consequences.sum_accident.ToString();

                    var HSE_ACCIDENT_Total = _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_accidents.Where(x => x.Year_of_WP == year).ToList();
                    //string Sum_accident_total = HSE_ACCIDENT_Total.Sum_accident.ToString();

                    var HSE_ACCIDENT = (from u in _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_spill_accident_and_percentages
                                        where u.Year_of_WP == year
                                        select new WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_spill_accident_and_percentage
                                        {
                                            Cause= u.Cause,
                                            sum_accident = u.sum_accident,
                                            Percentage_Spill = u.Percentage_Spill,
                                        }).GroupBy(x => x.Cause).Select(x => x.FirstOrDefault()).ToList();

                    #endregion
                    var GEO_ACTIVITIES = (from u in _context.WP_GEOPHYSICAL_ACTIVITIES_ACQUISITION_sum_and_counts
                                          where u.Year_of_WP == year
                                          select u).GroupBy(x => x.Contract_Type).Select(x => x.FirstOrDefault()).ToList();

                var HSE_VOLUME_OF_OILSPILL = (from o in _context.HSE_OIL_SPILL_REPORTING_NEWs
                                                         where o.Year_of_WP == year
                                                         select o
                                                             ).ToList().Sum(x => int.Parse(x.Volume_of_spill_bbls));

                var OILSPILL_REPORT = (from o in _context.WP_TOTAL_INCIDENCE_AND_OIL_SPILL_AND_RECOVEREDs
                                            where o.Year_of_WP == year
                                            group o by new
                                            {
                                                o.CompanyName
                                            }
                                            into g select new
                                            {
                                                Frequency = g.Min(x => x.Frequency),
                                                Highest_1st = g.Min(x => x.Frequency),
                                                Highest_2nd = g.Min(x => x.Frequency),
                                                Highest_3rd = g.Min(x => x.Frequency),
                                                TOTAL_QUANTITY_SPILLED = g.Sum(x => x.Total_Quantity_Spilled),
                                                CompanyName = g.Key,
                                                Year_of_WP = g.FirstOrDefault().Year_of_WP,
                                            }).OrderBy(x => x.Frequency).Take(5).ToList();
                    
                    #region GENERAL REPORT DATA POPULATION
                    var get_ReportContent_1 = WKP_Report.Where(x => x.Id == 1)?.FirstOrDefault();
                    var get_ReportContent_2 = WKP_Report.Where(x => x.Id == 2)?.FirstOrDefault();
                    if (get_ReportContent_1 != null)
                    {
                        string NO_OF_COMPANY_PRESENTED = WP_COUNT.Where(x => x.PRESENTED == GeneralModel.Presented).Count().ToString();
                        string SHOWED_UP = WP_COUNT.Where(x => x.PRESENTED == GeneralModel.ShowedButNoPresentation).Count().ToString();
                        string FAIL_TO_SHOW_UP = WP_COUNT.Where(x => x.PRESENTED == GeneralModel.FailedToShow).Count().ToString();
                        string NOT_INVITED = WP_COUNT.Where(x => x.PRESENTED == GeneralModel.NotInvited).Count().ToString();


                        get_ReportContent_1.Report_Content = get_ReportContent_1.Report_Content.Replace("(NO_OF_COMPANY_PRESENTED)", NO_OF_COMPANY_PRESENTED)
                            .Replace("(NO_OF_EP_COMPANIES)", E_and_P_companies.Count().ToString())
                            .Replace("(N)", year)
                            .Replace("(N + 1)", (int.Parse(year) + 1).ToString())
                            .Replace("(N - 1)", previousYear)
                            .Replace("(SHOWED_UP)", SHOWED_UP).Replace("(FAIL_TO_SHOW_UP)", SHOWED_UP).Replace("(NOT_INVITED)", NOT_INVITED)
                            .Replace("(ACQUIRED_3D)", WP_COUNT_GEOPHYSICAL_ACTIVITIES_ACQUISITION?.FirstOrDefault()?.Actual_year_aquired_data.ToString())
                            .Replace("(PROCESSED_3D)", WP_COUNT_GEOPHYSICAL_ACTIVITIES_PROCESSING.Where(x => x.Geo_Type_of_Data_being_Processed == GeneralModel.ThreeD)?.FirstOrDefault()?.Processed_Actual.ToString())
                            .Replace("(REPROCESSED_3D)", WP_COUNT_GEOPHYSICAL_ACTIVITIES_PROCESSING.Where(x => x.Geo_Type_of_Data_being_Processed == GeneralModel.ThreeD)?.FirstOrDefault()?.Reprocessed_Actual.ToString())
                            .Replace("(PROPOSED_ACQUIRED_3D)", WP_COUNT_GEOPHYSICAL_ACTIVITIES_ACQUISITION?.FirstOrDefault()?.Geo_type_of_data_acquired.ToString())
                            .Replace("(PROPOSED_PROCESS_2D)", WP_COUNT_GEOPHYSICAL_ACTIVITIES_PROCESSING.Where(x => x.Geo_Type_of_Data_being_Processed == GeneralModel.TwoD)?.FirstOrDefault()?.Processed_Proposed.ToString())
                            .Replace("(PROPOSED_PROCESS_3D)", WP_COUNT_GEOPHYSICAL_ACTIVITIES_PROCESSING.Where(x => x.Geo_Type_of_Data_being_Processed == GeneralModel.ThreeD)?.FirstOrDefault()?.Processed_Proposed.ToString())
                            .Replace("(PROPOSED_REPROCESSED_3D)", WP_COUNT_GEOPHYSICAL_ACTIVITIES_PROCESSING.Where(x => x.Geo_Type_of_Data_being_Processed == GeneralModel.ThreeD)?.FirstOrDefault()?.Reprocessed_Proposed.ToString())
                            .Replace("(NO_EXPLORATION_WELLS)", WP_COUNT_WELLS.Where(x => x.Category == GeneralModel.Exploration)?.FirstOrDefault()?.Actual_No_Drilled_in_Current_Year.ToString())
                            .Replace("(NO_PROPOSED_EXPLORATION_WELLS)", WP_COUNT_WELLS.Where(x => x.Category == GeneralModel.Exploration)?.FirstOrDefault()?.Proposed_No_Drilled.ToString())
                            .Replace("(NO_FIRST_APPRAISALS)", WP_COUNT_WELLS.Where(x => x.Category == GeneralModel.FirstAppraisal)?.FirstOrDefault()?.Actual_No_Drilled_in_Current_Year.ToString())
                            .Replace("(NO_PROPOSED_FIRST_APPRAISALS)", WP_COUNT_WELLS.Where(x => x.Category == GeneralModel.FirstAppraisal)?.FirstOrDefault()?.Proposed_No_Drilled.ToString())
                            .Replace("(NO_SECOND_APPRAISALS)", WP_COUNT_WELLS.Where(x => x.Category == GeneralModel.SecondAppraisal)?.FirstOrDefault()?.Actual_No_Drilled_in_Current_Year.ToString())
                            .Replace("(NO_PROPOSED_SECOND_APPRAISALS)", WP_COUNT_WELLS.Where(x => x.Category == GeneralModel.SecondAppraisal)?.FirstOrDefault()?.Proposed_No_Drilled.ToString())
                            .Replace("(NO_ORDINARY_APPRAISALS)", WP_COUNT_WELLS.Where(x => x.Category == GeneralModel.OrdinaryAppraisal)?.FirstOrDefault()?.Actual_No_Drilled_in_Current_Year.ToString())
                            .Replace("(NO_PROPOSED_ORDINARY_APPRAISALS)", WP_COUNT_WELLS.Where(x => x.Category == GeneralModel.OrdinaryAppraisal)?.FirstOrDefault()?.Proposed_No_Drilled.ToString())
                            .Replace("(NO_DEVELOPMENT_WELLS)", WP_COUNT_WELLS.Where(x => x.Category == GeneralModel.Development)?.FirstOrDefault()?.Actual_No_Drilled_in_Current_Year.ToString())
                            .Replace("(NO_PROPOSED_DEVELOPMENT_WELLS)", WP_COUNT_WELLS.Where(x => x.Category == GeneralModel.Development)?.FirstOrDefault()?.Proposed_No_Drilled.ToString())
                            .Replace("(NO_COMPLETION_WORKOVER)", WP_SUM_WORKOVERS_RECOMPLETION.Where(x => x.Year_of_WP_I == year)?.FirstOrDefault()?.Actual_Year.ToString())
                            .Replace("(NO_PROPOSED_COMPLETION_WORKOVER)", WP_SUM_WORKOVERS_RECOMPLETION.Where(x => x.Year_of_WP_I == year)?.FirstOrDefault()?.Proposed_Year.ToString())
                            .Replace("(PREVIOUS_NO_TOTAL_RECONCILED_NATIONAL_CRUDE)", WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES?.FirstOrDefault()?.Total_Reconciled_National_Crude_Oil_Production.ToString())
                            .Replace("(NO_TOTAL_RECONCILED_NATIONAL_CRUDE_JV)", WP_JV_Contract_Type.Where(x => x.Contract_Type == GeneralModel.JV)?.FirstOrDefault()?.Total_Reconciled_National_Crude_Oil_Production.ToString())
                            .Replace("(NO_TOTAL_RECONCILED_NATIONAL_CRUDE_PSC)", WP_JV_Contract_Type.Where(x => x.Contract_Type == GeneralModel.PSC)?.FirstOrDefault()?.Total_Reconciled_National_Crude_Oil_Production.ToString())
                            .Replace("(NO_TOTAL_RECONCILED_NATIONAL_CRUDE_SR)", WP_JV_Contract_Type.Where(x => x.Contract_Type == GeneralModel.SR)?.FirstOrDefault()?.Total_Reconciled_National_Crude_Oil_Production.ToString())
                            .Replace("(NO_TOTAL_RECONCILED_NATIONAL_CRUDE_MF)", WP_JV_Contract_Type.Where(x => x.Contract_Type == GeneralModel.MF)?.FirstOrDefault()?.Total_Reconciled_National_Crude_Oil_Production.ToString())
                            .Replace("(NO_TOTAL_RECONCILED_NATIONAL_CRUDE_YEAR_TO_DATE)", WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES?.FirstOrDefault()?.Total_Reconciled_National_Crude_Oil_Production.ToString())
                            .Replace("(NO_TOTAL_GAS_PRODUCED)", WP_GAS_PRODUCTION_ACTIVITIES_Percentages?.FirstOrDefault()?.Actual_Total_Gas_Produced.ToString())
                            .Replace("(NO_TOTAL_GAS_UTILIZED)", WP_GAS_PRODUCTION_ACTIVITIES_Percentages?.FirstOrDefault()?.Utilized_Gas_Produced.ToString())
                            .Replace("(PERCENTAGE_TOTAL_GAS_UTILIZED)", WP_GAS_PRODUCTION_ACTIVITIES_Percentages?.FirstOrDefault()?.Percentage_Utilized.ToString())
                            .Replace("(NO_TOTAL_GAS_FLARED)", WP_GAS_PRODUCTION_ACTIVITIES_Percentages?.FirstOrDefault()?.Flared_Gas_Produced.ToString())
                            .Replace("(NO_TOTAL_GAS_PRODUCED_JV)", WP_GAS_PRODUCTION_ACTIVITIES_Contract_Type.Where(x => x.Contract_Type == GeneralModel.JV)?.FirstOrDefault()?.Flared_Gas_Produced.ToString())
                            .Replace("(NO_TOTAL_GAS_PRODUCED_PSC)", WP_GAS_PRODUCTION_ACTIVITIES_Contract_Type.Where(x => x.Contract_Type == GeneralModel.PSC)?.FirstOrDefault()?.Flared_Gas_Produced.ToString())
                            .Replace("(NO_TOTAL_GAS_PRODUCED_MFIO)", WP_GAS_PRODUCTION_ACTIVITIES_Contract_Type.Where(x => x.Contract_Type == GeneralModel.MF)?.FirstOrDefault()?.Flared_Gas_Produced.ToString())
                            .Replace("(NO_TOTAL_GAS_PRODUCED_YEAR_TO_DATE)", WP_GAS_PRODUCTION_ACTIVITIES_Percentages_CY?.FirstOrDefault()?.Actual_Total_Gas_Produced.ToString())
                            .Replace("(NO_TOTAL_GAS_PRODUCED_YEAR_TO_DATE_UTILIZED)", WP_GAS_PRODUCTION_ACTIVITIES_Percentages_CY?.FirstOrDefault()?.Utilized_Gas_Produced.ToString())
                            .Replace("(PERCENTAGE_TOTAL_GAS_PRODUCED_YEAR_TO_DATE_UTILIZED)", WP_GAS_PRODUCTION_ACTIVITIES_Percentages_CY?.FirstOrDefault()?.Percentage_Utilized.ToString())
                            .Replace("(NO_TOTAL_GAS_PRODUCED_YEAR_TO_DATE_FLARED)", WP_GAS_PRODUCTION_ACTIVITIES_Percentages_CY?.FirstOrDefault()?.Flared_Gas_Produced.ToString())
                            .Replace("(NO_NIGERIA_OIL_RESERVES)", OIL_CONDENSATE_MMBBL?.FirstOrDefault()?.Reserves_as_at_MMbbl.ToString())
                            .Replace("(NO_NIGERIA_OIL_CONDENSATE)", OIL_CONDENSATE_MMBBL?.FirstOrDefault()?.Reserves_as_at_MMbbl_condensate.ToString())
                            .Replace("(NO_NIGERIA_OIL_RESERVE_GAS)", OIL_CONDENSATE_MMBBL?.FirstOrDefault()?.Reserves_as_at_MMbbl_gas.ToString())
                            .Replace("(NO_OF_ACCIDENTS)", HSE_ACCIDENT_Total?.FirstOrDefault()?.Sum_accident.ToString())
                            .Replace("(NO_OF_FATALITIES)", HSE_ACCIDENT_Consequences?.FirstOrDefault()?.sum_accident.ToString())
                            .Replace("(NO_OF_SPILLS)", HSE_ACCIDENT.Where(x => x.Cause == GeneralModel.Sabotage)?.FirstOrDefault()?.sum_accident.ToString())
                            .Replace("(NO_OF_RELEASE)", HSE_VOLUME_OF_OILSPILL.ToString())
                            .Replace("(PERCENTAGE_OF_SABOTAGE)", HSE_ACCIDENT.Where(x => x.Cause == GeneralModel.Sabotage)?.FirstOrDefault()?.Percentage_Spill.ToString())
                            .Replace("(PERCENTAGE_OF_EQUIPMENT_FAILURE)", HSE_ACCIDENT.Where(x => x.Cause == GeneralModel.EquipmentFailure)?.FirstOrDefault()?.Percentage_Spill.ToString())
                            .Replace("(PERCENTAGE_OF_HUMAN_ERROR)", HSE_ACCIDENT.Where(x => x.Cause == GeneralModel.HumanError)?.FirstOrDefault()?.Percentage_Spill.ToString())
                            .Replace("(PERCENTAGE_OF_MYSTERY_SPILLS)", HSE_ACCIDENT.Where(x => x.Cause == GeneralModel.MysterySpills)?.FirstOrDefault()?.Percentage_Spill.ToString())
                        ;

                        get_ReportContent_2.Report_Content = get_ReportContent_2.Report_Content
                            .Replace("(NO_OF_JV)", GEO_ACTIVITIES.Where(x => x.Contract_Type == GeneralModel.JV)?.FirstOrDefault()?.Actual_year_aquired_data.ToString())
                            .Replace("(NO_OF_JV_COMPANIES)", GEO_ACTIVITIES.Where(x => x.Contract_Type == GeneralModel.JV)?.FirstOrDefault()?.Count_Contract_Type.ToString())
                            .Replace("(NO_OF_PSC_COMPANIES)", GEO_ACTIVITIES.Where(x => x.Contract_Type == GeneralModel.PSC)?.FirstOrDefault()?.Count_Contract_Type.ToString())
                            .Replace("(NO_OF_PSC)", GEO_ACTIVITIES.Where(x => x.Contract_Type == GeneralModel.PSC)?.FirstOrDefault()?.Actual_year_aquired_data.ToString())
                            .Replace("(NO_OF_MF_COMPANIES)", GEO_ACTIVITIES.Where(x => x.Contract_Type == GeneralModel.MF)?.FirstOrDefault()?.Count_Contract_Type.ToString())
                            .Replace("(NO_OF_MF)", GEO_ACTIVITIES.Where(x => x.Contract_Type == GeneralModel.MF)?.FirstOrDefault()?.Actual_year_aquired_data.ToString())
                            .Replace("(NO_OF_INDIGENOUS_COMPANIES)", GEO_ACTIVITIES.Where(x => x.Contract_Type == GeneralModel.SR)?.FirstOrDefault()?.Count_Contract_Type.ToString())
                            .Replace("(NO_OF_INDIGENOUS)", GEO_ACTIVITIES.Where(x => x.Contract_Type == GeneralModel.SR)?.FirstOrDefault()?.Actual_year_aquired_data.ToString())
                            .Replace("(N)", year)
                            .Replace("(N + 1)", (int.Parse(year) + 1).ToString())
                            .Replace("(N - 1)", previousYear)
                            .Replace("(ACQUIRED_3D)", WP_COUNT_GEOPHYSICAL_ACTIVITIES_ACQUISITION?.FirstOrDefault()?.Actual_year_aquired_data.ToString())
                            ;

                        ADMIN_WORK_PROGRAM_REPORTs_Model.Add(new ADMIN_WORK_PROGRAM_REPORT()
                        {
                            Report_Content = get_ReportContent_1.Report_Content,
                        });
                        ADMIN_WORK_PROGRAM_REPORTs_Model.Add(new ADMIN_WORK_PROGRAM_REPORT()
                        {
                            Report_Content = get_ReportContent_2.Report_Content,
                        });
                    }
                    #endregion

                    //TOTAL_PRODUCED_WATER
                    int number;
                    int value= 0;
                    
                    var TOTAL_PRODUCED_WATER = new List<TOTAL_PRODUCED_WATER_Model>();
                    var result = _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.Where(o=> o.Year_of_WP == year).ToList();
                    result.ForEach(x =>
                    {
                        if (int.TryParse(x.Produced_water_volumes, out number))
                        {
                            value += int.Parse(x.Produced_water_volumes);
                            TOTAL_PRODUCED_WATER.Add(new TOTAL_PRODUCED_WATER_Model()
                            {
                                TOTAL_QUANTITY_SPILLED = value,
                                CompanyName = x.CompanyName,
                                Year_of_WP = x.Year_of_WP
                            });
                        }
                    });
                    var TOTAL_PRODUCED_WATER_Data = (from o in TOTAL_PRODUCED_WATER
                                                group o by new
                                                {
                                                    o.CompanyName
                                                }
                                                into g
                                                select new
                                                {
                                                    TOTAL_QUANTITY_SPILLED =  g.Sum(x =>Convert.ToInt64(x.TOTAL_QUANTITY_SPILLED)),
                                                    CompanyName = g.Key,
                                                    Year_of_WP = g?.FirstOrDefault().Year_of_WP
                                                }).ToList();

                    var HSE_QUANTITY  = (from o in _context.HSE_CAUSES_OF_SPILLs
                                              where o.Year_of_WP == year
                                              select o).ToList();

                    
                    var PRODUCTION_TOTAL_BARREL_PROPOSED = (from u in _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE_PROPOSEDs
                                                        where u.Year_of_WP == year
                                                        select u).GroupBy(x => x.Contract_Type).Select(x => x.FirstOrDefault()).ToList();
                    
                    var PLANNED_TERRAIN = (from u in _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrain_PLANNEDs
                                           where u.Year_of_WP == year select u).GroupBy(x => x.Terrain).Select(x => x.FirstOrDefault()).ToList();
                
                    
                    var FLARINGFORECAST = _context.WP_Gas_Production_Utilisation_And_Flaring_Forecasts.Where(x => x.Year_of_WP == year).ToList();


                    WorkProgrammeReport.ADMIN_WORK_PROGRAM_REPORT_Model = ADMIN_WORK_PROGRAM_REPORTs_Model;
                    WorkProgrammeReport.E_and_P_companies_Model = E_and_P_companies;
                    WorkProgrammeReport.WP_Presentations_Model = WP_COUNT;
                    WorkProgrammeReport.WP_GEOPHYSICAL_ACTIVITIES_ACQUISITION_Model = WP_COUNT_GEOPHYSICAL_ACTIVITIES_ACQUISITION;
                    WorkProgrammeReport.WP_GEOPHYSICAL_ACTIVITIES_PROCESSING_Model = WP_COUNT_GEOPHYSICAL_ACTIVITIES_PROCESSING;
                    WorkProgrammeReport.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELL_Model =  WP_COUNT_WELLS;
                    WorkProgrammeReport.WP_SUM_APPRAISAL_WELL_Model = WP_SUM_APPRAISAL_WELLS;
                    WorkProgrammeReport.WP_SUM_INITIAL_WELL_COMPLETION_JOBS_WORKOVERS_RECOMPLETION_Model = WP_SUM_WORKOVERS_RECOMPLETION;
                    WorkProgrammeReport.DRILLING_OPERATIONS_Appraisal_Model = WP_COUNT_Appraisal;
                    WorkProgrammeReport.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITY_Contract_Type_Model = WP_JV_Contract_Type;
                    WorkProgrammeReport.WP_GAS_PRODUCTION_ACTIVITY_CURRENTYEAR_Model = WP_CRUDE_OIL_CY;
                    WorkProgrammeReport.WP_GAS_PRODUCTION_ACTIVITY_PREVIOUSYEAR_Model = WP_CRUDE_OIL_PY;
                    WorkProgrammeReport.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE_Model = WP_OIL_PRODUCTION_total_barrel;
                    WorkProgrammeReport.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITY_monthly_ActivitY_OIL_PRODUCTION_by_Terrain_Model = WP_Terrain_Continental;
                    WorkProgrammeReport.PY_GAS_ACTIVITIES_Model = PY_GAS_ACTIVITIES;
                    WorkProgrammeReport.CY_GAS_ACTIVITIES_Model = CY_GAS_ACTIVITIES;
                    WorkProgrammeReport.GAS_ACTIVITIES_CONTRACTTYPES_Model = GAS_ACTIVITIES_CONTRACTTYPES;
                    WorkProgrammeReport.WP_RESERVES_UPDATES_OIL_CONDENSATE_MMBBL_Model = OIL_CONDENSATE_MMBBL;
                    WorkProgrammeReport.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_by_consequence_Model = HSE_ACCIDENT_Consequences;
                    WorkProgrammeReport.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_accident_Model = HSE_ACCIDENT_Total;
                    WorkProgrammeReport.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_spill_accident_and_percentage_Model = HSE_ACCIDENT;
                    WorkProgrammeReport.GEO_sum_and_count_Model = GEO_ACTIVITIES;
                    WorkProgrammeReport.VOLUME_OF_OILSPILL = HSE_VOLUME_OF_OILSPILL.ToString();
                    WorkProgrammeReport.OILSPILL_REPORT_Model = OILSPILL_REPORT;
                    WorkProgrammeReport.HSE_QUANTITY_Model = HSE_QUANTITY;
                    WorkProgrammeReport.TOTAL_PRODUCED_WATER_Model = TOTAL_PRODUCED_WATER_Data;
                    WorkProgrammeReport.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITY_monthly_ActivitY_OIL_PRODUCTION_by_Terrain_PLANNED_Model = PLANNED_TERRAIN;
                    WorkProgrammeReport.WP_Gas_Production_Utilisation_And_Flaring_Forecast_Model = FLARINGFORECAST;

                    //return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = WorkProgrammeReport, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    WorkProgrammeReport.Error = "Error : Year wasn't passed correctly";
                    //return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Year wasn't passed correctly", StatusCode = ResponseCodes.Failure };
                }
            }
            catch (Exception e)
            {

                WorkProgrammeReport.Error = "Error : " + e.Message;

            }
            
            return WorkProgrammeReport;

        }
        [HttpGet("GET_SEISMIC_DATA_REPORT")]
        public WorkProgrammeReport2_Model GET_SEISMIC_DATA_REPORT(string year)
        {
            WorkProgrammeReport2_Model WKP_Report2 = new WorkProgrammeReport2_Model();

            try
            {
                string previousYear = year != null ? (int.Parse(year) - 1).ToString() : "";
                string twoYearsAgo = year != null ? (int.Parse(year) - 1).ToString() : "";
                string proposedYear = year != null ? (int.Parse(year) + 1).ToString() : "";

                WKP_Report2.Seismic_Data_Approved_and_Acquired = _context.Sum_GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(x => x.Year_of_WP == year && x.Geo_type_of_data_acquired == GeneralModel.ThreeD).ToList();

                WKP_Report2.Seismic_Data_Approved_and_Acquired_PLANNED = (from o in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs
                                                                          where o.Year_of_WP == year && o.Geo_type_of_data_acquired == GeneralModel.ThreeD
                                                                          group o by new { o.Year_of_WP } into g
                                                                          select new
                                                                          {
                                                                              Year_of_WP = g.Key,
                                                                              CompanyName = g.FirstOrDefault().CompanyName,
                                                                              OML_Name = g.FirstOrDefault().OML_Name,
                                                                              Terrain = g.FirstOrDefault().Terrain,
                                                                              Name_of_Contractor = g.FirstOrDefault().Name_of_Contractor,
                                                                              Geo_type_of_data_acquired = g.FirstOrDefault().Geo_type_of_data_acquired,
                                                                              Actual_year_aquired_data = g.Sum(x => Convert.ToInt32(Convert.ToDouble(x.Actual_year_aquired_data))),
                                                                              Quantum_Approved = g.Sum(x => Convert.ToDouble(x.Quantum_Approved)),
                                                                              Quantum_Planned = g.Sum(x => Convert.ToDouble(x.Quantum_Planned)),

                                                                          }).ToList();


                WKP_Report2.Seismic_Data_Approved_and_Acquired_PREVIOUS = _context.Sum_GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(x => x.Year_of_WP == previousYear).ToList();

                WKP_Report2.Seismic_Data_Approved_and_Acquired_TWO_YEARS_AG0 = _context.Sum_GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(x => x.Year_of_WP == twoYearsAgo).ToList();


                WKP_Report2.Seismic_Data_Processing_and_Reprocessing_Activities_CURRENT = (from o in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs
                                                                                           where o.Year_of_WP == year
                                                                                           group o by new { o.CompanyName } into g
                                                                                           select new
                                                                                           {
                                                                                               Year_of_WP = g.Key,
                                                                                               CompanyName = g.FirstOrDefault().CompanyName,
                                                                                               OML_Name = g.FirstOrDefault().OML_Name,
                                                                                               Terrain = g.FirstOrDefault().Terrain,
                                                                                               Name_of_Contractor = g.FirstOrDefault().Name_of_Contractor,
                                                                                               Actual_year_aquired_data = g.Sum(x => Convert.ToInt32(Convert.ToDouble(x.Actual_year_aquired_data))),
                                                                                               Quantum_Approved = g.Sum(x => Convert.ToDouble(x.Quantum_Approved)),
                                                                                               Geo_Quantum_of_Data = g.Sum(x => Convert.ToDouble(x.Geo_Quantum_of_Data)),

                                                                                           }).ToList();

                WKP_Report2.Seismic_Data_Processing_and_Reprocessing_Activities_CURRENT_PLANNED = (from o in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs
                                                                                                   where o.Year_of_WP == proposedYear
                                                                                                   group o by new { o.CompanyName } into g
                                                                                                   select new
                                                                                                   {
                                                                                                       Year_of_WP = g.Key,
                                                                                                       CompanyName = g.FirstOrDefault().CompanyName,
                                                                                                       OML_Name = g.FirstOrDefault().OML_Name,
                                                                                                       Terrain = g.FirstOrDefault().Terrain,
                                                                                                       Name_of_Contractor = g.FirstOrDefault().Name_of_Contractor,
                                                                                                       Actual_year_aquired_data = g.Sum(x => Convert.ToInt32(Convert.ToDouble(x.Actual_year_aquired_data))),
                                                                                                       Quantum_Approved = g.Sum(x => Convert.ToDouble(x.Quantum_Approved)),
                                                                                                       Geo_Quantum_of_Data = g.Sum(x => Convert.ToDouble(x.Geo_Quantum_of_Data)),

                                                                                                   }).ToList();

                WKP_Report2.Seismic_Data_Processing_and_Reprocessing_Activities_PREVIOUS = (from o in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs
                                                                                            where o.Year_of_WP == previousYear
                                                                                            group o by new { o.CompanyName } into g
                                                                                            select new
                                                                                            {
                                                                                                Year_of_WP = g.Key,
                                                                                                CompanyName = g.FirstOrDefault().CompanyName,
                                                                                                OML_Name = g.FirstOrDefault().OML_Name,
                                                                                                Terrain = g.FirstOrDefault().Terrain,
                                                                                                Name_of_Contractor = g.FirstOrDefault().Name_of_Contractor,
                                                                                                Actual_year_aquired_data = g.Sum(x => Convert.ToInt32(Convert.ToDouble(x.Actual_year_aquired_data))),
                                                                                                Quantum_Approved = g.Sum(x => Convert.ToDouble(x.Quantum_Approved)),
                                                                                                Geo_Quantum_of_Data = g.Sum(x => Convert.ToDouble(x.Geo_Quantum_of_Data)),

                                                                                            }).ToList();


                WKP_Report2.Seismic_Data_Processing_and_Reprocessing_Activities_TWO_YEARS_AGO = (from o in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs
                                                                                                 where o.Year_of_WP == twoYearsAgo
                                                                                                 group o by new { o.CompanyName } into g
                                                                                                 select new
                                                                                                 {
                                                                                                     Year_of_WP = g.Key,
                                                                                                     CompanyName = g.FirstOrDefault().CompanyName,
                                                                                                     OML_Name = g.FirstOrDefault().OML_Name,
                                                                                                     Terrain = g.FirstOrDefault().Terrain,
                                                                                                     Name_of_Contractor = g.FirstOrDefault().Name_of_Contractor,
                                                                                                     Actual_year_aquired_data = g.Sum(x => Convert.ToInt32(Convert.ToDouble(x.Actual_year_aquired_data))),
                                                                                                     Quantum_Approved = g.Sum(x => Convert.ToDouble(x.Quantum_Approved)),
                                                                                                     Geo_Quantum_of_Data = g.Sum(x => Convert.ToDouble(x.Geo_Quantum_of_Data)),

                                                                                                 }).ToList();
            }

            catch (Exception e)
            {

                WKP_Report2.Error = "Error : " + e.Message;
            }

            return WKP_Report2;
        }
        [HttpGet("GET_DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_REPORT")]
        public WorkProgrammeReport2_Model GET_DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_REPORT(string year)
        {
            WorkProgrammeReport2_Model WKP_Report2 = new WorkProgrammeReport2_Model();

            try
            {
                string previousYear = year != null ? (int.Parse(year) - 1).ToString() : "";
                string twoYearsAgo = year != null ? (int.Parse(year) - 1).ToString() : "";
                string proposedYear = year != null ? (int.Parse(year) + 1).ToString() : "";

                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Exploration = _context.Sum_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.Exploration).ToList();

                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Appraisal = _context.Sum_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category.ToLower().Contains(GeneralModel.Appraisal.ToLower())).ToList();

                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Development = _context.WP_COUNT_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.Development).ToList();

                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Exploration_PY = _context.Sum_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == proposedYear && x.Category == GeneralModel.Exploration).ToList();

                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Appraisal_PY = _context.Sum_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == proposedYear && x.Category.ToLower().Contains(GeneralModel.Appraisal.ToLower())).ToList();

                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Development_PY = _context.WP_COUNT_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == proposedYear && x.Category == GeneralModel.Development).ToList();

            }

            catch (Exception e)
            {

                WKP_Report2.Error = "Error : " + e.Message;

            }

            return WKP_Report2;
        }
        [HttpGet("GET_OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS")]
        public WorkProgrammeReport2_Model GET_OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS(string year)
        {
            WorkProgrammeReport2_Model WKP_Report2 = new WorkProgrammeReport2_Model();

            try
            {
                string previousYear = year != null ? (int.Parse(year) - 1).ToString() : "";
                string twoYearsAgo = year != null ? (int.Parse(year) - 1).ToString() : "";
                string proposedYear = year != null ? (int.Parse(year) + 1).ToString() : "";
                
                WKP_Report2.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS = _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS_PY = _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Where(x=>x.Year_of_WP == year && x.Actual_Proposed == "Actual Year").ToList();

            }

            catch (Exception e)
            {

                WKP_Report2.Error = "Error : " + e.Message;
            }

            return WKP_Report2;
        }
        [HttpGet("GET_NIGERIA_CONTENT")]
        public WorkProgrammeReport2_Model GET_NIGERIA_CONTENT(string year)
        {
            WorkProgrammeReport2_Model WKP_Report2 = new WorkProgrammeReport2_Model();

            try
            {
                string previousYear = year != null ? (int.Parse(year) - 1).ToString() : "";
                string twoYearsAgo = year != null ? (int.Parse(year) - 1).ToString() : "";
                string proposedYear = year != null ? (int.Parse(year) + 1).ToString() : "";

                WKP_Report2.NIGERIA_CONTENT_TRAINING_PY = _context.NIGERIA_CONTENT_Trainings.Where(x => x.Year_of_WP == year && x.Actual_Proposed == "Proposed Year").ToList();

                WKP_Report2.NIGERIA_CONTENT_TRAINING = _context.NIGERIA_CONTENT_Trainings.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.NIGERIA_CONTENT_UPLOAD_SUCESSION_PLAN = _context.NIGERIA_CONTENT_Upload_Succession_Plans.Where(x => x.Year_of_WP == year).ToList();

            }

            catch (Exception e)
            {

                WKP_Report2.Error = "Error : " + e.Message;

            }

            return WKP_Report2;
        }

        [HttpGet("GET_OIL_GAS_CONDENSATE_PRODUCTION_ACTIVITIES")]
        public WorkProgrammeReport2_Model GET_OIL_GAS_CONDENSATE_PRODUCTION_ACTIVITIES(string year)
        {
            WorkProgrammeReport2_Model WKP_Report2 = new WorkProgrammeReport2_Model();

            try
            {
                string previousYear = year != null ? (int.Parse(year) - 1).ToString() : "";
                string twoYearsAgo = year != null ? (int.Parse(year) - 1).ToString() : "";
                string proposedYear = year != null ? (int.Parse(year) + 1).ToString() : "";


                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_NEW_TECHNOLOGY_CONFORMITY_ASSESSMENT = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_OPERATING_FACILITIES = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Operating_Facilities.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_CURRENT_PLANNED = _context.WP_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_CURRENT_PLANNEDs.Where(x => x.Fiveyear_Projection_Year == proposedYear).ToList();

                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_PROPOSED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_PROPOSEDs.Where(x => x.Year_of_WP == proposedYear).ToList();

                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_C_TYPE_PROPOSED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE_PROPOSEDs.Where(x => x.Year_of_WP == proposedYear).ToList();

                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_By_month_year_PROPOSED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_By_month_year_PROPOSEDs.Where(x => x.Year_of_WP == proposedYear).ToList();

                //error WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrain_PLANNED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrain_PLANNEDs.Where(x => x.Year_of_WP == proposedYear).ToList();

                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_Pivotted_PRODUCTION_BRKDWN_PLANNED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_Pivotted_by_company_productionmonth_year_breakdown_PLANNEDs.Where(x => x.Year_of_WP == proposedYear).ToList();

                //error WKP_Report2.GAS_PRODUCTION_ACTIVITIES_produced_utilized_flared_PLANNED = _context.WP_GAS_PRODUCTION_ACTIVITIES_produced_utilized_flared_PLANNEDs.Where(x => x.Year_of_WP == proposedYear).ToList();

                WKP_Report2.GAS_PRODUCTION_ACTIVITIES_contract_type_basis_PLANNED = _context.WP_GAS_PRODUCTION_ACTIVITIES_contract_type_basis_PLANNEDs.Where(x => x.Year_of_WP == proposedYear).ToList();

               
            }

            catch (Exception e)
            {

                WKP_Report2.Error = "Error : " + e.Message;

            }

            return WKP_Report2;
        }

        [HttpGet("GET_BUDGET")]
        public WorkProgrammeReport2_Model GET_BUDGET(string year)
        {
            WorkProgrammeReport2_Model WKP_Report2 = new WorkProgrammeReport2_Model();

            try
            {
                string previousYear = year != null ? (int.Parse(year) - 1).ToString() : "";
                string proposedYear = year != null ? (int.Parse(year) + 1).ToString() : "";

                WKP_Report2.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIES = _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIES = _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT = _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.BUDGET_PERFORMANCE_PRODUCTION_COST = _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.STRATEGIC_PLANS_ON_COMPANY_BASIS = _context.STRATEGIC_PLANS_ON_COMPANY_BAses.Where(x => x.Year_of_WP == year).ToList();

            }

            catch (Exception e)
            {

                WKP_Report2.Error = "Error : " + e.Message;

            }

            return WKP_Report2;
        }

        [HttpGet("GET_HSE")]
        public WorkProgrammeReport2_Model GET_HSE(string year)
        {
            WorkProgrammeReport2_Model WKP_Report2 = new WorkProgrammeReport2_Model();

            try
            {
                string previousYear = year != null ? (int.Parse(year) - 1).ToString() : "";
                string proposedYear = year != null ? (int.Parse(year) + 1).ToString() : "";


                WKP_Report2.FATALITIES_ACCIDENT_STATISTIC_TABLE = _context.WP_HSE_FATALITIES_accident_statistic_tables.Where(x => x.Year_of_WP == year && x.Fatalities_Type == GeneralModel.Fatalities).ToList();

                WKP_Report2.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW = _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships = _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisition = _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.HSE_CAUSES_OF_SPILL = _context.HSE_CAUSES_OF_SPILLs.Where(x => x.Year_of_WP == year).ToList();

            }

            catch (Exception e)
            {

                WKP_Report2.Error = "Error : " + e.Message;

            }

            return WKP_Report2;
        }

        [HttpGet("GET_RESERVES")]
        public WorkProgrammeReport2_Model GET_RESERVES(string year)
        {
            WorkProgrammeReport2_Model WKP_Report2 = new WorkProgrammeReport2_Model();

            try
            {
                string previousYear = year != null ? (int.Parse(year) - 1).ToString() : "";
                string proposedYear = year != null ? (int.Parse(year) + 1).ToString() : "";


                WKP_Report2.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_CURRENT_PLANNED = _context.WP_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_CURRENT_PLANNEDs.Where(x => x.Fiveyear_Projection_Year == proposedYear).ToList();

                //error WKP_Report2.RESERVES_REPLACEMENT_RATIO_VALUE_PIVOTTED = _context.WP_RESERVES_REPLACEMENT_RATIO_VALUE_PIVOTTEDs.ToList();
            }

            catch (Exception e)
            {

                WKP_Report2.Error = "Error : " + e.Message;

            }

            return WKP_Report2;
        }


        [HttpGet("Get_General_Report")]
        public WorkProgrammeReport2_Model Get_General_Report(string year)
        {
            WorkProgrammeReport2_Model WKP_Report2 = new WorkProgrammeReport2_Model();

            try
            {
                string previousYear = year != null ? (int.Parse(year) - 1).ToString() : "";
                string twoYearsAgo = year != null ? (int.Parse(year) - 1).ToString() : "";
                string proposedYear = year != null ? (int.Parse(year) + 1).ToString() : "";

                WKP_Report2.Seismic_Data_Approved_and_Acquired = _context.Sum_GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(x => x.Year_of_WP == year && x.Geo_type_of_data_acquired == GeneralModel.ThreeD).ToList();

                WKP_Report2.Seismic_Data_Approved_and_Acquired_PLANNED = (from o in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs
                                                                   where o.Year_of_WP == year && o.Geo_type_of_data_acquired == GeneralModel.ThreeD
                                                                   group o by new { o.Year_of_WP } into g
                                                                   select new 
                                                                   {
                                                                       Year_of_WP = g.Key,
                                                                       CompanyName = g.FirstOrDefault().CompanyName,
                                                                       OML_Name = g.FirstOrDefault().OML_Name,
                                                                       Terrain = g.FirstOrDefault().Terrain,
                                                                       Name_of_Contractor = g.FirstOrDefault().Name_of_Contractor,
                                                                       Geo_type_of_data_acquired = g.FirstOrDefault().Geo_type_of_data_acquired,
                                                                       Actual_year_aquired_data = g.Sum(x => Convert.ToInt32(Convert.ToDouble(x.Actual_year_aquired_data))),
                                                                       Quantum_Approved = g.Sum(x => Convert.ToDouble(x.Quantum_Approved)),
                                                                       Quantum_Planned = g.Sum(x => Convert.ToDouble(x.Quantum_Planned)),

                                                                   }).ToList();
                

                WKP_Report2.Seismic_Data_Approved_and_Acquired_PREVIOUS = _context.Sum_GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(x => x.Year_of_WP == previousYear ).ToList();

                WKP_Report2.Seismic_Data_Approved_and_Acquired_TWO_YEARS_AG0 = _context.Sum_GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(x => x.Year_of_WP == twoYearsAgo).ToList();


                WKP_Report2.Seismic_Data_Processing_and_Reprocessing_Activities_CURRENT = (from o in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs
                                                                  where o.Year_of_WP == year 
                                                                  group o by new { o.CompanyName } into g
                                                                  select new
                                                                  {
                                                                      Year_of_WP = g.Key,
                                                                      CompanyName = g.FirstOrDefault().CompanyName,
                                                                      OML_Name = g.FirstOrDefault().OML_Name,
                                                                      Terrain = g.FirstOrDefault().Terrain,
                                                                      Name_of_Contractor = g.FirstOrDefault().Name_of_Contractor,
                                                                      Actual_year_aquired_data = g.Sum(x => Convert.ToInt32(Convert.ToDouble(x.Actual_year_aquired_data))),
                                                                      Quantum_Approved = g.Sum(x => Convert.ToDouble(x.Quantum_Approved)),
                                                                      Geo_Quantum_of_Data = g.Sum(x => Convert.ToDouble(x.Geo_Quantum_of_Data)),

                                                                  }).ToList();

                WKP_Report2.Seismic_Data_Processing_and_Reprocessing_Activities_CURRENT_PLANNED = (from o in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs
                                                                  where o.Year_of_WP == proposedYear 
                                                                  group o by new { o.CompanyName } into g
                                                                  select new
                                                                  {
                                                                      Year_of_WP = g.Key,
                                                                      CompanyName = g.FirstOrDefault().CompanyName,
                                                                      OML_Name = g.FirstOrDefault().OML_Name,
                                                                      Terrain = g.FirstOrDefault().Terrain,
                                                                      Name_of_Contractor = g.FirstOrDefault().Name_of_Contractor,
                                                                      Actual_year_aquired_data = g.Sum(x => Convert.ToInt32(Convert.ToDouble(x.Actual_year_aquired_data))),
                                                                      Quantum_Approved = g.Sum(x => Convert.ToDouble(x.Quantum_Approved)),
                                                                      Geo_Quantum_of_Data = g.Sum(x => Convert.ToDouble(x.Geo_Quantum_of_Data)),

                                                                  }).ToList();
                
                WKP_Report2.Seismic_Data_Processing_and_Reprocessing_Activities_PREVIOUS = (from o in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs
                                                                                    where o.Year_of_WP == previousYear
                                                                           group o by new { o.CompanyName } into g
                                                                           select new
                                                                           {
                                                                               Year_of_WP = g.Key,
                                                                               CompanyName = g.FirstOrDefault().CompanyName,
                                                                               OML_Name = g.FirstOrDefault().OML_Name,
                                                                               Terrain = g.FirstOrDefault().Terrain,
                                                                               Name_of_Contractor = g.FirstOrDefault().Name_of_Contractor,
                                                                               Actual_year_aquired_data = g.Sum(x => Convert.ToInt32(Convert.ToDouble(x.Actual_year_aquired_data))),
                                                                               Quantum_Approved = g.Sum(x => Convert.ToDouble(x.Quantum_Approved)),
                                                                               Geo_Quantum_of_Data = g.Sum(x => Convert.ToDouble(x.Geo_Quantum_of_Data)),

                                                                           }).ToList();


                WKP_Report2.Seismic_Data_Processing_and_Reprocessing_Activities_TWO_YEARS_AGO = (from o in _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs
                                                                                    where o.Year_of_WP == twoYearsAgo
                                                                                    group o by new { o.CompanyName } into g
                                                                                    select new
                                                                                    {
                                                                                        Year_of_WP = g.Key,
                                                                                        CompanyName = g.FirstOrDefault().CompanyName,
                                                                                        OML_Name = g.FirstOrDefault().OML_Name,
                                                                                        Terrain = g.FirstOrDefault().Terrain,
                                                                                        Name_of_Contractor = g.FirstOrDefault().Name_of_Contractor,
                                                                                        Actual_year_aquired_data = g.Sum(x => Convert.ToInt32(Convert.ToDouble(x.Actual_year_aquired_data))),
                                                                                        Quantum_Approved = g.Sum(x => Convert.ToDouble(x.Quantum_Approved)),
                                                                                        Geo_Quantum_of_Data = g.Sum(x => Convert.ToDouble(x.Geo_Quantum_of_Data)),

                                                                                    }).ToList();


                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Exploration = _context.Sum_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.Exploration).ToList();
               
                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Appraisal   = _context.Sum_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category.ToLower().Contains( GeneralModel.Appraisal.ToLower() )).ToList();
                
                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Development = _context.WP_COUNT_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.Development ).ToList();
                
                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Exploration_PY = _context.Sum_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == proposedYear && x.Category == GeneralModel.Exploration).ToList();
               
                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Appraisal_PY   = _context.Sum_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == proposedYear && x.Category.ToLower().Contains( GeneralModel.Appraisal.ToLower() )).ToList();
                
                WKP_Report2.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS_Development_PY = _context.WP_COUNT_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == proposedYear && x.Category == GeneralModel.Development ).ToList();
                
                //err WKP_Report2.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_CURRENT = _context.WP_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_CURRENTs.Where(x => x.Company_Reserves_Year == proposedYear ).ToList();
                
                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTIONs.Where(x => x.Year_of_WP == year ).ToList();
                
                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPEs.Where(x => x.Year_of_WP == year ).ToList();
                
                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_By_month_year = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_By_month_years.Where(x => x.Year_of_WP == year ).ToList();
                
                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrain = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrains.Where(x => x.Year_of_WP == year ).ToList();
                
                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_by_ContractType_Pivotted = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_by_ContractType_Pivotteds.ToList();
                
                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_by_Terrain_Pivoted = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_by_Terrain_Pivoteds.ToList();
                
                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_Pivotted_by_company_productionmonth_year = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_Pivotted_by_company_productionmonth_year_breakdowns.ToList();

                //error WKP_Report2.GAS_PRODUCTION_ACTIVITIES_produced_utilized_flared = _context.WP_GAS_PRODUCTION_ACTIVITIES_produced_utilized_flareds.ToList();

                //error WKP_Report2.GAS_PRODUCTION_ACTIVITIES_contract_type_basis = _context.WP_GAS_PRODUCTION_ACTIVITIES_contract_type_bases.ToList();

                //error WKP_Report2.GAS_PRODUCTION_ACTIVITIES_terrain_pivotted = _context.WP_GAS_PRODUCTION_ACTIVITIES_terrain_pivotteds.ToList();

                //error WKP_Report2.GAS_PRODUCTION_ACTIVITIES_contract_type_pivoted = _context.WP_GAS_PRODUCTION_ACTIVITIES_contract_type_pivoteds.ToList();

                //error WKP_Report2.GAS_PRODUCTION_ACTIVITIES_penalty_payment = _context.WP_GAS_PRODUCTION_ACTIVITIES_penalty_payments.ToList();

                WKP_Report2.FATALITIES_ACCIDENT_STATISTIC_TABLE = _context.WP_HSE_FATALITIES_accident_statistic_tables.Where(x=>x.Year_of_WP == year && x.Fatalities_Type == GeneralModel.Fatalities).ToList();

                //WKP_Report2.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS = _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Where(x=>x.Year_of_WP == year && x.Actual_Proposed == "Actual Year").ToList();

                WKP_Report2.NIGERIA_CONTENT_TRAINING_PY = _context.NIGERIA_CONTENT_Trainings.Where(x => x.Year_of_WP == year && x.Actual_Proposed == "Proposed Year").ToList();

                WKP_Report2.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS = _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Where(x=>x.Year_of_WP == year ).ToList();

                WKP_Report2.NIGERIA_CONTENT_TRAINING = _context.NIGERIA_CONTENT_Trainings.Where(x => x.Year_of_WP == year ).ToList();

                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_NEW_TECHNOLOGY_CONFORMITY_ASSESSMENT = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.Where(x=>x.Year_of_WP == year).ToList();
                
                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_OPERATING_FACILITIES = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Operating_Facilities.Where(x=>x.Year_of_WP == year).ToList();

                WKP_Report2.NIGERIA_CONTENT_UPLOAD_SUCESSION_PLAN = _context.NIGERIA_CONTENT_Upload_Succession_Plans.Where(x=>x.Year_of_WP == year ).ToList();
                
                WKP_Report2.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW = _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs.Where(x=>x.Year_of_WP == year ).ToList();
                
                WKP_Report2.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships = _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Scholarships.Where(x=>x.Year_of_WP == year ).ToList();
                
                WKP_Report2.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisition = _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEW_Training_Skill_Acquisitions.Where(x=>x.Year_of_WP == year ).ToList();
                
                WKP_Report2.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIES = _context.BUDGET_PERFORMANCE_EXPLORATORY_ACTIVITIEs.Where(x=>x.Year_of_WP == year ).ToList();
                
                WKP_Report2.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIES = _context.BUDGET_PERFORMANCE_DEVELOPMENT_DRILLING_ACTIVITIEs.Where(x=>x.Year_of_WP == year ).ToList();
                
                WKP_Report2.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECT = _context.BUDGET_PERFORMANCE_FACILITIES_DEVELOPMENT_PROJECTs.Where(x=>x.Year_of_WP == year ).ToList();

                WKP_Report2.BUDGET_PERFORMANCE_PRODUCTION_COST = _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs.Where(x => x.Year_of_WP == year).ToList();

                WKP_Report2.STRATEGIC_PLANS_ON_COMPANY_BASIS = _context.STRATEGIC_PLANS_ON_COMPANY_BAses.Where(x => x.Year_of_WP == year).ToList();



                //WP_OML_WEIGHTED_SCORE_UNION_ALL_COMPANIES_by_Contract_Type_new
                //WKP_Report2.OML_WEIGHTED_SCORE_UNION_COMPANIES_BY_CONTRACT_TYPE__JV = _context.WP_OML_WEIGHTED_SCORE_UNION_ALL_COMPANIES_by_Contract_Type_news.Where(x=>x.Year_of_WP == year &&  x.Contract_Type == GeneralModel.JV).ToList().orderbyweightedscore;
                //WKP_Report2.OML_WEIGHTED_SCORE_UNION_COMPANIES_BY_CONTRACT_TYPE__JV = _context.WP_OML_WEIGHTED_SCORE_UNION_ALL_COMPANIES_by_Contract_Type_news.Where(x=>x.Year_of_WP == year &&  x.Contract_Type == GeneralModel.PSC).ToList();
                //WKP_Report2.OML_WEIGHTED_SCORE_UNION_COMPANIES_BY_CONTRACT_TYPE__JV = _context.WP_OML_WEIGHTED_SCORE_UNION_ALL_COMPANIES_by_Contract_Type_news.Where(x=>x.Year_of_WP == year &&  x.Contract_Type == GeneralModel.SR).ToList();
                //WKP_Report2.OML_WEIGHTED_SCORE_UNION_COMPANIES_BY_CONTRACT_TYPE__JV = _context.WP_OML_WEIGHTED_SCORE_UNION_ALL_COMPANIES_by_Contract_Type_news.Where(x=>x.Year_of_WP == year &&  x.Contract_Type == GeneralModel.MF).ToList();
                 
                 //err WKP_Report2.OML_Aggregated_Score_ALL_COMPANIES = _context.WP_OML_Aggregated_Score_ALL_COMPANIEs.Where(x => x.Year_of_WP == year).ToList().OrderByDescending(x => x.OML_Aggregated_Score);
               
                 WKP_Report2.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_CURRENT_PLANNED = _context.WP_RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE_CURRENT_PLANNEDs.Where(x => x.Fiveyear_Projection_Year == proposedYear).ToList();

                 WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_PROPOSED =       _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_PROPOSEDs.Where(x => x.Year_of_WP == proposedYear).ToList();
                
                 WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_C_TYPE_PROPOSED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE_PROPOSEDs.Where(x => x.Year_of_WP == proposedYear).ToList();
                 
                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_By_month_year_PROPOSED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_By_month_year_PROPOSEDs.Where(x => x.Year_of_WP == proposedYear).ToList();
               
                //error WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrain_PLANNED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrain_PLANNEDs.Where(x => x.Year_of_WP == proposedYear).ToList();
                
                WKP_Report2.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_Pivotted_PRODUCTION_BRKDWN_PLANNED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_Pivotted_by_company_productionmonth_year_breakdown_PLANNEDs.Where(x => x.Year_of_WP == proposedYear).ToList();
               
                //error WKP_Report2.GAS_PRODUCTION_ACTIVITIES_produced_utilized_flared_PLANNED = _context.WP_GAS_PRODUCTION_ACTIVITIES_produced_utilized_flared_PLANNEDs.Where(x => x.Year_of_WP == proposedYear).ToList();
                
                WKP_Report2.GAS_PRODUCTION_ACTIVITIES_contract_type_basis_PLANNED = _context.WP_GAS_PRODUCTION_ACTIVITIES_contract_type_basis_PLANNEDs.Where(x => x.Year_of_WP == proposedYear).ToList();
             
                //error WKP_Report2.RESERVES_REPLACEMENT_RATIO_VALUE_PIVOTTED = _context.WP_RESERVES_REPLACEMENT_RATIO_VALUE_PIVOTTEDs.ToList();

                WKP_Report2.HSE_CAUSES_OF_SPILL = _context.HSE_CAUSES_OF_SPILLs.Where(x=> x.Year_of_WP == year ).ToList();

            }
         
            catch (Exception e)
            {

                WKP_Report2.Error = "Error : " + e.Message;

            }
            
            return WKP_Report2;
        }
        #endregion



        [HttpGet("CONCESSIONSINFORMATION")]
        public async Task<WebApiResponse> Get_ADMIN_CONCESSIONS_INFORMATION_BY_CURRENT_YEAR(string year)
        {

            try { 
            var dateYear = DateTime.Now.AddYears(0).ToString("yyyy");
            var ConcessionsInformation = new List<ADMIN_CONCESSIONS_INFORMATION>();

            if (WKUserRole == GeneralModel.Admin)
            {
                ConcessionsInformation =await _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(c => c.Year == dateYear && c.DELETED_STATUS == null).ToListAsync();
            } 
            else
            {
                ConcessionsInformation =await _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(c => c.Year == dateYear && c.Company_ID == WKPCompanyId && c.DELETED_STATUS == null).ToListAsync();
            }

            if (year != null)
            {
                ConcessionsInformation =await _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(c => c.Year == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ConcessionsInformation, StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }

        [HttpGet("REPORTS_YEARLIST")]
        public async Task<object> REPORTS_YEARLIST()
        {
            int portalDate =int.Parse( _configuration.GetSection("AppSettings").GetSection("portalDate").Value.ToString()); 
            var yearList = from n in Enumerable.Range(0, (DateTime.Now.Year - portalDate) + 1)
                             select (DateTime.Now.Year -  n).ToString();

            return yearList;
        }

        [HttpGet("CONCESSIONSITUATION")]
        public async Task<WebApiResponse> Get_CONCESSION_SITUATION(string year )
        {

            var ConcessionSituation = new List<CONCESSION_SITUATION>();
            try { 

                if (WKUserRole == GeneralModel.Admin)
                {
                    ConcessionSituation =await _context.CONCESSION_SITUATIONs.Where(c => c.Year == year).ToListAsync();
                }
                else
                {
                    ConcessionSituation =await _context.CONCESSION_SITUATIONs.Where(c => c.Year == year && c.COMPANY_ID == WKPCompanyId && c.Year == year).ToListAsync();
                }


                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ConcessionSituation.OrderBy(x => x.Year), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }
        
        [HttpGet("GEOPHYSICALACTIVITIES")]
        public async Task<WebApiResponse> Get_GEOPHYSICAL_ACTIVITIES_ACQUISITION(string year )
        {
            var GEOPHYSICALACTIVITIES = new List<GEOPHYSICAL_ACTIVITIES_ACQUISITION>();
            try
            {
                if (WKUserRole == GeneralModel.Admin)
                {
                    GEOPHYSICALACTIVITIES =await _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(c => c.Year_of_WP == year).ToListAsync();
                }
                else
                {
                    GEOPHYSICALACTIVITIES =await _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
                }


                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GEOPHYSICALACTIVITIES.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
      
      
        [HttpGet("GEOPHYSICALPROCESSING")]
        public async Task<WebApiResponse> Get_GEOPHYSICAL_ACTIVITIES_PROCESSING(string year )
        {

            var GEOPHYSICALACTIVITIES = new List<GEOPHYSICAL_ACTIVITIES_PROCESSING>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                GEOPHYSICALACTIVITIES =await _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(c => c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                GEOPHYSICALACTIVITIES =await _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GEOPHYSICALACTIVITIES.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }


        [HttpGet("DRILLING-OPERATIONS")]
        public async Task<WebApiResponse> Get_DRILLING_OPERATIONS_CATEGORIES_OF_WELLS(string year )
        {

            var DRILLING_OPERATIONS = new List<DRILLING_OPERATIONS_CATEGORIES_OF_WELL>();
            try { 
                if (WKUserRole == GeneralModel.Admin)
                {
                    DRILLING_OPERATIONS =await _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(c => c.Year_of_WP == year).ToListAsync();
                }
                else
                {
                    DRILLING_OPERATIONS =await _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
                }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = DRILLING_OPERATIONS.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }

        
        [HttpGet("WORKOVERS_RECOMPLETION")]
        public async Task<WebApiResponse> Get_WORKOVERS_RECOMPLETION_JOBs(string year )
        {
            var WORKOVERS_RECOMPLETION = new List<WORKOVERS_RECOMPLETION_JOB1>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                WORKOVERS_RECOMPLETION =await _context.WORKOVERS_RECOMPLETION_JOBs1.Where(c => c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                WORKOVERS_RECOMPLETION =await _context.WORKOVERS_RECOMPLETION_JOBs1.Where(c => c.Year_of_WP == year && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = WORKOVERS_RECOMPLETION.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }

         
        [HttpGet("INITIAL_WELLCOMPLETION")]
        public async Task<WebApiResponse> Get_INITIAL_WELL_COMPLETION(string year )
        {
            var INITIAL_WELLCOMPLETION = new List<INITIAL_WELL_COMPLETION_JOB1>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                INITIAL_WELLCOMPLETION =await _context.INITIAL_WELL_COMPLETION_JOBs1.Where(c => c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                INITIAL_WELLCOMPLETION =await _context.INITIAL_WELL_COMPLETION_JOBs1.Where(c => c.Year_of_WP == year && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = INITIAL_WELLCOMPLETION.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }


        [HttpGet("FDP_EXPECTED_RESERVES")]
        public async Task<WebApiResponse> Get_FIELD_DEVELOPMENT_PLAN_EXPECTED_RESERVES(string year )
        {
            var FDP_Reserves = new List<FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERf>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                FDP_Reserves =await _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.Where(c=> c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                FDP_Reserves =await _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = FDP_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }


        [HttpGet("FDP_TOSUBMIT")]
        public async Task<WebApiResponse> Get_FIELD_DEVELOPMENT_PLAN_TOBESUBMITTED(string year )
        {
            var FDP_Reserves = new List<FIELD_DEVELOPMENT_PLAN>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                FDP_Reserves =await _context.FIELD_DEVELOPMENT_PLANs.Where(c => c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                FDP_Reserves =await _context.FIELD_DEVELOPMENT_PLANs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            
            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = FDP_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }


        [HttpGet("FDP_FIELDSTATUS")]
        public async Task<WebApiResponse> FIELD_DEVELOPMENT_FIELDS_AND_STATUS(string year )
        {
            var FDP_Fields = new List<FIELD_DEVELOPMENT_FIELDS_AND_STATUS>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                FDP_Fields =await _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.Where(c=> c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                FDP_Fields =await _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = FDP_Fields.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }


        [HttpGet("NDR")]
        public async Task<WebApiResponse> NDR(string year )
        {
            var NDR = new List<NDR>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                NDR =await _context.NDRs.Where(c=> c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                NDR =await _context.NDRs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            
            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = NDR.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("OIL_CONDENSATE_PRODUCTION_ACTIVITIES")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES(string year )
        {
            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITy>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.Where(c=> c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                OilCondensate =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            
            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }

        
        [HttpGet("OIL_CONDENSATE_MONTHLY_ACTIVITIES")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities(string year )
        {
            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.Where(c=> c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                OilCondensate =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("OIL_CONDENSATE_MONTHLY_ACTIVITIES_PROPOSED")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED(string year )
        {
            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.Where(c => c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                OilCondensate =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("OIL_CONDENSATE_MONTHLY_ACTIVITIES_PROPOSED_FIVEYEARS")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION(string year )
        {
            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.Where(c => c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                OilCondensate =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("GAS_PRODUCTION_ACTIVITIES")]
        public async Task<WebApiResponse> GAS_PRODUCTION_ACTIVITIES(string year )
        {
            var GasProduction = new List<GAS_PRODUCTION_ACTIVITy>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                GasProduction =await _context.GAS_PRODUCTION_ACTIVITIEs.Where(c => c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                GasProduction =await _context.GAS_PRODUCTION_ACTIVITIEs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GasProduction.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY")]
        public async Task<WebApiResponse> GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY(string year )
        {

            var GasProduction = new List<GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                GasProduction =await _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.Where(c => c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                GasProduction =await _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GasProduction.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("UNITIZATION")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION(string year )
        {

            var GasProduction = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                GasProduction =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.Where(c => c.Year_of_WP == year).ToListAsync();
            }
            else
            {
                GasProduction =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GasProduction.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("CONCESSION_RESERVES_FOR_1ST_JANUARY")]
        public async Task<WebApiResponse> CONCESSION_RESERVES_FOR_1ST_JANUARY(string year )
        {
            var Concession = new List<RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                Concession =await _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Where(c =>c.Year_of_WP == year && c.FLAG1 == "COMPANY_RESERVE_OF_PRECEDDING_YEAR").ToListAsync();
            }

            else
            {
                Concession =await _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Where(c => c.FLAG1 == "COMPANY_RESERVE_OF_PRECEDDING_YEAR" && c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = Concession.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }

        [HttpGet("CONCESSION_RESERVES_FOR_CURRENT_YEAR")]
        public async Task<WebApiResponse> CONCESSION_RESERVES_FOR_CURRENT_YEAR(string year)
        {
            var Concession = new List<RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE>();
            try
            {
                if (WKUserRole == GeneralModel.Admin)
                {
                    Concession = _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Where(c => c.FLAG1 == "COMPANY_CURRENT_RESERVE").ToList();
                }

                else
                {
                    Concession = _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Where(c => c.FLAG1 == "COMPANY_CURRENT_RESERVE" && c.COMPANY_ID == WKPCompanyId).ToList();
                }

                if (year != null)
                {
                    Concession = Concession.Where(c => c.Year_of_WP == year).ToList();
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = Concession.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError }; ;
            }
        }


        [HttpGet("RESERVES_OIL_CONDENSATE_PRODUCTION")]
        public async Task<WebApiResponse> RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION(string year )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("RESERVES_ADDITION")]
        public async Task<WebApiResponse> RESERVES_ADDITION(string year )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_Reserves_Addition>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_Additions.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_Additions.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("RESERVES_DECLINE")]
        public async Task<WebApiResponse> RESERVES_DECLINE(string year )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("RESERVES_LIFE_INDEX")]
        public async Task<WebApiResponse> RESERVES_UPDATES_LIFE_INDEX(string year )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_LIFE_INDEX>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_LIFE_INDices.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_LIFE_INDices.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("RESERVES_UPDATES_DEPLETION_RATE")]
        public async Task<WebApiResponse> RESERVES_UPDATES_DEPLETION_RATE(string year )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_DEPLETION_RATE>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_DEPLETION_RATEs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_DEPLETION_RATEs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("RESERVES_OIL_CONDENSATE_MMBBL")]
        public async Task<WebApiResponse> RESERVES_UPDATES_OIL_CONDENSATE_MMBBL(string year )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_MMBBL>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_OIL_CONDENSATE_MMBBLs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                OilCondensate_Reserves =await _context.RESERVES_UPDATES_OIL_CONDENSATE_MMBBLs.Where(c => c.Companyemail == WKPCompanyId).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("RESERVES_REPLACEMENT_RATIO")]
        public async Task<WebApiResponse> RESERVES_REPLACEMENT_RATIO(string year )
        {

            var OilCondensate_Reserves = new List<RESERVES_REPLACEMENT_RATIO>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves =await _context.RESERVES_REPLACEMENT_RATIOs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                OilCondensate_Reserves =await _context.RESERVES_REPLACEMENT_RATIOs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("BUDGET_CAPEX_OPEX")]
        public async Task<WebApiResponse> BUDGET_CAPEX_OPEX(string year )
        {
            var BudgetCapex = new List<BUDGET_CAPEX_OPEX>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                BudgetCapex =await _context.BUDGET_CAPEX_OPices.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                BudgetCapex =await _context.BUDGET_CAPEX_OPices.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            
            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = BudgetCapex.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("OIL_AND_GAS__MAINTENANCE_PROJECTS")]
        public async Task<WebApiResponse> OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS(string year )
        {
            var BudgetCapex = new List<OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                BudgetCapex =await _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                BudgetCapex =await _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = BudgetCapex.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("OIL_CONDENSATE_CONFORMITY")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment(string year )
        {
            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                OilCondensate =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                OilCondensate =await _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("FACILITIES_PROJECT_PERFORMANCE")]
        public async Task<WebApiResponse> FACILITIES_PROJECT_PERFORMANCE(string year )
        {
            var ResultData = new List<FACILITIES_PROJECT_PERFORMANCE>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.FACILITIES_PROJECT_PERFORMANCEs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.FACILITIES_PROJECT_PERFORMANCEs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("LEGAL_LITIGATION")]
        public async Task<WebApiResponse> LEGAL_LITIGATION(string year )
        {
            var ResultData = new List<LEGAL_LITIGATION>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.LEGAL_LITIGATIONs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.LEGAL_LITIGATIONs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
         
        
        [HttpGet("LEGAL_ARBITRATION")]
        public async Task<WebApiResponse> LEGAL_ARBITRATION(string year )
        {
            var ResultData = new List<LEGAL_ARBITRATION>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.LEGAL_ARBITRATIONs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.LEGAL_ARBITRATIONs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("NIGERIA_CONTENT_TRAINING")]
        public async Task<WebApiResponse> NIGERIA_CONTENT_TRAINING(string year )
        {
            var ResultData = new List<NIGERIA_CONTENT_Training>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.NIGERIA_CONTENT_Trainings.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.NIGERIA_CONTENT_Trainings.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("NIGERIA_CONTENT_SUCCESSIONPLAN")]
        public async Task<WebApiResponse> NIGERIA_CONTENT_Upload_Succession_Plan(string year )
        {
            var ResultData = new List<NIGERIA_CONTENT_Upload_Succession_Plan>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.NIGERIA_CONTENT_Upload_Succession_Plans.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.NIGERIA_CONTENT_Upload_Succession_Plans.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("STRATEGIC_PLANS_ON_COMPANY_BASIS")]
        public async Task<WebApiResponse> STRATEGIC_PLANS_ON_COMPANY_BASIS(string year )
        {
            var ResultData = new List<STRATEGIC_PLANS_ON_COMPANY_BASI>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.STRATEGIC_PLANS_ON_COMPANY_BAses.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.STRATEGIC_PLANS_ON_COMPANY_BAses.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW")]
        public async Task<WebApiResponse> HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW(string year )
        {
            var ResultData = new List<HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("HSE_MANAGEMENT_POSITION")]
        public async Task<WebApiResponse> HSE_MANAGEMENT_POSITION(string year )
        {
            var ResultData = new List<HSE_MANAGEMENT_POSITION>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.HSE_MANAGEMENT_POSITIONs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.HSE_MANAGEMENT_POSITIONs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("HSE_SAFETY_CULTURE_TRAINING")]
        public async Task<WebApiResponse> HSE_SAFETY_CULTURE_TRAINING(string year )
        {
            var ResultData = new List<HSE_SAFETY_CULTURE_TRAINING>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.HSE_SAFETY_CULTURE_TRAININGs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.HSE_SAFETY_CULTURE_TRAININGs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("HSE_OCCUPATIONAL_HEALTH_MANAGEMENT")]
        public async Task<WebApiResponse> HSE_OCCUPATIONAL_HEALTH_MANAGEMENT(string year )
        {
            var ResultData = new List<HSE_OCCUPATIONAL_HEALTH_MANAGEMENT>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("HSE_QUALITY_CONTROL")]
        public async Task<WebApiResponse> HSE_QUALITY_CONTROL(string year )
        {
            var ResultData = new List<HSE_QUALITY_CONTROL>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.HSE_QUALITY_CONTROLs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.HSE_QUALITY_CONTROLs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
          
        
        [HttpGet("HSE_CLIMATE_CHANGE_AND_AIR_QUALITY")]
        public async Task<WebApiResponse> HSE_CLIMATE_CHANGE_AND_AIR_QUALITY(string year )
        {
            var ResultData = new List<HSE_CLIMATE_CHANGE_AND_AIR_QUALITY>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }
        
        
        [HttpGet("HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW")]
        public async Task<WebApiResponse> HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW(string year )
        {
            var ResultData = new List<HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW>();
            try { 
            if (WKUserRole == GeneralModel.Admin)
            {
                ResultData =await _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.Where(c => c.Year_of_WP == year).ToListAsync();
            }

            else
            {
                ResultData =await _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.Where(c => c.COMPANY_ID == WKPCompanyId && c.Year_of_WP == year).ToListAsync();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error :  " + e.Message, StatusCode = ResponseCodes.InternalError };;
            }
        }

        }

    }