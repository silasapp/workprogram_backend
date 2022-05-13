using Microsoft.AspNetCore.Mvc;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ReportController : Controller
    {
        [HttpPost(Name = "GetIndex")]
        public void Index()
        {

        }
    }
}
