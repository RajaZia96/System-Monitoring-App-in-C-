using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace HideFormProgram
{
    class ApplicationRun
    {
        public static Timer time;
        List<string> taskManagerProcess;
        SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=HAIER-PC;Initial Catalog=myproject;Integrated Security=True";
        public ApplicationRun()
        {
            conn.ConnectionString = connectionString;
            taskManagerProcess = new List<string>();
            time = new Timer();
            time.Interval = 1000;
            time.Elapsed += new ElapsedEventHandler(timer_Start);
            time.Start();
        }

        private void timer_Start(object sender, ElapsedEventArgs e)
        {
            try
            {
                Process[] process = Process.GetProcesses();
                foreach (Process p in process)
                {
                    if (p.MainWindowTitle.Length > 0)
                    {
                        string x = Path.GetFileNameWithoutExtension(p.ProcessName);
                        if (!taskManagerProcess.Contains(x) && !x.Contains("devenv") && !x.Contains("eventDetection"))
                        {
                            //Console.WriteLine("process start   " + x);
                            //Console.WriteLine("p main window title  " + p.MainWindowTitle);
                            bool m = sendDataToDatabase(x, p.MainWindowTitle);
                            if (m == true)
                            {
                                time.Start();
                            }

                            taskManagerProcess.Add(x);
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.WriteLine("Error in ApplicationRun in timer_Start\n" + ex); }

            // throw new NotImplementedException();
        }

        private bool sendDataToDatabase(string x, string mainWindowTitle)
        {
            try
            {
                time.Stop();
                conn.Open();
                string query = "insert into ApplicationRun(Application,WindowTitle,Daat,Time) values(@app,@title,@date,@time)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@app", x);
                cmd.Parameters.AddWithValue("@title", mainWindowTitle);
                cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("d"));
                cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("HH:mm:ss"));
                int row = cmd.ExecuteNonQuery();
                conn.Close();
                if (row == 1)
                {
                    Console.WriteLine("Application is inserted Inserted");
                    return true;
                }
                else
                    Console.WriteLine("Insertion probelm in ApplicationRun");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in ApplicationRun in sendDataToDatabase\n" + ex);
                return false;
            }

            return false;

            //throw new NotImplementedException();
        }
        public static void timerstoper(string s)
        {
            try
            {
                if (s == "ON")
                {
                    time.Start();
                }
                if (s == "OFF")
                    time.Stop();
            }
            catch (Exception ex) { Debug.WriteLine("Error in ApplicationRun in timerstop\n" + ex); }
        }
    }
}
