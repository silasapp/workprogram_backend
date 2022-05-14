using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Backend_UMR_Work_Program
{
    public class Error_logs
    {

        public  void WriteLog(string msg)

        {

            if ((msg.ToString().Contains("Thread was being aborted") || msg.ToString().Contains("An error has occurred while establishing a connection to the server")))

            {

                // 'do nothing

            }

            else

            {

                try

                {

                    HttpContext context = HttpContext.Current;

                    string _path;

                    _path = "App_Logs\\ErrorLog.txt";

                    string path = context.Server.MapPath("") + "../\\" + _path;

                    System.IO.StreamWriter writer = new System.IO.StreamWriter(path, true);

                    // writer.WriteLine(msg + " " + New FINSMS_BLL().NigerianDate() & " " & New FINSMS_BLL().NigerianTime())

                    // writer.WriteLine((((msg + (" " + " ")) + new SSC_BLL()) + " @ " + NigerianTime()));

                    writer.WriteLine(msg);

                    writer.Close();

                }

                catch (Exception ex)

                {
                    Console.WriteLine(ex.Message);
                }

            }


        }

    }

}