using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideFormProgram
{
    class addApplicationInStartUp
    {
        public static bool RunOnStartUp()
        {
            return RunOnStartUp(Application.ProductName,Application.ExecutablePath);
        }

        private static bool RunOnStartUp(string productName, string executablePath)
        {
            RegistryKey rk;
            try {

                rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
                rk.SetValue(productName,executablePath);
            }
            catch (Exception)
            {
            }
            try
            {
                rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run",true);
                rk.SetValue(productName,executablePath);
            }
            catch (Exception) {
                return false;
            }

            return true;
            //throw new NotImplementedException();
        }
    }
}
