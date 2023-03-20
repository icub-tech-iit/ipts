 /*
 * Copyright (C) 2018 Istituto Italiano di Tecnologia
 * Authors: davide.tome@iit.it
 * CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Esd.IO.Ntcan;
using System.IO;
using System.Windows.Forms;
using System.IO.Ports;

namespace iCubProductionTestSuite.classes
{

    public class SerialUtils
    {
        private List<String> ports;
        private SerialPort port;
        private int messageId;

        public SerialUtils() { }

        // Get list of available Serial ports
        private List<String> getPorts() 
        {
            ports = new List<string>();

            foreach(String p in SerialPort.GetPortNames())
            {
                ports.Add(p);
            }
            return ports;

        }

      

      
        public List<String> Ports
        {
            get
            {
                ports = getPorts();
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

   
            port.Close();
        }

        public String receive()
        {
            int timeout = 0;
            String msg = "";

            // Open the Serial port for communication and catch error
            try
            {
                port.Open();
            }
            catch (IOException)
            {
                MessageBox.Show("Attenzione nessuna interfaccia Seriale presente!", "Errore",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

        
            
            port.Close();
            return msg;
        }
    }   
}
