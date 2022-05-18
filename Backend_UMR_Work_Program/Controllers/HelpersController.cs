using Backend_UMR_Work_Program.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using static Backend_UMR_Work_Program.Models.GeneralModel;

namespace Backend_UMR_Work_Program.Controllers
{
    public class HelpersController : Controller
    {
        public IConfiguration _configuration;
        public WKP_DBContext _context;
        public HelpersController (WKP_DBContext context, IConfiguration configuration){
            _context = context;
            _configuration = configuration;
       }



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
            body += "&copy; " + DateTime.Now.Year  + "<p>  Powered by NUPRC Work Program Team. </p>";
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




        //public string Encrypt(string clearText)
        //{
        //    try
        //    {
        //        byte[] b = ASCIIEncoding.ASCII.GetBytes(clearText);
        //        string crypt = Convert.ToBase64String(b);
        //        byte[] c = ASCIIEncoding.ASCII.GetBytes(crypt);
        //        string encrypt = Convert.ToBase64String(c);

        //        return encrypt;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error";
        //        throw ex;
        //    }
        //}

        //public string Decrypt(string cipherText)
        //{
        //    try
        //    {
        //        byte[] b;
        //        byte[] c;
        //        string decrypt;
        //        b = Convert.FromBase64String(cipherText);
        //        string crypt = ASCIIEncoding.ASCII.GetString(b);
        //        c = Convert.FromBase64String(crypt);
        //        decrypt = ASCIIEncoding.ASCII.GetString(c);

        //        return decrypt;
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error";
        //        throw ex;
        //    }

        //}

    }
}
