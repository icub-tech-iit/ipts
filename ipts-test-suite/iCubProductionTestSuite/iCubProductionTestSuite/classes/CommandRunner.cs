/*
* Copyright (C) 2025 Istituto Italiano di Tecnologia
* Authors: davide.tome@iit.it, jacopo.losi@iit.it
* CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Esd.IO.Ntcan;


namespace iCubProductionTestSuite.classes
{
    class CommandRunner
    {
        private Operation op;
        private List<TestInterface> tis;
        private List<OperationVariable> opvl;
        private bool pass = true;
        private CanUtils cu;
        private SerialUtils su;
        private List<CanMessage> cmsg;
        

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

        public List<CanMessage> Cmsg
        {
            get
            {
                return cmsg;
            }

            set
            {
                cmsg = value;
            }
        }

        public CommandRunner(Operation op, List<TestInterface> tis, List<OperationVariable> opvl, CanUtils cu_c, SerialUtils su_t)
        {
            this.tis = tis;
            this.op = op;
            this.opvl = opvl;
            this.cmsg = new List<CanMessage>();
            this.cu = cu_c;
            this.su = su_t;
        }

        public CommandRunner(Operation op, List<TestInterface> tis)
        {
            this.tis = tis;
            this.op = op;
        }

        public CommandRunner(Operation op)
        {
           this.op = op;
        }

        public bool send()
        {
            List<String> data = new List<string>();
            data.Add(op.Command);
            Console.WriteLine("Send operation with op.Command {0}", op.Command);
            if (op.AppendVar != null)
            {
                foreach (OperationVariable o in opvl)
                    if (o.Name.Equals(op.AppendVar)) 
                    {
                        data.Add(o.Value);
                        Console.WriteLine("Appending variable {0} with value {1} from appendVar {2}", o.Name, o.Value, op.AppendVar);
                    }
            }

            bool sent = false;
            if (op.Interf.Equals("CAN"))
            {
                foreach (TestInterface ti in tis)
                {
                    if (ti.Name.Equals("CAN"))
                    {
                        cu = new CanUtils(ti);
                        sent = cu.send(data);
                    }
                }
            }
            else if (op.Interf.Equals("SERIAL"))
            {
                foreach (TestInterface ti in tis)
                {
                    if (ti.Name.Equals("SERIAL"))
                    {
                        su = new SerialUtils(ti);
                        su.send(data);
                        sent = true;
                    }
                }
            }
            return sent;
        }

        public void receivePassFail(Operation prev_send)
        {
            List<String> prev_data = new List<string>();
            prev_data.Add(prev_send.Command);
            Console.WriteLine("Adding to prev_data the prev_send.Command {0}", prev_send.Command);
            
            // Add any appended variables from the previous operation
            if (prev_send.AppendVar != null)
            {
                foreach (OperationVariable o in opvl)
                {
                    if (o.Name.Equals(prev_send.AppendVar))
                    {
                        Console.WriteLine("Appending variable {0} with value {1} from appendVar {2}", o.Name, o.Value, prev_send.AppendVar);
                        prev_data.Add(o.Value);
                    }
                }
            }


            if (tis.Count.Equals(0)) { Pass = false; return; }

            int nrMess = Convert.ToInt16(op.LogMess);

            switch (op.Interf)
            {
                case "CAN":
                    foreach (TestInterface ti in tis)
                    {
                        if (ti.Name.Equals("CAN"))
                        {
                            cu = new CanUtils(ti);
                            // Try to receive, with resend if needed (handled in cu.receive)
                            if (nrMess > 0) for (int i = 0; i < nrMess; i++) Cmsg.Add(cu.receive(prev_data));
                            else Cmsg.Add(cu.receive(prev_data));
                            string[] vpl = op.ValPass.Split(' ');
                            for (int i = 0; i < vpl.Length; i++)
                            {
                                int b = Cmsg[0][i];
                                int value = Convert.ToInt32(vpl[i], 16);
                                if (!b.Equals(value)) Pass = false;
                            }
                        }
                    }
                    break;
                case "SERIAL":
                    foreach (TestInterface ti in tis)
                    {
                        if (ti.Name.Equals("SERIAL"))
                        {
                            su = new SerialUtils(ti);
                            string msg = su.receive();
                            if (!msg.Equals(op.ValPass)) Pass = false;
                        }
                    }
                    break;
                default: break;
            }
        }

        public void passFailDialog(String test)
        {
            DialogResult dialogResult = MessageBox.Show(op.Text, test, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No) Pass = false;
            else Pass = true;
            
        }

        public void yesNoDialog(String test)
        {
            FormDialog dialog = new FormDialog(op.Text, op.Image, true);
            dialog.ShowDialog();
            if (!dialog.Yn) Pass = false;
            else Pass = true;

        }

        public bool runBatch()
        {
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;
            string p = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            processInfo = new ProcessStartInfo("cmd.exe", "/c" + " \"" + p + "\\" + op.BatchDir + "\\" + op.BatchFile + "\"");
            processInfo.WorkingDirectory = @"" + p + "\\" + op.BatchDir;
            processInfo.CreateNoWindow = false;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = false;
            processInfo.RedirectStandardOutput = false;

            process = Process.Start(processInfo);
            process.WaitForExit();
            
            exitCode = process.ExitCode;
            Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();

            if (exitCode > 0) return false;
            else return true;
            
        }
    }
}
