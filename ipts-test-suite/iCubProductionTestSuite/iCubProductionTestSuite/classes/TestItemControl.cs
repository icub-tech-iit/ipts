 /*
 * Copyright (C) 2018 Istituto Italiano di Tecnologia
 * Authors: davide.tome@iit.it
 * CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace iCubProductionTestSuite
{
    public partial class TestItemControl : UserControl
    {
        public TestItemControl()
        {
            InitializeComponent();
        }

        public void setResult(string res)
        {
            if (res == "Pass") label1.ForeColor = System.Drawing.Color.Green;
            else if (res.Contains("Running")) label1.ForeColor = System.Drawing.Color.Blue;
            else label1.ForeColor = System.Drawing.Color.Red;
            label1.Text = res;
            this.Refresh();
        }

        public void resetResult()
        {
            label1.ForeColor = System.Drawing.Color.DarkGray;
            label1.Text = "TBD";
            this.Refresh();
        }

        public String CheckboxText
        {
            get
            {
                return checkBox1.Text;
            }

            set
            {
                checkBox1.Text = value;
            }
        }

        public bool CheckboxChecked
        {
            get
            {
                return checkBox1.Checked;
            }
        }

        public void setCheckboxChecked(bool status)
        {
            checkBox1.Checked = status;
            if (status)
            {
                label1.ForeColor = System.Drawing.Color.DarkGray; 
                label1.Text = "TBD";
                this.Refresh();
            }
            else
            {
                label1.ForeColor = System.Drawing.Color.Orange;
                label1.Text = "Skipped";
                this.Refresh();
            }
        }

        public void setCheckboxEnabled(bool status)
        {
            checkBox1.AutoCheck = status;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox1.Checked)
            {
                label1.ForeColor = System.Drawing.Color.Orange;
                label1.Text = "Skipped";
                this.Refresh();
            }
            else
            {
                label1.ForeColor = System.Drawing.Color.DarkGray;
                label1.Text = "TBD";
                this.Refresh();
            }
        }

       
    }
}
