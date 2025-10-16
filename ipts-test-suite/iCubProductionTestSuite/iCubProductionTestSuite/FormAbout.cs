 /*
 * Copyright (C) 2025 Istituto Italiano di Tecnologia
 * Authors: davide.tome@iit.it, jacopo.losi@iit.it
 * CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace iCubProductionTestSuite
{
    public partial class FormAbout : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FormAbout));
        public FormAbout(string ver)
        {
            InitializeComponent();
            this.labelVer.Text += " " + ver;
            log.InfoFormat("FormAbout Initialized. Application version: {0}", ver);

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
