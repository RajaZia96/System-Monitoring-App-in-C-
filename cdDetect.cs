using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace HideFormProgram
{
    class cdDetect
    {
        public event EventArrivedEventHandler cdAndUsbChnaged;
        // ONLY read cd name and serial numebr not not alert the program when cd enter............................
        public void getcd()
        {

           SelectQuery query = new SelectQuery("select * from win32_logicaldisk where drivetype=5");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            foreach (ManagementObject mo in searcher.Get())
            {
                if ((mo["volumename"] != null) || (mo["volumeserialnumber"] != null))
                {
                   // Console.WriteLine("Driver Name:{0}", mo["DeviceID"]);
                    //Console.WriteLine("CD named:{0}", mo["volumename"]);
                }
                else
                {
                   // Console.WriteLine("No Disk Found");
                }
            }
            detectCD();
            //throw new NotImplementedException();
        }

        //                  Detect cd when cd is inserted                  //////////

        public void detectCD()
        {
            try
            {
                WqlEventQuery q = new WqlEventQuery();
                q.EventClassName = "__InstanceModificationEvent";
                q.WithinInterval = new TimeSpan(0,0,1);
                q.Condition = @"TargetInstance ISA 'Win32_LogicalDisk' and TargetInstance.DriveType = 5";

                ConnectionOptions opt = new ConnectionOptions();
                opt.EnablePrivileges = true;
                opt.Authority = null;
                opt.Authentication = AuthenticationLevel.Default;

                ManagementScope scope = new ManagementScope("\\root\\CIMV2", opt);
                ManagementEventWatcher watcher = new ManagementEventWatcher(scope,q);
                watcher.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
                watcher.Start();


            }
            catch (ManagementException ex) {
                Console.WriteLine(ex.Message);
            }

        }

        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject obj = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            string drivename = (string)obj["DeviceID"];
            if (obj.Properties["VolumeName"].Value != null)
            {
                // Console.WriteLine("Cd is Inserted");
                //  Console.WriteLine("Drive Name:" + drivename);
                //  Console.WriteLine("CD Name: "+obj.Properties["VolumeName"].Value);
                USBDetect.saveInDatabase("CD", drivename, (string)obj.Properties["VolumeName"].Value, "Inserted");
                cdAndUsbChnaged(this, e);
            }
            else
            {
               // Console.WriteLine("Cd is ejected");
                USBDetect.saveInDatabase("CD", drivename, (string)obj.Properties["VolumeName"].Value, "Ejected");
                cdAndUsbChnaged(this, e);
            }
            // throw new NotImplementedException();
        }
    }
}




// url///
/////////https://stackoverflow.com/questions/1662794/detecting-eject-insert-of-removeable-media