using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace iCubProductionTestSuite.classes
{
    class Report
    {
        public StreamReader StreamReader { get; private set; }

        public void doReportTxt(ListBox lb, String iitcode, String serial, String result, bool view, bool debug, string dir, string fwdir)
        {
            string subdir = "";

           
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Directory.CreateDirectory(dir + "\\" + "PASS");
                Directory.CreateDirectory(dir + "\\" + "FAIL");
                Directory.CreateDirectory(dir + "\\" + "DEBUG");
            }

            if (result.Equals("PASSED")) subdir = "\\" + "PASS";
            else subdir = "\\" + "FAIL";
            if (debug) subdir = "\\" + "DEBUG";

            string f = dir + "\\" + subdir + "\\" + serial.ToString().PadLeft(4, '0') + "_" + iitcode + ".txt";

            if (File.Exists(f)) File.Delete(f);
            
            StreamWriter sw = new StreamWriter(f);

            for (int i = 0; i < lb.Items.Count; i++) sw.WriteLine(lb.Items[i].ToString());

            string fwlog = fwdir + "\\fw-log.txt";

            if (File.Exists(fwlog))
            {
                StreamReader sr = new StreamReader(fwlog);
                string commit = sr.ReadLine();
                sw.WriteLine("");
                sw.WriteLine("[icub-firmware-build " + commit + "]");
            }
           

            sw.Close();

           
            //open the report
            if (view) System.Diagnostics.Process.Start(f);

        }
    
       
    }
}
