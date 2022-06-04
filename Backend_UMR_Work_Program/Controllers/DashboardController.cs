using Microsoft.AspNetCore.Mvc;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        [HttpGet(Name = "GetDashboardIndex")]
        public void Index()
        {

        }
    }
}
