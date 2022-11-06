using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
namespace HideFormProgram
{
    class checkApplicationStartUp
    {
        public static bool IsInStartup()
        {
            return IsInStartup(Application.ProductName,Application.ExecutablePath);
        }

        private static bool IsInStartup(string productName, string executablePath)
        {
            RegistryKey rk;
            string value;
            try {
                rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
                value = rk.GetValue(productName).ToString();
                if (value == null)
                    return false;
                else if (!value.ToLower().Equals(executablePath.ToLower()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            } catch (Exception) {
            }

            try {
                rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
                value = rk.GetValue(productName).ToString();
                if (value == null)
                {
                    return false;
                }else if (!value.ToLower().Equals(executablePath.ToLower()))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
            //throw new NotImplementedException();t
        }
    }
}
