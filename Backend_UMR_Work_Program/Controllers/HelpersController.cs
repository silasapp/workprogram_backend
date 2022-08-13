//using Backend_UMR_Work_Program.BusinessLogic.IRepository;
using Backend_UMR_Work_Program.Controllers.Authentications;
using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using static Backend_UMR_Work_Program.Models.GeneralModel;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Azure.Storage.Blobs;

namespace Backend_UMR_Work_Program.Controllers
{
    public class HelpersController : Controller
    {
        public IConfiguration _configuration;
        public WKP_DBContext _context;
        IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        
        public HelpersController(WKP_DBContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

        }
        private string? WKPUserId => "1";
        private string? WKPUserName => "Name";
        private string? WKPUserEmail => "adeola.kween123@gmail.com";
        private string? WKUserRole => "Admin";
        // private string? WKPUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        //private string? WKPUserName => User.FindFirstValue(ClaimTypes.Name);
        //private string? WKPUserEmail => User.FindFirstValue(ClaimTypes.Email);
        //private string? WKUserRole => User.FindFirstValue(ClaimTypes.Role);

        public List<AppMessage> SaveMessage(int AppID, int userID, string subject, string content, string userElpsID, string type)
        {

            //messages messages = new messages()
            // {
            //     company_id = type.Contains("ompany") ? userID : 0,
            //     UserID = userID,
            //     AppId = AppID,
            //     subject = subject,
            //     content = content,
            //     sender_id = userElpsID,
            //     read = 0,
            //     UserType = type,
            //     date = DateTime.UtcNow.AddHours(1)
            // };
            // _context.messages.Add(messages);
            // _context.SaveChanges();

            // var msg = GetMessage(messages.id, Convert.ToInt32(messages.UserID));
            // return msg;
            return null;
        }

        public List<AppMessage> GetMessage(int msg_id, int seid)
        {

            return null;
        }

        public string SendEmailMessage(string email_to, string email_to_name, List<AppMessage> AppMessages, byte[] attach)
        {
            var result = "";
            var password = _configuration.GetSection("SmtpSettings").GetSection("Password").Value.ToString();
            var username = _configuration.GetSection("SmtpSettings").GetSection("Username").Value.ToString();
            var emailFrom = _configuration.GetSection("SmtpSettings").GetSection("SenderEmail").Value.ToString();
            var Host = _configuration.GetSection("SmtpSettings").GetSection("Server").Value.ToString();
            var Port = Convert.ToInt16(_configuration.GetSection("SmtpSettings").GetSection("Port").Value.ToString());

            var msgBody = CompanyMessageTemplate(AppMessages);

            MailMessage _mail = new MailMessage();
            SmtpClient client = new SmtpClient(Host, Port);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(username, password);
            _mail.From = new MailAddress(emailFrom);
            _mail.To.Add(new MailAddress(email_to, email_to_name));
            _mail.Subject = AppMessages.FirstOrDefault().Subject.ToString();
            _mail.IsBodyHtml = true;
            _mail.Body = msgBody;
            if (attach != null)
            {
                string name = "App Letter";
                Attachment at = new Attachment(new MemoryStream(attach), name);
                _mail.Attachments.Add(at);
            }
            //_mail.CC=
            try
            {
                client.Send(_mail);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string CompanyMessageTemplate(List<AppMessage> AppMessages)
        {
            var msg = AppMessages.FirstOrDefault();
            string body = "<div>";

            body += "<div style='width: 800px; background-color: #ece8d4; padding: 5px 0 5px 0;'><img style='width: 98%; height: 120px; display: block; margin: 0 auto;' src='~/images/nmdpra.png' alt='Logo'/></div>";
            body += "<div class='text-left' style='background-color: #ece8d4; width: 800px; min-height: 200px;'>";
            body += "<div style='padding: 10px 30px 30px 30px;'>";
            body += "<h5 style='text-align: center; font-weight: 300; padding-bottom: 10px; border-bottom: 1px solid #ddd;'>" + msg.Subject + "</h5>";
            body += "<p>Dear Sir/Madam,</p>";
            body += "<p style='line-height: 30px; text-align: justify;'>" + msg.Content + "</p>";

            body += "<table style = 'width: 100%;'><tbody>";
            body += "<tr><td style='width: 200px;'><strong>Company Name:</strong></td><td> " + msg.CompanyName + " </td></tr>";
            body += "</tbody></table><br/>";

            body += "<p> </p>";
            body += "&copy; " + DateTime.Now.Year + "<p>  Powered by NUPRC Work Program Team. </p>";
            body += "<div style='padding:10px 0 10px; 10px; background-color:#888; color:#f9f9f9; width:800px;'> &copy; " + DateTime.UtcNow.AddHours(1).Year + " Nigerian Midstream and Downstream Petroleum Regulatory Authority &minus; NMDPRA Nigeria</div></div></div>";

            return body;
        }

        public string Encrypt(string clearText)
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

        public string Decrypt(string cipherText)
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

        public int DecryptIDs(string ids)
        {
            int id = 0;
            var ID = this.Decrypt(ids);

            if (ID == "Error")
            {
                id = 0;
            }
            else
            {
                id = Convert.ToInt32(ID);
            }

            return id;
        }
        public int getSessionRoleID()
        {
            try
            {
                return Convert.ToInt32(Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionRoleID)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string getSessionEmail()
        {
            try
            {
                return Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionEmail));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int getSessionLogin()
        {
            try
            {
                int sessionLogin = Convert.ToInt32(Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionLogin)));
                return sessionLogin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public int getSessionUserID()
        {
            try
            {
                return Convert.ToInt32(Decrypt(_httpContextAccessor.HttpContext.Session.GetString(AuthController.sessionUserID)));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UploadedDocument UploadDocument(IFormFile document, string folderToSave)
        {
            var document_uniqueFileName = "";
            var document_FileExtension = "";
            var document_newFileName = "";
            var document_documentPath = "";
            var document_fileName = "";
            try
            {
                //For image cover
                if (document != null)
                {
                    if (document.Length > 0)
                    {

                        var up = Path.Combine(Directory.GetCurrentDirectory(), folderToSave);
                        string datetime = DateTime.Now.ToString("ddMMyyyy_HHmmss");

                        //var docName = ContentDispositionHeaderValue.Parse(document.ContentDisposition).FileName.Trim();
                        //FileExtension = Path.GetExtension(docName).ToString().Replace("\n", ''');

                        var uploads = Path.Combine(up, folderToSave);
                        document_fileName = document.FileName.Split(".")[0].Trim();
                        document_uniqueFileName = Convert.ToString(Guid.NewGuid());
                        document_FileExtension = document.FileName.Split(".")[1].Trim();
                        //newFileName = uniqueFileName + FileExtension;
                        document_newFileName = document_fileName +"_"+datetime+ "." + document_FileExtension;

                        // store path of folder in database
                        document_documentPath = "//Documents/" + folderToSave + "/" + document_newFileName;
                        using (var s = new FileStream(Path.Combine(uploads, document_newFileName),
                             FileMode.Create))
                        {
                            document.CopyTo(s);
                        }

                    }
                    var uploadedDoc = new UploadedDocument()
                    {
                        fileName = document_fileName,
                        filePath = document_documentPath
                    };
                    return uploadedDoc;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;

            }
        }
        public async Task<UploadedDocument> UploadBlobDocument(IFormFile document, string folderToSave)
        {
            try
            {
                if (document != null)
                {

                            string Connection = _configuration.GetSection("BlobStorage").GetSection("BlobConnectionString").Value.ToString();
                            string containerName = _configuration.GetSection("BlobStorage").GetSection("BlobContainerName").Value.ToString();
                            Stream myBlob = new MemoryStream();
                            string fileExtension = document.FileName.Split(".")[1].Trim();
                            string fileName = document.Name + Guid.NewGuid().ToString() + fileExtension;
                            myBlob = document.OpenReadStream();
                            var blobClient = new BlobContainerClient(Connection, containerName);
                            var blob = blobClient.GetBlobClient(fileName);
                            await blob.UploadAsync(myBlob);
                    
                            var uploadedDoc = new UploadedDocument()
                            {
                                fileName = fileName,
                                filePath = blob.Uri.AbsoluteUri,
                            };
                        
                            DownloadBlob(fileName);
                             return uploadedDoc;


                } return null;
            }
            catch (Exception ex)
            {
                return null;

            }
        }

        public static async Task DownloadBlob(string filePath, BlobClient blobClient = null)
        {
            string Connection = Environment.GetEnvironmentVariable("BlobConnectionString");
            string containerName = Environment.GetEnvironmentVariable("BlobContainerName");
            //BlobClient blobClient = new BlobClient();
            //var blobClient = new BlobContainerClient(Connection, containerName);
            //FileStream fileStream = File.OpenWrite(filePath);
            await blobClient.DownloadToAsync(filePath);
        }


    }
}
