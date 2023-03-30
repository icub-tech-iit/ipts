﻿ /*
 * Copyright (C) 2018 Istituto Italiano di Tecnologia
 * Authors: davide.tome@iit.it
 * CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Esd.IO.Ntcan;
using iCubProductionTestSuite.classes;


namespace iCubProductionTestSuite
{
    public partial class FormInput : Form
    {
        private String type;
        private int val_int;
        private String val_t;
        private String serial;
        private String user;
        private int selBoard;
        private String selCAN;
        private String selSERIAL;
        private String prevVal ="";

        public FormInput(String text, String type, String prev)
        {
            InitializeComponent();
            this.Text = "Input Value";
            label1.Text = text;
            this.textBox1.Text = prev;
            this.type = type;
            this.button1.Click += buttonInputValue_Click;
        }

    
        public FormInput(int last_sn)
        {
            InitializeComponent();
            this.Text = "Serial Number";
            this.label1.Text = "Inserire il serial number";
            this.textBox1.Text = Convert.ToString(last_sn);
            this.button1.Click += buttonSerial_Click;
        }

        public FormInput(String user)
        {
            InitializeComponent();
            this.Text = "Operatore";
            this.label1.Text = "Inserire il nome dell' operatore";
            this.textBox1.Text = user;
            this.button1.Click += buttonOperator_Click;
        }

        public FormInput(List<Testplan> tplist, SettingsFile sf)
        {
            InitializeComponent();
            int index=0;
            List<String> lb = new List<string>();

            foreach(Testplan t in tplist)
            {
                lb.Add(t.Iitcode + " - " + t.Boardname + " - Testplan rev. " + t.Rev);
                if (t.Idtestplan.Equals(sf.LastSel)) index = Convert.ToInt16(sf.LastSel);
            }

            this.label1.Text = "Selezionare Scheda da collaudare";
            this.textBox1.Visible = false;
            this.comboBox1.Visible = true;
            this.comboBox1.DataSource = lb;
            this.button1.Click += buttonSelBoard_Click;
            this.comboBox1.SelectedIndex = index;
        }

        public FormInput(TestInterface t)
        {
            InitializeComponent();
            int index = 0;
            int portCount = 0;

            List<String> lb = new List<string>();


            //switch (t.Name)
            //{
            //    case "CAN": CanUtils cu = new CanUtils(); portCount = cu.Ports.Count; foreach (String p in cu.Ports) lb.Add(p); break;
            //    case "SERIAL": SerialUtils su = new SerialUtils(); portCount = su.Ports.Count; foreach (String p in su.Ports) lb.Add(p); break;

            //    default: break;
            //}

            //if (portCount == 0)
            //{
            //    MessageBox.Show("Nessuna Interfaccia " + t.Name + " rilevata!", "Errore",
            //        MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    Application.Exit();
            //}
            //else
            //{
            //    this.label1.Text = "Selezionare Interfaccia " + t.Name;
            //    this.textBox1.Visible = false;
            //    this.comboBox1.Visible = true;
            //    this.comboBox1.DataSource = lb;
            //    this.button1.Click += buttonSelCAN_Click;
            //    this.comboBox1.SelectedIndex = index;
            //}



            switch (t.Name)
            {
                case "CAN":
                    CanUtils cu = new CanUtils();
                    portCount = cu.Ports.Count;
                    foreach (String p in cu.Ports)
                        lb.Add(p);
                    if (portCount == 0)
                    {
                        MessageBox.Show("Nessuna Interfaccia " + t.Name + " rilevata!", "Errore",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                    this.label1.Text = "Selezionare Interfaccia " + t.Name;
                    this.textBox1.Visible = false;
                    this.comboBox1.Visible = true;
                    this.comboBox1.DataSource = lb;
                    this.button1.Click += buttonSelCAN_Click;
                    this.comboBox1.SelectedIndex = index;
                    break;
                case "SERIAL":
                    //SerialUtils su = new SerialUtils();
                    //portCount = su.Ports.Count;
                    portCount = SerialPort.GetPortNames().Length;

                    foreach (String p in SerialPort.GetPortNames()) lb.Add(p);
                    if (portCount == 0)
                    {
                        MessageBox.Show("Nessuna Interfaccia " + t.Name + " rilevata!", "Errore",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                    this.label1.Text = "Selezionare Interfaccia " + t.Name;
                    this.textBox1.Visible = false;
                    this.comboBox1.Visible = true;
                    this.comboBox1.DataSource = lb;
                    this.button1.Click += buttonSelSERIAL_Click;
                    this.comboBox1.SelectedIndex = index;
                    break;

                default: break;
            }

            if (portCount == 0)
            {
                MessageBox.Show("Nessuna Interfaccia " + t.Name + " rilevata!", "Errore",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else
            {
                this.label1.Text = "Selezionare Interfaccia " + t.Name;
                this.textBox1.Visible = false;
                this.comboBox1.Visible = true;
                this.comboBox1.DataSource = lb;
                this.button1.Click += buttonSelCAN_Click;
                this.comboBox1.SelectedIndex = index;
            }

        }

        public int Val_int
        {
            get
            {
                return val_int;
            }

            set
            {
                val_int = value;
            }
        }

        public string Val_t
        {
            get
            {
                return val_t;
            }

            set
            {
                val_t = value;
            }
        }

        public string Val
        {
            get
            {
                return textBox1.Text;
            }
            
        }

        public String Serial
        {
            get
            {
                return serial;
            }

            set
            {
                serial = value;
            }
        }

        public string User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
            }
        }

        public int SelBoard
        {
            get
            {
                return selBoard;
            }

            set
            {
                selBoard = value;
            }
        }

        public string PrevVal
        {
            get
            {
                return prevVal;
            }

            set
            {
                prevVal = value;
            }
        }

        public int SelSerial
        {
            get
            {
                return SelSerial;
            }

            set
            {
                SelSerial = value;
            }
        }

        public string SelCAN { get => selCAN; set => selCAN = value; }
        public string SelSERIAL { get => selSERIAL; set => selSERIAL = value; }

        private void buttonSelCAN_Click(object sender, EventArgs e)
        {
            this.SelCAN = this.comboBox1.SelectedItem.ToString();
            this.Hide();
        }

        private void buttonSelSERIAL_Click(object sender, EventArgs e)
        {
            this.SelSERIAL = this.comboBox1.SelectedItem.ToString();
            this.Hide();
        }

        private void buttonSelBoard_Click(object sender, EventArgs e)
        {
            this.SelBoard = this.comboBox1.SelectedIndex;
            this.Hide();
        }

        private void buttonOperator_Click(object sender, EventArgs e)
        {
            if(this.textBox1.Text.Length == 0)
            {
                MessageBox.Show("Il nome operatore non deve essere nullo!", "Errore",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            this.User = this.textBox1.Text;
            this.Hide();
        }

        private void buttonSerial_Click(object sender, EventArgs e)
        {
           
                this.Serial = this.textBox1.Text;
           
            this.Hide();
        }

        private void buttonInputValue_Click(object sender, EventArgs e)
        {
            switch(type)
            {
                case "num":
                    int d;
                    if (int.TryParse(textBox1.Text, out d))
                    {
                        this.val_int = d;
                        PrevVal = textBox1.Text;
                    }
                    else
                    {
                        MessageBox.Show("Il valore immesso deve essere un numero intero.", "Errore",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    break;
                case "text":
                    if (!textBox1.Text.Equals(""))
                    {
                        this.val_t = textBox1.Text;
                        PrevVal = textBox1.Text;
                    }                      
                    else return;
                    break;
                case "serial":
                    if (!textBox1.Text.Equals(""))
                    {
                        this.Serial = this.textBox1.Text;
                    }
                    else return;
                    break;

            }
            this.Hide();
        }
    }
}
