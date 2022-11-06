using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideFormProgram
{
    public partial class MenuForm : Form
    {
        SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=HAIER-PC;Initial Catalog=myproject;Integrated Security=True";
        public MenuForm()
        {
            conn.ConnectionString = connectionString;
            InitializeComponent();
        }


                ///////////////////////represent close sign for hide program       
        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Hide();
        }
        ///////////////////////represent status menu
        private void label2_MouseClick_1(object sender, MouseEventArgs e)
        {
            showStatusMenu();            
        }
        private void showStatusMenu()
        {
            flowLayoutPanel1.Visible = false;
            panel3.Visible = false;
            button5.Visible = false;
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = true;
            pictureBox5.Visible = true;
            pictureBox6.Visible = true;
            pictureBox7.Visible = true;
            label8.Visible = true;
            label9.Visible = true;
            label10.Visible = true;
            label11.Visible = true;
            label12.Visible = true;
            label13.Visible = true;
            button1.Visible = button2.Visible = button3.Visible = button4.Visible = false;
            groupBox1.Visible = false;
            //throw new NotImplementedException();
        }
        private void showScreenshotMenu()
        {
            flowLayoutPanel1.Visible = true;
            panel3.Visible = false;
            button5.Visible = true;
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            button1.Visible = button2.Visible = button3.Visible = button4.Visible = false;
            groupBox1.Visible = false;
            flowLayoutPanel1.Controls.Clear();
            //throw new NotImplementedException();
        }

        private void showSettingMenu()
        {
            
            flowLayoutPanel1.Visible = true;
            panel3.Visible = true;
            panel3.Controls.Clear();
            flowLayoutPanel1.Controls.Clear();
            button5.Visible = false;
            pictureBox2.Visible = false;
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            pictureBox6.Visible = false;
            pictureBox7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            button1.Visible = button2.Visible = button3.Visible = button4.Visible = true;
            groupBox1.Visible = true;
            panel3.Controls.Add(button1);
            panel3.Controls.Add(button2);
            panel3.Controls.Add(button3);
            panel3.Controls.Add(button4);
            panel3.Controls.Add(groupBox1);
            panel3.Controls.Add(groupBox2);
            panel3.Controls.Add(label17);
            panel3.Controls.Add(comboBox1);
            panel3.Controls.Add(groupBox3);
            panel3.Controls.Add(groupBox4);
            button1_Click(this,EventArgs.Empty );
            flowLayoutPanel1.Controls.Add(panel3);
            // throw new NotImplementedException();
        }
        ///////////////////////represent setting menu
        private void label3_MouseClick(object sender, MouseEventArgs e)
        {
            showSettingMenu();
            getAllsetting();
        }
        ///////////////////////represent screen shot menu
        private void label4_MouseClick(object sender, MouseEventArgs e)
        {
            showScreenshotMenu();
            //*****************************************  Display Picture on Form *************************************************//
            try
            {
                conn.Open();
                int count = 0;
                string query = "select count(*) from ImageBackup";
                SqlCommand cmd = new SqlCommand(query, conn);
                count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    string query1 = "SELECT *From ImageBackup ORDER BY daat DESC,time DESC";
                    SqlCommand cmd1 = new SqlCommand(query1, conn);
                    using (SqlDataReader reader = cmd1.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string filename = reader["filename"].ToString();
                            string date = reader["daat"].ToString();
                            string time = reader["time"].ToString();
                            if (File.Exists(filename))
                            {
                                PictureBox pic = new PictureBox();
                                pic.ImageLocation = filename;
                                pic.Size = new System.Drawing.Size(150, 150);
                                pic.Cursor = Cursors.Hand;
                                pic.Padding = new Padding(10, 5, 10, 5);
                                pic.SizeMode = PictureBoxSizeMode.StretchImage;
                                pic.Click += new EventHandler(click_Event);
                                flowLayoutPanel1.Controls.Add(pic);
                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("No Data Found");
                }
                conn.Close();
            }
            catch (Exception ex) { Debug.WriteLine("Error in toolStripLabel5_Click\n" + ex); }
        }
        /////////////////////// open another form to display full image
        private void click_Event(object sender, EventArgs e)
        {
            Image img = ((PictureBox)sender).Image;
            pictureShow s = new pictureShow();
            s.setimage(((PictureBox)sender).Image);
            s.Show();
        }
        private void label1_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(this.label1,"Hide");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox8.SizeMode = PictureBoxSizeMode.Zoom;
            //pictureBox8.Enabled = false;
            //GetBrowsingHistory b = new GetBrowsingHistory();
            //FileSystemWatcherAndUsbDetect detect = new FileSystemWatcherAndUsbDetect();
            // Keylogger key = new Keylogger();
            showStatusMenu();
            setAllSetting();
        }

        ///////////////////////represent sign with close sign to uninstall program
        private void label6_MouseClick(object sender, MouseEventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you Want to uninstall Program.", "Uninstall", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {

                try
                {
                    conn.Open();
                    string query = "delete from LoginForm";
                    string query1 = "delete from FilePath";
                    string query2 = "delete from BROWSERHISTORY";
                    string query3 = "delete from FileWatcher";
                    string query4 = "delete from ImageBackup";
                    string query5 = "delete from KeyTyped";
                    string query6 = "delete from DeviceDetail";
                    string query7 = "delete from Settings";
                    string query8 = "delete from ApplicationRun";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlCommand cmd1 = new SqlCommand(query1, conn);
                    SqlCommand cmd2 = new SqlCommand(query2, conn);
                    SqlCommand cmd3 = new SqlCommand(query3, conn);
                    SqlCommand cmd4 = new SqlCommand(query4, conn);
                    SqlCommand cmd5 = new SqlCommand(query5, conn);
                    SqlCommand cmd6 = new SqlCommand(query6, conn);
                    SqlCommand cmd7 = new SqlCommand(query7,conn);
                    SqlCommand cmd8 = new SqlCommand(query8, conn);
                    int row = cmd.ExecuteNonQuery();
                    int row1 = cmd1.ExecuteNonQuery();
                    int row2 = cmd2.ExecuteNonQuery();
                    int row3 = cmd3.ExecuteNonQuery();
                    int row4 = cmd4.ExecuteNonQuery();
                    int row5 = cmd5.ExecuteNonQuery();
                    int row6 = cmd6.ExecuteNonQuery();
                    int row7 = cmd7.ExecuteNonQuery();
                    int row8 = cmd8.ExecuteNonQuery();
                    conn.Close();
                    if (row4 >0)
                    {
                        string file = Directory.GetCurrentDirectory();
                        file = file + "\\ABC\\";
                        if (Directory.Exists(file))
                        {
                            Directory.Delete(file);
                        }
                    }
                    if (row == 1)
                    {
                        bool check = removeApplicationStartUp.removeOnStartUp();
                        if (check == true)
                        {
                            MessageBox.Show("Successfully Deleted.\n Restart Your System.");
                            Environment.Exit(1);
                        }
                        else
                        {
                            MessageBox.Show("Some error occure. Please Restart your system and try again.");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Error in data Deleting");
                    }
                }
                catch (Exception ex) { Debug.WriteLine("error in unistall program\n" + ex); }
            }
            else if (result == DialogResult.No) { }
        }

        private void label6_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(this.label6, "Uninstall");
        }

        /////////////////////// represent start and stop application monitoring
        private void label7_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                string text = label7.Text;
                if (text == "Start")
                {
                    label7.Text = "Stop";
                    label7.ForeColor = Color.Red;
                    pictureBox8.Enabled = true;
                    ScreenShot.screenShotTimerStoper("ON");
                    Keylogger.KeyloggerstopTimer("ON");
                    GetBrowsingHistory.timerstoper("ON");
                    GetProcessAndPath.timerstoper("ON");
                    FileSystemWatcherAndUsbDetect.timerstoper("ON");
                    ApplicationRun.timerstoper("ON");
                }
                else
                {
                    label7.Text = "Start";
                    label7.ForeColor = Color.Blue;
                    pictureBox8.Enabled = false;
                    ScreenShot.screenShotTimerStoper("OFF");
                    Keylogger.KeyloggerstopTimer("OFF");
                    GetBrowsingHistory.timerstoper("OFF");
                    GetProcessAndPath.timerstoper("OFF");
                    FileSystemWatcherAndUsbDetect.timerstoper("OFF");
                    ApplicationRun.timerstoper("OFF");
                }
            }
            catch(Exception ex) { Debug.WriteLine("Error in label7 of start and stop\n"+ex); }
        }

        private void pictureBox4_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(this.pictureBox4, "Create,Delete,Renamed,Copy/Paste File&Folder");
        }

        private void pictureBox3_MouseHover(object sender, EventArgs e)
        {
            System.Windows.Forms.ToolTip tooltip = new ToolTip();
            tooltip.SetToolTip(this.pictureBox3, "KeyPressed History");
        }

        /////////////////////// for browsing history
        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {
            DisplayForm f = new DisplayForm("Browsing History");
            f.Show();
        }
        /////////////////////// for keypressed history
        private void pictureBox3_MouseClick(object sender, MouseEventArgs e)
        {
            DisplayForm f = new DisplayForm("Keypressed History");
            f.Show();
        }
        /////////////////////// for folder and file(create,delete,renamed,copy/paste) history
        private void pictureBox4_MouseClick(object sender, MouseEventArgs e)
        {
            DisplayForm f = new DisplayForm("File History");
            f.Show();
        }

        /////////////////////// for application used history
        private void pictureBox5_MouseClick(object sender, MouseEventArgs e)
        {
            DisplayForm f = new DisplayForm("Application History");
            f.Show();
        }

        /////////////////////// for cd and usb detect history
        private void pictureBox6_MouseClick(object sender, MouseEventArgs e)
        {
            DisplayForm f = new DisplayForm("External Device");
            f.Show();
        }
        /////////////////////// for Docs/file open  history
        private void pictureBox7_MouseClick(object sender, MouseEventArgs e)
        {
            DisplayForm f = new DisplayForm("File Monitor History");
            f.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = true;
            groupBox2.Visible = false;
            label17.Visible = false;
            comboBox1.Visible = false;
            groupBox3.Visible = false;
            groupBox4.Visible = false;
            button3.BackColor = ColorTranslator.FromHtml("#F0F0F0");
            button2.BackColor = ColorTranslator.FromHtml("#F0F0F0");
            button1.BackColor = Color.Green;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            groupBox2.Visible = true;
            groupBox4.Visible = false;
            label17.Visible = true;
            comboBox1.Visible = true;
            groupBox3.Visible = true;
            button3.BackColor = ColorTranslator.FromHtml("#F0F0F0");
            button2.BackColor = Color.Green;
            button1.BackColor = ColorTranslator.FromHtml("#F0F0F0");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            groupBox2.Visible = false;
            label17.Visible = false;
            comboBox1.Visible = false;
            groupBox3.Visible = false;
            groupBox4.Visible = true;
            button3.BackColor = Color.Green;
            button2.BackColor = ColorTranslator.FromHtml("#F0F0F0");
            button1.BackColor = ColorTranslator.FromHtml("#F0F0F0");
        }
                                                /// <summary>
                                                /// / get value from radiobutton 
                                                /// </summary>
                                                /// <param name="grb"></param>
                                                /// <returns></returns>
        private string getselectedRadioButton(GroupBox grb)
        {
            try {
                return grb.Controls.OfType<RadioButton>().SingleOrDefault(r => r.Checked == true).Text;
            }
            catch (Exception ex) { return null; }
                // throw new NotImplementedException();
        }
        public  string[] getSetting()
        {
            try {
                string[] data = new string[5];
                data[0] = getselectedRadioButton(groupBox1);
                data[1] = getselectedRadioButton(groupBox4);
                data[2] = comboBox1.Text.ToString();
                data[3] = numericUpDown1.Value.ToString();
                data[4] = numericUpDown2.Value.ToString();
                if(data[0]!=null&&data[1]!=null && data[2] != null && data[3] != null && data[4] != null)
                {
                    return data;
                }
            }
            catch (Exception ex) { Debug.WriteLine("error in MenuForm in getSetting\n"+ex);
                return null;
            }
            return null;

        }
        private void button4_Click(object sender, EventArgs e)
        {
            string groupbox1Value = getselectedRadioButton(groupBox1);
            string groupbox2Value = getselectedRadioButton(groupBox4);
            try
            {
                
                if(screenshotSelection!=getselectedRadioButton(groupBox1))
               ScreenShot.screenShotTimerStoper(groupbox1Value);

                if(keytypedSelection!=getselectedRadioButton(groupBox4))
               Keylogger.KeyloggerstopTimer(groupbox2Value);


                if (comboBox1.Text != screenshotTimerInterval)
                {
                    ScreenShot screen = new ScreenShot();
                }

                updateAllSetting();
            }
            catch (Exception ex) { Debug.WriteLine("Error in setting apply button\n"+ex); }

        }

        private void updateAllSetting()
        {
            try
            {
                conn.Open();
                string query = "update Settings set ScreenShotStatus=@sStatus,keyTypedStatus=@kStatus,snapShotInterval=@Interval,DeleteSnapShot=@DSnap,DeleteOtherData=@DDate";
                SqlCommand cmd = new SqlCommand(query,conn);
                cmd.Parameters.AddWithValue("sStatus",getselectedRadioButton(groupBox1));
                cmd.Parameters.AddWithValue("kStatus",getselectedRadioButton(groupBox4));
                cmd.Parameters.AddWithValue("Interval",comboBox1.Text.ToString());
                cmd.Parameters.AddWithValue("DSnap",numericUpDown1.Value.ToString());
                cmd.Parameters.AddWithValue("DDate",numericUpDown2.Value.ToString());
                int row = cmd.ExecuteNonQuery();
                conn.Close();
                if (row == 1)
                    MessageBox.Show("Successfully Updated");
                else
                    MessageBox.Show("Not Updated");
            }
            catch (Exception ex) { Debug.WriteLine("Error in MenuForm in  updateAllSetting\n" + ex); }

           // throw new NotImplementedException();
        }

        private void passwordUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                string oldpassword = old.Text.ToString();
                string newpassword = New.Text.ToString();
                if (string.IsNullOrEmpty(oldpassword) || string.IsNullOrEmpty(newpassword))
                {
                    MessageBox.Show("Fill the field.");
                }
                else
                {
                    conn.Open();
                    string query = "update LoginForm set password=@pass where password=@newpass";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@pass", newpassword);
                    cmd.Parameters.AddWithValue("@newpass", oldpassword);
                    int row = cmd.ExecuteNonQuery();
                    if (row == 1)
                        MessageBox.Show("Successfully updated");
                    else
                        MessageBox.Show("not updated");

                    conn.Close();
                }

            }
            catch (Exception ex) { Debug.WriteLine("error in password updater\n"+ex); }
        }


        /// <summary>
        /// get All setting when user click on setting tab it is easir for comporison
        /// </summary>
        public string screenshotSelection;
        public string keytypedSelection;
        public string screenshotTimerInterval;
        public decimal deletesnapshot;
        public decimal deleteOtherData;
        public void getAllsetting()
        {
            screenshotSelection = getselectedRadioButton(groupBox1);
            keytypedSelection = getselectedRadioButton(groupBox4);
            screenshotSelection = comboBox1.Text.ToString();
            deletesnapshot = numericUpDown1.Value;
            deleteOtherData = numericUpDown2.Value;
        }
            /// <summary>
            /// get all setting from database and insert into setting menu...
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
       public void setAllSetting()
        {
            string[] data = new string[5];
            try
            {
                conn.Open();
                string query = "select *from Settings";
                SqlCommand cmd = new SqlCommand(query,conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    data[0] = reader.GetString(0);
                    data[1] = reader.GetString(1);
                    data[2] = reader.GetString(2);
                    data[3] = reader.GetString(3);
                    data[4] = reader.GetString(4);
                }
                reader.Close();
                conn.Close();
                if (data[0] != null && data[1] != null && data[2] != null && data[3] != null && data[4] != null)
                {
                    if (data[0] == "ON")
                    {
                        radioButton5.Checked = true;
                        radioButton6.Checked = false;
                    }
                    else
                    {
                        radioButton5.Checked = false;
                        radioButton6.Checked = true;
                    }
                    if (data[1] == "ON")
                    {
                        radioButton3.Checked = true;
                        radioButton4.Checked = false;
                    }
                    else
                    {
                        radioButton3.Checked = false;
                        radioButton4.Checked = true;
                    }
                    comboBox1.Text = data[2];
                    numericUpDown1.Value = Convert.ToDecimal(data[3]);
                    numericUpDown2.Value = Convert.ToDecimal(data[4]);
                }
            }
            catch (Exception ex) { Debug.WriteLine("Error in MenuForm in setAllSetting\n"+ex); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string query = "delete from ImageBackup";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                int row = cmd.ExecuteNonQuery();
                if (row == 1)
                {
                    string file = Directory.GetCurrentDirectory();
                    file = file + "\\ABC\\";
                    if (Directory.Exists(file))
                    {
                        Directory.Delete(file);
                    }
                }
                conn.Close();
            }
            catch (Exception ex ) { Debug.WriteLine("Error MenuForm in  button5_Click\n" + ex); }
        }

    }
}
