using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Backend_UMR_Work_Program.Models.GeneralModel;

namespace Backend_UMR_Work_Program.Controllers
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        public WKP_DBContext _context;

        public DashboardController(WKP_DBContext context)
        {
            _context = context;
        }

        private string? WKPCompanyId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private string? WKPCompanyName => User.FindFirstValue(ClaimTypes.Name);
        private string? WKPCompanyEmail => User.FindFirstValue(ClaimTypes.Email);
        private string? WKUserRole => User.FindFirstValue(ClaimTypes.Role);
        private int? WKPCompanyNumber => Convert.ToInt32(User.FindFirstValue(ClaimTypes.PrimarySid));

        [HttpGet("Company_Dashboard_Report")]
        public async Task<object> Company_Dashboard(string year)
        {
            try
            {
                if (WKUserRole == GeneralModel.Admin)
                {
                    var data = new
                    {
                        year = year,
                        conc = await _context.CONCESSION_SITUATIONs.ToListAsync(),
                        reserve = await _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs.ToListAsync(),
                        reserveStatus = await _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs.ToListAsync()
                    };

                    var companyDashboard_DataList = new List<CompanyReportModel>();
                    var companyDashboard_Data = new CompanyReportModel();
                    var companyDashboard_Report = new CompanyDashboardModel();
                    var companyConcessions = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.DELETED_STATUS != "DELETED" select d).ToListAsync();


                    data.conc.GroupBy(x => x.OML_ID).ToList().ForEach(key =>
                    {
                        companyDashboard_Data = new CompanyReportModel()
                        {
                            concessionName = key.FirstOrDefault().OML_Name,
                            oil_NetProduction = data.reserve.Where(x => x.Year_of_WP == year).Sum(x => double.Parse(x.Company_Annual_Oil)),
                            AG_NetProduction = data.reserve.Where(x => x.Year_of_WP == year).Sum(x => double.Parse(x.Company_Annual_AG)),
                            NAG_NetProduction = data.reserve.Where(x => x.Year_of_WP == year).Sum(x => double.Parse(x.Company_Annual_NAG)),
                            condensate_NetProduction = data.reserve.Where(x => x.Year_of_WP == year).Sum(x => double.Parse(x.Company_Annual_Condensate)),
                            oil_Reserves = data.reserveStatus.Where(x => x.Year_of_WP == year && x.Company_Reserves_AnnualOilProduction != null).Sum(x => double.Parse(x?.Company_Reserves_AnnualOilProduction)),
                            AG_Reserves = data.reserveStatus.Where(x => x.Year_of_WP == year && x.Company_Reserves_AnnualGasAGProduction != null).Sum(x => double.Parse(x?.Company_Reserves_AnnualOilProduction)),
                            NAG_Reserves = data.reserveStatus.Where(x => x.Year_of_WP == year && x.Company_Reserves_AnnualGasNAGProduction != null).Sum(x => double.Parse(x?.Company_Reserves_AnnualOilProduction)),
                            condensate_Reserves = data.reserveStatus.Where(x => x.Year_of_WP == year && x.Company_Reserves_AnnualCondensateProduction != null).Sum(x => double.Parse(x?.Company_Reserves_AnnualCondensateProduction))

                        };
                        companyDashboard_Data.totalNetProduction = companyDashboard_Data.oil_NetProduction + companyDashboard_Data.AG_NetProduction + companyDashboard_Data.NAG_NetProduction + companyDashboard_Data.condensate_NetProduction;
                        companyDashboard_Data.totalReserves = companyDashboard_Data.oil_Reserves + companyDashboard_Data.AG_Reserves + companyDashboard_Data.NAG_Reserves + companyDashboard_Data.condensate_Reserves;
                        companyDashboard_DataList.Add(companyDashboard_Data);
                    });
                    companyDashboard_Report.CompanyReportModels = companyDashboard_DataList;


                    companyDashboard_Report.OML_Count = companyConcessions.Where(x => x.Consession_Type.ToLower() == GeneralModel.OML.ToLower()).Count();
                    companyDashboard_Report.OPL_Count = companyConcessions.Where(x => x.Consession_Type.ToLower() == GeneralModel.OPL.ToLower()).Count();
                    companyDashboard_Report.No_Of_ProducingFields_Count = await (from d in _context.COMPANY_FIELDs where d.DeletedStatus != true select d).CountAsync();

                    return companyDashboard_Report;

                }
                else
                {
                    var data = await (from conc in _context.CONCESSION_SITUATIONs
                                      join comp in _context.ADMIN_COMPANY_INFORMATIONs on conc.COMPANY_ID equals comp.COMPANY_ID
                                      join reserve in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs on conc.OML_ID equals reserve.OML_ID
                                      join reserveStatus in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs on conc.OML_ID equals reserveStatus.OML_ID
                                      where reserve.Year_of_WP == year && (reserve.Created_by == WKPCompanyEmail || reserve.CompanyNumber == WKPCompanyNumber)
                                      && (reserveStatus.Created_by == WKPCompanyEmail || reserveStatus.CompanyNumber == WKPCompanyNumber)

                                      select new
                                      {
                                          year = reserve.Year_of_WP,
                                          conc = conc,
                                          reserve = reserve,
                                          reserveStatus = reserveStatus,
                                          comp
                                      }).ToListAsync();

                    var companyDashboard_DataList = new List<CompanyReportModel>();
                    var companyDashboard_Data = new CompanyReportModel();
                    var companyDashboard_Report = new CompanyDashboardModel();
                    var companyConcessions = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.Company_ID == WKPCompanyId && d.DELETED_STATUS != "DELETED" select d).ToListAsync();



                    data.GroupBy(x => x.conc.OML_ID).ToList().ForEach(key =>
                    {
                        companyDashboard_Data = new CompanyReportModel()
                        {
                            concessionName = key.FirstOrDefault().conc.OML_Name,
                            oil_NetProduction = key.Where(x => x.year == year).Sum(x => double.Parse(x.reserve.Company_Annual_Oil)),
                            AG_NetProduction = key.Where(x => x.year == year).Sum(x => double.Parse(x.reserve.Company_Annual_AG)),
                            NAG_NetProduction = key.Where(x => x.year == year).Sum(x => double.Parse(x.reserve.Company_Annual_NAG)),
                            condensate_NetProduction = key.Where(x => x.year == year).Sum(x => double.Parse(x.reserve.Company_Annual_Condensate)),
                            oil_Reserves = key.Where(x => x.year == year && x.reserveStatus.Company_Reserves_AnnualOilProduction != null).Sum(x => double.Parse(x.reserveStatus?.Company_Reserves_AnnualOilProduction)),
                            AG_Reserves = key.Where(x => x.year == year && x.reserveStatus.Company_Reserves_AnnualGasAGProduction != null).Sum(x => double.Parse(x.reserveStatus?.Company_Reserves_AnnualOilProduction)),
                            NAG_Reserves = key.Where(x => x.year == year && x.reserveStatus.Company_Reserves_AnnualGasNAGProduction != null).Sum(x => double.Parse(x.reserveStatus?.Company_Reserves_AnnualOilProduction)),
                            condensate_Reserves = key.Where(x => x.year == year && x.reserveStatus.Company_Reserves_AnnualCondensateProduction != null).Sum(x => double.Parse(x.reserveStatus?.Company_Reserves_AnnualCondensateProduction)),

                        };
                        companyDashboard_Data.totalNetProduction = companyDashboard_Data.oil_NetProduction + companyDashboard_Data.AG_NetProduction + companyDashboard_Data.NAG_NetProduction + companyDashboard_Data.condensate_NetProduction;
                        companyDashboard_Data.totalReserves = companyDashboard_Data.oil_Reserves + companyDashboard_Data.AG_Reserves + companyDashboard_Data.NAG_Reserves + companyDashboard_Data.condensate_Reserves;
                        companyDashboard_DataList.Add(companyDashboard_Data);
                    });
                    companyDashboard_Report.CompanyReportModels = companyDashboard_DataList;


                    companyDashboard_Report.OML_Count = companyConcessions.Where(x => x.Consession_Type.ToLower() == GeneralModel.OML.ToLower()).Count();
                    companyDashboard_Report.OPL_Count = companyConcessions.Where(x => x.Consession_Type.ToLower() == GeneralModel.OPL.ToLower()).Count();
                    companyDashboard_Report.No_Of_ProducingFields_Count = await (from d in _context.COMPANY_FIELDs where d.CompanyNumber == WKPCompanyNumber && d.DeletedStatus != true select d).CountAsync();

                    return companyDashboard_Report;
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

        [HttpGet("DASHBOARD_TOTAL_GAS_BUDGET_RESERVES_DETAILS")]
        public async Task<object> DASHBOARD_TOTAL_GAS_BUDGET_RESERVES_DETAILS(string year)
        {

            try
            {

                if (WKUserRole == GeneralModel.Admin)
                {

                    var GasProduction = await (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.Year_of_WP == year select c).ToListAsync();
                    var TBPPCT = await (from c in _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs where c.Year_of_WP == year select c).ToListAsync();
                    var TROC = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs where c.Year_of_WP == year select c).ToListAsync();
                    var gasPlantCapacity = GasProduction.Sum(a => double.Parse(a.Gas_plant_capacity));
                    var gasFlareds = GasProduction.Sum(a => double.Parse(a.Flared));
                    var directProdCost = TBPPCT.Sum(a => double.Parse(a.DIRECT_COST_Actual));
                    var oilCondensate = TROC.Sum(a => double.Parse(a.Company_Annual_Oil)) + TROC.Sum(a => double.Parse(a.Company_Annual_Condensate));

                    return new { gasPlantCapacity = gasPlantCapacity, gasFlareds = gasFlareds, directProdCost = directProdCost, oilCondensate = oilCondensate };

                }
                else
                {

                    var GasProduction = await (from c in _context.GAS_PRODUCTION_ACTIVITIEs where c.Companyemail == WKPCompanyEmail && c.Year_of_WP == year select c).ToListAsync();
                    var TBPPCT = await (from c in _context.BUDGET_PERFORMANCE_PRODUCTION_COSTs where c.Companyemail == WKPCompanyEmail && c.Year_of_WP == year select c).ToListAsync();
                    var TROC = await (from c in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs where c.Companyemail == WKPCompanyEmail && c.Year_of_WP == year select c).ToListAsync();
                    var gasPlantCapacity = GasProduction.Sum(a => double.Parse(a.Gas_plant_capacity));
                    var gasFlareds = GasProduction.Sum(a => double.Parse(a.Flared));
                    var directProdCost = TBPPCT.Sum(a => double.Parse(a.DIRECT_COST_Actual));
                    var oilCondensate = TROC.Sum(a => double.Parse(a.Company_Annual_Oil)) + TROC.Sum(a => double.Parse(a.Company_Annual_Condensate));

                    return new { gasPlantCapacity = gasPlantCapacity, gasFlareds = gasFlareds, directProdCost = directProdCost, oilCondensate = oilCondensate };

                }






            }
            catch (Exception e)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }
    }
}
