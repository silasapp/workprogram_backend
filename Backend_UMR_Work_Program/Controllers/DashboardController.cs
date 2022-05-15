using Microsoft.AspNetCore.Mvc;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]/[action]")]
    public class DashboardController : ControllerBase
    {
        [HttpGet(Name = "GetDashboardIndex")]
        public void Index()
        {

        }
    }
}
