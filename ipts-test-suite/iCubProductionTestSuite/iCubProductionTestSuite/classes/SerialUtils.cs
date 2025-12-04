/*
 * Copyright (C) 2025 Istituto Italiano di Tecnologia
 * Authors: davide.tome@iit.it, jacopo.losi@iit.it
 * CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
 */

using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace iCubProductionTestSuite.classes
{
    public class SerialUtils : IDisposable
    {
        private List<string> ports;
        private SerialPort port;
        private int messageId;
        private readonly StringBuilder rxBuffer = new StringBuilder();
        private readonly object rxLock = new object();

        private bool disposed = false;

        private static readonly ILog log = LogManager.GetLogger(typeof(SerialUtils));

        public SerialUtils()
        {
            this.ports = new List<string>();
            this.port = new SerialPort();
        }

        public SerialUtils(TestInterface ti)
        {
            this.ports = new List<string>();

            string portName = ti?.NetPort ?? string.Empty;

            // Use sensible defaults if port is empty
            if (string.IsNullOrWhiteSpace(portName))
            {
                portName = "COM1"; // Default fallback port
            }

            this.port = new SerialPort(portName)
            {
                BaudRate = ParseBaud(ti?.Bitrate) ?? 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true,
                Encoding = Encoding.ASCII
            };

            this.port.ReadTimeout = 100;
            this.port.WriteTimeout = 1000;
            this.port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        private int? ParseBaud(string b)
        {
            if (string.IsNullOrWhiteSpace(b)) return null;
            if (int.TryParse(b, out int v)) return v;
            return null;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string indata = sp.ReadExisting();
                if (string.IsNullOrEmpty(indata)) return;

                lock (rxLock)
                {
                    rxBuffer.Append(indata);
                }
            }
            catch (Exception ex)
            {
                log.Warn("Error in DataReceivedHandler", ex);
            }
        }

        public List<String> Ports
        {
            get
            {
                return ports;
            }

        }

        public SerialPort Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        // Send and optionally keep the port open.
        public void send(List<string> data, bool leaveOpen = false)
        {
            if (data == null || data.Count == 0)
            {
                log.Warn("send called with empty data");
                return;
            }

            bool openedHere = false;
            try
            {
                if (!port.IsOpen)
                {
                    // configure before opening (redundant but safe)
                    port.BaudRate = port.BaudRate; // keep current
                    port.Parity = port.Parity;
                    port.StopBits = port.StopBits;
                    port.DataBits = port.DataBits;
                    port.Handshake = port.Handshake;
                    port.RtsEnable = port.RtsEnable;

                    port.Open();
                    openedHere = true;
                }

                // Clear any previous buffered data before sending a new command
                lock (rxLock)
                {
                    rxBuffer.Clear();
                }

                // Now write (first entry contains the command)
                port.Write(data[0]);
            }
            catch (UnauthorizedAccessException ex)
            {
                log.Error("Access denied to serial port", ex);
                MessageBox.Show("Attenzione nessuna interfaccia Seriale presente!", "Errore",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                log.Error("I/O error while writing to serial port", ex);
                MessageBox.Show("Errore di I/O sulla porta seriale.", "Errore",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (InvalidOperationException ex)
            {
                log.Error("Invalid operation on serial port", ex);
                MessageBox.Show("Operazione non valida sulla porta seriale.", "Errore",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (openedHere && !leaveOpen)
                {
                    try
                    {
                        port.Close();
                    }
                    catch (Exception ex)
                    {
                        log.Warn("Failed to close serial port", ex);
                    }
                }
            }
        }

        // Synchronous receive with timeout (ms). Returns trimmed string on success, empty on timeout/error.
        // This implementation accumulates incoming fragments and waits a short stabilisation delay
        // to reduce the chance of returning a partial chunk.
        public string ReceiveSync(int timeoutMs = 2000, bool leaveOpen = false)
        {
            bool openedHere = false;
            try
            {
                if (!port.IsOpen)
                {
                    port.Open();
                    openedHere = true;
                }

                var sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < timeoutMs)
                {
                    string s;
                    lock (rxLock)
                    {
                        s = rxBuffer.Length > 0 ? rxBuffer.ToString() : string.Empty;
                    }

                    if (!string.IsNullOrEmpty(s))
                    {
                        // small delay to allow fragmented packets to arrive
                        Thread.Sleep(20);

                        lock (rxLock)
                        {
                            s = rxBuffer.ToString();
                            rxBuffer.Clear();
                        }

                        // normalize and return
                        return s?.Trim() ?? string.Empty;
                    }

                    // small delay to avoid busy loop
                    Thread.Sleep(10);
                }

                // timeout
                return string.Empty;
            }
            catch (Exception ex)
            {
                log.Warn("Serial receive failed", ex);
                return string.Empty;
            }
            finally
            {
                if (openedHere && !leaveOpen)
                {
                    try
                    {
                        port.Close();
                    }
                    catch (Exception ex)
                    {
                        log.Warn("Failed to close serial port after receive", ex);
                    }
                }
            }
        }

        // legacy/event-driven getter; returns current buffer content without clearing
        public String receive()
        {
            lock (rxLock)
            {
                return rxBuffer.ToString() ?? string.Empty;
            }
        }

        public void Close()
        {
            try
            {
                if (port != null && port.IsOpen)
                    port.Close();
            }
            catch (Exception ex)
            {
                log.Warn("Error closing serial port", ex);
            }
        }

        // Add method to reconfigure port at runtime
        public bool ReconfigurePort(TestInterface ti)
        {
            if (ti == null || string.IsNullOrWhiteSpace(ti.NetPort))
                return false;

            try
            {
                // Close existing connection if open
                if (port.IsOpen)
                {
                    port.Close();
                }

                port.PortName = ti.NetPort;
                port.BaudRate = ParseBaud(ti.Bitrate) ?? 9600;
                
                log.InfoFormat("SerialUtils port reconfigured to {0} @ {1} baud", ti.NetPort, port.BaudRate);
                return true;
            }
            catch (Exception ex)
            {
                log.Warn("Failed to reconfigure serial port", ex);
                return false;
            }
        }

        // Add property to check if port is properly configured
        public bool IsPortConfigured
        {
            get
            {
                return !string.IsNullOrWhiteSpace(port?.PortName) && port.PortName != "COM1";
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            try
            {
                if (port != null)
                {
                    port.DataReceived -= DataReceivedHandler;
                    if (port.IsOpen) port.Close();
                    port.Dispose();
                }
            }
            catch (Exception ex)
            {
                log.Warn("Error disposing SerialUtils", ex);
            }

            GC.SuppressFinalize(this);
        }
    }
}
