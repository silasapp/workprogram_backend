﻿using System.Data;
using System.Data.SqlClient;
using AutoMapper;
using Backend_UMR_Work_Program.DataModels;
using ClosedXML.Excel;
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



        // [HttpPost("REMOVE_USER")]
        // public async Task<int> REMOVE_USER()
        // {
        //     try
        //     {
        //         var data = (from c in _context.ADMIN_COMPANY_INFORMATIONs where c.EMAIL == "damilare.olanrewaju@brandonetech.com" select c).ToList();

        //         _context.RemoveRange(data);
        //         return await _context.SaveChangesAsync();
               
        //     }
        //     catch (Exception e)
        //     {
        //         return 0;

        //     }
        // }


        [HttpGet("ViewTable")]
        public async Task<FileStreamResult> ViewTable(string tableName)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection("Server=tcp:workprogram.database.windows.net,1433;Initial Catalog=workprogram;Persist Security Info=False;User ID=workprogram;Password=Br@nd0ne;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"))
                {
                    await conn.OpenAsync();
                    string query0 = $"SELECT * FROM {tableName};";
                    SqlCommand cmd0 = new SqlCommand(query0, conn);
                    var reader = await cmd0.ExecuteReaderAsync();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    XLWorkbook wb = new XLWorkbook();
                    wb.Worksheets.Add(dt, "WKPTable");
                    Stream fs = new MemoryStream();
                    wb.SaveAs(fs);
                    fs.Position = 0;
                    var file = new FormFile(fs, 0, fs.Length, "name", "WKPTable");
                    return File(file.OpenReadStream(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.Name);
                }
               
            }
            catch (Exception e)
            {
                return null;

            }
        }
    }
}