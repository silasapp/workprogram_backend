using Microsoft.AspNetCore.Mvc;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ReportController : ControllerBase
    {
        [HttpPost(Name = "GetReportIndex")]
        public void Index()
        {

        }
    }
}
