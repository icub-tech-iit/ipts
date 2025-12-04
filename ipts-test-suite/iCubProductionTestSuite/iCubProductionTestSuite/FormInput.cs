/*
 * Copyright (C) 2025 Istituto Italiano di Tecnologia
 * Authors: davide.tome@iit.it, jacopo.losi@iit.it
 * CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
 */
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using iCubProductionTestSuite.classes;
using log4net;


namespace iCubProductionTestSuite
{
    public partial class FormInput : Form
    {
        private string type;  
        private int val_int;
        private string val_t; 
        private string serial; 
        private string user;   
        private int selBoard;
        private string selCAN;  
        private string selSERIAL; 
        private string prevVal = ""; 
        private bool isReady = false;

        private static readonly ILog log = LogManager.GetLogger(typeof(FormInput));

        public FormInput(String text, String type, String prev)
        {
            InitializeComponent();
            log.Info("FormInput Initialized.");
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
            int index = 0;
            List<String> lb = new List<string>();

            foreach (Testplan t in tplist)
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

            // Get ports according to interface type
            List<string> lb = GetPortsForInterface(t);

            if (lb.Count == 0)
            {
                MessageBox.Show(
                    "Nessuna Interfaccia " + t.Name + " rilevata!\n\n" +
                    "Utilizzerò la porta di default definita nel file di configurazione: " + t.NetPort + "\n\n" +
                    "La selezione sarà disponibile al momento dell'esecuzione del test.",
                    "Avviso - Interfaccia Non Disponibile",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                log.WarnFormat("Interface {0} not available at startup. Default port {1} will be used.", t.Name, t.NetPort);
                isReady = false;
                return;
            }

            ConfigureInterfaceDropdown(t, lb);
            isReady = true;
        }

        /// <summary>
        /// Configures the dropdown UI for interface selection.
        /// </summary>
        private void ConfigureInterfaceDropdown(TestInterface t, List<string> portList)
        {
            label1.Text = "Selezionare Interfaccia " + t.Name;
            textBox1.Visible = false;
            comboBox1.Visible = true;

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(portList.ToArray());
            comboBox1.DropDownStyle = ComboBoxStyle.DropDown;
            comboBox1.Text = !string.IsNullOrEmpty(t?.NetPort) ? t.NetPort : portList[0];

            button1.Click += (sender, e) =>
            {
                var selected = (comboBox1.Text ?? string.Empty).Trim();
                if (string.IsNullOrEmpty(selected))
                {
                    log.DebugFormat("Interface selection cancelled for {0}", t?.Name);
                    Hide();
                    return;
                }

                SetSelectedInterface(t, selected);
                Hide();
            };

            comboBox1.SelectedIndex = 0;
        }

        /// <summary>
        /// Sets the selected interface based on type.
        /// </summary>
        private void SetSelectedInterface(TestInterface t, string selected)
        {
            if (t?.Name.Equals("CAN", StringComparison.OrdinalIgnoreCase) == true)
            {
                SelCAN = selected;
                log.DebugFormat("CAN interface selected: {0}", selected);
            }
            else if (t?.Name.Equals("SERIAL", StringComparison.OrdinalIgnoreCase) == true)
            {
                SelSERIAL = selected;
                log.DebugFormat("SERIAL interface selected: {0}", selected);
            }
        }

        public bool IsReady()
        {
            return isReady;
        }

        /// <summary>
        /// Helper to centralize port discovery logic for each interface type.
        /// Returns empty list if no ports found or on exception.
        /// </summary>
        private List<string> GetPortsForInterface(TestInterface t)
        {
            var list = new List<string>();
            if (t == null || string.IsNullOrEmpty(t.Name))
                return list;

            switch (t.Name.ToUpperInvariant())
            {
                case "CAN":
                    try
                    {
                        // Using existing CanUtils to get CAN ports
                        CanUtils cu = new CanUtils();
                        if (cu?.Ports != null && cu.Ports.Count > 0)
                        {
                            list.AddRange(cu.Ports);
                            log.DebugFormat("Found {0} CAN port(s)", cu.Ports.Count);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.WarnFormat("Error discovering CAN ports: {0}", ex.Message);
                        // Return empty list; caller handles gracefully
                    }
                    break;

                case "SERIAL":
                    try
                    {
                        // Get serial port names from System.IO.Ports
                        string[] portNames = SerialPort.GetPortNames();
                        if (portNames != null && portNames.Length > 0)
                        {
                            list.AddRange(portNames);
                            log.DebugFormat("Found {0} SERIAL port(s): {1}", portNames.Length, string.Join(", ", portNames));
                        }
                    }
                    catch (Exception ex)
                    {
                        log.WarnFormat("Error discovering SERIAL ports: {0}", ex.Message);
                        // Return empty list; caller handles gracefully
                    }
                    break;

                default:
                    log.WarnFormat("Unknown interface type: {0}", t.Name);
                    break;
            }
            return list;
        }

        #region Properties

        public int Val_int
        {
            get { return val_int; }
            set { val_int = value; }
        }

        public string Val_t
        {
            get { return val_t; }
            set { val_t = value; }
        }

        public string Val
        {
            get { return textBox1.Text; }
        }

        public String Serial
        {
            get { return serial; }
            set { serial = value; }
        }

        public string User
        {
            get { return user; }
            set { user = value; }
        }

        public int SelBoard
        {
            get { return selBoard; }
            set { selBoard = value; }
        }

        public string PrevVal
        {
            get { return prevVal; }
            set { prevVal = value; }
        }

        /// <summary>
        /// Fixed: was recursive getter/setter. Now properly backed by field.
        /// </summary>
        public string SelCAN
        {
            get { return selCAN; }
            set { selCAN = value; }
        }

        /// <summary>
        /// Serial interface selection. Properly backed by field.
        /// </summary>
        public string SelSERIAL
        {
            get { return selSERIAL; }
            set { selSERIAL = value; }
        }

        #endregion

        #region Event Handlers

        private void buttonSelBoard_Click(object sender, EventArgs e)
        {
            this.SelBoard = this.comboBox1.SelectedIndex;
            this.Hide();
        }

        private void buttonOperator_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Length == 0)
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
            switch (type)
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
                    else
                        return;
                    break;

                case "serial":
                    if (!textBox1.Text.Equals(""))
                    {
                        this.Serial = this.textBox1.Text;
                    }
                    else
                        return;
                    break;
            }
            this.Hide();
        }

        #endregion
    }
}
