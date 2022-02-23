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
    public class AppException : Exception
    {
        public AppException(String message) : base(message)
        { }

        public AppException(String message, Exception inner) : base(message, inner) { }
    }
}
