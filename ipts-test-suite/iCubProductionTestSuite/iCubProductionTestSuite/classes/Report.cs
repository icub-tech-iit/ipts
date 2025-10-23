/*
* Copyright (C) 2025 Istituto Italiano di Tecnologia
* Authors: davide.tome@iit.it, jacopo.losi@iit.it
* CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
*/

using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iCubProductionTestSuite.classes
{
    class Report
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Report));
        public StreamReader StreamReader { get; private set; }

        public void doReportTxt(ListBox lb, String iitcode, String serial, String result, bool view, bool debug, string dir, string fwdir)
        {
            string subdir = "";

            // ensure directories exist
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Directory.CreateDirectory(Path.Combine(dir, "PASS"));
                Directory.CreateDirectory(Path.Combine(dir, "FAIL"));
                Directory.CreateDirectory(Path.Combine(dir, "DEBUG"));
            }

            if (result.Equals("PASSED")) subdir = Path.Combine("", "PASS");
            else subdir = Path.Combine("", "FAIL");
            if (debug) subdir = Path.Combine("", "DEBUG");

            string f = Path.Combine(dir, subdir.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), serial.ToString().PadLeft(4, '0') + "_" + iitcode + ".txt");

            // If file exists and is readonly, clear readonly first so we can delete/overwrite it
            if (File.Exists(f))
            {
                try
                {
                    var attrs = File.GetAttributes(f);
                    if ((attrs & FileAttributes.ReadOnly) != 0)
                    {
                        File.SetAttributes(f, attrs & ~FileAttributes.ReadOnly);
                        log.Info($"Cleared ReadOnly attribute on existing report file '{f}' before deletion.");
                    }

                    File.Delete(f);
                }
                catch (Exception ex)
                {
                    // If we cannot delete the old file, log and abort report creation to avoid inconsistent state
                    log.Error($"Unable to delete existing report file '{f}'. Aborting report write.", ex);
                    throw;
                }
            }

            // write report
            try
            {
                using (StreamWriter sw = new StreamWriter(f, false, Encoding.UTF8))
                {
                    for (int i = 0; i < lb.Items.Count; i++) sw.WriteLine(lb.Items[i].ToString());

                    string fwlog = Path.Combine(fwdir, "fw-log.txt");

                    if (File.Exists(fwlog))
                    {
                        using (StreamReader sr = new StreamReader(fwlog))
                        {
                            string commit = sr.ReadLine();
                            sw.WriteLine("");
                            sw.WriteLine("[icub-firmware-build " + commit + "]");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Failed to create report file '{f}'", ex);
                throw;
            }

            // Mark the created report file as ReadOnly to avoid accidental edits.
            // Not a security boundary, but sufficient for your use-case.
            try
            {
                var attrs = File.GetAttributes(f);
                if ((attrs & FileAttributes.ReadOnly) == 0)
                {
                    File.SetAttributes(f, attrs | FileAttributes.ReadOnly);
                    log.Info($"Set ReadOnly attribute on report file '{f}'");
                }
            }
            catch (Exception ex)
            {
                // Log a warning so we have visibility if attribute setting fails (permission, antivirus lock, etc.)
                // Do not show UI blocking messages here to avoid interrupting automated runs.
                log.Warn($"Unable to set ReadOnly attribute on report file '{f}'", ex);
            }

            //open the report
            if (view)
            {
                try
                {
                    System.Diagnostics.Process.Start(f);
                }
                catch (Exception ex)
                {
                    log.Warn($"Unable to open report file '{f}' automatically.", ex);
                }
            }
        }
    }
}
