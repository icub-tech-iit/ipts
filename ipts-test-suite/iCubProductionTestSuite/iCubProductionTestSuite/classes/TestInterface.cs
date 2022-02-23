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

namespace iCubProductionTestSuite.classes
{
    public class TestInterface
    {
        private String name;
        private String netPort;
        private List<String> bitrates = new List<String>();
        private String bitrate;
        private String messageID;
        

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Bitrate
        {
            get
            {
                return bitrate;
            }

            set
            {
                bitrate = value;
            }
        }


        public List<string> Bitrates
        {
            get
            {
                return bitrates;
            }

            set
            {
                bitrates = value;
            }
        }

        public string NetPort
        {
            get
            {
                return netPort;
            }

            set
            {
                netPort = value;
            }
        }

        public string MessageID
        {
            get
            {
                return messageID;
            }

            set
            {
                messageID = value;
            }
        }
    }
}
