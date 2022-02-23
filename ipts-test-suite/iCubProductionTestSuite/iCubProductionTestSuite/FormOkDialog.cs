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
    public partial class FormOkDialog : Form
    {
        private String text, imgpath;

        public FormOkDialog(String text, String imgpath)
        {
            InitializeComponent();
            this.text = text;
            this.imgpath = imgpath;
            labelText.Text = this.text;
            pictureBox1.BackgroundImageLayout = ImageLayout.Tile;
            pictureBox1.Image = Image.FromFile(@"../../" + this.imgpath);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
