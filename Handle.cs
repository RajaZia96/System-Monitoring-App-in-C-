using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideFormProgram
{
    class Handle
    {
        public string getPathThroughHandle(string Filename)
        {
            //Console.WriteLine("Filename  in hanlde " + Filename);
            Process tool = new Process();
            tool.StartInfo.FileName = "handle.exe";
            tool.StartInfo.UseShellExecute = false;
            tool.StartInfo.RedirectStandardOutput = true;
            tool.StartInfo.Arguments = Filename;
            tool.StartInfo.RedirectStandardError = true;
            tool.Start();
            string output = tool.StandardOutput.ReadToEnd();
           // Console.WriteLine("output: "+output);
            if (output.Contains("No matching handles found."))
            {
               // Console.WriteLine("Not Found");
                return "Not";
            }
            else
            {
                string[] split = output.Split(new string[] { "File" }, StringSplitOptions.None);
                string path = split[1].TrimStart();
                path = path.Split('\n')[0];
                int lastColon = path.LastIndexOf(':');
                int posTokenPath = path.LastIndexOf(':', lastColon - 1);
                string realPath = path.Substring(posTokenPath + lastColon - posTokenPath - 1).Trim();
                // Console.WriteLine("Data show " + realPath);
                return realPath;
            }
        }
    }
}
