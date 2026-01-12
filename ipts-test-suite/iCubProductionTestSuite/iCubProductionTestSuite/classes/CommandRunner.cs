/*
* Copyright (C) 2025 Istituto Italiano di Tecnologia
* Authors: davide.tome@iit.it, jacopo.losi@iit.it
* CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
*/
using Esd.IO.Ntcan;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace iCubProductionTestSuite.classes
{
    class CommandRunner
    {
        private Operation operation;
        private List<TestInterface> testInterfaces;
        private List<OperationVariable> operationVariables;
        private bool pass = true;
        private CanUtils canUtils;
        private SerialUtils serialUtils;
        private List<CanMessage> canMessages;

        private static readonly ILog log = LogManager.GetLogger(typeof(CommandRunner));

        public bool Pass { get => pass; set => pass = value; }
        public List<CanMessage> CanMessages { get => canMessages; set => canMessages = value; }

        // CommandRunner used by TestRunner: receives current su_t instance so send/receive share the same SerialUtils
        public CommandRunner(Operation op, List<TestInterface> tis, List<OperationVariable> opvl, CanUtils cu_c, SerialUtils su_t)
        {
            this.testInterfaces = tis;
            this.operation = op;
            this.operationVariables = opvl;
            this.canMessages = new List<CanMessage>();
            this.canUtils = cu_c;
            this.serialUtils = su_t; // reuse the shared instance if provided
        }

        public CommandRunner(Operation op, List<TestInterface> tis)
        {
            this.testInterfaces = tis;
            this.operation = op;
        }

        public CommandRunner(Operation op)
        {
           this.operation = op;
        }

        /// <summary>
        /// Validates serial port with polling (waits for port to enumerate after flash).
        /// </summary>
        private TestInterface EnsureSerialPortValid(TestInterface ti)
        {
            if (ti == null) return ti;

            string desiredPort = ti.NetPort;
            if (string.IsNullOrWhiteSpace(desiredPort))
            {
                log.Warn("No serial port configured; will prompt user.");
                return ti;
            }

            // Poll for the desired port (gives OS time to enumerate after flash)
            const int totalWaitMs = 5000;
            const int pollIntervalMs = 200;
            var sw = System.Diagnostics.Stopwatch.StartNew();

            while (sw.ElapsedMilliseconds < totalWaitMs)
            {
                var ports = SerialPort.GetPortNames();
                if (ports.Contains(desiredPort, StringComparer.OrdinalIgnoreCase))
                {
                    log.InfoFormat("SERIAL port {0} is available", desiredPort);
                    return ti;
                }

                if (ports.Length > 0)
                {
                    log.WarnFormat("Port {0} different from the one available: {1}", desiredPort, string.Join(", ", ports));
                    break; // Port exists but not the one we want; let user choose
                }

                log.DebugFormat("Polling for port {0}... ({1}ms)", desiredPort, sw.ElapsedMilliseconds);
                System.Threading.Thread.Sleep(pollIntervalMs);
            }

            // After polling, re-enumerate and show user selection if port still not found
            var finalPorts = SerialPort.GetPortNames();
            if (finalPorts.Length == 0)
            {
                log.Warn("No serial ports found after polling. Prompting user for manual entry.");
                string manual = PromptUserForManualPort("No serial ports detected. Enter COM port (e.g., COM5):");
                if (!string.IsNullOrEmpty(manual))
                    ti.NetPort = manual.Trim();
                return ti;
            }

            if (!finalPorts.Contains(desiredPort, StringComparer.OrdinalIgnoreCase))
            {
                log.WarnFormat("Configured port {0} not available. Found: {1}", desiredPort, string.Join(", ", finalPorts));
                // Show selection dialog
                try
                {
                    var fi = new FormInput(ti);
                    if (fi.IsReady())
                    {
                        fi.ShowDialog();
                        if (!string.IsNullOrEmpty(fi.SelSERIAL))
                        {
                            ti.NetPort = fi.SelSERIAL;
                            log.InfoFormat("User selected port: {0}", ti.NetPort);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Warn("Error in port selection dialog", ex);
                }
            }

            return ti;
        }

        /// <summary>
        /// Simple dialog for user to manually enter a COM port.
        /// </summary>
        private string PromptUserForManualPort(string message)
        {
            var dlg = new Form
            {
                Text = "Serial Port",
                Width = 350,
                Height = 140,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false
            };

            var lbl = new Label { Left = 10, Top = 10, Width = 320, Text = message, AutoSize = false };
            var txt = new TextBox { Left = 10, Top = 35, Width = 320, Text = "COM5" };
            var ok = new Button { Text = "OK", Left = 160, Top = 65, Width = 80, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Cancel", Left = 250, Top = 65, Width = 80, DialogResult = DialogResult.Cancel };

            dlg.Controls.AddRange(new Control[] { lbl, txt, ok, cancel });
            dlg.AcceptButton = ok;
            dlg.CancelButton = cancel;

            return dlg.ShowDialog() == DialogResult.OK ? txt.Text.Trim() : null;
        }

        public bool Send()
        {
            var data = BuildCommandData();
            log.InfoFormat("Send operation with command: {0}", operation.Command);

            if (operation.Interf.Equals("CAN"))
                return SendViaCan(data);
            else if (operation.Interf.Equals("SERIAL"))
                return SendViaSerial(data);

            return false;
        }

        private List<string> BuildCommandData()
        {
            var data = new List<string> { operation.Command };
            if (operation.AppendVar != null)
            {
                foreach (var opVar in operationVariables)
                {
                    if (opVar.Name.Equals(operation.AppendVar))
                    {
                        data.Add(opVar.Value);
                        log.DebugFormat("Appending variable {0} with value {1}", opVar.Name, opVar.Value);
                    }
                }
            }
            return data;
        }

        private bool SendViaCan(List<string> data)
        {
            foreach (var ti in testInterfaces)
            {
                if (ti.Name.Equals("CAN"))
                {
                    canUtils = new CanUtils(ti);
                    return canUtils.send(data);
                }
            }
            return false;
        }

        private bool SendViaSerial(List<string> data)
        {
            foreach (var ti in testInterfaces)
            {
                if (ti.Name.Equals("SERIAL"))
                {
                    var validTi = EnsureSerialPortValid(ti);
                    return AttemptSerialSend(validTi, data);
                }
            }
            return false;
        }

        private bool AttemptSerialSend(TestInterface validTi, List<string> data)
        {
            try
            {
                ConfigureSerialUtils(validTi);
                serialUtils.send(data, true);
                log.InfoFormat("Serial send succeeded on port {0}", validTi.NetPort);
                return true;
            }
            catch (IOException ioEx)
            {
                log.WarnFormat("IO error on send: {0}. Re-enumerating...", ioEx.Message);
                
                // One retry: re-enumerate and check if port appeared
                System.Threading.Thread.Sleep(500);
                var portsNow = SerialPort.GetPortNames();
                
                if (portsNow.Contains(validTi.NetPort, StringComparer.OrdinalIgnoreCase))
                {
                    log.InfoFormat("Port {0} now available after retry. Attempting again...", validTi.NetPort);
                    try
                    {
                        // Dispose old, create fresh
                        try { serialUtils?.Dispose(); } catch { }
                        serialUtils = new SerialUtils(validTi);
                        serialUtils.send(data, true);
                        log.InfoFormat("Retry send succeeded on port {0}", validTi.NetPort);
                        return true;
                    }
                    catch (Exception retryEx)
                    {
                        log.Error("Retry send failed", retryEx);
                        MessageBox.Show(
                            $"Serial send failed on {validTi.NetPort} even after retry.\n\nError: {retryEx.Message}",
                            "Serial Port Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return false;
                    }
                }
                else
                {
                    log.Error("Port not found on retry either", ioEx);
                    MessageBox.Show(
                        $"Port {validTi.NetPort} does not exist.\n\nAvailable ports: {(portsNow.Length > 0 ? string.Join(", ", portsNow) : "None")}",
                        "Serial Port Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                return HandleSerialSendError(validTi, data, ex);
            }
        }

        private void ConfigureSerialUtils(TestInterface validTi)
        {
            if (serialUtils == null || serialUtils.Port == null)
            {
                serialUtils = new SerialUtils(validTi);
                log.DebugFormat("Created new SerialUtils for port {0}", validTi.NetPort);
            }
            else if (!string.Equals(serialUtils.Port.PortName, validTi.NetPort, StringComparison.OrdinalIgnoreCase))
            {
                if (!serialUtils.ReconfigurePort(validTi))
                {
                    serialUtils?.Dispose();
                    serialUtils = new SerialUtils(validTi);
                }
            }
        }

        private bool HandleSerialSendError(TestInterface validTi, List<string> data, Exception ex)
        {
            log.Warn("Serial send failed. Attempting interactive port selection.", ex);
            var fi = new FormInput(validTi);
            if (!fi.IsReady()) return false;

            fi.ShowDialog();
            if (string.IsNullOrEmpty(fi.SelSERIAL)) return false;

            return RetrySerialSendWithNewPort(validTi, data, fi.SelSERIAL);
        }

        private bool RetrySerialSendWithNewPort(TestInterface validTi, List<string> data, string newPort)
        {
            try
            {
                validTi.NetPort = newPort;
                ConfigureSerialUtils(validTi);
                serialUtils.send(data, true);
                log.InfoFormat("Serial send succeeded after user override on port {0}", newPort);
                return true;
            }
            catch (Exception ex)
            {
                log.Error("Retry serial send failed", ex);
                MessageBox.Show($"Serial send failed after manual port selection.\n\nError: {ex.Message}", 
                    "Errore Comunicazione Seriale", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public void ReceivePassFail(Operation prevSend)
        {
            List<String> prev_data = new List<string>();
            prev_data.Add(prevSend.Command);
            log.DebugFormat("Adding to prev_data the prevSend.Command {0}", prevSend.Command);
            
            // Add any appended variables from the previous operation
            if (prevSend.AppendVar != null)
            {
                foreach (OperationVariable o in operationVariables)
                {
                    if (o.Name.Equals(prevSend.AppendVar))
                    {
                        log.DebugFormat("Appending variable {0} with value {1} from appendVar {2}", o.Name, o.Value, prevSend.AppendVar);
                        prev_data.Add(o.Value);
                    }
                }
            }

            if (testInterfaces == null || testInterfaces.Count == 0) { Pass = false; return; }

            int nrMess = Convert.ToInt16(operation.LogMess);

            switch (operation.Interf)
            {
                case "CAN":
                    foreach (TestInterface ti in testInterfaces)
                    {
                        if (ti.Name.Equals("CAN"))
                        {
                            canUtils = new CanUtils(ti);
                            if (nrMess > 0) for (int i = 0; i < nrMess; i++) canMessages.Add(canUtils.receive(prev_data));
                            else canMessages.Add(canUtils.receive(prev_data));
                            string[] vpl = operation.ValPass.Split(' ');
                            for (int i = 0; i < vpl.Length; i++)
                            {
                                int b = canMessages[0][i];
                                int value = Convert.ToInt32(vpl[i], 16);
                                if (!b.Equals(value)) Pass = false;
                            }
                        }
                    }
                    break;
                case "SERIAL":
                    foreach (TestInterface ti in testInterfaces)
                    {
                        if (ti.Name.Equals("SERIAL"))
                        {
                            // === LAZY PORT VALIDATION ===
                            TestInterface validTi = EnsureSerialPortValid(ti);

                            bool createdHere = false;
                            try
                            {
                                // Reuse shared instance when available and configured for same port
                                if (serialUtils == null || serialUtils.Port == null)
                                {
                                    serialUtils = new SerialUtils(validTi);
                                    createdHere = true;
                                    log.DebugFormat("Created new SerialUtils for receive on port {0}", validTi.NetPort);
                                }
                                else if (!string.Equals(serialUtils.Port.PortName, validTi.NetPort, StringComparison.OrdinalIgnoreCase))
                                {
                                    // different port: try reconfigure or recreate
                                    if (!serialUtils.ReconfigurePort(validTi))
                                    {
                                        try { serialUtils.Dispose(); } catch { }
                                        serialUtils = new SerialUtils(validTi);
                                        createdHere = true;
                                    }
                                }

                                // Wait synchronously for the reply (keep port open)
                                string msg = serialUtils.ReceiveSync(2000, true);

                                // compare normalized values
                                string expected = operation.ValPass ?? string.Empty;

                                if (!CompareSerialReply(serialUtils, msg, expected))
                                {
                                    Pass = false;
                                    log.WarnFormat("Serial reply mismatch. Expected: {0}, Got: {1}", expected, msg);
                                }
                                else
                                {
                                    log.InfoFormat("Serial receive succeeded on port {0}", validTi.NetPort);
                                }

                                // If we created a local SerialUtils here, dispose it now
                                if (createdHere)
                                {
                                    try { serialUtils.Dispose(); } catch { }
                                    serialUtils = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                Pass = false;
                                log.Error("Error in serial receivePassFail", ex);
                                MessageBox.Show("Errore nella ricezione seriale: " + ex.Message, "Errore",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    break;
                default: break;
            }
        }

        // Normalizes textual replies (collapse whitespace and trim)
        private static string NormalizeText(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return Regex.Replace(s, @"\s+", " ").Trim();
        }

        private static bool IsHexString(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var tokens = s.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var t in tokens)
            {
                var tok = t.Trim();
                if (tok.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    if (tok.Length != 4) return false;
                    if (!IsHexByte(tok.Substring(2))) return false;
                }
                else
                {
                    if (tok.Length != 2 || !IsHexByte(tok)) return false;
                }
            }
            return true;
        }

        private static bool IsHexByte(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                bool ok = (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
                if (!ok) return false;
            }
            return true;
        }

        private static byte[] ParseHexString(string s)
        {
            var tokens = s.Trim().Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var list = new List<byte>();
            foreach (var t in tokens)
            {
                var tok = t.Trim();
                string hex = tok.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? tok.Substring(2) : tok;
                list.Add(Convert.ToByte(hex, 16));
            }
            return list.ToArray();
        }

        // Compare reply with expected. Supports textual and hex-style comparisons.
        private static bool CompareSerialReply(SerialUtils su, string msg, string expected)
        {
            msg = msg ?? string.Empty;
            expected = expected ?? string.Empty;

            if (IsHexString(expected))
            {
                var expectedBytes = ParseHexString(expected);

                if (IsHexString(msg))
                {
                    var msgBytes = ParseHexString(msg);
                    return msgBytes.SequenceEqual(expectedBytes);
                }

                Encoding enc = Encoding.ASCII;
                try
                {
                    if (su?.Port != null) enc = su.Port.Encoding ?? Encoding.ASCII;
                }
                catch { enc = Encoding.ASCII; }

                var msgRaw = enc.GetBytes(msg);
                return msgRaw.SequenceEqual(expectedBytes);
            }
            else
            {
                var nMsg = NormalizeText(msg);
                var nExpected = NormalizeText(expected);
                return string.Equals(nMsg, nExpected, StringComparison.Ordinal);
            }
        }

        public void passFailDialog(String test)
        {
            DialogResult dialogResult = MessageBox.Show(operation.Text, test, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.No) Pass = false;
            else Pass = true;
        }

        public void yesNoDialog(String test)
        {
            FormDialog dialog = new FormDialog(operation.Text, operation.Image, true);
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

            processInfo = new ProcessStartInfo("cmd.exe", "/c" + " \"" + p + "\\" + operation.BatchDir + "\\" + operation.BatchFile + "\"");
            processInfo.WorkingDirectory = @"" + p + "\\" + operation.BatchDir;
            processInfo.CreateNoWindow = false;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = false;
            processInfo.RedirectStandardOutput = false;

            process = Process.Start(processInfo);
            process.WaitForExit();
            
            exitCode = process.ExitCode;
            log.DebugFormat("ExitCode: {0}", exitCode.ToString(), "ExecuteCommand");
            process.Close();

            if (exitCode > 0) return false;
            else return true;
        }
    }
}
