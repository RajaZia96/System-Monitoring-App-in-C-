using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideFormProgram
{
    class ImplementSetting
    {
        SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=HAIER-PC;Initial Catalog=myproject;Integrated Security=True";
        public ImplementSetting()
        {
            string Date = null;
            string time = null;
            string snapShot = null;
            string otherData = null;
            bool update = false;
            try
            {
                conn.Open();
                string query1 = "SELECT *From Settings";
                SqlCommand cmd1 = new SqlCommand(query1, conn);
                SqlDataReader reader = cmd1.ExecuteReader();
                while (reader.Read())
                {
                    snapShot = reader["DeleteSnapShot"].ToString();
                    otherData = reader["DeleteOtherData"].ToString();
                    Date = reader["Daat"].ToString();
                    time = reader["Time"].ToString();
                }
                reader.Close();
                if(!string.IsNullOrEmpty(snapShot)&& !string.IsNullOrEmpty(otherData) && !string.IsNullOrEmpty(Date) && !string.IsNullOrEmpty(time))
                {
                    DateTime oldDate = Convert.ToDateTime(Date+" "+time);
                    DateTime todayDate = DateTime.Now;
                    double days = (todayDate - oldDate).TotalDays;
                    int day = (int)days;

                    int snap = Convert.ToInt32(snapShot);
                    int other = Convert.ToInt32(otherData);

                    if (day >= snap)
                    {
                        string query4 = "delete from ImageBackup";
                        SqlCommand cmd = new SqlCommand(query4,conn);
                        int row = cmd.ExecuteNonQuery();
                        if (row >= 1)
                        {
                            string file = Directory.GetCurrentDirectory();
                            file = file + "\\ABC\\";
                            if (Directory.Exists(file))
                            {
                                Directory.Delete(file);
                            }
                            update = true;
                            Debug.WriteLine("SnapShot Data Delete After 2 days");
                        }
                        else
                            Debug.WriteLine("Error in SnapShot Data Deletation");
                    }

                    if (day >= other)
                    {
                        string query = "delete from FilePath";
                        string query2 = "delete from BROWSERHISTORY";
                        string query3 = "delete from FileWatcher";
                        string query5 = "delete from KeyTyped";
                        string query6 = "delete from DeviceDetail";
                        string query8 = "delete from ApplicationRun";
                        SqlCommand cmd = new SqlCommand(query,conn);
                        SqlCommand cmd2 = new SqlCommand(query2,conn);
                        SqlCommand cmd3 = new SqlCommand(query3, conn);
                        SqlCommand cmd5 = new SqlCommand(query5, conn);
                        SqlCommand cmd6 = new SqlCommand(query6, conn);
                        SqlCommand cmd8 = new SqlCommand(query8, conn);
                        int row = cmd.ExecuteNonQuery();
                        int row2 = cmd2.ExecuteNonQuery();
                        int row3 = cmd3.ExecuteNonQuery();
                        int row5 = cmd5.ExecuteNonQuery();
                        int row6 = cmd6.ExecuteNonQuery();
                        int row8 = cmd8.ExecuteNonQuery();
                       if(row>=1||row2>=1|| row3 >= 1 || row5 >= 1|| row6 >= 1 || row8 >= 1)
                        {
                            update = true;
                            Debug.WriteLine("Other Data Delete After 2 days");
                        }
                        else
                        {
                            Debug.WriteLine("Error in Other Data Deletation");
                        }


                    }

                }
                if (update == true)
                {
                    string query = "update Settings set date=@dat,time=@tim";
                    SqlCommand cmd = new SqlCommand(query,conn);
                    cmd.Parameters.AddWithValue("@dat",DateTime.Now.ToString("d"));
                    cmd.Parameters.AddWithValue("@tim", DateTime.Now.ToString("HH:mm:ss"));
                    int row = cmd.ExecuteNonQuery();
                    if (row == 1)
                    {
                        Debug.WriteLine("Successfully Date And time is Updated after Delete");
                    }
                    else
                    {
                        Debug.WriteLine("Error in Date And time Deletation");

                    }
                }

                conn.Close();
            }
            catch (Exception ex) { Debug.WriteLine("Error in ImplementSetting \n"+ex); }

        }
    }
}
