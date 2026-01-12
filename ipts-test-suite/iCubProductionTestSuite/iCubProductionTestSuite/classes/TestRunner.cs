/*
* Copyright (C) 2025 Istituto Italiano di Tecnologia
* Authors: davide.tome@iit.it, jacopo.losi@iit.it
* CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
*/

using Esd.IO.Ntcan;
using log4net;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace iCubProductionTestSuite.classes
{
    class TestRunner
    {
        private bool pass = true;
        private bool res;
        private Testplan tp;
        private string s;
        private Operation prevSendOperation = null;
        private string previousInputValue="";

        private static readonly ILog log = LogManager.GetLogger(typeof(TestRunner));

        public TestRunner(Testplan testplan)
        {
            tp = testplan;
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


        public bool RunTest(Test test, ListBox logBox, bool repeated, CanUtils cu_t, SerialUtils su_t)
        {
            var opvl = new List<OperationVariable>();
            Pass = true;

            for (int k = 0; k < test.OperationList.Count; ++k)
            {
                Operation o = test.OperationList[k];
                ExecuteOperation(o, test, logBox, opvl, cu_t, su_t, repeated);
            }

            logBox.Refresh();
            return Pass;
        }

        private void ExecuteOperation(Operation o, Test test, ListBox logBox, List<OperationVariable> opvl,
                                  CanUtils cu_t, SerialUtils su_t, bool repeated)
        {
            switch (o.Type)
            {
                case "wait":
                    HandleWait(o);
                    break;

                case "okDialog":
                    HandleOkDialog(o, test, logBox);
                    break;

                case "inputValue":
                    HandleInputValue(o, opvl);
                    break;

                case "send":
                    HandleSend(o, test, logBox, opvl, cu_t, su_t);
                    break;

                case "receivePassFail":
                    HandleReceivePassFail(o, test, logBox, opvl, cu_t, su_t, repeated);
                    break;

                case "passFailDialog":
                    HandlePassFailDialog(o, test, logBox, opvl, cu_t, su_t, repeated);
                    break;

                case "yesNoDialog":
                    HandleYesNoDialog(o, test, logBox, opvl, cu_t, su_t, repeated);
                    break;

                case "batch":
                    HandleBatch(o, test, logBox, repeated);
                    break;
                case "message":
                    MessageBox.Show(o.Text, "", MessageBoxButtons.OK);
                    break;

                default: break;
            }
        }

        private void HandleWait(Operation o)
        {
            int msec = Convert.ToInt16(o.Value);
            System.Threading.Thread.Sleep(msec);
        }

        private void HandleOkDialog(Operation o, Test test, ListBox logBox)
        {
            var f = new FormDialog(o.Text, o.Image);
            f.ShowDialog();
            if (o.Log != null && !o.Log.Equals("false"))
                AddLogEntry(logBox, test.Id, test.Name, "DONE");
        }

        private void HandleInputValue(Operation o, List<OperationVariable> opvl)
        {
            var fi = new FormInput(o.Text, o.Vartype, previousInputValue);
            fi.ShowDialog();
            var opvn = new OperationVariable(o.Var, fi.Val, o.Vartype);
            opvl.Add(opvn);
            previousInputValue = fi.PrevVal;
        }

        private void HandleSend(Operation o, Test test, ListBox logBox, List<OperationVariable> opvl,
                           CanUtils cu_t, SerialUtils su_t)
        {
            var crs = new CommandRunner(o, tp.TestInterfaces, opvl, cu_t, su_t);
            if (!crs.Send())
            {
                Pass = false;
                AddLogEntry(logBox, test.Id, test.Name, "SEND_FAIL");
            }
            prevSendOperation = o;
        }
        private void HandleReceivePassFail(Operation o, Test test, ListBox logBox, List<OperationVariable> opvl,
                                       CanUtils cu_t, SerialUtils su_t, bool repeated)
        {
            var crr = new CommandRunner(o, tp.TestInterfaces, opvl, cu_t, su_t);
            crr.ReceivePassFail(prevSendOperation);
            res = crr.Pass;
            if (!res) Pass = false;

            LogReceiveResult(logBox, o, test, crr, repeated);
        }

        private void HandlePassFailDialog(Operation o, Test test, ListBox logBox, List<OperationVariable> opvl,
                                       CanUtils cu_t, SerialUtils su_t, bool repeated)
        {
            var crpf = new CommandRunner(o, tp.TestInterfaces);
            crpf.passFailDialog(test.Name);
            res = crpf.Pass;
            if (!res) Pass = false;

            LogReceiveResult(logBox, o, test, crpf, repeated);
        }

        private void HandleYesNoDialog(Operation o, Test test, ListBox logBox, List<OperationVariable> opvl,
                                       CanUtils cu_t, SerialUtils su_t, bool repeated)
        {
            var cryn = new CommandRunner(o);
            cryn.yesNoDialog(test.Name);
            res = cryn.Pass;
            if (!res) Pass = false;
            
            LogReceiveResult(logBox, o, test, cryn, repeated);
        }

        private void HandleBatch(Operation o, Test test, ListBox logBox, bool repeated)
        {
            var crpb = new CommandRunner(o);
            res = crpb.runBatch();
            if (!res) Pass = false;
            
            LogReceiveResult(logBox, o, test, crpb, repeated);
        }
        private void LogReceiveResult(ListBox logBox, Operation o, Test test, CommandRunner crr, bool repeated)
        {
            if (o.Log == null || !o.Log.Equals("false"))
            {
                // Safely parse logMess; default to 0 if null or invalid
                int nrMess = 0;
                if (!string.IsNullOrWhiteSpace(o.LogMess) && int.TryParse(o.LogMess, out int parsed))
                {
                    nrMess = parsed;
                }

                // Only remove previous entries if we're repeating AND there are messages to remove
                if (repeated && nrMess > 0)
                {
                    for (int j = nrMess; j >= 1; j--)
                    {
                        if (logBox.Items.Count > 0)
                            logBox.Items.RemoveAt(logBox.Items.Count - j);
                    }
                }

                logBox.Refresh();

                // Log multi-message results (e.g., receivePassFail with logMess > 0)
                if (nrMess > 0 && crr.CanMessages != null && crr.CanMessages.Count > 0)
                {
                    for (int i = 0; i < nrMess && i < crr.CanMessages.Count; i++)
                    {
                        string logEntry = i == 0
                            ? FormatLogEntry(test.Id, test.Name, CmdResult, ExtractMessageContent(crr.CanMessages[i]))
                            : FormatLogEntry("", "", "", ExtractMessageContent(crr.CanMessages[i]));
                        logBox.Items.Add(logEntry);
                    }
                }
                else
                {
                    // Log single-line result for operations without logMess (passFailDialog, yesNoDialog, batch)
                    // or for receivePassFail without logMess specified
                    AddLogEntry(logBox, test.Id, test.Name, CmdResult);
                }
            }
        }

        private string ExtractMessageContent(CanMessage msg)
        {
            string fullMsg = msg.ToString();
            return fullMsg.Length > 23 ? fullMsg.Substring(23) : fullMsg;
        }

        private string FormatLogEntry(string testId, string testName, string result, string message = "")
        {
            return message.Length > 0
                ? string.Format("{0,-3} {1,-40} {2,-8} {3,-30}", testId, testName, result, " [" + message + "]")
                : string.Format("{0,-3} {1,-40} {2,-8}", testId, testName, result);
        }

        private void AddLogEntry(ListBox logBox, string testId, string testName, string result)
        {
            logBox.Items.Add(FormatLogEntry(testId + ")", testName, result));
        }
    }
}
