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


namespace iCubProductionTestSuite.classes
{

    public class CanUtils
    {
        private List<String> ports;
        private CanPort port;
        private int messageId;

        public CanUtils() { }

        public CanUtils(TestInterface ti)
        {
            // Create a new CAN port instance
            this.port = new CanPort(Convert.ToInt16(ti.NetPort));           
            this.messageId = Convert.ToInt32(ti.MessageID, 16);
        }

        // Get list of available CAN ports
        private List<String> getPorts() 
        {
            ports = new List<string>();

            foreach(CanPortInfo  p in CanPortInfo.Ports)
            {
                ports.Add(p.NetNo.ToString());
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

        public CanPort Port
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
           
            // Open the CAN port for communication and catch error
            try
            {
                port.Open();
            }
            catch (IOException)
            {
                MessageBox.Show("Attenzione nessuna interfaccia CAN presente!", "Errore",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Configure the bit rate to 500 KBit/s
            port.BitRate = new CanBitRate(CanBitRateTable.Cia1000KBit);

            //  Single CAN message on the stack
            CanMessage cmsg = new CanMessage();

            // Initialize the CAN message with properties and the indexer as
            // data message with 8 bytes in standard frame format (11 bit identifier)
            cmsg.Identifier = 0x001;
            cmsg.DataLength = Convert.ToByte(data.Count);

            //for(int i = 0; i < data.Length; i++) cmsg[i] = Convert.ToByte(data[i]);

            //int value = Convert.ToInt32(data, 16);
            //data[0] = data[0].Substring(2, 2);
            for(int i=0; i<data.Count; i++)
            {
                cmsg[i] = Convert.ToByte(data[i]);
            }



            // Transmit this message without blocking.
            port.Send(ref cmsg);

            // Close the port        
            port.Close();
        }

        public CanMessage receive()
        {
            int timeout = 0;

            // Open the CAN port for communication and catch error
            try
            {
                port.Open();
            }
            catch (IOException)
            {
                MessageBox.Show("Attenzione nessuna interfaccia CAN presente!", "Errore",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            // Configure timeout
            port.ReceiveTimeout = 3000;
         
            // Set the bit rate to 1000 KBit/s
            port.BitRate = new CanBitRate(CanBitRateTable.Cia1000KBit);

            // Enable CAN identifier to receive
           // for (int id = 0; id < 2048; id++)
            port.AddToMessageFilter(CanMessageType.Data, messageId);

            // Define array for CAN messages and length
            CanMessage cmsg = new CanMessage();

            // Stay in loop until a CAN message is received.
            while (true)
            {
                if (port.Read(ref cmsg) < 0)
                {
                    Console.WriteLine("Reading CAN data timed out");
                    timeout++;
                    if (timeout > 8)
                    {
                        MessageBox.Show("CAN timeout!", "Errore CAN",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        timeout = 0;
                        break;
                    }else continue;
                }
               
                Console.WriteLine(cmsg.ToString());
                break;
            }
            
            port.Close();
            return cmsg;
        }
    }   
}
