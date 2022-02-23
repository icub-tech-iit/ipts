/*
* Copyright (C) 2018 Istituto Italiano di Tecnologia
* Authors: davide.tome@iit.it
* CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private CanMessage cmsg;
        

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

        public CanMessage Cmsg
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

        public CommandRunner(Operation op, List<TestInterface> tis, List<OperationVariable> opvl)
        {
            this.tis = tis;
            this.op = op;
            this.opvl = opvl;
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

        public void send()
        {
            List<String> data = new List<string>();

            data.Add(op.Command);

            if(op.AppendVar !=  null)
            {
                foreach(OperationVariable o in opvl)
                    if(o.Name.Equals(op.AppendVar)) data.Add(o.Value);
            }

            foreach (TestInterface ti in tis)
            {
                switch(ti.Name)
                {
                    case "CAN":
                        cu = new CanUtils(ti);
                        cu.send(data);
                        break;
                    default:
                        break;
                }
            }
        }

        public void receivePassFail()
        {
            if (tis.Count.Equals(0)) { Pass = false; return; }

            foreach (TestInterface ti in tis)
            {
                switch (ti.Name)
                {
                    case "CAN":
                        cu = new CanUtils(ti);
                        // Byte c = cu.receive();
                         Cmsg = cu.receive();
                        //cu.receive();
                        string[] vpl = op.ValPass.Split(' ');
                        for(int i = 0; i < vpl.Length; i++)
                        {
                            int b = Cmsg[i];
                            int value = Convert.ToInt32(vpl[i], 16);
                            if (!b.Equals(value)) Pass = false;
                           
                        }                        
                        break;
                   
                    default:
                        break;
                }
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
