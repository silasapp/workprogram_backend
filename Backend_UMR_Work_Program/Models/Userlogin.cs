using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Data.OracleClient;

namespace nes_workflow
{
    public class Userlogin
    {
        public Userlogin()
        {
            //
            // TODO: Add constructor logic here
            //
        }





        private string usernamecandidate;
        public string Usernamecandidate
        {
            get { return usernamecandidate; }
            set { usernamecandidate = value; }
        }

        private string passwordcan;
        public string Passwordcan
        {
            get { return passwordcan; }
            set { passwordcan = value; }
        }

        private string Name;

        public string myName
        {
            get { return Name; }
            set { Name = value; }
        }

        private string Phoneno;

        public string MyPhoneno
        {
            get { return Phoneno; }
            set { Phoneno = value; }
        }

        private string MSIDN;

        public string MyMSIDN
        {
            get { return MSIDN; }
            set { MSIDN = value; }
        }

        private string myRole;

        public string MyRole
        {
            get { return myRole; }
            set { myRole = value; }
        }






        private string myErrMsg;
        public string ErrMsg
        {
            get { return myErrMsg; }
            set { myErrMsg = value; }
        }

        private string LM;
        public string Linemanager
        {
            get { return LM; }
            set { LM = value; }
        }

        private bool myStatus;
        public bool AuthenticationStatus
        {
            get { return myStatus; }
            set { myStatus = value; }
        }


        private string nes_name;
        public string Nes_name
        {
            get { return nes_name; }
            set { nes_name = value; }
        }


        private string nes_email;
        public string Nes_email
        {
            get { return nes_email; }
            set { nes_email = value; }
        }


        private string nes_username;
        public string Nes_username
        {
            get { return nes_username; }
            set { nes_username = value; }
        }


        private string nes_role;
        public string Nes_role
        {
            get { return nes_role; }
            set { nes_role = value; }
        }

        private string sub_nes_role;
        public string sub_Nes_role
        {
            get { return sub_nes_role; }
            set { sub_nes_role = value; }
        }



        //  public bool isAutheticate(string uname, string pword) // Pass the Isauthentication into access for username and password
        public bool isAutheticate(string uname) // Pass the Isauthentication into access for username and password
        {
            Connection mycon = new Connection();
            OracleConnection con = new OracleConnection(mycon.Myconnection);
         
            OracleCommand cmd = new OracleCommand("select * from NES_USERS where username = '" + uname + "' ", con);
            // OracleCommand cmd = new OracleCommand("select * from TXRMS_USERS where username = '" + uname + "' and password  = '" + pword + "'  ", con);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;

            try
            {
                con.Open();
                // SqlDataReader rd = cmd.ExecuteReader();
                System.Data.OracleClient.OracleDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    //usernamecandidate = rd["username"].ToString();
                    //Name = rd["NAME"].ToString();

                    nes_name = rd["NAME"].ToString();
                    nes_email = rd["EMAIL"].ToString();
                    nes_username = rd["username"].ToString();
                    nes_role = rd["role"].ToString();
                    sub_nes_role = rd["sub_role"].ToString();

                    myStatus = true;
                }
                else
                {
                    //Label6.Visible = true;
                    myStatus = false;
                }

            }
            catch (Exception ex)
            {
                myStatus = false;
                ErrMsg = ex.Message;
                con.Close();
            }
            finally
            {
                con.Close();
            }
            return myStatus;

        }



        public bool isAutheticate_users_in_setup(string uname) // Pass the Isauthentication into access for username and password
        {
            Connection mycon = new Connection();
            OracleConnection con = new OracleConnection(mycon.Myconnection);

            OracleCommand cmd = new OracleCommand("select * from NES_USERS where username = '" + uname + "' ", con);
            // OracleCommand cmd = new OracleCommand("select * from TXRMS_USERS where username = '" + uname + "' and password  = '" + pword + "'  ", con);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;

            try
            {
                con.Open();
                // SqlDataReader rd = cmd.ExecuteReader();
                System.Data.OracleClient.OracleDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    //usernamecandidate = rd["username"].ToString();
                    //Name = rd["NAME"].ToString();

                    //nes_name = rd["NAME"].ToString();
                    //nes_email = rd["EMAIL"].ToString();
                    //nes_username = rd["username"].ToString();
                    //nes_role = rd["role"].ToString();

                    myStatus = true;
                }
                else
                {
                    //Label6.Visible = true;
                    myStatus = false;
                }

            }
            catch (Exception ex)
            {
                myStatus = false;
                ErrMsg = ex.Message;
                con.Close();
            }
            finally
            {
                con.Close();
            }
            return myStatus;

        }




        public bool isAutheticate_vendor(string uname, string pword, string vendor) // Pass the Isauthentication into access for username and password
        {
            Connection mycon = new Connection();
            OracleConnection con = new OracleConnection(mycon.Myconnection);

            // OracleCommand cmd = new OracleCommand("select * from SOCR_USERS where username = '" + uname + "' ", con);
            OracleCommand cmd = new OracleCommand("select * from NMS_USERS where username = '" + uname + "' and password  = '" + pword + "' and VENDOR  = '" + vendor + "'  ", con);

            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            // cmd.Parameters.Add("username_parameter", SqlDbType.VarChar, 100).Value = uname;

            try
            {
                con.Open();
                // SqlDataReader rd = cmd.ExecuteReader();
                System.Data.OracleClient.OracleDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {
                    usernamecandidate = rd["username"].ToString();
                    Name = rd["NAME"].ToString();

                    myStatus = true;
                }
                else
                {
                    //Label6.Visible = true;
                    myStatus = false;
                }

            }
            catch (Exception ex)
            {
                myStatus = false;
                ErrMsg = ex.Message;
                con.Close();
            }
            finally
            {
                con.Close();
            }
            return myStatus;

        }





        /////



        public bool isAutheticate_check_approvals_NPQA(string uname) // Pass the Isauthentication into access for username and password
        {
            Connection mycon = new Connection();
            OracleConnection con = new OracleConnection(mycon.Myconnection);

            // OracleCommand cmd = new OracleCommand("select * from SOCR_USERS where username = '" + uname + "' ", con);
            OracleCommand cmd = new OracleCommand("select * from NMS_MAIN where NPQA_USERNAME = '" + uname + "' ", con);

            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            // cmd.Parameters.Add("username_parameter", SqlDbType.VarChar, 100).Value = uname;

            try
            {
                con.Open();
                // SqlDataReader rd = cmd.ExecuteReader();
                System.Data.OracleClient.OracleDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {

                    myStatus = true;
                }
                else
                {
                    //Label6.Visible = true;
                    myStatus = false;
                }

            }
            catch (Exception ex)
            {
                myStatus = false;
                ErrMsg = ex.Message;
                con.Close();
            }
            finally
            {
                con.Close();
            }
            return myStatus;

        }


        /////////// OSS MANAGER


        public bool isAutheticate_check_approvals_oss_Manager(string uname) // Pass the Isauthentication into access for username and password
        {
            Connection mycon = new Connection();
            OracleConnection con = new OracleConnection(mycon.Myconnection);


            // OracleCommand cmd = new OracleCommand("select * from SOCR_USERS where username = '" + uname + "' ", con);
            OracleCommand cmd = new OracleCommand("select * from NMS_MAIN where OSS_MANAGER_USERNAME = '" + uname + "' ", con);

            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            // cmd.Parameters.Add("username_parameter", SqlDbType.VarChar, 100).Value = uname;

            try
            {
                con.Open();
                // SqlDataReader rd = cmd.ExecuteReader();
                System.Data.OracleClient.OracleDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {

                    myStatus = true;
                }
                else
                {
                    //Label6.Visible = true;
                    myStatus = false;
                }

            }
            catch (Exception ex)
            {
                myStatus = false;
                ErrMsg = ex.Message;
                con.Close();
            }
            finally
            {
                con.Close();
            }
            return myStatus;

        }




        /////////// MANAGER


        public bool isAutheticate_check_approvals_SUPERVISOR_Manager(string uname) // Pass the Isauthentication into access for username and password
        {
            Connection mycon = new Connection();
            OracleConnection con = new OracleConnection(mycon.Myconnection);

            // OracleCommand cmd = new OracleCommand("select * from SOCR_USERS where username = '" + uname + "' ", con);
            OracleCommand cmd = new OracleCommand("select * from NMS_MAIN where APPROVAL_USERNAME = '" + uname + "' ", con);

            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            // cmd.Parameters.Add("username_parameter", SqlDbType.VarChar, 100).Value = uname;

            try
            {
                con.Open();
                // SqlDataReader rd = cmd.ExecuteReader();
                System.Data.OracleClient.OracleDataReader rd = cmd.ExecuteReader();

                if (rd.Read())
                {

                    myStatus = true;
                }
                else
                {
                    //Label6.Visible = true;
                    myStatus = false;
                }

            }
            catch (Exception ex)
            {
                myStatus = false;
                ErrMsg = ex.Message;
                con.Close();
            }
            finally
            {
                con.Close();
            }
            return myStatus;

        }





    }
}