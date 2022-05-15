using System;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Backend_UMR_Work_Program.Services;

namespace Backend_UMR_Work_Program.Models
{
    public class Account
    {
        private string? email, password;
        private string? companyEmail, companyName, name, companyId, contractType;
        private string? id, COMPANYNAME, chairperson, scribe, presentation_date, presentation_time, meeting_room, days_to_go, system_date;

        private readonly AppSettings _appSettings;
        //private IConfiguration _configuration;
        private Connection mycon;

        public Account(IOptions<AppSettings> appSettings, Connection connection)
        {
            _appSettings = appSettings.Value;
            mycon = connection;
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


        private void Update_login_date_time()
        {
            system_date = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"); // DATE TIME
            //Connection mycon = new Connection();
            SqlConnection con = new SqlConnection(this.mycon.Myconnection);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            con.Open();

            try
            {
                // string email_days_to_go = Check_Days_to_send_Email + "Day(s)";

                cmd.CommandText = "Update ADMIN_COMPANY_INFORMATION  SET  last_login_date = '" + system_date + "'    WHERE  EMAIL  = '" + this.email + "' ";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                string myMsgErr = ex.Message;
                con.Close();
            }
            finally
            {
                con.Close();
                //  Response.Redirect("tx_view_status_installation.aspx");
            }
        }



        private void isAutheticate(string email, string password) // Pass the Isauthentication into access for username and password
        {
            this.email = email.ToLower();
            this.password = password;
            //Connection mycon = new Connection();
            SqlConnection con = new SqlConnection(this.mycon.Myconnection);

            SqlCommand cmd = new SqlCommand("select * from ADMIN_COMPANY_INFORMATION where EMAIL = '" + email + "' and PASSWORDS =   '" + password + "' COLLATE Latin1_General_CS_AS  and STATUS_ =   'Activated' ", con);

            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            try
            {
                con.Open();

                System.Data.SqlClient.SqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    //Session.Remove("Concession_Held");
                    //Session.Remove("BACK_PAGE_SESSION");

                     this.companyEmail = this.email;

                    this.companyName = rd["COMPANY_NAME"].ToString();

                    this.name = rd["NAME"].ToString();

                    this.companyId = rd["COMPANY_ID"].ToString();

                    return_Data_from_ADMIN_CONCESSIONS_INFORMATION();

                    Update_login_date_time();


                    //if (Request.QueryString["ReturnUrl"] != null) //check if the user is requestin a page before redirected to login
                    //{
                    //    FormsAuthentication.RedirectFromLoginPage(TextBox1.Text, false);
                    //}
                    //else
                    //{
                    //    FormsAuthentication.SetAuthCookie(TextBox1.Text, false);
                    //    Response.Redirect("work_programme_landing_page.aspx");
                    //}

                }
                else
                {
                    string? strMsg = "Wrong Username or/and Password !!!! ..Please try again ";
                }

            }
            catch (Exception ex)
            {
                //myStatus = false;
                string ErrMsg = ex.Message;
                con.Close();
            }
            finally
            {
                con.Close();
            }
            //  return myStatus;

        }


        private void return_Data_from_ADMIN_CONCESSIONS_INFORMATION() // Pass the Isauthentication into access for username and password
        {
            //Connection mycon = new Connection();
            SqlConnection con = new SqlConnection(this.mycon.Myconnection);

            SqlCommand cmd = new SqlCommand("select * from ADMIN_CONCESSIONS_INFORMATION where COMPANY_EMAIL = '" + this.email + "' ", con);
            // OracleCommand cmd = new OracleCommand("select * from TXRMS_USERS where username = '" + uname + "' and password  = '" + pword + "'  ", con);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            try
            {
                con.Open();
                System.Data.SqlClient.SqlDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    // Session["Company_Email"] = TextBox1.Text;

                    this.contractType = rd["Contract_Type"].ToString();
                }

            }
            catch (Exception ex)
            {
                //myStatus = false;
                string ErrMsg = ex.Message;
                con.Close();
            }
            finally
            {
                con.Close();
            }
            //  return myStatus;

        }





        private void Work_Program_Email_Notification()
        {
            ////***************************************** Mailing to user that he has requested Space**********

            mails requester_mail = new mails();

            requester_mail.NES_subject = "Work Program Notification";

            requester_mail.Official_name = " <b> '" + this.email + "'  </b> ";

            requester_mail.NES_body = "You have been scheduled to officiate Work Program activity . <br/>  <br/> Please find Work Program details below.Thank you.";

            requester_mail.Chairperson = "Project Manager's Comment: ";

            requester_mail.Scribe = "Project Manager's Comment: ";

            requester_mail.Presentation_date = "Project Manager's Comment: ";

            requester_mail.Presentation_time = "Project Manager's Comment: ";

            requester_mail.Days_to_go = "Project Manager's Comment: ";

            requester_mail.Meeting_room = "Project Manager's Comment: ";

            requester_mail.ToRecieved = "anthony.nwosu@brandonetech.com";
            requester_mail.ToRecievedCC = "tonygentle2000@yahoo.com";

            //requester_mail.ToRecieved = "anthony.nwosu@brandonetech.com";
            //requester_mail.ToRecievedCC = "tonygentle2000@yahoo.com";
            //  requester_mail.Applink = "Click <a href=http://workprogram.azurewebsites.net/> HERE</a> to Action the request";

            requester_mail.NES_SendMail_Project_Team();

            //Label48.Text = requester_mail.Email_test;


            ////***************************************************** END****************************************
        }


        private void wp_check_ADMIN_DATETIME_PRESENTATION_Table_and_send_email()
        {

            //Connection mycon = new Connection();
            SqlConnection con = new SqlConnection(this.mycon.Myconnection);

            SqlDataAdapter da = new SqlDataAdapter("Select * from ADMIN_DATETIME_PRESENTATION ", con);
            //   SqlDataAdapter da = new SqlDataAdapter("Select * from ADMIN_DATETIME_PRESENTATION where id = 117 ", con);
            DataTable result = new DataTable();

            da.Fill(result);

            string countrecords = result.Rows.Count.ToString();

            int count_record = int.Parse(countrecords);
            for (int m = 0; m < count_record; m++)
            {
                try
                {
                    id = result.Rows[m]["id"].ToString(); // REPORT REF
                    string? COMPANYNAME = result.Rows[m]["COMPANYNAME"].ToString(); // 

                    string? chairperson = result.Rows[m]["CHAIRPERSON"].ToString(); // 
                    string? scribe = result.Rows[m]["SCRIBE"].ToString(); // 

                    string? presentation_date = result.Rows[m]["DATE_TIME_TEXT"].ToString(); // 
                    string? presentation_time = result.Rows[m]["wp_time"].ToString(); // 

                    string? meeting_room = result.Rows[m]["MEETINGROOM"].ToString(); // 
                                                                                    //string days_to_go = result.Rows[m]["wp_date"].ToString(); // 


                    string? wp_date = result.Rows[m]["wp_date"].ToString(); // 
                    DateTime wp_date_scheduled = System.Convert.ToDateTime(wp_date); // This Holds the value for the initiation date ...

                    string? SystemDate_string = DateTime.Now.ToString("MM/dd/yyyy"); // DATE TIME

                    DateTime System_endDate = System.Convert.ToDateTime(SystemDate_string);

                    TimeSpan ts_totalduration;

                    ts_totalduration = wp_date_scheduled.Subtract(System_endDate);

                    int weekends = 0;

                    //// The for is to check if the date for the work program has passed ... this mean the start date must me less than the end date
                    //for (DateTime tempDt = wp_date_scheduled; tempDt.Date < System_endDate.Date; tempDt = tempDt.AddDays(1))
                    //{
                    //    if (tempDt.DayOfWeek == DayOfWeek.Saturday || tempDt.DayOfWeek == DayOfWeek.Sunday)
                    //        weekends++;
                    //}


                    for (DateTime tempDt = System_endDate; tempDt.Date < wp_date_scheduled.Date; tempDt = tempDt.AddDays(1))
                    {
                        if (tempDt.DayOfWeek == DayOfWeek.Saturday || tempDt.DayOfWeek == DayOfWeek.Sunday)
                            weekends++;
                    }


                    ts_totalduration = ts_totalduration.Subtract(new TimeSpan(weekends, 0, 0, 0));

                    string Check_Days_to_send_Email = ts_totalduration.Days.ToString();

                    if (Check_Days_to_send_Email == "7")
                    {
                        string Check1 = "Yes";
                    }

                    if (Check_Days_to_send_Email == "3")
                    {
                        string Check2 = "Yes";
                    }

                    if (Check_Days_to_send_Email == "1")
                    {
                        string Check3 = "Yes";
                    }


                    string Check = "Yes";

                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                    con.Close();
                }
                finally
                {
                    con.Close();
                }

            }



        }




        public void Check_and_Login()
        {
            
            string? cs = mycon.Myconnection;

            using (SqlConnection con = new SqlConnection(cs))
            {

                SqlCommand cmd = new SqlCommand();
                cmd = new SqlCommand("Select * from ADMIN_COMPANY_INFORMATION", con);

                cmd.CommandType = CommandType.Text;
                cmd.Connection = con;
                con.Open();

                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds, "ADMIN_COMPANY_INFORMATION");

                DataTable dt = ds.Tables[0];
                int rowCount = dt.Rows.Count;

                int i = 1;

                if (rowCount > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string? EMAIL = row["EMAIL"].ToString();
                        string? PASSWORDS = row["PASSWORDS"].ToString();
                        string? STATUS_ = row["STATUS_"].ToString();

                        string? COMPANY_NAME = row["COMPANY_NAME"].ToString();
                        string? NAME = row["NAME"].ToString();
                        // string STATUS_ = row["STATUS_"].ToString();



                        if (EMAIL == this.email && PASSWORDS == this.password && STATUS_ == "Activated")
                        {
                            //Session.Remove("Concession_Held");

                            this.companyEmail = this.email;

                            this.companyName = COMPANY_NAME;

                            this.name = NAME;

                            return_Data_from_ADMIN_CONCESSIONS_INFORMATION();

                            Update_login_date_time();


                            //if (Request.QueryString["ReturnUrl"] != null) //check if the user is requestin a page before redirected to login
                            //{
                            //    FormsAuthentication.RedirectFromLoginPage(TextBox1.Text, false);
                            //}
                            //else
                            //{
                            //    FormsAuthentication.SetAuthCookie(TextBox1.Text, false);
                            //    Response.Redirect("work_programme_landing_page.aspx");
                            //}
                        }
                        else
                        {
                            string strMsg = "Wrong Username or/and Password !!!! ..Please try again ";
                            //Response.Write("<script>alert('" + strMsg + "')</script>");
                        }
                        i++;
                    }
                }
            }
        }
    }
}