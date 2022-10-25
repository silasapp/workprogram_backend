using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;
using Newtonsoft.Json;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using static Backend_UMR_Work_Program.Models.ViewModel;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]")]
    public class EvaluationController : ControllerBase
    {
        private Account _account;
        public WKP_DBContext _context;

        public EvaluationController(WKP_DBContext context, Account account)
        {
            _account = account;
            _context = context;
        }

        {
            var reel = await (from a in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where a.Year_of_WP == year && a.OML_Name.ToLower().Contains("opl") select a).ToListAsync();
            var acqlist = new List<double>();
            long value = 0;
            int i = 0;

            while (i < reel.Count())
            {
                double acq = (Convert.ToInt64(reel[i].Quantum) / Convert.ToInt64(reel[i].Quantum_Planned)) * 100;
                acqlist.Add(acq);
                i++;
            }

            acqlist.Sort();
            return acqlist.Take(10);
        }

        [HttpGet("Best10_OPLExploratoryIndex")]
         public async Task<object> Best10OPLExploratoryIndex(string year)
        {
            var acqlist = new List<double>();
            long value = 0;
            int i = 0;

            while (i < reel.Count())
            {
                var drillingFieldPresent = Convert.ToInt32(reel[i].No_of_discovered_field) > 0 ? 100 : 0;
                acqlist.Add(drillingFieldPresent);
                i++;
            }

            acqlist.Sort();
            return acqlist.Take(10);
        }
        public async Task<object> Best10OPLDiscoveryIndex(string year)
        {
            var acqlist = new List<double>();
            long value = 0;
            int i = 0;

            while (i < reel.Count())
            {
                var discoveredFieldPresent = Convert.ToInt32(reel[i].No_of_discovered_field) > 0 ? 100 : 0;
                acqlist.Add(discoveredFieldPresent);
                i++;
            }

            acqlist.Sort();
            return acqlist.Take(10);
        }
        public async Task<object> Best10OPLConcessionRentalsIndex(string year)
        {
            var acqlist = new List<double>();
            long value = 0;
            int i = 0;

            while (i < reel.Count())
            {
                var concessionRentalPaid = Convert.ToInt32(reel[i].Has_the_Concession_Rentals_been_paid) > 0 ? 100 : 0;
                acqlist.Add(concessionRentalPaid);
                i++;
            }

            acqlist.Sort();
            return acqlist.Take(10);
        }
        public async Task<object> Best10OMLAcquisitionIndex(string year)
        {
            var reel = await (from a in _context.GEOPHYSICAL_ACTIVITIES_ACQUISITIONs where a.Year_of_WP == year && a.OML_Name.ToLower().Contains("oml") select a).ToListAsync();
            var acqlist = new List<double>();
            long value = 0;
            int i = 0;

            while (i < reel.Count())
            {
                double acq = (Convert.ToInt64(reel[i].Quantum) / Convert.ToInt64(reel[i].Quantum_Planned)) * 100;
                acqlist.Add(acq);
                i++;
            }

            acqlist.Sort();
            return acqlist.Take(10);
        }
        public async Task<object> Best10OMLExploratoryIndex(string year)
        {
            var acqlist = new List<double>();
            long value = 0;
            int i = 0;

            while (i < reel.Count())
            {
                var drillingFieldPresent = Convert.ToInt32(reel[i].No_of_discovered_field) > 0 ? 100 : 0;
                acqlist.Add(drillingFieldPresent);
                i++;
            }

            acqlist.Sort();
            return acqlist.Take(10);
        }
        public async Task<object> Best10OMLDiscoveryIndex(string year)
        {
            var acqlist = new List<double>();
            long value = 0;
            int i = 0;

            while (i < reel.Count())
            {
                var discoveredFieldPresent = Convert.ToInt32(reel[i].No_of_discovered_field) > 0 ? 100 : 0;
                acqlist.Add(discoveredFieldPresent);
                i++;
            }

            acqlist.Sort();
            return acqlist.Take(10);
        }
        public async Task<object> Best10OMLConcessionRentalsIndex(string year)
        {
            var acqlist = new List<double>();
            long value = 0;
            int i = 0;

            while (i < reel.Count())
            {
                var concessionRentalPaid = Convert.ToInt32(reel[i].Has_the_Concession_Rentals_been_paid) > 0 ? 100 : 0;
                acqlist.Add(concessionRentalPaid);
                i++;
            }

            acqlist.Sort();
            return acqlist.Take(10);
        }




        {
            int i = 0;
            netProduction_Data.GroupBy(x => x.conc.OML_ID).ToList().ForEach(key =>
            {
                
                double CAPEX = key.Where(x => x.year.ToString() == (N).ToString()).Sum(x => double.Parse(x.reserve.Company_Annual_Oil));
                double OPEX = key.Where(x => x.year.ToString() == (N).ToString()).Sum(x => double.Parse(x.reserve.Company_Annual_AG));
                
                double oil_NetProduction_CY = key.Where(x => x.year.ToString() == (N).ToString()).Sum(x => double.Parse(x.reserve.Company_Annual_Oil));
                double AG_NetProduction_CY = key.Where(x => x.year.ToString() == (N).ToString()).Sum(x => double.Parse(x.reserve.Company_Annual_AG));
                double NAG_NetProduction_CY = key.Where(x => x.year.ToString() == (N).ToString()).Sum(x => double.Parse(x.reserve.Company_Annual_NAG));
                double Condensate_NetProduction_CY = key.Where(x => x.year.ToString() == (N).ToString()).Sum(x => double.Parse(x.reserve.Company_Annual_Condensate));
                NetProduction_CY = oil_NetProduction_CY + AG_NetProduction_CY + NAG_NetProduction_CY + Condensate_NetProduction_CY;

                double COPEX = CAPEX + OPEX;
                double costEfficiency_Index = COPEX > 0 ? NetProduction_CY / COPEX : 0;

            {
            }
        [HttpGet("Concession_CSR_Index")]
        public async Task<object> Concession_CSR_Index(string year, string type)
        {
            int N = int.Parse(year);
            var data = await (from conc in _context.CONCESSION_SITUATIONs
                              join comp in _context.ADMIN_COMPANY_INFORMATIONs on conc.COMPANY_ID equals comp.COMPANY_ID
                              join csr in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs on conc.Id.ToString() equals csr.OML_ID
                              where csr.Actual_proposed == GeneralModel.ActualYear && csr.Actual_Proposed_Year == year
                               select new
                              {
                                  year = year,
                                  conc = conc,
                                  csr = csr,
                                  comp
                              }).ToListAsync();

            var acqlist = new List<Concession_Index>();
            int i = 0;
            data.GroupBy(x => x.conc.OML_ID).ToList().ForEach(key =>
            {
                
                double Actual_CSR = key.Sum(x => double.Parse(x.csr.Actual_Spent));
                double Planned_CSR = key.Sum(x => double.Parse(x.csr.Budget_));
                
                double CSR_Index = Planned_CSR > 0 ? Actual_CSR / Planned_CSR : 0;

        }
        [HttpGet("Concession_StatutoryPayment_Index")]
        public async Task<object> Concession_StatutoryPayment_Index(string year, string type)
        {
            int N = int.Parse(year);
            var data = await (from conc in _context.CONCESSION_SITUATIONs
                              join comp in _context.ADMIN_COMPANY_INFORMATIONs on conc.COMPANY_ID equals comp.COMPANY_ID
                              join csr in _context.HSE_SUSTAINABLE_DEVELOPMENT_COMMUNITY_PROJECT_PROGRAM_CSR_NEWs on conc.Id.ToString() equals csr.OML_ID
                              where csr.Actual_proposed == GeneralModel.ActualYear && csr.Actual_Proposed_Year == year
                               select new
                              {
                                  year = year,
                                  conc = conc,
                                  csr = csr,
                                  comp
                              }).ToListAsync();

            var acqlist = new List<Concession_Index>();
            int i = 0;
            data.GroupBy(x => x.conc.OML_ID).ToList().ForEach(key =>
            {
                
                double Actual_CSR = key.Sum(x => double.Parse(x.csr.Actual_Spent));
                double Planned_CSR = key.Sum(x => double.Parse(x.csr.Budget_));
                
                double CSR_Index = Planned_CSR > 0 ? Actual_CSR / Planned_CSR : 0;

                acqlist.Add(new Concession_Index()
                {
                    companyName = key?.FirstOrDefault()?.comp?.COMPANY_NAME,
                    concessionName = key?.FirstOrDefault()?.conc.OML_Name,
                    concessionType = type,
                    CSR_Index = CSR_Index
                });
            });
            return acqlist;
        }
    }
}
