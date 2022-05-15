using Microsoft.AspNetCore.Mvc;
using Backend_UMR_Work_Program.Models;

namespace Backend_UMR_Work_Program.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private Account _account;
        public AccountController(Account account)
        {
            _account = account;
        }

        [HttpPost(Name = "Authenticate")]
        public object Authenticate(string email, string password)
        {
            _account.isAutheticate(email, password);
            return null;
        }

        //private string id, COMPANYNAME, chairperson, scribe, presentation_date, presentation_time, meeting_room, days_to_go, system_date;

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    // string foldertosave = Server.MapPath("") + "\\Test\\";

        //}


        //public string ShowClaimsInTable()
        //{
        //    //long exp = long.Parse(ClaimsPrincipal.Current.FindFirst("exp").Value);
        //    //DateTime expireTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        //    //expireTime = expireTime.AddSeconds(exp);

        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendFormat("<table>");
        //    sb.AppendFormat("<tr><td>App Client Id:</td><td>{0}</td></tr>", ConfigurationManager.AppSettings["aad.clientid"]);
        //    sb.AppendFormat("<tr><td>App URI:</td><td>{0}</td></tr>", ConfigurationManager.AppSettings["aad.appiduri"]);
        //    sb.AppendFormat("<tr><td>Unique Name:</td><td>{0}</td></tr>", ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value);
        //    //sb.AppendFormat("<tr><td>Object Id:</td><td>{0}</td></tr>", ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value);
        //    //sb.AppendFormat("<tr><td>tenant Id:</td><td>{0}</td></tr>", ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value);
        //    //sb.AppendFormat("<tr><td>Name:</td><td>{0}</td></tr>", ClaimsPrincipal.Current.FindFirst("name").Value);
        //    //sb.AppendFormat("<tr><td>Given Name:</td><td>{0}</td></tr>", ClaimsPrincipal.Current.FindFirst(ClaimTypes.GivenName).Value);
        //    //sb.AppendFormat("<tr><td>Surname:</td><td>{0}</td></tr>", ClaimsPrincipal.Current.FindFirst(ClaimTypes.Surname).Value);
        //    //sb.AppendFormat("<tr><td>UPN:</td><td>{0}</td></tr>", ClaimsPrincipal.Current.FindFirst(ClaimTypes.Upn).Value);
        //    //sb.AppendFormat("<tr><td>Expires:</td><td>{0}</td></tr>", expireTime.ToString("u"));
        //    sb.AppendFormat("</table>");

        //    return sb.ToString();
        //}

        //protected void Button2_Click(object sender, EventArgs e)
        //{
        //    if (TextBox1.Text == "")
        //    {
        //        string strMsg = "Please Enter Email";
        //        Response.WriteAsync("<script>alert('" + strMsg + "')</script>");
        //        TextBox1.Focus();
        //        return;
        //    }

        //    if (TextBox2.Text == "")
        //    {
        //        string strMsg = "Please Enter Password ";
        //        Response.WriteAsync("<script>alert('" + strMsg + "')</script>");
        //        TextBox2.Focus();
        //        return;
        //    }


        //    // Response.Redirect("work_programme_landing_page.aspx");

        //    TextBox1.Text = TextBox1.Text.ToLower();

        //    isAutheticate(TextBox1.Text, Encrypt(TextBox2.Text));

        //    // CHECK_and_LOGIN();
        //}








    }
}
