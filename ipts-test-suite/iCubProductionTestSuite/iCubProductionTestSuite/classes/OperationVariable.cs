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
    class OperationVariable
    {
        private String name;
        private String value;

        public OperationVariable(String name, String value, String type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }

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

        public string Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public string Type { get; private set; }
    }
}
