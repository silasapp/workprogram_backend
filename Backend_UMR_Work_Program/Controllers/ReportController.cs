﻿using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using AutoMapper;
using System.Security.Claims;

namespace Backend_UMR_Work_Program.Controllers
{
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
        //_httpContextAccessor = httpContextAccessor;
        _account = account;
        _context = context;
        _configuration = configuration;
         _mapper = mapper;
        _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor, _mapper);
    }
        private string? WKPUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string? WKPUserName => User.FindFirstValue(ClaimTypes.Name);
        private string? WKPUserEmail => User.FindFirstValue(ClaimTypes.Email);
        private string? WKPUserRole => User.FindFirstValue(ClaimTypes.Role);



        [HttpGet("WORKPROGRAMME_REPORT")]
        public async Task<WebApiResponse> WORKPROGRAMME_REPORT(string year = null)
        {
            try
            {
                WorkProgrammeReport_Model WorkProgrammeReport = new WorkProgrammeReport_Model();

                string previousYear = year !=null ? (int.Parse(year) - 1).ToString(): "";

                var WKP_Report = _context.ADMIN_WORK_PROGRAM_REPORTs.Where(x => x.Id <= 5).ToList();
              
                if (year != null)
                {
                    var E_and_P_companies = _context.WP_COUNT_ADMIN_DATETIME_PRESENTATION_BY_TOTAL_COUNT_YEARLies.Where(x => x.YEAR == year).ToList();

                    var WP_COUNT = _context.WP_COUNT_ADMIN_DATETIME_PRESENTATION_BY_YEAR_PRESENTED_CATEGORies.Where(x => x.Year == year && (x.PRESENTED == GeneralModel.Presented || x.PRESENTED == GeneralModel.FailedToShow || x.PRESENTED == GeneralModel.NotInvited || x.PRESENTED == GeneralModel.ShowedButNoPresentation)).ToList();
                   
                    var WP_COUNT_GEOPHYSICAL_ACTIVITIES_ACQUISITION = _context.WP_GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(x => x.Year_of_WP == year && x.Geo_type_of_data_acquired == GeneralModel.ThreeD).ToList();

                    var WP_COUNT_GEOPHYSICAL_ACTIVITIES_PROCESSING_3D = _context.WP_GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(x => x.Year_of_WP == year && (x.Geo_Type_of_Data_being_Processed == GeneralModel.ThreeD || x.Geo_Type_of_Data_being_Processed == GeneralModel.TwoD)).ToList();

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
                    //var WP_COUNT_FirstAppraisal = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.FirstAppraisal).ToList();
                    //var WP_COUNT_SecondAppraisal = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.SecondAppraisal).ToList();
                    //var WP_COUNT_Appraisal = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.Appraisal).ToList();
                    //var WP_COUNT_OrdinaryAppraisal = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.Appraisal).ToList();

                    var WP_JV_Contract_Type = (from u in _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Contract_Types
                                               where u.Year_of_WP == year && u.Contract_Type == GeneralModel.JV
                                               select u).ToList().GroupBy(x => x.Contract_Type).Select(x => x.FirstOrDefault()).ToList();

                    //var WP_JV_Contract_Type = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.JV).ToList();
                    //string JV_Total_Reconciled_National_Crude_Oil_Production = WP_JV_Contract_Type.FirstOrDefault().Total_Reconciled_National_Crude_Oil_Production.ToString();
                    //var WP_MF_Contract_Type = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.MF).ToList();
                    //string MF_Total_Reconciled_National_Crude_Oil_Production = WP_MF_Contract_Type.FirstOrDefault().Total_Reconciled_National_Crude_Oil_Production.ToString();
                    //var WP_PSC_Contract_Type = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.PSC).ToList();
                    //string PSC_Total_Reconciled_National_Crude_Oil_Production = WP_PSC_Contract_Type.FirstOrDefault().Total_Reconciled_National_Crude_Oil_Production.ToString();
                    //var WP_SR_Contract_Type = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.SR).ToList();
                    //string SR_Total_Reconciled_National_Crude_Oil_Production = WP_SR_Contract_Type.FirstOrDefault().Total_Reconciled_National_Crude_Oil_Production.ToString();

                    var WP_CRUDE_OIL = (from u in _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs where u.Year_of_WP == previousYear || u.Year_of_WP == year
                                        group u by new{ u.Year_of_WP } into g select new {
                                                         Year = g.Key,
                                                         Total_Reconciled_National_Crude_Oil_Production = g.Sum(c=> Convert.ToInt64(Convert.ToDouble(c.Total_Reconciled_National_Crude_Oil_Production))),
                                                     }).ToList();
                    var WP_CRUDE_OIL_PY = WP_CRUDE_OIL.Where(x => x.Year.ToString() == previousYear).ToList();
                    var WP_CRUDE_OIL_CY = WP_CRUDE_OIL.Where(x => x.Year.ToString() == year).ToList();
                    //CONVERT(INT, CONVERT(VARCHAR(12), a.value))
                    
                    var WP_OIL_PRODUCTION_total_barrel = (from u in _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPEs
                                                          where u.Year_of_WP == year select u).ToList().GroupBy(x => x.Contract_Type).Select(x => x.FirstOrDefault()).ToList();

                    var WP_Terrain_Continental = (from u in _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrains
                                                  where u.Year_of_WP == year select u).ToList().GroupBy(x => x.Terrain).Select(x => x.FirstOrDefault()).ToList();
                    #region Gas Production Activities For Previous Year 
                    //var PY_GAS_ACTIVITIES = (from u in _context.WP_GAS_PRODUCTION_ACTIVITIES_Percentages
                    //                            where u.Year_of_WP == previousYear
                    //                            select new 
                    //                            {
                    //                                Actual_Total_Gas_Produced = u.Actual_Total_Gas_Produced,
                    //                                Utilized_Gas_Produced = u.Utilized_Gas_Produced,
                    //                                Percentage_Utilized = u.Percentage_Utilized > 0 ? Decimal.Round(Convert.ToDecimal(u.Percentage_Utilized), 2) : u.Percentage_Utilized,
                    //                                Flared_Gas_Produced = u.Flared_Gas_Produced
                    //                            }).ToList();
                    //var PY_GAS_PRODUCTION = (from u in _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types
                    //                         where u.Year_of_WP == previousYear
                    //                         select new WP_GAS_PRODUCTION_ACTIVITIES_Contract_Type
                    //                         {
                    //                             Actual_Total_Gas_Produced = u.Actual_Total_Gas_Produced,
                    //                             Utilized_Gas_Produced = u.Utilized_Gas_Produced,
                    //                             Flared_Gas_Produced = u.Flared_Gas_Produced
                    //                         }).GroupBy(x => x.Contract_Type).Select(x => x.FirstOrDefault()).ToList();


                   
                    #endregion

                    #region Gas Production Activities For Current Year 
                    var WP_GAS_ACTIVITIES_CY = (from u in _context.WP_GAS_PRODUCTION_ACTIVITIES_Percentages
                                                where u.Year_of_WP == year
                                                select new WP_GAS_PRODUCTION_ACTIVITIES_Percentage
                                                {
                                                    Actual_Total_Gas_Produced = u.Actual_Total_Gas_Produced,
                                                    Utilized_Gas_Produced = u.Utilized_Gas_Produced,
                                                    Percentage_Utilized = u.Percentage_Utilized> 0? Decimal.Round(Convert.ToDecimal(u.Percentage_Utilized), 2): u.Percentage_Utilized,
                                                    Flared_Gas_Produced = u.Flared_Gas_Produced
                                                }).ToList();
                    //string Actual_Total_Gas_Produced_CY = WP_GAS_ACTIVITIES_CY.Actual_Total_Gas_Produced.ToString();
                    //string Utilized_Gas_Produced_CY = WP_GAS_ACTIVITIES_CY.Utilized_Gas_Produced.ToString();
                    //string Flared_Gas_Produced_CY = WP_GAS_ACTIVITIES_CY.Flared_Gas_Produced.ToString();
                    //string Percentage_Utilized_CY = WP_GAS_ACTIVITIES_CY.Percentage_Utilized.ToString();
                    //Decimal CY_GasPercentage = Decimal.Parse(Percentage_Utilized_CY);
                    //CY_GasPercentage = Decimal.Round(CY_GasPercentage, 2);
                    //Percentage_Utilized_CY = CY_GasPercentage.ToString();

                    //var CY_GAS_PRODUCTION = (from u in _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types
                    //                         where u.Year_of_WP == year
                    //                         select new WP_GAS_PRODUCTION_ACTIVITIES_Contract_Type
                    //                         {
                    //                             Actual_Total_Gas_Produced = u.Actual_Total_Gas_Produced,
                    //                             Utilized_Gas_Produced = u.Utilized_Gas_Produced,
                    //                             Flared_Gas_Produced = u.Flared_Gas_Produced
                    //                         }).GroupBy(x => x.Contract_Type).Select(x => x.FirstOrDefault()).ToList();


                    //var CY_GAS_PRODUCTION_JV = _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.JV).ToList();
                    //string JV_Total_Gas_Produced_CY = WP_GAS_PRODUCTION_JV.FirstOrDefault().Actual_Total_Gas_Produced.ToString();

                    //var CY_GAS_PRODUCTION_MF = _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.MF).ToList();
                    //string MF_Total_Gas_Produced_CY = WP_GAS_PRODUCTION_MF.FirstOrDefault().Actual_Total_Gas_Produced.ToString();

                    //var CY_GAS_PRODUCTION_PSC = _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.PSC).ToList();
                    //string PSC_Total_Gas_Produced_CY = WP_GAS_PRODUCTION_PSC.FirstOrDefault().Actual_Total_Gas_Produced.ToString();

                    #endregion

                    var OIL_CONDENSATE_MMBBL = _context.WP_RESERVES_UPDATES_OIL_CONDENSATE_MMBBLs.Where(x => x.Year_of_WP == year).ToList();
                    //string Reserves_as_at_MMbbl = OIL_CONDENSATE_MMBBL.Reserves_as_at_MMbbl.ToString();
                    //string Reserves_as_at_MMbbl_gas = OIL_CONDENSATE_MMBBL.Reserves_as_at_MMbbl_gas.ToString();
                    //string Reserves_as_at_MMbbl_condensate = OIL_CONDENSATE_MMBBL.Reserves_as_at_MMbbl_condensate.ToString();

                    #region HSE Accident
                    var HSE_ACCIDENT_Consequences = _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_by_consequences.Where(x => x.Year_of_WP == year && x.Consequence == GeneralModel.Fatality).ToList();
                    //string Sum_accident = HSE_ACCIDENT_Consequences.sum_accident.ToString();

                    var HSE_ACCIDENT_Total = _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_accidents.Where(x => x.Year_of_WP == year).ToList();
                    //string Sum_accident_total = HSE_ACCIDENT_Total.Sum_accident.ToString();

                    //var HSE_ACCIDENT = (from u in _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_spill_accident_and_percentages
                    //                    where u.Year_of_WP == year
                    //                    select new WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_spill_accident_and_percentage
                    //                    {
                    //                        sum_accident = u.sum_accident,
                    //                        Percentage_Spill = u.Percentage_Spill,
                    //                    }).GroupBy(x => x.Cause).Select(x => x.FirstOrDefault()).ToList();

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
                                                TOTAL_QUANTITY_SPILLED = g.Sum(x => Convert.ToInt32(x.Total_Quantity_Spilled)),
                                                CompanyName = g.Key,
                                                Year_of_WP = g.FirstOrDefault().Year_of_WP,

                                            }).OrderBy(x => x.Frequency).Take(5);

                    //var Oil_Spill_REPORT_1st = Oil_Spill_REPORT.Take(1);
                    //var Oil_Spill_REPORT_2nd = Oil_Spill_REPORT.Take(2);
                    //var Oil_Spill_REPORT_3rd = Oil_Spill_REPORT.Take(3);
                    //var Oil_Spill_REPORT_4th = Oil_Spill_REPORT.Take(4);
                    //var Oil_Spill_REPORT_5th = Oil_Spill_REPORT.Take(5);

                    var QUANTITY_RECOVERED = (from o in _context.HSE_CAUSES_OF_SPILLs
                                              where o.Year_of_WP == year
                                              select o).ToList();

                    //var Total_no_of_QUANTITY_RECOVERED = QUANTITY_RECOVERED.Sum(x => int.Parse(x.Total_Quantity_Recovered));
                    //var Total_no_of_QUANTITY_SPILLED = QUANTITY_RECOVERED.Sum(x => int.Parse(x.Total_Quantity_Spilled));
                    //var TOTAL_PRODUCED_WATER = _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.Where(x => x.Year_of_WP == year).Sum(x => int.Parse(x.Produced_water_volumes));

                    var PRODUCTION_TOTAL_BARREL_PROPOSED = (from u in _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE_PROPOSEDs
                                                        where u.Year_of_WP == year
                                                        select u).GroupBy(x => x.Contract_Type).Select(x => x.FirstOrDefault()).ToList();
                    
                    var PLANNED_TERRAIN = (from u in _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrain_PLANNEDs
                                           where u.Year_of_WP == year select u).GroupBy(x => x.Terrain).Select(x => x.FirstOrDefault()).ToList();
                
                    
                    var FLARINGFORECAST = _context.WP_Gas_Production_Utilisation_And_Flaring_Forecasts.Where(x => x.Year_of_WP == year).ToList();


                    WorkProgrammeReport.ADMIN_WORK_PROGRAM_REPORT_Model = WKP_Report;
                    WorkProgrammeReport.E_and_P_companies_Model = E_and_P_companies;
                    WorkProgrammeReport.WP_Presentations_Model = WP_COUNT;
                    WorkProgrammeReport.WP_GEOPHYSICAL_ACTIVITIES_ACQUISITION_Model = WP_COUNT_GEOPHYSICAL_ACTIVITIES_ACQUISITION;
                    WorkProgrammeReport.WP_GEOPHYSICAL_ACTIVITIES_PROCESSING_Model = WP_COUNT_GEOPHYSICAL_ACTIVITIES_PROCESSING_3D;
                    WorkProgrammeReport.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELL_Model =  WP_COUNT_WELLS;
                    WorkProgrammeReport.WP_SUM_APPRAISAL_WELL_Model = WP_SUM_APPRAISAL_WELLS;
                    WorkProgrammeReport.WP_SUM_INITIAL_WELL_COMPLETION_JOBS_WORKOVERS_RECOMPLETION_Model = WP_SUM_WORKOVERS_RECOMPLETION;
                    WorkProgrammeReport.DRILLING_OPERATIONS_Appraisal_Model = WP_COUNT_Appraisal;
                    WorkProgrammeReport.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITY_Contract_Type_Model = WP_JV_Contract_Type;
                    WorkProgrammeReport.WP_GAS_PRODUCTION_ACTIVITY_CURRENTYEAR_Model = WP_CRUDE_OIL_CY;
                    WorkProgrammeReport.WP_GAS_PRODUCTION_ACTIVITY_PREVIOUSYEAR_Model = WP_CRUDE_OIL_PY;
                    WorkProgrammeReport.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE_Model = WP_OIL_PRODUCTION_total_barrel;
                    WorkProgrammeReport.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITY_monthly_ActivitY_OIL_PRODUCTION_by_Terrain_Model = WP_Terrain_Continental;
                    //WorkProgrammeReport.WP_GAS_PRODUCTION_ACTIVITY_Percentage_Model = PY_GAS_ACTIVITIES;
                    //WorkProgrammeReport.WP_GAS_PRODUCTION_Model = PY_GAS_PRODUCTION;
                    WorkProgrammeReport.WP_RESERVES_UPDATES_OIL_CONDENSATE_MMBBL_Model = OIL_CONDENSATE_MMBBL;
                    WorkProgrammeReport.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_by_consequence_Model = HSE_ACCIDENT_Consequences;
                    WorkProgrammeReport.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_accident_Model = HSE_ACCIDENT_Total;
                    //WorkProgrammeReport.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_spill_accident_and_percentage_Model = HSE_ACCIDENT;
                    WorkProgrammeReport.GEO_sum_and_count_Model = GEO_ACTIVITIES;
                    WorkProgrammeReport.VOLUME_OF_OILSPILL = HSE_VOLUME_OF_OILSPILL.ToString();
                    WorkProgrammeReport.OILSPILL_REPORT_Model = OILSPILL_REPORT;
                    WorkProgrammeReport.HSE_CAUSES_OF_SPILL_Model = QUANTITY_RECOVERED;
                    //WorkProgrammeReport.TOTAL_PRODUCED_WATER_Model = TOTAL_PRODUCED_WATER.ToString();
                    WorkProgrammeReport.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITY_monthly_ActivitY_OIL_PRODUCTION_by_Terrain_PLANNED_Model = PLANNED_TERRAIN;
                    WorkProgrammeReport.WP_Gas_Production_Utilisation_And_Flaring_Forecast_Model = FLARINGFORECAST;

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = WorkProgrammeReport, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Year wasn't passed correctly", StatusCode = ResponseCodes.Failure };
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
            
        }

        [HttpGet("CONCESSIONSINFORMATION")]
        public async Task<WebApiResponse> Get_ADMIN_CONCESSIONS_INFORMATION_BY_CURRENT_YEAR(string year = null)
        {

            try { 
            var dateYear = DateTime.Now.AddYears(0).ToString("yyyy");
            var ConcessionsInformation = new List<ADMIN_CONCESSIONS_INFORMATION>();

            if (WKPUserRole == GeneralModel.Admin)
            {
                ConcessionsInformation = _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(c => c.Year == dateYear && c.DELETED_STATUS == null).ToList();
            }
            else
            {
                ConcessionsInformation = _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(c => c.Year == dateYear && c.Company_ID == WKPUserId && c.DELETED_STATUS == null).ToList();
            }

            if (year != null)
            {
                ConcessionsInformation = _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(c => c.Year == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ConcessionsInformation, StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }

        [HttpGet("CONCESSIONSITUATION")]
        public async Task<WebApiResponse> Get_CONCESSION_SITUATION(string year = null )
        {

            var ConcessionSituation = new List<CONCESSION_SITUATION>();
            try { 

                if (WKPUserRole == GeneralModel.Admin)
                {
                    ConcessionSituation = _context.CONCESSION_SITUATIONs.Where(c => c.Year == year).ToList();
                }
                else
                {
                    ConcessionSituation = _context.CONCESSION_SITUATIONs.Where(c => c.Year == year && c.COMPANY_ID == WKPUserId).ToList();
                }


                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ConcessionSituation.OrderBy(x => x.Year), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }

        
        [HttpGet("GEOPHYSICALACTIVITIES")]
        public async Task<WebApiResponse> Get_GEOPHYSICAL_ACTIVITIES_ACQUISITION(string year = null )
        {
            var GEOPHYSICALACTIVITIES = new List<GEOPHYSICAL_ACTIVITIES_ACQUISITION>();
            try
            {
                if (WKPUserRole == GeneralModel.Admin)
                {
                    GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(c => c.Year_of_WP == year).ToList();
                }
                else
                {
                    GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == WKPUserId).ToList();
                }


                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GEOPHYSICALACTIVITIES.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
      
      
        [HttpGet("GEOPHYSICALPROCESSING")]
        public async Task<WebApiResponse> Get_GEOPHYSICAL_ACTIVITIES_PROCESSING(string year = null )
        {

            var GEOPHYSICALACTIVITIES = new List<GEOPHYSICAL_ACTIVITIES_PROCESSING>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == WKPUserId).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GEOPHYSICALACTIVITIES.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }


        [HttpGet("DRILLING-OPERATIONS")]
        public async Task<WebApiResponse> Get_DRILLING_OPERATIONS_CATEGORIES_OF_WELLS(string year = null )
        {

            var DRILLING_OPERATIONS = new List<DRILLING_OPERATIONS_CATEGORIES_OF_WELL>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                DRILLING_OPERATIONS = _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                DRILLING_OPERATIONS = _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == WKPUserId).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = DRILLING_OPERATIONS.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }

        
        [HttpGet("WORKOVERS_RECOMPLETION")]
        public async Task<WebApiResponse> Get_WORKOVERS_RECOMPLETION_JOBs(string year = null )
        {
            var WORKOVERS_RECOMPLETION = new List<WORKOVERS_RECOMPLETION_JOB1>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                WORKOVERS_RECOMPLETION = _context.WORKOVERS_RECOMPLETION_JOBs1.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                WORKOVERS_RECOMPLETION = _context.WORKOVERS_RECOMPLETION_JOBs1.Where(c => c.Year_of_WP == year && c.COMPANY_ID == WKPUserId).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = WORKOVERS_RECOMPLETION.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }

         
        [HttpGet("INITIAL_WELLCOMPLETION")]
        public async Task<WebApiResponse> Get_INITIAL_WELL_COMPLETION(string year = null )
        {
            var INITIAL_WELLCOMPLETION = new List<INITIAL_WELL_COMPLETION_JOB1>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                INITIAL_WELLCOMPLETION = _context.INITIAL_WELL_COMPLETION_JOBs1.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                INITIAL_WELLCOMPLETION = _context.INITIAL_WELL_COMPLETION_JOBs1.Where(c => c.Year_of_WP == year && c.COMPANY_ID == WKPUserId).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = INITIAL_WELLCOMPLETION.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }


        [HttpGet("FDP_EXPECTED_RESERVES")]
        public async Task<WebApiResponse> Get_FIELD_DEVELOPMENT_PLAN_EXPECTED_RESERVES(string year = null )
        {
            var FDP_Reserves = new List<FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERf>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                FDP_Reserves = _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.ToList();
            }
            else
            {
                FDP_Reserves = _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                FDP_Reserves = FDP_Reserves.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = FDP_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }


        [HttpGet("FDP_TOSUBMIT")]
        public async Task<WebApiResponse> Get_FIELD_DEVELOPMENT_PLAN_TOBESUBMITTED(string year = null )
        {
            var FDP_Reserves = new List<FIELD_DEVELOPMENT_PLAN>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                FDP_Reserves = _context.FIELD_DEVELOPMENT_PLANs.ToList();
            }
            else
            {
                FDP_Reserves = _context.FIELD_DEVELOPMENT_PLANs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                FDP_Reserves = FDP_Reserves.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = FDP_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }


        [HttpGet("FDP_FIELDSTATUS")]
        public async Task<WebApiResponse> FIELD_DEVELOPMENT_FIELDS_AND_STATUS(string year = null )
        {
            var FDP_Fields = new List<FIELD_DEVELOPMENT_FIELDS_AND_STATUS>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                FDP_Fields = _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.ToList();
            }
            else
            {
                FDP_Fields = _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                FDP_Fields = FDP_Fields.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = FDP_Fields.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }


        [HttpGet("NDR")]
        public async Task<WebApiResponse> NDR(string year = null )
        {
            var NDR = new List<NDR>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                NDR = _context.NDRs.ToList();
            }
            else
            {
                NDR = _context.NDRs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                NDR = NDR.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = NDR.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("OIL_CONDENSATE_PRODUCTION_ACTIVITIES")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES(string year = null )
        {
            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITy>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.ToList();
            }
            else
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate = OilCondensate.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }

        
        [HttpGet("OIL_CONDENSATE_MONTHLY_ACTIVITIES")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities(string year = null )
        {
            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.ToList();
            }
            else
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate = OilCondensate.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("OIL_CONDENSATE_MONTHLY_ACTIVITIES_PROPOSED")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED(string year = null )
        {
            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.ToList();
            }
            else
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate = OilCondensate.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("OIL_CONDENSATE_MONTHLY_ACTIVITIES_PROPOSED_FIVEYEARS")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION(string year = null )
        {
            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.ToList();
            }
            else
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate = OilCondensate.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("GAS_PRODUCTION_ACTIVITIES")]
        public async Task<WebApiResponse> GAS_PRODUCTION_ACTIVITIES(string year = null )
        {
            var GasProduction = new List<GAS_PRODUCTION_ACTIVITy>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                GasProduction = _context.GAS_PRODUCTION_ACTIVITIEs.ToList();
            }
            else
            {
                GasProduction = _context.GAS_PRODUCTION_ACTIVITIEs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                GasProduction = GasProduction.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GasProduction.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY")]
        public async Task<WebApiResponse> GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY(string year = null )
        {

            var GasProduction = new List<GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                GasProduction = _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.ToList();
            }
            else
            {
                GasProduction = _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                GasProduction = GasProduction.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GasProduction.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("UNITIZATION")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION(string year = null )
        {

            var GasProduction = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                GasProduction = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.ToList();
            }
            else
            {
                GasProduction = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                GasProduction = GasProduction.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GasProduction.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("CONCESSION_RESERVES_FOR_1ST_JANUARY")]
        public async Task<WebApiResponse> CONCESSION_RESERVES_FOR_1ST_JANUARY(string year = null )
        {
            var Concession = new List<RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                Concession = _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Where(c => c.FLAG1 == "COMPANY_RESERVE_OF_PRECEDDING_YEAR").ToList();
            }

            else
            {
                Concession = _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Where(c => c.FLAG1 == "COMPANY_RESERVE_OF_PRECEDDING_YEAR" && c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                Concession = Concession.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = Concession.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("RESERVES_OIL_CONDENSATE_PRODUCTION")]
        public async Task<WebApiResponse> RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION(string year = null )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate_Reserves = OilCondensate_Reserves.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("RESERVES_ADDITION")]
        public async Task<WebApiResponse> RESERVES_ADDITION(string year = null )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_Reserves_Addition>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_Additions.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_Additions.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate_Reserves = OilCondensate_Reserves.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("RESERVES_DECLINE")]
        public async Task<WebApiResponse> RESERVES_DECLINE(string year = null )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate_Reserves = OilCondensate_Reserves.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("RESERVES_LIFE_INDEX")]
        public async Task<WebApiResponse> RESERVES_UPDATES_LIFE_INDEX(string year = null )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_LIFE_INDEX>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_LIFE_INDices.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_LIFE_INDices.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate_Reserves = OilCondensate_Reserves.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("RESERVES_UPDATES_DEPLETION_RATE")]
        public async Task<WebApiResponse> RESERVES_UPDATES_DEPLETION_RATE(string year = null )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_DEPLETION_RATE>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_DEPLETION_RATEs.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_DEPLETION_RATEs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate_Reserves = OilCondensate_Reserves.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("RESERVES_OIL_CONDENSATE_MMBBL")]
        public async Task<WebApiResponse> RESERVES_UPDATES_OIL_CONDENSATE_MMBBL(string year = null )
        {
            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_MMBBL>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_MMBBLs.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_MMBBLs.Where(c => c.Companyemail == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate_Reserves = OilCondensate_Reserves.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("RESERVES_REPLACEMENT_RATIO")]
        public async Task<WebApiResponse> RESERVES_REPLACEMENT_RATIO(string year = null )
        {

            var OilCondensate_Reserves = new List<RESERVES_REPLACEMENT_RATIO>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_REPLACEMENT_RATIOs.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_REPLACEMENT_RATIOs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate_Reserves = OilCondensate_Reserves.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate_Reserves.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("BUDGET_CAPEX_OPEX")]
        public async Task<WebApiResponse> BUDGET_CAPEX_OPEX(string year = null )
        {
            var BudgetCapex = new List<BUDGET_CAPEX_OPEX>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                BudgetCapex = _context.BUDGET_CAPEX_OPices.ToList();
            }

            else
            {
                BudgetCapex = _context.BUDGET_CAPEX_OPices.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                BudgetCapex = BudgetCapex.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = BudgetCapex.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("OIL_AND_GAS__MAINTENANCE_PROJECTS")]
        public async Task<WebApiResponse> OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS(string year = null )
        {
            var BudgetCapex = new List<OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                BudgetCapex = _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.ToList();
            }

            else
            {
                BudgetCapex = _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                BudgetCapex = BudgetCapex.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = BudgetCapex.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("OIL_CONDENSATE_CONFORMITY")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment(string year = null )
        {
            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.ToList();
            }

            else
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                OilCondensate = OilCondensate.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = OilCondensate.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("FACILITIES_PROJECT_PERFORMANCE")]
        public async Task<WebApiResponse> FACILITIES_PROJECT_PERFORMANCE(string year = null )
        {
            var ResultData = new List<FACILITIES_PROJECT_PERFORMANCE>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.FACILITIES_PROJECT_PERFORMANCEs.ToList();
            }

            else
            {
                ResultData = _context.FACILITIES_PROJECT_PERFORMANCEs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("LEGAL_LITIGATION")]
        public async Task<WebApiResponse> LEGAL_LITIGATION(string year = null )
        {
            var ResultData = new List<LEGAL_LITIGATION>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.LEGAL_LITIGATIONs.ToList();
            }

            else
            {
                ResultData = _context.LEGAL_LITIGATIONs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
         
        
        [HttpGet("LEGAL_ARBITRATION")]
        public async Task<WebApiResponse> LEGAL_ARBITRATION(string year = null )
        {
            var ResultData = new List<LEGAL_ARBITRATION>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.LEGAL_ARBITRATIONs.ToList();
            }

            else
            {
                ResultData = _context.LEGAL_ARBITRATIONs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("NIGERIA_CONTENT_TRAINING")]
        public async Task<WebApiResponse> NIGERIA_CONTENT_TRAINING(string year = null )
        {
            var ResultData = new List<NIGERIA_CONTENT_Training>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.NIGERIA_CONTENT_Trainings.ToList();
            }

            else
            {
                ResultData = _context.NIGERIA_CONTENT_Trainings.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("NIGERIA_CONTENT_SUCCESSIONPLAN")]
        public async Task<WebApiResponse> NIGERIA_CONTENT_Upload_Succession_Plan(string year = null )
        {
            var ResultData = new List<NIGERIA_CONTENT_Upload_Succession_Plan>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.NIGERIA_CONTENT_Upload_Succession_Plans.ToList();
            }

            else
            {
                ResultData = _context.NIGERIA_CONTENT_Upload_Succession_Plans.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("STRATEGIC_PLANS_ON_COMPANY_BASIS")]
        public async Task<WebApiResponse> STRATEGIC_PLANS_ON_COMPANY_BASIS(string year = null )
        {
            var ResultData = new List<STRATEGIC_PLANS_ON_COMPANY_BASI>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.STRATEGIC_PLANS_ON_COMPANY_BAses.ToList();
            }

            else
            {
                ResultData = _context.STRATEGIC_PLANS_ON_COMPANY_BAses.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW")]
        public async Task<WebApiResponse> HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW(string year = null )
        {
            var ResultData = new List<HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.ToList();
            }

            else
            {
                ResultData = _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("HSE_MANAGEMENT_POSITION")]
        public async Task<WebApiResponse> HSE_MANAGEMENT_POSITION(string year = null )
        {
            var ResultData = new List<HSE_MANAGEMENT_POSITION>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_MANAGEMENT_POSITIONs.ToList();
            }

            else
            {
                ResultData = _context.HSE_MANAGEMENT_POSITIONs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("HSE_SAFETY_CULTURE_TRAINING")]
        public async Task<WebApiResponse> HSE_SAFETY_CULTURE_TRAINING(string year = null )
        {
            var ResultData = new List<HSE_SAFETY_CULTURE_TRAINING>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_SAFETY_CULTURE_TRAININGs.ToList();
            }

            else
            {
                ResultData = _context.HSE_SAFETY_CULTURE_TRAININGs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("HSE_OCCUPATIONAL_HEALTH_MANAGEMENT")]
        public async Task<WebApiResponse> HSE_OCCUPATIONAL_HEALTH_MANAGEMENT(string year = null )
        {
            var ResultData = new List<HSE_OCCUPATIONAL_HEALTH_MANAGEMENT>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.ToList();
            }

            else
            {
                ResultData = _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("HSE_QUALITY_CONTROL")]
        public async Task<WebApiResponse> HSE_QUALITY_CONTROL(string year = null )
        {
            var ResultData = new List<HSE_QUALITY_CONTROL>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_QUALITY_CONTROLs.ToList();
            }

            else
            {
                ResultData = _context.HSE_QUALITY_CONTROLs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
          
        
        [HttpGet("HSE_CLIMATE_CHANGE_AND_AIR_QUALITY")]
        public async Task<WebApiResponse> HSE_CLIMATE_CHANGE_AND_AIR_QUALITY(string year = null )
        {
            var ResultData = new List<HSE_CLIMATE_CHANGE_AND_AIR_QUALITY>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.ToList();
            }

            else
            {
                ResultData = _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        
        
        [HttpGet("HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW")]
        public async Task<WebApiResponse> HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW(string year = null )
        {
            var ResultData = new List<HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW>();
            try { 
            if (WKPUserRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.ToList();
            }

            else
            {
                ResultData = _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.Where(c => c.COMPANY_ID == WKPUserId).ToList();
            }

            if(year != null)
            {
                ResultData = ResultData.Where(c => c.Year_of_WP == year).ToList();
            }

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ResultData.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
        


        }

    }