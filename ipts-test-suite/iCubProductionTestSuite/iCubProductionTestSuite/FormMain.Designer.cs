using System;

namespace iCubProductionTestSuite
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonSelDeselAllTest = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.openTestReportFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTestPlan = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxTestMode = new System.Windows.Forms.GroupBox();
            this.radioButtonDebug = new System.Windows.Forms.RadioButton();
            this.radioButtonProduction = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.labelOperator = new System.Windows.Forms.Label();
            this.buttonClearLog = new System.Windows.Forms.Button();
            this.buttonSaveLog = new System.Windows.Forms.Button();
            this.startStop1 = new iCubProductionTestSuite.classes.StartStop();
            this.menuStrip1.SuspendLayout();
            this.groupBoxTestMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSelDeselAllTest
            // 
            this.buttonSelDeselAllTest.Enabled = false;
            this.buttonSelDeselAllTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSelDeselAllTest.Location = new System.Drawing.Point(16, 849);
            this.buttonSelDeselAllTest.Name = "buttonSelDeselAllTest";
            this.buttonSelDeselAllTest.Size = new System.Drawing.Size(109, 23);
            this.buttonSelDeselAllTest.TabIndex = 1;
            this.buttonSelDeselAllTest.Text = "Sselect all tests";
            this.buttonSelDeselAllTest.UseVisualStyleBackColor = true;
            this.buttonSelDeselAllTest.Click += new System.EventHandler(this.buttonSelDeselAllTest_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemFile,
            this.toolStripMenuItemHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(977, 29);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItemFile
            // 
            this.toolStripMenuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openTestReportFolderToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.toolStripMenuItemFile.Name = "toolStripMenuItemFile";
            this.toolStripMenuItemFile.Size = new System.Drawing.Size(46, 25);
            this.toolStripMenuItemFile.Text = "File";
            // 
            // openTestReportFolderToolStripMenuItem
            // 
            this.openTestReportFolderToolStripMenuItem.Name = "openTestReportFolderToolStripMenuItem";
            this.openTestReportFolderToolStripMenuItem.Size = new System.Drawing.Size(256, 26);
            this.openTestReportFolderToolStripMenuItem.Text = "Open TestReports folder...";
            this.openTestReportFolderToolStripMenuItem.Click += new System.EventHandler(this.openTestReportFolderToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(256, 26);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // toolStripMenuItemHelp
            // 
            this.toolStripMenuItemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.toolStripMenuItemHelp.Name = "toolStripMenuItemHelp";
            this.toolStripMenuItemHelp.Size = new System.Drawing.Size(54, 25);
            this.toolStripMenuItemHelp.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 26);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // panelTestPlan
            // 
            this.panelTestPlan.AutoScroll = true;
            this.panelTestPlan.AutoSize = true;
            this.panelTestPlan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTestPlan.Location = new System.Drawing.Point(10, 96);
            this.panelTestPlan.Name = "panelTestPlan";
            this.panelTestPlan.Size = new System.Drawing.Size(331, 747);
            this.panelTestPlan.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(367, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "Test Log";
            // 
            // groupBoxTestMode
            // 
            this.groupBoxTestMode.Controls.Add(this.radioButtonDebug);
            this.groupBoxTestMode.Controls.Add(this.radioButtonProduction);
            this.groupBoxTestMode.Location = new System.Drawing.Point(10, 32);
            this.groupBoxTestMode.Name = "groupBoxTestMode";
            this.groupBoxTestMode.Size = new System.Drawing.Size(200, 42);
            this.groupBoxTestMode.TabIndex = 15;
            this.groupBoxTestMode.TabStop = false;
            this.groupBoxTestMode.Text = "Test mode";
            // 
            // radioButtonDebug
            // 
            this.radioButtonDebug.AutoSize = true;
            this.radioButtonDebug.Location = new System.Drawing.Point(97, 19);
            this.radioButtonDebug.Name = "radioButtonDebug";
            this.radioButtonDebug.Size = new System.Drawing.Size(57, 17);
            this.radioButtonDebug.TabIndex = 1;
            this.radioButtonDebug.Text = "Debug";
            this.radioButtonDebug.UseVisualStyleBackColor = true;
            this.radioButtonDebug.CheckedChanged += new System.EventHandler(this.radioButtonDebug_CheckedChanged);
            // 
            // radioButtonProduction
            // 
            this.radioButtonProduction.AutoSize = true;
            this.radioButtonProduction.Checked = true;
            this.radioButtonProduction.Location = new System.Drawing.Point(6, 19);
            this.radioButtonProduction.Name = "radioButtonProduction";
            this.radioButtonProduction.Size = new System.Drawing.Size(76, 17);
            this.radioButtonProduction.TabIndex = 0;
            this.radioButtonProduction.TabStop = true;
            this.radioButtonProduction.Text = "Production";
            this.radioButtonProduction.UseVisualStyleBackColor = true;
            this.radioButtonProduction.CheckedChanged += new System.EventHandler(this.radioButtonProduction_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(278, 16);
            this.label2.TabIndex = 16;
            this.label2.Text = "Test Name                                      Status";
            // 
            // listBoxLog
            // 
            this.listBoxLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.Location = new System.Drawing.Point(370, 96);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.ScrollAlwaysVisible = true;
            this.listBoxLog.Size = new System.Drawing.Size(607, 823);
            this.listBoxLog.TabIndex = 18;
            // 
            // labelOperator
            // 
            this.labelOperator.AutoSize = true;
            this.labelOperator.Image = global::iCubProductionTestSuite.Properties.Resources.user;
            this.labelOperator.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelOperator.Location = new System.Drawing.Point(367, 942);
            this.labelOperator.Name = "labelOperator";
            this.labelOperator.Size = new System.Drawing.Size(28, 13);
            this.labelOperator.TabIndex = 19;
            this.labelOperator.Text = "       ";
            this.labelOperator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonClearLog
            // 
            this.buttonClearLog.Enabled = false;
            this.buttonClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClearLog.Image = global::iCubProductionTestSuite.Properties.Resources.clear;
            this.buttonClearLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonClearLog.Location = new System.Drawing.Point(821, 65);
            this.buttonClearLog.Name = "buttonClearLog";
            this.buttonClearLog.Size = new System.Drawing.Size(75, 25);
            this.buttonClearLog.TabIndex = 14;
            this.buttonClearLog.Text = "Clear Log";
            this.buttonClearLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonClearLog.UseVisualStyleBackColor = true;
            this.buttonClearLog.Click += new System.EventHandler(this.buttonClearLog_Click);
            // 
            // buttonSaveLog
            // 
            this.buttonSaveLog.Enabled = false;
            this.buttonSaveLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSaveLog.Image = global::iCubProductionTestSuite.Properties.Resources.download_button;
            this.buttonSaveLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSaveLog.Location = new System.Drawing.Point(902, 64);
            this.buttonSaveLog.Name = "buttonSaveLog";
            this.buttonSaveLog.Size = new System.Drawing.Size(75, 26);
            this.buttonSaveLog.TabIndex = 13;
            this.buttonSaveLog.Text = "Save Log";
            this.buttonSaveLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonSaveLog.UseVisualStyleBackColor = true;
            this.buttonSaveLog.Click += new System.EventHandler(this.buttonSaveLog_Click);
            // 
            // startStop1
            // 
            this.startStop1.Location = new System.Drawing.Point(10, 878);
            this.startStop1.Margin = new System.Windows.Forms.Padding(4);
            this.startStop1.Name = "startStop1";
            this.startStop1.Size = new System.Drawing.Size(331, 77);
            this.startStop1.TabIndex = 17;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(989, 857);
            this.Controls.Add(this.labelOperator);
            this.Controls.Add(this.listBoxLog);
            this.Controls.Add(this.startStop1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBoxTestMode);
            this.Controls.Add(this.buttonClearLog);
            this.Controls.Add(this.buttonSaveLog);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelTestPlan);
            this.Controls.Add(this.buttonSelDeselAllTest);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IPTS - iCub Production Test Suite";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBoxTestMode.ResumeLayout(false);
            this.groupBoxTestMode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        #endregion
        private System.Windows.Forms.Button buttonSelDeselAllTest;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemFile;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemHelp;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Panel panelTestPlan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonSaveLog;
        private System.Windows.Forms.Button buttonClearLog;
        private System.Windows.Forms.GroupBox groupBoxTestMode;
        private System.Windows.Forms.RadioButton radioButtonDebug;
        private System.Windows.Forms.RadioButton radioButtonProduction;
        private System.Windows.Forms.Label label2;
        private classes.StartStop startStop1;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.Label labelOperator;
        private System.Windows.Forms.ToolStripMenuItem openTestReportFolderToolStripMenuItem;
    }
}

