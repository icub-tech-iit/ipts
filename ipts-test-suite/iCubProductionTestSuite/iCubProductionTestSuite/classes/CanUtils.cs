/*
 * Copyright (C) 2025 Istituto Italiano di Tecnologia
 * Authors: davide.tome@iit.it, jacopo.losi@iit.it
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

        private List<string> lastSentData = null;
        private int maxSendRetries = 2;
        private int maxReceiveRetries = 2;
        private int receiveTimeoutMs = 5000;

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

       
        public bool send(List<string> data)
        {
            lastSentData = new List<string>(data);
            int attempts = 0;
            bool sent = false;

            while (attempts < maxSendRetries && !sent)
            {
                try
                {
                    port.Open();
                    port.BitRate = new CanBitRate(CanBitRateTable.Cia1000KBit);

                    CanMessage cmsg = new CanMessage
                    {
                        Identifier = 0x001,
                        DataLength = Convert.ToByte(data.Count)
                    };
                    for (int i = 0; i < data.Count; i++)
                        cmsg[i] = Convert.ToByte(data[i]);

                    port.Send(ref cmsg);
                    sent = true;
                    Console.WriteLine("Sent CAN message: " + cmsg.ToString());
                }
                catch (InvalidOperationException ex)
                {
                    MessageBox.Show("Errore apertura CAN port: " + ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false; // No point retrying if the port can't be opened due to invalid state
                }
                catch (IOException)
                {
                    attempts++;
                    if (attempts >= maxSendRetries)
                    {
                        MessageBox.Show("Errore invio CAN dopo vari tentativi!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    System.Threading.Thread.Sleep(100); // Small delay before retry
                }
                finally
                {
                    if(port.IsOpen && port != null)
                    { 
                        port.Close();
                    }
                }
            }
            return sent;
        }

        public CanMessage receive(List<string> prev_data)
        {
            bool received = false;
            lastSentData = prev_data;
            if (lastSentData == null)
            {
                MessageBox.Show("Nessun messaggio CAN inviato da ritentare!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return default(CanMessage);
            }

            int attempts = 0;
            CanMessage cmsg = new CanMessage();

            while (attempts < maxReceiveRetries && !received)
            {
                try
                {
                    port.Open();
                    // Set receive timeout (ms)
                    port.ReceiveTimeout = receiveTimeoutMs;
                    // Set CAN bit rate to 1000 KBit/s (1 Mbit/s)
                    port.BitRate = new CanBitRate(CanBitRateTable.Cia1000KBit);
                    // Add CAN message filter for expected identifier (from ipts.xml)
                    port.AddToMessageFilter(CanMessageType.Data, messageId);

                    // Try to read a CAN message
                    if (port.Read(ref cmsg) >= 0)
                    {
                        received = true;
                        Console.WriteLine(cmsg.ToString());
                        lastSentData.Clear();
                        return cmsg;
                    }
                }
                catch (IOException)
                {
                    // Optionally handle port errors here
                    MessageBox.Show("Problemi nella ricezione dal CAN port. Ritento...", "Warning CAN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                    port.Close();
                }

                // If not received, resend the last message and try again
                Console.WriteLine("Message not yet received. Retrying...");
                send(lastSentData);
                attempts++;
            }

            MessageBox.Show("CAN timeout dopo vari tentativi di ricezione!", "Errore CAN", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return default(CanMessage);
        }
    }
}
