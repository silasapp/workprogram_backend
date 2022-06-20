using AutoMapper;
using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using static Backend_UMR_Work_Program.Models.ViewModel;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PresentationScheduleController : ControllerBase
    {
        private Account _account;
        public WKP_DBContext _context;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private string UserWorkRole = "Admin";

        public PresentationScheduleController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            helpersController = new HelpersController(_context, configuration, _httpContextAccessor, _mapper);
        }
        
        
        
        
       
    }
}
