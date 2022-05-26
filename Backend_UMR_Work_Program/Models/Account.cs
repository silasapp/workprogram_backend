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



        public async Task<UserToken> isAutheticate(string? email, string? password)
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
                        var concessionInfo = (from c in _context.ADMIN_CONCESSIONS_INFORMATIONs where c.COMPANY_EMAIL == email.Trim() select c).FirstOrDefault();
                        var contractType = concessionInfo?.Contract_Type ?? "";
                        var companyName = getUser?.COMPANY_NAME == "Admin" ? "Admin" : "Company";

                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name, getUser.COMPANY_NAME ?? ""),
                                new Claim(ClaimTypes.Email, getUser.EMAIL ?? ""),
                                new Claim(ClaimTypes.NameIdentifier, getUser.COMPANY_ID ?? ""),
                                new Claim(ClaimTypes.GivenName, getUser?.NAME ?? ""),
                                new Claim(ClaimTypes.Role, companyName)
                            }),
                            Expires = DateTime.UtcNow.AddDays(7),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };
                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        UserToken tok = new UserToken { CompanyId = getUser.COMPANY_ID, CompanyName = getUser.COMPANY_NAME, CompanyEmail = getUser.EMAIL, Name = getUser.NAME, ContractType = contractType, token = tokenHandler.WriteToken(token), code = 1 };
                        return tok;
                    }
                    return new UserToken { code = 2 };
                }
                return new UserToken { code = 3 };


            }
            catch (Exception ex)
            {
                return new UserToken { code = 0 };
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
    }
}