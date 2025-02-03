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
    public partial class FormDialog : Form
    {
        private String text, imgpath;
        private bool yn;

        public FormDialog(String text, String imgpath)
        {
            InitializeComponent();
            this.text = text;
            this.imgpath = imgpath;
            textBox1.Text = this.text;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Image = Image.FromFile(@"../../" + this.imgpath);
            buttonNo.Visible = false;
            buttonYes.Visible = false;
            this.AcceptButton = buttonOk;
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

        }

        public FormDialog(String text, String imgpath, bool yn)
        {
            InitializeComponent();
            this.text = text;
            this.imgpath = imgpath;
            textBox1.Text = this.text;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Image = Image.FromFile(@"../../" + this.imgpath);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            buttonOk.Visible = false;
            this.AcceptButton = buttonYes;
        }

        public bool Yn
        {
            get
            {
                return yn;
            }

            set
            {
                yn = value;
            }
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {
            this.yn = true;
            this.Hide();
        }

        private void buttonNo_Click(object sender, EventArgs e)
        {
            this.yn = false;
            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
