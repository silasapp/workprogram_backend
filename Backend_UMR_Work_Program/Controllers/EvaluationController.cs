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
        [HttpGet("Best10OPLAcquisitionIndex")]
        public async Task<object> Best10OPLAcquisitionIndex(string year)
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
        [HttpGet("Best10OPLExploratoryIndex")]
        public async Task<object> Best10OPLExploratoryIndex(string year)
        {
            var reel = await (from a in _context.CONCESSION_SITUATIONs where a.Year == year && a.OML_Name.ToLower().Contains("opl") select a).ToListAsync();
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

        [HttpGet("Best10OPLDiscoveryIndex")]
        public async Task<object> Best10OPLDiscoveryIndex(string year)
        {
            var reel = await (from a in _context.CONCESSION_SITUATIONs where a.Year == year && a.OML_Name.ToLower().Contains("opl") select a).ToListAsync();
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

        [HttpGet("Best10OPLConcessionRentalsIndex")]
        public async Task<object> Best10OPLConcessionRentalsIndex(string year)
        {
            var reel = await (from a in _context.CONCESSION_SITUATIONs where a.Year == year && a.OML_Name.ToLower().Contains("opl") select a).ToListAsync();
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

        [HttpGet("Best10OMLAcquisitionIndex")]
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

        [HttpGet("Best10OMLExploratoryIndex")]
        public async Task<object> Best10OMLExploratoryIndex(string year)
        {
            var reel = await (from a in _context.CONCESSION_SITUATIONs where a.Year == year && a.OML_Name.ToLower().Contains("oml") select a).ToListAsync();
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

        [HttpGet("Best10OMLDiscoveryIndex")]
        public async Task<object> Best10OMLDiscoveryIndex(string year)
        {
            var reel = await (from a in _context.CONCESSION_SITUATIONs where a.Year == year && a.OML_Name.ToLower().Contains("oml") select a).ToListAsync();
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

        [HttpGet("Best10OMLConcessionRentalsIndex")]
        public async Task<object> Best10OMLConcessionRentalsIndex(string year)
        {
            var reel = await (from a in _context.CONCESSION_SITUATIONs where a.Year == year && a.OML_Name.ToLower().Contains("oml") select a).ToListAsync();
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

        // public async Task<object> Best10OMLRoyaltyPaymentIndex(string year)
        // {
        //     var reel = await (from a in _context.CONCESSION_SITUATIONs where a.Year == year && a.OML_Name.ToLower().Contains("oml") select a).ToListAsync();
        //     var acqlist = new List<double>();
        //     long value = 0;
        //     int i = 0;

        //     while (i < reel.Count())
        //     {
        //         var concessionRentalPaid = Convert.ToInt32(reel[i]) > 0 ? 100 : 0;
        //         acqlist.Add(concessionRentalPaid);
        //         i++;
        //     }

        //     acqlist.Sort();
        //     return acqlist.Take(10);
        // }

        [HttpGet("Best10OMLReverseReplacementRatio")]
        public async Task<object> Best10OMLReverseReplacementRatio(string year)
        {
            var reel = await (from a in _context.CONCESSION_SITUATIONs where a.Year == year && a.OML_Name.ToLower().Contains("oml") select a).ToListAsync();
            var acqlist = new List<double>();
            long value = 0;
            int i = 0;

            while (i < reel.Count())
            {
                var concessionRentalPaid = Convert.ToInt32(reel[i]) > 0 ? 100 : 0;
                acqlist.Add(concessionRentalPaid);
                i++;
            }

            acqlist.Sort();
            return acqlist.Take(10);
        }

    }
}
