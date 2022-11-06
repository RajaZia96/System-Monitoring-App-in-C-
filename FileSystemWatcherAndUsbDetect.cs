using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace HideFormProgram
{
    class FileSystemWatcherAndUsbDetect
    {
        List<string> ClipBoardFilePath;
        List<string> watcherPath;
        FileSystemWatcher watcher;
       public static System.Timers.Timer time1;
        int counter = 0;
        int matchCounter = 0;
        ClipboardMonitor monitor;
        USBDetect usb;
        cdDetect cd;
        SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=HAIER-PC;Initial Catalog=myproject;Integrated Security=True";
        int tick = 0;

        public FileSystemWatcherAndUsbDetect()
        {
            monitor = new ClipboardMonitor();
            usb = new USBDetect();
            cd = new cdDetect();
            monitor.ClipboardChanged += new EventHandler<ClipboardChangedEventArgs>(this.ClipBoardChange);
            usb.cdAndUsbChnaged += new System.Management.EventArrivedEventHandler(CdandUSBChange);
            cd.cdAndUsbChnaged += new System.Management.EventArrivedEventHandler(CdandUSBChange);
            conn.ConnectionString = connectionString;
            Parallel.Invoke(() =>
            {
               // Console.WriteLine("Usb Detection Run");
                cd.getcd();
            },
           () =>
           {
              // Console.WriteLine("Usb Detection Run");
               usb.detect();
           },
           () =>
           {
              // Console.WriteLine("start program run");
               startProgram();
           }
          );
        }

        public static void timerstoper(string s)
        {
            try
            {
                if (s == "ON")
                {
                    time1.Start();
                }
                if (s == "OFF")
                    time1.Stop();
            }
            catch (Exception ex) { Debug.WriteLine("Error in GetBrowsingHistory in timerstop\n" + ex); }
        }
        private void CdandUSBChange(object sender, EventArrivedEventArgs e)
        {
            conn.Close();
            watcher.Dispose();
            startProgram();
            //throw new NotImplementedException();
        }

        private void ClipBoardChange(object sender, ClipboardChangedEventArgs e)
        {
            conn.Close();
            watcher.Dispose();
            startProgram();
            // throw new NotImplementedException();
        }

        private void startProgram()
        {
            conn.Open();
            GetClipBoardData();
            DriveInfo[] driver = DriveInfo.GetDrives();
             foreach (DriveInfo d  in driver)
             {
                 if (d.IsReady == true && !Convert.ToString(d).Contains("C:\\"))
                 {
                     fileWatcher(Convert.ToString(d));
                 }
             }
           // fileWatcher("F:\\");
            time1 = new System.Timers.Timer();
            time1.Elapsed += new ElapsedEventHandler(timer_run);
            time1.Interval = 1000;
            time1.Start();
            // throw new NotImplementedException();
        }

        private void timer_run(object sender, ElapsedEventArgs e)
        {
           // Console.WriteLine(++tick);
            if (counter > 0)
            {
                if (watcherPath.Count > 0 && ClipBoardFilePath.Count > 0)
                {

                    time1.Stop();
                    if (watcherPath.Count == ClipBoardFilePath.Count)
                    {

                        for (int k = 0; k < ClipBoardFilePath.Count; k++)
                        {
                            foreach (string s in watcherPath)
                            {
                                if (Path.GetFileName(ClipBoardFilePath[k]) == Path.GetFileName(s))
                                {
                                    matchCounter++;
                                }
                            }
                        }
                        // Console.WriteLine("Length is equal "+matchCounter);
                        if (matchCounter == ClipBoardFilePath.Count)
                        {
                            bool check = SaveDataInDatabase();
                            if (check == true)
                            {
                                time1.Start();
                                matchCounter = 0;
                                counter = 0;
                            }
                        }
                        else
                        {
                            NewCreatedFile(watcherPath);
                            watcherPath.Clear();
                            counter = 0;
                        }
                    }
                }
                else
                {
                    // Console.WriteLine("File");
                    NewCreatedFile(watcherPath);
                    watcherPath.Clear();
                    counter = 0;
                }

            }
            //throw new NotImplementedException();
        }

        private void NewCreatedFile(List<string> watcherPath)
        {
            try
            {
                foreach (string s in watcherPath)
                {
                  //  Console.WriteLine("File Created " + s);
                    SaveDataInDataBase("Created", s);
                }
            }
            catch { }
            time1.Start();
            // throw new NotImplementedException();
        }
        private bool SaveDataInDatabase()
        {
            int index = 0;
            foreach (string s in ClipBoardFilePath)
            {
               // Console.WriteLine("File Moved from " + s + "  to  " + watcherPath[index]);
                SaveDataInDataBase("Moved", "File Moved from " + s + "  to  " + watcherPath[index]);
                index++;
            }
            if (index == ClipBoardFilePath.Count)
            {
                // Console.WriteLine("Run");
                watcherPath.Clear();
                return true;
            }
            return false;
            //throw new NotImplementedException();
        }
        private void GetClipBoardData()
        {
            try
            {
                ClipBoardFilePath = new List<string>();
                IDataObject data = Clipboard.GetDataObject();
                if (data != null)
                {
                    string[] format = data.GetFormats();

                    if (format.Length >= 9)
                    {
                        int Index = Array.FindIndex(format, m => m == "FileDrop");
                        object text = data.GetData(format[Index]);
                        if (text is string[])
                        {
                            string[] a = (string[])text;
                            // ClipBoardFilePath = new string[a.Length];
                            foreach (string s in a)
                            {
                                ClipBoardFilePath.Add(s);
                                FileAttributes attr = File.GetAttributes(s);
                                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                                {
                                    string[] len = Directory.GetFiles(s, "*.*", SearchOption.AllDirectories);
                                    ClipBoardFilePath.AddRange(len);
                                    var len1 = Directory.GetDirectories(s, "*.*", SearchOption.AllDirectories);
                                    ClipBoardFilePath.AddRange(len1);

                                }
                            }
                        }

                    }
                }

               /* if (ClipBoardFilePath.Count != 0)
                {
                    // Console.WriteLine("Total match "+totalFileCount);

                    for (int j = 0; j < ClipBoardFilePath.Count; j++)
                    {
                        Console.WriteLine(ClipBoardFilePath[j] + "    ");
                    }
                }
                else
                {
                    Console.WriteLine("Empty ");
                }*/
            }
            catch { }
            // throw new NotImplementedException();
        }

        private void fileWatcher(string v)
        {
            watcherPath = new List<string>();
            if (Directory.Exists(v))
            {
                try
                {
                    watcher = new FileSystemWatcher();
                    watcher.Path = v;
                    watcher.Filter = "*.*";
                    watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                    watcher.IncludeSubdirectories = true;
                    watcher.EnableRaisingEvents = true;
                    // watcher.Changed += new FileSystemEventHandler(onChanged);
                    watcher.Created += new FileSystemEventHandler(check_event);
                    watcher.Renamed += new RenamedEventHandler(file_Renamed);
                    watcher.Deleted += new FileSystemEventHandler(deleted_File);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error:" + ex);
                }
            }
            //throw new NotImplementedException();
        }
        private void deleted_File(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.Contains("$RECYCLE.BIN") && !e.FullPath.Contains("~"))
            {
               // Console.WriteLine("File: " + e.FullPath + "  " + e.ChangeType);
                SaveDataInDataBase(e.ChangeType + "", e.FullPath + "");
            }
            //throw new NotImplementedException();
        }

        private void file_Renamed(object sender, RenamedEventArgs e)
        {
            if (!e.FullPath.Contains("$RECYCLE.BIN") && !e.FullPath.Contains("~") && !e.OldFullPath.Contains("~"))
            {
               // Console.WriteLine("File Renamed from " + e.OldFullPath + " to " + e.FullPath);
                string d = "File Renamed from " + e.OldFullPath + " to " + e.FullPath;
                SaveDataInDataBase("Renamed", d);
            }
            //throw new NotImplementedException();
        }
        string previousCreated = null;
        private void check_event(object sender, FileSystemEventArgs e)
        {
            if (!e.FullPath.Contains("$RECYCLE.BIN") && !e.FullPath.Contains("~"))
            {
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    if (previousCreated == null)
                    {
                        previousCreated = e.FullPath;
                    }
                    else if (previousCreated != e.FullPath)
                    { previousCreated = e.FullPath; }
                    else { return; }
                    counter++;
                    watcherPath.Add(e.FullPath);
                    // Console.WriteLine("File: " + e.FullPath + "  " + e.ChangeType);
                }
                // throw new NotImplementedException();
            }
        }

        public void SaveDataInDataBase(string action, string data)
        {
            try
            {
                
                    string query = "insert into FileWatcher(Action,DATA,daat,time) values(@action,@data,@d,@time)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@action", action);
                    cmd.Parameters.AddWithValue("@data", data);
                    cmd.Parameters.AddWithValue("@d", DateTime.Now.ToString("d"));
                    cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("HH:mm:ss"));
                    int row = cmd.ExecuteNonQuery();
                    if (row == 1)
                        Debug.WriteLine("Insert Successfully");
                    else
                        Debug.WriteLine("Not insert");
                
            }
            catch (SqlException ex) { Console.WriteLine("error in FileSystemWatcehrAndUsbDetect in SaveDataInDataBase \n" + ex); }
        }
    }
}
