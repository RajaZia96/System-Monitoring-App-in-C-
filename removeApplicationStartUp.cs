using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace HideFormProgram
{
    class removeApplicationStartUp
    {
        public static bool removeOnStartUp()
        {
            return removeOnStartUp(Application.ProductName,Application.ExecutablePath);
        }

        private static bool removeOnStartUp(string productName, string executablePath)
        {
            RegistryKey rk;
            try
            {
                rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
                if (executablePath == null)
                {
                    rk.DeleteValue(productName);
                }
                else
                {
                    if (rk.GetValue(productName).ToString().ToLower() == executablePath.ToLower())
                    {
                        rk.DeleteValue(productName);
                    }
                }
                return true;
            }
            catch (Exception) { }
            try
            {
                rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
                if (executablePath == null)
                {
                    rk.DeleteValue(productName);
                }
                else
                {
                    if(rk.GetValue(productName).ToString().ToLower() == executablePath.ToLower())
                    {
                        rk.DeleteValue(productName);
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
            //throw new NotImplementedException();
        }
    }
}
