 /*
 * Copyright (C) 2025 Istituto Italiano di Tecnologia
 * Authors: davide.tome@iit.it, jacopo.losi@iit.it
 * CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
 */

using Esd.IO.Ntcan;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;
using log4net;

namespace iCubProductionTestSuite.classes
{

    public class SerialUtils
    {
        private List<String> ports;
        private SerialPort port;
        private int messageId;
        private String serialRx;
        private static string serialRX;

        private static readonly ILog log = LogManager.GetLogger(typeof(SerialUtils));

        public SerialUtils() {
            this.ports = new List<string>();
            this.port = new SerialPort();
          //  this.ports = getPorts();
        }

        public SerialUtils(TestInterface ti) {
            this.ports = new List<string>();

            this.port = new SerialPort(ti.Name);

        //    this.ports = getPorts();
            port.PortName = ti.NetPort;
            port.BaudRate = 9600;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.DataBits = 8;
            port.Handshake = Handshake.None;
            port.RtsEnable = true;

            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        // Get list of available Serial ports
        private List<String> getPorts() 
        {
            string[] serialPorts;
            
            serialPorts = SerialPort.GetPortNames();
            foreach(String p in serialPorts)
            {
                ports.Add(p);
               
                    log.Info(p);
            }
            return ports;

        }


        private static void DataReceivedHandler(
                    object sender,
                    SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            serialRX = indata;
            
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

       
        public void send(List<String> data)
        {
           
            // Open the Serial port for communication and catch error
            try
            {
                port.Open();
            }
            catch (IOException)
            {
                MessageBox.Show("Attenzione nessuna interfaccia Seriale presente!", "Errore",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Configure the bit rate to 500 KBit/s
            port.BaudRate = 9600;

            // Open the Serial port for communication and catch error
            if (!port.IsOpen)
            {
                MessageBox.Show("Attenzione nessuna interfaccia Seriale presente!", "Errore",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ;
            }

            port.Write(data[0]);

            // Open the Serial port for communication and catch error
            if (!port.IsOpen)
            {
                MessageBox.Show("Attenzione nessuna interfaccia Seriale presente!", "Errore",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                return ;
            }

            //          port.Close();
        }

        public String receive()
        {
           
            return serialRX;
        }
    }   
}
