/*
* Copyright (C) 2018 Istituto Italiano di Tecnologia
* Authors: davide.tome@iit.it
* CopyPolicy: Released under the terms of the AGPL
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Esd.IO.Ntcan;
using iCubProductionTestSuite.classes;

namespace iCubProductionTestSuite.classes
{
    class TestRunner
    {
        private bool pass = true;
        private bool res;
        private Testplan tp;
        private string s;
        private String prev="";

        public TestRunner(Testplan testplan)
        {
            this.tp = testplan;
        }

        List<OperationVariable> opvl;

        public bool Pass
        {
            get
            {
                return pass;
            }

            set
            {
                pass = value;
            }
        }

        public string Result
        {
            get
            {
                if (Pass) return "PASS";
                else return "FAIL";
            }

        }

        public string CmdResult
        {
            get
            {
                if (res) return "PASS";
                else return "FAIL";
            }

        }


        
        public bool runTest(Test test, ListBox logBox, bool repeated, CanUtils cu_t, SerialUtils su_t)
        {
            opvl = new List<OperationVariable>();
            Pass = true;
            foreach (Operation o in test.OperationList)
            {

                switch (o.Type)
                {
                    case "wait":
                        int msec = Convert.ToInt16(o.Value);
                        System.Threading.Thread.Sleep(msec);
                        break;

                    case "okDialog":
                        FormDialog f = new FormDialog(o.Text, o.Image);
                        f.ShowDialog();
                        if (o.Log != null && !o.Log.Equals("false"))
                        {
                            s = string.Format("{0,-3} {1,-40} {2,-8}", test.Id + ")", test.Name, "DONE");
                            logBox.Items.Add(s);
                        }
                        break;

                    case "inputValue":
                        FormInput fi = new FormInput(o.Text, o.Vartype, prev);
                        fi.ShowDialog();
                        OperationVariable opvn = new OperationVariable(o.Var, fi.Val, o.Vartype);
                        opvl.Add(opvn);
                        prev = fi.PrevVal;
                        break;

                    case "send":
                        CommandRunner crs = new CommandRunner(o, tp.TestInterfaces, opvl, cu_t, su_t);
                        crs.send();
                        break;

                    case "receivePassFail":
                        CommandRunner crr = new CommandRunner(o, tp.TestInterfaces, opvl, cu_t, su_t);
                        crr.receivePassFail();
                        res = crr.Pass;
                        if (!res) Pass = false;
                        if (o.Log == null || !o.Log.Equals("false"))
                        {
                            int nrMess = Convert.ToInt16(o.LogMess);

                            if (repeated && (o.Log == null || !o.Log.Equals("false")))
                            {
                                for(int j = nrMess; j >=1; j--) logBox.Items.RemoveAt(logBox.Items.Count - j);
                            }
                            logBox.Refresh();

                            if (nrMess > 0)
                            {
                                for(int i = 0; i < nrMess; i++)
                                {
                                    if(i == 0) s = string.Format("{0,-3} {1,-40} {2,-8} {3,-30}", test.Id + ")", test.Name, CmdResult, " [" + crr.Cmsg[i].ToString().Substring(23, crr.Cmsg[i].ToString().Length - 23) + "]");
                                    else s = string.Format("{0,-3} {1,-40} {2,-8} {3,-30}", "", "", "", " [" + crr.Cmsg[i].ToString().Substring(23, crr.Cmsg[i].ToString().Length - 23) + "]");
                                    logBox.Items.Add(s);
                                }

                            }
                            else
                            {
                                s = string.Format("{0,-3} {1,-40} {2,-8}", test.Id + ")", test.Name, CmdResult);
                                logBox.Items.Add(s);
                            }
                        }
                        break;

                    case "passFailDialog":
                        CommandRunner crpf = new CommandRunner(o, tp.TestInterfaces);
                        crpf.passFailDialog(test.Name);
                        res = crpf.Pass;
                        if (!res) Pass = false;
                        s = string.Format("{0,-3} {1,-40} {2,-8}", test.Id + ")", test.Name, CmdResult);
                        if (repeated && (o.Log == null || !o.Log.Equals("false"))) logBox.Items.RemoveAt(logBox.Items.Count - 1);
                        if (o.Log == null || !o.Log.Equals("false")) logBox.Items.Add(s);
                        break;

                    case "yesNoDialog":
                        CommandRunner cryn = new CommandRunner(o);
                        cryn.yesNoDialog(test.Name);
                        res = cryn.Pass;
                        if (!res) Pass = false;
                        s = string.Format("{0,-3} {1,-40} {2,-8}", test.Id + ")", test.Name, CmdResult);
                        if (repeated && (o.Log == null || !o.Log.Equals("false"))) logBox.Items.RemoveAt(logBox.Items.Count - 1);
                        if (o.Log == null || !o.Log.Equals("false")) logBox.Items.Add(s);
                        break;

                    case "batch":
                        CommandRunner crpb = new CommandRunner(o);
                        res = crpb.runBatch();
                       // res = crpb.Pass;
                        if (!res) Pass = false;
                        s = string.Format("{0,-3} {1,-40} {2,-8}", test.Id + ")", test.Name, CmdResult);
                        if (repeated && (o.Log == null || !o.Log.Equals("false"))) logBox.Items.RemoveAt(logBox.Items.Count - 1);
                        if (o.Log == null || !o.Log.Equals("false")) logBox.Items.Add(s);
                        break;
                    case "message":
                        MessageBox.Show(o.Text, "", MessageBoxButtons.OK);
                        break;

                    default: break;
                }
            }
            logBox.Refresh();
            return Pass;
        }
    }
}
