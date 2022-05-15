using Microsoft.AspNetCore.Mvc;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]/[action]")]
    public class PresentationController : ControllerBase
    {
        [HttpPost(Name = "GetPresentationIndex")]
        public void Index()
        {
            
        }
    }
}
