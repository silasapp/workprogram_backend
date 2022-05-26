using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using AutoMapper;

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
        _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor);
        //_helpersController = new HelpersController(_context, _configuration);
    }


        [HttpGet(Name = "WORKPROGRAMME_REPORT")]
        public async Task<WebApiResponse> WORKPROGRAMME_REPORT(string year = null)
        {
            try
            {

                string previousYear = year !=null ? (int.Parse(year) - 1).ToString(): "";

                var WKP_Report = _context.ADMIN_WORK_PROGRAM_REPORTs.Where(x => x.Id == 1).FirstOrDefault();
                var WKP_Report2_Acquisition_Activities = _context.ADMIN_WORK_PROGRAM_REPORTs.Where(x => x.Id == 2).FirstOrDefault();
                var WKP_Report3_Reprocessing_Activities = _context.ADMIN_WORK_PROGRAM_REPORTs.Where(x => x.Id == 3).FirstOrDefault();
                var WKP_Report4_Crude_Oil_Production = _context.ADMIN_WORK_PROGRAM_REPORTs.Where(x => x.Id == 4).FirstOrDefault();
                var WKP_Report5_Produced_Utilized = _context.ADMIN_WORK_PROGRAM_REPORTs.Where(x => x.Id == 5).FirstOrDefault();
                if (year != null)
                {
                    var E_and_P_companies = _context.WP_COUNT_ADMIN_DATETIME_PRESENTATION_BY_TOTAL_COUNT_YEARLies.Where(x => x.YEAR == year).ToList();

                    var WP_COUNT_PRESENTEDs = _context.WP_COUNT_ADMIN_DATETIME_PRESENTATION_BY_YEAR_PRESENTED_CATEGORies.Where(x => x.Year == year && x.PRESENTED == GeneralModel.Presented).ToList();

                    var WP_COUNT_FAILED_TO_SHOW_UP = _context.WP_COUNT_ADMIN_DATETIME_PRESENTATION_BY_YEAR_PRESENTED_CATEGORies.Where(x => x.Year == year && x.PRESENTED == GeneralModel.FailedToShow).ToList();

                    var WP_COUNT_NOT_INVITED = _context.WP_COUNT_ADMIN_DATETIME_PRESENTATION_BY_YEAR_PRESENTED_CATEGORies.Where(x => x.Year == year && x.PRESENTED == GeneralModel.NotInvited).ToList();

                    var WP_COUNT_SHOWED_UP_BUT_COULD_NOT_PRESENT = _context.WP_COUNT_ADMIN_DATETIME_PRESENTATION_BY_YEAR_PRESENTED_CATEGORies.Where(x => x.Year == year && x.PRESENTED == GeneralModel.ShowedButNoPresentation).ToList();

                    var WP_COUNT_GEOPHYSICAL_ACTIVITIES_ACQUISITION = _context.WP_GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(x => x.Year_of_WP == year && x.Geo_type_of_data_acquired == GeneralModel.ThreeD).ToList();

                    var WP_COUNT_GEOPHYSICAL_ACTIVITIES_PROCESSING_3D = _context.WP_GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(x => x.Year_of_WP == year && x.Geo_Type_of_Data_being_Processed == GeneralModel.ThreeD).ToList();

                    var WP_COUNT_GEOPHYSICAL_ACTIVITIES_PROCESSING_2D = _context.WP_GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(x => x.Year_of_WP == year && x.Geo_Type_of_Data_being_Processed == GeneralModel.ThreeD).ToList();

                    var WP_COUNT_WELLS_Exploration = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.Exploration).ToList();

                    var WP_COUNT_WELLS_Development = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.Development).ToList();

                    var WP_SUM_APPRAISAL_WELLS = (from o in _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs
                                                  where o.Category.Contains("Appraisal") && o.Year_of_WP == year
                                                  group o by new
                                                  {
                                                      o.Category
                                                  }
                                                    into g
                                                  select new
                                                  {
                                                      Actual_No_Drilled_in_Current_Year = g.Sum(x => Convert.ToInt32(x.Actual_No_Drilled_in_Current_Year)),
                                                      Proposed_No_Drilled = g.Sum(x => Convert.ToInt32(x.Proposed_No_Drilled)),
                                                      Year_of_WP = g.FirstOrDefault().Year_of_WP,

                                                  }).ToList();


                    var WP_SUM_WORKOVERS_RECOMPLETION = _context.WP_SUM_INITIAL_WELL_COMPLETION_JOBS_WORKOVERS_RECOMPLETIONs.Where(x => x.Year_of_WP_I == year).ToList();

                    var WP_COUNT_FirstAppraisal = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.FirstAppraisal).ToList();
                    var WP_COUNT_SecondAppraisal = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.SecondAppraisal).ToList();
                    var WP_COUNT_Appraisal = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.Appraisal).ToList();
                    var WP_COUNT_OrdinaryAppraisal = _context.WP_DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(x => x.Year_of_WP == year && x.Category == GeneralModel.Appraisal).ToList();

                    var WP_JV_Contract_Type = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.JV).ToList();
                    string JV_Total_Reconciled_National_Crude_Oil_Production = WP_JV_Contract_Type.FirstOrDefault().Total_Reconciled_National_Crude_Oil_Production.ToString();
                    var WP_MF_Contract_Type = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.MF).ToList();
                    string MF_Total_Reconciled_National_Crude_Oil_Production = WP_MF_Contract_Type.FirstOrDefault().Total_Reconciled_National_Crude_Oil_Production.ToString();
                    var WP_PSC_Contract_Type = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.PSC).ToList();
                    string PSC_Total_Reconciled_National_Crude_Oil_Production = WP_PSC_Contract_Type.FirstOrDefault().Total_Reconciled_National_Crude_Oil_Production.ToString();
                    var WP_SR_Contract_Type = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.SR).ToList();
                    string SR_Total_Reconciled_National_Crude_Oil_Production = WP_SR_Contract_Type.FirstOrDefault().Total_Reconciled_National_Crude_Oil_Production.ToString();

                    var WP_crude_oil_FOR_CURRENT_YEAR = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Total_reconciled_crude_oils.Where(x => x.Year_of_WP == year).ToList();
                    var WP_crude_oil_FOR_PREV_YEAR = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Total_reconciled_crude_oils.Where(x => x.Year_of_WP == previousYear).ToList();
                    var WP_OIL_PRODUCTION_total_barrel = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPEs.Where(x => x.Year_of_WP == year).ToList();
                    var WP_OIL_PRODUCTION_total_barrel_JV = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPEs.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.JV).ToList();
                    var WP_OIL_PRODUCTION_total_barrel_MF = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPEs.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.MF).ToList();
                    var WP_OIL_PRODUCTION_total_barrel_SR = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPEs.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.SR).ToList();
                    var WP_OIL_PRODUCTION_total_barrel_PSC = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_Total_reconciled_crude_oils.Where(x => x.Year_of_WP == year).ToList();

                    var WP_Terrain_Continental_Shelf_or_Offshore = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrains.Where(x => x.Year_of_WP == year && x.Terrain == GeneralModel.ContinentalShelf).ToList();
                    var WP_Terrain_Continental_Deep_Offshore = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrains.Where(x => x.Year_of_WP == year && x.Terrain == GeneralModel.DeepOffshore).ToList();

                    var WP_Terrain_Continental_Terrain_Onshore = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrains.Where(x => x.Year_of_WP == year && x.Terrain == GeneralModel.Onshore).ToList();


                    #region Gas Production Activities For Previous Year 
                    var WP_GAS_ACTIVITIES_PY = _context.WP_GAS_PRODUCTION_ACTIVITIES_Percentages.Where(x => x.Year_of_WP == previousYear).FirstOrDefault();

                    string Actual_Total_Gas_Produced_PY = WP_GAS_ACTIVITIES_PY.Actual_Total_Gas_Produced.ToString();
                    string Utilized_Gas_Produced_PY = WP_GAS_ACTIVITIES_PY.Utilized_Gas_Produced.ToString();
                    string Flared_Gas_Produced_PY = WP_GAS_ACTIVITIES_PY.Flared_Gas_Produced.ToString();
                    string Percentage_Utilized_PY = WP_GAS_ACTIVITIES_PY.Percentage_Utilized.ToString();
                    Decimal PY_GasPercentage = Decimal.Parse(Percentage_Utilized_PY);
                    PY_GasPercentage = Decimal.Round(PY_GasPercentage, 2);
                    Percentage_Utilized_PY = PY_GasPercentage.ToString();

                    var WP_GAS_PRODUCTION_JV = _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == previousYear && x.Contract_Type == GeneralModel.JV).ToList();
                    string JV_Total_Gas_Produced_PY = WP_GAS_PRODUCTION_JV.FirstOrDefault().Actual_Total_Gas_Produced.ToString();

                    var WP_GAS_PRODUCTION_MF = _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == previousYear && x.Contract_Type == GeneralModel.MF).ToList();
                    string MF_Total_Gas_Produced_PY = WP_GAS_PRODUCTION_MF.FirstOrDefault().Actual_Total_Gas_Produced.ToString();

                    var WP_GAS_PRODUCTION_PSC = _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == previousYear && x.Contract_Type == GeneralModel.PSC).ToList();
                    string PSC_Total_Gas_Produced_PY = WP_GAS_PRODUCTION_PSC.FirstOrDefault().Actual_Total_Gas_Produced.ToString();

                    #endregion

                    #region Gas Production Activities For Current Year 
                    var WP_GAS_ACTIVITIES_CY = _context.WP_GAS_PRODUCTION_ACTIVITIES_Percentages.Where(x => x.Year_of_WP == year).FirstOrDefault();

                    string Actual_Total_Gas_Produced_CY = WP_GAS_ACTIVITIES_CY.Actual_Total_Gas_Produced.ToString();
                    string Utilized_Gas_Produced_CY = WP_GAS_ACTIVITIES_CY.Utilized_Gas_Produced.ToString();
                    string Flared_Gas_Produced_CY = WP_GAS_ACTIVITIES_CY.Flared_Gas_Produced.ToString();
                    string Percentage_Utilized_CY = WP_GAS_ACTIVITIES_CY.Percentage_Utilized.ToString();
                    Decimal CY_GasPercentage = Decimal.Parse(Percentage_Utilized_CY);
                    CY_GasPercentage = Decimal.Round(CY_GasPercentage, 2);
                    Percentage_Utilized_CY = CY_GasPercentage.ToString();

                    var CY_GAS_PRODUCTION_JV = _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.JV).ToList();
                    string JV_Total_Gas_Produced_CY = WP_GAS_PRODUCTION_JV.FirstOrDefault().Actual_Total_Gas_Produced.ToString();

                    var CY_GAS_PRODUCTION_MF = _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.MF).ToList();
                    string MF_Total_Gas_Produced_CY = WP_GAS_PRODUCTION_MF.FirstOrDefault().Actual_Total_Gas_Produced.ToString();

                    var CY_GAS_PRODUCTION_PSC = _context.WP_GAS_PRODUCTION_ACTIVITIES_Contract_Types.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.PSC).ToList();
                    string PSC_Total_Gas_Produced_CY = WP_GAS_PRODUCTION_PSC.FirstOrDefault().Actual_Total_Gas_Produced.ToString();

                    #endregion

                    var OIL_CONDENSATE_MMBBL = _context.WP_RESERVES_UPDATES_OIL_CONDENSATE_MMBBLs.Where(x => x.Year_of_WP == year).FirstOrDefault();
                    string Reserves_as_at_MMbbl = OIL_CONDENSATE_MMBBL.Reserves_as_at_MMbbl.ToString();
                    string Reserves_as_at_MMbbl_gas = OIL_CONDENSATE_MMBBL.Reserves_as_at_MMbbl_gas.ToString();
                    string Reserves_as_at_MMbbl_condensate = OIL_CONDENSATE_MMBBL.Reserves_as_at_MMbbl_condensate.ToString();

                    #region HSE Accident
                    var HSE_ACCIDENT_Consequences = _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_by_consequences.Where(x => x.Year_of_WP == year && x.Consequence == GeneralModel.Fatality).FirstOrDefault();
                    string Sum_accident = HSE_ACCIDENT_Consequences.sum_accident.ToString();
                    var HSE_ACCIDENT_Total = _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_accidents.Where(x => x.Year_of_WP == year).FirstOrDefault();
                    string Sum_accident_total = HSE_ACCIDENT_Total.Sum_accident.ToString();


                    var HSE_ACCIDENT_Sabotage = _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_spill_accident_and_percentages.Where(x => x.Year_of_WP == year && x.Cause == GeneralModel.Sabotage).FirstOrDefault();
                    string Sabotage_Sum_accident = HSE_ACCIDENT_Sabotage.sum_accident.ToString();
                    string Sabotage_Percentage_Spill = HSE_ACCIDENT_Sabotage.Percentage_Spill.ToString();
                    Decimal S_DECIMAL = Decimal.Parse(Sabotage_Percentage_Spill);
                    S_DECIMAL = Decimal.Round(S_DECIMAL, 2);
                    Sabotage_Percentage_Spill = S_DECIMAL.ToString();

                    var HSE_ACCIDENT_Error = _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_spill_accident_and_percentages.Where(x => x.Year_of_WP == year && x.Cause == GeneralModel.HumanError).FirstOrDefault();
                    string Error_Sum_accident = HSE_ACCIDENT_Error.sum_accident.ToString();
                    string Error_Percentage_Spill = HSE_ACCIDENT_Error.Percentage_Spill.ToString();
                    Decimal E_DECIMAL = Decimal.Parse(Error_Percentage_Spill);
                    E_DECIMAL = Decimal.Round(E_DECIMAL, 2);
                    Error_Percentage_Spill = E_DECIMAL.ToString();


                    var HSE_ACCIDENT_MS = _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_spill_accident_and_percentages.Where(x => x.Year_of_WP == year && x.Cause == GeneralModel.MysterySpills).FirstOrDefault();
                    string MS_Sum_accident = HSE_ACCIDENT_MS.sum_accident.ToString();
                    string MS_Percentage_Spill = HSE_ACCIDENT_MS.Percentage_Spill.ToString();
                    Decimal MS_DECIMAL = Decimal.Parse(MS_Percentage_Spill);
                    MS_DECIMAL = Decimal.Round(MS_DECIMAL, 2);
                    MS_Percentage_Spill = MS_DECIMAL.ToString();

                    var HSE_ACCIDENT_EF = _context.WP_HSE_ACCIDENT_INCIDENCE_REPORTING_TYPE_OF_ACCIDENT_NEW_total_spill_accident_and_percentages.Where(x => x.Year_of_WP == year && x.Cause == GeneralModel.EquipmentFailure).FirstOrDefault();
                    string EF_Sum_accident = HSE_ACCIDENT_EF.sum_accident.ToString();
                    string EF_Percentage_Spill = HSE_ACCIDENT_EF.Percentage_Spill.ToString();
                    Decimal EF_DECIMAL = Decimal.Parse(EF_Percentage_Spill);
                    EF_DECIMAL = Decimal.Round(EF_DECIMAL, 2);
                    EF_Percentage_Spill = EF_DECIMAL.ToString();
                    #endregion

                    #region GEO Activities
                    var JV_GEO_ACTIVITIES = _context.WP_GEOPHYSICAL_ACTIVITIES_ACQUISITION_sum_and_counts.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.JV).FirstOrDefault();
                    string JV_Count_Contract_Type = JV_GEO_ACTIVITIES.Count_Contract_Type.ToString();
                    string JV_Actual_year_aquired_data = JV_GEO_ACTIVITIES.Actual_year_aquired_data.ToString();
                    string JV_proposed_year_data = JV_GEO_ACTIVITIES.proposed_year_data.ToString();

                    var MF_GEO_ACTIVITIES = _context.WP_GEOPHYSICAL_ACTIVITIES_ACQUISITION_sum_and_counts.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.MF).FirstOrDefault();
                    string MF_Count_Contract_Type = MF_GEO_ACTIVITIES.Count_Contract_Type.ToString();
                    string MF_Actual_year_aquired_data = MF_GEO_ACTIVITIES.Actual_year_aquired_data.ToString();
                    string MF_proposed_year_data = MF_GEO_ACTIVITIES.proposed_year_data.ToString();

                    var PSC_GEO_ACTIVITIES = _context.WP_GEOPHYSICAL_ACTIVITIES_ACQUISITION_sum_and_counts.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.PSC).FirstOrDefault();
                    string PSC_Count_Contract_Type = PSC_GEO_ACTIVITIES.Count_Contract_Type.ToString();
                    string PSC_Actual_year_aquired_data = PSC_GEO_ACTIVITIES.Actual_year_aquired_data.ToString();
                    string PSC_proposed_year_data = PSC_GEO_ACTIVITIES.proposed_year_data.ToString();

                    var HSE_REPORTING_NEW_volume_of_spill = (from o in _context.HSE_OIL_SPILL_REPORTING_NEWs
                                                             where o.Year_of_WP == year
                                                             select o
                                                             ).ToList().Sum(x => int.Parse(x.Volume_of_spill_bbls));

                    var GEO_sum_and_count_SR = (from o in _context.WP_GEOPHYSICAL_ACTIVITIES_ACQUISITION_sum_and_counts
                                                where o.Year_of_WP == year && o.Contract_Type == GeneralModel.SR
                                                select o).FirstOrDefault();

                    string Count_Contract_Type = GEO_sum_and_count_SR.Count_Contract_Type.ToString();
                    string Actual_year_aquired_data = GEO_sum_and_count_SR.Actual_year_aquired_data.ToString();
                    string proposed_year_data = GEO_sum_and_count_SR.proposed_year_data.ToString();

                    #endregion
                    var Oil_Spill_REPORT = (from o in _context.WP_TOTAL_INCIDENCE_AND_OIL_SPILL_AND_RECOVEREDs
                                            where o.Year_of_WP == year
                                            group o by new
                                            {
                                                o.CompanyName
                                            }
                                                   into g
                                            select new
                                            {
                                                Frequency = g.Min(x => x.Frequency),
                                                Highest_1st = g.Min(x => x.Frequency),
                                                Highest_2nd = g.Min(x => x.Frequency),
                                                Highest_3rd = g.Min(x => x.Frequency),
                                                TOTAL_QUANTITY_SPILLED = g.Sum(x => Convert.ToInt32(x.Total_Quantity_Spilled)),
                                                CompanyName = g.Key,
                                                Year_of_WP = g.FirstOrDefault().Year_of_WP,

                                            }).OrderBy(x => x.Frequency);

                    var Oil_Spill_REPORT_1st = Oil_Spill_REPORT.Take(1);
                    var Oil_Spill_REPORT_2nd = Oil_Spill_REPORT.Take(2);
                    var Oil_Spill_REPORT_3rd = Oil_Spill_REPORT.Take(3);
                    var Oil_Spill_REPORT_4th = Oil_Spill_REPORT.Take(4);
                    var Oil_Spill_REPORT_5th = Oil_Spill_REPORT.Take(5);


                    var QUANTITY_RECOVERED = (from o in _context.HSE_CAUSES_OF_SPILLs
                                              where o.Year_of_WP == year
                                              select o).ToList();
                    var Total_no_of_QUANTITY_RECOVERED = QUANTITY_RECOVERED.Sum(x => int.Parse(x.Total_Quantity_Recovered));
                    var Total_no_of_QUANTITY_SPILLED = QUANTITY_RECOVERED.Sum(x => int.Parse(x.Total_Quantity_Spilled));
                    var Total_PRODUCED_WATER = _context.HSE_PRODUCED_WATER_MANAGEMENT_NEW_UPDATEDs.Where(x => x.Year_of_WP == year).Sum(x => int.Parse(x.Produced_water_volumes));


                    WP_SUM_APPRAISAL_WELLS = WP_SUM_APPRAISAL_WELLS;

                    var PRODUCTION_total_barrel_PROPOSED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE_PROPOSEDs.Where(x => x.Year_of_WP == year);
                    var JV_PRODUCTION_total_barrel_PROPOSED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE_PROPOSEDs.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.JV);
                    var MF_PRODUCTION_total_barrel_PROPOSED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE_PROPOSEDs.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.MF);
                    var PSC_PRODUCTION_total_barrel_PROPOSED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE_PROPOSEDs.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.PSC);
                    var SR_PRODUCTION_total_barrel_PROPOSED = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_CONTRACT_TYPE_PROPOSEDs.Where(x => x.Year_of_WP == year && x.Contract_Type == GeneralModel.SR);

                    var Planned_Terrain_Continental_Shelf_or_Offshore = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrain_PLANNEDs.Where(x => x.Year_of_WP == year && x.Terrain == GeneralModel.ContinentalShelf).ToList();
                    var Planned_Terrain_Continental_Onshore = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrain_PLANNEDs.Where(x => x.Year_of_WP == year && x.Terrain == GeneralModel.Onshore).ToList();
                    var Planned_Terrain_Continental_DeepOffshore = _context.WP_OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_OIL_PRODUCTION_by_Terrain_PLANNEDs.Where(x => x.Year_of_WP == year && x.Terrain == GeneralModel.DeepOffshore).ToList();
                    var FlaringForecast = _context.WP_Gas_Production_Utilisation_And_Flaring_Forecasts.Where(x => x.Year_of_WP == year).ToList();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data =WP_COUNT_Appraisal, StatusCode = ResponseCodes.Success };
                }
                else
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = WKP_Report, StatusCode = ResponseCodes.Success };

                }
            }


            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Failure };
            }
            
        }

        [HttpGet(Name = "CONCESSIONSINFORMATION")]
        public async Task<WebApiResponse> Get_ADMIN_CONCESSIONS_INFORMATION_BY_CURRENT_YEAR(string year = null)
        {

            //var userRole = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionRoleName));
            //var userEmail = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionEmail));
            //var companyID = _helpersController.Decrypt(_httpContextAccessor.HttpContext.Session.GetString(Authentications.AuthController.sessionUserID));

            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            try { 
            var dateYear = DateTime.Now.AddYears(0).ToString("yyyy");
            var ConcessionsInformation = new List<ADMIN_CONCESSIONS_INFORMATION>();

            if (userRole == GeneralModel.Admin)
            {
                ConcessionsInformation = _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(c => c.Year == dateYear && c.DELETED_STATUS == null).ToList();
            }
            else
            {
                ConcessionsInformation = _context.ADMIN_CONCESSIONS_INFORMATIONs.Where(c => c.Year == dateYear && c.Company_ID == companyID && c.DELETED_STATUS == null).ToList();
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


        [HttpGet(Name = "CONCESSIONSITUATION")]
        public async Task<WebApiResponse> Get_CONCESSION_SITUATION(string year = null )
        {

            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ConcessionSituation = new List<CONCESSION_SITUATION>();
            try { 

                if (userRole == GeneralModel.Admin)
                {
                    ConcessionSituation = _context.CONCESSION_SITUATIONs.Where(c => c.Year == year).ToList();
                }
                else
                {
                    ConcessionSituation = _context.CONCESSION_SITUATIONs.Where(c => c.Year == year && c.COMPANY_ID == companyID).ToList();
                }


                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = ConcessionSituation.OrderBy(x => x.Year), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }

        
        [HttpGet(Name = "GEOPHYSICALACTIVITIES")]
        public async Task<WebApiResponse> Get_GEOPHYSICAL_ACTIVITIES_ACQUISITION(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var GEOPHYSICALACTIVITIES = new List<GEOPHYSICAL_ACTIVITIES_ACQUISITION>();
            try
            {
                if (userRole == GeneralModel.Admin)
                {
                    GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(c => c.Year_of_WP == year).ToList();
                }
                else
                {
                    GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == companyID).ToList();
                }


                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GEOPHYSICALACTIVITIES.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }
      
      
        [HttpGet(Name = "GEOPHYSICALPROCESSING")]
        public async Task<WebApiResponse> Get_GEOPHYSICAL_ACTIVITIES_PROCESSING(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var GEOPHYSICALACTIVITIES = new List<GEOPHYSICAL_ACTIVITIES_PROCESSING>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                GEOPHYSICALACTIVITIES = _context.GEOPHYSICAL_ACTIVITIES_PROCESSINGs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == companyID).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = GEOPHYSICALACTIVITIES.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }


        [HttpGet(Name = "DRILLING-OPERATIONS")]
        public async Task<WebApiResponse> Get_DRILLING_OPERATIONS_CATEGORIES_OF_WELLS(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var DRILLING_OPERATIONS = new List<DRILLING_OPERATIONS_CATEGORIES_OF_WELL>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                DRILLING_OPERATIONS = _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                DRILLING_OPERATIONS = _context.DRILLING_OPERATIONS_CATEGORIES_OF_WELLs.Where(c => c.Year_of_WP == year && c.COMPANY_ID == companyID).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = DRILLING_OPERATIONS.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }

        
        [HttpGet(Name = "WORKOVERS_RECOMPLETION")]
        public async Task<WebApiResponse> Get_WORKOVERS_RECOMPLETION_JOBs(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var WORKOVERS_RECOMPLETION = new List<WORKOVERS_RECOMPLETION_JOB1>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                WORKOVERS_RECOMPLETION = _context.WORKOVERS_RECOMPLETION_JOBs1.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                WORKOVERS_RECOMPLETION = _context.WORKOVERS_RECOMPLETION_JOBs1.Where(c => c.Year_of_WP == year && c.COMPANY_ID == companyID).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = WORKOVERS_RECOMPLETION.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }

         
        [HttpGet(Name = "INITIAL_WELLCOMPLETION")]
        public async Task<WebApiResponse> Get_INITIAL_WELL_COMPLETION(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var INITIAL_WELLCOMPLETION = new List<INITIAL_WELL_COMPLETION_JOB1>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                INITIAL_WELLCOMPLETION = _context.INITIAL_WELL_COMPLETION_JOBs1.Where(c => c.Year_of_WP == year).ToList();
            }
            else
            {
                INITIAL_WELLCOMPLETION = _context.INITIAL_WELL_COMPLETION_JOBs1.Where(c => c.Year_of_WP == year && c.COMPANY_ID == companyID).ToList();
            }


            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = INITIAL_WELLCOMPLETION.OrderBy(x => x.Year_of_WP), StatusCode = ResponseCodes.Success };
            }

            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Failure : " + e.Message, StatusCode = ResponseCodes.Success };
            }
        }


        [HttpGet(Name = "FDP_EXPECTED_RESERVES")]
        public async Task<WebApiResponse> Get_FIELD_DEVELOPMENT_PLAN_EXPECTED_RESERVES(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var FDP_Reserves = new List<FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERf>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                FDP_Reserves = _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.ToList();
            }
            else
            {
                FDP_Reserves = _context.FIELD_DEVELOPMENT_PLAN_EXCESSIVE_RESERVEs.Where(c => c.COMPANY_ID == companyID).ToList();
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


        [HttpGet(Name = "FDP_TOSUBMIT")]
        public async Task<WebApiResponse> Get_FIELD_DEVELOPMENT_PLAN_TOBESUBMITTED(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var FDP_Reserves = new List<FIELD_DEVELOPMENT_PLAN>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                FDP_Reserves = _context.FIELD_DEVELOPMENT_PLANs.ToList();
            }
            else
            {
                FDP_Reserves = _context.FIELD_DEVELOPMENT_PLANs.Where(c => c.COMPANY_ID == companyID).ToList();
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


        [HttpGet(Name = "FDP_FIELDSTATUS")]
        public async Task<WebApiResponse> FIELD_DEVELOPMENT_FIELDS_AND_STATUS(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var FDP_Fields = new List<FIELD_DEVELOPMENT_FIELDS_AND_STATUS>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                FDP_Fields = _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.ToList();
            }
            else
            {
                FDP_Fields = _context.FIELD_DEVELOPMENT_FIELDS_AND_STATUSes.Where(c => c.COMPANY_ID == companyID).ToList();
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


        [HttpGet(Name = "NDR")]
        public async Task<WebApiResponse> NDR(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var NDR = new List<NDR>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                NDR = _context.NDRs.ToList();
            }
            else
            {
                NDR = _context.NDRs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "OIL_CONDENSATE_PRODUCTION_ACTIVITIES")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITy>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.ToList();
            }
            else
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIEs.Where(c => c.COMPANY_ID == companyID).ToList();
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

        
        [HttpGet(Name = "OIL_CONDENSATE_MONTHLY_ACTIVITIES")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activity>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.ToList();
            }
            else
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "OIL_CONDENSATE_MONTHLY_ACTIVITIES_PROPOSED")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSED>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.ToList();
            }
            else
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_monthly_Activities_PROPOSEDs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "OIL_CONDENSATE_MONTHLY_ACTIVITIES_PROPOSED_FIVEYEARS")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTION>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.ToList();
            }
            else
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_FIVE_YEAR_PROJECTIONs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "GAS_PRODUCTION_ACTIVITIES")]
        public async Task<WebApiResponse> GAS_PRODUCTION_ACTIVITIES(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var GasProduction = new List<GAS_PRODUCTION_ACTIVITy>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                GasProduction = _context.GAS_PRODUCTION_ACTIVITIEs.ToList();
            }
            else
            {
                GasProduction = _context.GAS_PRODUCTION_ACTIVITIEs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY")]
        public async Task<WebApiResponse> GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var GasProduction = new List<GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLY>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                GasProduction = _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.ToList();
            }
            else
            {
                GasProduction = _context.GAS_PRODUCTION_ACTIVITIES_DOMESTIC_SUPPLies.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "UNITIZATION")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var GasProduction = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATION>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                GasProduction = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.ToList();
            }
            else
            {
                GasProduction = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_UNITIZATIONs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "CONCESSION_RESERVES_FOR_1ST_JANUARY")]
        public async Task<WebApiResponse> CONCESSION_RESERVES_FOR_1ST_JANUARY(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var Concession = new List<RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVE>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                Concession = _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Where(c => c.FLAG1 == "COMPANY_RESERVE_OF_PRECEDDING_YEAR").ToList();
            }

            else
            {
                Concession = _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.Where(c => c.FLAG1 == "COMPANY_RESERVE_OF_PRECEDDING_YEAR" && c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "RESERVES_OIL_CONDENSATE_PRODUCTION")]
        public async Task<WebApiResponse> RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTION>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "RESERVES_ADDITION")]
        public async Task<WebApiResponse> RESERVES_ADDITION(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_Reserves_Addition>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_Additions.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_Additions.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "RESERVES_DECLINE")]
        public async Task<WebApiResponse> RESERVES_DECLINE(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINE>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_Reserves_DECLINEs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "RESERVES_LIFE_INDEX")]
        public async Task<WebApiResponse> RESERVES_UPDATES_LIFE_INDEX(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate_Reserves = new List<RESERVES_UPDATES_LIFE_INDEX>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_LIFE_INDices.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_LIFE_INDices.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "RESERVES_UPDATES_DEPLETION_RATE")]
        public async Task<WebApiResponse> RESERVES_UPDATES_DEPLETION_RATE(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate_Reserves = new List<RESERVES_UPDATES_DEPLETION_RATE>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_DEPLETION_RATEs.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_DEPLETION_RATEs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "RESERVES_OIL_CONDENSATE_MMBBL")]
        public async Task<WebApiResponse> RESERVES_UPDATES_OIL_CONDENSATE_MMBBL(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate_Reserves = new List<RESERVES_UPDATES_OIL_CONDENSATE_MMBBL>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_MMBBLs.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_UPDATES_OIL_CONDENSATE_MMBBLs.Where(c => c.Companyemail == companyID).ToList();
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
        
        
        [HttpGet(Name = "RESERVES_REPLACEMENT_RATIO")]
        public async Task<WebApiResponse> RESERVES_REPLACEMENT_RATIO(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate_Reserves = new List<RESERVES_REPLACEMENT_RATIO>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate_Reserves = _context.RESERVES_REPLACEMENT_RATIOs.ToList();
            }

            else
            {
                OilCondensate_Reserves = _context.RESERVES_REPLACEMENT_RATIOs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "BUDGET_CAPEX_OPEX")]
        public async Task<WebApiResponse> BUDGET_CAPEX_OPEX(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var BudgetCapex = new List<BUDGET_CAPEX_OPEX>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                BudgetCapex = _context.BUDGET_CAPEX_OPices.ToList();
            }

            else
            {
                BudgetCapex = _context.BUDGET_CAPEX_OPices.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "OIL_AND_GAS__MAINTENANCE_PROJECTS")]
        public async Task<WebApiResponse> OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTS(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var BudgetCapex = new List<OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECT>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                BudgetCapex = _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.ToList();
            }

            else
            {
                BudgetCapex = _context.OIL_AND_GAS_FACILITY_MAINTENANCE_PROJECTs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "OIL_CONDENSATE_CONFORMITY")]
        public async Task<WebApiResponse> OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var OilCondensate = new List<OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessment>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.ToList();
            }

            else
            {
                OilCondensate = _context.OIL_CONDENSATE_PRODUCTION_ACTIVITIES_New_Technology_Conformity_Assessments.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "FACILITIES_PROJECT_PERFORMANCE")]
        public async Task<WebApiResponse> FACILITIES_PROJECT_PERFORMANCE(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<FACILITIES_PROJECT_PERFORMANCE>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.FACILITIES_PROJECT_PERFORMANCEs.ToList();
            }

            else
            {
                ResultData = _context.FACILITIES_PROJECT_PERFORMANCEs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "LEGAL_LITIGATION")]
        public async Task<WebApiResponse> LEGAL_LITIGATION(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<LEGAL_LITIGATION>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.LEGAL_LITIGATIONs.ToList();
            }

            else
            {
                ResultData = _context.LEGAL_LITIGATIONs.Where(c => c.COMPANY_ID == companyID).ToList();
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
         
        
        [HttpGet(Name = "LEGAL_ARBITRATION")]
        public async Task<WebApiResponse> LEGAL_ARBITRATION(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<LEGAL_ARBITRATION>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.LEGAL_ARBITRATIONs.ToList();
            }

            else
            {
                ResultData = _context.LEGAL_ARBITRATIONs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "NIGERIA_CONTENT_TRAINING")]
        public async Task<WebApiResponse> NIGERIA_CONTENT_TRAINING(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<NIGERIA_CONTENT_Training>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.NIGERIA_CONTENT_Trainings.ToList();
            }

            else
            {
                ResultData = _context.NIGERIA_CONTENT_Trainings.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "NIGERIA_CONTENT_SUCCESSIONPLAN")]
        public async Task<WebApiResponse> NIGERIA_CONTENT_Upload_Succession_Plan(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<NIGERIA_CONTENT_Upload_Succession_Plan>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.NIGERIA_CONTENT_Upload_Succession_Plans.ToList();
            }

            else
            {
                ResultData = _context.NIGERIA_CONTENT_Upload_Succession_Plans.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "STRATEGIC_PLANS_ON_COMPANY_BASIS")]
        public async Task<WebApiResponse> STRATEGIC_PLANS_ON_COMPANY_BASIS(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<STRATEGIC_PLANS_ON_COMPANY_BASI>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.STRATEGIC_PLANS_ON_COMPANY_BAses.ToList();
            }

            else
            {
                ResultData = _context.STRATEGIC_PLANS_ON_COMPANY_BAses.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW")]
        public async Task<WebApiResponse> HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEW>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.ToList();
            }

            else
            {
                ResultData = _context.HSE_TECHNICAL_SAFETY_CONTROL_STUDIES_NEWs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "HSE_MANAGEMENT_POSITION")]
        public async Task<WebApiResponse> HSE_MANAGEMENT_POSITION(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<HSE_MANAGEMENT_POSITION>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_MANAGEMENT_POSITIONs.ToList();
            }

            else
            {
                ResultData = _context.HSE_MANAGEMENT_POSITIONs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "HSE_SAFETY_CULTURE_TRAINING")]
        public async Task<WebApiResponse> HSE_SAFETY_CULTURE_TRAINING(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<HSE_SAFETY_CULTURE_TRAINING>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_SAFETY_CULTURE_TRAININGs.ToList();
            }

            else
            {
                ResultData = _context.HSE_SAFETY_CULTURE_TRAININGs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "HSE_OCCUPATIONAL_HEALTH_MANAGEMENT")]
        public async Task<WebApiResponse> HSE_OCCUPATIONAL_HEALTH_MANAGEMENT(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<HSE_OCCUPATIONAL_HEALTH_MANAGEMENT>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.ToList();
            }

            else
            {
                ResultData = _context.HSE_OCCUPATIONAL_HEALTH_MANAGEMENTs.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "HSE_QUALITY_CONTROL")]
        public async Task<WebApiResponse> HSE_QUALITY_CONTROL(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<HSE_QUALITY_CONTROL>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_QUALITY_CONTROLs.ToList();
            }

            else
            {
                ResultData = _context.HSE_QUALITY_CONTROLs.Where(c => c.COMPANY_ID == companyID).ToList();
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
          
        
        [HttpGet(Name = "HSE_CLIMATE_CHANGE_AND_AIR_QUALITY")]
        public async Task<WebApiResponse> HSE_CLIMATE_CHANGE_AND_AIR_QUALITY(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<HSE_CLIMATE_CHANGE_AND_AIR_QUALITY>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.ToList();
            }

            else
            {
                ResultData = _context.HSE_CLIMATE_CHANGE_AND_AIR_QUALITies.Where(c => c.COMPANY_ID == companyID).ToList();
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
        
        
        [HttpGet(Name = "HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW")]
        public async Task<WebApiResponse> HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW(string year = null )
        {
            var userRole = "Admin";
            var userEmail = "test@mailinator.com";
            var companyID = "NND/001";

            var ResultData = new List<HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEW>();
            try { 
            if (userRole == GeneralModel.Admin)
            {
                ResultData = _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.ToList();
            }

            else
            {
                ResultData = _context.HSE_INSPECTION_AND_MAINTENANCE_FACILITY_TYPE_NEWs.Where(c => c.COMPANY_ID == companyID).ToList();
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