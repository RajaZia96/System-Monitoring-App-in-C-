using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HideFormProgram
{
  public static  class GetData
    {
        public static string getData()
        {
            string text = GetMeText();
            if (text != null)
            {
                return text;
            }
            return null;
        }
       public static string GetMeText()
        {
            string res = "starting value";
            Thread staThread = new Thread(x =>
            {
                try
                {
                    res = Clipboard.GetText();
                }
                catch (Exception ex)
                {
                    res = ex.Message;
                }
            });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
            return res;
        }
    }
}
