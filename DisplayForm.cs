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
    public partial class DisplayForm : Form
    {
        public string Click_text;
        public string Selectquery;
        SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=HAIER-PC;Initial Catalog=myproject;Integrated Security=True";
        public DisplayForm(string s)
        {
            conn.ConnectionString = connectionString;
            Click_text = s;
            InitializeComponent();
        }

        private void DisplayForm_Load(object sender, EventArgs e)
        {
            string query = null;
            this.Text = Click_text;
            switch (Click_text)
            {
                case "Browsing History":
                    query = "SELECT *FROM BROWSERHISTORY ORDER BY Daat DESC,TIME DESC";
                    displayData(query);
                    break;
                case "Keypressed History":
                     query = "SELECT *FROM KeyTyped ORDER BY Daat DESC,time DESC";
                    displayData(query);
                    break;
                case "File History":
                    query = "SELECT *FROM FileWatcher ORDER BY daat DESC,time DESC";
                    displayData(query);
                    break;
                case "Application History":
                    query = "SELECT *FROM ApplicationRun ORDER BY Daat DESC,Time DESC";
                    displayData(query);
                    break;
                case "External Device":
                    query = "SELECT *FROM DeviceDetail ORDER BY Daat DESC,time DESC";
                    displayData(query);
                    break;
                case "File Monitor History":
                    query = "SELECT *FROM FilePath ORDER BY Daat DESC,TIME DESC";
                    displayData(query);
                    break;

            }
        }
        public void displayData(string q)
        {
            Selectquery = q;
            DataTable table = new DataTable();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(q,conn);
                SqlDataAdapter ad = new SqlDataAdapter(cmd);
                ad.Fill(table);
                table.Columns["Daat"].ColumnName = "Date";
                table.Columns["TIME"].ColumnName = "Time";
                if (table.Columns[1].ColumnName == "DATA")
                    table.Columns["DATA"].ColumnName = "Path";
                if (table.Columns[0].ColumnName == "BROWSERTYPE")
                    table.Columns["BROWSERTYPE"].ColumnName = "Browser";
                dataGridView1.DataSource = new BindingSource(table, null);
                conn.Close();
            }
            catch (Exception ex) { Debug.WriteLine("Error DisplayForm in DisplayData \n" + ex); }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query=null;
            try
            {
                switch (Click_text)
                {
                    case "Browsing History":
                        query = "delete from BROWSERHISTORY";
                        deletData(query);
                        break;
                    case "Keypressed History":
                        query = "delete from KeyTyped";
                        deletData(query);
                        break;
                    case "File History":
                        query = "delete from FileWatcher";
                        deletData(query);
                        break;
                    case "Application History":
                        query = "delete from ApplicationRun";
                        deletData(query);
                        break;
                    case "External Device":
                        query = "delete from DeviceDetail";
                        deletData(query);
                        break;
                    case "File Monitor History":
                        query = "delete from FilePath";
                        deletData(query);
                        break;

                }
            }
            catch (Exception ex) { Debug.WriteLine("Error DisplayForm in button1_Click \n" + ex); }
        }

        private void deletData(string query)
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query,conn);
                int row = cmd.ExecuteNonQuery();
                conn.Close();
                // MessageBox.Show(""+row);
                if (row > 0)
                {
                    MessageBox.Show("Successfully Deleted");
                    if (!string.IsNullOrEmpty(Selectquery))
                    {
                        dataGridView1.Controls.Clear();
                        displayData(Selectquery);
                    }
                }
                else
                    MessageBox.Show("Problem occure");
                

                
            }
            catch (Exception ex) { Debug.WriteLine("Error DisplayForm in deletData \n" + ex); }
        }
    }
}
