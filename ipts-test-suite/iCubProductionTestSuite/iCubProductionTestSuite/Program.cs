 /*
 * Copyright (C) 2018 Istituto Italiano di Tecnologia
 * Authors: davide.tome@iit.it
 * CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace iCubProductionTestSuite
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///
        // Initialize log4net
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        [STAThread]
        static void Main()
        { 
            log.Info("Starting iCub Production Test Suite");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
