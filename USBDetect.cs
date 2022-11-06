using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Windows;
using System.Data.SqlClient;
using System.Diagnostics;

namespace HideFormProgram
{
    class USBDetect
    {
        public event EventArrivedEventHandler cdAndUsbChnaged;
       public static SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=HAIER-PC;Initial Catalog=myproject;Integrated Security=True";
        public USBDetect()
        {
            conn.ConnectionString = connectionString;
            //     background_Work();
            DriveInfo[] drive = DriveInfo.GetDrives();
            foreach (DriveInfo d in drive)
            {
                if (d.DriveType == DriveType.Removable)
                {
                  /*  Console.WriteLine("Disk is aleardy Inserted");
                    Console.WriteLine("Name: " + d.Name);
                    Console.WriteLine("Driver Format: " + d.VolumeLabel);
                    Console.WriteLine("Total Size: " + (d.TotalSize / 1024) / 1024 / 1024 + "GB");*/
                    saveInDatabase("USB", d.VolumeLabel, d.Name, "Aleardy Inserted");
                    //detect();
                }
                else
                {
                    //detect();
                }
            }
        //    detect();
          

        }

        
        public void detect()
        {
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2");
            watcher.EventArrived += new EventArrivedEventHandler(insert_EventArrived);
            watcher.Query = query;
            watcher.Start();

            ManagementEventWatcher remove = new ManagementEventWatcher();
            WqlEventQuery q = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 3");
            remove.EventArrived += new EventArrivedEventHandler(remove_EventArrived);
            remove.Query = q;
            remove.Start();

            //throw new NotImplementedException();
        }
        
        private void remove_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string name = e.NewEvent.Properties["DriveName"].Value.ToString();
            //Console.WriteLine("Disk is Removed ="+name);
            saveInDatabase("USB", "", name, "Removed");
            cdAndUsbChnaged(this,e);
            //throw new NotImplementedException();
        }

        private void insert_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string name = e.NewEvent.Properties["DriveName"].Value.ToString();
           // Console.WriteLine("Disk is inserted");
            DriveInfo drive = new DriveInfo(name);
           // Console.WriteLine("Name: "+drive.Name);
           // Console.WriteLine("Driver Format: " + drive.VolumeLabel);
           // Console.WriteLine("Total Size: " +( drive.TotalSize/1024)/1024/1024+"GB");
            saveInDatabase("USB",drive.VolumeLabel,drive.Name,"Inserted");
            cdAndUsbChnaged(this, e);
            //throw new NotImplementedException();
        }
        public static void saveInDatabase(string type,string name,string dname,string state)
        {
            try {
                conn.Open();
                string query = "insert into DeviceDetail(DeviceType,DeviceName,DriveName,DeviceState,Daat,time) values(@type,@name,@dname,@state,@date,@time)";
                SqlCommand cmd = new SqlCommand(query,conn);
                cmd.Parameters.AddWithValue("@type",type);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@dname", dname);
                cmd.Parameters.AddWithValue("@state", state);
                cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("d"));
                cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("HH:mm:ss"));
                int row = cmd.ExecuteNonQuery();
                conn.Close();
                if (row == 1)
                    Debug.WriteLine("usb and cd Data Inserted");
                else
                    Debug.WriteLine("Insertion probelm in cd and usb");
            } catch(Exception ex) { Debug.WriteLine("Error in usb and cd data insert\n"+ex); }

        }
    }

}
