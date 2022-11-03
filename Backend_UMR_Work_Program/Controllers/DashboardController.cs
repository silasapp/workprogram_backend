using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Backend_UMR_Work_Program.Models.GeneralModel;

namespace Backend_UMR_Work_Program.Controllers
    {
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
            try { 
            var data = await (from conc in _context.CONCESSION_SITUATIONs
                                            join comp in _context.ADMIN_COMPANY_INFORMATIONs on conc.COMPANY_ID equals comp.COMPANY_ID
                                            join reserve in _context.RESERVES_UPDATES_OIL_CONDENSATE_Company_Annual_PRODUCTIONs on conc.Id.ToString() equals reserve.OML_ID
                                            join reserveStatus in _context.RESERVES_UPDATES_OIL_CONDENSATE_STATUS_OF_RESERVEs on conc.Id.ToString() equals reserveStatus.OML_ID
                                            where reserve.Year_of_WP == year && (reserve.Created_by == "gbenga.maku@newcross.com" || reserve.CompanyNumber == WKPCompanyNumber)
                                            && (reserveStatus.Created_by == "gbenga.maku@newcross.com" || reserveStatus.CompanyNumber == WKPCompanyNumber)
                                            select new
                                            {
                                                year = reserve.Year_of_WP,
                                                conc = conc,
                                                reserve = reserve,
                                                reserveStatus = reserveStatus,
                                                comp
                                            }).ToListAsync();

            var companyDashboard_Data = new CompanyReportModel();
            var companyDashboard_Report = new CompanyDashboardModel();
            var companyConcessions = await (from d in _context.ADMIN_CONCESSIONS_INFORMATIONs where d.CompanyNumber == WKPCompanyNumber && d.DELETED_STATUS != "DELETED" select d).ToListAsync();


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
                        AG_Reserves = key.Where(x => x.year == year && x.reserveStatus.Company_Reserves_AnnualGasAGProduction  != null).Sum(x => double.Parse(x.reserveStatus?.Company_Reserves_AnnualOilProduction)),
                        NAG_Reserves = key.Where(x => x.year == year && x.reserveStatus.Company_Reserves_AnnualGasNAGProduction != null).Sum(x => double.Parse(x.reserveStatus?.Company_Reserves_AnnualOilProduction)),
                        condensate_Reserves = key.Where(x => x.year == year && x.reserveStatus.Company_Reserves_AnnualCondensateProduction != null).Sum(x => double.Parse(x.reserveStatus?.Company_Reserves_AnnualCondensateProduction)),

                     };
                   companyDashboard_Data.totalNetProduction = companyDashboard_Data.oil_NetProduction + companyDashboard_Data.AG_NetProduction + companyDashboard_Data.NAG_NetProduction + companyDashboard_Data.condensate_NetProduction;
                   companyDashboard_Data.totalReserves = companyDashboard_Data.oil_Reserves + companyDashboard_Data.AG_Reserves + companyDashboard_Data.NAG_Reserves + companyDashboard_Data.condensate_Reserves;
                   companyDashboard_Report.CompanyReportModel.Add(companyDashboard_Data);
            });
            companyDashboard_Report.OML_Count = companyConcessions.Where(x=> x.Consession_Type.ToLower() == GeneralModel.OML.ToLower()).Count();
            companyDashboard_Report.OPL_Count = companyConcessions.Where(x=> x.Consession_Type.ToLower() == GeneralModel.OPL.ToLower()).Count();
            companyDashboard_Report.No_Of_ProducingFields_Count = await (from d in _context.COMPANY_FIELDs where d.CompanyNumber == WKPCompanyNumber && d.DeletedStatus != true select d).CountAsync();

            return companyDashboard_Report;
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error : " + e.Message, StatusCode = ResponseCodes.InternalError };
            }
        }

    }
}
