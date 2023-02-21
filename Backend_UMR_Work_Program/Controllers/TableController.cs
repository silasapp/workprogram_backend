using AutoMapper;
using Backend_UMR_Work_Program.DataModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]")]
    public class TableController : ControllerBase
    {
        //private Account _account;
        public WKP_DBContext _context;
        public IConfiguration _configuration;
        HelpersController _helpersController;
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public TableController(WKP_DBContext context, IConfiguration configuration, HelpersController helpersController, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
            _helpersController = new HelpersController(_context, _configuration, _httpContextAccessor, _mapper);
        }



        [HttpPost("REMOVE_USER")]
        public async Task<int> REMOVE_USER()
        {
            try
            {
                var data = (from c in _context.ADMIN_COMPANY_INFORMATIONs where c.EMAIL == "damilare.olanrewaju@brandonetech.com" select c).ToList();

                _context.RemoveRange(data);
                return await _context.SaveChangesAsync();
               
            }
            catch (Exception e)
            {
                return 0;

            }
        }
    }
}
