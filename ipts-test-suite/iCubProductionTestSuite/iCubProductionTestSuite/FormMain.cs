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
using System.Windows.Controls;
using System.Windows.Forms;
using iCubProductionTestSuite.classes;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using log4net;

namespace iCubProductionTestSuite
{
    public partial class FormMain : Form
    {
        ConfigParser cp;
        ConfigFile cf;
        Testplan tp;
        TestRunner tr;
        List<ConfigFile> conflist;
        List<Testplan> tplist;
        SettingsFile sf;
        CanUtils cu;
        SerialUtils su;
        GroupedControls controlsDebug = new GroupedControls();
        bool STOP = false;
        bool PASS = true;
        bool DEBUG = false;
        String LAST_SN = "0";
        String OPERATOR = "";
        static readonly String SW_VER = "1.7.3 - 21/04/2026"; // refer to  https://github.com/mesh-iit/ipts
        String RESULT = ""; 
        static String CONFIG_DIR = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        static String CONFIG_FILE = "ipts.xml";
        static String SETTINGS_FILE = "settings.xml";
        static String REPORTS_DIR = System.IO.Path.Combine(CONFIG_DIR, "TestReports");
        static String FW_DIR = System.IO.Path.Combine( CONFIG_DIR, "tools", "boards", "icub-firmware-build");
        private static readonly ILog log = LogManager.GetLogger(typeof(FormMain));

        public FormMain()
        {
            InitializeComponent();
           log.Info("FormMain Initialized");
        }


        private void FormMain_Load(object sender, EventArgs e)
        {
            // Ensure the form fits the main screen
            var screen = Screen.PrimaryScreen.WorkingArea;
            if (this.Width > screen.Width) this.Width = screen.Width;
            if (this.Height > screen.Height) this.Height = screen.Height;
            this.Left = screen.Left + (screen.Width - this.Width) / 2;
            this.Top = screen.Top + (screen.Height - this.Height) / 2;

            startStop1.ButtonStartClick += new EventHandler(StartStop_ButtonStartClick);
            startStop1.ButtonStopClick += new EventHandler(StartStop_ButtonStopClick);

            //istanzio lista controlli relativi al modo esecuzione (prod/debug)
            controlsDebug.addControl(buttonSelDeselAllTest);
            controlsDebug.addControl(buttonSaveLog);
            controlsDebug.addControl(buttonClearLog);

            //verifico esistenza main report folder ed eventuale creazione
            if (!Directory.Exists(REPORTS_DIR)) Directory.CreateDirectory(REPORTS_DIR);

            //istanzio classi necessarie per parsing
            cp = new ConfigParser();
            cf = new ConfigFile();
            conflist = new List<ConfigFile>();
            tplist = new List<Testplan>();
            tp = new Testplan();
            sf = new SettingsFile();


            //carico startup actions
            try
            {
                cp.readStartupFromConfig(CONFIG_DIR + "\\" + CONFIG_FILE);
            }
            catch (Exception ape)
            {
                MessageBox.Show(ape.Message);
                Application.Exit();
            }

            //carico lista boards
            try
            {
                tplist = cp.readBoardsFromConfig(CONFIG_DIR + "\\" + CONFIG_FILE);
            }
            catch (Exception ape)
            {
                MessageBox.Show(ape.Message);
                Application.Exit();
            }

            //carico file settings
            try
            {
                sf = cp.readSettings(CONFIG_DIR + "\\" + SETTINGS_FILE);
            }
            catch (Exception ape)
            {
                MessageBox.Show(ape.Message);
                Application.Exit();
            }

            OPERATOR = sf.User;

            //scelta operatore
            FormInput fi_u = new FormInput(this.OPERATOR);
            fi_u.ShowDialog();
            OPERATOR = fi_u.User;
            sf.User = OPERATOR;
            
            //scelta scheda da testare
            FormInput fi_b = new FormInput(tplist, sf);
            fi_b.ShowDialog();

            foreach(SettingsFile.Setting s in sf.Settings)
            {
                if (s.TestplanId.Equals(tplist[fi_b.SelBoard].Idtestplan))
                {
                    LAST_SN = s.LastSN;
                    sf.LastSel = s.TestplanId;
                }
            }

            try
            {
                tp = cp.readConfig(CONFIG_DIR + "\\" + CONFIG_FILE, tplist[fi_b.SelBoard].Idtestplan);
            }
            catch (Exception ape)
            {
                MessageBox.Show(ape.Message);
                Application.Exit();
            }

            // Scelta delle interfacce di comunicazione
            foreach (TestInterface t in tp.TestInterfaces)
            {
                log.Debug("Selecting interface: " + t.Name);
                
                if (t.Name.Equals("CAN"))
                {
                    FormInput fi_i = new FormInput(t);
                    if (fi_i.IsReady()) { fi_i.ShowDialog(); }
                    if (!string.IsNullOrEmpty(fi_i.SelCAN)) 
                    { 
                        t.NetPort = fi_i.SelCAN; 
                        cu = new CanUtils(t); 
                    }
                }
                else if (t.Name.Equals("SERIAL"))
                {
                    FormInput fi_i = new FormInput(t);
                    if (fi_i.IsReady()) { fi_i.ShowDialog(); }
                    
                    if (!string.IsNullOrEmpty(fi_i.SelSERIAL)) 
                    { 
                        t.NetPort = fi_i.SelSERIAL; 
                        su = new SerialUtils(t);
                    }
                    else
                    {
                        // SERIAL interface not found at startup - use default initialization
                        // The interface will be reconfigured just-in-time when first used
                        log.WarnFormat("SERIAL interface not available at startup. Using default initialization for {0}", t.NetPort);
                        su = new SerialUtils(t); // Creates with default "COM1" if ti.NetPort is empty
                    }
                }
            }

            //setto titolo del form
            this.Text += " -  " + tp.Boardname + " - " + tp.Iitcode + " - Testplan rev. " + tp.Rev;

            //setto label operator
            this.labelOperator.Text += OPERATOR + " - " + DateTime.Now.ToString("dddd, dd MMMM yyyy");

            //riempo la lista dei tests da eseguire
            int y = 0;
            
            foreach (Test t in tp.TestList)
            {
                TestItemControl tic = new TestItemControl();
                tic.Name = "tic" + t.Id.ToString();
                tic.Top = y;
                tic.CheckboxText = t.Name;
                tic.setCheckboxEnabled(false);
                controlsDebug.addControl(tic);
                panelTestPlan.Controls.Add(tic);
                tp.addControl(tic);
                y += 20;
            }

            tr = new TestRunner(tp);
        }

        protected async void StartStop_ButtonStartClick(object sender, EventArgs e)
        {
            
            bool view = false;
            bool repeated = false;
            bool repeat = true;
            PASS = true;

            FormInput fi_s = new FormInput("Inserisci Serial Number (es. 0123 , A1234)", "serial" ,this.LAST_SN);
            fi_s.ShowDialog();
            LAST_SN = fi_s.Serial;

            //verifica interfacce (CAN, Seriale..)
            if (checkErrorInterfaces()) return;

            if (radioButtonProduction.Checked) listBoxLog.Items.Clear();

            tp.resetTestResult();

            listBoxLog.Items.Add("****************************************************************");
            listBoxLog.Items.Add("IPTS - iCub Production Test Suite [Version :" + SW_VER + "]");
            listBoxLog.Items.Add(Environment.NewLine);
            listBoxLog.Items.Add("DUT       : " + tp.Boardname + " - " + tp.Iitcode);
            listBoxLog.Items.Add("SN        : " + LAST_SN.ToString().PadLeft(4, '0'));
            listBoxLog.Items.Add("Operatore : " + OPERATOR);
            listBoxLog.Items.Add(Environment.NewLine);
            listBoxLog.Items.Add("Start : " + DateTime.Now);
            listBoxLog.Items.Add("----------------------------------------------------------------");
            listBoxLog.Items.Add("TESTPLAN (rev. " + tp.Rev + ") :");
            listBoxLog.Items.Add(Environment.NewLine);

            //eseguo i tests
            foreach (Test t in tp.TestPlan)
            {
                //inserisco un ritardo per catturare eventuale stop
                await System.Threading.Tasks.Task.Delay(3000);
                int testid = Convert.ToInt16(t.Id) - 1;

                //se premuto lo stop interrompo esecuzione
                if (STOP)
                {
                    if (MessageBox.Show("STOP utente ricevuto! Vuoi terminare il test?", "Info",
                             MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        listBoxLog.Items.Add("----------------------------------------------------------------");
                        listBoxLog.Items.Add("End : " + DateTime.Now);
                        listBoxLog.Items.Add(Environment.NewLine);
                        listBoxLog.Items.Add("**************************STOPPED BY USER************************");
                        STOP = false;
                        startStop1.setStartEnabled(true);
                        return;
                    }
                    else STOP = false;
                }
                repeated = false;
                bool PASS_TMP = true;
                //eseguo il test
                while (repeat)
                {
                    tp.setTestResult(testid, "Running...");
                    this.Refresh();
                    PASS_TMP = tr.RunTest(t, this.listBoxLog, repeated, cu, su);
                    

                    if (!PASS_TMP)
                    {
                        tp.setTestResult(testid, "Fail");
                        if (MessageBox.Show(this, "Vuoi ripetere il test : " + t.Name + " ?", "Test Fallito!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                        {
                            repeat = false;
                        }
                        else
                        {
                            //PASS = true;
                            repeated = true;
                        }
                    }
                    else { repeat = false; repeated = false; }
                }
                repeat = true;
                if (!PASS_TMP) PASS = false;

                if (!PASS_TMP && !DEBUG && t.StopOnFail != null && t.StopOnFail == "true")
                {
                    
                    startStop1.setStartEnabled(true);
                    break;
                }
                else
                {
                    if(!PASS_TMP) tp.setTestResult(testid, "Fail");
                    else tp.setTestResult(testid, "Pass");

                }
            }

            if (!PASS) RESULT = "FAILED";
            else RESULT = "PASSED";

            listBoxLog.Items.Add(Environment.NewLine);
            listBoxLog.Items.Add("----------------------------------------------------------------");
            listBoxLog.Items.Add("End : " + DateTime.Now);
            listBoxLog.Items.Add(Environment.NewLine);
            listBoxLog.Items.Add("****************************************************************");
            listBoxLog.Items.Add("End of Tests - " + RESULT);

            if (!DEBUG)
            {
                DialogResult dialogResult = MessageBox.Show(this, "TEST " + RESULT + " !!!\n\n Vuoi vedere il report?", "End of Tests", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dialogResult == DialogResult.Yes) view = true;

                Report rep = new Report();
                rep.doReportTxt(listBoxLog, tp.Iitcode, LAST_SN, RESULT, view, false, System.IO.Path.Combine(REPORTS_DIR, tp.ReportsDir), FW_DIR);

                //Aggiorno SN
                double d;

                if (double.TryParse(LAST_SN.Substring(0,1), out d)) LAST_SN = Convert.ToString(Convert.ToInt16(fi_s.Serial) + 1);
                else LAST_SN = LAST_SN.Substring(0,1) + Convert.ToString(Convert.ToInt16(fi_s.Serial.Substring(1,fi_s.Serial.Length - 1)) + 1).PadLeft(4, '0');
               

                //aggiorno file settings
                try
                {
                    cp.writeSettings(CONFIG_DIR + "\\" + SETTINGS_FILE, OPERATOR, sf.LastSel, LAST_SN);
                }
                catch (Exception ape)
                {
                    MessageBox.Show(ape.Message);
                    Application.Exit();
                }

            }

            startStop1.setStartEnabled(true);
        }

        protected void StartStop_ButtonStopClick(object sender, EventArgs e)
        {
            STOP = true;
        }

        private bool checkErrorInterfaces()
        {
            bool error = false;

            foreach (TestInterface t in tp.TestInterfaces)
            {
                switch (t.Name)
                {
                    case "CAN":
                        cu = new CanUtils();
                        if (cu.Ports.Count == 0)
                        {
                            error = true;
                            MessageBox.Show(this, "Errore interfaccia CAN, controllare collegamenti", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            startStop1.setStartEnabled(true);
                        }
                        break;

                    case "SERIAL":
                        // SERIAL interface check is now non-blocking; it will be validated at runtime
                        // This allows testing boards without initial SERIAL connectivity
                        if (SerialPort.GetPortNames().Length == 0)
                        {
                            log.WarnFormat("No SERIAL ports detected at validation time. Tests using SERIAL will attempt lazy initialization.");
                            // Do not fail here - SERIAL may become available after firmware flash
                        }
                        break;

                    default: break;
                }
            }

            return error;
        }

        private void buttonSelDeselAllTest_Click(object sender, EventArgs e)
        {
            //deseleziono tutti i tests
            if (buttonSelDeselAllTest.Text.Equals("Deselect all tests"))
            {
                for (int i = 0; i < panelTestPlan.Controls.Count; i++)
                {
                    TestItemControl tic = (TestItemControl)panelTestPlan.Controls[i];
                    tic.setCheckboxChecked(false);
                }
                buttonSelDeselAllTest.Text = "Select all tests";
            }
            else //seleziono tutti i tests
            {
                for (int i = 0; i < panelTestPlan.Controls.Count; i++)
                {
                    TestItemControl tic = (TestItemControl)panelTestPlan.Controls[i];
                    tic.setCheckboxChecked(true);
                }
                buttonSelDeselAllTest.Text = "Deselect all tests";
            }
        }
        
        private void radioButtonProduction_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonDebug.Checked)
            {
                controlsDebug.setEnable(true);
                for (int i = 0; i < panelTestPlan.Controls.Count; i++)
                {
                    TestItemControl tic = (TestItemControl)panelTestPlan.Controls[i];
                    tic.setCheckboxChecked(false);
                    buttonSelDeselAllTest.Text = "Select all tests";
                }
                DEBUG = true;
            }
            else
            {
                controlsDebug.setEnable(false);
                for (int i = 0; i < panelTestPlan.Controls.Count; i++)
                {
                    TestItemControl tic = (TestItemControl)panelTestPlan.Controls[i];
                    tic.setCheckboxChecked(true);
                    tic.resetResult();
                }
                DEBUG = false;
            }
        }

        
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout fa = new FormAbout(SW_VER);
            fa.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            listBoxLog.Items.Clear();
        }

        private void buttonSaveLog_Click(object sender, EventArgs e)
        {
            Report rep = new Report();
            rep.doReportTxt(listBoxLog, tp.Iitcode, LAST_SN, RESULT, true, true, System.IO.Path.Combine(REPORTS_DIR, tp.ReportsDir), FW_DIR);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void openTestReportFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            if (Directory.Exists(REPORTS_DIR)) Process.Start(REPORTS_DIR);
            else MessageBox.Show(this, "La directory " + REPORTS_DIR +" non esiste...", "Warning",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void radioButtonDebug_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void startStop1_Load(object sender, EventArgs e)
        {

        }
    }
}
