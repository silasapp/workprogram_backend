using System;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Backend_UMR_Work_Program.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using static Backend_UMR_Work_Program.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace Backend_UMR_Work_Program.Models
{
    public class Account
    {
        private string? Email, Password;
        private string? CompanyEmail, CompanyName, Name, CompanyId, ContractType;
        private string? id, COMPANYNAME, chairperson, scribe, presentation_date, presentation_time, meeting_room, days_to_go, system_date;

        private readonly AppSettings _appSettings;
        //private IConfiguration _configuration;
        private Connection mycon;
        public WKP_DBContext _context;

        public Account(IOptions<AppSettings> appSettings, Connection connection, WKP_DBContext context)
        {
            _appSettings = appSettings.Value;
            mycon = connection;
            _context = context;
        }

        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }



        private string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return cipherText;
        }



        public async Task<UserToken> isAutheticate(string email, string password)
        {
            try
            {
                this.Email = email.ToLower();
                this.Password = password;
                if (CheckEmail(email))
                {
                    var getUser = (from a in _context.ADMIN_COMPANY_INFORMATIONs where a.EMAIL == email.Trim() && a.PASSWORDS == Encrypt(password) && a.STATUS_ == "Activated" select a).FirstOrDefault();
                    if (getUser != null)
                    {
                        getUser.LAST_LOGIN_DATE = DateTime.Now;
                        _context.Entry(getUser).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                        var concessionInfo = (from c in _context.ADMIN_CONCESSIONS_INFORMATIONs where c.COMPANY_EMAIL == this.Email select c).FirstOrDefault();
                        
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name, getUser.COMPANY_NAME),
                                new Claim(ClaimTypes.Email, getUser.EMAIL),
                                new Claim(ClaimTypes.NameIdentifier, getUser.COMPANY_ID),
                                new Claim(ClaimTypes.GivenName, getUser.NAME),
                                new Claim(ClaimTypes.Role, concessionInfo.Contract_Type)
                            }),
                            Expires = DateTime.UtcNow.AddDays(7),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };
                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        UserToken tok = new UserToken { CompanyId = getUser.COMPANY_ID, CompanyName = getUser.COMPANY_NAME, CompanyEmail = getUser.EMAIL, Name = getUser.NAME, ContractType = concessionInfo.Contract_Type, token = tokenHandler.WriteToken(token), code = 1 };
                        return tok;
                    }
                }
                return new UserToken { code = 3 };


            }
            catch (Exception ex)
            {
                return new UserToken { code = 3 };
            }
        }



        public bool CheckEmail(string email)
        {
            var getUser = (from a in _context.ADMIN_COMPANY_INFORMATIONs where a.EMAIL == email.Trim() select a).FirstOrDefault();

                    if (getUser == null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
        }

        public object GetData()
        {
            var code = "1295";
            using (SqlConnection con = new SqlConnection(this.mycon.Myconnection))
            {
                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("Select * from ADMIN_DATETIME_PRESENTATION where COMPANY_ID = '" + code + "'", con);

                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds, "ADMIN_COMPANY_INFORMATION");

                DataTable dt = ds.Tables[0];
                int rowCount = dt.Rows.Count;
                //SqlDataAdapter da = new SqlDataAdapter("Select * from ADMIN_DATETIME_PRESENTATION where COMPANY_ID = '" + code + "'", con);
                //   SqlDataAdapter da = new SqlDataAdapter("Select * from ADMIN_DATETIME_PRESENTATION where id = 117 ", con);
                //DataTable result = new DataTable();

                //da.Fill(result);
                return dt;
            }

                
        }

        //public void CHECK_and_LOGIN()
        //{
        //    string cs = ConfigurationManager.ConnectionStrings["App_ConnectionString"].ConnectionString;

        //    using (SqlConnection con = new SqlConnection(cs))
        //    {

        //        SqlCommand cmd = new SqlCommand();
        //        cmd = new SqlCommand("Select * from ADMIN_COMPANY_INFORMATION", con);

        //        cmd.CommandType = CommandType.Text;
        //        cmd.Connection = con;
        //        con.Open();

        //        SqlDataAdapter da = new SqlDataAdapter();
        //        da.SelectCommand = cmd;
        //        DataSet ds = new DataSet();
        //        da.Fill(ds, "ADMIN_COMPANY_INFORMATION");

        //        DataTable dt = ds.Tables[0];
        //        int rowCount = dt.Rows.Count;

        //        int i = 1;

        //        if (rowCount > 0)
        //        {
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                string EMAIL = row["EMAIL"].ToString();
        //                string PASSWORDS = row["PASSWORDS"].ToString();
        //                string STATUS_ = row["STATUS_"].ToString();

        //                string COMPANY_NAME = row["COMPANY_NAME"].ToString();
        //                string NAME = row["NAME"].ToString();
        //                // string STATUS_ = row["STATUS_"].ToString();



        //                if (EMAIL == TextBox1.Text && PASSWORDS == TextBox2.Text && STATUS_ == "Activated")
        //                {
        //                    Session.Remove("Concession_Held");

        //                    Session["Company_Email"] = TextBox1.Text;

        //                    Session["CompanyName"] = COMPANY_NAME;

        //                    Session["NAME"] = NAME;

        //                    return_Data_from_ADMIN_CONCESSIONS_INFORMATION();

        //                    Update_login_date_time();


        //                    if (Request.QueryString["ReturnUrl"] != null) //check if the user is requestin a page before redirected to login
        //                    {
        //                        FormsAuthentication.RedirectFromLoginPage(TextBox1.Text, false);
        //                    }
        //                    else
        //                    {
        //                        FormsAuthentication.SetAuthCookie(TextBox1.Text, false);
        //                        Response.Redirect("work_programme_landing_page.aspx");
        //                    }


        //                }
        //                else
        //                {
        //                    string strMsg = "Wrong Username or/and Password !!!! ..Please try again ";
                            
        //                }
        //                i++;
        //            }
        //        }
        //    }
        //}
    }
}