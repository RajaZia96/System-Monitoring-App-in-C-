using NDde.Client;
using Oracle.ManagedDataAccess.Client;
using SHDocVw;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Automation;

namespace HideFormProgram
{
    class GetBrowsingHistory
    {
        SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=HAIER-PC;Initial Catalog=myproject;Integrated Security=True";
        public static string previousGoogle = null;
        public static string previousFirefox = null;
        public static string previousInternet = null;
        public static Timer timer1;
        public GetBrowsingHistory()
        {
            conn.ConnectionString = connectionString;
             timer1 = new Timer();
            timer1.Elapsed += new ElapsedEventHandler(Timer_click);
            timer1.Enabled = true;
            timer1.Interval = 2000;
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
            catch (Exception ex) { Debug.WriteLine("Error in GetBrowsingHistory in timerstop\n"+ex); }
        }
        private void Timer_click(object sender, ElapsedEventArgs e)
        {
            ////////////////// get the open process of Internet Explore
            Process[] p = Process.GetProcessesByName("iexplore");
            if (p.Length > 0)
            {

                string url = InternetExplorer();
                if (url != null)
                {
                    if (previousInternet == null)
                    {
                        //Console.WriteLine("Internet Explorer..............");
                        previousInternet = url;
                       // Console.WriteLine(url);
                        saveUrlinDatabase("Internet Explorer", url);
                    }
                    else if (previousInternet != url)
                    {
                       // Console.WriteLine("Internet Explorer..............");
                        previousInternet = url;
                      //  Console.WriteLine(url);
                        saveUrlinDatabase("Internet Explorer", url);
                    }
                    else
                    {
                    }
                }
            }
            ////////////////// get the open process of firefox
            Process[] f = Process.GetProcessesByName("firefox");
            if (f.Length > 0)
            {

                string url = UrlFireFox();
                if (url != null)
                {
                    if (previousFirefox == null)
                    {
                        //Console.WriteLine("FireFox..............");
                        previousFirefox = url;
                        //Console.WriteLine(url);
                        saveUrlinDatabase("FireFox", url);
                    }
                    else if (previousFirefox != url)
                    {
                        //Console.WriteLine("FireFox..............");
                        previousFirefox = url;
                       // Console.WriteLine(url);
                        saveUrlinDatabase("FireFox", url);
                    }
                    else
                    {
                    }
                }
            }

            ////////////////// get the open process of Googel Chrome
            Process[] c = Process.GetProcessesByName("chrome");
            if (c.Length > 0)
            {
                string url = fastChromeUrl();
                if (url != null)
                {
                    if (previousGoogle == null)
                    {
                       // Console.WriteLine("Google Chrome..............");
                        previousGoogle = url;
                        //Console.WriteLine(url);
                       // Library.WriteErrorLog("Google chrome");
                       // Library.WriteErrorLog(url);
                        saveUrlinDatabase("Google Chrome", url);
                    }
                    else if (previousGoogle != url)
                    {
                        // Console.WriteLine("Google Chrome..............");
                        previousGoogle = url;
                        //Console.WriteLine(url);
                      //  Library.WriteErrorLog(url);
                          saveUrlinDatabase("Google Chrome", url);
                    }
                    else
                    {
                    }
                }
            }
            //throw new NotImplementedException();
        }

        private string InternetExplorer()
        {
            string website = null;
            try
            {

                SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindows();
                string filename;
                foreach (InternetExplorer ie in shellWindows)
                {
                    filename = Path.GetFileNameWithoutExtension(ie.FullName).ToLower();

                    if (filename.Equals("iexplore"))
                    {
                        website = (ie.LocationURL.ToString());
                    }
                }
                if (website == null)
                {
                    return null;
                }
                else
                {
                    return website;
                }
            }
            catch
            {
                return null;
            }
            // throw new NotImplementedException();
        }

        private string UrlFireFox()
        {
            try
            {
                DdeClient dde = new DdeClient("firefox", "WWW_GetWindowInfo");
                dde.Connect();
                string url = dde.Request("URL", int.MaxValue);
                string[] text = url.Split(new string[] { "\",\"" }, StringSplitOptions.RemoveEmptyEntries);
                dde.Disconnect();
                return text[0].Substring(1);
            }
            catch
            {
                return null;
            }
            //throw new NotImplementedException();
        }

        private void saveUrlinDatabase(string v, string url)
        {
            var time = DateTime.Now.ToString("hh:mm: tt");
            DateTime today = DateTime.UtcNow.Date;
            try
            {
                conn.Open();
                string query = "insert into BROWSERHISTORY(BROWSERTYPE,URL,Daat,TIME) values(@type,@url,@d,@time)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@type", v);
                    cmd.Parameters.AddWithValue("@url", url);
                    cmd.Parameters.AddWithValue("@d", DateTime.Now.ToString("d"));
                    cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("HH:mm:ss"));
                int row = cmd.ExecuteNonQuery();
                if (row == 1)
                    Debug.WriteLine("Insert Successfully");
                else
                    Debug.WriteLine("Not insert");
            }
                conn.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error occure...................\n" + ex);
            }
            // throw new NotImplementedException();
        }

        private string fastChromeUrl()
        {
            Process[] procsChrome = Process.GetProcessesByName("chrome");
            foreach (Process chrome in procsChrome)
            {
                if (chrome.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }
                AutomationElement elm = AutomationElement.FromHandle(chrome.MainWindowHandle);
                AutomationElement elmUrlBar = null;
                try
                {
                    var elm1 = elm.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "Google Chrome"));
                    if (elm1 == null) { continue; }
                    var elm2 = TreeWalker.RawViewWalker.GetLastChild(elm1);
                    var elm3 = elm2.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, ""));
                    var elm4 = TreeWalker.RawViewWalker.GetNextSibling(elm3);
                    var elm5 = elm4.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar));
                    var elm6 = elm5.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, ""));
                    elmUrlBar = elm6.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

                }
                catch
                {
                    continue;
                }
                if (elmUrlBar == null) { continue; }

                if ((bool)elmUrlBar.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty))
                {
                    continue;
                }
                AutomationPattern[] pattren = elmUrlBar.GetSupportedPatterns();
                if (pattren.Length == 1)
                {

                    string ret = "";
                    try
                    {
                        ret = ((ValuePattern)elmUrlBar.GetCurrentPattern(pattren[0])).Current.Value;
                        return ret;

                    }
                    catch { }

                }
            }
            return null;
            //throw new NotImplementedException();
        }
    }
}
