 /*
 * Copyright (C) 2018 Istituto Italiano di Tecnologia
 * Authors: davide.tome@iit.it
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

namespace iCubProductionTestSuite
{
    public partial class FormAbout : Form
    {
        public FormAbout(string ver)
        {
            InitializeComponent();
            this.labelVer.Text += " " + ver;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
