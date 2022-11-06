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
using Utilities;

namespace HideFormProgram
{
    public partial class idandpasswordForm : Form
    {
        SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=KHAN;Initial Catalog=myproject;Integrated Security=True";
        globalKeyboardHook ghk;
        public static bool flag = false;
        public idandpasswordForm()
        {
            conn.ConnectionString = connectionString;
            ghk = new globalKeyboardHook();                        // call Global hook Keyboard
            InitializeComponent();
        }

        private void idandpasswordForm_Load(object sender, EventArgs e)
        {
            bool ifregister = checkApplicationStartUp.IsInStartup();            // first check is program is in registery..
            if (ifregister == true)
            {
                // MessageBox.Show("Program is aleardy installed");
                this.Visible = false;
                this.ShowInTaskbar = false;
                ImplementSetting setting = new ImplementSetting();
                ghk.KeyDown += new KeyEventHandler(ghk_KeyDown);
                Parallel.Invoke(() =>
                {

                   //  GetBrowsingHistory history = new GetBrowsingHistory();
                },
                () =>
                {

                    //  ScreenShot screen = new ScreenShot();
                },
                () =>
                {
                   // FileSystemWatcherAndUsbDetect file = new FileSystemWatcherAndUsbDetect();
                },
                () =>
                {
                  //  GetProcessAndPath p = new GetProcessAndPath(); 
                },
                () =>
                {
                  //  Keylogger key = new Keylogger();
                },
                () =>
                {
                    //ApplicationRun run = new ApplicationRun();
                }
                );


            }
            else
            {
                DialogResult result = MessageBox.Show("Program is not installed.\nDo you want to install it.", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    GetIDAndPassword idAndPassword = new GetIDAndPassword();
                    idAndPassword.Visible = true;
                }
                else if (result == DialogResult.No)
                {
                    Environment.Exit(0);
                }
            }
            this.Visible = false;                   // hide form when program startup
            this.ShowInTaskbar = false;
        }
        private void ghk_KeyDown(object sender, KeyEventArgs e)
        {

            if (flag)
            {
                this.Hide();
                //this.Visible = false;
                //this.ShowInTaskbar = false;
                flag = false;
            }
            else
            {
                // this.Show();
                this.Visible = true;
                this.ShowInTaskbar = true;
                flag = true;
            }
            e.Handled = true;
            //throw new NotImplementedException();
        }

        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            this.Hide();
            flag = false;
        }

        private void label3_MouseClick(object sender, MouseEventArgs e)
        {
            string username = textBox1.Text.ToString().Trim();
            string passwrod = textBox2.Text.ToString().Trim();
            textBox1.Text = "";
            textBox2.Text = "";
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please Enter correct user name");
            }
            if (string.IsNullOrEmpty(passwrod))
            {
                MessageBox.Show("Please Enter correct Password.");
            }
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(passwrod))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT *FROM LoginForm where userName=@user and password=@pass";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", passwrod);
                    object row = cmd.ExecuteScalar();
                    conn.Close();
                    if (row != null)
                    {
                        MenuForm a = new MenuForm();
                        a.Visible = true;
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("No Record Exists");
                    }
                }
                catch (Exception ex) { Debug.WriteLine("Error\n" + ex); }
            }

        }

        private void idandpasswordForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ghk.unhook();
        }
    }
}
