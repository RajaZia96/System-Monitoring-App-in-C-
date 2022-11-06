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
namespace HideFormProgram
{
    class GetProcessAndPath
    {
        SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=HAIER-PC;Initial Catalog=myproject;Integrated Security=True";
        public static Timer timer1;
        public static Dictionary<int, string> array;
        public static Dictionary<int, bool> ifFound;
        int tick_tick = 0;
        Handle handle;
        public GetProcessAndPath()
        {
            conn.ConnectionString = connectionString;
            handle = new Handle();
            array = new Dictionary<int, string>();
            ifFound = new Dictionary<int, bool>();
            timer1 = new Timer();
            timer1.Elapsed += new ElapsedEventHandler(start_timer);
            timer1.Interval = 8000;
            timer1.Start();
            Debug.WriteLine("Timer start");
            //  Console.WriteLine("Press  q to stop timer:");
           // while (Console.Read() != 'q') ;
        }

        public static void timerstoper(string s)
        {
            try
            {
                if (s == "ON")
                {
                    timer1.Start();
                }
                if (s == "OFF")
                    timer1.Stop();
            }
            catch (Exception ex) { Debug.WriteLine("Error in GetBrowsingHistory in timerstop\n" + ex); }
        }

        private void start_timer(object sender, ElapsedEventArgs e)
        {
            Process[] process = Process.GetProcesses();
            foreach (Process p in process)
            {
                if (p.MainWindowTitle.Length > 0)
                {
                    if (!array.ContainsKey(p.Id))
                    {
                        array[p.Id] = "";                       // store the new  Process ID
                        ifFound[p.Id] = false;
                    }
                    if (p.ProcessName == "EXCEL")
                    {
                        string title = p.MainWindowTitle.ToString();
                        string splitExtension = title.Split(new string[] { " - " }, StringSplitOptions.None)[1].Trim();
                        string modifiedName = "\"" + splitExtension + "\"";
                        if (array[p.Id] != modifiedName || ifFound[p.Id] == false)
                        {
                            string path = handle.getPathThroughHandle(modifiedName);
                            if (path != "Not")
                            {
                                ifFound[p.Id] = true;
                                array[p.Id] = modifiedName;
                              //  Console.WriteLine("Path Find : " + path);
                                SaveFile(modifiedName, path);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        //   Console.WriteLine(splitExtension);
                    }

                    if (p.ProcessName == "MSACCESS" || p.ProcessName == "notepad" || p.ProcessName == "WINWORD" || p.ProcessName == "WinRAR" || p.ProcessName == "vlc" || p.ProcessName == "POWERPNT" || p.ProcessName == "AcroRd32" || p.ProcessName == "notepad++" || p.ProcessName == "mspaint")
                    {
                        int processID = p.Id;
                        string title = p.MainWindowTitle.ToString();
                        // Console.WriteLine(p.MainWindowTitle);
                        // string extension = Path.GetExtension(title);
                        //string[] extensionSplit = extension.Split(' ');
                        //string filename = Path.GetFileNameWithoutExtension(title);
                        string FileName = title.Split(new string[] { " - " }, StringSplitOptions.None)[0];


                        if (p.ProcessName == "MSACCESS")
                        {
                            FileName = title.Split(new string[] { " - " }, StringSplitOptions.None)[1];
                            FileName = FileName.Split(':')[0];
                            //Console.WriteLine(FileName);
                        }

                        if (array[processID] == "")                     // check if process is new 
                        {
                            array[processID] = FileName;
                            string path = GetFilePath(processID).Trim('"');
                            if (!string.IsNullOrEmpty(path))


                            {
                                ifFound[processID] = true;
                                //Console.WriteLine("Send path to store: " + path);
                                SaveFile(FileName, path);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (array[processID] != FileName || ifFound[processID] == false)                  // check if user open another file  inside app
                        {
                            array[processID] = FileName;
                            string modifiedFileName = "\"" + FileName + "\"";
                            string path = handle.getPathThroughHandle(modifiedFileName);
                            if (path != "Not")
                            {
                                ifFound[processID] = true;
                               // Console.WriteLine("Path Find : " + path);
                                SaveFile(FileName, path);
                            }
                            else
                            {
                                continue;
                            }
                        }


                        if (ifFound[processID] == false)
                        {
                            SaveFile(FileName, "Not Found");
                            /* string[] pathstring = new string[10];
                             checkAllDirectory check = new checkAllDirectory();
                             pathstring = check.getDrive(FileName);
                             if (pathstring != null)
                             {
                                 foreach (string path in pathstring)
                                 {
                                     if (path != null)
                                     {
                                         ifFound[processID] = true;
                                         Console.WriteLine("Path found in checkAllDirectory " + path);
                                     }
                                 }
                             }
                             else
                                 continue;
                         }*/
                        }

                    }
                }
            }
        }
        //throw new NotImplementedException();

            public void SaveFile(string fileName,string Path)
        {
            try
            {
                conn.Open();
                string query = "insert into FilePath(FileName,Path,Daat,TIME) values(@filename,@path,@d,@time)";
                SqlCommand cmd = new SqlCommand(query,conn);
                cmd.Parameters.AddWithValue("@filename",fileName);
                cmd.Parameters.AddWithValue("@path",Path);
                cmd.Parameters.AddWithValue("@d", DateTime.Now.ToString("d"));
                cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("HH:mm:ss"));
                int row = cmd.ExecuteNonQuery();
                if (row == 1)
                    Debug.WriteLine("Insert Successfully");
                else
                    Debug.WriteLine("Insert Successfully");
                conn.Close();

            }
            catch (SqlException ex) { Debug.WriteLine("Error in Database\n"+ex); }
        }

        private string GetFilePath(int processID)
        {
            string wmiQuery = string.Format("select CommandLine from Win32_Process where ProcessId={0}", processID);
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery))
            {
                using (ManagementObjectCollection retObjectCollection = searcher.Get())
                {
                    foreach (ManagementObject retObject in retObjectCollection)
                    {
                        if (retObject["CommandLine"] != null)
                        {
                            string s = (string.Format("[{0}]", retObject["CommandLine"]));
                            string k = s.Substring(s.IndexOf("EXE") + 4);
                            k = k.Remove(k.IndexOf("]"));
                            if (k.Contains(" \""))
                            {
                                string[] split = k.Split(new string[] { " \"" }, StringSplitOptions.None);
                              //s  Console.WriteLine("split[1]="+split[1]);
                                    
                                return split[1];
                            }
                            else
                                return k;
                        }
                        return null;
                    }
                    return null;
                }
                //throw new NotImplementedException();
            }
        }
    }
}